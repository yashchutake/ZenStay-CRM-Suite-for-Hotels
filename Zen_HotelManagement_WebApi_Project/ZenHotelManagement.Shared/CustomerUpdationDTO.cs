namespace ZenHotelManagement.Shared
{
    public record CustomerUpdationDTO(
               string IdType,
               string Name,
               string Gender,
               string Address,
               string Country,
               string MobileNo
           );
}
