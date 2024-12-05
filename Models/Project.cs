using System.Text.Json.Serialization;

public class Project
{
  private DateTime? _startDate;
  private DateTime? _endDate;
  private DateTime? _createdAt;

  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("name")]
  public string? Name { get; set; }

  [JsonPropertyName("description")]
  public string? Description { get; set; }

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

  [JsonPropertyName("status")]
  public string? Status { get; set; }

  [JsonPropertyName("createdAt")]
  public DateTime? CreatedAt
  {
    get => _createdAt;
    set => _createdAt = value?.ToUniversalTime();
  }

  public ICollection<ProjectParticipation>? Participants { get; set; }
}