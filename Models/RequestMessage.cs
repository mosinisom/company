using System.Text.Json;
using System.Text.Json.Serialization;
namespace Backend.Models;

public class RequestMessage
{
  [JsonPropertyName("action")]
  public string Action { get; set; }
  public int? Id { get; set; }
  public int? DepartmentId { get; set; }
  public int? EmployeeId { get; set; }

  [JsonPropertyName("data")]
  public JsonElement Data { get; set; }
}