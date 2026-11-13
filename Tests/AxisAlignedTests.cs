using System.Globalization;

namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class AxisAlignedRectangleTests
{
	private static readonly AxisAlignedRectangle R = new AxisAlignedRectangle(new Vector2(1f, 2f), new Vector2(4f, 6f));

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(new Vector2(1f, 2f), R.Minimum);
		Assert.AreEqual(new Vector2(4f, 6f), R.Maximum);
		Assert.AreEqual(new Vector2(3f, 4f), R.Dimensions);
		Assert.AreEqual(new Vector2(1.5f, 2f), R.HalfDimensions);
		Assert.AreEqual(new Vector2(2.5f, 4f), R.Center);
		Assert.AreEqual(5f, R.Diagonal);
		Assert.AreEqual(14f, R.Perimeter);
		Assert.AreEqual(12f, R.Area);
		Assert.IsTrue(R.IsFinite);
		Assert.IsFalse(R.IsEmpty);

		AxisAlignedRectangle centered = new AxisAlignedRectangle(new Vector2(4f, 2f));
		Assert.AreEqual(new Vector2(-2f, -1f), centered.Minimum);
		Assert.AreEqual(new Vector2(2f, 1f), centered.Maximum);

		AxisAlignedRectangle r = R;
		r.Minimum = Vector2.Zero;
		r.Maximum = new Vector2(1f);
		Assert.AreEqual(new AxisAlignedRectangle(Vector2.Zero, new Vector2(1f)), r);
	}

	[TestMethod]
	public void Empty_IsEmptyAndNotFinite()
	{
		Assert.IsTrue(AxisAlignedRectangle.Empty.IsEmpty);
		Assert.IsFalse(AxisAlignedRectangle.Empty.IsFinite);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		AxisAlignedRectangle same = new AxisAlignedRectangle(new Vector2(1f, 2f), new Vector2(4f, 6f));
		Assert.IsTrue(R == same);
		Assert.IsFalse(R != same);
		Assert.IsTrue(R.Equals((object)same));
		Assert.IsFalse(R.Equals((object)R.Minimum));
		Assert.AreEqual(R.GetHashCode(), same.GetHashCode());
		AxisAlignedRectangle moved = AxisAlignedRectangle.Translate(R, new Vector2(0.01f, 0f));
		Assert.IsFalse(R.ApproxEquals(moved));
		Assert.IsTrue(R.ApproxEquals(moved, 0.1f));
		Assert.IsTrue(R != moved);
	}

	[TestMethod]
	public void GetCircumscribedCircle_And_Vertices()
	{
		Circle2 c = R.GetCircumscribedCircle();
		Assert.AreEqual(R.Center, c.Center);
		AssertEx.AreEqual(2.5f, c.Radius);
		Vector2[] vertices = R.GetVertices().ToArray();
		Assert.HasCount(4, vertices);
		CollectionAssert.AreEquivalent(new[] { new Vector2(1f, 2f), new Vector2(4f, 2f), new Vector2(1f, 6f), new Vector2(4f, 6f) }, vertices);
		foreach (Vector2 v in vertices)
			Assert.IsTrue(c.Contains(v + 1e-4f*(c.Center - v)));
	}

	[TestMethod]
	public void UnionIntersectionExtendInflate()
	{
		AxisAlignedRectangle other = new AxisAlignedRectangle(new Vector2(3f, -1f), new Vector2(5f, 3f));
		Assert.AreEqual(new AxisAlignedRectangle(new Vector2(1f, -1f), new Vector2(5f, 6f)), AxisAlignedRectangle.Union(R, other));
		Assert.AreEqual(new AxisAlignedRectangle(new Vector2(3f, 2f), new Vector2(4f, 3f)), AxisAlignedRectangle.Intersection(R, other));
		Assert.IsTrue(AxisAlignedRectangle.Intersection(R, new AxisAlignedRectangle(new Vector2(10f), new Vector2(11f))).IsEmpty);

		AxisAlignedRectangle r = AxisAlignedRectangle.Empty;
		r.ExtendBy(new Vector2(1f, 1f));
		r.ExtendBy(new Vector2(-1f, 3f));
		Assert.AreEqual(new AxisAlignedRectangle(new Vector2(-1f, 1f), new Vector2(1f, 3f)), r);
		r.ExtendBy(R);
		Assert.AreEqual(new AxisAlignedRectangle(new Vector2(-1f, 1f), new Vector2(4f, 6f)), r);
		r.ExtendBy(new Circle2(new Vector2(0f, 10f), 2f));
		Assert.AreEqual(new AxisAlignedRectangle(new Vector2(-2f, 1f), new Vector2(4f, 12f)), r);

		AxisAlignedRectangle inflated = R;
		inflated.Inflate(new Vector2(1f, 0.5f));
		Assert.AreEqual(new AxisAlignedRectangle(new Vector2(0f, 1.5f), new Vector2(5f, 6.5f)), inflated);
	}

	[TestMethod]
	public void Translate_And_ScaleAroundCenter()
	{
		Vector2 offset = new Vector2(1f, -1f);
		AxisAlignedRectangle translated = new AxisAlignedRectangle(new Vector2(2f, 1f), new Vector2(5f, 5f));
		Assert.AreEqual(translated, AxisAlignedRectangle.Translate(R, offset));
		AxisAlignedRectangle r = R;
		r.Translate(offset);
		Assert.AreEqual(translated, r);

		AxisAlignedRectangle scaled = AxisAlignedRectangle.ScaleAroundCenter(R, 2f);
		Assert.AreEqual(R.Center, scaled.Center);
		Assert.AreEqual(2f*R.Dimensions, scaled.Dimensions);
		r = R;
		r.ScaleAroundCenter(2f);
		Assert.AreEqual(scaled, r);
	}

	[TestMethod]
	public void ClosestPoint_Distance_Contains()
	{
		Assert.AreEqual(new Vector2(2f, 3f), R.GetClosestPoint(new Vector2(2f, 3f)));
		Assert.AreEqual(new Vector2(4f, 6f), R.GetClosestPoint(new Vector2(7f, 10f)));
		Assert.AreEqual(new Vector2(1f, 4f), R.GetClosestPoint(new Vector2(-3f, 4f)));
		Assert.AreEqual(5f, R.GetDistanceTo(new Vector2(7f, 10f)));
		Assert.AreEqual(0f, R.GetDistanceTo(new Vector2(2f, 3f)));
		Assert.IsTrue(R.Contains(new Vector2(1f, 2f)));
		Assert.IsTrue(R.Contains(new Vector2(2.5f, 5f)));
		Assert.IsFalse(R.Contains(new Vector2(0.9f, 3f)));
		Assert.IsTrue(R.Contains(new AxisAlignedRectangle(new Vector2(2f, 3f), new Vector2(4f, 6f))));
		Assert.IsFalse(R.Contains(new AxisAlignedRectangle(new Vector2(2f, 3f), new Vector2(5f, 6f))));
		Assert.IsTrue(R.Contains(new Circle2(new Vector2(2.5f, 4f), 1.5f)));
		Assert.IsFalse(R.Contains(new Circle2(new Vector2(2.5f, 4f), 1.6f)));
	}

	[TestMethod]
	public void Intersects()
	{
		Assert.IsTrue(R.Intersects(new AxisAlignedRectangle(new Vector2(4f, 6f), new Vector2(5f, 7f))));
		Assert.IsTrue(R.Intersects(new AxisAlignedRectangle(new Vector2(0f), new Vector2(10f))));
		Assert.IsFalse(R.Intersects(new AxisAlignedRectangle(new Vector2(4.1f, 0f), new Vector2(5f, 7f))));
		Assert.IsTrue(R.Intersects(new Circle2(new Vector2(5f, 4f), 1f)));
		Assert.IsFalse(R.Intersects(new Circle2(new Vector2(5f, 7f), 1f)));
		Assert.IsTrue(R.Intersects(new Circle2(new Vector2(5f, 7f), 1.5f)));
		Assert.IsTrue(R.Intersects(new Circle2(new Vector2(2f, 3f), 0.1f)));
	}

	[TestMethod]
	public void ToString_And_Parse()
	{
		const string text = "1 2 4 6";
		Assert.AreEqual(text, R.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("1.0 2.0 4.0 6.0", R.ToString("F1", CultureInfo.InvariantCulture));
		Assert.AreEqual(R, AxisAlignedRectangle.Parse(text, CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual(text, R.ToString());
			Assert.AreEqual("1.0 2.0 4.0 6.0", R.ToString("F1"));
			Assert.AreEqual(R, AxisAlignedRectangle.Parse(text));
			Assert.AreEqual(AxisAlignedRectangle.Empty, AxisAlignedRectangle.Parse(AxisAlignedRectangle.Empty.ToString()));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => AxisAlignedRectangle.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => AxisAlignedRectangle.Parse("1 2 3", CultureInfo.InvariantCulture));
	}
}

[TestClass]
public sealed class AxisAlignedBoxTests
{
	private static readonly AxisAlignedBox B = new AxisAlignedBox(new Vector3(1f, 2f, 3f), new Vector3(3f, 5f, 9f));

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(new Vector3(1f, 2f, 3f), B.Minimum);
		Assert.AreEqual(new Vector3(3f, 5f, 9f), B.Maximum);
		Assert.AreEqual(new Vector3(2f, 3f, 6f), B.Dimensions);
		Assert.AreEqual(new Vector3(1f, 1.5f, 3f), B.HalfDimensions);
		Assert.AreEqual(new Vector3(2f, 3.5f, 6f), B.Center);
		Assert.AreEqual(7f, B.Diagonal);
		Assert.AreEqual(2f*(6f + 18f + 12f), B.SurfaceArea);
		Assert.AreEqual(36f, B.Volume);
		Assert.IsTrue(B.IsFinite);
		Assert.IsFalse(B.IsEmpty);

		AxisAlignedBox centered = new AxisAlignedBox(new Vector3(2f, 4f, 6f));
		Assert.AreEqual(new Vector3(-1f, -2f, -3f), centered.Minimum);
		Assert.AreEqual(new Vector3(1f, 2f, 3f), centered.Maximum);

		AxisAlignedBox b = B;
		b.Minimum = Vector3.Zero;
		b.Maximum = new Vector3(1f);
		Assert.AreEqual(new AxisAlignedBox(Vector3.Zero, new Vector3(1f)), b);
	}

	[TestMethod]
	public void Empty_IsEmptyAndNotFinite()
	{
		Assert.IsTrue(AxisAlignedBox.Empty.IsEmpty);
		Assert.IsFalse(AxisAlignedBox.Empty.IsFinite);
	}

	[TestMethod]
	public void Equality_HashCode()
	{
		AxisAlignedBox same = new AxisAlignedBox(new Vector3(1f, 2f, 3f), new Vector3(3f, 5f, 9f));
		Assert.IsTrue(B == same);
		Assert.IsFalse(B != same);
		Assert.IsTrue(B.Equals((object)same));
		Assert.IsTrue(B.Equals(same));
		Assert.IsFalse(B.Equals((object)B.Minimum));
		Assert.AreEqual(B.GetHashCode(), same.GetHashCode());
		Assert.IsTrue(B != AxisAlignedBox.Translate(B, Vector3.UnitX));
		AxisAlignedBox moved = AxisAlignedBox.Translate(B, new Vector3(0f, 0f, 0.01f));
		Assert.IsTrue(B.ApproxEquals(same));
		Assert.IsFalse(B.ApproxEquals(moved));
		Assert.IsTrue(B.ApproxEquals(moved, 0.1f));
	}

	[TestMethod]
	public void CircumscribedSphere_And_Vertices()
	{
		Sphere s = B.GetCircumscribedSphere();
		Assert.AreEqual(B.Center, s.Center);
		AssertEx.AreEqual(3.5f, s.Radius);
		Vector3[] vertices = B.GetVertices().ToArray();
		Assert.HasCount(8, vertices);
		Assert.AreEqual(8, vertices.Distinct().Count());
		foreach (Vector3 v in vertices)
		{
			Assert.IsTrue(v.X == 1f || v.X == 3f);
			Assert.IsTrue(v.Y == 2f || v.Y == 5f);
			Assert.IsTrue(v.Z == 3f || v.Z == 9f);
			AssertEx.AreEqual(s.Radius, Vector3.Distance(v, s.Center));
		}
	}

	[TestMethod]
	public void HalfSpaces_BoundTheBox()
	{
		HalfSpace[] halfSpaces = B.GetHalfSpaces().ToArray();
		Assert.HasCount(6, halfSpaces);
		Vector3[] inside = { B.Center, B.Minimum, B.Maximum, new Vector3(2.9f, 2.1f, 8f) };
		Vector3[] outside = { new Vector3(0.9f, 3f, 5f), new Vector3(2f, 5.1f, 5f), new Vector3(2f, 3f, 2.9f), new Vector3(10f) };
		foreach (Vector3 p in inside)
			Assert.IsTrue(halfSpaces.All(h => h.Contains(p)), p.ToString());
		foreach (Vector3 p in outside)
			Assert.IsFalse(halfSpaces.All(h => h.Contains(p)), p.ToString());
	}

	[TestMethod]
	public void UnionIntersectionExtendInflate()
	{
		AxisAlignedBox other = new AxisAlignedBox(new Vector3(2f, 0f, 4f), new Vector3(6f, 3f, 5f));
		Assert.AreEqual(new AxisAlignedBox(new Vector3(1f, 0f, 3f), new Vector3(6f, 5f, 9f)), AxisAlignedBox.Union(B, other));
		Assert.AreEqual(new AxisAlignedBox(new Vector3(2f, 2f, 4f), new Vector3(3f, 3f, 5f)), AxisAlignedBox.Intersection(B, other));
		Assert.IsTrue(AxisAlignedBox.Intersection(B, AxisAlignedBox.Translate(B, new Vector3(10f))).IsEmpty);

		AxisAlignedBox b = AxisAlignedBox.Empty;
		b.ExtendBy(new Vector3(1f));
		b.ExtendBy(new Vector3(-1f, 2f, 0f));
		Assert.AreEqual(new AxisAlignedBox(new Vector3(-1f, 1f, 0f), new Vector3(1f, 2f, 1f)), b);
		b.ExtendBy(B);
		Assert.AreEqual(new AxisAlignedBox(new Vector3(-1f, 1f, 0f), new Vector3(3f, 5f, 9f)), b);
		b.ExtendBy(new Sphere(new Vector3(0f, 0f, 20f), 1f));
		Assert.AreEqual(new AxisAlignedBox(new Vector3(-1f, -1f, 0f), new Vector3(3f, 5f, 21f)), b);

		AxisAlignedBox inflated = B;
		inflated.Inflate(new Vector3(1f, 2f, 3f));
		Assert.AreEqual(new AxisAlignedBox(new Vector3(0f, 0f, 0f), new Vector3(4f, 7f, 12f)), inflated);
	}

	[TestMethod]
	public void Translate_And_ScaleAroundCenter()
	{
		AxisAlignedBox moved = new AxisAlignedBox(new Vector3(2f, 2f, 2f), new Vector3(4f, 5f, 8f));
		Assert.AreEqual(moved, AxisAlignedBox.Translate(B, new Vector3(1f, 0f, -1f)));
		AxisAlignedBox b = B;
		b.Translate(new Vector3(1f, 0f, -1f));
		Assert.AreEqual(moved, b);

		AxisAlignedBox scaled = AxisAlignedBox.ScaleAroundCenter(B, 0.5f);
		Assert.AreEqual(B.Center, scaled.Center);
		Assert.AreEqual(0.5f*B.Dimensions, scaled.Dimensions);
		b = B;
		b.ScaleAroundCenter(0.5f);
		Assert.AreEqual(scaled, b);
	}

	[TestMethod]
	public void Transform_ReturnsBoundsOfTransformedBox()
	{
		AxisAlignedBox unit = new AxisAlignedBox(new Vector3(-1f), new Vector3(1f));
		Matrix3 rotation = Matrix3.Rotation(Axis.Z, SingleConstants.Pi/4f);
		float h = MathF.Sqrt(2f);
		AxisAlignedBox expected = new AxisAlignedBox(new Vector3(-h, -h, -1f), new Vector3(h, h, 1f));
		AxisAlignedBox rotated = AxisAlignedBox.Transform(unit, rotation, true);
		AssertEx.AreEqual(expected.Minimum, rotated.Minimum);
		AssertEx.AreEqual(expected.Maximum, rotated.Maximum);

		AffineTransform at = new AffineTransform(rotation, new Vector3(10f, 0f, 0f));
		AxisAlignedBox moved = AxisAlignedBox.Transform(unit, at);
		AssertEx.AreEqual(expected.Minimum + new Vector3(10f, 0f, 0f), moved.Minimum);
		AssertEx.AreEqual(expected.Maximum + new Vector3(10f, 0f, 0f), moved.Maximum);

		// For a scaling + rotation the bounds are tight: each face touches a transformed corner.
		AffineTransform srt = AffineTransform.ScalingRotationTranslation(new Vector3(2f, 0.5f, 3f), Samples.Axes[4], 0.8f, new Vector3(1f, 2f, 3f));
		AxisAlignedBox bounds = AxisAlignedBox.Transform(B, srt);
		Vector3[] corners = B.GetVertices().Select(v => Vector3.Transform(v, srt)).ToArray();
		for (int axis = 0; axis < 3; axis++)
		{
			AssertEx.AreEqual(corners.Min(c => c[axis]), bounds.Minimum[axis], 1e-4f);
			AssertEx.AreEqual(corners.Max(c => c[axis]), bounds.Maximum[axis], 1e-4f);
		}

		AxisAlignedBox b = unit;
		b.Transform(rotation, true);
		AssertEx.AreEqual(expected.Maximum, b.Maximum);
		b = unit;
		b.Transform(at);
		AssertEx.AreEqual(expected.Maximum + new Vector3(10f, 0f, 0f), b.Maximum);
	}

	[TestMethod]
	public void Transform_GeneralMatrix_GivesTightBoundsOfTransformedCorners()
	{
		AffineTransform general = new AffineTransform(Samples.GeneralMatrix3, new Vector3(1f, 2f, 3f));
		Vector3[] corners = B.GetVertices().Select(v => Vector3.Transform(v, general)).ToArray();
		foreach (bool orthogonal in new[] { false, true })
		{
			AxisAlignedBox[] results = { AxisAlignedBox.Transform(B, general, orthogonal), B, AxisAlignedBox.Transform(B, general.Basis, orthogonal) };
			results[1].Transform(general, orthogonal);
			Vector3[] linearCorners = B.GetVertices().Select(v => v*general.Basis).ToArray();
			for (int axis = 0; axis < 3; axis++)
			{
				for (int i = 0; i < 2; i++)
				{
					AssertEx.AreEqual(corners.Min(c => c[axis]), results[i].Minimum[axis], 1e-4f, $"orthogonal={orthogonal}");
					AssertEx.AreEqual(corners.Max(c => c[axis]), results[i].Maximum[axis], 1e-4f, $"orthogonal={orthogonal}");
				}
				AssertEx.AreEqual(linearCorners.Min(c => c[axis]), results[2].Minimum[axis], 1e-4f, $"orthogonal={orthogonal}");
				AssertEx.AreEqual(linearCorners.Max(c => c[axis]), results[2].Maximum[axis], 1e-4f, $"orthogonal={orthogonal}");
			}
		}
	}

	[TestMethod]
	public void FromOrientedBox_BoundsAllCorners()
	{
		OrientedBox box = new OrientedBox(new Vector3(1f, 2f, 3f), Matrix3.Rotation(Samples.Axes[3], 0.7f), new Vector3(1f, 2f, 0.5f));
		AxisAlignedBox aab = AxisAlignedBox.FromOrientedBox(box);
		Vector3[] corners = box.GetVertices().ToArray();
		for (int axis = 0; axis < 3; axis++)
		{
			AssertEx.AreEqual(corners.Min(c => c[axis]), aab.Minimum[axis], 1e-5f);
			AssertEx.AreEqual(corners.Max(c => c[axis]), aab.Maximum[axis], 1e-5f);
		}
	}

	[TestMethod]
	public void ClosestPoint_Distance_Contains()
	{
		Assert.AreEqual(new Vector3(2f, 3f, 4f), B.GetClosestPoint(new Vector3(2f, 3f, 4f)));
		Assert.AreEqual(new Vector3(3f, 5f, 9f), B.GetClosestPoint(new Vector3(5f, 8f, 15f)));
		Assert.AreEqual(7f, B.GetDistanceTo(new Vector3(5f, 8f, 15f)));
		Assert.AreEqual(0f, B.GetDistanceTo(B.Center));
		Assert.IsTrue(B.Contains(B.Minimum));
		Assert.IsTrue(B.Contains(B.Center));
		Assert.IsFalse(B.Contains(new Vector3(0f, 3f, 4f)));
		Assert.IsTrue(B.Contains(AxisAlignedBox.ScaleAroundCenter(B, 0.5f)));
		Assert.IsFalse(B.Contains(AxisAlignedBox.ScaleAroundCenter(B, 1.5f)));
		Assert.IsTrue(B.Contains(new Sphere(B.Center, 1f)));
		Assert.IsFalse(B.Contains(new Sphere(B.Center, 1.1f)));
	}

	[TestMethod]
	public void Intersects_PlaneAndHalfSpace()
	{
		Assert.IsTrue(B.Intersects(new Plane(Vector3.UnitZ, new Vector3(0f, 0f, 5f))));
		Assert.IsTrue(B.Intersects(new Plane(Vector3.UnitZ, new Vector3(0f, 0f, 9f))));
		Assert.IsFalse(B.Intersects(new Plane(Vector3.UnitZ, new Vector3(0f, 0f, 9.5f))));
		Assert.IsFalse(B.Intersects(new Plane(Vector3.Normalize(new Vector3(1f, 1f, 1f)), new Vector3(10f))));
		// Half-space contains points behind the normal.
		Assert.IsTrue(B.Intersects(new HalfSpace(Vector3.UnitX, new Vector3(1.5f, 0f, 0f))));
		Assert.IsTrue(B.Intersects(new HalfSpace(Vector3.UnitX, new Vector3(10f, 0f, 0f))));
		Assert.IsFalse(B.Intersects(new HalfSpace(Vector3.UnitX, new Vector3(0.5f, 0f, 0f))));
		Assert.IsTrue(B.Intersects(new HalfSpace(-Vector3.UnitX, new Vector3(0.5f, 0f, 0f))));
	}

	[TestMethod]
	public void Intersects_Triangle()
	{
		Assert.IsTrue(B.Intersects(new Triangle3(B.Center, B.Center + Vector3.UnitX, B.Center + Vector3.UnitY)));
		// Large triangle slicing through the box without any vertex inside.
		Assert.IsTrue(B.Intersects(new Triangle3(new Vector3(-10f, -10f, 6f), new Vector3(20f, -10f, 6f), new Vector3(0f, 20f, 6f))));
		Assert.IsFalse(B.Intersects(new Triangle3(new Vector3(-10f, -10f, 20f), new Vector3(20f, -10f, 20f), new Vector3(0f, 20f, 20f))));
		// Triangles whose bounding boxes overlap the box near its (3, 5, 9) corner; x + y + z <= 17 inside the box.
		Assert.IsFalse(B.Intersects(new Triangle3(new Vector3(12.5f, 2f, 3f), new Vector3(1f, 13.5f, 3f), new Vector3(1f, 2f, 14.5f))));
		Assert.IsTrue(B.Intersects(new Triangle3(new Vector3(11.5f, 2f, 3f), new Vector3(1f, 12.5f, 3f), new Vector3(1f, 2f, 13.5f))));
		// Bounding boxes overlap, triangle plane crosses the box, but the triangle itself misses it.
		Assert.IsFalse(B.Intersects(new Triangle3(new Vector3(0f, 0f, 6f), new Vector3(1.5f, -1f, 6f), new Vector3(-1f, 2.5f, 6f))));
	}

	[TestMethod]
	public void Intersects_Boxes_And_Spheres()
	{
		Assert.IsTrue(B.Intersects(new AxisAlignedBox(new Vector3(3f, 5f, 9f), new Vector3(4f))));
		Assert.IsFalse(B.Intersects(AxisAlignedBox.Translate(B, new Vector3(2.1f, 0f, 0f))));
		Assert.IsTrue(B.Intersects(new OrientedBox(B.Center, Matrix3.Rotation(Axis.Y, 0.5f), new Vector3(0.1f))));
		Assert.IsFalse(B.Intersects(new OrientedBox(new Vector3(20f), Matrix3.Rotation(Axis.Y, 0.5f), new Vector3(1f))));
		Assert.IsTrue(B.Intersects(new Sphere(new Vector3(4f, 3f, 5f), 1.01f)));
		Assert.IsFalse(B.Intersects(new Sphere(new Vector3(4f, 6f, 10f), 1.7f)));
		Assert.IsTrue(B.Intersects(new Sphere(new Vector3(4f, 6f, 10f), 1.74f)));
	}

	[TestMethod]
	public void ToString_And_Parse()
	{
		const string text = "1 2 3 3 5 9";
		Assert.AreEqual(text, B.ToString(CultureInfo.InvariantCulture));
		Assert.AreEqual("1.0 2.0 3.0 3.0 5.0 9.0", B.ToString("F1", CultureInfo.InvariantCulture));
		Assert.AreEqual(B, AxisAlignedBox.Parse(text, CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual(text, B.ToString());
			Assert.AreEqual("1.0 2.0 3.0 3.0 5.0 9.0", B.ToString("F1"));
			Assert.AreEqual(B, AxisAlignedBox.Parse(text));
			Assert.AreEqual(AxisAlignedBox.Empty, AxisAlignedBox.Parse(AxisAlignedBox.Empty.ToString()));
		}
		Assert.ThrowsExactly<ArgumentNullException>(() => AxisAlignedBox.Parse(null));
		Assert.ThrowsExactly<FormatException>(() => AxisAlignedBox.Parse("1 2 3 4 5", CultureInfo.InvariantCulture));
	}
}
