public class Department
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int? ManagerId { get; set; }
  public DateTime CreatedAt { get; set; }

  // Навигационные свойства
  public Employee Manager { get; set; }
  public ICollection<Employee> Employees { get; set; }
}