using Microsoft.EntityFrameworkCore;
using ZenHotelManagement.Entities.Models;
using ZenHotelManagement.Repository.Configuration;

namespace ZenHotelManagement.Repository
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CabDriverConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new RoomConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            //modelBuilder.ApplyConfiguration(new CabBookingConfiguration());
            //modelBuilder.ApplyConfiguration(new BookingConfiguration());
        }


        public DbSet<User>? Users { get; set; }
        public DbSet<Room>? Rooms { get; set; }
        public DbSet<Employee>? Employees { get; set; }
        public DbSet<Customer>? Customers { get; set; }
        public DbSet<CabDriver>? CabDrivers { get; set; }
        public DbSet<Customer>? Customer { get; set; }
        public DbSet<CabBooking>? CabBooking { get; set; }
        public DbSet<RoomBooking>? Bookings { get; set; }

    }
}
