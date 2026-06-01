namespace GodotGadgets.Roguelike;

public readonly record struct RngData(ulong Seed, ulong State)
{
    public static RngData Empty => new(0, 0);
}