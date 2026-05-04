namespace Distags.Core.Interfaces;

public interface ISequentialLevelActionExecutor
{
    Task ExecuteSequentialForLevelAsync(long levelId, Func<Task> action);

    Task<TResult> ExecuteSequentialForLevelAsync<TResult>(long levelId, Func<Task<TResult>> action);
}