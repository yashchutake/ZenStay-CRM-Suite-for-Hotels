using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Service.Contracts
{    public interface IEmployeeService
    {
        IEnumerable<EmployeeDto> GetAllEmployees(bool trackChanges);
        IEnumerable<EmployeeDto> GetAllEmployeesIncludingDeleted(bool trackChanges);        EmployeeDto GetEmployeeById(int employeeId, bool trackChanges);
        EmployeeDto GetEmployeeByIdIncludingDeleted(int employeeId, bool trackChanges);
        EmployeeDto GetEmployeeByAdharNo(string adharNo, bool trackChanges);
        EmployeeDto CreateEmployee(EmployeeDtoForOperation employee);
        void UpdateEmployee(int employeeId, EmployeeDtoForOperation employeeForUpdate, bool trackChanges);
        void DeleteEmployee(int employeeId, bool trackChanges);
    }
}
