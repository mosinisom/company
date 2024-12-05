using Backend.Data;
using Microsoft.EntityFrameworkCore;

public class DepartmentService : IDepartmentService
{
  private readonly IServiceScopeFactory _scopeFactory;

  public DepartmentService(IServiceScopeFactory scopeFactory)
  {
    _scopeFactory = scopeFactory;
  }

  public async Task<IEnumerable<Department>> GetAllAsync()
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    return await context.Departments
        .Include(d => d.Manager)
        .ToListAsync();
  }

  public async Task<Department> GetByIdAsync(int id)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    return await context.Departments
        .Include(d => d.Manager)
        .FirstOrDefaultAsync(d => d.Id == id);
  }

  public async Task<Department> CreateAsync(Department department)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Departments.Add(department);
    await context.SaveChangesAsync();
    return department;
  }

  public async Task<Department> UpdateAsync(Department department)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Departments.Update(department);
    await context.SaveChangesAsync();
    return department;
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