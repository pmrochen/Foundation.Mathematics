using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class BezierCurve3Tests
{
	private static readonly BezierCurve3 C = new BezierCurve3(Vector3.Zero, new Vector3(1f, 2f, 0f), new Vector3(3f, 2f, 1f), new Vector3(4f, 0f, 2f));

	// Control points evenly spaced on a line give a uniformly parameterized straight curve.
	private static readonly BezierCurve3 Straight = new BezierCurve3(Vector3.Zero, new Vector3(1f, 2f, 2f), new Vector3(2f, 4f, 4f), new Vector3(3f, 6f, 6f));

	[TestMethod]
	public void Constructors_Properties_ToArray()
	{
		Assert.AreEqual(Vector3.Zero, C.ControlPoint0);
		Assert.AreEqual(new Vector3(1f, 2f, 0f), C.ControlPoint1);
		Assert.AreEqual(new Vector3(3f, 2f, 1f), C.ControlPoint2);
		Assert.AreEqual(new Vector3(4f, 0f, 2f), C.ControlPoint3);
		CollectionAssert.AreEqual(new[] { C.ControlPoint0, C.ControlPoint1, C.ControlPoint2, C.ControlPoint3 }, C.ToArray());
		Assert.AreEqual(C, new BezierCurve3(C.ToArray()));
		Assert.AreEqual(new BezierCurve3(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero), BezierCurve3.Zero);

		BezierCurve3 c = BezierCurve3.Zero;
		c.ControlPoint0 = Vector3.UnitX;
		c.ControlPoint1 = Vector3.UnitY;
		c.ControlPoint2 = Vector3.UnitZ;
		c.ControlPoint3 = new Vector3(1f);
		Assert.AreEqual(new BezierCurve3(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ, new Vector3(1f)), c);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		BezierCurve3 same = new BezierCurve3(C.ToArray());
		Assert.IsTrue(C == same);
		Assert.IsFalse(C != same);
		Assert.IsTrue(C.Equals((object)same));
		Assert.AreEqual(C.GetHashCode(), same.GetHashCode());
		BezierCurve3 moved = BezierCurve3.Translate(C, new Vector3(0.01f, 0f, 0f));
		Assert.IsTrue(C != moved);
		Assert.IsFalse(C.ApproxEquals(moved, 1e-3f));
		Assert.IsTrue(C.ApproxEquals(moved, 0.1f));
	}

	[TestMethod]
	public void Evaluate_InterpolatesEndpoints()
	{
		Assert.AreEqual(C.ControlPoint0, C.Evaluate(0f));
		Assert.AreEqual(C.ControlPoint3, C.Evaluate(1f));
		AssertEx.AreEqual((C.ControlPoint0 + 3f*C.ControlPoint1 + 3f*C.ControlPoint2 + C.ControlPoint3)/8f, C.Evaluate(0.5f));
		foreach (float t in new[] { 0f, 0.2f, 0.5f, 0.9f })
		{
			Vector3 p = C.Evaluate(t);
			AssertEx.AreEqual(p.X, C.EvaluateX(t));
			AssertEx.AreEqual(p.Y, C.EvaluateY(t));
			AssertEx.AreEqual(p.Z, C.EvaluateZ(t));
		}
		AssertEx.AreEqual(new Vector3(1.5f, 3f, 3f), Straight.Evaluate(0.5f));
	}

	[TestMethod]
	public void Derivative_Tangent_Speed()
	{
		AssertEx.AreEqual(3f*(C.ControlPoint1 - C.ControlPoint0), C.CalculateDerivative(0f));
		AssertEx.AreEqual(3f*(C.ControlPoint3 - C.ControlPoint2), C.CalculateDerivative(1f));
		const float h = 1e-3f;
		foreach (float t in new[] { 0.25f, 0.5f, 0.75f })
		{
			Vector3 numeric = (C.Evaluate(t + h) - C.Evaluate(t - h))/(2f*h);
			AssertEx.AreEqual(numeric, C.CalculateDerivative(t), 1e-2f);
			AssertEx.AreEqual(Vector3.Normalize(C.CalculateDerivative(t)), C.CalculateTangent(t));
			AssertEx.AreEqual(C.CalculateDerivative(t).Magnitude, C.CalculateSpeed(t));
		}
		AssertEx.AreEqual(9f, Straight.CalculateSpeed(0.3f), 1e-4f);
	}

	[TestMethod]
	public void Length_And_Time()
	{
		AssertEx.AreEqual(9f, Straight.CalculateLength(), 1e-4f);
		AssertEx.AreEqual(9f, Straight.CalculateLength(1f), 1e-4f);
		AssertEx.AreEqual(9f, Straight.CalculateLength(2f), 1e-4f);
		AssertEx.AreEqual(3f, Straight.CalculateLength(1f/3f), 1e-4f);
		Assert.AreEqual(0f, Straight.CalculateLength(0f));
		Assert.AreEqual(0f, Straight.CalculateLength(-1f));
		AssertEx.AreEqual(0.5f, Straight.CalculateTime(4.5f, 10).Value, 1e-4f);
		Assert.AreEqual(0f, Straight.CalculateTime(0f, 10));
		Assert.AreEqual(1f, Straight.CalculateTime(10f, 10));

		// Curved arc: length lies between the chord and the control polygon.
		float length = C.CalculateLength();
		float chord = Vector3.Distance(C.ControlPoint0, C.ControlPoint3);
		float polygon = Vector3.Distance(C.ControlPoint0, C.ControlPoint1) + Vector3.Distance(C.ControlPoint1, C.ControlPoint2) +
			Vector3.Distance(C.ControlPoint2, C.ControlPoint3);
		Assert.IsTrue(length > chord && length < polygon, $"{chord} < {length} < {polygon}");
		float? time = C.CalculateTime(0.4f*length, 20);
		Assert.IsTrue(time.HasValue);
		AssertEx.AreEqual(0.4f*length, C.CalculateLength(time.Value), 1e-4f);
	}

	[TestMethod]
	public void Translate_And_Transform()
	{
		Vector3 offset = new Vector3(1f, -1f, 2f);
		BezierCurve3 moved = BezierCurve3.Translate(C, offset);
		AssertEx.AreEqual(C.Evaluate(0.3f) + offset, moved.Evaluate(0.3f));
		BezierCurve3 c = C;
		c.Translate(offset);
		Assert.AreEqual(moved, c);

		Matrix3 m = Samples.GeneralMatrix3;
		BezierCurve3 transformed = BezierCurve3.Transform(C, m);
		AssertEx.AreEqual(C.Evaluate(0.3f)*m, transformed.Evaluate(0.3f), 1e-5f);
		c = C;
		c.Transform(m);
		Assert.AreEqual(transformed, c);

		AffineTransform at = new AffineTransform(m, offset);
		BezierCurve3 affine = BezierCurve3.Transform(C, at);
		AssertEx.AreEqual(Vector3.Transform(C.Evaluate(0.7f), at), affine.Evaluate(0.7f), 1e-5f);
		c = C;
		c.Transform(at);
		Assert.AreEqual(affine, c);
	}

	[TestMethod]
	public void ToString_And_Parse()
	{
		string text = C.ToString(CultureInfo.InvariantCulture);
		Assert.AreEqual("0 0 0 1 2 0 3 2 1 4 0 2", text);
		Assert.AreEqual(C, BezierCurve3.Parse(text, CultureInfo.InvariantCulture));
		Assert.ThrowsExactly<FormatException>(() => BezierCurve3.Parse("1 2 3", CultureInfo.InvariantCulture));
		Assert.ThrowsExactly<ArgumentNullException>(() => BezierCurve3.Parse(null));
	}
}

