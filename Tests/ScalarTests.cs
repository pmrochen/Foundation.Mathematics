namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class ScalarTests
{
	[TestMethod]
	public void IsFinite_DetectsInfinityAndNaN()
	{
		Assert.IsTrue(Scalar.IsFinite(1.5f));
		Assert.IsFalse(Scalar.IsFinite(Single.PositiveInfinity));
		Assert.IsFalse(Scalar.IsFinite(Single.NegativeInfinity));
		Assert.IsFalse(Scalar.IsFinite(Single.NaN));
		Assert.IsTrue(Scalar.IsFinite(-2.0));
		Assert.IsFalse(Scalar.IsFinite(Double.PositiveInfinity));
		Assert.IsFalse(Scalar.IsFinite(Double.NaN));
	}

	[TestMethod]
	public void Log2_ReturnsBinaryLogarithm()
	{
		AssertEx.AreEqual(3f, Scalar.Log2(8f));
		AssertEx.AreEqual(-1f, Scalar.Log2(0.5f));
		Assert.AreEqual(10.0, Scalar.Log2(1024.0), 1e-12);
	}

	[TestMethod]
	public void Square_MultipliesValueBySelf()
	{
		Assert.AreEqual(6.25f, Scalar.Square(-2.5f));
		Assert.AreEqual(9.0, Scalar.Square(3.0));
	}

	[TestMethod]
	public void Sign_ReturnsMinusOneZeroOrOne()
	{
		Assert.AreEqual(1, Scalar.Sign(7));
		Assert.AreEqual(-1, Scalar.Sign(-7));
		Assert.AreEqual(0, Scalar.Sign(0));
		Assert.AreEqual(1f, Scalar.Sign(0.1f));
		Assert.AreEqual(-1f, Scalar.Sign(-0.1f));
		Assert.AreEqual(0f, Scalar.Sign(0f));
		Assert.AreEqual(1.0, Scalar.Sign(2.0));
		Assert.AreEqual(-1.0, Scalar.Sign(-2.0));
		Assert.AreEqual(0.0, Scalar.Sign(0.0));
	}

	[TestMethod]
	public void Round_RoundsHalfAwayFromZero()
	{
		Assert.AreEqual(3, Scalar.Round(2.5f));
		Assert.AreEqual(-3, Scalar.Round(-2.5f));
		Assert.AreEqual(2, Scalar.Round(2.4f));
		Assert.AreEqual(-2, Scalar.Round(-2.4f));
		Assert.AreEqual(3, Scalar.Round(2.5));
		Assert.AreEqual(-3, Scalar.Round(-2.5));
		Assert.AreEqual(0, Scalar.Round(0.0));
	}

	[TestMethod]
	public void Truncate_RoundsTowardsZero()
	{
		Assert.AreEqual(2, Scalar.Truncate(2.9f));
		Assert.AreEqual(-2, Scalar.Truncate(-2.9f));
		Assert.AreEqual(2, Scalar.Truncate(2.9));
		Assert.AreEqual(-2, Scalar.Truncate(-2.9));
	}

	[TestMethod]
	public void Fractional_ReturnsValueMinusFloor()
	{
		AssertEx.AreEqual(0.25f, Scalar.Fractional(3.25f));
		AssertEx.AreEqual(0.75f, Scalar.Fractional(-3.25f));
		Assert.AreEqual(0.5, Scalar.Fractional(-0.5), 1e-12);
	}

	[TestMethod]
	public void RadiansAndDegrees_AreInverse()
	{
		AssertEx.AreEqual(SingleConstants.Pi, Scalar.Radians(180f));
		AssertEx.AreEqual(90f, Scalar.Degrees(SingleConstants.HalfPi), 1e-4f);
		Assert.AreEqual(Math.PI, Scalar.Radians(180.0), 1e-12);
		Assert.AreEqual(180.0, Scalar.Degrees(Math.PI), 1e-12);
		Assert.AreEqual(37.5, Scalar.Degrees(Scalar.Radians(37.5)), 1e-12);
	}

	[TestMethod]
	public void ApproxEquals_UsesStrictTolerance()
	{
		Assert.IsTrue(Scalar.ApproxEquals(1f, 1f + 5e-7f));
		Assert.IsFalse(Scalar.ApproxEquals(1f, 1.00001f));
		Assert.IsTrue(Scalar.ApproxEquals(1f, 1.05f, 0.1f));
		Assert.IsFalse(Scalar.ApproxEquals(1f, 1.1f, 0.1f));
		Assert.IsTrue(Scalar.ApproxEquals(1.0, 1.0));
		Assert.IsFalse(Scalar.ApproxEquals(1.0, 1.0 + 1e-14));
		Assert.IsTrue(Scalar.ApproxEquals(1.0, 1.4, 0.5));
	}

	[TestMethod]
	public void Lerp_InterpolatesLinearly()
	{
		Assert.AreEqual(2f, Scalar.Lerp(2f, 6f, 0f));
		Assert.AreEqual(6f, Scalar.Lerp(2f, 6f, 1f));
		Assert.AreEqual(4f, Scalar.Lerp(2f, 6f, 0.5f));
		Assert.AreEqual(10f, Scalar.Lerp(2f, 6f, 2f));
		Assert.AreEqual(-1.0, Scalar.Lerp(-3.0, 1.0, 0.5));
	}

	[TestMethod]
	public void StepAndPulse_AreThresholdFunctions()
	{
		Assert.AreEqual(0f, Scalar.Step(1f, 0.5f));
		Assert.AreEqual(1f, Scalar.Step(1f, 1f));
		Assert.AreEqual(1.0, Scalar.Step(1.0, 2.0));
		Assert.AreEqual(0f, Scalar.Pulse(1f, 2f, 0.5f));
		Assert.AreEqual(1f, Scalar.Pulse(1f, 2f, 1.5f));
		Assert.AreEqual(0f, Scalar.Pulse(1f, 2f, 2f));
		Assert.AreEqual(1.0, Scalar.Pulse(1.0, 2.0, 1.0));
	}

	[TestMethod]
	public void BoxStep_IsClampedLinearRamp()
	{
		Assert.AreEqual(0f, Scalar.BoxStep(1f, 3f, 0f));
		Assert.AreEqual(0.5f, Scalar.BoxStep(1f, 3f, 2f));
		Assert.AreEqual(1f, Scalar.BoxStep(1f, 3f, 4f));
		Assert.AreEqual(0.25, Scalar.BoxStep(0.0, 4.0, 1.0));
		Assert.AreEqual(1.0, Scalar.BoxStep(0.0, 4.0, 4.0));
	}

	[TestMethod]
	public void SmoothStep_IsClampedCubicHermite()
	{
		Assert.AreEqual(0f, Scalar.SmoothStep(0f, 1f, -1f));
		Assert.AreEqual(1f, Scalar.SmoothStep(0f, 1f, 2f));
		Assert.AreEqual(0.5f, Scalar.SmoothStep(0f, 1f, 0.5f));
		AssertEx.AreEqual(0.15625f, Scalar.SmoothStep(0f, 2f, 0.5f)); // t = 0.25 -> 3t^2 - 2t^3
		Assert.AreEqual(0.5, Scalar.SmoothStep(-1.0, 1.0, 0.0), 1e-12);
		Assert.AreEqual(1.0, Scalar.SmoothStep(-1.0, 1.0, 1.0));
	}

	[TestMethod]
	public void SmootherStep_IsClampedQuinticHermite()
	{
		Assert.AreEqual(0f, Scalar.SmootherStep(0f, 1f, 0f));
		Assert.AreEqual(1f, Scalar.SmootherStep(0f, 1f, 1f));
		Assert.AreEqual(0.5f, Scalar.SmootherStep(0f, 1f, 0.5f));
		AssertEx.AreEqual(0.103515625f, Scalar.SmootherStep(0f, 1f, 0.25f)); // 6t^5 - 15t^4 + 10t^3
		Assert.AreEqual(0.103515625, Scalar.SmootherStep(0.0, 1.0, 0.25), 1e-12);
		Assert.AreEqual(0.0, Scalar.SmootherStep(0.0, 1.0, -3.0));
	}
}

