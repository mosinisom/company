using System.Text.Json.Serialization;

public class Position
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("title")]
  public string Title { get; set; }

  [JsonPropertyName("salaryRange")]
  public string SalaryRange { get; set; }

  [JsonPropertyName("responsibilities")]
  public string Responsibilities { get; set; }

  public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}