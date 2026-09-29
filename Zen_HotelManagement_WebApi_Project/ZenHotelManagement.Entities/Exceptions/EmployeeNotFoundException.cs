namespace EmployeePortalWebApi.Entities.Exceptions
{
    public class EmployeeNotFoundException : NotFoundException
    {
        public EmployeeNotFoundException(int EmployeeId)
           : base($"This Employee with id : {EmployeeId} doesn't exist in the database")
        {

        }
    }
}
