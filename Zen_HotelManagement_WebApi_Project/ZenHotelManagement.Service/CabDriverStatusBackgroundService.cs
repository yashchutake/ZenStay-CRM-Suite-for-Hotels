using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZenHotelManagement.Service.Contracts;

namespace ZenHotelManagement.Service
{
    public class CabDriverStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CabDriverStatusBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(1); // Check every minute for real-time updates

        public CabDriverStatusBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<CabDriverStatusBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_period);
              while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {                    using var scope = _serviceProvider.CreateScope();
                    var serviceManager = scope.ServiceProvider.GetRequiredService<IServiceManager>();
                    var cabBookingService = serviceManager.CabBookingService;
                    var roomBookingService = serviceManager.RoomBookingService;
                    
                    // Complete expired cab rides
                    cabBookingService.CompleteExpiredRides();
                    
                    // Update cab driver availability based on current time
                    cabBookingService.UpdateDriverAvailabilityBasedOnCurrentTime();
                    
                    // Complete expired room bookings
                    roomBookingService.CompleteExpiredRoomBookings();
                    
                    // Update room availability based on current time
                    roomBookingService.UpdateRoomAvailabilityBasedOnCurrentTime();
                      _logger.LogInformation("Cab driver and room statuses updated at {Time}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating cab driver and room statuses at {Time}", DateTime.Now);
                }
            }
        }
    }
}
