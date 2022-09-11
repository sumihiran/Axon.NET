namespace Axon.Messaging;

public class MetaDataTests
{
    [Fact]
    public void MetaDataIsImmutable()
    {
        // Arrange
        var initialMetadata = MetaData.With("key1", "value1");

        // Act
        var metaDataWithMoreItems = initialMetadata.And("key2", "value2");

        // Assert
        Assert.False(ReferenceEquals(initialMetadata, metaDataWithMoreItems));
        Assert.Single(initialMetadata);
        Assert.Equal(2, metaDataWithMoreItems.Count);
    }
}
