using Microsoft.Extensions.Logging;

namespace SteamWebApi.DelegatingHandlers;

public class RateLimitHandler : DelegatingHandler
{
    private readonly ILogger<RateLimitHandler>? _Logger;

    private readonly RateLimitSettings _Settings;
    private readonly SemaphoreSlim _Semaphore = new SemaphoreSlim(1, 1);
    private DateTime _LastRequestTime = DateTime.MinValue;

    public RateLimitHandler(RateLimitSettings settings, ILogger<RateLimitHandler>? logger)
    {
        _Settings = settings;
        _Logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await _Semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            DateTime utcNow = DateTime.UtcNow;
            int timeSinceLastRequestMs = (int)(utcNow -  _LastRequestTime).TotalMilliseconds;

            if (timeSinceLastRequestMs < _Settings.RequestDelayMs)
            {
                int delayMs = _Settings.RequestDelayMs - timeSinceLastRequestMs;
                _Logger?.LogDebug("Waiting {delay} ms before sending request.", delayMs);
                await Task.Delay(delayMs, cancellationToken).ConfigureAwait(false);
            }

            _LastRequestTime = utcNow;

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _Semaphore.Release();
        }
    }
}