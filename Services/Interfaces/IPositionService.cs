public interface IPositionService
{
  Task<IEnumerable<Position>> GetAllAsync();
  Task<Position> GetByIdAsync(int id);
  Task<Position> CreateAsync(Position position);
  Task<Position> UpdateAsync(Position position);
  Task<bool> DeleteAsync(int id);
}