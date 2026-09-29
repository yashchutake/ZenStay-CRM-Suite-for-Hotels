namespace ZenHotelManagement.WebApp.Services.Contracts
{
    public interface ICaptchaService
    {
        string GenerateCaptchaCode();
        bool ValidateCaptchaCode(string userInputCaptcha);

    }
}
