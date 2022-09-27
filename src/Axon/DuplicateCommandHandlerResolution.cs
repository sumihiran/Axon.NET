namespace Axon;

/// <summary>
/// Default set of reasonable <see cref="IDuplicateCommandHandlerResolver"/> implementations. Can be used to configure
/// how a CommandBus should react upon a duplicate subscription of a command handling function.
/// </summary>
public static class DuplicateCommandHandlerResolution
{
    /// <summary>
    /// A <see cref="IDuplicateCommandHandlerResolver"/> implementation that allows handlers to silently override
    /// previous registered handlers for the same command.
    /// </summary>
    /// <returns>An instance that silently accepts duplicates.</returns>
    public static readonly IDuplicateCommandHandlerResolver SilentOverride = SilentOverrideCallback.Wrap();

    private static DuplicateCommandHandlerResolver SilentOverrideCallback => (commandName, first, second) => second;
}
