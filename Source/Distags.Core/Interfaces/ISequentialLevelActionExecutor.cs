namespace Distags.Core.Interfaces;

public interface ISequentialLevelActionExecutor
{
    Task ExecuteSequentialForLevelAsync(long levelId, Func<Task> action);
}