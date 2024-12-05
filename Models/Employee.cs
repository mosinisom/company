public class Employee
{
  public int Id { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string Email { get; set; }
  public string Phone { get; set; }
  public DateTime BirthDate { get; set; }
  public DateTime HireDate { get; set; }
  public int? DepartmentId { get; set; }  
  public int PositionId { get; set; }   
  public string Address { get; set; }
  public DateTime CreatedAt { get; set; }

  // Навигационные свойства
  public Department? Department { get; set; }
  public Position Position { get; set; }
  public ICollection<ProjectParticipation> ProjectParticipations { get; set; }
}