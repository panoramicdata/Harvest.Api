using Xunit;
using Xunit.Abstractions;

namespace Harvest.Test
{
	public class UserTests : HarvestTest
	{
		public UserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public async Task GetMe()
		{
			var harvestUser = await HarvestClient.Users.GetMeAsync().ConfigureAwait(false);
			Assert.NotNull(harvestUser);
		}
	}
}
