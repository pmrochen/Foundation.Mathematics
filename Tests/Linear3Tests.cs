namespace Foundation.Mathematics.Tests;

/// <summary>
/// Shared shapes for Line3/Ray3/LineSegment3 tests. All are centered on the X axis around x = 5.
/// </summary>
internal static class LinearTargets
{
	public static readonly Plane Plane = new Plane(Vector3.UnitX, new Vector3(5f, 0f, 0f));
	public static readonly Triangle3 Triangle = new Triangle3(new Vector3(5f, -1f, -1f), new Vector3(5f, 2f, -1f), new Vector3(5f, -1f, 2f));
	public static readonly AxisAlignedBox Box = new AxisAlignedBox(new Vector3(4f, -1f, -1f), new Vector3(6f, 1f, 1f));
	public static readonly OrientedBox OrientedBox = new OrientedBox(new Vector3(5f, 0f, 0f), Matrix3.Rotation(Axis.Z, SingleConstants.Pi/4f), new Vector3(1f, 1f, 1f));
	public static readonly Sphere Sphere = new Sphere(new Vector3(5f, 0f, 0f), 1f);
	public static readonly Ellipsoid Ellipsoid = new Ellipsoid(new Vector3(5f, 0f, 0f), Matrix3.Identity, new Vector3(2f, 1f, 1f));
}

[TestClass]
public sealed class Line3Tests
{
	private static readonly Line3 XAxis = new Line3(Vector3.Zero, Vector3.UnitX);

	[TestMethod]
	public void Constructors_Properties_Equality()
	{
		Line3 l = new Line3(new Vector3(1f, 2f, 3f), new Vector3(0f, 0f, 2f));
		Assert.AreEqual(new Vector3(1f, 2f, 3f), l.Origin);
		Assert.AreEqual(new Vector3(0f, 0f, 2f), l.Direction);
		Assert.AreEqual(l, Line3.FromRay(new Ray3(new Vector3(1f, 2f, 3f), new Vector3(0f, 0f, 2f))));
		Line3 same = new Line3(new Vector3(1f, 2f, 3f), new Vector3(0f, 0f, 2f));
		Assert.IsTrue(l == same);
		Assert.IsFalse(l != same);
		Assert.IsTrue(l.Equals((object)same));
		Assert.IsFalse(l.Equals((object)new Ray3(l.Origin, l.Direction)));
		Assert.AreEqual(l.GetHashCode(), same.GetHashCode());
		Line3 moved = Line3.Translate(l, new Vector3(0.01f, 0f, 0f));
		Assert.IsFalse(l.ApproxEquals(moved));
		Assert.IsTrue(l.ApproxEquals(moved, 0.1f));
		l.Origin = Vector3.Zero;
		l.Direction = Vector3.UnitX;
		Assert.AreEqual(XAxis, l);
	}

	[TestMethod]
	public void Translate_Transform_Normalize_Evaluate()
	{
		Line3 l = new Line3(new Vector3(1f, 0f, 0f), new Vector3(0f, 3f, 0f));
		Line3 t = l;
		t.Translate(new Vector3(1f, 1f, 1f));
		Assert.AreEqual(new Line3(new Vector3(2f, 1f, 1f), new Vector3(0f, 3f, 0f)), t);

		Matrix3 r = Matrix3.Rotation(Axis.Z, SingleConstants.HalfPi);
		Line3 rotated = Line3.Transform(l, r);
		AssertEx.AreEqual(new Vector3(0f, 1f, 0f), rotated.Origin);
		AssertEx.AreEqual(new Vector3(-3f, 0f, 0f), rotated.Direction);
		t = l;
		t.Transform(r);
		Assert.AreEqual(rotated, t);

		AffineTransform at = new AffineTransform(r, new Vector3(0f, 0f, 5f));
		Line3 moved = Line3.Transform(l, at);
		AssertEx.AreEqual(new Vector3(0f, 1f, 5f), moved.Origin);
		AssertEx.AreEqual(new Vector3(-3f, 0f, 0f), moved.Direction); // directions are not translated
		t = l;
		t.Transform(at);
		Assert.AreEqual(moved, t);

		Assert.AreEqual(Vector3.UnitY, Line3.Normalize(l).Direction);
		t = l;
		t.Normalize();
		Assert.AreEqual(Vector3.UnitY, t.Direction);
		Assert.AreEqual(new Vector3(1f, 6f, 0f), l.Evaluate(2f));
		Assert.AreEqual(new Vector3(1f, -3f, 0f), l.Evaluate(-1f));
	}

