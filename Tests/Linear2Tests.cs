namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class Line2Tests
{
	// Horizontal line y = 1.
	private static readonly Line2 L = new Line2(new Vector2(0f, 1f), Vector2.UnitX);

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(new Vector2(0f, 1f), L.Origin);
		Assert.AreEqual(Vector2.UnitX, L.Direction);
		Line2 angled = new Line2(new Vector2(1f, 2f), SingleConstants.Pi/4f);
		AssertEx.AreEqual(Vector2.Normalize(new Vector2(1f, 1f)), angled.Direction);
		AssertEx.AreEqual(1f, angled.Slope);
		AssertEx.AreEqual(SingleConstants.Pi/4f, angled.InclinationAngle);
		AssertEx.AreEqual(-2f, new Line2(Vector2.Zero, new Vector2(1f, -2f)).Slope);
		AssertEx.AreEqual(SingleConstants.HalfPi, new Line2(Vector2.Zero, Vector2.UnitY).InclinationAngle);

		Line2 l = L;
		l.Origin = Vector2.Zero;
		l.Direction = Vector2.UnitY;
		Assert.AreEqual(new Line2(Vector2.Zero, Vector2.UnitY), l);
		Assert.AreEqual(new Line2(new Vector2(1f, 2f), new Vector2(3f, 4f)), Line2.FromRay(new Ray2(new Vector2(1f, 2f), new Vector2(3f, 4f))));
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Line2 same = new Line2(new Vector2(0f, 1f), Vector2.UnitX);
		Assert.IsTrue(L == same);
		Assert.IsFalse(L != same);
		Assert.IsTrue(L.Equals((object)same));
		Assert.IsFalse(L.Equals((object)new Ray2(new Vector2(0f, 1f), Vector2.UnitX)));
		Assert.AreEqual(L.GetHashCode(), same.GetHashCode());
		Line2 moved = Line2.Translate(L, new Vector2(0f, 0.01f));
		Assert.IsFalse(L.ApproxEquals(moved));
		Assert.IsTrue(L.ApproxEquals(moved, 0.1f));
	}

	[TestMethod]
	public void Translate_Transform_Normalize_Evaluate()
	{
		Line2 l = new Line2(new Vector2(1f, 1f), new Vector2(3f, 4f));
		Assert.AreEqual(new Line2(new Vector2(2f, 0f), new Vector2(3f, 4f)), Line2.Translate(l, new Vector2(1f, -1f)));
		Line2 t = l;
		t.Translate(new Vector2(1f, -1f));
		Assert.AreEqual(new Vector2(2f, 0f), t.Origin);

		Matrix2 r = Matrix2.Rotation(SingleConstants.HalfPi);
		Line2 rotated = Line2.Transform(l, r);
		AssertEx.AreEqual(new Vector2(-1f, 1f), rotated.Origin);
		AssertEx.AreEqual(new Vector2(-4f, 3f), rotated.Direction);
		t = l;
		t.Transform(r);
		Assert.AreEqual(rotated, t);

		AssertEx.AreEqual(new Vector2(0.6f, 0.8f), Line2.Normalize(l).Direction);
		t = l;
		t.Normalize();
		AssertEx.AreEqual(new Vector2(0.6f, 0.8f), t.Direction);
		Assert.AreEqual(l.Origin, t.Origin);

		Assert.AreEqual(new Vector2(7f, 9f), l.Evaluate(2f));
		Assert.AreEqual(new Vector2(-2f, -3f), l.Evaluate(-1f));
	}

	[TestMethod]
	public void ParallelAndCoincident()
	{
		Assert.IsTrue(L.IsParallel(new Line2(new Vector2(5f, 5f), new Vector2(-1f, 0f))));
		Assert.IsFalse(L.IsParallel(new Line2(Vector2.Zero, Vector2.Normalize(new Vector2(1f, 1f)))));
		Assert.IsTrue(L.IsCoincident(new Line2(new Vector2(7f, 1f), -Vector2.UnitX)));
		Assert.IsFalse(L.IsCoincident(new Line2(new Vector2(7f, 2f), Vector2.UnitX)));
	}

	[TestMethod]
	public void ClosestPoint_And_DistancesToPoint()
	{
		Assert.AreEqual(new Vector2(3f, 1f), L.GetClosestPoint(new Vector2(3f, 5f)));
		Assert.AreEqual(4f, L.GetDistanceTo(new Vector2(3f, 5f)));
		Assert.AreEqual(4f, L.GetSignedDistanceTo(new Vector2(3f, 5f)));
		Assert.AreEqual(-2f, L.GetSignedDistanceTo(new Vector2(-3f, -1f)));

		Line2 unnormalized = new Line2(new Vector2(0f, 1f), new Vector2(2f, 0f));
		AssertEx.AreEqual(new Vector2(3f, 1f), unnormalized.GetClosestPoint(new Vector2(3f, 5f), false));
		AssertEx.AreEqual(4f, unnormalized.GetDistanceTo(new Vector2(3f, 5f), false));
		AssertEx.AreEqual(new Vector2(3f, 1f), L.GetClosestPoint(new Vector2(3f, 5f), true));
		AssertEx.AreEqual(4f, L.GetDistanceTo(new Vector2(3f, 5f), true));
	}

	[TestMethod]
	public void DistanceToLine_IsNonZeroOnlyForParallelLines()
	{
		Line2 parallel = new Line2(new Vector2(5f, 4f), Vector2.UnitX);
		AssertEx.AreEqual(3f, L.GetDistanceTo(parallel));
		AssertEx.AreEqual(3f, L.GetSignedDistanceTo(parallel));
		AssertEx.AreEqual(-3f, L.GetSignedDistanceTo(new Line2(new Vector2(5f, -2f), Vector2.UnitX)));
		Assert.AreEqual(0f, L.GetDistanceTo(new Line2(Vector2.Zero, Vector2.UnitY)));
	}

	[TestMethod]
	public void IntersectLine()
	{
		Line2 vertical = new Line2(new Vector2(3f, -5f), Vector2.UnitY);
		Assert.IsTrue(L.Intersects(vertical));
		Assert.AreEqual(3f, L.FindIntersection(vertical));
		Line2 diagonal = new Line2(Vector2.Zero, new Vector2(2f, 2f));
		Assert.AreEqual(1f, L.FindIntersection(diagonal));
		Assert.AreEqual(0.5f, diagonal.FindIntersection(L));

		Line2 parallel = new Line2(new Vector2(0f, 2f), Vector2.UnitX);
		Assert.IsFalse(L.Intersects(parallel));
		Assert.IsNull(L.FindIntersection(parallel));

		Line2 coincident = new Line2(new Vector2(4f, 1f), -Vector2.UnitX);
		Assert.IsTrue(L.Intersects(coincident));
		Assert.AreEqual(0f, L.FindIntersection(coincident));
	}

	[TestMethod]
	public void IntersectRay()
	{
		Assert.AreEqual(2f, L.FindIntersection(new Ray2(new Vector2(2f, 5f), -Vector2.UnitY)));
		Assert.IsTrue(L.Intersects(new Ray2(new Vector2(2f, 5f), new Vector2(1f, -1f))));
		Assert.AreEqual(6f, L.FindIntersection(new Ray2(new Vector2(2f, 5f), new Vector2(1f, -1f))));
		Assert.IsNull(L.FindIntersection(new Ray2(new Vector2(2f, 5f), Vector2.UnitY)));
		Assert.IsFalse(L.Intersects(new Ray2(new Vector2(2f, 5f), Vector2.UnitX)));
		// Collinear ray: parameter of the ray origin on the line.
		Assert.AreEqual(4f, L.FindIntersection(new Ray2(new Vector2(4f, 1f), Vector2.UnitX)));
	}

	[TestMethod]
	public void IntersectSegment()
	{
		Assert.AreEqual(2f, L.FindIntersection(new LineSegment2(new Vector2(2f, 0f), new Vector2(2f, 3f))));
		Assert.IsTrue(L.Intersects(new LineSegment2(new Vector2(2f, 0f), new Vector2(2f, 3f))));
		Assert.IsNull(L.FindIntersection(new LineSegment2(new Vector2(2f, 2f), new Vector2(2f, 3f))));
		Assert.IsFalse(L.Intersects(new LineSegment2(new Vector2(2f, 2f), new Vector2(2f, 3f))));
		Assert.IsNull(L.FindIntersection(new LineSegment2(new Vector2(0f, 2f), new Vector2(5f, 2f))));
		// Endpoint touching.
		Assert.AreEqual(2f, L.FindIntersection(new LineSegment2(new Vector2(2f, 1f), new Vector2(2f, 3f))));
		// Collinear segment: parameter of the segment start on the line.
		Assert.AreEqual(-3f, L.FindIntersection(new LineSegment2(new Vector2(-3f, 1f), new Vector2(5f, 1f))));
	}

	[TestMethod]
	public void IntersectRectangle()
	{
		AxisAlignedRectangle rect = new AxisAlignedRectangle(new Vector2(2f, 0f), new Vector2(5f, 3f));
		AssertEx.AreEqual(new Interval(2f, 5f), L.FindIntersection(rect));
		Assert.IsTrue(L.Intersects(rect));
		Line2 diagonal = new Line2(new Vector2(0f, -2f), new Vector2(1f, 1f));
		AssertEx.AreEqual(new Interval(2f, 5f), diagonal.FindIntersection(rect));
		Line2 miss = new Line2(new Vector2(0f, 4f), Vector2.UnitX);
		Assert.IsNull(miss.FindIntersection(rect));
		Assert.IsFalse(miss.Intersects(rect));
		Line2 reversed = new Line2(new Vector2(10f, 1f), -Vector2.UnitX);
		AssertEx.AreEqual(new Interval(5f, 8f), reversed.FindIntersection(rect));
	}

	[TestMethod]
	public void IntersectCircle()
	{
		Circle2 circle = new Circle2(new Vector2(4f, 1f), 2f);
		AssertEx.AreEqual(new Interval(2f, 6f), L.FindIntersection(circle));
		Assert.IsTrue(L.Intersects(circle));
		Line2 tangent = new Line2(new Vector2(0f, 3f), Vector2.UnitX);
		AssertEx.AreEqual(new Interval(4f), tangent.FindIntersection(circle));
		Assert.IsTrue(tangent.Intersects(circle));
		Line2 miss = new Line2(new Vector2(0f, 3.5f), Vector2.UnitX);
		Assert.IsNull(miss.FindIntersection(circle));
		Assert.IsFalse(miss.Intersects(circle));
	}
}

