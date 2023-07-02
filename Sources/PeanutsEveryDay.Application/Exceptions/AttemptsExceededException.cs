namespace PeanutsEveryDay.Application.Exceptions;

public class AttemptsExceededException : Exception
{
    private const string _message = "The number of attempts to retrieve the page is exceeded";

    public AttemptsExceededException() : base(_message) { }
}
