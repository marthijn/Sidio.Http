namespace Sidio.Http;

/// <summary>
/// Represents an exception that is thrown when an HTTP request is invalid.
/// This exception is typically thrown during the validation of an <see cref="IHttpRequest"/> instance,
/// indicating that the request does not meet the required criteria for a valid HTTP request.
/// </summary>
public sealed class InvalidHttpRequestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidHttpRequestException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public InvalidHttpRequestException(string message) : base(message)
    {
    }
}