[TestClass]
public sealed class Ray2Tests
{
	private static readonly Ray2 R = new Ray2(new Vector2(1f, 1f), Vector2.UnitX);

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(new Vector2(1f, 1f), R.Origin);
		Assert.AreEqual(Vector2.UnitX, R.Direction);
		Ray2 r = R;
		r.Origin = Vector2.Zero;
		r.Direction = new Vector2(1f, 2f);
		Assert.AreEqual(new Ray2(Vector2.Zero, new Vector2(1f, 2f)), r);
		AssertEx.AreEqual(2f, r.Slope);
		AssertEx.AreEqual(MathF.Atan2(2f, 1f), r.InclinationAngle);
		Assert.AreEqual(new Ray2(new Vector2(1f, 2f), new Vector2(3f, 4f)), Ray2.FromLine(new Line2(new Vector2(1f, 2f), new Vector2(3f, 4f))));
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Ray2 same = new Ray2(new Vector2(1f, 1f), Vector2.UnitX);
		Assert.IsTrue(R == same);
		Assert.IsFalse(R != same);
		Assert.IsTrue(R.Equals((object)same));
		Assert.IsFalse(R.Equals((object)new Line2(new Vector2(1f, 1f), Vector2.UnitX)));
		Assert.AreEqual(R.GetHashCode(), same.GetHashCode());
		Ray2 other = new Ray2(new Vector2(1f, 1f), new Vector2(1f, 0.01f));
		Assert.IsFalse(R.ApproxEquals(other));
		Assert.IsTrue(R.ApproxEquals(other, 0.1f));
	}

	[TestMethod]
	public void Translate_Transform_Normalize_Evaluate()
	{
		Ray2 r = new Ray2(new Vector2(1f, 0f), new Vector2(0f, 2f));
		Assert.AreEqual(new Ray2(new Vector2(2f, 2f), new Vector2(0f, 2f)), Ray2.Translate(r, new Vector2(1f, 2f)));
		Ray2 t = r;
		t.Translate(new Vector2(1f, 2f));
		Assert.AreEqual(new Vector2(2f, 2f), t.Origin);
		Ray2 rotated = Ray2.Transform(r, Matrix2.Rotation(SingleConstants.HalfPi));
		AssertEx.AreEqual(new Vector2(0f, 1f), rotated.Origin);
		AssertEx.AreEqual(new Vector2(-2f, 0f), rotated.Direction);
		t = r;
		t.Transform(Matrix2.Rotation(SingleConstants.HalfPi));
		Assert.AreEqual(rotated, t);
		Assert.AreEqual(Vector2.UnitY, Ray2.Normalize(r).Direction);
		t = r;
		t.Normalize();
		Assert.AreEqual(Vector2.UnitY, t.Direction);
		Assert.AreEqual(new Vector2(1f, 3f), r.Evaluate(1.5f));
	}

	[TestMethod]
	public void ClosestPoint_And_Distance_ClampToOrigin()
	{
		Assert.AreEqual(new Vector2(4f, 1f), R.GetClosestPoint(new Vector2(4f, 3f)));
		Assert.AreEqual(new Vector2(1f, 1f), R.GetClosestPoint(new Vector2(-4f, 3f)));
		Assert.AreEqual(2f, R.GetDistanceTo(new Vector2(4f, 3f)));
		Assert.AreEqual(5f, R.GetDistanceTo(new Vector2(-2f, 5f)));
		Ray2 unnormalized = new Ray2(new Vector2(1f, 1f), new Vector2(5f, 0f));
		AssertEx.AreEqual(new Vector2(4f, 1f), unnormalized.GetClosestPoint(new Vector2(4f, 3f), false));
		AssertEx.AreEqual(5f, unnormalized.GetDistanceTo(new Vector2(-2f, 5f), false));
		AssertEx.AreEqual(2f, R.GetDistanceTo(new Vector2(4f, 3f), true));
		AssertEx.AreEqual(new Vector2(1f, 1f), R.GetClosestPoint(new Vector2(-4f, 3f), true));
	}

	[TestMethod]
	public void IntersectLine_ReturnsRayParameter()
	{
		Line2 vertical = new Line2(new Vector2(4f, 0f), Vector2.UnitY);
		Assert.AreEqual(3f, R.FindIntersection(vertical));
		Assert.IsTrue(R.Intersects(vertical));
		Line2 behind = new Line2(new Vector2(-4f, 0f), Vector2.UnitY);
		Assert.IsNull(R.FindIntersection(behind));
		Assert.IsFalse(R.Intersects(behind));
		Assert.IsNull(R.FindIntersection(new Line2(Vector2.Zero, Vector2.UnitX)));
		Assert.AreEqual(1.5f, new Ray2(new Vector2(1f, 1f), new Vector2(2f, 0f)).FindIntersection(vertical));
	}

	[TestMethod]
	public void IntersectRectangle_ClampsToOrigin()
	{
		AxisAlignedRectangle rect = new AxisAlignedRectangle(new Vector2(3f, 0f), new Vector2(6f, 2f));
		AssertEx.AreEqual(new Interval(2f, 5f), R.FindIntersection(rect));
		Assert.IsTrue(R.Intersects(rect));
		Ray2 inside = new Ray2(new Vector2(4f, 1f), Vector2.UnitX);
		AssertEx.AreEqual(new Interval(0f, 2f), inside.FindIntersection(rect));
		Ray2 away = new Ray2(new Vector2(1f, 1f), -Vector2.UnitX);
		Assert.IsNull(away.FindIntersection(rect));
		Assert.IsFalse(away.Intersects(rect));
	}

	[TestMethod]
	public void IntersectCircle_ClampsToOrigin()
	{
		Circle2 circle = new Circle2(new Vector2(5f, 1f), 1f);
		AssertEx.AreEqual(new Interval(3f, 5f), R.FindIntersection(circle));
		Assert.IsTrue(R.Intersects(circle));
		Ray2 inside = new Ray2(new Vector2(5f, 1f), Vector2.UnitY);
		AssertEx.AreEqual(new Interval(0f, 1f), inside.FindIntersection(circle));
		Ray2 away = new Ray2(new Vector2(1f, 1f), -Vector2.UnitX);
		Assert.IsNull(away.FindIntersection(circle));
		Assert.IsFalse(away.Intersects(circle));
		Assert.IsFalse(new Ray2(new Vector2(1f, 3f), Vector2.UnitX).Intersects(circle));
	}
}

