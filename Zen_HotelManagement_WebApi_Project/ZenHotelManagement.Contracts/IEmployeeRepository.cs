using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Contracts
{    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetAllEmployees(bool trackChanges);
        IEnumerable<Employee> GetAllEmployeesIncludingDeleted(bool trackChanges);
        Employee? GetEmployeeByAdharNo(string AdharNo, bool trackChanges);        
        Employee? GetEmployeeById(int employeeId, bool trackChanges);
        Employee? GetEmployeeByIdIncludingDeleted(int employeeId, bool trackChanges);
        void CreateEmployee(Employee employee);
    }
}
