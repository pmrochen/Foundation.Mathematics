namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class OrientedBoxTests
{
	private static readonly Matrix3 Rot45Z = Matrix3.Rotation(Axis.Z, SingleConstants.Pi/4f);
	private static readonly OrientedBox B = new OrientedBox(new Vector3(1f, 2f, 3f), Rot45Z, new Vector3(1f, 2f, 3f));

	[TestMethod]
	public void Constructor_And_Properties()
	{
		Assert.AreEqual(new Vector3(1f, 2f, 3f), B.Center);
		Assert.AreEqual(Rot45Z, B.Basis);
		Assert.AreEqual(new Vector3(1f, 2f, 3f), B.HalfDimensions);
		Assert.AreEqual(new Vector3(2f, 4f, 6f), B.Dimensions);
		AssertEx.AreEqual(2f*MathF.Sqrt(14f), B.Diagonal);
		Assert.AreEqual(2f*(8f + 24f + 12f), B.SurfaceArea);
		Assert.AreEqual(48f, B.Volume);
		Assert.IsTrue(B.IsFinite);
		Assert.IsFalse(new OrientedBox(Vector3.Zero, Matrix3.Identity, new Vector3(Single.NaN)).IsFinite);

		OrientedBox b = B;
		b.Center = Vector3.Zero;
		b.Basis = Matrix3.Identity;
		b.HalfDimensions = new Vector3(1f);
		Assert.AreEqual(new OrientedBox(Vector3.Zero, Matrix3.Identity, new Vector3(1f)), b);
		b.Dimensions = new Vector3(4f);
		Assert.AreEqual(new Vector3(2f), b.HalfDimensions);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		OrientedBox same = new OrientedBox(new Vector3(1f, 2f, 3f), Rot45Z, new Vector3(1f, 2f, 3f));
		Assert.IsTrue(B == same);
		Assert.IsFalse(B != same);
		Assert.IsTrue(B.Equals((object)same));
		Assert.AreEqual(B.GetHashCode(), same.GetHashCode());
		OrientedBox moved = OrientedBox.Translate(B, new Vector3(0.01f, 0f, 0f));
		Assert.IsTrue(B != moved);
		Assert.IsFalse(B.ApproxEquals(moved, 1e-3f));
		Assert.IsTrue(B.ApproxEquals(moved, 0.1f));
	}

	[TestMethod]
	public void FromAxisAlignedBox()
	{
		AxisAlignedBox aab = new AxisAlignedBox(new Vector3(0f, 0f, 0f), new Vector3(2f, 4f, 6f));
		Assert.AreEqual(new OrientedBox(new Vector3(1f, 2f, 3f), Matrix3.Identity, new Vector3(1f, 2f, 3f)), OrientedBox.FromAxisAlignedBox(aab));

		Matrix3 sr = Matrix3.ScalingRotation(new Vector3(2f, 1f, 3f), Axis.Y, 0.4f);
		OrientedBox byMatrix = OrientedBox.FromAxisAlignedBox(aab, sr);
		AssertEx.AreEqual(aab.Center*sr, byMatrix.Center);
		AssertEx.AreEqual(new Vector3(2f, 2f, 9f), byMatrix.HalfDimensions);
		AssertEx.AreEqual(Matrix3.Rotation(Axis.Y, 0.4f), byMatrix.Basis);

		AffineTransform at = new AffineTransform(sr, new Vector3(1f, 1f, 1f));
		OrientedBox byAffine = OrientedBox.FromAxisAlignedBox(aab, at);
		AssertEx.AreEqual(Vector3.Transform(aab.Center, at), byAffine.Center);
		AssertEx.AreEqual(new Vector3(2f, 2f, 9f), byAffine.HalfDimensions);

		OrientedBox orthogonal = OrientedBox.FromAxisAlignedBox(aab, Rot45Z, true);
		Assert.AreEqual(Rot45Z, orthogonal.Basis);
		Assert.AreEqual(aab.HalfDimensions, orthogonal.HalfDimensions);
		OrientedBox orthogonalAffine = OrientedBox.FromAxisAlignedBox(aab, new AffineTransform(Rot45Z, Vector3.UnitX), true);
		Assert.AreEqual(Rot45Z, orthogonalAffine.Basis);
	}

	[TestMethod]
	public void Vertices_AreCornersOfTheBox()
	{
		Vector3[] vertices = B.GetVertices().ToArray();
		Assert.HasCount(8, vertices);
		foreach (Vector3 v in vertices)
		{
			AssertEx.AreEqual(B.HalfDimensions.Magnitude, Vector3.Distance(v, B.Center), 1e-5f);
			Vector3 local = Rot45Z*(v - B.Center);
			AssertEx.AreEqual(B.HalfDimensions, Vector3.Abs(local), 1e-5f);
		}
		Assert.AreEqual(8, vertices.Select(v => Rot45Z*(v - B.Center)).Select(l => (Math.Sign(l.X), Math.Sign(l.Y), Math.Sign(l.Z))).Distinct().Count());
	}

	[TestMethod]
	public void HalfSpaces_BoundTheBox()
	{
		HalfSpace[] halfSpaces = B.GetHalfSpaces().ToArray();
		Assert.HasCount(6, halfSpaces);
		Vector3 x = B.Basis.Row0, y = B.Basis.Row1, z = B.Basis.Row2;
		Vector3[] inside = { B.Center, B.Center + 0.9f*x + 1.9f*y - 2.9f*z };
		Vector3[] outside = { B.Center + 1.1f*x, B.Center - 2.1f*y, B.Center + 3.1f*z };
		foreach (Vector3 p in inside)
			Assert.IsTrue(halfSpaces.All(h => h.Contains(p)));
		foreach (Vector3 p in outside)
			Assert.IsFalse(halfSpaces.All(h => h.Contains(p)));
	}

	[TestMethod]
	public void CircumscribedBox_And_Sphere()
	{
		AxisAlignedBox aab = B.GetCircumscribedBox();
		Vector3[] vertices = B.GetVertices().ToArray();
		for (int axis = 0; axis < 3; axis++)
		{
			AssertEx.AreEqual(vertices.Min(v => v[axis]), aab.Minimum[axis], 1e-5f);
			AssertEx.AreEqual(vertices.Max(v => v[axis]), aab.Maximum[axis], 1e-5f);
		}
		Sphere s = B.GetCircumscribedSphere();
		Assert.AreEqual(B.Center, s.Center);
		AssertEx.AreEqual(MathF.Sqrt(14f), s.Radius);
	}

	[TestMethod]
	public void Orthonormalize_MovesScaleIntoHalfDimensions()
	{
		OrientedBox b = new OrientedBox(Vector3.Zero, Matrix3.ScalingRotation(new Vector3(2f, 3f, 4f), Axis.X, 0.3f), new Vector3(1f));
		OrientedBox o = OrientedBox.Orthonormalize(b);
		AssertEx.IsOrthonormal(o.Basis);
		AssertEx.AreEqual(new Vector3(2f, 3f, 4f), o.HalfDimensions);
		b.Orthonormalize();
		Assert.IsTrue(o.ApproxEquals(b, 1e-6f));
	}

	[TestMethod]
	public void Translate_And_Transform()
	{
		OrientedBox moved = OrientedBox.Translate(B, new Vector3(1f, 1f, 1f));
		Assert.AreEqual(new Vector3(2f, 3f, 4f), moved.Center);
		OrientedBox b = B;
		b.Translate(new Vector3(1f, 1f, 1f));
		Assert.AreEqual(moved, b);

		Matrix3 r = Matrix3.Rotation(Axis.X, 0.6f);
		AffineTransform at = new AffineTransform(r, new Vector3(-2f, 0f, 5f));
		OrientedBox byMatrix = OrientedBox.Transform(B, r);
		OrientedBox byAffine = OrientedBox.Transform(B, at);
		Vector3[] vertices = B.GetVertices().ToArray();
		Vector3[] vm = byMatrix.GetVertices().ToArray();
		Vector3[] va = byAffine.GetVertices().ToArray();
		for (int i = 0; i < vertices.Length; i++)
		{
			AssertEx.AreEqual(vertices[i]*r, vm[i], 1e-5f);
			AssertEx.AreEqual(Vector3.Transform(vertices[i], at), va[i], 1e-5f);
		}
		b = B;
		b.Transform(r, true);
		Assert.IsTrue(byMatrix.ApproxEquals(b, 1e-6f));
		b = B;
		b.Transform(at);
		Assert.IsTrue(byAffine.ApproxEquals(b, 1e-6f));

		OrientedBox scaled = OrientedBox.Transform(B, Matrix3.Scaling(2f));
		AssertEx.AreEqual(2f*B.HalfDimensions, scaled.HalfDimensions);
		AssertEx.AreEqual(2f*B.Center, scaled.Center);
	}

	[TestMethod]
	public void ClosestPoint_Distance_Contains()
	{
		OrientedBox box = new OrientedBox(Vector3.Zero, Rot45Z, new Vector3(1f, 1f, 1f));
		float d = MathF.Sqrt(2f);
		Assert.IsTrue(box.Contains(Vector3.Zero));
		Assert.IsTrue(box.Contains(new Vector3(1.4f, 0f, 0f)));
		Assert.IsFalse(box.Contains(new Vector3(1f, 1f, 0f)));
		Assert.IsFalse(box.Contains(new Vector3(0f, 0f, 1.1f)));
		AssertEx.AreEqual(new Vector3(d, 0f, 0f), box.GetClosestPoint(new Vector3(5f, 0f, 0f)));
		AssertEx.AreEqual(5f - d, box.GetDistanceTo(new Vector3(5f, 0f, 0f)));
		float h = MathF.Sqrt(0.5f);
		AssertEx.AreEqual(new Vector3(h, h, 1f), box.GetClosestPoint(new Vector3(2f, 2f, 3f)));
		AssertEx.AreEqual(Vector3.Distance(new Vector3(2f, 2f, 3f), new Vector3(h, h, 1f)), box.GetDistanceTo(new Vector3(2f, 2f, 3f)));
		AssertEx.AreEqual(0f, box.GetDistanceTo(new Vector3(0.1f, 0.2f, 0.3f)));
	}

	[TestMethod]
	public void Intersects_Planar()
	{
		OrientedBox box = new OrientedBox(Vector3.Zero, Rot45Z, new Vector3(1f, 1f, 1f));
		Assert.IsTrue(box.Intersects(new Plane(Vector3.UnitX, new Vector3(1.4f, 0f, 0f))));
		Assert.IsFalse(box.Intersects(new Plane(Vector3.UnitX, new Vector3(1.5f, 0f, 0f))));
		Assert.IsTrue(box.Intersects(new HalfSpace(-Vector3.UnitX, new Vector3(1.4f, 0f, 0f))));
		Assert.IsFalse(box.Intersects(new HalfSpace(-Vector3.UnitX, new Vector3(1.5f, 0f, 0f))));
		Assert.IsTrue(box.Intersects(new HalfSpace(Vector3.UnitX, new Vector3(1.5f, 0f, 0f))));
		Assert.IsTrue(box.Intersects(new Triangle3(new Vector3(1.3f, -5f, -5f), new Vector3(1.3f, 5f, -5f), new Vector3(1.3f, 0f, 5f))));
		Assert.IsFalse(box.Intersects(new Triangle3(new Vector3(1.5f, -5f, -5f), new Vector3(1.5f, 5f, -5f), new Vector3(1.5f, 0f, 5f))));
	}

	[TestMethod]
	public void Intersects_Volumes()
	{
		OrientedBox box = new OrientedBox(Vector3.Zero, Rot45Z, new Vector3(1f, 1f, 1f));
		Assert.IsTrue(box.Intersects(new AxisAlignedBox(new Vector3(1.3f, -0.1f, -0.1f), new Vector3(2f, 0.1f, 0.1f))));
		Assert.IsFalse(box.Intersects(new AxisAlignedBox(new Vector3(0.8f, 0.8f, -1f), new Vector3(2f, 2f, 1f))));
		Assert.IsTrue(box.Intersects(new OrientedBox(new Vector3(2.5f, 0f, 0f), Rot45Z, new Vector3(1f))));
		Assert.IsTrue(box.Intersects(new OrientedBox(new Vector3(2.8f, 0f, 0f), Rot45Z, new Vector3(1f))));
		Assert.IsFalse(box.Intersects(new OrientedBox(new Vector3(2.9f, 0f, 0f), Rot45Z, new Vector3(1f))));
		Assert.IsTrue(box.Intersects(new Sphere(new Vector3(2f, 0f, 0f), 0.6f)));
		Assert.IsFalse(box.Intersects(new Sphere(new Vector3(2f, 0f, 0f), 0.55f)));
		// (1, 1, 0) is (2 - sqrt(2))/sqrt(2) ~ 0.414 away from the face x + y = sqrt(2).
		Assert.IsTrue(box.Intersects(new Sphere(new Vector3(1f, 1f, 0f), 0.42f)));
		Assert.IsFalse(box.Intersects(new Sphere(new Vector3(1f, 1f, 0f), 0.4f)));
	}

	[TestMethod]
	public void IntersectsOrientedBox_AgreesWithReferenceSeparatingAxisTest()
	{
		var rng = new MersenneTwister(1234);
		int checkedPairs = 0, intersecting = 0;
		for (int i = 0; i < 2000; i++)
		{
			OrientedBox a = SeparatingAxis.RandomBox(rng, 0f);
			OrientedBox b = SeparatingAxis.RandomBox(rng, 3f);
			float gap = SeparatingAxis.MaxGap(a.GetVertices().ToArray(), b.GetVertices().ToArray(), SeparatingAxis.BoxBoxAxes(a.Basis, b.Basis));
			if (Math.Abs(gap) < 1e-3f)
				continue; // too close to touching to decide reliably in single precision
			bool expected = gap < 0f;
			Assert.AreEqual(expected, a.Intersects(b), $"Pair {i}: gap {gap}");
			Assert.AreEqual(expected, b.Intersects(a), $"Pair {i} (swapped): gap {gap}");
			checkedPairs++;
			if (expected)
				intersecting++;
		}
		Assert.IsGreaterThan(1500, checkedPairs);
		Assert.IsGreaterThan(200, intersecting);
		Assert.IsLessThan(checkedPairs - 200, intersecting);
	}

	[TestMethod]
	public void IntersectsAxisAlignedBox_AgreesWithReferenceSeparatingAxisTest()
	{
		var rng = new MersenneTwister(4321);
		int checkedPairs = 0;
		for (int i = 0; i < 1000; i++)
		{
			OrientedBox a = SeparatingAxis.RandomBox(rng, 0f);
			OrientedBox r = SeparatingAxis.RandomBox(rng, 3f);
			AxisAlignedBox b = new AxisAlignedBox(r.Center - r.HalfDimensions, r.Center + r.HalfDimensions);
			float gap = SeparatingAxis.MaxGap(a.GetVertices().ToArray(), b.GetVertices().ToArray(), SeparatingAxis.BoxBoxAxes(a.Basis, Matrix3.Identity));
			if (Math.Abs(gap) < 1e-3f)
				continue;
			Assert.AreEqual(gap < 0f, a.Intersects(b), $"Pair {i}: gap {gap}");
			Assert.AreEqual(gap < 0f, b.Intersects(a), $"Pair {i} (swapped): gap {gap}");
			checkedPairs++;
		}
		Assert.IsGreaterThan(700, checkedPairs);
	}

	[TestMethod]
	public void IntersectsTriangle_AgreesWithReferenceSeparatingAxisTest()
	{
		var rng = new MersenneTwister(99);
		int checkedPairs = 0, intersecting = 0;
		for (int i = 0; i < 2000; i++)
		{
			OrientedBox box = SeparatingAxis.RandomBox(rng, 0f);
			Vector3[] tri = { SeparatingAxis.RandomPoint(rng, 3f), SeparatingAxis.RandomPoint(rng, 3f), SeparatingAxis.RandomPoint(rng, 3f) };
			Triangle3 triangle = new Triangle3(tri[0], tri[1], tri[2]);
			if (triangle.Area < 0.1f)
				continue;
			float gap = SeparatingAxis.MaxGap(box.GetVertices().ToArray(), tri, SeparatingAxis.BoxTriangleAxes(box.Basis, tri));
			if (Math.Abs(gap) < 1e-3f)
				continue;
			bool expected = gap < 0f;
			Assert.AreEqual(expected, box.Intersects(triangle), $"OBB case {i}: gap {gap}");
			Assert.AreEqual(expected, triangle.Intersects(box), $"OBB case {i} (triangle): gap {gap}");

			AxisAlignedBox aab = new AxisAlignedBox(box.Center - box.HalfDimensions, box.Center + box.HalfDimensions);
			float aabGap = SeparatingAxis.MaxGap(aab.GetVertices().ToArray(), tri, SeparatingAxis.BoxTriangleAxes(Matrix3.Identity, tri));
			if (Math.Abs(aabGap) >= 1e-3f)
			{
				Assert.AreEqual(aabGap < 0f, aab.Intersects(triangle), $"AABB case {i}: gap {aabGap}");
				Assert.AreEqual(aabGap < 0f, triangle.Intersects(aab), $"AABB case {i} (triangle): gap {aabGap}");
			}
			checkedPairs++;
			if (expected)
				intersecting++;
		}
		Assert.IsGreaterThan(1000, checkedPairs);
		Assert.IsGreaterThan(100, intersecting);
	}
}

