using System.Numerics;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class MersenneTwisterTests
{
	[TestMethod]
	public void DefaultConstructor_MatchesReferenceInitByArray()
	{
		// Reference output of mt19937ar.c: init_by_array({0x123, 0x234, 0x345, 0x456}).
		uint[] expected = { 1067595299u, 955945823u, 477289528u, 4107218783u, 4228976476u, 3344332714u, 3355579695u, 227628506u, 810200273u, 2591290167u };
		var mt = new MersenneTwister();
		foreach (uint value in expected)
			Assert.AreEqual(value, mt.GetUInt32());
	}

	[TestMethod]
	public void KeyConstructor_MatchesDefaultConstructorForSameKey()
	{
		var a = new MersenneTwister();
		var b = new MersenneTwister(new[] { 0x123, 0x234, 0x345, 0x456 });
		for (int i = 0; i < 1000; i++)
			Assert.AreEqual(a.GetUInt32(), b.GetUInt32());
	}

	[TestMethod]
	public void SeedConstructor_MatchesReferenceInitGenrand()
	{
		// Reference output of init_genrand(5489), the standard std::mt19937 default seed.
		uint[] expected = { 3499211612u, 581869302u, 3890346734u, 3586334585u, 545404204u };
		var mt = new MersenneTwister(5489);
		foreach (uint value in expected)
			Assert.AreEqual(value, mt.GetUInt32());

		// The 10000th output of std::mt19937 with the default seed.
		var std = new MersenneTwister(5489);
		uint last = 0;
		for (int i = 0; i < 10000; i++)
			last = std.GetUInt32();
		Assert.AreEqual(4123659995u, last);
	}

	[TestMethod]
	public void NullKey_FallsBackToDefaultSeed()
	{
		var a = new MersenneTwister((int[])null);
		var b = new MersenneTwister(5489);
		for (int i = 0; i < 10; i++)
			Assert.AreEqual(b.GetUInt32(), a.GetUInt32());
	}

	[TestMethod]
	public void SameSeed_SameSequence_DifferentSeed_DifferentSequence()
	{
		var a = new MersenneTwister(42);
		var b = new MersenneTwister(42);
		var c = new MersenneTwister(43);
		bool differs = false;
		for (int i = 0; i < 100; i++)
		{
			uint va = a.GetUInt32();
			Assert.AreEqual(va, b.GetUInt32());
			differs |= va != c.GetUInt32();
		}
		Assert.IsTrue(differs);
	}

	[TestMethod]
	public void DerivedOutputs_AreConsistentWithUInt32()
	{
		var a = new MersenneTwister(7);
		var b = new MersenneTwister(7);
		for (int i = 0; i < 100; i++)
		{
			uint u = a.GetUInt32();
			Assert.AreEqual((int)(u >> 1), b.GetInt31());
			u = a.GetUInt32();
			Assert.AreEqual((int)u, b.GetInt32());
			u = a.GetUInt32();
			Assert.AreEqual(u*(1.0/4294967295.0), b.GetDouble01());
			u = a.GetUInt32();
			Assert.AreEqual(u*(1.0/4294967296.0), b.GetDouble01RightOpen());
			u = a.GetUInt32();
			Assert.AreEqual((u + 0.5)*(1.0/4294967296.0), b.GetDouble01Open());
			u = a.GetUInt32();
			Assert.AreEqual((float)(u*(1.0/4294967295.0)), b.GetSingle01());
			// Right-open and open single-precision values are rounded down, never up to 1.
			u = a.GetUInt32();
			Assert.AreEqual(FloorToSingle(u*(1.0/4294967296.0)), b.GetSingle01RightOpen());
			u = a.GetUInt32();
			Assert.AreEqual(FloorToSingle((u + 0.5)*(1.0/4294967296.0)), b.GetSingle01Open());
			uint hi = a.GetUInt32() >> 5, lo = a.GetUInt32() >> 6;
			Assert.AreEqual((hi*67108864.0 + lo)*(1.0/9007199254740992.0), b.GetDouble01RightOpen53Bits());
		}
	}

	/// <summary>Largest float not greater than x, computed independently of the library.</summary>
	internal static float FloorToSingle(double x)
	{
		float f = (float)x;
		while ((double)f > x)
			f = MathF.BitDecrement(f);
		return f;
	}

	[TestMethod]
	public void FloatingPointOutputs_StayInRange()
	{
		var mt = new MersenneTwister(2024);
		for (int i = 0; i < 10000; i++)
		{
			double d = mt.GetDouble01();
			Assert.IsTrue(d >= 0.0 && d <= 1.0);
			d = mt.GetDouble01RightOpen();
			Assert.IsTrue(d >= 0.0 && d < 1.0);
			d = mt.GetDouble01Open();
			Assert.IsTrue(d > 0.0 && d < 1.0);
			d = mt.GetDouble01RightOpen53Bits();
			Assert.IsTrue(d >= 0.0 && d < 1.0);
			float f = mt.GetSingle01();
			Assert.IsTrue(f >= 0f && f <= 1f);
			f = mt.GetSingle01RightOpen();
			Assert.IsTrue(f >= 0f && f < 1f);
			f = mt.GetSingle01Open();
			Assert.IsTrue(f > 0f && f < 1f);
			Assert.IsTrue(mt.GetInt31() >= 0);
		}
	}

	[TestMethod]
	public void ImplementsGenericGeneratorInterface()
	{
		IRandomNumberGenerator<uint> generator = new MersenneTwister(5489);
		Assert.AreEqual(UInt32.MaxValue, generator.MaxValue);
		Assert.AreEqual(3499211612u, generator.GetNext());
	}
}

