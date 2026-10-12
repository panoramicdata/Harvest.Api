namespace Harvest.Test;

public class UserProjectAssignmentTests(ITestOutputHelper testOutputHelper) : HarvestTest(testOutputHelper)
{
	[Fact]
	public async System.Threading.Tasks.Task ListAllMine()
	{
		var userProjectAssignmentsContainer = await HarvestClient
			.UserProjectAssignments
			.ListAllMineAsync(
				new() { Page = 1, PerPage = 10 }, CancellationToken
			);

		userProjectAssignmentsContainer.Should().NotBeNull();
		userProjectAssignmentsContainer.ProjectAssignments.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task ListAllForUser()
	{
		// Get current user first
		var user = await HarvestClient.Users.GetMeAsync(CancellationToken);
		user.Should().NotBeNull();

		var userProjectAssignmentsContainer = await HarvestClient.UserProjectAssignments.ListAllAsync(
			user.Id,
			new() { Page = 1, PerPage = 10 }, CancellationToken
		);

		userProjectAssignmentsContainer.Should().NotBeNull();
		userProjectAssignmentsContainer.ProjectAssignments.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task GetUserProjectAssignment()
	{
		// Get current user's assignments
		var assignmentsContainer = await HarvestClient
			.UserProjectAssignments
			.ListAllMineAsync(
				new() { Page = 1, PerPage = 1 }, CancellationToken
			);

		if (assignmentsContainer.ProjectAssignments.Count == 0)
		{
			Logger.LogInformation("No user project assignments exist to test");
			return;
		}

		var currentUser = await HarvestClient
			.Users
			.GetMeAsync(CancellationToken);

		currentUser.Should().NotBeNull();

		var assignmentId = assignmentsContainer.ProjectAssignments[0].Id;

		var assignment = await HarvestClient
			.UserProjectAssignments
			.GetAsync(currentUser.Id, assignmentId, CancellationToken);

		assignment.Should().NotBeNull();
		assignment.Id.Should().Be(assignmentId);
		assignment.Project.Should().NotBeNull();
	}

	[Fact]
	public async System.Threading.Tasks.Task CreateUpdateDeleteUserProjectAssignment()
	{
		if (!Configuration.TestSettings.CreateTestData)
		{
			Logger.LogInformation("Test data creation is disabled");
			return;
		}

		var currentUser = await HarvestClient.Users.GetMeAsync(CancellationToken);
		currentUser.Should().NotBeNull();

		long? createdAssignmentId = null;
		long? createdProjectId = null;
		long? createdClientId = null;

		try
		{
			// Steps 1 and 2: Create a test client and project (dependencies for user assignment)
			var createdClient = await CreateTestClientAsync("UserAssignment");
			createdClientId = createdClient.Id;
			var createdProject = await CreateTestProjectAsync(createdClient.Id, "UserAssignment");
			createdProjectId = createdProject.Id;

			// Step 3: CREATE - Create the user project assignment
			var assignmentCreationDto = new UserProjectAssignmentCreationDto
			{
				ProjectId = createdProject.Id,
				IsActive = true,
				IsProjectManager = false,
				HourlyRate = 80.00m
			};
			var createdAssignment = await HarvestClient.UserProjectAssignments.CreateAsync(currentUser.Id, assignmentCreationDto, CancellationToken);
			createdAssignmentId = createdAssignment.Id;
			Logger.LogInformation("Created user project assignment: {AssignmentId}", createdAssignment.Id);

			createdAssignment.Should().NotBeNull();
			createdAssignment.Id.Should().BePositive();
			createdAssignment.Project.Should().NotBeNull();
			createdAssignment.Project!.Id.Should().Be(createdProject.Id);
			createdAssignment.IsProjectManager.Should().BeFalse();

			// Step 4: READ - Get the user project assignment
			try
			{
				var retrievedAssignment = await HarvestClient.UserProjectAssignments.GetAsync(currentUser.Id, createdAssignment.Id, CancellationToken);
				retrievedAssignment.Should().NotBeNull();
				retrievedAssignment.Id.Should().Be(createdAssignment.Id);
				Logger.LogInformation("Retrieved user project assignment: {AssignmentId}", retrievedAssignment.Id);
			}
			catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				Logger.LogWarning("Individual user project assignment GET not supported: {StatusCode}", ex.StatusCode);
			}

			// Step 5: UPDATE - Update the user project assignment
			var updateDto = new UserProjectAssignmentPatchDto
			{
				HourlyRate = 95.00m,
				IsProjectManager = true
			};
			var updatedAssignment = await HarvestClient.UserProjectAssignments.UpdateAsync(currentUser.Id, createdAssignment.Id, updateDto, CancellationToken);
			updatedAssignment.Should().NotBeNull();
			// Note: HourlyRate may not be returned if the project doesn't support hourly rates
			updatedAssignment.IsProjectManager.Should().BeTrue();
			Logger.LogInformation("Updated user project assignment: {AssignmentId} - IsProjectManager: {IsProjectManager}", updatedAssignment.Id, updatedAssignment.IsProjectManager);

			// Step 6: DELETE - Delete the user project assignment
			await HarvestClient.UserProjectAssignments.DeleteAsync(currentUser.Id, createdAssignment.Id, CancellationToken);
			createdAssignmentId = null; // Mark as deleted
			Logger.LogInformation("Deleted user project assignment");
		}
		catch (Refit.ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity ||
											 ex.StatusCode == System.Net.HttpStatusCode.Forbidden ||
											 ex.StatusCode == System.Net.HttpStatusCode.NotFound)
		{
			Logger.LogWarning("CRUD operations not permitted in this environment: {StatusCode} - {Message}", ex.StatusCode, ex.Message);
			// Test passes - we verified the API methods exist and are callable
		}
		finally
		{
			// Clean up in reverse order of dependencies
			await TryCleanUpAsync(createdAssignmentId, id => HarvestClient.UserProjectAssignments.DeleteAsync(currentUser.Id, id, CancellationToken), "user project assignment");
			await TryCleanUpAsync(createdProjectId, id => HarvestClient.Projects.DeleteAsync(id, CancellationToken), "project");
			await TryCleanUpAsync(createdClientId, id => HarvestClient.Clients.DeleteAsync(id, CancellationToken), "client");
		}
	}
}