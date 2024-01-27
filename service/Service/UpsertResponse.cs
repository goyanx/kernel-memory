using Microsoft.KernelMemory;

using System.Text.Json.Serialization;

namespace Microsoft.KernelMemory.Service;

public class UpsertResponse
{
    [JsonPropertyName("index")]
    [JsonPropertyOrder(0)]
    public string Index { get; set; } = string.Empty;

    [JsonPropertyName("documentId")]
    [JsonPropertyOrder(1)]
    public string DocumentId { get; set; } = string.Empty;


}
