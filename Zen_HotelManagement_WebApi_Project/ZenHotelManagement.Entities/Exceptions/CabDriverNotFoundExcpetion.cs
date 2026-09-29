namespace EmployeePortalWebApi.Entities.Exceptions
{
    public class CabDriverNotFoundExcpetion : NotFoundException
    {
        public CabDriverNotFoundExcpetion(int cabDriverId)
            : base($"This CabDriver with id : {cabDriverId} doesn't exist in the database")
        {
            
        }
    }
}
