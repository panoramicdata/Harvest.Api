namespace Harvest.Test;

/// <summary>
/// Manages test data creation and cleanup for integration tests
/// </summary>
public class TestDataManager(HarvestClient harvestClient, TestSettings testSettings) : IDisposable
{
	private readonly List<long> _createdProjectIds = [];
	private readonly List<long> _createdTaskIds = [];
	private readonly List<long> _createdClientIds = [];
	private readonly List<long> _createdTimeEntryIds = [];
	private bool _disposed;

	/// <summary>
	/// Creates a test client if needed
	/// </summary>
	public async Task<Client> EnsureTestClientAsync(string clientName = "Default Client", bool trackForCleanup = true)
	{
		if (!testSettings.CreateTestData)
		{
			// Return first available client
			var clients = await harvestClient.Clients.ListAllAsync(page: 1, perPage: 1, cancellationToken: CancellationToken.None);
			if (clients.Clients.Count > 0)
			{
				return clients.Clients[0];
			}

			throw new InvalidOperationException("No clients available and test data creation is disabled");
		}

		var fullClientName = $"{testSettings.TestDataPrefix}{clientName}";
		var existingClients = await harvestClient.Clients.ListAllAsync(cancellationToken: CancellationToken.None);
		var existingClient = existingClients.Clients.FirstOrDefault(c => c.Name == fullClientName);

		if (existingClient != null)
		{
			return existingClient;
		}

		try
		{
			// Create new client
			var creationDto = new ClientCreationDto
			{
				Name = fullClientName,
				IsActive = true,
				Currency = "USD"
			};

			var createdClient = await harvestClient.Clients.CreateAsync(creationDto, CancellationToken.None);
			if (trackForCleanup)
			{
				_createdClientIds.Add(createdClient.Id);
			}

			return createdClient;
		}
		catch (Refit.ApiException)
		{
			// Fall back to first available client if creation fails
			var clients = await harvestClient.Clients.ListAllAsync(page: 1, perPage: 1, cancellationToken: CancellationToken.None);
			if (clients.Clients.Count > 0)
			{
				return clients.Clients[0];
			}

			throw new InvalidOperationException("Unable to create or find a client");
		}
	}

	/// <summary>
	/// Creates a test project if needed
	/// </summary>
	public async Task<Project> EnsureTestProjectAsync(string projectName = "Default Project", long? clientId = null, bool trackForCleanup = true)
	{
		if (!testSettings.CreateTestData)
		{
			// Return first available project
			var projects = await harvestClient.Projects.ListAllAsync(page: 1, perPage: 1);
			if (projects.Projects.Count > 0)
			{
				return projects.Projects[0];
			}

			throw new InvalidOperationException("No projects available and test data creation is disabled");
		}

		var fullProjectName = $"{testSettings.TestDataPrefix}{projectName}";
		var existingProjects = await harvestClient.Projects.ListAllAsync();
		var existingProject = existingProjects.Projects.FirstOrDefault(p => p.Name == fullProjectName);

		if (existingProject != null)
		{
			return existingProject;
		}

		try
		{
			// Ensure we have a client
			var client = clientId.HasValue
				? await harvestClient.Clients.GetAsync(clientId.Value, CancellationToken.None)
				: await EnsureTestClientAsync(trackForCleanup: false);

			// Create new project
			var creationDto = new ProjectCreationDto
			{
				Name = fullProjectName,
				ClientId = client.Id,
				BillBy = "project",
				BudgetBy = "none",
				IsBillable = true
			};

			var createdProject = await harvestClient.Projects.CreateAsync(creationDto, CancellationToken.None);
			if (trackForCleanup)
			{
				_createdProjectIds.Add(createdProject.Id);
			}

			return createdProject;
		}
		catch (Refit.ApiException)
		{
			// Fall back to first available project if creation fails
			var projects = await harvestClient.Projects.ListAllAsync(page: 1, perPage: 1);
			if (projects.Projects.Count > 0)
			{
				return projects.Projects[0];
			}

			throw new InvalidOperationException("Unable to create or find a project");
		}
	}

