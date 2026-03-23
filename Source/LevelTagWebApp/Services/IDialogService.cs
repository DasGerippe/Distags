using LevelTagWebApp.Components.Dialogs;

namespace LevelTagWebApp.Services;

public interface IDialogService
{
    void Register<TDialog>(TDialog dialog) where TDialog : DialogBase;

    void Unregister<TDialog>(TDialog dialog) where TDialog : DialogBase;

    TDialog? Get<TDialog>() where TDialog : DialogBase;
}