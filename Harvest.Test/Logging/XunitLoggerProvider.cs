using Microsoft.Extensions.Logging;
using System;

namespace Harvest.Test.Logging;

public class XunitLoggerProvider(ITestOutputHelper output, LogLevel minLevel) : ILoggerProvider
{
	public XunitLoggerProvider(ITestOutputHelper output)
		: this(output, LogLevel.Trace)
	{
	}

	public ILogger CreateLogger(string categoryName) => new XunitLogger(output, categoryName, minLevel);

	public void Dispose() =>
		GC.SuppressFinalize(this);
}