	[TestMethod]
	public void ClosestPoint_And_Distance()
	{
		Assert.AreEqual(new Vector3(3f, 0f, 0f), XAxis.GetClosestPoint(new Vector3(3f, 4f, 0f)));
		Assert.AreEqual(new Vector3(-3f, 0f, 0f), XAxis.GetClosestPoint(new Vector3(-3f, 0f, 4f)));
		Assert.AreEqual(5f, XAxis.GetDistanceTo(new Vector3(-3f, 3f, 4f)));
		Line3 unnormalized = new Line3(Vector3.Zero, new Vector3(4f, 0f, 0f));
		AssertEx.AreEqual(new Vector3(3f, 0f, 0f), unnormalized.GetClosestPoint(new Vector3(3f, 4f, 0f), false));
		AssertEx.AreEqual(5f, unnormalized.GetDistanceTo(new Vector3(-3f, 3f, 4f), false));
		AssertEx.AreEqual(5f, XAxis.GetDistanceTo(new Vector3(-3f, 3f, 4f), true));
		AssertEx.AreEqual(new Vector3(3f, 0f, 0f), XAxis.GetClosestPoint(new Vector3(3f, 4f, 0f), true));
	}

	[TestMethod]
	public void IntersectPlane()
	{
		Assert.IsTrue(XAxis.Intersects(LinearTargets.Plane));
		Assert.AreEqual(5f, XAxis.FindIntersection(LinearTargets.Plane));
		Assert.AreEqual(-1f, new Line3(new Vector3(10f, 0f, 0f), new Vector3(5f, 1f, 0f)).FindIntersection(LinearTargets.Plane));
		Line3 parallel = new Line3(Vector3.Zero, Vector3.UnitY);
		Assert.IsFalse(parallel.Intersects(LinearTargets.Plane));
		Assert.IsNull(parallel.FindIntersection(LinearTargets.Plane));
	}

	[TestMethod]
	public void IntersectTriangle()
	{
		Assert.AreEqual(5f, XAxis.FindIntersection(LinearTargets.Triangle));
		Assert.IsTrue(XAxis.Intersects(LinearTargets.Triangle));
		Line3 reversed = new Line3(new Vector3(10f, 0f, 0f), -Vector3.UnitX);
		Assert.AreEqual(5f, reversed.FindIntersection(LinearTargets.Triangle));
		Line3 behind = new Line3(new Vector3(10f, 0f, 0f), Vector3.UnitX);
		Assert.AreEqual(-5f, behind.FindIntersection(LinearTargets.Triangle));
		Line3 miss = new Line3(new Vector3(0f, 1.5f, 1.5f), Vector3.UnitX);
		Assert.IsNull(miss.FindIntersection(LinearTargets.Triangle));
		Assert.IsFalse(miss.Intersects(LinearTargets.Triangle));
		Line3 parallel = new Line3(new Vector3(0f, 0f, 0f), Vector3.UnitY);
		Assert.IsNull(parallel.FindIntersection(LinearTargets.Triangle));
	}

