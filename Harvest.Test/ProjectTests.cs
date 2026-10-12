namespace Harvest.Test;

public class ProjectTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListAllProjects()
	{
		var projectsContainer = await HarvestClient.Projects.ListAsync(new() { Page = 1, PerPage = 10 }, CancellationToken);

		projectsContainer.Should().NotBeNull();
		projectsContainer.Projects.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task GetProject()
	{
		var projectsContainer = await HarvestClient.Projects.ListAsync(new() { Page = 1, PerPage = 1 }, CancellationToken);
		if (FirstOrLog(projectsContainer.Projects, "projects") is not { } first)
		{
			return;
		}

		var project = await HarvestClient.Projects.GetAsync(first.Id, CancellationToken);

		project.Should().NotBeNull();
		project.Id.Should().Be(first.Id);
		project.Name.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public System.Threading.Tasks.Task CreateUpdateDeleteProject()
		=> RunCrudScenarioAsync(async () =>
		{
			var client = await CreateTestClientAsync("Project", address: null);
			var createdProject = await CreateTestProjectAsync(client.Id, "CRUD", notes: $"{Prefix}Created via CRUD test");
			createdProject.Id.Should().BePositive();
			createdProject.Name.Should().StartWith($"{Prefix}CRUD Test Project");
			createdProject.IsBillable.Should().BeTrue();

			var retrievedProject = await HarvestClient.Projects.GetAsync(createdProject.Id, CancellationToken);
			retrievedProject.Id.Should().Be(createdProject.Id);
			retrievedProject.Name.Should().Be(createdProject.Name);
			Log.Retrieved(Logger, "project", retrievedProject.Id);

			var updateDto = new ProjectPatchDto
			{
				Name = $"{createdProject.Name} - Updated",
				Notes = $"{Prefix}Updated via CRUD test"
			};
			var updatedProject = await HarvestClient.Projects.UpdateAsync(createdProject.Id, updateDto, CancellationToken);
			updatedProject.Id.Should().Be(createdProject.Id);
			updatedProject.Name.Should().Be(updateDto.Name);
			updatedProject.Notes.Should().Be(updateDto.Notes);
			Log.Updated(Logger, "project", updatedProject.Id);

			await DeleteTrackedAsync("project", createdProject.Id);
		});
}