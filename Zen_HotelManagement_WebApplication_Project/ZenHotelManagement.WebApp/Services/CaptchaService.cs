using ZenHotelManagement.WebApp.Services.Contracts;

public class CaptchaService : ICaptchaService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CaptchaService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerateCaptchaCode()
    {
        var captchaCode = new Random().Next(1000, 9999).ToString();
        _httpContextAccessor.HttpContext.Session.SetString("CaptchaCode", captchaCode);
        return captchaCode;
    }

    public bool ValidateCaptchaCode(string userInputCaptcha)
    {
        var storedCaptchaCode = _httpContextAccessor.HttpContext.Session.GetString("CaptchaCode");
        return storedCaptchaCode == userInputCaptcha;
    }
}