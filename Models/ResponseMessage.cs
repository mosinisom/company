using System.Text.Json.Serialization;

namespace Backend.Models
{
  public class ResponseMessage
  {
    [JsonPropertyName("action")]
    public string Action { get; set; }

    [JsonPropertyName("data")]
    public object Data { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }
  }
}