using Xunit;
using Xunit.Abstractions;

namespace Harvest.Test
{
	public class ClientTests : HarvestTest
	{
		public ClientTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
		{
		}

		[Fact]
		public async Task GetAllClients()
		{
			await HarvestClient.Clients.ListAllAsync().ConfigureAwait(false);
		}
	}
}
