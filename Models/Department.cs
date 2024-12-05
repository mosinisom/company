using System.Text.Json.Serialization;


public class Department
{
  private DateTime? _startDate;

  [JsonPropertyName("id")]
  public int Id { get; set; }
  [JsonPropertyName("name")]
  public string? Name { get; set; }
  [JsonPropertyName("managerId")]
  public int? ManagerId { get; set; }
  [JsonPropertyName("createdAt")]
  public DateTime? CreatedAt 
  { 
    get => _startDate;
    set => _startDate = value?.ToUniversalTime();
  
  }

  public Employee? Manager { get; set; }
  public ICollection<Employee>? Employees { get; set; }
}