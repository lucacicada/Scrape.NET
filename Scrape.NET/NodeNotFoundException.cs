namespace Scrape.NET;

using System;
using System.Runtime.Serialization;

/// <summary>
///     Represents an error that occur when selecting a node that does not exists.
/// </summary>
public class NodeNotFoundException : NodeException
{
    /// <summary>
    ///     The selector that have caused the exception.
    /// </summary>
    public string? NodeSelector { get; }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class.</summary>
    public NodeNotFoundException()
    {

    }

    /// <summary>Initializes a new instance of the <see cref="AttributeNotFoundException" /> class with serialized data.</summary>
    /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="info" /> is <see langword="null" />.</exception>
    /// <exception cref="SerializationException">The class name is <see langword="null" /> or <see cref="System.Exception.HResult" /> is zero (0).</exception>
    protected NodeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        NodeSelector = info.GetString("AttributeNotFound_NodeSelector");
    }

    /// <summary>Initializes a new instance of the <see cref="NodeNotFoundException" /> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    public NodeNotFoundException(string? message) : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="NodeNotFoundException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public NodeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="NodeNotFoundException" /> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="nodeSelector">The selector that have caused the exception.</param>
    public NodeNotFoundException(string? message, string? nodeSelector) : base(message)
    {
        NodeSelector = nodeSelector;
    }

    /// <summary>Initializes a new instance of the <see cref="NodeNotFoundException" /> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="nodeSelector">The selector that have caused the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
    public NodeNotFoundException(string? message, string? nodeSelector, Exception? innerException) : base(message, innerException)
    {
        NodeSelector = nodeSelector;
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("AttributeNotFound_NodeSelector", NodeSelector, typeof(string));
    }
}
