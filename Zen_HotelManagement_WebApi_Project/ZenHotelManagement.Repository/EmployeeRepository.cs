using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repository) : base(repository)
        {
        }
        public IEnumerable<Employee> GetAllEmployees(bool trackChanges) =>
            FindByCondition(e => !e.IsDeleted, trackChanges)
            .OrderBy(e => e.Name ?? string.Empty).ToList();

        public IEnumerable<Employee> GetAllEmployeesIncludingDeleted(bool trackChanges) =>
            FindAll(trackChanges)
            .OrderBy(e => e.Name ?? string.Empty).ToList();

        public Employee? GetEmployeeByAdharNo(string AdharNo, bool trackChanges) =>
            FindByCondition(e => e.AdharNo != null && e.AdharNo.Equals(AdharNo) && !e.IsDeleted, trackChanges).SingleOrDefault();

        public Employee? GetEmployeeById(int employeeId, bool trackChanges) =>
            FindByCondition(e => e.EmployeeId.Equals(employeeId) && !e.IsDeleted, trackChanges).SingleOrDefault();

        public Employee? GetEmployeeByIdIncludingDeleted(int employeeId, bool trackChanges) =>
            FindByCondition(e => e.EmployeeId.Equals(employeeId), trackChanges).SingleOrDefault();

        public void CreateEmployee(Employee employee) => Create(employee);

    }
}

