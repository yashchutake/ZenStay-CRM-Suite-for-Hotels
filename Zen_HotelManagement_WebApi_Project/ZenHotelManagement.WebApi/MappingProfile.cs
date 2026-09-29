using AutoMapper;
using ZenHotelManagement.Entities.Models;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.WebApi
{
    public class MappingProfile : Profile
    {        
        public MappingProfile()
        {
            CreateMap<Room, RoomDto>();
            CreateMap<RoomForCreateDto, Room>();
            CreateMap<RoomForUpdateDto, Room>();

            CreateMap<CabDriver, CabDriverDto>();
            CreateMap<CabDriverDtoForOperation, CabDriver>();

            CreateMap<CabBooking, CabBookingDto>();
            CreateMap<CabBookingDtoForOperation, CabBooking>();            // Update RoomBooking mappings to include Room details and calculate PendingAmount
            CreateMap<RoomBooking, RoomBookingDto>()
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room))
                .ForMember(dest => dest.PendingAmount, opt => opt.MapFrom(src => 
                    src.TotalAmount.HasValue ? Math.Max(0, src.TotalAmount.Value - src.AmountPaid) : (decimal?)null));
            CreateMap<RoomBookingDtoForCreation, RoomBooking>()
                .ForMember(dest => dest.PendingAmount, opt => opt.MapFrom(src => 
                    src.TotalAmount.HasValue ? Math.Max(0, src.TotalAmount.Value - src.AmountPaid) : (decimal?)null));
            CreateMap<RoomBookingDtoForUpdation, RoomBooking>()
                .ForMember(dest => dest.PendingAmount, opt => opt.MapFrom(src => 
                    Math.Max(0, src.TotalAmount - src.AmountPaid)));

            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeDtoForOperation, Employee>();            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId ?? ""))
                .ForMember(dest => dest.IdType, opt => opt.MapFrom(src => src.IdType ?? ""))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? ""))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender ?? ""))
                .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.MobileNo ?? ""));
            CreateMap<Customer, CustomerDTOWithBookings>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId ?? ""))
                .ForMember(dest => dest.IdType, opt => opt.MapFrom(src => src.IdType ?? ""))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? ""))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender ?? ""))
                .ForMember(dest => dest.MobileNo, opt => opt.MapFrom(src => src.MobileNo ?? ""))
                .ForMember(dest => dest.RoomBookings, opt => opt.MapFrom(src => src.RoomBookings))
                .ForMember(dest => dest.CabBookings, opt => opt.MapFrom(src => src.CabBookings));
            CreateMap<CustomerCreationDTO, Customer>()
                .ForMember(dest => dest.RoomBookings, opt => opt.Ignore())
                .ForMember(dest => dest.CabBookings, opt => opt.Ignore());
            CreateMap<CustomerUpdationDTO, Customer>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