[TestClass]
[DoNotParallelize] // Some tests use the shared Random.DefaultGenerator.
public sealed class RandomTests
{
	/// <summary>Generator returning a fixed value, for probing range boundaries.</summary>
	private sealed class ConstantGenerator : IRandomNumberGenerator<uint>
	{
		private readonly uint value_;
		public ConstantGenerator(uint value) { value_ = value; }
		public uint MaxValue => UInt32.MaxValue;
		public uint GetNext() => value_;
	}

	private static MersenneTwister Rng() => new MersenneTwister(12345);

	[TestMethod]
	public void GetInt32_IsInclusiveAndRoughlyUniform()
	{
		var rng = Rng();
		int[] counts = new int[4];
		const int samples = 40000;
		for (int i = 0; i < samples; i++)
		{
			int v = Random.GetInt32(-1, 2, rng);
			Assert.IsTrue(v >= -1 && v <= 2, v.ToString());
			counts[v + 1]++;
		}
		foreach (int count in counts)
			Assert.IsTrue(Math.Abs(count - samples/4) < samples/40, String.Join(", ", counts));
		Assert.AreEqual(5, Random.GetInt32(5, 5, rng));
	}

	[TestMethod]
	public void GetInt32_Boundaries()
	{
		Assert.AreEqual(0, Random.GetInt32(0, 9, new ConstantGenerator(0u)));
		Assert.AreEqual(9, Random.GetInt32(0, 9, new ConstantGenerator(UInt32.MaxValue)));
	}

	[TestMethod]
	public void RealRanges_WithSeededGenerator()
	{
		var rng = Rng();
		double sum = 0.0;
		const int samples = 20000;
		for (int i = 0; i < samples; i++)
		{
			float f = Random.GetSingle(-2f, 3f, rng);
			Assert.IsTrue(f >= -2f && f <= 3f);
			double d = Random.GetDouble(10.0, 11.0, rng);
			Assert.IsTrue(d >= 10.0 && d <= 11.0);
			sum += d;
			f = Random.GetSingleRightOpen(-2f, 3f, rng);
			Assert.IsTrue(f >= -2f && f <= 3f);
			d = Random.GetDoubleRightOpen(10.0, 11.0, rng);
			Assert.IsTrue(d >= 10.0 && d <= 11.0);
			f = Random.GetSingle01(rng);
			Assert.IsTrue(f >= 0f && f <= 1f);
			d = Random.GetDouble01(rng);
			Assert.IsTrue(d >= 0.0 && d <= 1.0);
			f = Random.GetSingle01RightOpen(rng);
			Assert.IsTrue(f >= 0f && f <= 1f);
			d = Random.GetDouble01RightOpen(rng);
			Assert.IsTrue(d >= 0.0 && d < 1.0);
		}
		Assert.AreEqual(10.5, sum/samples, 0.01);
	}