	/// <summary>
	/// Creates a test task if needed
	/// </summary>
	public async System.Threading.Tasks.Task<Models.Task> EnsureTestTaskAsync(string taskName = "Default Task", bool trackForCleanup = true)
	{
		if (!testSettings.CreateTestData)
		{
			// Return first available task
			var tasks = await harvestClient.Tasks.ListAllAsync(page: 1, perPage: 1, cancellationToken: CancellationToken.None);
			if (tasks.Tasks.Count > 0)
			{
				return tasks.Tasks[0];
			}

			throw new InvalidOperationException("No tasks available and test data creation is disabled");
		}

		var fullTaskName = $"{testSettings.TestDataPrefix}{taskName}";
		var existingTasks = await harvestClient.Tasks.ListAllAsync(cancellationToken: CancellationToken.None);
		var existingTask = existingTasks.Tasks.FirstOrDefault(t => t.Name == fullTaskName);

		if (existingTask != null)
		{
			return existingTask;
		}

		try
		{
			// Create new task
			var creationDto = new TaskCreationDto
			{
				Name = fullTaskName,
				BillableByDefault = true,
				DefaultHourlyRate = 50.00m,
				IsDefault = false,
				IsActive = true
			};

			var createdTask = await harvestClient.Tasks.CreateAsync(creationDto, CancellationToken.None);
			if (trackForCleanup)
			{
				_createdTaskIds.Add(createdTask.Id);
			}

			return createdTask;
		}
		catch (Refit.ApiException)
		{
			// Fall back to first available task if creation fails
			var tasks = await harvestClient.Tasks.ListAllAsync(page: 1, perPage: 1, cancellationToken: CancellationToken.None);
			if (tasks.Tasks.Count > 0)
			{
				return tasks.Tasks[0];
			}

			throw new InvalidOperationException("Unable to create or find a task");
		}
	}

	/// <summary>
	/// Creates a test time entry
	/// </summary>
	public async Task<TimeEntry> CreateTestTimeEntryAsync(string notes = "Test time entry", decimal hours = 1.0m, bool trackForCleanup = true)
	{
		var user = await harvestClient.Users.GetMeAsync(CancellationToken.None);
		var project = await EnsureTestProjectAsync();
		var task = await EnsureTestTaskAsync();

		var creationDto = new TimeEntryCreationDto
		{
			UserId = user.Id,
			ProjectId = project.Id,
			TaskId = task.Id,
			SpentDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
			Hours = hours,
			Notes = $"{testSettings.TestDataPrefix}{notes}"
		};

		var timeEntry = await harvestClient.TimeEntries.CreateAsync(creationDto, CancellationToken.None);
		if (trackForCleanup)
		{
			_createdTimeEntryIds.Add(timeEntry.Id);
		}

		return timeEntry;
	}

	/// <summary>
	/// Adds a time entry ID to the tracking list for cleanup
	/// </summary>
	public void TrackTimeEntry(long timeEntryId) => _createdTimeEntryIds.Add(timeEntryId);

	/// <summary>
	/// Removes a time entry ID from the tracking list
	/// </summary>
	public void UntrackTimeEntry(long timeEntryId) => _createdTimeEntryIds.Remove(timeEntryId);

	/// <summary>
	/// Adds a project ID to the tracking list for cleanup
	/// </summary>
	public void TrackProject(long projectId) => _createdProjectIds.Add(projectId);

	/// <summary>
	/// Removes a project ID from the tracking list
	/// </summary>
	public void UntrackProject(long projectId) => _createdProjectIds.Remove(projectId);

	/// <summary>
	/// Adds a task ID to the tracking list for cleanup
	/// </summary>
	public void TrackTask(long taskId) => _createdTaskIds.Add(taskId);

	/// <summary>
	/// Removes a task ID from the tracking list
	/// </summary>
	public void UntrackTask(long taskId) => _createdTaskIds.Remove(taskId);

	/// <summary>
	/// Adds a client ID to the tracking list for cleanup
	/// </summary>
	public void TrackClient(long clientId) => _createdClientIds.Add(clientId);

	/// <summary>
	/// Removes a client ID from the tracking list
	/// </summary>
	public void UntrackClient(long clientId) => _createdClientIds.Remove(clientId);

	/// <summary>
	/// Cleans up all created test data
	/// </summary>
	public async System.Threading.Tasks.Task CleanupAsync()
	{
		if (!testSettings.CleanupTestData)
		{
			return;
		}

		// Clean up in reverse order to avoid dependency issues
		foreach (var timeEntryId in _createdTimeEntryIds)
		{
			try
			{
				await harvestClient.TimeEntries.DeleteAsync(timeEntryId, CancellationToken.None);
			}
			catch (Refit.ApiException)
			{
				// Ignore cleanup failures
			}
		}

		foreach (var projectId in _createdProjectIds)
		{
			try
			{
				await harvestClient.Projects.DeleteAsync(projectId, CancellationToken.None);
			}
			catch (Refit.ApiException)
			{
				// Ignore cleanup failures
			}
		}

		foreach (var taskId in _createdTaskIds)
		{
			try
			{
				await harvestClient.Tasks.DeleteAsync(taskId, CancellationToken.None);
			}
			catch (Refit.ApiException)
			{
				// Ignore cleanup failures
			}
		}

		foreach (var clientId in _createdClientIds)
		{
			try
			{
				await harvestClient.Clients.DeleteAsync(clientId, CancellationToken.None);
			}
			catch (Refit.ApiException)
			{
				// Ignore cleanup failures
			}
		}
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			// Note: CleanupAsync should be called explicitly by tests
			// We don't call it here because it's async and Dispose should be synchronous
			_disposed = true;
		}
	}
}