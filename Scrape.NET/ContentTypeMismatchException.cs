namespace Scrape.NET;

using System;
using System.Runtime.Serialization;

/// <summary>
///     The exception that is thrown when the "Content-Type" header does not match the expected value.
/// </summary>
public class ContentTypeMismatchException : Exception
{
    private static string CreateMessage(string? message, string? expectedType, string? receivedType)
    {
        return message ?? $"Invalid content type, epxected '{expectedType}', found: '{receivedType}'";
    }

    /// <summary>
    ///     The expected "Content-Type" header.
    /// </summary>
    public string? ExpectedType { get; }

    /// <summary>
    ///     The actual "Content-Type" header.
    /// </summary>
    public string? ReceivedType { get; }

    /// <summary>Initializes a new instance of the <see cref="ContentTypeMismatchException" /> class.</summary>
    public ContentTypeMismatchException()
    {
    }

    ///// <summary>Initializes a new instance of the <see cref="ContentTypeMismatchException" /> class.</summary>
    ///// <param name="expectedType">The expected "Content-Type" header.</param>
    ///// <param name="receivedType">The actual "Content-Type" header.</param>
    //public ContentTypeMismatchException(string expectedType, string? receivedType)
    //    : base(CreateMessage(null, expectedType, receivedType))
    //{
    //    ExpectedType = expectedType;
    //    ReceivedType = receivedType;
    //}

    /// <summary>Initializes a new instance of the <see cref="ContentTypeMismatchException" /> class with serialized data.</summary>
    /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
    /// <exception cref="ArgumentNullException"><paramref name="info" /> is <see langword="null" />.</exception>
    /// <exception cref="SerializationException">The class name is <see langword="null" /> or <see cref="Exception.HResult" /> is zero (0).</exception>
    protected ContentTypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        ExpectedType = info.GetString("ContentTypeMismatchException_ExpectedType");
        ReceivedType = info.GetString("ContentTypeMismatchException_ReceivedType");
    }

    /// <summary>Initializes a new instance of the <see cref="ContentTypeMismatchException" /> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="expectedType">The expected "Content-Type" header.</param>
    /// <param name="receivedType">The actual "Content-Type" header.</param>
    public ContentTypeMismatchException(string? message, string? expectedType, string? receivedType)
        : base(CreateMessage(message, expectedType, receivedType))
    {
        ExpectedType = expectedType;
        ReceivedType = receivedType;
    }

    /// <summary>Initializes a new instance of the <see cref="ContentTypeMismatchException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="expectedType">The expected "Content-Type" header.</param>
    /// <param name="receivedType">The actual "Content-Type" header.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public ContentTypeMismatchException(string? message, string? expectedType, string? receivedType, Exception innerException)
        : base(CreateMessage(message, expectedType, receivedType), innerException)
    {
        ExpectedType = expectedType;
        ReceivedType = receivedType;
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("ContentTypeMismatchException_ExpectedType", ExpectedType, typeof(string));
        info.AddValue("ContentTypeMismatchException_ReceivedType", ReceivedType, typeof(string));
    }
}
