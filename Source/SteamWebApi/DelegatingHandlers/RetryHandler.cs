using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;

namespace SteamWebApi.DelegatingHandlers;

public class RetryHandler : DelegatingHandler
{
    private readonly HttpStatusCode[] _RetryStatusCodes =
    [
        HttpStatusCode.TooManyRequests,
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.Locked,
        HttpStatusCode.TooManyRequests,
        HttpStatusCode.ServiceUnavailable,
    ];

    private readonly ILogger<RetryHandler>? _Logger;
    private readonly RetrySettings _RetrySettings;

    public RetryHandler(RetrySettings retrySettings, ILogger<RetryHandler>? logger)
    {
        _RetrySettings = retrySettings;
        _Logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _RetrySettings.RetryCount == 0
            ? base.SendAsync(request, cancellationToken)
            : SendWithRetriesAsync(request, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendWithRetriesAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        int maxAttempts = _RetrySettings.RetryCount + 1;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode || !IsRetryable(response))
                return response;

            HttpStatusCode statusCode = response.StatusCode;
            RetryConditionHeaderValue? retryAfterHeader = response.Headers.RetryAfter;

            _Logger?.LogWarning(
                "Attempt {attempt} was unsuccessful. Received status code {statusCode}. Retry-After header: {retryAfterHeader}.",
                attempt, (int)statusCode, retryAfterHeader);

            if (attempt >= _RetrySettings.RetryCount)
                break;

            TimeSpan retryDelay = GetRetryDelay(retryAfterHeader);
            if (retryDelay > _RetrySettings.MaxAcceptedRetryDelay)
            {
                _Logger?.LogWarning("Received or configured retry delay {retryDelay} is too large.", retryDelay);
                return CreateRetryDelayTooLargeResponse(request);
            }

            _Logger?.LogDebug("Waiting {retryDelay} before retrying.", retryDelay);
            await Task.Delay(retryDelay, cancellationToken).ConfigureAwait(false);
        }

        _Logger?.LogWarning("Maximum retries exhausted.");
        return CreateRetriesExhaustedResponse(request);
    }

    private bool IsRetryable(HttpResponseMessage response)
    {
        return _RetryStatusCodes.Contains(response.StatusCode) || response.Headers.RetryAfter != null;
    }

    private TimeSpan GetRetryDelay(RetryConditionHeaderValue? retryAfterHeader)
    {
        if (retryAfterHeader != null)
        {
            if (retryAfterHeader.Delta != null)
                return retryAfterHeader.Delta.Value;

            if (retryAfterHeader.Date != null)
                return retryAfterHeader.Date.Value - DateTimeOffset.UtcNow;
        }

        return _RetrySettings.RetryDelay;
    }

    private HttpResponseMessage CreateRetryDelayTooLargeResponse(HttpRequestMessage request)
    {
        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            RequestMessage = request,
            ReasonPhrase = "The retry delay that was received in the Retry-After header or configured was too large.",
        };
    }

    private HttpResponseMessage CreateRetriesExhaustedResponse(HttpRequestMessage request)
    {
        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            RequestMessage = request,
            ReasonPhrase = "The maximum number of retries has been exhausted.",
        };
    }
}