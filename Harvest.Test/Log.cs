using System.Net;

namespace Harvest.Test;

/// <summary>The live tests' log messages, source-generated so that no argument is boxed or formatted when disabled.</summary>
internal static partial class Log
{
	[LoggerMessage(Level = LogLevel.Information, Message = "Test data creation is disabled")]
	public static partial void TestDataCreationDisabled(ILogger logger);

	[LoggerMessage(Level = LogLevel.Warning, Message = "CRUD operations not permitted in this environment: {StatusCode} - {Message}")]
	public static partial void CrudNotPermitted(ILogger logger, HttpStatusCode statusCode, string message);

	[LoggerMessage(Level = LogLevel.Information, Message = "No {Description} exist to test")]
	public static partial void NothingToTest(ILogger logger, string description);

	[LoggerMessage(Level = LogLevel.Information, Message = "Created {Kind}: {Id} - {Name}")]
	public static partial void Created(ILogger logger, string kind, long id, string? name);

	[LoggerMessage(Level = LogLevel.Information, Message = "Retrieved {Kind}: {Id}")]
	public static partial void Retrieved(ILogger logger, string kind, long id);

	[LoggerMessage(Level = LogLevel.Information, Message = "Updated {Kind}: {Id}")]
	public static partial void Updated(ILogger logger, string kind, long id);

	[LoggerMessage(Level = LogLevel.Information, Message = "Deleted {Kind}: {Id}")]
	public static partial void Deleted(ILogger logger, string kind, long id);

	[LoggerMessage(Level = LogLevel.Information, Message = "Cleaned up test {Kind}: {Id}")]
	public static partial void CleanedUp(ILogger logger, string kind, long id);

	[LoggerMessage(Level = LogLevel.Warning, Message = "Time entry created but response deserialization failed: {Message}")]
	public static partial void TimeEntryResponseUnreadable(ILogger logger, string message);

	[LoggerMessage(Level = LogLevel.Information, Message = "{Available} of 7 PTO endpoints are available")]
	public static partial void PtoSummary(ILogger logger, int available);

	[LoggerMessage(Level = LogLevel.Information, Message = "PTO {Name}: available")]
	public static partial void PtoAvailable(ILogger logger, string name);

	[LoggerMessage(Level = LogLevel.Information, Message = "PTO {Name}: unavailable ({Message})")]
	public static partial void PtoUnavailable(ILogger logger, string name, string message);
}