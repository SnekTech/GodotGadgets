using System.Runtime.CompilerServices;
using Dumpify;

namespace GodotGadgets.Extensions;

public static class CommonExtensions
{
    public static void DumpGd<T>(
        this T? obj,
        string? label = null,
        int? maxDepth = null,
        IRenderer? renderer = null,
        bool? useDescriptors = null,
        ColorConfig? colors = null,
        MembersConfig? members = null,
        TypeNamingConfig? typeNames = null,
        TableConfig? tableConfig = null,
        OutputConfig? outputConfig = null,
        TypeRenderingConfig? typeRenderingConfig = null,
        [CallerArgumentExpression(nameof(obj))]
        string? autoLabel = null
    )
    {
        members = members is null ? new MembersConfig { IncludeFields = true } : null;

        GD.Print(obj.DumpText(
            label,
            maxDepth,
            renderer,
            useDescriptors,
            colors,
            members,
            typeNames,
            tableConfig,
            outputConfig,
            typeRenderingConfig,
            autoLabel
        ));
    }

    public static T PickRandom<T>(this List<T> list)
    {
        var randomIndex = GD.RandRange(0, list.Count - 1);
        return list[randomIndex];
    }

    public static void Shuffle<T>(this List<T> list)
    {
        var random = new Random();
        var array = list.ToArray();
        random.Shuffle(array);
        list.Clear();
        list.AddRange(array);
    }

    public static float Clamp01(this float x) => float.Clamp(x, 0, 1);
    public static double Clamp01(this double x) => double.Clamp(x, 0, 1);

    public static Vector2I ToVector2I(this Vector2 vector2)
    {
        var (x, y) = vector2.Round();
        return new Vector2I((int)x, (int)y);
    }
}