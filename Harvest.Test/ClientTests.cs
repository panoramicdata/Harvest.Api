namespace Harvest.Test;

public class ClientTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListAllClients()
	{
		var clientsContainer = await HarvestClient.Clients.ListAsync(new() { Page = 1, PerPage = 10 }, CancellationToken);

		clientsContainer.Should().NotBeNull();
		clientsContainer.Clients.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task GetClient()
	{
		var clientsContainer = await HarvestClient.Clients.ListAsync(new() { Page = 1, PerPage = 1 }, CancellationToken);
		if (FirstOrLog(clientsContainer.Clients, "clients") is not { } first)
		{
			return;
		}

		var client = await HarvestClient.Clients.GetAsync(first.Id, CancellationToken);

		client.Should().NotBeNull();
		client.Id.Should().Be(first.Id);
		client.Name.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public System.Threading.Tasks.Task CreateUpdateDeleteClient()
		=> RunCrudScenarioAsync(async () =>
		{
			var createdClient = await CreateTestClientAsync("CRUD", address: $"{Prefix}123 Test Street");
			createdClient.Id.Should().BePositive();
			createdClient.Name.Should().StartWith($"{Prefix}CRUD Test Client");
			createdClient.IsActive.Should().BeTrue();

			var retrievedClient = await HarvestClient.Clients.GetAsync(createdClient.Id, CancellationToken);
			retrievedClient.Id.Should().Be(createdClient.Id);
			retrievedClient.Name.Should().Be(createdClient.Name);
			Log.Retrieved(Logger, "client", retrievedClient.Id);

			var updateDto = new ClientPatchDto
			{
				Name = $"{createdClient.Name} - Updated",
				Address = $"{Prefix}456 Updated Street"
			};
			var updatedClient = await HarvestClient.Clients.UpdateAsync(createdClient.Id, updateDto, CancellationToken);
			updatedClient.Id.Should().Be(createdClient.Id);
			updatedClient.Name.Should().Be(updateDto.Name);
			updatedClient.Address.Should().Be(updateDto.Address);
			Log.Updated(Logger, "client", updatedClient.Id);

			await DeleteTrackedAsync("client", createdClient.Id);
		});
}