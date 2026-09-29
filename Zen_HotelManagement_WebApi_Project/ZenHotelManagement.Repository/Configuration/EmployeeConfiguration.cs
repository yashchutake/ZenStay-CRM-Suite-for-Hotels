using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository.Configuration
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {

            builder.HasData(
                new Employee
                {
                    EmployeeId = 101,
                    Name = "Yash Chutake",
                    Age = 23,
                    Gender = "Male",
                    Position = "Manager",
                    Salary = 70000.00m,
                    MobileNo = "9834452728",
                    AdharNo = "123412341234",
                    Email = "yash@gmail.com",

                },
                new Employee
                {
                    EmployeeId = 102,
                    Name = "Rushikesh Kolte",
                    Age = 22,
                    Gender = "Male",
                    Position = "Accountant",
                    Salary = 45000.00m,
                    MobileNo = "9876543210",
                    AdharNo = "234523452345",
                    Email = "rushikesh@gmail.com",

                },
                 new Employee
                 {
                     EmployeeId = 103,
                     Name = "Shaibaz Hasware",
                     Age = 25,
                     Gender = "Male",
                     Position = "Kitchen Staff",
                     Salary = 40000.00m,
                     MobileNo = "9123456789",
                     AdharNo = "345634563456",
                     Email = "shaibaz@gmail.com",

                 },
                 new Employee
                 {
                     EmployeeId = 104,
                     Name = "Abhishek Khodade",
                     Age = 21,
                     Gender = "Male",
                     Position = "Front Desk Clerk",
                     Salary = 40000.00m,
                     MobileNo = "9988776655",
                     AdharNo = "456745674567",
                     Email = "abhishek@gmail.com",
                 },
                 new Employee
                 {
                     EmployeeId = 105,
                     Name = "Yash Kashid",
                     Age = 27,
                     Gender = "Male",
                     Position = "Kitchen Staff",
                     Salary = 42000.00m,
                     MobileNo = "9876543211",
                     AdharNo = "567856785678",
                     Email = "yashkashid@gmail.com",
                 },
                 new Employee
                 {
                     EmployeeId = 106,
                     Name = "Samiksha Shelke",
                     Age = 24,
                     Gender = "Female",
                     Position = "Accountant",
                     Salary = 40000.00m,
                     MobileNo = "9123456780",
                     AdharNo = "678967896789",
                     Email = "samiksha@gmail.com",
                 },
                 new Employee
                 {
                     EmployeeId = 107,
                     Name = "Tony Stark",
                     Age = 45,
                     Gender = "Male",
                     Position = "Room service",
                     Salary = 12000.00m,
                     MobileNo = "9123456781",
                     AdharNo = "123412341234",
                     Email = "tony@stark.com",
                 },
                new Employee
                {
                    EmployeeId = 108,
                    Name = "John Cena",
                    Age = 43,
                    Gender = "Male",
                    Position = "Manager",
                    Salary = 85000.00m,
                    MobileNo = "9123456782",
                    AdharNo = "234523452345",
                    Email = "john@wwe.com",
                },
                new Employee
                {
                    EmployeeId = 109,
                    Name = "Randy Orton",
                    Age = 41,
                    Gender = "Male",
                    Position = "Housekeeping",
                    Salary = 60000.00m,
                    MobileNo = "9123456783",
                    AdharNo = "345634563456",
                    Email = "randy@wwe.com",
                },
                new Employee
                {
                    EmployeeId = 110,
                    Name = "Brock Lesnar",
                    Age = 44,
                    Gender = "Male",
                    Position = "Waiter",
                    Salary = 50000.00m,
                    MobileNo = "9123456784",
                    AdharNo = "456745674567",
                    Email = "brock@wwe.com",
                },
                new Employee
                {
                    EmployeeId = 111,
                    Name = "Reha Repli",
                    Age = 48,
                    Gender = "Female",
                    Position = "Waitress",
                    Salary = 50000.00m,
                    MobileNo = "9123456785",
                    AdharNo = "567856785678",
                    Email = "reha@gmail.com",
                },
                new Employee
                {
                    EmployeeId = 112,
                    Name = "Bruce Wayne",
                    Age = 40,
                    Gender = "Male",
                    Position = "Manager",
                    Salary = 15000.00m,
                    MobileNo = "9123456786",
                    AdharNo = "678967896789",
                    Email = "brucewayne@g.com",
                },
                new Employee
                {
                    EmployeeId = 113,
                    Name = "Clark Kent",
                    Age = 35,
                    Gender = "Male",
                    Position = "Waiter",
                    Salary = 55000.00m,
                    MobileNo = "9123456787",
                    AdharNo = "789078907890",
                    Email = "clark.kent@gl.com",
                }

                );

        }
    }
}

