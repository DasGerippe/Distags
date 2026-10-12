using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Distags.WebApp.DistagsApi;

public static class HttpResponseMessageExtensions
{
    public static async Task<ApiResult> ToApiResultAsync(
        this Task<HttpResponseMessage> responseMessageTask, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage responseMessage = await responseMessageTask;
        return await responseMessage.ToApiResultAsync(cancellationToken);
    }

    public static async Task<ApiResult> ToApiResultAsync(
        this HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        if (responseMessage.IsSuccessStatusCode)
        {
            return ApiResult.Success(responseMessage.StatusCode);
        }
        else
        {
            ProblemDetails? problemDetails = await GetProblemDetailsAsync(responseMessage, cancellationToken);
            return ApiResult.Failure(responseMessage.StatusCode, problemDetails);
        }
    }

    public static async Task<ApiResult<TResponse>> ToApiResultAsync<TResponse>(
        this Task<HttpResponseMessage> responseMessageTask, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage responseMessage = await responseMessageTask;
        return await responseMessage.ToApiResultAsync<TResponse>(cancellationToken);
    }

    public static async Task<ApiResult<TResponse>> ToApiResultAsync<TResponse>(
        this HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        if (responseMessage.IsSuccessStatusCode)
        {
            TResponse response = await GetResponseAsync<TResponse>(responseMessage, cancellationToken);
            return ApiResult<TResponse>.Success(responseMessage.StatusCode, response);
        }
        else
        {
            ProblemDetails? problemDetails = await GetProblemDetailsAsync(responseMessage, cancellationToken);
            return ApiResult<TResponse>.Failure(responseMessage.StatusCode, problemDetails);
        }
    }

    private static async Task<TResponse> GetResponseAsync<TResponse>(
        HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        if (responseMessage.StatusCode == HttpStatusCode.NoContent)
            throw new InvalidOperationException("API response has no content.");

        TResponse? response = await responseMessage.Content
            .ReadFromJsonAsync<TResponse>(JsonSerializerOptions.Web, cancellationToken)
            .ConfigureAwait(false);

        if (response == null)
            throw new JsonException($"Could not deserialize response to {typeof(TResponse)}.");

        return response;
    }

    private static async Task<ProblemDetails?> GetProblemDetailsAsync(
        HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            return await responseMessage.Content
                .ReadFromJsonAsync<ProblemDetails>(JsonSerializerOptions.Web, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
