using Distags.WebApp.Components.Dialogs;

namespace Distags.WebApp.Services;

public interface IDialogService
{
    void Register<TDialog>(TDialog dialog) where TDialog : DialogBase;

    void Unregister<TDialog>(TDialog dialog) where TDialog : DialogBase;

    TDialog? Get<TDialog>() where TDialog : DialogBase;
}