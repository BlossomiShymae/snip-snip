using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlossomiShymae.SnipSnip.Core
{
    public record FileInfo
    {
        [JsonPropertyName("name")]
        public required string Name { get; init; } 
        [JsonPropertyName("type")]
        public required string Type { get; init; } 
        [JsonPropertyName("mtime")]
        public required string MTime { get; init; } 
        [JsonPropertyName("size")]
        public long? Size { get; init; }
    }
}