[TestClass]
public sealed class LineSegment2Tests
{
	private static readonly LineSegment2 S = new LineSegment2(new Vector2(1f, 1f), new Vector2(4f, 5f));

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(new Vector2(1f, 1f), S.Start);
		Assert.AreEqual(new Vector2(4f, 5f), S.End);
		Assert.AreEqual(S, new LineSegment2(new[] { new Vector2(1f, 1f), new Vector2(4f, 5f) }));
		Assert.AreEqual(5f, S.Length);
		AssertEx.AreEqual(new Vector2(0.6f, 0.8f), S.Direction);
		Assert.AreEqual(new Vector2(2.5f, 3f), S.Center);
		AssertEx.AreEqual(4f/3f, S.Slope);
		AssertEx.AreEqual(MathF.Atan2(4f, 3f), S.InclinationAngle);
		Assert.IsTrue(S.IsFinite);
		Assert.IsFalse(new LineSegment2(Vector2.Zero, Vector2.PositiveInfinity).IsFinite);
		CollectionAssert.AreEqual(new[] { S.Start, S.End }, S.GetEndpoints().ToArray());

		LineSegment2 s = S;
		s.Start = Vector2.Zero;
		s.End = Vector2.UnitX;
		Assert.AreEqual(new LineSegment2(Vector2.Zero, Vector2.UnitX), s);
	}

	[TestMethod]
	public void FactoryMethods()
	{
		Line2 line = new Line2(new Vector2(1f, 1f), new Vector2(2f, 0f));
		Assert.AreEqual(new LineSegment2(new Vector2(1f, 1f), new Vector2(3f, 1f)), LineSegment2.FromLine(line));
		Assert.AreEqual(new LineSegment2(new Vector2(-1f, 1f), new Vector2(5f, 1f)), LineSegment2.FromLine(line, new Interval(-1f, 2f)));
		Ray2 ray = new Ray2(new Vector2(1f, 1f), new Vector2(0f, 3f));
		Assert.AreEqual(new LineSegment2(new Vector2(1f, 1f), new Vector2(1f, 4f)), LineSegment2.FromRay(ray));
		Assert.AreEqual(new LineSegment2(new Vector2(1f, 4f), new Vector2(1f, 7f)), LineSegment2.FromRay(ray, new Interval(1f, 2f)));
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		LineSegment2 same = new LineSegment2(new Vector2(1f, 1f), new Vector2(4f, 5f));
		Assert.IsTrue(S == same);
		Assert.IsFalse(S != same);
		Assert.IsTrue(S.Equals((object)same));
		Assert.IsFalse(S.Equals((object)new LineSegment2(S.End, S.Start)));
		Assert.AreEqual(S.GetHashCode(), same.GetHashCode());
		LineSegment2 moved = LineSegment2.Translate(S, new Vector2(0.01f, 0f));
		Assert.IsFalse(S.ApproxEquals(moved));
		Assert.IsTrue(S.ApproxEquals(moved, 0.1f));
	}

	[TestMethod]
	public void Translate_Transform_Evaluate()
	{
		LineSegment2 moved = new LineSegment2(new Vector2(2f, 0f), new Vector2(5f, 4f));
		Assert.AreEqual(moved, LineSegment2.Translate(S, new Vector2(1f, -1f)));
		LineSegment2 s = S;
		s.Translate(new Vector2(1f, -1f));
		Assert.AreEqual(moved, s);
		Matrix2 m = Matrix2.Scaling(new Vector2(2f, -1f));
		Assert.AreEqual(new LineSegment2(new Vector2(2f, -1f), new Vector2(8f, -5f)), LineSegment2.Transform(S, m));
		s = S;
		s.Transform(m);
		Assert.AreEqual(new LineSegment2(new Vector2(2f, -1f), new Vector2(8f, -5f)), s);
		Assert.AreEqual(S.Start, S.Evaluate(0f));
		Assert.AreEqual(S.End, S.Evaluate(1f));
		Assert.AreEqual(S.Center, S.Evaluate(0.5f));
	}

	[TestMethod]
	public void ClosestPoint_And_Distance_ClampToEndpoints()
	{
		LineSegment2 s = new LineSegment2(Vector2.Zero, new Vector2(4f, 0f));
		Assert.AreEqual(new Vector2(2f, 0f), s.GetClosestPoint(new Vector2(2f, 3f)));
		Assert.AreEqual(Vector2.Zero, s.GetClosestPoint(new Vector2(-2f, 3f)));
		Assert.AreEqual(new Vector2(4f, 0f), s.GetClosestPoint(new Vector2(7f, -4f)));
		Assert.AreEqual(3f, s.GetDistanceTo(new Vector2(2f, 3f)));
		Assert.AreEqual(5f, s.GetDistanceTo(new Vector2(7f, -4f)));
	}

	[TestMethod]
	public void IntersectLine_ReturnsSegmentParameter()
	{
		Line2 line = new Line2(new Vector2(0f, 3f), Vector2.UnitX);
		Assert.AreEqual(0.5f, S.FindIntersection(line));
		Assert.IsTrue(S.Intersects(line));
		Line2 miss = new Line2(new Vector2(0f, 6f), Vector2.UnitX);
		Assert.IsNull(S.FindIntersection(miss));
		Assert.IsFalse(S.Intersects(miss));
	}

	[TestMethod]
	public void IntersectSegment()
	{
		LineSegment2 a = new LineSegment2(Vector2.Zero, new Vector2(4f, 4f));
		LineSegment2 b = new LineSegment2(new Vector2(0f, 4f), new Vector2(4f, 0f));
		Assert.AreEqual(0.5f, a.FindIntersection(b));
		Assert.IsTrue(a.Intersects(b));
		LineSegment2 c = new LineSegment2(new Vector2(0f, 4f), new Vector2(1f, 3f));
		Assert.IsNull(a.FindIntersection(c));
		Assert.IsFalse(a.Intersects(c));
		LineSegment2 parallel = new LineSegment2(new Vector2(1f, 0f), new Vector2(5f, 4f));
		Assert.IsNull(a.FindIntersection(parallel));

		// Touching at an endpoint.
		LineSegment2 touching = new LineSegment2(new Vector2(4f, 4f), new Vector2(6f, 0f));
		Assert.AreEqual(1f, a.FindIntersection(touching));

		// Collinear overlapping segments: parameter of the first overlapping point on this segment.
		LineSegment2 overlap = new LineSegment2(new Vector2(2f, 2f), new Vector2(6f, 6f));
		AssertEx.AreEqual(0.5f, a.FindIntersection(overlap).Value);
		LineSegment2 overlapReversed = new LineSegment2(new Vector2(6f, 6f), new Vector2(1f, 1f));
		AssertEx.AreEqual(0.25f, a.FindIntersection(overlapReversed).Value);
		LineSegment2 containing = new LineSegment2(new Vector2(-1f, -1f), new Vector2(6f, 6f));
		AssertEx.AreEqual(0f, a.FindIntersection(containing).Value);
		LineSegment2 disjoint = new LineSegment2(new Vector2(5f, 5f), new Vector2(6f, 6f));
		Assert.IsNull(a.FindIntersection(disjoint));
	}

	[TestMethod]
	public void IntersectRectangle_ClampsToSegment()
	{
		AxisAlignedRectangle rect = new AxisAlignedRectangle(new Vector2(1f, -1f), new Vector2(3f, 1f));
		LineSegment2 through = new LineSegment2(Vector2.Zero, new Vector2(4f, 0f));
		AssertEx.AreEqual(new Interval(0.25f, 0.75f), through.FindIntersection(rect));
		Assert.IsTrue(through.Intersects(rect));
		LineSegment2 startsInside = new LineSegment2(new Vector2(2f, 0f), new Vector2(6f, 0f));
		AssertEx.AreEqual(new Interval(0f, 0.25f), startsInside.FindIntersection(rect));
		LineSegment2 inside = new LineSegment2(new Vector2(1.5f, 0f), new Vector2(2.5f, 0f));
		AssertEx.AreEqual(new Interval(0f, 1f), inside.FindIntersection(rect));
		LineSegment2 shortOfIt = new LineSegment2(new Vector2(-2f, 0f), new Vector2(0.5f, 0f));
		Assert.IsNull(shortOfIt.FindIntersection(rect));
		Assert.IsFalse(shortOfIt.Intersects(rect));
	}

	[TestMethod]
	public void IntersectCircle_ClampsToSegment()
	{
		Circle2 circle = new Circle2(new Vector2(2f, 0f), 1f);
		LineSegment2 through = new LineSegment2(Vector2.Zero, new Vector2(4f, 0f));
		AssertEx.AreEqual(new Interval(0.25f, 0.75f), through.FindIntersection(circle));
		Assert.IsTrue(through.Intersects(circle));
		LineSegment2 inside = new LineSegment2(new Vector2(2f, 0f), new Vector2(2.5f, 0f));
		AssertEx.AreEqual(new Interval(0f, 1f), inside.FindIntersection(circle));
		LineSegment2 shortOfIt = new LineSegment2(Vector2.Zero, new Vector2(0.5f, 0f));
		Assert.IsNull(shortOfIt.FindIntersection(circle));
		Assert.IsFalse(shortOfIt.Intersects(circle));
	}
}
