public interface IEmployeeService
{
  Task<IEnumerable<EmployeeDto>> GetAllAsync();
  Task<EmployeeDto> GetByIdAsync(int id);
  Task<EmployeeDto> CreateAsync(Employee employee);
  Task<EmployeeDto> UpdateAsync(Employee employee);
  Task<bool> DeleteAsync(int id);
  Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(int departmentId);
}