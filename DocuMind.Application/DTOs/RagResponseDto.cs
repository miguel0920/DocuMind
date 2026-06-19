using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DocuMind.Application.DTOs
{
    public class RagResponseDto
    {
        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public List<string> Sources { get; set; } = [];
    }
}