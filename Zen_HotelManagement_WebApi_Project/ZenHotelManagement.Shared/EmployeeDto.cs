namespace ZenHotelManagement.Shared
{
    public record EmployeeDto(int EmployeeId, string Name, int Age, string Gender, string Position, decimal Salary, string MobileNo, string AdharNo, string Email, bool IsDeleted);
}
