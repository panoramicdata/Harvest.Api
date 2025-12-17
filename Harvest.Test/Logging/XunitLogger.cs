using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;

namespace Harvest.Test.Logging;

public class XunitLogger(ITestOutputHelper output, string category, LogLevel minLogLevel) : ILogger
{
	private static readonly string[] _newLineChars = [Environment.NewLine];

	public void Log<TState>(
		LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}

		// Buffer the message into a single string in order to avoid shearing the message when running across multiple threads.
		var messageBuilder = new StringBuilder();

		var firstLinePrefix = $"| {category} {logLevel}: ";
		var lines = formatter(state, exception).Split(_newLineChars, StringSplitOptions.RemoveEmptyEntries);
		messageBuilder.Append(firstLinePrefix).AppendLine(lines.FirstOrDefault());

		var additionalLinePrefix = "|" + new string(' ', firstLinePrefix.Length - 1);
		foreach (var line in lines.Skip(1))
		{
			messageBuilder.Append(additionalLinePrefix).AppendLine(line);
		}

		if (exception != null)
		{
			lines = exception.ToString().Split(_newLineChars, StringSplitOptions.RemoveEmptyEntries);
			additionalLinePrefix = "| ";
			foreach (var line in lines.Skip(1))
			{
				messageBuilder.Append(additionalLinePrefix).AppendLine(line);
			}
		}

		// Remove the last line-break, because ITestOutputHelper only has WriteLine.
		var message = messageBuilder.ToString();
		if (message.EndsWith(Environment.NewLine, StringComparison.Ordinal))
		{
			message = message[..^Environment.NewLine.Length];
		}

		try
		{
			output.WriteLine(message);
		}
		catch
		{
			// We could fail because we're on a background thread and our captured ITestOutputHelper is
			// busted (if the test "completed" before the background thread fired).
			// So, ignore this. There isn't really anything we can do but hope the
			// caller has additional loggers registered
		}
	}

	public bool IsEnabled(LogLevel logLevel)
		=> logLevel >= minLogLevel;

	public IDisposable BeginScope<TState>(TState state)
		=> new NullScope();

	private class NullScope : IDisposable
	{
		public void Dispose()
		{
			// No resources to dispose
		}
	}
}

public class XunitLogger<T>(ITestOutputHelper output) : ILogger<T>, IDisposable
{
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) => output.WriteLine(state.ToString());

	public bool IsEnabled(LogLevel logLevel) => true;

	public IDisposable BeginScope<TState>(TState state) => this;

	public void Dispose() => GC.SuppressFinalize(this);
}