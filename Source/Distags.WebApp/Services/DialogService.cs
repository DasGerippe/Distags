using Distags.WebApp.Components.Dialogs;

namespace Distags.WebApp.Services;

public class DialogService : IDialogService
{
    private readonly Dictionary<Type, DialogBase> _Dialogs = [];

    public void Register<TDialog>(TDialog dialog) where TDialog : DialogBase
    {
        _Dialogs.Add(dialog.GetType(), dialog);
    }

    public void Unregister<TDialog>(TDialog dialog) where TDialog : DialogBase
    {
        Type dialogType = dialog.GetType();
        if (_Dialogs.TryGetValue(dialogType, out DialogBase? cachedDialog) && cachedDialog == dialog)
            _Dialogs.Remove(dialogType);
    }

    public TDialog? Get<TDialog>() where TDialog : DialogBase
    {
        _Dialogs.TryGetValue(typeof(TDialog), out DialogBase? dialog);
        return dialog as TDialog;
    }
}