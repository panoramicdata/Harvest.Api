using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;

namespace Harvest.Handlers;

/// <summary>
/// Retries throttled and transient failures. A request is retried (up to <see cref="HarvestClientOptions.MaxRetries"/>
/// times) only when its body can be sent again and either the status is 429 (Harvest's rate limit) or 503, or the status
/// is another 5xx and the verb is idempotent. The wait is Harvest's <c>Retry-After</c> when sent, else an exponential
/// back-off from <see cref="HarvestClientOptions.RetryBaseDelay"/>, never longer than
/// <see cref="HarvestClientOptions.MaxRetryDelay"/>. Only the method and path are logged, never the query string.
/// </summary>
/// <param name="logger">The logger retries are reported to.</param>
/// <param name="maxRetries">The maximum number of retries.</param>
/// <param name="retryBaseDelay">The initial back-off, doubled on each retry.</param>
/// <param name="maxRetryDelay">The longest single wait before a retry.</param>
internal sealed partial class RetryHandler(ILogger logger, int maxRetries, TimeSpan retryBaseDelay, TimeSpan maxRetryDelay)
	: DelegatingHandler
{
	private static readonly HashSet<HttpMethod> _idempotentMethods =
		[HttpMethod.Get, HttpMethod.Head, HttpMethod.Put, HttpMethod.Delete, HttpMethod.Options, HttpMethod.Trace];

	private readonly ILogger _logger = logger;
	private readonly int _maxRetries = maxRetries;
	private readonly TimeSpan _retryBaseDelay = retryBaseDelay;
	private readonly TimeSpan _maxRetryDelay = maxRetryDelay;

	/// <summary>Creates a handler with the retry settings of <paramref name="options"/>.</summary>
	/// <param name="options">The client options.</param>
	public RetryHandler(HarvestClientOptions options)
		: this(options.Logger, options.MaxRetries, options.RetryBaseDelay, options.MaxRetryDelay)
	{
	}

	/// <summary>Waits before a retry; replaced in tests so that no test really waits.</summary>
	internal Func<TimeSpan, CancellationToken, System.Threading.Tasks.Task> Delay { get; set; } = System.Threading.Tasks.Task.Delay;

	/// <summary>The clock a <c>Retry-After</c> date is measured against; replaced in tests.</summary>
	internal TimeProvider TimeProvider { get; set; } = TimeProvider.System;

	/// <inheritdoc />
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var retries = IsReplayable(request.Content) ? _maxRetries : 0;
		var backoff = _retryBaseDelay;
		for (var attempt = 0; attempt < retries; attempt++)
		{
			var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
			if (!IsRetryable(request.Method, response.StatusCode))
			{
				return response;
			}

			var wait = CapDelay(RetryAfter(response) ?? backoff);
			LogRetrying(_logger, (int)response.StatusCode, request.Method, request.RequestUri!.AbsolutePath, wait, attempt + 1);
			response.Dispose();
			await Delay(wait, cancellationToken).ConfigureAwait(false);
			backoff = NextBackoff(backoff);
		}

		// The last attempt (or the only one, for a body that cannot be replayed) returns whatever happens.
		return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
	}

	[LoggerMessage(Level = LogLevel.Warning, Message = "Harvest answered {Status} to {Method} {Path}; retry {Retry} in {Wait}.")]
	private static partial void LogRetrying(ILogger logger, int status, HttpMethod method, string path, TimeSpan wait, int retry);

	private TimeSpan CapDelay(TimeSpan wait) => wait > _maxRetryDelay ? _maxRetryDelay : wait;

	// Doubling is capped so it can never overflow TimeSpan.
	private TimeSpan NextBackoff(TimeSpan backoff) => backoff > _maxRetryDelay / 2 ? _maxRetryDelay : backoff * 2;

	/// <summary>
	/// Whether the body can be sent again unchanged: none, buffered bytes, or JSON content (which serializes its value
	/// afresh on every send). Streams are not, since they may be read-once.
	/// </summary>
	internal static bool IsReplayable(HttpContent? content)
		=> content is null or ByteArrayContent or ReadOnlyMemoryContent or JsonContent;

	/// <summary>
	/// 429 and 503 mean the request was not processed, so any verb is retried. Other 5xx may follow a partial or complete
	/// write, so only idempotent verbs are retried on them (never POST or PATCH).
	/// </summary>
	internal static bool IsRetryable(HttpMethod method, HttpStatusCode status)
		=> status is HttpStatusCode.TooManyRequests or HttpStatusCode.ServiceUnavailable
			|| ((int)status >= 500 && _idempotentMethods.Contains(method));

	private TimeSpan? RetryAfter(HttpResponseMessage response)
	{
		var header = response.Headers.RetryAfter;
		if (header?.Delta is { } delta)
		{
			return delta;
		}

		if (header?.Date is { } date)
		{
			var wait = date - TimeProvider.GetUtcNow();
			return wait > TimeSpan.Zero ? wait : TimeSpan.Zero;
		}

		return null;
	}
}