	[TestMethod]
	public void IntersectBoxes()
	{
		AssertEx.AreEqual(new Interval(4f, 6f), XAxis.FindIntersection(LinearTargets.Box));
		Assert.IsTrue(XAxis.Intersects(LinearTargets.Box));
		float d = MathF.Sqrt(2f);
		AssertEx.AreEqual(new Interval(5f - d, 5f + d), XAxis.FindIntersection(LinearTargets.OrientedBox));
		Assert.IsTrue(XAxis.Intersects(LinearTargets.OrientedBox));
		Line3 miss = new Line3(new Vector3(0f, 1.5f, 0f), Vector3.UnitX);
		Assert.IsNull(miss.FindIntersection(LinearTargets.Box));
		Assert.IsFalse(miss.Intersects(LinearTargets.Box));
		Assert.IsFalse(miss.Intersects(LinearTargets.OrientedBox));
		// The rotated box reaches y = sqrt(2) at x = 5, beyond the axis-aligned box.
		Line3 between = new Line3(new Vector3(0f, 1.2f, 0f), Vector3.UnitX);
		Assert.IsFalse(between.Intersects(LinearTargets.Box));
		Assert.IsTrue(between.Intersects(LinearTargets.OrientedBox));
		AssertEx.AreEqual(new Interval(5f - (d - 1.2f), 5f + (d - 1.2f)), between.FindIntersection(LinearTargets.OrientedBox), 1e-5f); // |x - 5| + |y| <= sqrt(2)
	}

	[TestMethod]
	public void IntersectSphere()
	{
		AssertEx.AreEqual(new Interval(4f, 6f), XAxis.FindIntersection(LinearTargets.Sphere));
		Assert.IsTrue(XAxis.Intersects(LinearTargets.Sphere));
		Line3 tangent = new Line3(new Vector3(0f, 1f, 0f), Vector3.UnitX);
		AssertEx.AreEqual(new Interval(5f), tangent.FindIntersection(LinearTargets.Sphere));
		Line3 miss = new Line3(new Vector3(0f, 1.01f, 0f), Vector3.UnitX);
		Assert.IsNull(miss.FindIntersection(LinearTargets.Sphere));
		Assert.IsFalse(miss.Intersects(LinearTargets.Sphere));
	}

	[TestMethod]
	public void IntersectEllipsoid()
	{
		AssertEx.AreEqual(new Interval(3f, 7f), XAxis.FindIntersection(LinearTargets.Ellipsoid));
		Assert.IsTrue(XAxis.Intersects(LinearTargets.Ellipsoid));
		Line3 vertical = new Line3(new Vector3(5f, -10f, 0f), Vector3.UnitY);
		AssertEx.AreEqual(new Interval(9f, 11f), vertical.FindIntersection(LinearTargets.Ellipsoid), 1e-4f);
		Line3 miss = new Line3(new Vector3(0f, 1.1f, 0f), Vector3.UnitX);
		Assert.IsNull(miss.FindIntersection(LinearTargets.Ellipsoid));
		Assert.IsFalse(miss.Intersects(LinearTargets.Ellipsoid));
	}
}

[TestClass]
public sealed class Ray3Tests
{
	private static readonly Ray3 R = new Ray3(Vector3.Zero, Vector3.UnitX);
	private static readonly Ray3 Away = new Ray3(new Vector3(10f, 0f, 0f), Vector3.UnitX);
	private static readonly Ray3 Inside = new Ray3(new Vector3(5f, 0f, 0f), Vector3.UnitX);

	[TestMethod]
	public void Constructors_Properties_Equality()
	{
		Ray3 r = new Ray3(new Vector3(1f, 2f, 3f), new Vector3(0f, 0f, 2f));
		Assert.AreEqual(new Vector3(1f, 2f, 3f), r.Origin);
		Assert.AreEqual(new Vector3(0f, 0f, 2f), r.Direction);
		Assert.AreEqual(r, Ray3.FromLine(new Line3(new Vector3(1f, 2f, 3f), new Vector3(0f, 0f, 2f))));
		Ray3 same = r;
		Assert.IsTrue(r == same);
		Assert.IsFalse(r != same);
		Assert.IsTrue(r.Equals((object)same));
		Assert.IsFalse(r.Equals((object)Line3.FromRay(r)));
		Assert.AreEqual(r.GetHashCode(), same.GetHashCode());
		Ray3 moved = Ray3.Translate(r, new Vector3(0.01f, 0f, 0f));
		Assert.IsFalse(r.ApproxEquals(moved));
		Assert.IsTrue(r.ApproxEquals(moved, 0.1f));
		r.Origin = Vector3.Zero;
		r.Direction = Vector3.UnitX;
		Assert.AreEqual(R, r);
	}

