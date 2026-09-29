using System.Net;
using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Services
{
    public class RoomService : IRoomService
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:5001/api/rooms/";
        public RoomService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        //public Room CreateRoom(Room room)
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    var response = client.PostAsJsonAsync(_apiBaseUrl, room).Result;

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var createdRoom = response.Content.ReadFromJsonAsync<Room>().Result;
        //        return createdRoom!;
        //    }
        //    throw new Exception("Failed to create room. Status: " + response.StatusCode);
        //}

        //public Room CreateRoom(Room room)
        //{
        //    var client = _httpClientFactory.CreateClient();

        //    // Check if the room already exists
        //    var checkResponse = client.GetAsync($"{_apiBaseUrl}/{room.RoomNo}").Result;
        //    if (checkResponse.IsSuccessStatusCode)
        //    {
        //        throw new Exception("Room number already exists.");
        //    }

        //    // Create the room if it doesn't exist
        //    var response = client.PostAsJsonAsync(_apiBaseUrl, room).Result;

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var createdRoom = response.Content.ReadFromJsonAsync<Room>().Result;
        //        return createdRoom!;
        //    }
        //    throw new Exception("Failed to create room. Status: " + response.StatusCode);
        //}

        public Room CreateRoom(Room room)
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                // Check if the room already exists
                var checkResponse = client.GetAsync($"{_apiBaseUrl}/{room.RoomNo}").Result;
                if (checkResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Room number already exists.");
                }
                else
                {
                    // Log the status code and response content for debugging
                    var checkContent = checkResponse.Content.ReadAsStringAsync().Result;
                    Console.WriteLine($"Check Response Status: {checkResponse.StatusCode}, Content: {checkContent}");
                }

                // Create the room if it doesn't exist
                var response = client.PostAsJsonAsync(_apiBaseUrl, room).Result;

                if (response.IsSuccessStatusCode)
                {
                    var createdRoom = response.Content.ReadFromJsonAsync<Room>().Result;
                    return createdRoom!;
                }
                else
                {
                    // Handle different status codes
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            throw new Exception("Bad request. Please check the room details.");
                        case HttpStatusCode.Unauthorized:
                            throw new Exception("Unauthorized. Please check your credentials.");
                        case HttpStatusCode.Forbidden:
                            throw new Exception("Forbidden. You do not have permission to create a room.");
                        case HttpStatusCode.NotFound:
                            throw new Exception("API endpoint not found.");
                        case HttpStatusCode.InternalServerError:
                            throw new Exception("Server error. Please try again later.");
                        default:
                            throw new Exception("Failed to create room. Status: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "Error creating room");

                // Provide a user-friendly message
                throw new Exception($"Error: {ex.Message}");
            }
        }


        public IEnumerable<Room> GetAllRooms()
        {
            var client = _httpClientFactory.CreateClient();
            var response = client.GetAsync(_apiBaseUrl).Result;
            if (response.IsSuccessStatusCode)
            {
                var rooms = response.Content.ReadFromJsonAsync<IEnumerable<Room>>().Result;
                return rooms ?? Enumerable.Empty<Room>();
            }
            return Enumerable.Empty<Room>();
        }

        public IEnumerable<Room> GetRoomsByType(string roomType)
        {

            var client = _httpClientFactory.CreateClient();
            var response = client.GetAsync($"{_apiBaseUrl}/type/{roomType}").Result;
            if (response.IsSuccessStatusCode)
            {
                var rooms = response.Content.ReadFromJsonAsync<IEnumerable<Room>>().Result;
                return rooms ?? Enumerable.Empty<Room>();
            }
            return Enumerable.Empty<Room>();
        }
        public void UpdateRoom(string roomNo, Room room)
        {
            var client = _httpClientFactory.CreateClient();
            var response = client.PutAsJsonAsync($"{_apiBaseUrl}/{roomNo}", room).Result;
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result;
                throw new Exception($"Failed to update room. Status: {response.StatusCode}. Details: {errorContent}");
            }
        }

        public async Task<bool> UpdateRoomCleanStatusAsync(string roomNo, bool isClean)
        {
            var client = _httpClientFactory.CreateClient();
            var content = System.Net.Http.Json.JsonContent.Create(new { roomStatus = true, isClean });
            var response = await client.PutAsync($"{_apiBaseUrl}{roomNo}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRoomAsync(string roomNo)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                
                // First get the current room state
                var room = await GetRoomByNoAsync(roomNo);
                if (room == null)
                {
                    return false;
                }

                // Toggle IsDeleted and set RoomStatus accordingly
                room.IsDeleted = !room.IsDeleted;
                room.RoomStatus = !room.IsDeleted; // true when restoring, false when deleting

                // Update the room with new status
                var updateResponse = await client.PutAsJsonAsync($"{_apiBaseUrl}{roomNo}", room);
                if (!updateResponse.IsSuccessStatusCode)
                {
                    return false;
                }

                // Now perform the delete/restore operation
                var deleteResponse = await client.DeleteAsync($"{_apiBaseUrl}{roomNo}");
                return deleteResponse.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }        }

        public async Task<Room?> GetRoomByNoAsync(string roomNo)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}number/{roomNo}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Room>();
            }
            return null;
        }

        public async Task<Room?> GetRoomByIdAsync(int roomId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_apiBaseUrl}{roomId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Room>();
            }
            return null;
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(_apiBaseUrl);
            if (response.IsSuccessStatusCode)
            {
                var rooms = await response.Content.ReadFromJsonAsync<IEnumerable<Room>>();
                return rooms?.Where(r => r.RoomStatus == true && !r.IsDeleted) ?? Enumerable.Empty<Room>();
            }
            return Enumerable.Empty<Room>();
        }
    }



}
