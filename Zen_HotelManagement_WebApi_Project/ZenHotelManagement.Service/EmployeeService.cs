using AutoMapper;
using EmployeePortalWebApi.Entities.Exceptions;
using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Models;
using ZenHotelManagement.Service.Contracts;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        public EmployeeService(IRepositoryManager repository, IMapper mapper)
        {
            _repositoryManager = repository;
            _mapper = mapper;
        }


        public IEnumerable<EmployeeDto> GetAllEmployees(bool trackChanges)
        {
            var employees = _repositoryManager.Employee.GetAllEmployees(trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return employeesDto;
        }

        public EmployeeDto GetEmployeeById(int employeeId, bool trackChanges)
        {
            var employee = _repositoryManager.Employee.GetEmployeeById(employeeId, trackChanges);
            if (employee is null)
                throw new EmployeeNotFoundException(employeeId);
            var empployeeDto = _mapper.Map<EmployeeDto>(employee);
            return empployeeDto;
        }
        public EmployeeDto GetEmployeeByAdharNo(string adharNo, bool trackChanges)
        {
            var employee = _repositoryManager.Employee.GetEmployeeByAdharNo(adharNo, trackChanges);
            if (employee is null)
                throw new EmployeeNotFoundException(int.Parse(adharNo));
            var empployeeDto = _mapper.Map<EmployeeDto>(employee);
            return empployeeDto;
        }
        public EmployeeDto CreateEmployee(EmployeeDtoForOperation employee)
        {
            var employeeEntity = _mapper.Map<Employee>(employee);
            _repositoryManager.Employee.CreateEmployee(employeeEntity);
            _repositoryManager.Save();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return employeeToReturn;
        }
        public void UpdateEmployee(int employeeId, EmployeeDtoForOperation employeeForUpdate, bool trackChanges)
        {
            var employeeEntity = _repositoryManager.Employee.GetEmployeeByIdIncludingDeleted(employeeId, trackChanges);
            if (employeeEntity is null)
                throw new EmployeeNotFoundException(employeeId);
            _mapper.Map(employeeForUpdate, employeeEntity);
            _repositoryManager.Save();

        }
        public void DeleteEmployee(int employeeId, bool trackChanges)
        {
            var employeeEntity = _repositoryManager.Employee.GetEmployeeById(employeeId, trackChanges);
            if (employeeEntity is null)
                throw new EmployeeNotFoundException(employeeId);

            // Soft delete - mark as deleted
            employeeEntity.IsDeleted = true;
            _repositoryManager.Save();
        }

        public IEnumerable<EmployeeDto> GetAllEmployeesIncludingDeleted(bool trackChanges)
        {
            var employees = _repositoryManager.Employee.GetAllEmployeesIncludingDeleted(trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return employeesDto;
        }
        public EmployeeDto GetEmployeeByIdIncludingDeleted(int employeeId, bool trackChanges)
        {
            var employee = _repositoryManager.Employee.GetEmployeeByIdIncludingDeleted(employeeId, trackChanges);
            if (employee is null)
                throw new EmployeeNotFoundException(employeeId);
            var empployeeDto = _mapper.Map<EmployeeDto>(employee);
            return empployeeDto;
        }
    }
}