[TestClass]
public sealed class BezierCurve2Tests
{
	private static readonly BezierCurve2 C = new BezierCurve2(Vector2.Zero, new Vector2(1f, 2f), new Vector2(3f, 2f), new Vector2(4f, 0f));
	private static readonly BezierCurve2 Straight = new BezierCurve2(Vector2.Zero, new Vector2(1f, 1f), new Vector2(2f, 2f), new Vector2(3f, 3f));

	[TestMethod]
	public void Constructors_Properties_ToArray()
	{
		Assert.AreEqual(Vector2.Zero, C.ControlPoint0);
		Assert.AreEqual(new Vector2(1f, 2f), C.ControlPoint1);
		Assert.AreEqual(new Vector2(3f, 2f), C.ControlPoint2);
		Assert.AreEqual(new Vector2(4f, 0f), C.ControlPoint3);
		Assert.AreEqual(C, new BezierCurve2(C.ToArray()));
		Assert.AreEqual(new BezierCurve2(Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero), BezierCurve2.Zero);
		BezierCurve2 c = BezierCurve2.Zero;
		c.ControlPoint0 = Vector2.UnitX;
		c.ControlPoint1 = Vector2.UnitY;
		c.ControlPoint2 = new Vector2(2f);
		c.ControlPoint3 = new Vector2(3f);
		Assert.AreEqual(new BezierCurve2(Vector2.UnitX, Vector2.UnitY, new Vector2(2f), new Vector2(3f)), c);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		BezierCurve2 same = new BezierCurve2(C.ToArray());
		Assert.IsTrue(C == same);
		Assert.IsFalse(C != same);
		Assert.IsTrue(C.Equals((object)same));
		Assert.AreEqual(C.GetHashCode(), same.GetHashCode());
		BezierCurve2 moved = BezierCurve2.Translate(C, new Vector2(0.01f, 0f));
		Assert.IsFalse(C.ApproxEquals(moved, 1e-3f));
		Assert.IsTrue(C.ApproxEquals(moved, 0.1f));
	}

