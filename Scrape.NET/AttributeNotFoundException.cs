namespace Scrape.NET;

using System;
using System.Runtime.Serialization;

/// <summary>
///     Represents an error that occur when selecting an attribute that does not exists.
/// </summary>
public class AttributeNotFoundException : NodeException
{
    /// <summary>
    ///     The name of the attribute that is missing.
    /// </summary>
    public string? AttributeName { get; }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class.</summary>
    public AttributeNotFoundException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with serialized data.</summary>
    /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="info" /> is <see langword="null" />.</exception>
    /// <exception cref="SerializationException">The class name is <see langword="null" /> or <see cref="Exception.HResult" /> is zero (0).</exception>
    protected AttributeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        AttributeName = info.GetString("AttributeNotFound_AttributeName");
    }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    public AttributeNotFoundException(string? message) : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public AttributeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="attributeName">The name of the attribute that is missing.</param>
    public AttributeNotFoundException(string? message, string? attributeName) : base(message)
    {
        AttributeName = attributeName;
    }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="attributeName">The name of the attribute that is missing.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public AttributeNotFoundException(string? message, string? attributeName, Exception? innerException) : base(message, innerException)
    {
        AttributeName = attributeName;
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("AttributeNotFound_AttributeName", AttributeName, typeof(string));
    }
}
