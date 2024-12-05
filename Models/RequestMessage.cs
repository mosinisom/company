using System.Text.Json;
using System.Text.Json.Serialization;
namespace Backend.Models;

public class RequestMessage
{
  [JsonPropertyName("action")]
  public string Action { get; set; }
  [JsonPropertyName("id")]
  public int? Id { get; set; }
  [JsonPropertyName("departmentId")]
  public int? DepartmentId { get; set; }
  [JsonPropertyName("employeeId")]
  public int? EmployeeId { get; set; }

  [JsonPropertyName("data")]
  public JsonElement Data { get; set; }
}