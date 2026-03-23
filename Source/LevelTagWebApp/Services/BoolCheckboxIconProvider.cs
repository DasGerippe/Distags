namespace LevelTagWebApp.Services;

public class BoolCheckboxIconProvider : ICheckboxIconProvider<bool>
{
    public string GetIconId(bool currentState)
    {
        return currentState ? "square-check-solid" : "square-regular";
    }

    public string GetIconClass(bool currentState)
    {
        return currentState ? "text-primary" : "text-muted";
    }
}