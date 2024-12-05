using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

public class PositionService : IPositionService
{
  private readonly IServiceScopeFactory _scopeFactory;

  public PositionService(IServiceScopeFactory scopeFactory)
  {
    _scopeFactory = scopeFactory;
  }

  public async Task<IEnumerable<Position>> GetAllAsync()
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    return await context.Positions.ToListAsync();
  }

  public async Task<Position> GetByIdAsync(int id)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    return await context.Positions.FindAsync(id);
  }

  public async Task<Position> CreateAsync(Position position)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Positions.Add(position);
    await context.SaveChangesAsync();
    return position;
  }

  public async Task<Position> UpdateAsync(Position position)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Positions.Update(position);
    await context.SaveChangesAsync();
    return position;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var position = await context.Positions.FindAsync(id);
    if (position == null) return false;

    context.Positions.Remove(position);
    await context.SaveChangesAsync();
    return true;
  }
}