	[TestMethod]
	public void ClosedRanges_IncludeButDoNotExceedUpperBound()
	{
		var max = new ConstantGenerator(UInt32.MaxValue);
		var zero = new ConstantGenerator(0u);
		Assert.AreEqual(0.0, Random.GetDouble01(zero));
		Assert.AreEqual(0f, Random.GetSingle01(zero));
		Assert.AreEqual(-2.0, Random.GetDouble(-2.0, 3.0, zero));
		Assert.IsTrue(Random.GetDouble01(max) <= 1.0, $"GetDouble01 returned {Random.GetDouble01(max):R}");
		Assert.IsTrue(Random.GetDouble(-2.0, 3.0, max) <= 3.0, $"GetDouble returned {Random.GetDouble(-2.0, 3.0, max):R}");
		Assert.IsTrue(Random.GetSingle01(max) <= 1f);
		Assert.AreEqual(1.0, Random.GetDouble01(max));
		Assert.AreEqual(1f, Random.GetSingle01(max));
		Assert.AreEqual(3.0, Random.GetDouble(-2.0, 3.0, max));
		Assert.AreEqual(3f, Random.GetSingle(-2f, 3f, max));
	}

	[TestMethod]
	public void RightOpenRanges_ExcludeUpperBound()
	{
		var max = new ConstantGenerator(UInt32.MaxValue);
		Assert.IsTrue(Random.GetDouble01RightOpen(max) < 1.0, $"GetDouble01RightOpen returned {Random.GetDouble01RightOpen(max):R}");
		Assert.IsTrue(Random.GetDoubleRightOpen(0.0, 2.0, max) < 2.0, $"GetDoubleRightOpen returned {Random.GetDoubleRightOpen(0.0, 2.0, max):R}");
		// The double result is below 1 but would round to 1f/max in single precision.
		Assert.IsTrue(Random.GetSingle01RightOpen(max) < 1f, $"GetSingle01RightOpen returned {Random.GetSingle01RightOpen(max):R}");
		Assert.IsTrue(Random.GetSingleRightOpen(0f, 2f, max) < 2f, $"GetSingleRightOpen returned {Random.GetSingleRightOpen(0f, 2f, max):R}");
		Assert.IsTrue(Random.GetSingleRightOpen(1e6f, 1e6f + 1f, max) < 1e6f + 1f);
		Assert.IsTrue(Random.GetDoubleRightOpen(1e15, 1e15 + 1.0, max) < 1e15 + 1.0);
		var zero = new ConstantGenerator(0u);
		Assert.AreEqual(0.0, Random.GetDouble01RightOpen(zero));
		Assert.AreEqual(-2f, Random.GetSingleRightOpen(-2f, 3f, zero));
	}

	// Generator outputs near the top of the range, where rounding to the nearest float would give exactly 1.
	private static readonly uint[] EdgeOutputs =
	{
		0u, 1u, 3u, 0xFFFFFFu, 0x80000000u, 0x80000081u, 0xFFFFFF7Fu, 0xFFFFFF80u, 0xFFFFFF81u, UInt32.MaxValue - 1u, UInt32.MaxValue,
	};

	[TestMethod]
	public void GetSingle01RightOpen_RoundsDown()
	{
		foreach (uint next in EdgeOutputs)
		{
			float expected = MersenneTwisterTests.FloorToSingle(next/4294967296.0);
			float actual = Random.GetSingle01RightOpen(new ConstantGenerator(next));
			Assert.AreEqual(expected, actual, $"next = {next:X8}");
			Assert.IsTrue(actual < 1f);
		}
		// In [0.5, 1) floats are 2^-24 apart, so the result keeps the top 24 bits of the generator output.
		Assert.AreEqual((0xFFFFFFFFu >> 8)/16777216f, Random.GetSingle01RightOpen(new ConstantGenerator(UInt32.MaxValue)));
		Assert.AreEqual(3f/4294967296f, Random.GetSingle01RightOpen(new ConstantGenerator(3u)));
	}

