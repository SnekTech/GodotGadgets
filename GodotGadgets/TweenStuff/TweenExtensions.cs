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
            // token 与 target 的树生命周期绑定: target 退场时它同步被取消, 而续体要到下一帧才被泵出,
            // 所以"tween 正常播完、同一帧节点才死"这种情况, 在续体里检查 ct 就能看见取消。
            // 链接在**使用点**自建(不依赖调用方传什么), 否则 ct = default 时检查是真空的假安全。
            using var linked = ct.LinkWithNodeDestroy(target);

            await target.TweenGlobalPosition(destinationGlobal, duration)
                .SetEasing(Easing.InBack)
                .PlayAsyncGD(linked.Token);

            // 必须在碰节点之前: 已排队的续体无法取消, 这是唯一能挡住它的手段
            linked.Token.ThrowIfCancellationRequested();

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