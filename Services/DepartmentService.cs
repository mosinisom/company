using Backend.Data;
using Microsoft.EntityFrameworkCore;

public class DepartmentService : IDepartmentService
{
  private readonly IServiceScopeFactory _scopeFactory;

  public DepartmentService(IServiceScopeFactory scopeFactory)
  {
    _scopeFactory = scopeFactory;
  }

  public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var departments = await context.Departments
        .Include(d => d.Manager)
        .ToListAsync();

    return departments.Select(d => new DepartmentDto
    {
      Id = d.Id,
      Name = d.Name,
      ManagerId = d.ManagerId,
      CreatedAt = d.CreatedAt,
      ManagerName = d.Manager != null ? $"{d.Manager.FirstName} {d.Manager.LastName}" : null
    });
  }

  public async Task<DepartmentDto?> GetByIdAsync(int id)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var department = await context.Departments
        .Include(d => d.Manager)
        .FirstOrDefaultAsync(d => d.Id == id);

    if (department == null) return null;

    return new DepartmentDto
    {
      Id = department.Id,
      Name = department.Name,
      ManagerId = department.ManagerId,
      CreatedAt = department.CreatedAt,
      ManagerName = department.Manager != null ? $"{department.Manager.FirstName} {department.Manager.LastName}" : null
    };
  }

  public async Task<DepartmentDto> CreateAsync(DepartmentDto departmentDto)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var department = new Department
    {
      Name = departmentDto.Name,
      ManagerId = departmentDto.ManagerId,
      CreatedAt = departmentDto.CreatedAt ?? DateTime.UtcNow
    };

    context.Departments.Add(department);
    await context.SaveChangesAsync();

    return await GetByIdAsync(department.Id);
  }

  public async Task<DepartmentDto> UpdateAsync(DepartmentDto departmentDto)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var department = await context.Departments.FindAsync(departmentDto.Id);
    if (department == null) throw new KeyNotFoundException($"Department with id {departmentDto.Id} not found");

    department.Name = departmentDto.Name;
    department.ManagerId = departmentDto.ManagerId;
    department.CreatedAt = departmentDto.CreatedAt;

    await context.SaveChangesAsync();

    return await GetByIdAsync(department.Id);
  }

  public async Task<bool> DeleteAsync(int id)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var department = await context.Departments.FindAsync(id);
    if (department == null) return false;

    context.Departments.Remove(department);
    await context.SaveChangesAsync();
    return true;
  }
}