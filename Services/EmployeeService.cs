using Backend.Data;
using Microsoft.EntityFrameworkCore;

public class EmployeeService : IEmployeeService
{
  private readonly IServiceScopeFactory _scopeFactory;

  public EmployeeService(IServiceScopeFactory scopeFactory)
  {
    _scopeFactory = scopeFactory;
  }

  private static EmployeeDto MapToDto(Employee employee)
  {
    return new EmployeeDto
    {
      Id = employee.Id,
      FirstName = employee.FirstName,
      LastName = employee.LastName,
      Email = employee.Email,
      Phone = employee.Phone,
      BirthDate = employee.BirthDate,
      HireDate = employee.HireDate,
      DepartmentId = employee.DepartmentId,
      DepartmentName = employee.Department?.Name,
      PositionId = employee.PositionId,
      PositionTitle = employee.Position?.Title,
      Address = employee.Address
    };
  }

  public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var employees = await context.Employees
        .Include(e => e.Department)
        .Include(e => e.Position)
        .Select(e => MapToDto(e))
        .ToListAsync();

    return employees;
  }

  public async Task<EmployeeDto> GetByIdAsync(int id)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var employee = await context.Employees
        .Include(e => e.Department)
        .Include(e => e.Position)
        .FirstOrDefaultAsync(e => e.Id == id);
    return employee != null ? MapToDto(employee) : null;
  }

  public async Task<EmployeeDto> CreateAsync(Employee employee)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Employees.Add(employee);
    await context.SaveChangesAsync();
    return MapToDto(employee);
  }

  public async Task<EmployeeDto> UpdateAsync(Employee employee)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Employees.Update(employee);
    await context.SaveChangesAsync();
    return MapToDto(employee);
  }

  public async Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(int departmentId)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var employees = await context.Employees
        .Include(e => e.Department)
        .Include(e => e.Position)
        .Where(e => e.DepartmentId == departmentId)
        .Select(e => MapToDto(e))
        .ToListAsync();
    return employees;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var employee = await context.Employees.FindAsync(id);
    if (employee == null) return false;

    context.Employees.Remove(employee);
    await context.SaveChangesAsync();
    return true;
  }
}