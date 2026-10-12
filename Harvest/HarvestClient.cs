using Harvest.Handlers;
using Harvest.Interfaces;
using Harvest.Serialization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Harvest;

/// <summary>
/// A client for the Harvest API v2 (<see href="https://help.getharvest.com/api-v2/"/>). Each resource is exposed as a
/// property; every call takes a <see cref="CancellationToken"/> as its last argument.
/// </summary>
/// <remarks>
/// <para>
/// A client is thread-safe and intended to be long-lived: create one per account and token, and dispose it when done.
/// </para>
/// <para>
/// A non-success response raises <see cref="HarvestApiException"/> (a Refit <see cref="ApiException"/>) carrying
/// Harvest's error message. Throttled (429) and transient server-error responses are retried first, as configured by
/// <see cref="HarvestClientOptions.MaxRetries"/>.
/// </para>
/// </remarks>
public sealed class HarvestClient : IHarvestClient
{
	/// <summary>The address every request is sent to.</summary>
	public static Uri BaseAddress { get; } = new("https://api.harvestapp.com");

	private readonly HttpClient _httpClient;

	/// <summary>Creates a client that sends requests over the network, through <see cref="HarvestClientOptions.ProxyUrl"/> if set.</summary>
	/// <param name="options">The client options.</param>
	/// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">An option is invalid.</exception>
	public HarvestClient(HarvestClientOptions options) : this(options, CreateTransport(Validated(options)))
	{
	}

	/// <summary>
	/// Creates a client that sends requests through <paramref name="innerHandler"/> (for example an instrumented handler,
	/// or a stub in tests). The client takes ownership of the handler and disposes it;
	/// <see cref="HarvestClientOptions.ProxyUrl"/> is ignored.
	/// </summary>
	/// <param name="options">The client options.</param>
	/// <param name="innerHandler">The handler that sends requests to the network.</param>
	/// <exception cref="ArgumentNullException"><paramref name="options"/> or <paramref name="innerHandler"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException">An option is invalid.</exception>
	public HarvestClient(HarvestClientOptions options, HttpMessageHandler innerHandler)
	{
		ArgumentNullException.ThrowIfNull(innerHandler);
		Validated(options);
		_httpClient = CreateHttpClient(options, new RetryHandler(options) { InnerHandler = innerHandler });
		Clients = For<IClientApi>();
		Companies = For<ICompanyApi>();
		Projects = For<IProjectApi>();
		ProjectTaskAssignments = For<IProjectTaskAssignmentApi>();
		Tasks = For<ITaskApi>();
		TimeEntries = For<ITimeEntryApi>();
		Users = For<IUserApi>();
		UserProjectAssignments = For<IUserProjectAssignmentApi>();
	}

	/// <summary>
	/// The <c>User-Agent</c> sent when <see cref="HarvestClientOptions.UserAgent"/> is not set:
	/// <c>Harvest.Api/{version} (+https://github.com/panoramicdata/Harvest.Api)</c>.
	/// </summary>
	public static string DefaultUserAgent { get; } =
		$"Harvest.Api/{typeof(HarvestClient).Assembly.GetName().Version!.ToString(3)} (+https://github.com/panoramicdata/Harvest.Api)";

	/// <summary>
	/// The JSON settings for request and response bodies: snake_case property names, nulls omitted when writing, enums as
	/// snake_case strings.
	/// </summary>
	internal static JsonSerializerOptions JsonOptions { get; } = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
	};

	/// <summary>The Refit settings shared by every resource API.</summary>
	internal static RefitSettings Settings { get; } = new()
	{
		ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions),
		UrlParameterFormatter = new HarvestUrlParameterFormatter(),
		// Unbuffered, Refit sends a serialized body as a read-once push stream, which RetryHandler could not resend.
		Buffered = true,
		ExceptionFactory = HarvestErrorMapper.CreateAsync
	};

	/// <inheritdoc />
	public IClientApi Clients { get; }

	/// <inheritdoc />
	public ICompanyApi Companies { get; }

	/// <inheritdoc />
	public IProjectApi Projects { get; }

	/// <inheritdoc />
	public IProjectTaskAssignmentApi ProjectTaskAssignments { get; }

	/// <inheritdoc />
	public ITaskApi Tasks { get; }

	/// <inheritdoc />
	public ITimeEntryApi TimeEntries { get; }

	/// <inheritdoc />
	public IUserApi Users { get; }

	/// <inheritdoc />
	public IUserProjectAssignmentApi UserProjectAssignments { get; }

	/// <inheritdoc />
	public void Dispose() => _httpClient.Dispose();

	private T For<T>() => RestService.For<T>(_httpClient, Settings);

	// Validation comes before the transport is created, so invalid options do not leave an undisposed network handler.
	private static HarvestClientOptions Validated(HarvestClientOptions options)
	{
		ArgumentNullException.ThrowIfNull(options);
		options.Validate();
		return options;
	}

	/// <summary>The network handler used when the caller supplies none.</summary>
	internal static HttpClientHandler CreateTransport(HarvestClientOptions options)
		=> new() { Proxy = CreateProxy(options.ProxyUrl) };

	private static WebProxy? CreateProxy(string? proxyUrl) => proxyUrl is null
		? null
		: new WebProxy
		{
			Address = new Uri(proxyUrl),
			BypassProxyOnLocal = false,
			UseDefaultCredentials = false,
		};

	private static HttpClient CreateHttpClient(HarvestClientOptions options, HttpMessageHandler pipeline)
	{
		var httpClient = new HttpClient(pipeline, disposeHandler: true)
		{
			BaseAddress = BaseAddress,
			Timeout = options.Timeout
		};
		var headers = httpClient.DefaultRequestHeaders;
		headers.Authorization = new AuthenticationHeaderValue("Bearer", options.AccessToken);
		headers.Add("Harvest-Account-Id", options.AccountId.ToString(CultureInfo.InvariantCulture));
		// Harvest asks for "AppName (contact)", which is not a strict RFC 9110 product list, so it is not validated.
		_ = headers.TryAddWithoutValidation("User-Agent", options.UserAgent ?? DefaultUserAgent);
		headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		return httpClient;
	}
}