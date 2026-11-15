using Distags.WebApp.DistagsApi;

namespace Distags.WebApp.Services;

public abstract class ApiResourceProvider<TResource> where TResource : class
{
    private readonly Lazy<Task> _InitializationTask;
    private TResource? _Resource;

    protected ApiResourceProvider(Func<Task<ApiResult<TResource>>> apiRequestFunc, TResource defaultResource)
    {
        _InitializationTask = new Lazy<Task>(
            async () =>
            {
                ApiResult<TResource> apiResult = await apiRequestFunc.Invoke();
                _Resource = apiResult.Value ?? defaultResource;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    protected ApiResourceProvider(Func<Task<ApiResult<TResource>>> apiRequestFunc)
    {
        _InitializationTask = new Lazy<Task>(
            async () =>
            {
                ApiResult<TResource> apiResult = await apiRequestFunc.Invoke();
                apiResult.EnsureSuccess();
                _Resource = apiResult.Value;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public async Task<TResource> GetResourceAsync()
    {
        await _InitializationTask.Value;
        return _Resource!;
    }
}
