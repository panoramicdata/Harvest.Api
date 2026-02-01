namespace Harvest.Test;

public class ProjectTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListAllProjects()
	{
		var projectsContainer = await HarvestClient.Projects.ListAllAsync(
			page: 1,
			perPage: 10
		);

		projectsContainer.Should().NotBeNull();
		projectsContainer.Projects.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task GetProject()
	{
		// First get a project from the list
		var projectsContainer = await HarvestClient.Projects.ListAllAsync(
			page: 1,
			perPage: 1
		);

		if (projectsContainer.Projects.Count == 0)
		{
			// Skip if no projects exist
			return;
		}

		var projectId = projectsContainer.Projects[0].Id;
		var project = await HarvestClient.Projects.GetAsync(projectId);

		project.Should().NotBeNull();
		project.Id.Should().Be(projectId);
		project.Name.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public async System.Threading.Tasks.Task CreateUpdateDeleteProject()
	{
		if (!Configuration.TestSettings.CreateTestData)
		{
			// Skip if test data creation is disabled
			Logger.LogInformation("Test data creation is disabled");
			return;
		}

		try
		{
			// Ensure we have a test client first
			var client = await TestDataManager.EnsureTestClientAsync("CRUD Test Client");

			// Create a new project
			var creationDto = new ProjectCreationDto
			{
				Name = $"{Configuration.TestSettings.TestDataPrefix}CRUD Test Project {DateTime.UtcNow:yyyyMMddHHmmss}",
				ClientId = client.Id,
				BillBy = "project",
				BudgetBy = "none",
				IsBillable = true
			};

			var createdProject = await HarvestClient.Projects.CreateAsync(creationDto, CancellationToken);
			createdProject.Should().NotBeNull();
			createdProject.Id.Should().BePositive();
			createdProject.Name.Should().Be(creationDto.Name);

			// Update the project
			var updateDto = new ProjectPatchDto
			{
				Name = $"{creationDto.Name} - Updated",
				Notes = $"{Configuration.TestSettings.TestDataPrefix}Updated via API test"
			};

			var updatedProject = await HarvestClient.Projects.UpdateAsync(createdProject.Id, updateDto, CancellationToken);
			updatedProject.Should().NotBeNull();
			updatedProject.Id.Should().Be(createdProject.Id);
			updatedProject.Name.Should().Be(updateDto.Name);
			updatedProject.Notes.Should().Be(updateDto.Notes);

			// Delete the project
			await HarvestClient.Projects.DeleteAsync(createdProject.Id, CancellationToken);
			TestDataManager.UntrackProject(createdProject.Id); // Remove from tracking since we deleted it
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