[TestClass]
public sealed class ConstantsTests
{
	[TestMethod]
	public void SingleConstants_MatchSystemMath()
	{
		Assert.AreEqual(MathF.PI, SingleConstants.Pi);
		Assert.AreEqual(MathF.E, SingleConstants.E);
		AssertEx.AreEqual(MathF.PI*0.5f, SingleConstants.HalfPi);
		AssertEx.AreEqual(MathF.PI*2f, SingleConstants.TwoPi);
		AssertEx.AreEqual(MathF.PI*MathF.PI, SingleConstants.PiSquared);
		AssertEx.AreEqual(1f/MathF.PI, SingleConstants.InversePi);
		AssertEx.AreEqual(MathF.Sqrt(2f), SingleConstants.Sqrt2);
		AssertEx.AreEqual(MathF.Sqrt(3f), SingleConstants.Sqrt3);
		AssertEx.AreEqual(MathF.Log(2f), SingleConstants.Ln2);
		AssertEx.AreEqual(MathF.PI/180f, SingleConstants.DegreesToRadians);
		AssertEx.AreEqual(180f/MathF.PI, SingleConstants.RadiansToDegrees, 1e-4f);
		Assert.AreEqual(Single.PositiveInfinity, SingleConstants.Infinity);
		Assert.AreEqual(1e-6f, SingleConstants.Tolerance);
	}

