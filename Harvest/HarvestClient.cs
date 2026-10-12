using Harvest.Handlers;
using Harvest.Interfaces;
using Harvest.Serialization;
using System.Net;
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
	/// <summary>The address every request is sent to, except those of <see cref="Accounts"/>.</summary>
	public static Uri BaseAddress { get; } = new("https://api.harvestapp.com");

	/// <summary>
	/// The Harvest ID address that <see cref="Accounts"/> sends its requests to, without the <c>Harvest-Account-Id</c> header.
	/// </summary>
	public static Uri IdBaseAddress { get; } = new("https://id.getharvest.com");

	private readonly HttpClient _httpClient;

	// Shares _httpClient's pipeline (retries and transport) but has its own base address and headers.
	private readonly HttpClient _idHttpClient;

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
		_ = Validated(options);
		var pipeline = new RetryHandler(options) { InnerHandler = innerHandler };
		_httpClient = CreateHttpClient(options, pipeline, forAccount: true);
		_idHttpClient = CreateHttpClient(options, pipeline, forAccount: false);
		Accounts = RestService.For<IAccountApi>(_idHttpClient, Settings);
		BillableRates = For<IBillableRateApi>();
		Clients = For<IClientApi>();
		Companies = For<ICompanyApi>();
		Contacts = For<IContactApi>();
		CostRates = For<ICostRateApi>();
		EstimateItemCategories = For<IEstimateItemCategoryApi>();
		EstimateMessages = For<IEstimateMessageApi>();
		Estimates = For<IEstimateApi>();
		ExpenseCategories = For<IExpenseCategoryApi>();
		Expenses = For<IExpenseApi>();
		InvoiceItemCategories = For<IInvoiceItemCategoryApi>();
		InvoiceMessages = For<IInvoiceMessageApi>();
		InvoicePayments = For<IInvoicePaymentApi>();
		Invoices = For<IInvoiceApi>();
		ProjectTaskAssignments = For<IProjectTaskAssignmentApi>();
		ProjectUserAssignments = For<IProjectUserAssignmentApi>();
		Projects = For<IProjectApi>();
		PtoAllocations = For<IPtoAllocationApi>();
		PtoAssignments = For<IPtoAssignmentApi>();
		PtoBalances = For<IPtoBalanceApi>();
		PtoHolidayCalendars = For<IPtoHolidayCalendarApi>();
		PtoRequests = For<IPtoRequestApi>();
		PtoTypes = For<IPtoTypeApi>();
		PtoWorkSchedules = For<IPtoWorkScheduleApi>();
		Reports = For<IReportApi>();
		Roles = For<IRoleApi>();
		Tasks = For<ITaskApi>();
		Teammates = For<ITeammateApi>();
		TimeEntries = For<ITimeEntryApi>();
		UserProjectAssignments = For<IUserProjectAssignmentApi>();
		Users = For<IUserApi>();
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
		ExceptionFactory = response => new ValueTask<Exception?>(HarvestErrorMapper.CreateAsync(response)),
		// Refit would wrap every exception thrown while sending in its ApiRequestException; surface them as themselves
		// (TimeoutException, HttpRequestException, OperationCanceledException...), as before.
		TransportExceptionFactory = static (_, exception, _) => exception
	};

	/// <inheritdoc />
	public IAccountApi Accounts { get; }

	/// <inheritdoc />
	public IBillableRateApi BillableRates { get; }

	/// <inheritdoc />
	public IClientApi Clients { get; }

	/// <inheritdoc />
	public ICompanyApi Companies { get; }

	/// <inheritdoc />
	public IContactApi Contacts { get; }

	/// <inheritdoc />
	public ICostRateApi CostRates { get; }

	/// <inheritdoc />
	public IEstimateItemCategoryApi EstimateItemCategories { get; }

	/// <inheritdoc />
	public IEstimateMessageApi EstimateMessages { get; }

	/// <inheritdoc />
	public IEstimateApi Estimates { get; }

	/// <inheritdoc />
	public IExpenseCategoryApi ExpenseCategories { get; }

	/// <inheritdoc />
	public IExpenseApi Expenses { get; }

	/// <inheritdoc />
	public IInvoiceItemCategoryApi InvoiceItemCategories { get; }

	/// <inheritdoc />
	public IInvoiceMessageApi InvoiceMessages { get; }

	/// <inheritdoc />
	public IInvoicePaymentApi InvoicePayments { get; }

	/// <inheritdoc />
	public IInvoiceApi Invoices { get; }

	/// <inheritdoc />
	public IProjectTaskAssignmentApi ProjectTaskAssignments { get; }

	/// <inheritdoc />
	public IProjectUserAssignmentApi ProjectUserAssignments { get; }

	/// <inheritdoc />
	public IProjectApi Projects { get; }

	/// <inheritdoc />
	public IPtoAllocationApi PtoAllocations { get; }

	/// <inheritdoc />
	public IPtoAssignmentApi PtoAssignments { get; }

	/// <inheritdoc />
	public IPtoBalanceApi PtoBalances { get; }

	/// <inheritdoc />
	public IPtoHolidayCalendarApi PtoHolidayCalendars { get; }

	/// <inheritdoc />
	public IPtoRequestApi PtoRequests { get; }

	/// <inheritdoc />
	public IPtoTypeApi PtoTypes { get; }

	/// <inheritdoc />
	public IPtoWorkScheduleApi PtoWorkSchedules { get; }

	/// <inheritdoc />
	public IReportApi Reports { get; }

	/// <inheritdoc />
	public IRoleApi Roles { get; }

	/// <inheritdoc />
	public ITaskApi Tasks { get; }

	/// <inheritdoc />
	public ITeammateApi Teammates { get; }

	/// <inheritdoc />
	public ITimeEntryApi TimeEntries { get; }

	/// <inheritdoc />
	public IUserProjectAssignmentApi UserProjectAssignments { get; }

	/// <inheritdoc />
	public IUserApi Users { get; }

	/// <inheritdoc />
	public void Dispose()
	{
		_idHttpClient.Dispose();
		_httpClient.Dispose();
	}

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

	// The account client (api.harvestapp.com) owns the shared pipeline and sends Harvest-Account-Id; the Harvest ID client
	// (id.getharvest.com) does neither.
	private static HttpClient CreateHttpClient(HarvestClientOptions options, HttpMessageHandler pipeline, bool forAccount)
	{
		var httpClient = new HttpClient(pipeline, disposeHandler: forAccount)
		{
			BaseAddress = forAccount ? BaseAddress : IdBaseAddress,
			Timeout = options.Timeout
		};
		var headers = httpClient.DefaultRequestHeaders;
		headers.Authorization = new AuthenticationHeaderValue("Bearer", options.AccessToken);
		if (forAccount)
		{
			headers.Add("Harvest-Account-Id", options.AccountId.ToString(CultureInfo.InvariantCulture));
		}

		// Harvest asks for "AppName (contact)", which is not a strict RFC 9110 product list, so it is not validated.
		_ = headers.TryAddWithoutValidation("User-Agent", options.UserAgent ?? DefaultUserAgent);
		headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		return httpClient;
	}
}