	[TestMethod]
	public void GetSingleRightOpen_RoundsTowardsMin()
	{
		foreach (uint next in EdgeOutputs)
		{
			var generator = new ConstantGenerator(next);
			// The closed [0, 1] variant is scaled; only an exact 1 reaches max and is moved to the float below it.
			double u = next/(double)UInt32.MaxValue;
			float expected = (next == UInt32.MaxValue) ? MathF.BitDecrement(2f) : MersenneTwisterTests.FloorToSingle(2.0*u);
			Assert.AreEqual(expected, Random.GetSingleRightOpen(0f, 2f, generator), $"next = {next:X8}");
			float shifted = Random.GetSingleRightOpen(-2f, 3f, generator);
			Assert.IsTrue(shifted >= -2f && shifted < 3f, $"next = {next:X8}: {shifted:R}");
			expected = (next == UInt32.MaxValue) ? MathF.BitDecrement(3f) : MersenneTwisterTests.FloorToSingle(-2.0 + 5.0*u);
			Assert.AreEqual(expected, shifted, $"next = {next:X8}");
		}
		// The largest result is the float just below max, even when max is tiny compared to the range.
		var max = new ConstantGenerator(UInt32.MaxValue);
		Assert.AreEqual(MathF.BitDecrement(1f), Random.GetSingleRightOpen(-1000f, 1f, max));
		Assert.AreEqual(MathF.BitDecrement(1f), Random.GetSingleRightOpen(-1e9f, 1f, max));
	}

	[TestMethod]
	public void GetSingleRightOpen_HandlesExtremeRanges()
	{
		var zero = new ConstantGenerator(0u);
		var half = new ConstantGenerator(0x80000000u);
		var max = new ConstantGenerator(UInt32.MaxValue);

		// max - min overflows single precision.
		Assert.AreEqual(-3e38f, Random.GetSingleRightOpen(-3e38f, 3e38f, zero));
		Assert.IsTrue(Math.Abs(Random.GetSingleRightOpen(-3e38f, 3e38f, half)) < 1e30f);
		Assert.AreEqual(MathF.BitDecrement(3e38f), Random.GetSingleRightOpen(-3e38f, 3e38f, max));

		// A range one float wide only contains its minimum.
		float lo = 1e6f, hi = MathF.BitIncrement(1e6f);
		foreach (var generator in new[] { zero, half, max })
			Assert.AreEqual(lo, Random.GetSingleRightOpen(lo, hi, generator));
	}

	/// <summary>Exact value of a finite double times 2^1074 (every finite double is a multiple of 2^-1074).</summary>
	private static BigInteger Scaled(double x)
	{
		long bits = BitConverter.DoubleToInt64Bits(x);
		int exponent = (int)((bits >> 52) & 0x7FF);
		long mantissa = bits & 0xFFFFFFFFFFFFFL;
		if (exponent != 0)
			mantissa |= 1L << 52;
		else
			exponent = 1;
		BigInteger value = new BigInteger(mantissa) << (exponent - 1);
		return (bits < 0) ? -value : value;
	}

	/// <summary>
	/// Asserts that result lies in [min, max) and within three ulps of min + (max - min)*next/(2^32 - 1), using exact
	/// arithmetic (all quantities scaled by 2^1074*(2^32 - 1)). The ulp is that of the larger bound, as the result
	/// of a range spanning zero cannot be more precise than its bounds. Three ulps cover the rounding of the
	/// interpolation min*(1 - u) + max*u: the error of u counts against both bounds, plus two products and a sum.
	/// </summary>
	private static void AssertInRangeAndClose(double min, double max, uint next, double result)
	{
		string context = $"[{min:R}, {max:R}) next = {next:X8}: {result:R}";
		Assert.IsTrue(result >= min && result < max, context);
		BigInteger exact = Scaled(min)*(UInt32.MaxValue - next) + Scaled(max)*next;
		double magnitude = Math.Max(Math.Abs(min), Math.Abs(max));
		BigInteger ulp = Scaled(magnitude) - Scaled(Math.BitDecrement(magnitude));
		Assert.IsTrue(BigInteger.Abs(Scaled(result)*UInt32.MaxValue - exact) <= 3*ulp*UInt32.MaxValue, context);
	}

	// Ascending ranges only: the right-open functions require min < max.
	// No range is wider than Double.MaxValue: max - min overflows for those, which the double functions do not support.
	private static readonly (double min, double max)[] AscendingDoubleRanges =
	{
		(0.0, 1.0), (-2.0, 3.0), (1e15, 1e15 + 1.0), (-1e10, 1e-5), (1e-300, 1e300), (-8e307, 8e307),
		(1.0, Math.BitIncrement(1.0)), (5e-324, 1e-320),
	};

	private static readonly (double min, double max)[] DoubleRanges =
		AscendingDoubleRanges.Concat(new[] { (3.0, -2.0), (1e15 + 1.0, 1e15), (8e307, -8e307) }).ToArray();

