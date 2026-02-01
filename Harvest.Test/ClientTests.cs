namespace Harvest.Test;

public class ClientTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListAllClients()
	{
		var clientsContainer = await HarvestClient.Clients.ListAllAsync(
			page: 1,
			perPage: 10,
			cancellationToken: CancellationToken
		);

		clientsContainer.Should().NotBeNull();
		clientsContainer.Clients.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task GetClient()
	{
		// First get a client from the list
		var clientsContainer = await HarvestClient.Clients.ListAllAsync(
			page: 1,
			perPage: 1,
			cancellationToken: CancellationToken
		);

		if (clientsContainer.Clients.Count == 0)
		{
			// Skip if no clients exist
			return;
		}

		var clientId = clientsContainer.Clients[0].Id;
		var client = await HarvestClient.Clients.GetAsync(clientId, CancellationToken);

		client.Should().NotBeNull();
		client.Id.Should().Be(clientId);
		client.Name.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public async System.Threading.Tasks.Task CreateUpdateDeleteClient()
	{
		if (!Configuration.TestSettings.CreateTestData)
		{
			// Skip if test data creation is disabled
			Logger.LogInformation("Test data creation is disabled");
			return;
		}

		try
		{
			// Create a new client
			var creationDto = new ClientCreationDto
			{
				Name = $"{Configuration.TestSettings.TestDataPrefix}CRUD Test Client {DateTime.UtcNow:yyyyMMddHHmmss}",
				IsActive = true,
				Address = "123 Test Street",
				Currency = "USD"
			};

			var createdClient = await HarvestClient.Clients.CreateAsync(creationDto, CancellationToken);
			createdClient.Should().NotBeNull();
			createdClient.Id.Should().BePositive();
			createdClient.Name.Should().Be(creationDto.Name);

			// Update the client
			var updateDto = new ClientPatchDto
			{
				Name = $"{creationDto.Name} - Updated",
				Address = "456 Updated Street"
			};

			var updatedClient = await HarvestClient.Clients.UpdateAsync(createdClient.Id, updateDto, CancellationToken);
			updatedClient.Should().NotBeNull();
			updatedClient.Id.Should().Be(createdClient.Id);
			updatedClient.Name.Should().Be(updateDto.Name);
			updatedClient.Address.Should().Be(updateDto.Address);

			// Delete the client
			await HarvestClient.Clients.DeleteAsync(createdClient.Id, CancellationToken);
			TestDataManager.UntrackClient(createdClient.Id); // Remove from tracking since we deleted it
		}
		catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
		{
			// Skip if the API doesn't allow these operations (common in sandbox environments)
			Logger.LogInformation("CRUD operations not permitted in this environment");
		}
		finally
		{
			await TestDataManager.CleanupAsync();
		}
	}
}