	[TestMethod]
	public void Evaluate_Derivative_Length()
	{
		Assert.AreEqual(C.ControlPoint0, C.Evaluate(0f));
		Assert.AreEqual(C.ControlPoint3, C.Evaluate(1f));
		AssertEx.AreEqual(new Vector2(2f, 1.5f), C.Evaluate(0.5f));
		AssertEx.AreEqual(2f, C.EvaluateX(0.5f));
		AssertEx.AreEqual(1.5f, C.EvaluateY(0.5f));
		AssertEx.AreEqual(new Vector2(3f, 6f), C.CalculateDerivative(0f));
		AssertEx.AreEqual(new Vector2(3f, -6f), C.CalculateDerivative(1f));
		AssertEx.AreEqual(Vector2.UnitX, C.CalculateTangent(0.5f));
		AssertEx.AreEqual(C.CalculateDerivative(0.3f).Magnitude, C.CalculateSpeed(0.3f));
		AssertEx.AreEqual(3f*MathF.Sqrt(2f), Straight.CalculateLength(), 1e-4f);
		AssertEx.AreEqual(MathF.Sqrt(2f), Straight.CalculateLength(1f/3f), 1e-4f);
		Assert.AreEqual(0f, Straight.CalculateLength(0f));
		AssertEx.AreEqual(0.25f, Straight.CalculateTime(0.75f*MathF.Sqrt(2f), 10).Value, 1e-4f);
		Assert.AreEqual(0f, C.CalculateTime(-1f, 10));
		Assert.AreEqual(1f, C.CalculateTime(100f, 10));
		float length = C.CalculateLength();
		AssertEx.AreEqual(0.6f*length, C.CalculateLength(C.CalculateTime(0.6f*length, 20).Value), 1e-4f);
	}

