using Microsoft.JSInterop;

namespace LevelTagWebApp.Services;

public class HistoryService
{
    private readonly IJSRuntime _JsRuntime;

    public HistoryService(IJSRuntime jsRuntime)
    {
        _JsRuntime = jsRuntime;
    }

    public async Task ReplaceState(string url, CancellationToken cancellationToken = default)
    {
        await _JsRuntime.InvokeVoidAsync("history.replaceState", cancellationToken, null, string.Empty, url);
    }
}