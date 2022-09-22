namespace Axon;

using Axon.Messaging;
using Axon.Messaging.ResponseTypes;

public class SimpleQueryBusTest
{
    [Fact]
    public async Task Should_ReturnSubscriptions_When_Subscribed()
    {
        var sut = new SimpleQueryBus();
        var responseType = typeof(string);

        await sut.SubscribeAsync("query-1", responseType, QueryHandler<string, string>(payload => payload.ToString()));

        Assert.Single(sut.Subscriptions);
        Assert.Single(sut.Subscriptions.Values);

        await sut.SubscribeAsync("query-1", QueryHandler<string, string>(payload => "prefix:" + payload.ToString()));

        Assert.Single(sut.Subscriptions);
        Assert.Equal(2, sut.Subscriptions.Values.First().Count);

        await sut.SubscribeAsync<IQueryMessage<string, string>, string>(
            "query-2",
            typeof(string),
            QueryHandler<string, string>(payload => "prefix:" + payload.ToString()));

        Assert.Equal(2, sut.Subscriptions.Count);
    }

    [Fact]
    public async Task SubscribingSameHandlerTwice_InvokedOnce()
    {
        // Arrange
        IQueryBus sut = new SimpleQueryBus();
        var invocationCount = 0;
        var handler = QueryHandler<string, object>(_ =>
        {
            invocationCount++;
            return 9999;
        });
        var queryMessage =
            new GenericQueryMessage<string, object>("request", "question", ResponseTypes.InstanceOf<object>());

        // Act
        var registration = await sut.SubscribeAsync("question", handler);
        var result = await sut.QueryAsync(queryMessage);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(9999, result.Payload);
        Assert.Equal(1, invocationCount);

        await registration.DisposeAsync();
        await Assert.ThrowsAsync<NoHandlerForQueryException>(() => sut.QueryAsync(queryMessage));
    }

    [Fact]
    public async Task QueryDoesNotArriveAtUnsubscribedHandler()
    {
        // Arrange
        var sut = new SimpleQueryBus();
        await sut.SubscribeAsync(typeof(string).FullName!, QueryHandler<string, string>(_ => "expected"));

        await (await sut.SubscribeAsync(typeof(string).FullName!, QueryHandler<string, string>(_ => "not expected")))
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
        await sut.SubscribeAsync(queryName, QueryHandler<string, string>(_ => "AB"));
        await sut.SubscribeAsync(queryName, QueryHandler<string, string>(_ => "NM"));
        await sut.SubscribeAsync(queryName, QueryHandler<string, string>(_ => "XY"));

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
        await sut.SubscribeAsync(queryName, QueryHandler<string, string>(_ => null!));
        await sut.SubscribeAsync(queryName, QueryHandler<string, string>(_ => "answer"));

        var queryMessage =
            new GenericQueryMessage<string, string>("request", ResponseTypes.InstanceOf<string>());
        var stream = sut.ScatterGatherAsync(queryMessage);

        var results = await stream.Select(response => response.Payload).ToListAsync();

        // Assert
        Assert.Single(results);
        Assert.Contains("answer", results);
    }

    private static MessageHandler<IQueryMessage<TPayload, TResponse>> QueryHandler<TPayload, TResponse>(
        Func<TPayload, TResponse> call)
        where TPayload : class
        where TResponse : class
        => new DelegatingQueryHandler<TPayload, TResponse>(call);

    private class DelegatingQueryHandler<TPayload, TResponse> : MessageHandler<IQueryMessage<TPayload, TResponse>>
        where TPayload : class
        where TResponse : class
    {
        private readonly Func<TPayload, TResponse> call;

        public DelegatingQueryHandler(Func<TPayload, TResponse> call) => this.call = call;

        /// <inheritdoc />
        public override Task<object?> HandleAsync(IQueryMessage<TPayload, TResponse> message) =>
            Task.FromResult((object?)this.call.Invoke(message.Payload!));
    }
}
