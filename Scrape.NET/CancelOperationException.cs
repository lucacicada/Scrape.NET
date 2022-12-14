namespace Scrape.NET;

using System;

/// <summary>
///     The exception that is thrown when is intended to cancel an operation without resulting in an error.
/// </summary>
public sealed class CancelOperationException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="CancelOperationException" /> class.</summary>
    public CancelOperationException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="CancelOperationException" /> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    public CancelOperationException(string? message) : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="CancelOperationException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public CancelOperationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
