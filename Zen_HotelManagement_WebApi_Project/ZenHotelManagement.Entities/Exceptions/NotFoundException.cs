namespace EmployeePortalWebApi.Entities.Exceptions
{
    public abstract class NotFoundException : Exception
    {
        //I dont want to create object of this class explicitly that why it is declared as protected 
        protected NotFoundException(string message) : base(message)
        {

        }
       
    }
}
