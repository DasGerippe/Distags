namespace LevelTagWebApp.Services;

public class TimeRefreshService : IDisposable
{
    private readonly Timer _Timer;

    private bool _Disposed;

    public event Action? OnTick;

    public TimeRefreshService()
    {
        TimeSpan refreshInterval = TimeSpan.FromSeconds(30);
        _Timer = new Timer(_ => OnTick?.Invoke(), null, refreshInterval, refreshInterval);
    }

    public void Dispose()
    {
        if (_Disposed)
            return;

        _Timer.Dispose();
        _Disposed = true;
    }
}