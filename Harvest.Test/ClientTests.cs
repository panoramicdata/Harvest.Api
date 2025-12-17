namespace Harvest.Test;

public class ClientTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async Task GetAllClients()
	{
		var result = await HarvestClient.Clients.ListAllAsync(cancellationToken: CancellationToken);
		result.Should().NotBeNull();
	}
}
