using GTweens.Tweens;
using GTweensGodot.Extensions;

namespace GodotGadgets.TweenStuff;

[UsedImplicitly]
public sealed class SingleTweenPlayer : IDisposable
{
    GTween? _currentTween;

    public void KillPreviousAndPlay(GTween newTween)
    {
        _currentTween?.Kill();
        _currentTween = newTween;
        newTween.Play();
    }
    
    public void Dispose()
    {
        _currentTween?.Kill();
    }
}