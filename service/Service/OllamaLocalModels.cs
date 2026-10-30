using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;

namespace Microsoft.KernelMemory.Service;

internal sealed class OllamaLocalConfig
{
    public string Endpoint { get; set; } = "http://127.0.0.1:11434";
    public string EmbeddingModel { get; set; } = "nomic-embed-text:latest";
    public string TextModel { get; set; } = "redule26/huihui_ai_qwen2.5-vl-7b-abliterated:latest";
}

internal sealed class OllamaEmbeddingGenerator : ITextEmbeddingGenerator
{
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromMinutes(2) };
    private readonly OllamaLocalConfig _config;

    public OllamaEmbeddingGenerator(OllamaLocalConfig config) => this._config = config;

    public int MaxTokens => 2048;

    public int CountTokens(string text) => Math.Max(1, (text.Length + 3) / 4);

    public async Task<Embedding> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        using var response = await Client.PostAsJsonAsync(
            $"{this._config.Endpoint.TrimEnd('/')}/api/embed",
            new { model = this._config.EmbeddingModel, input = text }, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (result?.Embeddings is not { Length: > 0 } || result.Embeddings[0].Length == 0)
        {
            throw new InvalidOperationException("Ollama returned no embedding vector");
        }

        return new Embedding(result.Embeddings[0]);
    }

    private sealed class OllamaEmbeddingResponse
    {
        public float[][] Embeddings { get; set; } = Array.Empty<float[]>();
    }
}

internal sealed class OllamaTextGenerator : ITextGenerator
{
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromMinutes(3) };
    private readonly OllamaLocalConfig _config;

    public OllamaTextGenerator(OllamaLocalConfig config) => this._config = config;

    public int MaxTokenTotal => 8192;

    public int CountTokens(string text) => Math.Max(1, (text.Length + 3) / 4);

    public async IAsyncEnumerable<string> GenerateTextAsync(
        string prompt,
        TextGenerationOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var response = await Client.PostAsJsonAsync(
            $"{this._config.Endpoint.TrimEnd('/')}/api/chat",
            new
            {
                model = this._config.TextModel,
                messages = new[] { new { role = "user", content = prompt } },
                stream = false,
                think = false,
                options = new { num_predict = options.MaxTokens ?? 300, temperature = options.Temperature }
            }, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrEmpty(result?.Message?.Content))
        {
            yield return result.Message.Content;
        }
    }

    private sealed class OllamaChatResponse
    {
        public OllamaChatMessage? Message { get; set; }
    }

    private sealed class OllamaChatMessage
    {
        public string Content { get; set; } = string.Empty;
    }
}