	[TestMethod]
	public void Translate_And_Transform()
	{
		BezierCurve2 moved = BezierCurve2.Translate(C, new Vector2(1f, -1f));
		AssertEx.AreEqual(C.Evaluate(0.3f) + new Vector2(1f, -1f), moved.Evaluate(0.3f));
		BezierCurve2 c = C;
		c.Translate(new Vector2(1f, -1f));
		Assert.AreEqual(moved, c);
		Matrix2 m = new Matrix2(2f, 1f, -1f, 0.5f);
		BezierCurve2 transformed = BezierCurve2.Transform(C, m);
		AssertEx.AreEqual(C.Evaluate(0.3f)*m, transformed.Evaluate(0.3f));
		c = C;
		c.Transform(m);
		Assert.AreEqual(transformed, c);
	}

	[TestMethod]
	public void FromPackedBezierCurve2_AddsFixedEndpoints()
	{
		PackedBezierCurve2 packed = new PackedBezierCurve2(new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f));
		BezierCurve2 full = BezierCurve2.FromPackedBezierCurve2(packed);
		Assert.AreEqual(new BezierCurve2(Vector2.Zero, packed.ControlPoint1, packed.ControlPoint2, new Vector2(1f)), full);
		BezierCurve2 implicitFull = packed;
		Assert.AreEqual(full, implicitFull);
	}

	[TestMethod]
	public void ToString_And_Parse()
	{
		string text = C.ToString(CultureInfo.InvariantCulture);
		Assert.AreEqual("0 0 1 2 3 2 4 0", text);
		Assert.AreEqual(C, BezierCurve2.Parse(text, CultureInfo.InvariantCulture));
		Assert.ThrowsExactly<FormatException>(() => BezierCurve2.Parse("1 2 3", CultureInfo.InvariantCulture));
	}
}

