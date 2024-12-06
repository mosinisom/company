public class DepartmentDto
{
  public int Id { get; set; }
  public string? Name { get; set; }
  public int? ManagerId { get; set; }
  public DateTime? CreatedAt { get; set; }
  public string? ManagerName { get; set; } 
}