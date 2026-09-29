using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Services.Contracts;
using ZenHotelManagement.WebApp.Utilities;
using System.Text.Json;

namespace ZenHotelManagement.WebApp.Services
{
    public class CustomerService : ICustomerService
    {
        private IHttpClientFactory _httpClientFactory;
        private IRoomBookingService _roomBookingService;
        private ICabBookingService _cabBookingService;
        public CustomerService(IHttpClientFactory httpClientFactory, IRoomBookingService roomBookingService, ICabBookingService cabBookingService)
        {
            _httpClientFactory = httpClientFactory;
            _roomBookingService = roomBookingService;
            _cabBookingService = cabBookingService;
        }
        public async Task<Customer> AddCustomer(Customer customer)
        {
            var httpClient = _httpClientFactory.CreateClient("API");

            // Create the request body matching the API expected format
            var customerData = new
            {
                customerId = customer.CustomerId,
                idType = customer.IdType,
                name = customer.Name,
                gender = customer.Gender,
                address = customer.Address,
                country = customer.Country,
                mobileNo = customer.MobileNo,
                roomBookings = customer.RoomBookings?.Select(rb => new
                {
                    roomId = rb.RoomId,
                    checkInDate = rb.CheckInDate,
                    checkOutDate = rb.CheckOutDate,
                    totalAmount = rb.TotalAmount,
                    amountPaid = rb.AmountPaid,
                    bookingDate = rb.BookingDate,
                    bookingStatus = rb.BookingStatus,
                    numberOfGuests = rb.NumberOfGuests
                }).ToList(),
                cabBookings = customer.CabBookings?.Select(cb => new
                {
                    cabDriverId = cb.CabDriverId,
                    pickUpDateTime = cb.PickUpDateTime,
                    dropOffDateTime = cb.DropOffDateTime,
                    pickUpLocation = cb.PickUpLocation,
                    dropOffLocation = cb.DropOffLocation,
                    fare = cb.Fare
                }).ToList()
            };

            var json = System.Text.Json.JsonSerializer.Serialize(customerData);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("api/customers", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                // For now, return the customer as-is since we don't need to parse the full response
                return customer;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create customer: {response.StatusCode} - {errorContent}");
            }
        }

