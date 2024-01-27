
using System.Text.Json.Serialization;

namespace Microsoft.KernelMemory;

public class Upsert
{
    [JsonPropertyName("index")]
    [JsonPropertyOrder(0)]
    public string Index { get; set; } = string.Empty;

    [JsonPropertyName("documentId")]
    [JsonPropertyOrder(1)]
    public string DocumentId { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    [JsonPropertyOrder(2)]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    [JsonPropertyOrder(10)]
    public TagCollection Tags { get; set; } = new TagCollection();



}
