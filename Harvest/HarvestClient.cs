using Harvest.ContractResolvers;
using Harvest.Interfaces;
using Newtonsoft.Json;
using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace Harvest
{
	public class HarvestClient : IHarvestClient, IDisposable
	{
		private readonly HttpClient _httpClient;

		public HarvestClient(int accountId, string accessToken, string? proxyUrl = null)
		{
			if (string.IsNullOrWhiteSpace(accessToken))
			{
				throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));
			}

			var refitSettings = new RefitSettings
			{
				ContentSerializer = new JsonContentSerializer(new JsonSerializerSettings
				{
					ContractResolver = new SnakeCaseContractResolver(),
					NullValueHandling = NullValueHandling.Ignore
				})
			};

			WebProxy? proxy = null;
			if (proxyUrl != null)
			{
				proxy = new WebProxy
				{
					Address = new Uri(proxyUrl),
					BypassProxyOnLocal = false,
					UseDefaultCredentials = false,

					//// *** These credentials are given to the proxy server, not the web server ***
					//Credentials = new NetworkCredential(
					//	userName: proxyUserName,
					//	password: proxyPassword);
				};
			}

			var httpClientHandler = new HttpClientHandler
			{
				Proxy = proxy,
			};

			_httpClient = new HttpClient(handler: httpClientHandler, disposeHandler: true)
			{
				BaseAddress = new Uri("https://api.harvestapp.com"),
				DefaultRequestHeaders =
				{
					Authorization = new AuthenticationHeaderValue("Bearer", accessToken),
					UserAgent =
					{
						new ProductInfoHeaderValue("harvest.net", Assembly.GetExecutingAssembly().GetName().Version.ToString())
					},
				},
			};

			_httpClient.DefaultRequestHeaders.Add("Harvest-Account-Id", accountId.ToString());

			Companies = RestService.For<ICompanyApi>(_httpClient, refitSettings);
			Clients = RestService.For<IClientApi>(_httpClient, refitSettings);
			TimeEntries = RestService.For<ITimeEntryApi>(_httpClient, refitSettings);
			Projects = RestService.For<IProjectApi>(_httpClient, refitSettings);
			Tasks = RestService.For<ITaskApi>(_httpClient, refitSettings);
			ProjectTaskAssignments = RestService.For<IProjectTaskAssignmentApi>(_httpClient, refitSettings);
			Users = RestService.For<IUserApi>(_httpClient, refitSettings);
			UserProjectAssignments = RestService.For<IUserProjectAssignmentApi>(_httpClient, refitSettings);
		}

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
		}
	}
}