	[TestMethod]
	public void Translate_Transform_Normalize_Evaluate()
	{
		Ray3 r = new Ray3(new Vector3(1f, 0f, 0f), new Vector3(0f, 3f, 0f));
		Ray3 t = r;
		t.Translate(new Vector3(1f, 1f, 1f));
		Assert.AreEqual(new Ray3(new Vector3(2f, 1f, 1f), new Vector3(0f, 3f, 0f)), t);
		Matrix3 m = Matrix3.Rotation(Axis.Z, SingleConstants.HalfPi);
		Ray3 rotated = Ray3.Transform(r, m);
		AssertEx.AreEqual(new Vector3(0f, 1f, 0f), rotated.Origin);
		AssertEx.AreEqual(new Vector3(-3f, 0f, 0f), rotated.Direction);
		t = r;
		t.Transform(m);
		Assert.AreEqual(rotated, t);
		AffineTransform at = new AffineTransform(m, new Vector3(0f, 0f, 5f));
		Ray3 moved = Ray3.Transform(r, at);
		AssertEx.AreEqual(new Vector3(0f, 1f, 5f), moved.Origin);
		AssertEx.AreEqual(new Vector3(-3f, 0f, 0f), moved.Direction);
		t = r;
		t.Transform(at);
		Assert.AreEqual(moved, t);
		Assert.AreEqual(Vector3.UnitY, Ray3.Normalize(r).Direction);
		t = r;
		t.Normalize();
		Assert.AreEqual(Vector3.UnitY, t.Direction);
		Assert.AreEqual(new Vector3(1f, 6f, 0f), r.Evaluate(2f));
	}

	[TestMethod]
	public void ClosestPoint_And_Distance_ClampToOrigin()
	{
		Assert.AreEqual(new Vector3(3f, 0f, 0f), R.GetClosestPoint(new Vector3(3f, 4f, 0f)));
		Assert.AreEqual(Vector3.Zero, R.GetClosestPoint(new Vector3(-3f, 4f, 0f)));
		Assert.AreEqual(4f, R.GetDistanceTo(new Vector3(3f, 4f, 0f)));
		Assert.AreEqual(5f, R.GetDistanceTo(new Vector3(-3f, 4f, 0f)));
		Ray3 unnormalized = new Ray3(Vector3.Zero, new Vector3(2f, 0f, 0f));
		AssertEx.AreEqual(new Vector3(3f, 0f, 0f), unnormalized.GetClosestPoint(new Vector3(3f, 4f, 0f), false));
		AssertEx.AreEqual(5f, unnormalized.GetDistanceTo(new Vector3(-3f, 4f, 0f), false));
		AssertEx.AreEqual(Vector3.Zero, R.GetClosestPoint(new Vector3(-3f, 4f, 0f), true));
		AssertEx.AreEqual(4f, R.GetDistanceTo(new Vector3(3f, 4f, 0f), true));
	}

	[TestMethod]
	public void IntersectPlane()
	{
		Assert.AreEqual(5f, R.FindIntersection(LinearTargets.Plane));
		Assert.IsTrue(R.Intersects(LinearTargets.Plane));
		Assert.IsNull(Away.FindIntersection(LinearTargets.Plane));
		Assert.IsFalse(Away.Intersects(LinearTargets.Plane));
		Assert.IsNull(new Ray3(Vector3.Zero, Vector3.UnitY).FindIntersection(LinearTargets.Plane));
	}

	[TestMethod]
	public void IntersectTriangle()
	{
		Assert.AreEqual(5f, R.FindIntersection(LinearTargets.Triangle));
		Assert.IsTrue(R.Intersects(LinearTargets.Triangle));
		Assert.IsNull(Away.FindIntersection(LinearTargets.Triangle));
		Assert.IsFalse(Away.Intersects(LinearTargets.Triangle));
		Assert.IsFalse(new Ray3(new Vector3(0f, 1.5f, 1.5f), Vector3.UnitX).Intersects(LinearTargets.Triangle));
	}

