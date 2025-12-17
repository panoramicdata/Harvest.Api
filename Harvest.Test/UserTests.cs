namespace Harvest.Test;

public class UserTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async Task GetMe()
	{
		var harvestUser = await HarvestClient.Users.GetMeAsync(CancellationToken);
		harvestUser.Should().NotBeNull();
	}
}
