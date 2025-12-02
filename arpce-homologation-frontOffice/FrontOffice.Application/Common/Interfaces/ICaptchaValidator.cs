namespace FrontOffice.Application.Common.Interfaces;

public interface ICaptchaValidator
{
    Task<bool> ValidateAsync(string captchaToken);
}