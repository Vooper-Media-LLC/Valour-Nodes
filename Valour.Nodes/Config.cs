using System.Text.Json.Serialization;

namespace Valour.Nodes;

public class Config
{
    [JsonInclude]
    [JsonPropertyName("apiKey")]
    public string ApiKey { get; set; }
}
