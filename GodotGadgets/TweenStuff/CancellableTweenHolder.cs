using GodotGadgets.Tasks;
using GTweens.Tweens;
using GTweensGodot.Extensions;

namespace GodotGadgets.TweenStuff;

[UsedImplicitly]
public sealed class CancellableTweenHolder : IDisposable
{
    CancellationTokenSource _ctsPrevious = new();

    public Task CancelPreviousAndPlayAsync(GTween newTween, CancellationToken ct = default)
    {
        _ctsPrevious.CancelAndDispose();
        _ctsPrevious = new CancellationTokenSource();

        return newTween.PlayAsync(ct.LinkTo(_ctsPrevious.Token).Token);
    }

    public void Dispose()
    {
        _ctsPrevious.Dispose();
    }
}