	[TestMethod]
	public void IntersectBoxes_ClampToOrigin()
	{
		AssertEx.AreEqual(new Interval(4f, 6f), R.FindIntersection(LinearTargets.Box));
		AssertEx.AreEqual(new Interval(0f, 1f), Inside.FindIntersection(LinearTargets.Box));
		Assert.IsNull(Away.FindIntersection(LinearTargets.Box));
		Assert.IsTrue(R.Intersects(LinearTargets.Box));
		Assert.IsFalse(Away.Intersects(LinearTargets.Box));

		float d = MathF.Sqrt(2f);
		AssertEx.AreEqual(new Interval(5f - d, 5f + d), R.FindIntersection(LinearTargets.OrientedBox));
		AssertEx.AreEqual(new Interval(0f, d), Inside.FindIntersection(LinearTargets.OrientedBox));
		Assert.IsNull(Away.FindIntersection(LinearTargets.OrientedBox));
		Assert.IsTrue(R.Intersects(LinearTargets.OrientedBox));
		Assert.IsFalse(Away.Intersects(LinearTargets.OrientedBox));
	}

	[TestMethod]
	public void IntersectSphere_ClampsToOrigin()
	{
		AssertEx.AreEqual(new Interval(4f, 6f), R.FindIntersection(LinearTargets.Sphere));
		AssertEx.AreEqual(new Interval(0f, 1f), Inside.FindIntersection(LinearTargets.Sphere));
		Assert.IsNull(Away.FindIntersection(LinearTargets.Sphere));
		Assert.IsTrue(R.Intersects(LinearTargets.Sphere));
		Assert.IsFalse(Away.Intersects(LinearTargets.Sphere));
	}

	[TestMethod]
	public void IntersectEllipsoid_ClampsToOrigin()
	{
		AssertEx.AreEqual(new Interval(3f, 7f), R.FindIntersection(LinearTargets.Ellipsoid));
		AssertEx.AreEqual(new Interval(0f, 2f), Inside.FindIntersection(LinearTargets.Ellipsoid));
		Assert.IsNull(Away.FindIntersection(LinearTargets.Ellipsoid));
		Assert.IsTrue(R.Intersects(LinearTargets.Ellipsoid));
		Assert.IsTrue(Inside.Intersects(LinearTargets.Ellipsoid));
		Assert.IsFalse(Away.Intersects(LinearTargets.Ellipsoid));
		Assert.IsFalse(new Ray3(new Vector3(0f, 1.1f, 0f), Vector3.UnitX).Intersects(LinearTargets.Ellipsoid));
	}
}

[TestClass]
public sealed class LineSegment3Tests
{
	private static readonly LineSegment3 Through = new LineSegment3(Vector3.Zero, new Vector3(10f, 0f, 0f));
	private static readonly LineSegment3 Short = new LineSegment3(Vector3.Zero, new Vector3(2f, 0f, 0f));
	private static readonly LineSegment3 FromInside = new LineSegment3(new Vector3(5f, 0f, 0f), new Vector3(15f, 0f, 0f));

	[TestMethod]
	public void Constructors_And_Properties()
	{
		LineSegment3 s = new LineSegment3(new Vector3(1f, 2f, 3f), new Vector3(3f, 5f, 9f));
		Assert.AreEqual(new Vector3(1f, 2f, 3f), s.Start);
		Assert.AreEqual(new Vector3(3f, 5f, 9f), s.End);
		Assert.AreEqual(s, new LineSegment3(new[] { new Vector3(1f, 2f, 3f), new Vector3(3f, 5f, 9f) }));
		Assert.AreEqual(7f, s.Length);
		AssertEx.AreEqual(new Vector3(2f, 3f, 6f)/7f, s.Direction);
		Assert.AreEqual(new Vector3(2f, 3.5f, 6f), s.Center);
		Assert.IsTrue(s.IsFinite);
		Assert.IsFalse(new LineSegment3(Vector3.Zero, new Vector3(Single.NaN)).IsFinite);
		CollectionAssert.AreEqual(new[] { s.Start, s.End }, s.GetEndpoints().ToArray());
		Assert.AreEqual(s.Start, s.Evaluate(0f));
		Assert.AreEqual(s.End, s.Evaluate(1f));
		s.Start = Vector3.Zero;
		s.End = Vector3.UnitZ;
		Assert.AreEqual(new LineSegment3(Vector3.Zero, Vector3.UnitZ), s);
	}

