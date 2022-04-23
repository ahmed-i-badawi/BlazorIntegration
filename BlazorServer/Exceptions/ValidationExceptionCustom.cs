
namespace BlazorServer.Exceptions;

public class ValidationExceptionCustom : Exception
{

    public ValidationExceptionCustom(string message)
    {
        Error = message;
    }

    public string Error { get; }
}
