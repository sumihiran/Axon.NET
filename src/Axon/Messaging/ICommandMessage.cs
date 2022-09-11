namespace Axon.Messaging;

/// <summary>
/// Represents a Message carrying a command as its payload. These messages carry an intention to change application
/// state.
/// </summary>
/// <typeparam name="TPayload">The type of payload contained in the message.</typeparam>
public interface ICommandMessage<out TPayload> : IMessage<TPayload>
    where TPayload : class
{
    /// <summary>
    /// Gets the name of the command to execute. This is an indication of what should be done, using the payload as
    /// parameter.
    /// </summary>
    string CommandName { get; }
}
