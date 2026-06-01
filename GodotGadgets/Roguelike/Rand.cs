namespace GodotGadgets.Roguelike;

public static class Rand
{
    private static readonly RandomGenerator Generator = new(RngData.Empty);

    public static void Reset(RngData data) => Generator.Reset(data);

    public static void RandomizeSeed()
    {
        var seed = (ulong)new Random().Next();
        Reset(new RngData(seed, 0));
    }

    public static float Float() => Generator.PickFloat();
    public static RngData Data => new(Generator.Seed, Generator.State);
}