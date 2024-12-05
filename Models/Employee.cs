using System.Text.Json.Serialization;

public class Employee
{
  private DateTime? _birthDate;
  private DateTime? _hireDate;
  private DateTime? _createdAt;

  [JsonPropertyName("id")]
  public int Id { get; set; }
  [JsonPropertyName("firstName")]
  public string? FirstName { get; set; }
  [JsonPropertyName("lastName")]
  public string? LastName { get; set; }
  [JsonPropertyName("email")]
  public string? Email { get; set; }
  [JsonPropertyName("phone")]
  public string? Phone { get; set; }
  [JsonPropertyName("birthDate")]
  public DateTime? BirthDate 
  { 
    get => _birthDate; 
    set => _birthDate = value?.ToUniversalTime(); 
  }
  [JsonPropertyName("hireDate")]
  public DateTime? HireDate 
  { 
    get => _hireDate; 
    set => _hireDate = value?.ToUniversalTime(); 
  }
  [JsonPropertyName("departmentId")]
  public int? DepartmentId { get; set; }
  [JsonPropertyName("positionId")]
  public int? PositionId { get; set; }
  [JsonPropertyName("address")]
  public string? Address { get; set; }
  [JsonPropertyName("city")]
  public DateTime? CreatedAt
  {
    get => _createdAt;
    set => _createdAt = value?.ToUniversalTime();
  }

  public Department? Department { get; set; }
  public Position? Position { get; set; }
  public ICollection<ProjectParticipation>? ProjectParticipations { get; set; }
}