/// <summary>
/// Brute-force separating axis test used as a reference implementation.
/// </summary>
internal static class SeparatingAxis
{
	public static float Next(MersenneTwister rng, float min, float max) => min + (max - min)*rng.GetSingle01();

	public static Vector3 RandomPoint(MersenneTwister rng, float extent) =>
		new Vector3(Next(rng, -extent, extent), Next(rng, -extent, extent), Next(rng, -extent, extent));

	public static OrientedBox RandomBox(MersenneTwister rng, float centerExtent)
	{
		Vector3 axis = Vector3.Normalize(RandomPoint(rng, 1f) + new Vector3(1e-3f));
		Matrix3 basis = Matrix3.Rotation(axis, Next(rng, -SingleConstants.Pi, SingleConstants.Pi));
		Vector3 halfDims = new Vector3(Next(rng, 0.2f, 1.5f), Next(rng, 0.2f, 1.5f), Next(rng, 0.2f, 1.5f));
		return new OrientedBox(RandomPoint(rng, centerExtent), basis, halfDims);
	}

	public static IEnumerable<Vector3> BoxBoxAxes(Matrix3 a, Matrix3 b)
	{
		for (int i = 0; i < 3; i++)
		{
			yield return a[i];
			yield return b[i];
			for (int j = 0; j < 3; j++)
				yield return Vector3.Cross(a[i], b[j]);
		}
	}

