using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Distags.Api.IntegrationTests.Infrastructure;

/// <summary>
/// Collects log entries so tests can assert on them. Used instead of reaching for Serilog's static Log.Logger
/// </summary>
public sealed class CapturingLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentQueue<CapturedLogEntry> _Entries = new ConcurrentQueue<CapturedLogEntry>();

    public IReadOnlyCollection<CapturedLogEntry> Entries => _Entries.ToArray();

    public ILogger CreateLogger(string categoryName)
    {
        return new CapturingLogger(categoryName, _Entries);
    }

    public void Dispose() { }

    private sealed class CapturingLogger : ILogger
    {
        private readonly string _CategoryName;
        private readonly ConcurrentQueue<CapturedLogEntry> _Entries;

        public CapturingLogger(string categoryName, ConcurrentQueue<CapturedLogEntry> entries)
        {
            _CategoryName = categoryName;
            _Entries = entries;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            _Entries.Enqueue(new CapturedLogEntry
            {
                CategoryName = _CategoryName,
                LogLevel = logLevel,
                Message = formatter(state, exception),
                Exception = exception,
            });
        }
    }
}

public sealed class CapturedLogEntry
{
    public required string CategoryName { get; init; }

    public required LogLevel LogLevel { get; init; }

    public required string Message { get; init; }

    public Exception? Exception { get; init; }
}
