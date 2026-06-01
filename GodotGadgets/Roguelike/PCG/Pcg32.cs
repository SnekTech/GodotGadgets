namespace GodotGadgets.Roguelike.PCG;

/// <summary>
/// Implements the <code>pcg32</code> random number generator described
/// at <a href="http://www.pcg-random.org/using-pcg-c.html">http://www.pcg-random.org/using-pcg-c.html</a>.
/// </summary>
public sealed class Pcg32
{
	private readonly ulong _inc;

	/// <summary>
	/// Initializes a new instance of the <see cref="Pcg32"/> pseudorandom number generator.
	/// </summary>
	/// <param name="seed">The starting state for the RNG; you can pass any 64-bit value.</param>
	/// <param name="state">The output sequence for the RNG; you can pass any 64-bit value, although only the low
	/// 63 bits are significant.</param>
	/// <remarks>For this generator, there are 2<sup>63</sup> possible sequences of pseudorandom numbers. Each sequence
	/// is entirely distinct and has a period of 2<sup>64</sup>. The <paramref name="seed"/> argument selects which
	/// stream you will use. The <paramref name="state"/> argument specifies where you are in that 2<sup>64</sup> period.</remarks>
	public Pcg32(ulong seed, ulong state)
	{
		Seed = seed;
		// implements pcg_setseq_64_srandom_r
		_inc = (seed << 1) | 1u;
		Step();
		State += state;
		Step();
	}

	public ulong Seed { get; }

	public ulong State { get; set; }

	/// <summary>
	/// Generates the next random number.
	/// </summary>
	/// <returns></returns>
	public uint GenerateNext()
	{
		// implements pcg_setseq_64_xsh_rr_32_random_r
		var oldState = State;
		Step();
		return Helpers.OutputXshRr(oldState);
	}

	/// <summary>
	/// Generates a uniformly distributed 32-bit unsigned integer less than <paramref name="bound"/> (i.e., <c>x</c> where
	/// <c>0 &lt;= x &lt; bound</c>.
	/// </summary>
	/// <param name="bound">The exclusive upper bound of the random number to be generated.</param>
	/// <returns>A random number between <c>0</c> and <paramref name="bound"/> (exclusive).</returns>
	public uint GenerateNext(uint bound)
	{
		// implements pcg_setseq_64_xsh_rr_32_boundedrand_r
		uint threshold = ((uint) -bound) % bound;
		while (true)
		{
			uint r = GenerateNext();
			if (r >= threshold)
				return r % bound;
		}
	}

	/// <summary>
	/// Advances the RNG by <paramref name="delta"/> steps, doing so in <c>log(delta)</c> time.
	/// </summary>
	/// <param name="delta">The number of steps to advance; pass <c>2<sup>64</sup> - delta</c> (i.e., <c>-delta</c>) to go backwards.</param>
	public void Advance(ulong delta)
	{
		// implements pcg_setseq_64_advance_r
		State = Helpers.Advance(State, delta, Helpers.Multiplier64, _inc);
	}
	
	private void Step()
	{
		// corresponds to pcg_setseq_64_step_r
		State = unchecked(State * Helpers.Multiplier64 + _inc);
	}

}