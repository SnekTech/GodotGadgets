namespace GodotGadgets.TooltipSystem;

public readonly record struct TooltipContent(string Title, string Content);

public static class TooltipContentExtensions
{
    extension(TooltipContent)
    {
        public static TooltipContent New(string title, string content) => new(title, content);
    }
}