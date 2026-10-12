using Harvest.Test.Logging;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Harvest.Test;

/// <summary>
/// The base of the live tests. Credentials come from user secrets (<c>AccountId</c> and <c>AccessToken</c>) or, failing
/// that, an optional <c>appsettings.json</c>. Everything a test creates through the helpers here is tracked and deleted,
/// newest first, when the test finishes.
/// </summary>
public abstract class HarvestTest : IAsyncLifetime
{
	private readonly List<TrackedResource> _trackedResources = [];

	protected ILogger Logger { get; }

	protected HarvestClient HarvestClient { get; }

	protected Configuration Configuration { get; }

	/// <summary>The prefix that marks test data, so it can be recognised and cleaned up by hand if need be.</summary>
	protected string Prefix => Configuration.TestSettings.TestDataPrefix;

	protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

	protected HarvestTest(ITestOutputHelper testOutputHelper)
	{
		Logger = new LoggerFactory()
			.AddXunit(testOutputHelper, LogLevel.Trace)
			.CreateLogger<HarvestTest>();

		Configuration = LoadConfiguration();
		HarvestClient = new HarvestClient(new HarvestClientOptions
		{
			AccountId = Configuration.AccountId,
			AccessToken = Configuration.AccessToken
		});
	}

	private static Configuration LoadConfiguration()
		=> new ConfigurationBuilder()
			.SetBasePath(Path.Combine(AppContext.BaseDirectory, "../../.."))
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
			.AddUserSecrets<HarvestTest>(optional: true)
			.Build()
			.Get<Configuration>() ?? new Configuration();

	/// <summary>A unique, prefixed name for a test object, for example <c>TEST_CRUD Test Client 20260101120000</c>.</summary>
	protected string TestName(string purpose, string kind)
		=> $"{Prefix}{purpose} Test {kind} {DateTime.UtcNow:yyyyMMddHHmmss}";

	/// <summary>
	/// Runs a create-read-update-delete scenario, unless test data creation is disabled. An account that does not permit
	/// the operations (403 or 422) is logged rather than failed: the API methods were still shown to be callable.
	/// </summary>
	protected async Task RunCrudScenarioAsync(Func<Task> scenario)
	{
		if (!Configuration.TestSettings.CreateTestData)
		{
			Log.TestDataCreationDisabled(Logger);
			return;
		}

		try
		{
			await scenario();
		}
		catch (Refit.ApiException ex) when (ex.StatusCode is HttpStatusCode.UnprocessableEntity or HttpStatusCode.Forbidden)
		{
			Log.CrudNotPermitted(Logger, ex.StatusCode, ex.Message);
		}
	}

	/// <summary>The first item of a list, or <see langword="null"/> (logged) when the account has none to test with.</summary>
	protected T? FirstOrLog<T>(IReadOnlyList<T> items, string description) where T : class
	{
		if (items.Count == 0)
		{
			Log.NothingToTest(Logger, description);
			return null;
		}

		return items[0];
	}

	/// <summary>Creates a prefixed client and tracks it for deletion.</summary>
	protected async Task<Client> CreateTestClientAsync(string purpose, string? address)
	{
		var client = await HarvestClient.Clients.CreateAsync(new ClientCreationDto
		{
			Name = TestName(purpose, "Client"),
			IsActive = true,
			Address = address,
			Currency = "USD"
		}, CancellationToken);
		Track("client", client.Id, client.Name, id => HarvestClient.Clients.DeleteAsync(id, CancellationToken));
		return client;
	}

	/// <summary>Creates a prefixed, billable project under a client and tracks it for deletion.</summary>
	protected async Task<Project> CreateTestProjectAsync(long clientId, string purpose, string? notes)
	{
		var project = await HarvestClient.Projects.CreateAsync(new ProjectCreationDto
		{
			Name = TestName(purpose, "Project"),
			ClientId = clientId,
			BillBy = "Project",
			BudgetBy = "project",
			IsBillable = true,
			Notes = notes
		}, CancellationToken);
		Track("project", project.Id, project.Name, id => HarvestClient.Projects.DeleteAsync(id, CancellationToken));
		return project;
	}

	/// <summary>Creates a prefixed, billable task and tracks it for deletion.</summary>
	protected async Task<Models.Task> CreateTestTaskAsync(string purpose, decimal defaultHourlyRate)
	{
		var task = await HarvestClient.Tasks.CreateAsync(new TaskCreationDto
		{
			Name = TestName(purpose, "Task"),
			BillableByDefault = true,
			DefaultHourlyRate = defaultHourlyRate,
			IsDefault = false,
			IsActive = true
		}, CancellationToken);
		Track("task", task.Id, task.Name, id => HarvestClient.Tasks.DeleteAsync(id, CancellationToken));
		return task;
	}

	/// <summary>Assigns a task to a project as billable and tracks the assignment for deletion.</summary>
	protected async Task<ProjectTaskAssignment> CreateTestTaskAssignmentAsync(long projectId, long taskId, decimal hourlyRate)
	{
		var assignment = await HarvestClient.ProjectTaskAssignments.CreateAsync(projectId, new ProjectTaskAssignmentCreationDto
		{
			TaskId = taskId,
			IsActive = true,
			Billable = true,
			HourlyRate = hourlyRate
		}, CancellationToken);
		Track("task assignment", assignment.Id, null, id => HarvestClient.ProjectTaskAssignments.DeleteAsync(projectId, id, CancellationToken));
		return assignment;
	}

	/// <summary>Records an object a test created itself, so that it is deleted if the test does not get that far.</summary>
	protected void Track(string kind, long id, string? name, Func<long, Task> deleteAsync)
	{
		_trackedResources.Add(new TrackedResource(kind, id, deleteAsync));
		Log.Created(Logger, kind, id, name);
	}

	/// <summary>Deletes a tracked object as a step of the test itself; unlike clean-up, a failure fails the test.</summary>
	protected async Task DeleteTrackedAsync(string kind, long id)
	{
		var resource = _trackedResources.Single(tracked => tracked.Kind == kind && tracked.Id == id);
		await resource.DeleteAsync(id);
		_ = _trackedResources.Remove(resource);
		Log.Deleted(Logger, kind, id);
	}

	public ValueTask InitializeAsync() => ValueTask.CompletedTask;

	public async ValueTask DisposeAsync()
	{
		if (Configuration.TestSettings.CleanupTestData)
		{
			// Newest first: an assignment goes before its project, and a project before its client.
			for (var index = _trackedResources.Count - 1; index >= 0; index--)
			{
				await TryCleanUpAsync(_trackedResources[index]);
			}
		}

		_trackedResources.Clear();
		HarvestClient.Dispose();
		GC.SuppressFinalize(this);
	}

	private async Task TryCleanUpAsync(TrackedResource resource)
	{
		try
		{
			await resource.DeleteAsync(resource.Id);
			Log.CleanedUp(Logger, resource.Kind, resource.Id);
		}
		catch (Refit.ApiException)
		{
			// Already deleted, or never fully created
		}
	}

	private sealed record TrackedResource(string Kind, long Id, Func<long, Task> DeleteAsync);
}