        public void UpdateCustomer(string customerId, Customer customerForUpdate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Customer>> ViewAllCustomers()
        {
            var httpClient = _httpClientFactory.CreateClient("API");
            var response = await httpClient.GetAsync("api/customers");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Parse the response and map to Customer objects
                var customersData = System.Text.Json.JsonSerializer.Deserialize<List<dynamic>>(json, options);

                // For simplicity, let's create a basic mapping
                // You might want to create proper DTOs for this
                var customers = new List<Customer>();

                using (var document = JsonDocument.Parse(json))
                {
                    foreach (var element in document.RootElement.EnumerateArray())
                    {
                        var customer = new Customer
                        {
                            Id = element.GetProperty("id").GetInt32(),
                            CustomerId = element.TryGetProperty("customerId", out var custId) && custId.ValueKind != JsonValueKind.Null ? custId.GetString() : "",
                            IdType = element.TryGetProperty("idType", out var idType) && idType.ValueKind != JsonValueKind.Null ? idType.GetString() : "",
                            Name = element.TryGetProperty("name", out var name) && name.ValueKind != JsonValueKind.Null ? name.GetString() : "",
                            Gender = element.TryGetProperty("gender", out var gender) && gender.ValueKind != JsonValueKind.Null ? gender.GetString() : "",
                            Address = element.TryGetProperty("address", out var addr) && addr.ValueKind != JsonValueKind.Null ? addr.GetString() : null,
                            Country = element.TryGetProperty("country", out var country) && country.ValueKind != JsonValueKind.Null ? country.GetString() : null,
                            MobileNo = element.TryGetProperty("mobileNo", out var mobile) && mobile.ValueKind != JsonValueKind.Null ? mobile.GetString() : "",
                            RoomBookings = new List<RoomBooking>(),
                            CabBookings = new List<CabBooking>()
                        };
                        // Parse room bookings if they exist
                        if (element.TryGetProperty("roomBookings", out var roomBookings))
                        {
                            foreach (var booking in roomBookings.EnumerateArray())
                            {
                                var checkInDate = booking.GetProperty("checkInDate").GetDateTime(); var checkOutDate = booking.TryGetProperty("checkOutDate", out var checkOut) && checkOut.ValueKind != JsonValueKind.Null ? checkOut.GetDateTime() : (DateTime?)null;
                                var bookingStatus = booking.TryGetProperty("bookingStatus", out var status) && status.ValueKind != JsonValueKind.Null ? status.GetString() : "";

                                // Create a temporary RoomBooking object for status calculation
                                var tempBooking = new RoomBooking
                                {
                                    BookingStatus = bookingStatus,
                                    CheckInDate = checkInDate,
                                    CheckOutDate = checkOutDate
                                };

                                // Use RealTimeStatusCalculator for consistent minute-precision status calculation
                                bookingStatus = RealTimeStatusCalculator.CalculateRoomBookingStatus(tempBooking);

                                // Parse room data if available
                                Room? room = null;
                                if (booking.TryGetProperty("room", out var roomData) && roomData.ValueKind != JsonValueKind.Null)
                                {
                                    room = new Room
                                    {
                                        RoomId = roomData.GetProperty("roomId").GetInt32(),
                                        RoomNo = roomData.TryGetProperty("roomNo", out var roomNo) && roomNo.ValueKind != JsonValueKind.Null ? roomNo.GetString() : "",
                                        RoomType = roomData.TryGetProperty("roomType", out var roomType) && roomType.ValueKind != JsonValueKind.Null ? roomType.GetString() : "",
                                        IsAc = roomData.TryGetProperty("isAc", out var isAc) && isAc.ValueKind != JsonValueKind.Null ? isAc.GetBoolean() : false,
                                        CostPerNight = roomData.TryGetProperty("costPerNight", out var costPerNight) && costPerNight.ValueKind != JsonValueKind.Null ? costPerNight.GetDecimal() : 0,
                                        RoomCapacity = roomData.TryGetProperty("roomCapacity", out var roomCapacity) && roomCapacity.ValueKind != JsonValueKind.Null ? roomCapacity.GetInt32() : 1,
                                        RoomStatus = roomData.TryGetProperty("roomStatus", out var roomStatus) && roomStatus.ValueKind != JsonValueKind.Null ? roomStatus.GetBoolean() : true,
                                        IsClean = roomData.TryGetProperty("isClean", out var isClean) && isClean.ValueKind != JsonValueKind.Null ? isClean.GetBoolean() : true
                                    };
                                }

                                customer.RoomBookings.Add(new RoomBooking
                                {
                                    BookingId = booking.GetProperty("bookingId").GetInt32(),
                                    RoomId = booking.GetProperty("roomId").GetInt32(),
                                    CheckInDate = booking.GetProperty("checkInDate").GetDateTime(),
                                    CheckOutDate = checkOutDate,
                                    TotalAmount = booking.TryGetProperty("totalAmount", out var total) && total.ValueKind != JsonValueKind.Null ? total.GetDecimal() : null,
                                    AmountPaid = booking.GetProperty("amountPaid").GetDecimal(),
                                    BookingDate = booking.GetProperty("bookingDate").GetDateTime(),
                                    BookingStatus = bookingStatus,
                                    NumberOfGuests = booking.GetProperty("numberOfGuests").GetInt32(),
                                    Room = room
                                });
                            }
                        }
                        // Parse cab bookings if they exist
                        if (element.TryGetProperty("cabBookings", out var cabBookings))
                        {
                            foreach (var cabBooking in cabBookings.EnumerateArray())
                            {
                                var dropOffDateTime = cabBooking.GetProperty("dropOffDateTime").GetDateTime(); var isRideCompleted = cabBooking.TryGetProperty("isRideCompleted", out var completed) && completed.ValueKind != JsonValueKind.Null ? completed.GetBoolean() : false;

                                // Create a temporary CabBooking object for status calculation
                                var tempCabBooking = new CabBooking
                                {
                                    IsRideCompleted = isRideCompleted,
                                    DropOffDateTime = dropOffDateTime
                                };

                                // Use RealTimeStatusCalculator for consistent minute-precision completion status
                                isRideCompleted = RealTimeStatusCalculator.CalculateCabBookingCompletionStatus(tempCabBooking);

                                customer.CabBookings.Add(new CabBooking
                                {
                                    CabBookingId = cabBooking.GetProperty("cabBookingId").GetInt32(),
                                    CabDriverId = cabBooking.GetProperty("cabDriverId").GetInt32(),
                                    PickUpDateTime = cabBooking.GetProperty("pickUpDateTime").GetDateTime(),
                                    DropOffDateTime = dropOffDateTime,
                                    PickUpLocation = cabBooking.TryGetProperty("pickUpLocation", out var pickup) && pickup.ValueKind != JsonValueKind.Null ? pickup.GetString() : "",
                                    DropOffLocation = cabBooking.TryGetProperty("dropOffLocation", out var dropoff) && dropoff.ValueKind != JsonValueKind.Null ? dropoff.GetString() : "",
                                    Fare = cabBooking.GetProperty("fare").GetDecimal(),
                                    IsRideCompleted = isRideCompleted
                                });
                            }
                        }

                        customers.Add(customer);
                    }
                }

                return customers;
            }
            else
            {
                throw new Exception($"Failed to fetch customers: {response.StatusCode}");
            }
        }

        public Customer ViewCustomerById(string customerId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Customer> ViewCustomersByRoomType(string roomType)
        {
            throw new NotImplementedException();
        }
    }
}
