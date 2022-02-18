using System.Text.Json.Serialization;

namespace Valour.Nodes;

public class Config
{
    [JsonInclude]
    [JsonPropertyName("api_key")]
    public string API_Key { get; set; }
}
