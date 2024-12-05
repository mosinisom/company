public interface IProjectService
{
  Task<Project> GetByIdAsync(int id);
  Task<IEnumerable<Project>> GetAllAsync();
  Task<Project> CreateAsync(Project project);
  Task<Project> UpdateAsync(Project project);
  Task<bool> DeleteAsync(int id);
  Task<IEnumerable<Project>> GetByEmployeeAsync(int employeeId);
}