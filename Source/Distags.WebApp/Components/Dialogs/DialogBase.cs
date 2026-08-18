using Distags.WebApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Distags.WebApp.Components.Dialogs;

public abstract class DialogBase : ComponentBase, IAsyncDisposable
{
    private IJSObjectReference? _BootstrapModalModule;

    private bool _Disposed;

    protected abstract string ModalId { get; }

    [Inject]
    public IJSRuntime? JsRuntime { get; set; }

    [Inject]
    public IDialogService? DialogService { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (JsRuntime != null && DialogService != null)
            {
                _BootstrapModalModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/bootstrap-modal.js");
                DialogService.Register(this);
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected ValueTask ShowAsync()
    {
        return _BootstrapModalModule?.InvokeVoidAsync("showModal", $"#{ModalId}") ?? ValueTask.CompletedTask;
    }

    protected ValueTask HideAsync()
    {
        return _BootstrapModalModule?.InvokeVoidAsync("hideModal", $"#{ModalId}") ?? ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_Disposed)
            return;

        DialogService?.Unregister(this);

        if (_BootstrapModalModule != null)
        {
            try
            {
                await _BootstrapModalModule.DisposeAsync();
            }
            catch (JSDisconnectedException) { }
        }

        _Disposed = true;
    }
}