	public static IEnumerable<Vector3> BoxTriangleAxes(Matrix3 box, Vector3[] tri)
	{
		Vector3[] edges = { tri[1] - tri[0], tri[2] - tri[1], tri[0] - tri[2] };
		yield return Vector3.Cross(edges[0], edges[1]);
		for (int i = 0; i < 3; i++)
		{
			yield return box[i];
			foreach (Vector3 e in edges)
				yield return Vector3.Cross(box[i], e);
		}
	}

	/// <summary>
	/// Returns the largest normalized gap between the projections of two point sets over the axes.
	/// Positive means some axis separates the sets; negative means they overlap on every axis.
	/// </summary>
	public static float MaxGap(Vector3[] a, Vector3[] b, IEnumerable<Vector3> axes)
	{
		float maxGap = Single.NegativeInfinity;
		foreach (Vector3 rawAxis in axes)
		{
			if (rawAxis.MagnitudeSquared < 1e-8f)
				continue;
			Vector3 axis = Vector3.Normalize(rawAxis);
			float aMin = a.Min(p => Vector3.Dot(p, axis)), aMax = a.Max(p => Vector3.Dot(p, axis));
			float bMin = b.Min(p => Vector3.Dot(p, axis)), bMax = b.Max(p => Vector3.Dot(p, axis));
			maxGap = Math.Max(maxGap, Math.Max(bMin - aMax, aMin - bMax));
		}
		return maxGap;
	}
}
