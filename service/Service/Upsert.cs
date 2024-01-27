using Microsoft.KernelMemory;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
namespace Microsoft.KernelMemory.Service
{
    public class TagCollectionJsonConverter : JsonConverter<TagCollection>
    {
        public override TagCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var collection = new TagCollection();

            // Assuming JSON structure as: {"key1": ["value1", "value2"], "key2": ["value3"]}
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return collection;

                // Get the key
                string key = reader.GetString();
                reader.Read();

                // Get the array of values
                var values = JsonSerializer.Deserialize<List<string?>>(ref reader, options);
                collection.Add(key, values);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, TagCollection value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var pair in value)
            {
                writer.WritePropertyName(pair.Key);
                JsonSerializer.Serialize(writer, pair.Value, options);
            }

            writer.WriteEndObject();
        }
    }

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
        [JsonConverter(typeof(TagCollectionJsonConverter))] // Apply the custom converter
        public TagCollection Tags { get; set; } = new TagCollection();
    }
}