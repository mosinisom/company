using Backend.Data;
using Microsoft.EntityFrameworkCore;

public class ProjectService : IProjectService
{
  private readonly IServiceScopeFactory _scopeFactory;

  public ProjectService(IServiceScopeFactory scopeFactory)
  {
    _scopeFactory = scopeFactory;
  }

  public async Task<IEnumerable<Project>> GetAllAsync()
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    return await context.Projects
        .Include(p => p.Participants)
        .ToListAsync();
  }

  public async Task<Project> GetByIdAsync(int id)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    return await context.Projects
        .Include(p => p.Participants)
        .FirstOrDefaultAsync(p => p.Id == id);
  }

  public async Task<Project> CreateAsync(Project project)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Projects.Add(project);
    await context.SaveChangesAsync();
    return project;
  }

  public async Task<Project> UpdateAsync(Project project)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Projects.Update(project);
    await context.SaveChangesAsync();
    return project;
  }


  public async Task<bool> DeleteAsync(int id)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var project = await context.Projects.FindAsync(id);
    if (project == null)
    {
      return false;
    }

    context.Projects.Remove(project);
    await context.SaveChangesAsync();
    return true;
  }

  public async Task<IEnumerable<Project>> GetByEmployeeAsync(int employeeId)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    return await context.Projects
        .Include(p => p.Participants)
        .Where(p => p.Participants.Any(p => p.EmployeeId == employeeId))
        .ToListAsync();
  }
}