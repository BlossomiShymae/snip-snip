using System.Text.Json.Serialization;

namespace snip_snip.Models
{
    public class CommunityDragonFileInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = default!;
        [JsonPropertyName("type")]
        public string Type { get; init; } = default!;
        [JsonPropertyName("mtime")]
        public string MTime { get; init; } = default!;
        [JsonPropertyName("size")]
        public long? Size { get; init; }
    }
}