	[TestMethod]
	public void GetDoubleRightOpen_StaysInRangeAndClose()
	{
		var mt = new MersenneTwister(77);
		IEnumerable<uint> outputs = EdgeOutputs.Concat(Enumerable.Range(0, 200).Select(_ => mt.GetUInt32()));
		foreach (uint next in outputs)
			foreach (var (min, max) in AscendingDoubleRanges)
				AssertInRangeAndClose(min, max, next, Random.GetDoubleRightOpen(min, max, new ConstantGenerator(next)));
	}

	[TestMethod]
	public void GetDoubleRightOpen_NarrowRange_CoversEveryValue()
	{
		// [1e15, 1e15 + 1) holds 8 doubles, 0.125 apart; all of them, and nothing else, must be produced.
		const double min = 1e15, max = 1e15 + 1.0;
		var values = new HashSet<double>();
		for (uint k = 0; k < 1 << 16; k++)
			values.Add(Random.GetDoubleRightOpen(min, max, new ConstantGenerator(k << 16)));
		Assert.HasCount(8, values);
		Assert.IsTrue(values.All(v => v >= min && v < max));
	}

	[TestMethod]
	public void GetDoubleRightOpen_ExcludesUpperBound()
	{
		var max = new ConstantGenerator(UInt32.MaxValue);
		Assert.AreEqual(1.0, Random.GetDoubleRightOpen(1.0, Math.BitIncrement(1.0), max));
		// The largest result is always the value just below max, whatever the size of the range.
		Assert.AreEqual(Math.BitDecrement(8e307), Random.GetDoubleRightOpen(-8e307, 8e307, max));
		Assert.AreEqual(Math.BitDecrement(1.0), Random.GetDoubleRightOpen(-1e10, 1.0, max));
		Assert.AreEqual(Math.BitDecrement(3.0), Random.GetDoubleRightOpen(-2.0, 3.0, max));
	}

	[TestMethod]
	public void ClosedRanges_HandleWideRanges()
	{
		var zero = new ConstantGenerator(0u);
		var half = new ConstantGenerator(UInt32.MaxValue/2u + 1u);
		var max = new ConstantGenerator(UInt32.MaxValue);
		// Double ranges wider than Double.MaxValue are not supported (max - min overflows), so these stay below it.
		Assert.AreEqual(-8e307, Random.GetDouble(-8e307, 8e307, zero));
		Assert.AreEqual(8e307, Random.GetDouble(-8e307, 8e307, max));
		Assert.AreEqual(8e307, Random.GetDouble(8e307, -8e307, zero));
		Assert.IsTrue(Math.Abs(Random.GetDouble(-8e307, 8e307, half)) < 1e300);
		// Single ranges are computed in double precision, so their full width is supported.
		Assert.AreEqual(-3e38f, Random.GetSingle(-3e38f, 3e38f, zero));
		Assert.AreEqual(3e38f, Random.GetSingle(-3e38f, 3e38f, max));
		Assert.IsTrue(Math.Abs(Random.GetSingle(-3e38f, 3e38f, half)) < 1e30f);
		Assert.AreEqual(Single.MaxValue, Random.GetSingle(-Single.MaxValue, Single.MaxValue, max));
	}

	[TestMethod]
	public void ClosedRanges_StayWithinBounds()
	{
		var mt = new MersenneTwister(78);
		foreach (uint next in EdgeOutputs.Concat(Enumerable.Range(0, 200).Select(_ => mt.GetUInt32())))
		{
			var generator = new ConstantGenerator(next);
			foreach (var (min, max) in DoubleRanges)
			{
				double v = Random.GetDouble(min, max, generator);
				Assert.IsTrue(v >= Math.Min(min, max) && v <= Math.Max(min, max), $"[{min:R}, {max:R}] {next:X8}: {v:R}");
				double u = next/(double)UInt32.MaxValue;
				double expected = min*(1.0 - u) + max*u; // overflow-free reference
				Assert.IsTrue(Math.Abs(v - expected) <= Math.Max(1e-12*Math.Max(Math.Abs(min), Math.Abs(max)), 4.0*Double.Epsilon), $"[{min:R}, {max:R}] {next:X8}: {v:R} vs {expected:R}");
			}
			float f = Random.GetSingle(-2f, 3f, generator);
			Assert.IsTrue(f >= -2f && f <= 3f);
		}
		Assert.AreEqual(3.0, Random.GetDouble(-2.0, 3.0, new ConstantGenerator(UInt32.MaxValue)));
		Assert.AreEqual(-2.0, Random.GetDouble(-2.0, 3.0, new ConstantGenerator(0u)));
	}

