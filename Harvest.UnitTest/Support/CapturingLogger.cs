using Microsoft.Extensions.Logging;

namespace Harvest.UnitTest.Support;

/// <summary>A logger that records every entry, for asserting what a handler logs.</summary>
internal sealed class CapturingLogger : ILogger
{
	/// <summary>The entries logged, in order.</summary>
	public List<(LogLevel Level, string Message)> Entries { get; } = [];

	/// <inheritdoc />
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

	/// <inheritdoc />
	public bool IsEnabled(LogLevel logLevel) => true;

	/// <inheritdoc />
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
		=> Entries.Add((logLevel, formatter(state, exception)));
}