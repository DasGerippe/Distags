using SteamWebApi.Dtos;
using SteamWebApi.Exceptions;
using System.Text.Json;

namespace SteamWebApi;

public class SteamWebApiClient
{
    private readonly HttpClient _HttpClient;

    public SteamWebApiClient(HttpClient httpClient)
    {
        _HttpClient = httpClient;
    }

    internal Task<TResponseContent> GetAsync<TResponseContent>(
        string requestUri, CancellationToken cancellationToken = default)
        where TResponseContent : class
    {
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
        return SendAsync<TResponseContent>(requestMessage, cancellationToken);
    }

    internal Task<TResponseContent> PostAsync<TResponseContent>(
        string requestUri, HttpContent? httpContent, CancellationToken cancellationToken = default)
        where TResponseContent : class
    {
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = httpContent,
        };
        return SendAsync<TResponseContent>(requestMessage, cancellationToken);
    }

    internal async Task<TResponseContent> SendAsync<TResponseContent>(
        HttpRequestMessage requestMessage, CancellationToken cancellationToken = default)
        where TResponseContent : class
    {
        HttpResponseMessage responseMessage =
            await _HttpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);

        TResponseContent responseContent =
            await GetSuccessfulResponseContent<TResponseContent>(responseMessage, cancellationToken).ConfigureAwait(false);

        return responseContent;
    }

    private async Task<TResponseContent> GetSuccessfulResponseContent<TResponseContent>(
        HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
        where TResponseContent : class
    {
        if (!responseMessage.IsSuccessStatusCode)
            throw new SteamWebApiRequestException(responseMessage);

        Stream responseContentStream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        SteamResponse<TResponseContent>? steamResponse = await JsonSerializer
            .DeserializeAsync<SteamResponse<TResponseContent>>(responseContentStream, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return steamResponse?.Content ?? throw new SteamWebApiResponseException("HTTP response content was null.");
    }
}