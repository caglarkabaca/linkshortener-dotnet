using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinkShortenerAPI.Models.DTO;

public class ErrorResponseDTO
{
    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}