	[TestMethod]
	public void DoubleConstants_MatchSystemMath()
	{
		// Local helper: compile-time constants as "actual" confuse the MSTest analyzers.
		static void Near(double expected, double actual, double delta = 0.0) => Assert.AreEqual(expected, actual, delta);

		Near(Math.PI, DoubleConstants.Pi, 1e-15);
		Near(Math.E, DoubleConstants.E, 1e-15);
		Near(Math.PI*0.5, DoubleConstants.HalfPi, 1e-15);
		Near(Math.PI*2.0, DoubleConstants.TwoPi, 1e-15);
		Near(Math.PI*Math.PI, DoubleConstants.PiSquared, 1e-14);
		Near(1.0/Math.PI, DoubleConstants.InversePi, 1e-15);
		Near(Math.Sqrt(2.0), DoubleConstants.Sqrt2, 1e-15);
		Near(Math.Sqrt(3.0), DoubleConstants.Sqrt3, 1e-15);
		Near(Math.Log(2.0), DoubleConstants.Ln2, 1e-15);
		Near(Math.PI/180.0, DoubleConstants.DegreesToRadians, 1e-15);
		Near(180.0/Math.PI, DoubleConstants.RadiansToDegrees, 1e-13);
		Near(Double.PositiveInfinity, DoubleConstants.Infinity);
		Near(1e-15, DoubleConstants.Tolerance);
	}

	[TestMethod]
	public void Axes_FlagsMatchAxisBitPositions()
	{
		Assert.AreEqual(1 << (int)Axis.X, (int)Axes.X);
		Assert.AreEqual(1 << (int)Axis.Y, (int)Axes.Y);
		Assert.AreEqual(1 << (int)Axis.Z, (int)Axes.Z);
		Assert.AreEqual(1 << (int)Axis.W, (int)Axes.W);
		Assert.AreEqual(Axes.X | Axes.Y, Axes.XY);
		Assert.AreEqual(Axes.X | Axes.Y | Axes.Z, Axes.XYZ);
		Assert.AreEqual(Axes.X | Axes.Y | Axes.Z | Axes.W, Axes.XYZW);
		Assert.AreEqual(Axes.Y | Axes.Z | Axes.W, Axes.YZW);
	}

	[TestMethod]
	public void EulerOrder_ValuesAreDistinctAndMarkedRotatingFrame()
	{
		var orders = new[] { EulerOrder.XYZ, EulerOrder.XZY, EulerOrder.YZX, EulerOrder.YXZ, EulerOrder.ZXY, EulerOrder.ZYX };
		Assert.AreEqual(orders.Length, orders.Distinct().Count());
		foreach (EulerOrder order in orders)
			Assert.AreEqual(1, (int)order & 1, order.ToString());
		Assert.AreEqual(0, (int)EulerOrder.Unspecified);
	}
}

[TestClass]
public sealed class RombergTests
{
	[TestMethod]
	public void Estimate_Single_IntegratesPolynomialExactly()
	{
		// Integral of x^2 over [0, 3] = 9
		AssertEx.AreEqual(9f, Romberg.Estimate(0f, 3f, x => x*x, 4), 1e-4f);
	}

	[TestMethod]
	public void Estimate_Double_IntegratesSine()
	{
		// Integral of sin(x) over [0, pi] = 2
		Assert.AreEqual(2.0, Romberg.Estimate(0.0, Math.PI, Math.Sin, 8), 1e-10);
	}

	[TestMethod]
	public void Estimate_Double_IntegratesExponential()
	{
		Assert.AreEqual(Math.E - 1.0, Romberg.Estimate(0.0, 1.0, Math.Exp, 6), 1e-10);
	}

	[TestMethod]
	public void Estimate_OrderOne_IsTrapezoidRule()
	{
		// 0.5*h*(f(a) + f(b)) with h = 2, f(0) = 0, f(2) = 4
		Assert.AreEqual(4.0, Romberg.Estimate(0.0, 2.0, x => x*x, 1), 1e-12);
	}

	[TestMethod]
	public void Estimate_ReversedBounds_NegatesResult()
	{
		Assert.AreEqual(-2.0, Romberg.Estimate(Math.PI, 0.0, Math.Sin, 8), 1e-10);
	}
}
