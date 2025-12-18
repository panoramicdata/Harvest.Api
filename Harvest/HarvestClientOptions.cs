using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Harvest;

public class HarvestClientOptions
{
	public required int AccountId { get; init; }

	public required string AccessToken { get; init; }

	public string? ProxyUrl { get; init; }

	public ILogger Logger { get; init; } = NullLogger.Instance;
}