[TestClass]
public sealed class PackedBezierCurve2Tests
{
	// CSS "ease" timing function.
	private static readonly PackedBezierCurve2 Ease = new PackedBezierCurve2(new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f));

	[TestMethod]
	public void StaticFields_And_Properties()
	{
		Assert.AreEqual(Vector2.Zero, PackedBezierCurve2.Zero.ControlPoint1);
		Assert.AreEqual(Vector2.Zero, PackedBezierCurve2.Zero.ControlPoint2);
		Assert.AreEqual(Vector2.Zero, PackedBezierCurve2.Linear.ControlPoint1);
		Assert.AreEqual(new Vector2(1f), PackedBezierCurve2.Linear.ControlPoint2);
		Assert.AreEqual(Vector2.Zero, Ease.ControlPoint0);
		Assert.AreEqual(new Vector2(1f), Ease.ControlPoint3);
		Assert.AreEqual(new Vector2(0.25f, 0.1f), Ease.ControlPoint1);
		Assert.AreEqual(new Vector2(0.25f, 1f), Ease.ControlPoint2);
		CollectionAssert.AreEqual(new[] { Ease.ControlPoint1, Ease.ControlPoint2 }, Ease.ToArray());
		Assert.AreEqual(Ease, new PackedBezierCurve2(Ease.ToArray()));

		PackedBezierCurve2 c = Ease;
		c.ControlPoint1 = Vector2.UnitX;
		c.ControlPoint2 = Vector2.UnitY;
		Assert.AreEqual(new PackedBezierCurve2(Vector2.UnitX, Vector2.UnitY), c);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		PackedBezierCurve2 same = new PackedBezierCurve2(new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f));
		Assert.IsTrue(Ease == same);
		Assert.IsFalse(Ease != same);
		Assert.IsTrue(Ease.Equals((object)same));
		Assert.AreEqual(Ease.GetHashCode(), same.GetHashCode());
		PackedBezierCurve2 other = new PackedBezierCurve2(new Vector2(0.26f, 0.1f), new Vector2(0.25f, 1f));
		Assert.IsTrue(Ease != other);
		Assert.IsFalse(Ease.ApproxEquals(other));
		Assert.IsTrue(Ease.ApproxEquals(other, 0.1f));
	}

	[TestMethod]
	public void Evaluation_MatchesFullBezierCurve()
	{
		BezierCurve2 full = BezierCurve2.FromPackedBezierCurve2(Ease);
		foreach (float t in new[] { 0f, 0.1f, 0.5f, 0.8f, 1f })
		{
			AssertEx.AreEqual(full.Evaluate(t), Ease.Evaluate(t));
			AssertEx.AreEqual(full.EvaluateX(t), Ease.EvaluateX(t));
			AssertEx.AreEqual(full.EvaluateY(t), Ease.EvaluateY(t));
			AssertEx.AreEqual(full.CalculateDerivative(t), Ease.CalculateDerivative(t), 1e-5f);
			AssertEx.AreEqual(full.CalculateTangent(t), Ease.CalculateTangent(t), 1e-5f);
			AssertEx.AreEqual(full.CalculateSpeed(t), Ease.CalculateSpeed(t), 1e-5f);
			AssertEx.AreEqual(full.CalculateLength(t), Ease.CalculateLength(t), 1e-5f);
		}
		Assert.AreEqual(Vector2.Zero, Ease.Evaluate(0f));
		Assert.AreEqual(new Vector2(1f), Ease.Evaluate(1f));
		AssertEx.AreEqual(MathF.Sqrt(2f), PackedBezierCurve2.Linear.CalculateLength(), 1e-4f);
		AssertEx.AreEqual(0.5f, PackedBezierCurve2.Linear.CalculateTime(MathF.Sqrt(0.5f), 10).Value, 1e-4f);
		Assert.AreEqual(0f, Ease.CalculateTime(0f, 10));
		Assert.AreEqual(1f, Ease.CalculateTime(10f, 10));
	}

	[TestMethod]
	public void FindYAtX_InvertsTheXCoordinate()
	{
		foreach (float x in new[] { 0f, 0.1f, 0.25f, 0.5f, 0.75f, 0.99f, 1f })
			AssertEx.AreEqual(x, PackedBezierCurve2.Linear.FindYAtX(x), 1e-4f);
		foreach (float t in new[] { 0.05f, 0.2f, 0.5f, 0.7f, 0.95f })
			AssertEx.AreEqual(Ease.EvaluateY(t), Ease.FindYAtX(Ease.EvaluateX(t)), 1e-4f, $"t = {t}");
		// Known value for CSS ease at x = 0.5.
		AssertEx.AreEqual(0.8024f, Ease.FindYAtX(0.5f), 1e-3f);
	}

	[TestMethod]
	public void FromBezierCurve_RequiresUnitEndpoints()
	{
		BezierCurve2 valid = new BezierCurve2(Vector2.Zero, new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f), new Vector2(1f));
		Assert.AreEqual(Ease, PackedBezierCurve2.FromBezierCurve(valid));
		Assert.AreEqual(Ease, (PackedBezierCurve2)valid);
		BezierCurve2 badStart = new BezierCurve2(new Vector2(0.1f, 0f), new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f), new Vector2(1f));
		BezierCurve2 badEnd = new BezierCurve2(Vector2.Zero, new Vector2(0.25f, 0.1f), new Vector2(0.25f, 1f), new Vector2(2f, 1f));
		Assert.ThrowsExactly<ArgumentException>(() => PackedBezierCurve2.FromBezierCurve(badStart));
		Assert.ThrowsExactly<ArgumentException>(() => PackedBezierCurve2.FromBezierCurve(badEnd));
	}

	[TestMethod]
	public void ToString_And_Parse()
	{
		string text = Ease.ToString(CultureInfo.InvariantCulture);
		Assert.AreEqual("0.25 0.1 0.25 1", text);
		Assert.AreEqual(Ease, PackedBezierCurve2.Parse(text, CultureInfo.InvariantCulture));
		Assert.AreEqual("0.25 0.10 0.25 1.00", Ease.ToString("F2", CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual(text, Ease.ToString());
			Assert.AreEqual(Ease, PackedBezierCurve2.Parse(text));
		}
		Assert.ThrowsExactly<FormatException>(() => PackedBezierCurve2.Parse("1 2 3", CultureInfo.InvariantCulture));
		Assert.ThrowsExactly<ArgumentNullException>(() => PackedBezierCurve2.Parse(null));
	}
}