	[TestMethod]
	public void VectorsOnAndInUnitShapes()
	{
		var rng = Rng();
		for (int i = 0; i < 1000; i++)
		{
			AssertEx.AreEqual(1f, Random.GetVectorOnUnitCircle(rng).Magnitude);
			Assert.IsTrue(Random.GetVectorOnUnitDisk(rng).Magnitude <= 1f);
			Assert.IsTrue(Random.GetVectorInUnitSphere(rng).Magnitude <= 1f);
			AssertEx.AreEqual(1f, Random.GetVectorOnUnitSphere(rng).Magnitude);
		}
	}

	[TestMethod]
	public void VectorsOnUnitSphere_AreSpreadOverBothHemispheres()
	{
		var rng = Rng();
		Vector3 mean = Vector3.Zero;
		const int samples = 5000;
		for (int i = 0; i < samples; i++)
			mean += Random.GetVectorOnUnitSphere(rng);
		mean /= samples;
		Assert.IsTrue(mean.Magnitude < 0.05f, mean.ToString());
	}

	[TestMethod]
	public void VectorsOnSphericalCone_StayWithinHalfAngle()
	{
		var rng = Rng();
		const float halfAngle = 0.3f;
		Vector3 direction = Vector3.Normalize(new Vector3(1f, -2f, 0.5f));
		for (int i = 0; i < 1000; i++)
		{
			Vector3 v = Random.GetVectorOnUnitSphericalCone(halfAngle, rng);
			AssertEx.AreEqual(1f, v.Magnitude);
			Assert.IsTrue(v.Z >= MathF.Cos(halfAngle) - 1e-6f);

			Vector3 w = Random.GetVectorOnUnitSphericalCone(direction, halfAngle, rng);
			AssertEx.AreEqual(1f, w.Magnitude, 1e-5f);
			Assert.IsTrue(Vector3.Angle(direction, w) <= halfAngle + 1e-3f);

			Vector3 z = Random.GetVectorOnUnitSphericalCone(Vector3.UnitZ, halfAngle, rng);
			Assert.IsTrue(Vector3.Angle(Vector3.UnitZ, z) <= halfAngle + 1e-3f);
		}
	}

	[TestMethod]
	public void RotationMatricesAndQuaternions_AreProperRotations()
	{
		var rng = Rng();
		for (int i = 0; i < 200; i++)
		{
			Matrix3 m = Random.GetRotationMatrix(rng);
			AssertEx.IsOrthonormal(m, 1e-5f);
			AssertEx.AreEqual(1f, m.Determinant, 1e-5f);
			AssertEx.AreEqual(1f, Random.GetQuaternion(rng).Magnitude);
		}
	}

	[TestMethod]
	public void DefaultGeneratorOverloads_ProduceValuesInRange()
	{
		Assert.IsNotNull(Random.DefaultGenerator);
		for (int i = 0; i < 100; i++)
		{
			int n = Random.GetInt32(1, 6);
			Assert.IsTrue(n >= 1 && n <= 6);
			float f = Random.GetSingle(1f, 2f);
			Assert.IsTrue(f >= 1f && f <= 2f);
			double d = Random.GetDouble(1.0, 2.0);
			Assert.IsTrue(d >= 1.0 && d <= 2.0);
			Assert.IsTrue(Random.GetSingleRightOpen(1f, 2f) >= 1f);
			Assert.IsTrue(Random.GetDoubleRightOpen(1.0, 2.0) >= 1.0);
			Assert.IsTrue(Random.GetSingle01() >= 0f);
			Assert.IsTrue(Random.GetDouble01() >= 0.0);
			Assert.IsTrue(Random.GetSingle01RightOpen() >= 0f);
			Assert.IsTrue(Random.GetDouble01RightOpen() >= 0.0);
			AssertEx.AreEqual(1f, Random.GetVectorOnUnitCircle().Magnitude);
			Assert.IsTrue(Random.GetVectorOnUnitDisk().Magnitude <= 1f);
			Assert.IsTrue(Random.GetVectorInUnitSphere().Magnitude <= 1f);
			AssertEx.AreEqual(1f, Random.GetVectorOnUnitSphere().Magnitude);
			Assert.IsTrue(Random.GetVectorOnUnitSphericalCone(0.5f).Z >= MathF.Cos(0.5f) - 1e-6f);
			Assert.IsTrue(Vector3.Angle(Vector3.UnitX, Random.GetVectorOnUnitSphericalCone(Vector3.UnitX, 0.5f)) <= 0.5f + 1e-3f);
			AssertEx.IsOrthonormal(Random.GetRotationMatrix(), 1e-5f);
			AssertEx.AreEqual(1f, Random.GetQuaternion().Magnitude);
		}
	}
}

