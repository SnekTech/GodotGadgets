using GodotGadgets.Tasks;
using GodotTask;
using GTweens.Tweens;

namespace GodotGadgets.TweenStuff;

[UsedImplicitly]
public sealed class CancellableTweenHolder : IDisposable
{
    readonly LatestAction _latestAction = new();

    public GDTask CancelPreviousAndPlayAsync(GTween newTween, CancellationToken ct = default) =>
        _latestAction.RunAsync(async latestToken =>
        {
            using var linked = ct.LinkTo(latestToken);
            await newTween.PlayAsyncGD(linked.Token);
        });

    public void Dispose() => _latestAction.Dispose();
}