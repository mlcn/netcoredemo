using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Tests
{
	public class TestLoggerFactory : ILoggerFactory
	{
		public TestLogger Logger { get; private set; }

		public void Dispose()
		{
			Logger = null;
		}

		public ILogger CreateLogger(string categoryName)
		{
			Logger = new TestLogger(categoryName);
			return Logger;
		}

		public void AddProvider(ILoggerProvider provider)
		{
		}
	}

	public class TestLogger : ILogger
	{
		public TestLogger(string categoryName)
		{
			this.Logs = new List<string>();
			prefix = categoryName;
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			Logs.Add($"{prefix};{logLevel}:{eventId.Id}:{state}:{exception?.Message}");
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}

		public List<string> Logs { get; }
		readonly string prefix;
	}
}
