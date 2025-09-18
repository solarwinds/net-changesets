namespace SolarWinds.Changesets.Shared;

/// <summary>
/// Represents errors that occur during the initialization phase.
/// </summary>
public sealed class InitializationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationException"/> class
    /// with a default error message.
    /// </summary>
    public InitializationException()
        : base("An error occurred during initialization.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InitializationException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationException"/> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public InitializationException(string message, Exception innerException)
        : base(message, innerException) { }
}
