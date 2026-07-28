using GodotGadgets.Tasks;
using GodotTask;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

namespace GodotGadgets.TweenStuff;

public static class TweenExtensions
{
    extension(CanvasItem target)
    {
        public GDTask FadeOutAsync(float duration, CancellationToken ct = default)
            => target.TweenAlphaAsync(0, duration, ct);

        public GDTask FadeInAsync(float duration, CancellationToken ct = default)
        {
            target.Modulate = target.Modulate with { A = 0 };
            return target.TweenAlphaAsync(1, duration, ct);
        }

        GDTask TweenAlphaAsync(float to, float duration, CancellationToken ct = default) =>
            target.TweenModulateAlpha(to, duration)
                .SetEasing(Easing.InOutCubic)
                .PlayAsyncUntilNodeDestroy(target, ct);
    }

    extension(Control target)
    {
        public async GDTask SlideInAsync(Vector2 destinationGlobal, float duration = 0.6f, CancellationToken ct = default)
        {
            target.Show();
            await target.TweenGlobalPosition(destinationGlobal, duration)
                .SetEasing(Easing.OutBack)
                .PlayAsyncUntilNodeDestroy(target, ct);
        }

        public async GDTask SlideOutAsync(Vector2 destinationGlobal, float duration = 0.6f,
            CancellationToken ct = default)
        {
            await target.TweenGlobalPosition(destinationGlobal, duration)
                .SetEasing(Easing.InBack)
                .PlayAsyncUntilNodeDestroy(target, ct);
            target.Hide();
        }
    }

    extension(GTween tween)
    {
        public GDTask PlayAsyncUntilNodeDestroy(Node node, CancellationToken ct = default) =>
            tween.PlayAsyncGD(ct.LinkWithNodeDestroy(node).Token);

        public GDTask PlayAsyncGD(CancellationToken ct = default) => tween.PlayAsync(ct).AsGDTask();
    }
}