[TestClass]
public sealed class PerlinTests
{
	[TestMethod]
	public void MatchesReferenceImplementation()
	{
		// Value printed by Ken Perlin's reference ImprovedNoise implementation.
		Assert.AreEqual(0.13691995878400012, Perlin.Noise(3.14, 42.0, 7.0), 1e-12);
		AssertEx.AreEqual(0.13691995878400012f, Perlin.Noise(3.14f, 42f, 7f), 1e-4f);
	}

	[TestMethod]
	public void IsZeroAtLatticePoints()
	{
		for (int i = -3; i <= 3; i++)
		{
			Assert.AreEqual(0f, Perlin.Noise((float)i));
			Assert.AreEqual(0.0, Perlin.Noise((double)i));
			Assert.AreEqual(0f, Perlin.Noise((float)i, 2f*i + 1f));
			Assert.AreEqual(0.0, Perlin.Noise((double)i, 5.0));
			Assert.AreEqual(0f, Perlin.Noise((float)i, 7f, -2f*i));
			Assert.AreEqual(0.0, Perlin.Noise((double)i, -1.0, 3.0));
		}
	}

	[TestMethod]
	public void IsBoundedDeterministicAndNonTrivial()
	{
		float maxAbs = 0f;
		for (int i = 0; i < 2000; i++)
		{
			float x = i*0.137f - 50f, y = i*0.071f + 3.3f, z = i*0.029f - 7.1f;
			float n1 = Perlin.Noise(x), n2 = Perlin.Noise(x, y), n3 = Perlin.Noise(x, y, z);
			foreach (float n in new[] { n1, n2, n3 })
				Assert.IsTrue(n >= -1.0001f && n <= 1.0001f, n.ToString());
			Assert.AreEqual(n3, Perlin.Noise(x, y, z));
			maxAbs = Math.Max(maxAbs, Math.Max(Math.Abs(n1), Math.Max(Math.Abs(n2), Math.Abs(n3))));
		}
		Assert.IsTrue(maxAbs > 0.3f);
	}

	[TestMethod]
	public void SingleAndDoubleVariantsAgree()
	{
		for (int i = 0; i < 200; i++)
		{
			float x = i*0.37f - 20f, y = i*0.11f + 0.5f, z = -i*0.23f;
			AssertEx.AreEqual((float)Perlin.Noise((double)x), Perlin.Noise(x), 1e-4f);
			AssertEx.AreEqual((float)Perlin.Noise((double)x, y), Perlin.Noise(x, y), 1e-4f);
			AssertEx.AreEqual((float)Perlin.Noise((double)x, y, z), Perlin.Noise(x, y, z), 1e-4f);
		}
	}

	[TestMethod]
	public void VectorOverloads_MatchScalarOverloads()
	{
		Assert.AreEqual(Perlin.Noise(1.3f, -2.7f), Perlin.Noise(new Vector2(1.3f, -2.7f)));
		Assert.AreEqual(Perlin.Noise(1.3f, -2.7f, 0.4f), Perlin.Noise(new Vector3(1.3f, -2.7f, 0.4f)));
	}

	[TestMethod]
	public void IsContinuous_AndPeriodicWith256()
	{
		for (int i = 0; i < 200; i++)
		{
			double x = i*0.173 - 10.0, y = i*0.091, z = i*0.057 + 1.0;
			Assert.IsTrue(Math.Abs(Perlin.Noise(x, y, z) - Perlin.Noise(x + 1e-4, y, z)) < 1e-3);
			Assert.IsTrue(Math.Abs(Perlin.Noise(x, y) - Perlin.Noise(x, y + 1e-4)) < 1e-3);
			Assert.AreEqual(Perlin.Noise(x, y, z), Perlin.Noise(x + 256.0, y, z), 1e-9);
			Assert.AreEqual(Perlin.Noise(x), Perlin.Noise(x - 256.0), 1e-9);
		}
	}
}
