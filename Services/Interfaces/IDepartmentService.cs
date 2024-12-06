public interface IDepartmentService
{
  Task<IEnumerable<DepartmentDto>> GetAllAsync();
  Task<DepartmentDto?> GetByIdAsync(int id);
  Task<DepartmentDto> CreateAsync(DepartmentDto departmentDto);
  Task<DepartmentDto> UpdateAsync(DepartmentDto departmentDto);
  Task<bool> DeleteAsync(int id);
}