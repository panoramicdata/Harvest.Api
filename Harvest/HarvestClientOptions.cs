using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Harvest;

/// <summary>
/// Settings for a <see cref="HarvestClient"/>. The client copies what it needs at construction: changing an options
/// object afterwards does not affect a client that already exists.
/// </summary>
public class HarvestClientOptions
{
	/// <summary>The default number of retries for throttled (429) and transient server-error responses.</summary>
	public static int DefaultMaxRetries => 3;

	/// <summary>
	/// The Harvest account ID, sent as the <c>Harvest-Account-Id</c> header. Find it, with a personal access token, at
	/// <see href="https://id.getharvest.com/developers"/>.
	/// </summary>
	public required int AccountId { get; init; }

	/// <summary>The personal access token or OAuth2 access token, sent as a bearer token.</summary>
	public required string AccessToken { get; init; }

	/// <summary>
	/// The <c>User-Agent</c> header. Harvest requires every request to name the application and give a link or contact
	/// email address, for example <c>MyApp (ops@example.com)</c>. Leave <see langword="null"/> for
	/// <see cref="HarvestClient.DefaultUserAgent"/>, which names this library and its repository.
	/// </summary>
	public string? UserAgent { get; init; }

	/// <summary>
	/// The URL of an HTTP proxy to send requests through, or <see langword="null"/> for none. Ignored when the client is
	/// constructed with its own <see cref="System.Net.Http.HttpMessageHandler"/>.
	/// </summary>
	public string? ProxyUrl { get; init; }

	/// <summary>
	/// How many times a request is retried after Harvest throttles it (HTTP 429) or answers with a transient server error
	/// (HTTP 5xx, retried for idempotent verbs only; 503 for any verb). Zero disables retrying. Defaults to
	/// <see cref="DefaultMaxRetries"/>.
	/// </summary>
	public int MaxRetries { get; init; } = DefaultMaxRetries;

	/// <summary>
	/// The wait before the first retry when Harvest sends no <c>Retry-After</c> header; doubled after each retry up to
	/// <see cref="MaxRetryDelay"/>. Defaults to one second.
	/// </summary>
	public TimeSpan RetryBaseDelay { get; init; } = TimeSpan.FromSeconds(1);

	/// <summary>
	/// The longest single wait before a retry, including one requested by <c>Retry-After</c>. Defaults to 60 seconds
	/// (Harvest's rate-limit window is 15 seconds for most endpoints and 15 minutes for reports).
	/// </summary>
	public TimeSpan MaxRetryDelay { get; init; } = TimeSpan.FromSeconds(60);

	/// <summary>
	/// The overall time allowed for one call, including any retries and the waits between them. Defaults to five minutes.
	/// </summary>
	public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(5);

	/// <summary>The logger that retries are reported to. Defaults to <see cref="NullLogger.Instance"/>.</summary>
	public ILogger Logger { get; init; } = NullLogger.Instance;

	/// <summary>Throws if any setting is invalid.</summary>
	/// <exception cref="ArgumentException">A setting is invalid; the message names it.</exception>
	internal void Validate()
	{
		ValidateIdentity();
		ValidateRetriesAndTimeout();
	}

	/// <summary>Throws if the account, token or User-Agent is invalid.</summary>
	private void ValidateIdentity()
	{
		if (string.IsNullOrWhiteSpace(AccessToken))
		{
			throw new ArgumentException("Access token cannot be null or empty.", nameof(AccessToken));
		}

		if (AccountId <= 0)
		{
			throw new ArgumentException("Account ID must be positive.", nameof(AccountId));
		}

		if (UserAgent is not null && string.IsNullOrWhiteSpace(UserAgent))
		{
			throw new ArgumentException("User-Agent cannot be blank; leave it null for the default.", nameof(UserAgent));
		}
	}

	/// <summary>Throws if a retry or timeout setting is invalid.</summary>
	private void ValidateRetriesAndTimeout()
	{
		if (MaxRetries < 0)
		{
			throw new ArgumentException("MaxRetries cannot be negative.", nameof(MaxRetries));
		}

		if (RetryBaseDelay < TimeSpan.Zero || MaxRetryDelay < TimeSpan.Zero)
		{
			throw new ArgumentException("Retry delays cannot be negative.", nameof(RetryBaseDelay));
		}

		if (Timeout <= TimeSpan.Zero && Timeout != System.Threading.Timeout.InfiniteTimeSpan)
		{
			throw new ArgumentException("Timeout must be positive or Timeout.InfiniteTimeSpan.", nameof(Timeout));
		}
	}
}