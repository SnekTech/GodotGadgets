using System.Runtime.CompilerServices;
using Dumpify;

// use Godot namespace for convenience
namespace Godot;

public static class ExtraGodotExtensions
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
}