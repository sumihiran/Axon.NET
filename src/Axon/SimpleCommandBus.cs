namespace Axon;

using System.Collections.Concurrent;
using Axon.Messaging;

/// <summary>
/// Implementation of the CommandBus that dispatches commands to the handlers subscribed to that specific command's name.
/// </summary>
public class SimpleCommandBus : ICommandBus
{
    private readonly ConcurrentDictionary<string, IMessageHandler<ICommandMessage<object>>> subscriptions = new();
    private readonly IDuplicateCommandHandlerResolver duplicateCommandHandlerResolver;
    private readonly CommandCallback<object, object> defaultCommandCallback;

    // TODO: TransactionManager
    // TODO: MessageMonitor
    // TODO: MessageHandlerInterceptors
    // TODO: DefaultCommandCallback

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleCommandBus"/> class.
    /// </summary>
    /// <param name="builder">The <see cref="Builder"/> used to instantiate a SimpleCommandBus instance.</param>
    protected SimpleCommandBus(Builder builder)
    {
        this.duplicateCommandHandlerResolver = builder.DuplicateCommandHandlerResolver;
        this.defaultCommandCallback = builder.DefaultCommandCallback;
    }

    /// <inheritdoc />
    public async Task<ICommandResultMessage<TResult>> DispatchAsync<TResult>(ICommandMessage<object> command)
        where TResult : class
    {
        var handler = this.FindCommandHandlerFor(command.CommandName);

        if (handler is null || !handler.CanHandle(command))
        {
            throw new NoHandlerForCommandException(
                $"No handler was subscribed to command {command.CommandName}");
        }

        return GenericCommandResultMessage.AsCommandResultMessage<TResult>(
            await this.HandleAsync<TResult>(command, handler)
                .ConfigureAwait(false));
    }

    /// <inheritdoc />
    public async Task DispatchAsync(ICommandMessage<object> command)
    {
        var result = await this.DispatchAsync<object>(command).ConfigureAwait(false);
        await this.defaultCommandCallback(command, result).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<IRegistration> SubscribeAsync(
        string commandName, IMessageHandler<ICommandMessage<object>> handler)
    {
        _ = this.subscriptions.AddOrUpdate(
            commandName,
            _ => handler,
            (_, existingHandler) =>
                this.duplicateCommandHandlerResolver.Resolve(commandName, existingHandler, handler));

        return Task.FromResult<IRegistration>(new Registration(() => this.subscriptions.Remove(commandName, out _)));
    }

    /// <summary>
    /// Invoke the actual handler.
    /// </summary>
    /// <param name="command">The actual command to handle.</param>
    /// <param name="handler">The handler that must be invoked for this command.</param>
    /// <typeparam name="TResult">The type of result expected from the command handler.</typeparam>
    /// <returns>The result of the message handling.</returns>
    protected virtual async Task<IResultMessage<TResult>> HandleAsync<TResult>(
        ICommandMessage<object> command,
        IMessageHandler<ICommandMessage<object>> handler)
        where TResult : class
    {
        IResultMessage<TResult> resultMessage;
        try
        {
            var result = await handler.HandleAsync(command).ConfigureAwait(false);
            if (result is IResultMessage<TResult> checkedResultMessage)
            {
                resultMessage = checkedResultMessage;
            }
            else if (result is IMessage<TResult> message)
            {
                resultMessage = new GenericResultMessage<TResult>(message.Payload, message.MetaData);
            }
            else
            {
                resultMessage = new GenericResultMessage<TResult>((TResult?)result);
            }
        }
        catch (Exception exception)
        {
            resultMessage = GenericResultMessage.AsErrorResultMessage<TResult>(exception);
        }

        return resultMessage;
    }

    private IMessageHandler<ICommandMessage<object>>? FindCommandHandlerFor(string commandName) =>
        this.subscriptions.GetValueOrDefault(commandName);

    /// <summary>
    /// Builder class to instantiate a <see cref="SimpleCommandBus"/>.
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Gets the <see cref="IDuplicateCommandHandlerResolver"/> used to resolves the road to take when a duplicate
        /// command handler is subscribed.
        /// </summary>
        internal IDuplicateCommandHandlerResolver DuplicateCommandHandlerResolver { get; private set; } =
            DuplicateCommandHandlerResolution.SilentOverride;

        /// <summary>
        /// Gets <see cref="CommandCallback{TPayload,TResult}"/> to use when commands are dispatched in a
        /// "fire and forget" method.
        /// </summary>
        internal CommandCallback<object, object> DefaultCommandCallback { get; private set; }
            = NoOpCallback.Instance;

        /// <summary>
        /// Sets the <see cref="IDuplicateCommandHandlerResolver"/> used to resolves the road to take when a duplicate
        /// command handler is subscribed.
        /// </summary>
        /// <param name="duplicateCommandHandlerResolver">
        /// A <see cref="IDuplicateCommandHandlerResolver"/> used to resolves the road to take when a duplicate command
        /// handler is subscribed.
        /// </param>
        /// <returns>The current builder instance, for fluent interfacing.</returns>
        public Builder WithDuplicateCommandHandlerResolver(
            IDuplicateCommandHandlerResolver duplicateCommandHandlerResolver)
        {
            this.DuplicateCommandHandlerResolver = duplicateCommandHandlerResolver;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IDuplicateCommandHandlerResolver"/> used to resolves the road to take when a duplicate
        /// command handler is subscribed.
        /// </summary>
        /// <param name="duplicateCommandHandlerResolver">
        /// A <see cref="DuplicateCommandHandlerResolver"/> used to resolves the road to take when a duplicate command
        /// handler is subscribed.
        /// </param>
        /// <returns>The current builder instance, for fluent interfacing.</returns>
        public Builder WithDuplicateCommandHandlerResolver(
            DuplicateCommandHandlerResolver duplicateCommandHandlerResolver)
        {
            this.DuplicateCommandHandlerResolver = duplicateCommandHandlerResolver.Wrap();
            return this;
        }

        /// <summary>
        /// Sets the callback to use when commands are dispatched in a "fire and forget" method, such as
        /// <see cref="SimpleCommandBus.DispatchAsync"/>. Defaults to <see cref="NoOpCallback"/>.
        /// </summary>
        /// <param name="defaultCommandCallback">
        /// The callback to invoke when no explicit callback is provided for a command.
        /// </param>
        /// <returns>The current Builder instance, for fluent interfacing.</returns>
        public Builder WithDefaultCommandCallback(CommandCallback<object, object> defaultCommandCallback)
        {
            this.DefaultCommandCallback = defaultCommandCallback;
            return this;
        }

        /// <summary>
        /// Initializes A <see cref="SimpleCommandBus"/> as specified through this Builder.
        /// </summary>
        /// <returns>A  <see cref="SimpleCommandBus"/> as specified through this Builder.</returns>
        public SimpleCommandBus Build() => new(this);
    }
}
