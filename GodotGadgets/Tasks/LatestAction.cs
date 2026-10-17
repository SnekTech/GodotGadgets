using GodotTask;

namespace GodotGadgets.Tasks;

public sealed class LatestAction : IDisposable
{
    CancellationTokenSource _current = new();

    public GDTask RunAsync(Func<CancellationToken, GDTask> action)
    {
        _current.CancelAndDispose();
        _current = new CancellationTokenSource();
        return action(_current.Token);
    }
    
    public void Dispose() => _current.CancelAndDispose();
}