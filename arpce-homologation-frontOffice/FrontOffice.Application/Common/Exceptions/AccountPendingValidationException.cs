namespace FrontOffice.Application.Common.Exceptions;

public class AccountPendingValidationException : Exception
{
    public AccountPendingValidationException(string message) : base(message)
    {
    }
}