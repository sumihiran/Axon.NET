namespace Axon;

using Axon.Messaging;
using Axon.Messaging.ResponseTypes;

public class SimpleQueryBusTest
{
    [Fact]
    public async Task Should_ReturnSubscriptions_When_Subscribed()
    {
        var sut = new SimpleQueryBus();

        await sut.SubscribeAsync<string>(
            "query-1", typeof(string), message => Task.FromResult<object?>(message.Payload));

        Assert.Single(sut.Subscriptions);
        Assert.Single(sut.Subscriptions.Values);

        await sut.SubscribeAsync<string>(
            "query-1", typeof(string), message => Task.FromResult<object?>("prefix:" + message.Payload));

        Assert.Single(sut.Subscriptions);
        Assert.Equal(2, sut.Subscriptions.Values.First().Count);

        await sut.SubscribeAsync<string>(
            "query-2", typeof(string), message => Task.FromResult<object?>("prefix:" + message.Payload));

        Assert.Equal(2, sut.Subscriptions.Count);
    }

    [Fact]
    public async Task SubscribingSameHandlerTwice_InvokedOnce()
    {
        // Arrange
        var sut = new SimpleQueryBus();
        var invocationCount = 0;
        var queryMessage =
            new GenericQueryMessage<string, string>("request", "question", ResponseTypes.InstanceOf<string>());

        Task<object?> Handler(IQueryMessage<object, string> message)
        {
            invocationCount++;
            return Task.FromResult<object?>("9999");
        }

        // Act
        var registration1 = await sut.SubscribeAsync<string>("question", typeof(string), Handler);
        var registration2 = await sut.SubscribeAsync<string>("question", typeof(string), Handler);

        var result = await sut.QueryAsync(queryMessage);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("9999", result.Payload);
        Assert.Equal(1, invocationCount);

        await registration1.CancelAsync();
        await Assert.ThrowsAsync<NoHandlerForQueryException>(() => sut.QueryAsync(queryMessage));
    }

    [Fact]
    public async Task QueryDoesNotArriveAtUnsubscribedHandler()
    {
        // Arrange
        var sut = new SimpleQueryBus();
        var queryName = typeof(string).FullName!;
        var responseType = typeof(string);

        await sut.SubscribeAsync<string>(queryName, responseType, _ => Task.FromResult<object?>("expected"));

        await (await sut.SubscribeAsync<string>(
                queryName, responseType, _ => Task.FromResult<object?>("not expected")))
            .DisposeAsync();

        var queryMessage =
            new GenericQueryMessage<string, string>("request", ResponseTypes.InstanceOf<string>());
        var result = await sut.QueryAsync(queryMessage);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("expected", result.Payload);
    }

    [Fact]
    public async Task ScatterGatherAsync_ShouldReturnsEmptyStream_When_NoHandlersAvailable()
    {
        // Arrange
        var sut = new SimpleQueryBus();
        var queryMessage =
            new GenericQueryMessage<string, string>("request", ResponseTypes.InstanceOf<string>());
        var result = sut.ScatterGatherAsync(queryMessage);

        var results = await result.ToListAsync();
        Assert.Empty(results);
    }

    [Fact]
    public async Task ScatterGatherAsync_Should_ReturnAsyncEnumerable()
    {
        // Arrange
        var sut = new SimpleQueryBus();
        var queryName = typeof(string).FullName!;

        // Act
        await sut.SubscribeAsync<string>(queryName, typeof(string), _ => Task.FromResult<object?>("AB"));
        await sut.SubscribeAsync<string>(queryName, typeof(string), _ => Task.FromResult<object?>("NM"));
        await sut.SubscribeAsync<string>(queryName, typeof(string), _ => Task.FromResult<object?>("XY"));

        var queryMessage =
            new GenericQueryMessage<string, string>("request", ResponseTypes.InstanceOf<string>());
        var stream = sut.ScatterGatherAsync(queryMessage);

        var results = await stream.Select(response => response.Payload).ToListAsync();

        // Assert
        Assert.Equal(3, results.Count);
        Assert.Contains("NM", results);
        Assert.Contains("XY", results);
        Assert.Contains("AB", results);
    }

    [Fact]
    public async Task ScatterGatherAsync_Should_IgnoreNullResponses()
    {
        // Arrange
        var sut = new SimpleQueryBus();
        var queryName = typeof(string).FullName!;

        // Act
        await sut.SubscribeAsync<string>(queryName, typeof(string), _ => Task.FromResult<object?>(null!));
        await sut.SubscribeAsync<string>(queryName, typeof(string), _ => Task.FromResult<object?>("answer"));

        var queryMessage =
            new GenericQueryMessage<string, string>("request", ResponseTypes.InstanceOf<string>());
        var stream = sut.ScatterGatherAsync(queryMessage);

        var results = await stream.Select(response => response.Payload).ToListAsync();

        // Assert
        Assert.Single(results);
        Assert.Contains("answer", results);
    }
}
