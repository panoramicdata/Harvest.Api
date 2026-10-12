using Harvest.Test.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;
using System.Reflection;

namespace Harvest.Test;

public class HarvestTest : IDisposable
{
	protected ILogger Logger { get; }

	protected HarvestClient HarvestClient { get; }

	protected Configuration Configuration { get; }

	protected TestDataManager TestDataManager { get; }

	protected static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

	protected HarvestTest(ITestOutputHelper iTestOutputHelper)
	{
		Logger = new LoggerFactory()
			//.AddDebug(LogLevel.Trace)
			.AddXunit(iTestOutputHelper, LogLevel.Trace)
			.CreateLogger<HarvestTest>();

		Configuration = LoadConfiguration("appsettings.json");
		HarvestClient = new HarvestClient(new HarvestClientOptions
		{
			AccountId = Configuration.AccountId,
			AccessToken = Configuration.AccessToken
		});

		TestDataManager = new TestDataManager(HarvestClient, Configuration.TestSettings);
	}

	private static Configuration LoadConfiguration(string jsonFilePath)
	{
		var location = typeof(HarvestTest).GetTypeInfo().Assembly.Location;
		var dirPath = Path.Combine(Path.GetDirectoryName(location), "../../..");

		Configuration configuration;
		var configurationRoot = new ConfigurationBuilder()
			.SetBasePath(dirPath)
			.AddJsonFile(jsonFilePath, false, false)
			.Build();
		var services = new ServiceCollection();
		_ = services
			.AddOptions()
			.Configure<Configuration>(configurationRoot);
		using (var sp = services.BuildServiceProvider())
		{
			var options = sp.GetService<IOptions<Configuration>>();
			configuration = options.Value;
		}

		return configuration;
	}

	/// <summary>
	/// Creates a uniquely named, prefixed client for a CRUD test; the caller deletes it.
	/// </summary>
	protected async Task<Client> CreateTestClientAsync(string purpose)
	{
		var client = await HarvestClient.Clients.CreateAsync(new ClientCreationDto
		{
			Name = $"{Configuration.TestSettings.TestDataPrefix}{purpose} Test Client {DateTime.UtcNow:yyyyMMddHHmmss}",
			IsActive = true,
			Currency = "USD"
		}, CancellationToken);
		Logger.LogInformation("Created test client: {ClientId} - {ClientName}", client.Id, client.Name);
		return client;
	}

	/// <summary>
	/// Creates a uniquely named, prefixed project under a client for a CRUD test; the caller deletes it.
	/// </summary>
	protected async Task<Project> CreateTestProjectAsync(long clientId, string purpose)
	{
		var project = await HarvestClient.Projects.CreateAsync(new ProjectCreationDto
		{
			Name = $"{Configuration.TestSettings.TestDataPrefix}{purpose} Test Project {DateTime.UtcNow:yyyyMMddHHmmss}",
			ClientId = clientId,
			BillBy = "Project",
			BudgetBy = "project",
			IsBillable = true
		}, CancellationToken);
		Logger.LogInformation("Created test project: {ProjectId} - {ProjectName}", project.Id, project.Name);
		return project;
	}

	/// <summary>
	/// Deletes a test object if it was created and not already deleted, ignoring API errors.
	/// </summary>
	protected async System.Threading.Tasks.Task TryCleanUpAsync(long? id, Func<long, System.Threading.Tasks.Task> deleteAsync, string description)
	{
		if (!id.HasValue)
		{
			return;
		}

		try
		{
			await deleteAsync(id.Value);
			Logger.LogInformation("Cleaned up test {Description}: {Id}", description, id.Value);
		}
		catch (Refit.ApiException)
		{
			// Already deleted or doesn't exist
		}
	}

	public void Dispose() => TestDataManager?.Dispose();
}