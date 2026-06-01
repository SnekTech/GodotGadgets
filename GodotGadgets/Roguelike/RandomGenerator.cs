using GodotGadgets.Roguelike.PCG;

namespace GodotGadgets.Roguelike;

public class RandomGenerator(RngData data)
{
    private Pcg32 _generator = new(data.Seed, data.State);

    public void Reset(RngData data)
    {
        var (seed, state) = data;
        _generator = new Pcg32(seed, state);
    }

    public ulong Seed => _generator.Seed;

    public ulong State
    {
        get => _generator.State;
        set => _generator.State = value;
    }

    public int PickInt() => (int)_generator.GenerateNext();

    public bool PickBool() => _generator.GenerateNext() > (uint.MaxValue / 2);

    public float PickFloat() => _generator.GenerateNext() / (float)uint.MaxValue;
}