	[TestMethod]
	public void FactoryMethods()
	{
		Line3 line = new Line3(new Vector3(1f, 1f, 1f), new Vector3(2f, 0f, 0f));
		Assert.AreEqual(new LineSegment3(new Vector3(1f, 1f, 1f), new Vector3(3f, 1f, 1f)), LineSegment3.FromLine(line));
		Assert.AreEqual(new LineSegment3(new Vector3(-1f, 1f, 1f), new Vector3(5f, 1f, 1f)), LineSegment3.FromLine(line, new Interval(-1f, 2f)));
		Ray3 ray = new Ray3(new Vector3(1f, 1f, 1f), new Vector3(0f, 0f, 3f));
		Assert.AreEqual(new LineSegment3(new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, 4f)), LineSegment3.FromRay(ray));
		Assert.AreEqual(new LineSegment3(new Vector3(1f, 1f, 4f), new Vector3(1f, 1f, 7f)), LineSegment3.FromRay(ray, new Interval(1f, 2f)));
	}

	[TestMethod]
	public void Equality_Translate_Transform()
	{
		LineSegment3 same = new LineSegment3(Vector3.Zero, new Vector3(10f, 0f, 0f));
		Assert.IsTrue(Through == same);
		Assert.IsFalse(Through != same);
		Assert.IsTrue(Through.Equals((object)same));
		Assert.AreEqual(Through.GetHashCode(), same.GetHashCode());
		LineSegment3 moved = LineSegment3.Translate(Through, new Vector3(0f, 0.01f, 0f));
		Assert.IsFalse(Through.ApproxEquals(moved));
		Assert.IsTrue(Through.ApproxEquals(moved, 0.1f));
		LineSegment3 t = Through;
		t.Translate(new Vector3(0f, 0.01f, 0f));
		Assert.AreEqual(moved, t);

		Matrix3 m = Matrix3.Rotation(Axis.Z, SingleConstants.HalfPi);
		LineSegment3 rotated = LineSegment3.Transform(Through, m);
		AssertEx.AreEqual(new Vector3(0f, 10f, 0f), rotated.End);
		t = Through;
		t.Transform(m);
		Assert.AreEqual(rotated, t);
		AffineTransform at = new AffineTransform(m, Vector3.UnitZ);
		LineSegment3 transformed = LineSegment3.Transform(Through, at);
		AssertEx.AreEqual(Vector3.UnitZ, transformed.Start);
		AssertEx.AreEqual(new Vector3(0f, 10f, 1f), transformed.End);
		t = Through;
		t.Transform(at);
		Assert.AreEqual(transformed, t);
	}

	[TestMethod]
	public void ClosestPoint_And_Distance_ClampToEndpoints()
	{
		Assert.AreEqual(new Vector3(3f, 0f, 0f), Through.GetClosestPoint(new Vector3(3f, 4f, 0f)));
		Assert.AreEqual(Vector3.Zero, Through.GetClosestPoint(new Vector3(-3f, 4f, 0f)));
		Assert.AreEqual(new Vector3(10f, 0f, 0f), Through.GetClosestPoint(new Vector3(13f, 0f, 4f)));
		Assert.AreEqual(5f, Through.GetDistanceTo(new Vector3(13f, 0f, 4f)));
	}

	[TestMethod]
	public void IntersectHalfSpace_And_Plane()
	{
		HalfSpace behind = new HalfSpace(Vector3.UnitX, new Vector3(1f, 0f, 0f));
		Assert.IsTrue(Short.Intersects(behind));
		Assert.IsFalse(LineSegment3.Translate(Short, new Vector3(5f, 0f, 0f)).Intersects(behind));
		Assert.AreEqual(0.5f, Through.FindIntersection(LinearTargets.Plane));
		Assert.IsTrue(Through.Intersects(LinearTargets.Plane));
		Assert.IsNull(Short.FindIntersection(LinearTargets.Plane));
		Assert.IsFalse(Short.Intersects(LinearTargets.Plane));
	}

	[TestMethod]
	public void IntersectTriangle()
	{
		Assert.AreEqual(0.5f, Through.FindIntersection(LinearTargets.Triangle));
		Assert.IsTrue(Through.Intersects(LinearTargets.Triangle));
		Assert.IsNull(Short.FindIntersection(LinearTargets.Triangle));
		Assert.IsFalse(Short.Intersects(LinearTargets.Triangle));
	}

	[TestMethod]
	public void IntersectBoxes_ClampToSegment()
	{
		AssertEx.AreEqual(new Interval(0.4f, 0.6f), Through.FindIntersection(LinearTargets.Box));
		AssertEx.AreEqual(new Interval(0f, 0.1f), FromInside.FindIntersection(LinearTargets.Box));
		Assert.IsNull(Short.FindIntersection(LinearTargets.Box));
		Assert.IsTrue(Through.Intersects(LinearTargets.Box));
		Assert.IsFalse(Short.Intersects(LinearTargets.Box));
		LineSegment3 within = new LineSegment3(new Vector3(4.5f, 0f, 0f), new Vector3(5.5f, 0f, 0f));
		AssertEx.AreEqual(new Interval(0f, 1f), within.FindIntersection(LinearTargets.Box));

		float d = MathF.Sqrt(2f);
		AssertEx.AreEqual(new Interval((5f - d)/10f, (5f + d)/10f), Through.FindIntersection(LinearTargets.OrientedBox));
		AssertEx.AreEqual(new Interval(0f, d/10f), FromInside.FindIntersection(LinearTargets.OrientedBox));
		Assert.IsNull(Short.FindIntersection(LinearTargets.OrientedBox));
		Assert.IsTrue(Through.Intersects(LinearTargets.OrientedBox));
		Assert.IsFalse(Short.Intersects(LinearTargets.OrientedBox));
	}

	[TestMethod]
	public void IntersectSphere_ClampsToSegment()
	{
		AssertEx.AreEqual(new Interval(0.4f, 0.6f), Through.FindIntersection(LinearTargets.Sphere));
		AssertEx.AreEqual(new Interval(0f, 0.1f), FromInside.FindIntersection(LinearTargets.Sphere));
		Assert.IsNull(Short.FindIntersection(LinearTargets.Sphere));
		Assert.IsTrue(Through.Intersects(LinearTargets.Sphere));
		Assert.IsFalse(Short.Intersects(LinearTargets.Sphere));
	}

	[TestMethod]
	public void IntersectEllipsoid_ClampsToSegment()
	{
		AssertEx.AreEqual(new Interval(0.3f, 0.7f), Through.FindIntersection(LinearTargets.Ellipsoid));
		AssertEx.AreEqual(new Interval(0f, 0.2f), FromInside.FindIntersection(LinearTargets.Ellipsoid));
		Assert.IsNull(Short.FindIntersection(LinearTargets.Ellipsoid));
		Assert.IsTrue(Through.Intersects(LinearTargets.Ellipsoid));
		Assert.IsTrue(FromInside.Intersects(LinearTargets.Ellipsoid));
		Assert.IsFalse(Short.Intersects(LinearTargets.Ellipsoid));
		// Segment entirely inside.
		LineSegment3 within = new LineSegment3(new Vector3(4.5f, 0f, 0f), new Vector3(5.5f, 0f, 0f));
		Assert.IsTrue(within.Intersects(LinearTargets.Ellipsoid));
		AssertEx.AreEqual(new Interval(0f, 1f), within.FindIntersection(LinearTargets.Ellipsoid));
	}
}
