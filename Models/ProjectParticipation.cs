using System.Text.Json.Serialization;

public class ProjectParticipation
{
  private DateTime? _startDate;
  private DateTime? _endDate;

  [JsonPropertyName("id")]
  public int Id { get; set; }
  [JsonPropertyName("employeeId")]
  public int? EmployeeId { get; set; }
  [JsonPropertyName("projectId")]
  public int? ProjectId { get; set; }
  [JsonPropertyName("role")]
  public string? Role { get; set; }
  [JsonPropertyName("startDate")]
  public DateTime? StartDate
  {
    get => _startDate;
    set => _startDate = value?.ToUniversalTime();
  }
  [JsonPropertyName("endDate")]
  public DateTime? EndDate
  {
    get => _endDate;
    set => _endDate = value?.ToUniversalTime();
  }

  public Employee? Employee { get; set; }
  public Project? Project { get; set; }
}