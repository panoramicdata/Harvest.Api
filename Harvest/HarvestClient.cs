using Harvest.ContractResolvers;
using Harvest.Interfaces;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Harvest;

public class HarvestClient : IHarvestClient, IDisposable
{
	private readonly HttpClient _httpClient;

	public HarvestClient(HarvestClientOptions options)
	{
		if (string.IsNullOrWhiteSpace(options.AccessToken))
		{
			throw new ArgumentException("Access token cannot be null or empty.", nameof(options));
		}

		var refitSettings = CreateRefitSettings();
		_httpClient = CreateHttpClient(options);
		Companies = RestService.For<ICompanyApi>(_httpClient, refitSettings);
		Clients = RestService.For<IClientApi>(_httpClient, refitSettings);
		TimeEntries = RestService.For<ITimeEntryApi>(_httpClient, refitSettings);
		Projects = RestService.For<IProjectApi>(_httpClient, refitSettings);
		Tasks = RestService.For<ITaskApi>(_httpClient, refitSettings);
		ProjectTaskAssignments = RestService.For<IProjectTaskAssignmentApi>(_httpClient, refitSettings);
		Users = RestService.For<IUserApi>(_httpClient, refitSettings);
		UserProjectAssignments = RestService.For<IUserProjectAssignmentApi>(_httpClient, refitSettings);
	}

	private static RefitSettings CreateRefitSettings()
		=> new()
		{
			ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
			{
				PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				Converters = { new JsonStringEnumConverter(SnakeCaseNamingPolicy.Instance) }
			})
		};

	private static HttpClient CreateHttpClient(HarvestClientOptions options)
	{
		var httpClientHandler = new HttpClientHandler
		{
			Proxy = CreateProxy(options.ProxyUrl),
		};

		var httpClient = new HttpClient(handler: httpClientHandler, disposeHandler: true)
		{
			BaseAddress = new Uri("https://api.harvestapp.com"),
			DefaultRequestHeaders =
			{
				Authorization = new AuthenticationHeaderValue("Bearer", options.AccessToken),
				UserAgent =
				{
					new ProductInfoHeaderValue(
						"harvest.net",
						Assembly.GetExecutingAssembly().GetName().Version?.ToString()
					)
				},
			},
		};

		httpClient.DefaultRequestHeaders.Add("Harvest-Account-Id", options.AccountId.ToString(CultureInfo.InvariantCulture));
		return httpClient;
	}

	private static WebProxy? CreateProxy(string? proxyUrl) => proxyUrl is null
			? null
			: new WebProxy
			{
				Address = new Uri(proxyUrl),
				BypassProxyOnLocal = false,
				UseDefaultCredentials = false,
			};

	/// <summary>
	/// Clients
	/// </summary>
	public IClientApi Clients { get; }

	/// <summary>
	/// Companies
	/// </summary>
	public ICompanyApi Companies { get; }

	/// <summary>
	/// TimeEntries
	/// </summary>
	public ITimeEntryApi TimeEntries { get; }

	/// <summary>
	/// Projects
	/// </summary>
	public IProjectApi Projects { get; }

	/// <summary>
	/// ProjectTaskAssignments
	/// </summary>
	public IProjectTaskAssignmentApi ProjectTaskAssignments { get; }

	/// <summary>
	/// Users
	/// </summary>
	public IUserApi Users { get; }

	/// <summary>
	/// User project assignments
	/// </summary>
	public IUserProjectAssignmentApi UserProjectAssignments { get; }

	/// <summary>
	/// Tasks
	/// </summary>
	public ITaskApi Tasks { get; }

	public void Dispose()
	{
		_httpClient.Dispose();

		GC.SuppressFinalize(this);
	}
}