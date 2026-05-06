using Microsoft.JSInterop;

namespace Distags.WebApp.Services;

public class HistoryManager
{
    private readonly IJSRuntime _JsRuntime;

    public HistoryManager(IJSRuntime jsRuntime)
    {
        _JsRuntime = jsRuntime;
    }

    public async Task ReplaceState(string url, CancellationToken cancellationToken = default)
    {
        await _JsRuntime.InvokeVoidAsync("history.replaceState", cancellationToken, null, string.Empty, url);
    }
}