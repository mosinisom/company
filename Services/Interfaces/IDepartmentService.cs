public interface IDepartmentService
{
  Task<Department> GetByIdAsync(int id);
  Task<IEnumerable<Department>> GetAllAsync();
  Task<Department> CreateAsync(Department department);
  Task<Department> UpdateAsync(Department department);
  Task<bool> DeleteAsync(int id);
}