namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class Triangle3Tests
{
	// Right triangle in the z = 0 plane, counter-clockwise when viewed from +Z.
	private static readonly Triangle3 T = new Triangle3(Vector3.Zero, new Vector3(4f, 0f, 0f), new Vector3(0f, 4f, 0f));

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(Vector3.Zero, T.Vertex0);
		Assert.AreEqual(new Vector3(4f, 0f, 0f), T.Vertex1);
		Assert.AreEqual(new Vector3(0f, 4f, 0f), T.Vertex2);
		Assert.AreEqual(T, new Triangle3(new[] { Vector3.Zero, new Vector3(4f, 0f, 0f), new Vector3(0f, 4f, 0f) }));
		CollectionAssert.AreEqual(new[] { T.Vertex0, T.Vertex1, T.Vertex2 }, T.GetVertices().ToArray());
		AssertEx.AreEqual(8f + 4f*MathF.Sqrt(2f), T.Perimeter);
		AssertEx.AreEqual(8f, T.Area);
		Assert.AreEqual(Vector3.UnitZ, T.Normal);
		Assert.AreEqual(Vector3.UnitZ, Triangle3.ComputeNormal(T.Vertex0, T.Vertex1, T.Vertex2));
		Assert.IsTrue(T.IsFinite);
		Assert.IsFalse(new Triangle3(Vector3.Zero, Vector3.Zero, new Vector3(Single.NaN)).IsFinite);

		Triangle3 t = T;
		t.Vertex0 = Vector3.UnitX;
		t.Vertex1 = Vector3.UnitY;
		t.Vertex2 = Vector3.UnitZ;
		Assert.AreEqual(new Triangle3(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ), t);
	}

	[TestMethod]
	public void Area_IsTranslationInvariant()
	{
		Triangle3 t = new Triangle3(new Vector3(1f, 2f, 3f), new Vector3(4f, 6f, 3f), new Vector3(1f, 2f, 8f));
		AssertEx.AreEqual(12.5f, t.Area, 1e-4f);
		AssertEx.AreEqual(12.5f, Triangle3.Translate(t, new Vector3(100f, -50f, 20f)).Area, 1e-3f);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Triangle3 same = new Triangle3(Vector3.Zero, new Vector3(4f, 0f, 0f), new Vector3(0f, 4f, 0f));
		Assert.IsTrue(T == same);
		Assert.IsFalse(T != same);
		Assert.IsTrue(T.Equals((object)same));
		Assert.IsFalse(T.Equals((object)Triangle3.Flip(T)));
		Assert.AreEqual(T.GetHashCode(), same.GetHashCode());
		Triangle3 moved = Triangle3.Translate(T, new Vector3(0f, 0f, 0.01f));
		Assert.IsFalse(T.ApproxEquals(moved, 1e-3f));
		Assert.IsTrue(T.ApproxEquals(moved, 0.1f));
	}

	[TestMethod]
	public void CircumscribedBox_And_Sphere()
	{
		Assert.AreEqual(new AxisAlignedBox(Vector3.Zero, new Vector3(4f, 4f, 0f)), T.GetCircumscribedBox());
		Sphere s = T.GetCircumscribedSphere();
		AssertEx.AreEqual(new Vector3(2f, 2f, 0f), s.Center);
		AssertEx.AreEqual(2f*MathF.Sqrt(2f), s.Radius);

		Triangle3 general = new Triangle3(new Vector3(1f, 2f, 3f), new Vector3(-2f, 0.5f, 4f), new Vector3(0f, -1f, 1f));
		Sphere g = general.GetCircumscribedSphere();
		foreach (Vector3 v in general.GetVertices())
			AssertEx.AreEqual(g.Radius, Vector3.Distance(v, g.Center), 1e-4f);
		AssertEx.AreEqual(0f, Plane.FromTriangle(general).GetSignedDistanceTo(g.Center), 1e-4f);
	}

	[TestMethod]
	public void TangentSpaceBasis()
	{
		Matrix3 basis = T.GetTangentSpaceBasis(Vector2.Zero, Vector2.UnitX, Vector2.UnitY, false);
		AssertEx.AreEqual(Matrix3.Identity, basis);
		Matrix3 weighted = T.GetTangentSpaceBasis(Vector2.Zero, Vector2.UnitX, Vector2.UnitY, true);
		AssertEx.AreEqual(new Matrix3(0.5f, 0f, 0f, 0f, 0.5f, 0f, 0f, 0f, 1f), weighted);
		// Mirrored V coordinate flips the bitangent.
		Matrix3 flipped = T.GetTangentSpaceBasis(Vector2.Zero, Vector2.UnitX, -Vector2.UnitY, false);
		AssertEx.AreEqual(-Vector3.UnitY, flipped.Row1);
		// Degenerate UVs produce zero tangent and bitangent.
		Matrix3 degenerate = T.GetTangentSpaceBasis(Vector2.Zero, Vector2.Zero, Vector2.Zero, false);
		Assert.AreEqual(Vector3.Zero, degenerate.Row0);
		Assert.AreEqual(Vector3.Zero, degenerate.Row1);
		Assert.AreEqual(Vector3.UnitZ, degenerate.Row2);

		Assert.AreEqual(basis, Triangle3.ComputeTangentSpaceBasis(T.Vertex0, T.Vertex1, T.Vertex2, Vector2.Zero, Vector2.UnitX, Vector2.UnitY, false));
		Assert.AreEqual(weighted, Triangle3.ComputeTangentSpaceBasis(T.Vertex0, T.Vertex1, T.Vertex2, Vector2.Zero, Vector2.UnitX, Vector2.UnitY, true));
	}

	[TestMethod]
	public void Translate_Transform_Flip()
	{
		Vector3 offset = new Vector3(1f, 2f, 3f);
		Triangle3 moved = Triangle3.Translate(T, offset);
		Assert.AreEqual(new Triangle3(offset, new Vector3(5f, 2f, 3f), new Vector3(1f, 6f, 3f)), moved);
		Triangle3 t = T;
		t.Translate(offset);
		Assert.AreEqual(moved, t);

		Matrix3 r = Matrix3.Rotation(Axis.X, SingleConstants.HalfPi);
		Triangle3 rotated = Triangle3.Transform(T, r);
		AssertEx.AreEqual(new Vector3(0f, 0f, 4f), rotated.Vertex2);
		AssertEx.AreEqual(-Vector3.UnitY, rotated.Normal);
		t = T;
		t.Transform(r);
		Assert.AreEqual(rotated, t);
		AffineTransform at = new AffineTransform(r, offset);
		Triangle3 transformed = Triangle3.Transform(T, at);
		AssertEx.AreEqual(new Vector3(1f, 2f, 7f), transformed.Vertex2);
		t = T;
		t.Transform(at);
		Assert.AreEqual(transformed, t);

		Triangle3 flipped = Triangle3.Flip(T);
		Assert.AreEqual(new Triangle3(T.Vertex2, T.Vertex1, T.Vertex0), flipped);
		Assert.AreEqual(-Vector3.UnitZ, flipped.Normal);
		t = T;
		t.Flip();
		Assert.AreEqual(flipped, t);
	}

	[TestMethod]
	public void Evaluate_And_BarycentricCoords()
	{
		Assert.AreEqual(T.Vertex0, T.Evaluate(0f, 0f));
		Assert.AreEqual(T.Vertex1, T.Evaluate(1f, 0f));
		Assert.AreEqual(T.Vertex2, T.Evaluate(0f, 1f));
		Assert.AreEqual(new Vector3(1f, 2f, 0f), T.Evaluate(0.25f, 0.5f));
		Assert.AreEqual(new Vector3(1f, 2f, 0f), T.Evaluate(0.25f, 0.25f, 0.5f));

		Triangle3 general = new Triangle3(new Vector3(1f, 2f, 3f), new Vector3(-2f, 0.5f, 4f), new Vector3(0f, -1f, 1f));
		Vector3[] coords = { new Vector3(1f, 0f, 0f), new Vector3(0.2f, 0.3f, 0.5f), new Vector3(-0.5f, 1f, 0.5f) };
		foreach (Vector3 c in coords)
			AssertEx.AreEqual(c, general.GetBarycentricCoords(general.Evaluate(c.X, c.Y, c.Z)), 1e-5f);
		// Points off the plane are projected.
		AssertEx.AreEqual(new Vector3(0.5f, 0.25f, 0.25f), T.GetBarycentricCoords(new Vector3(1f, 1f, 7f)));
	}

	[TestMethod]
	public void ClosestPoint_And_Distance_CoverAllRegions()
	{
		(Vector3 point, Vector3 closest)[] cases =
		{
			(new Vector3(1f, 1f, 5f), new Vector3(1f, 1f, 0f)),    // face
			(new Vector3(-1f, -1f, 0f), Vector3.Zero),              // vertex 0
			(new Vector3(5f, -1f, 2f), new Vector3(4f, 0f, 0f)),    // vertex 1
			(new Vector3(-1f, 6f, 0f), new Vector3(0f, 4f, 0f)),    // vertex 2
			(new Vector3(2f, -3f, 0f), new Vector3(2f, 0f, 0f)),    // edge 0-1
			(new Vector3(3f, 3f, 0f), new Vector3(2f, 2f, 0f)),     // edge 1-2
			(new Vector3(-2f, 2f, 1f), new Vector3(0f, 2f, 0f)),    // edge 2-0
			(new Vector3(2f, 1f, 0f), new Vector3(2f, 1f, 0f)),     // inside
		};
		foreach (var (point, closest) in cases)
		{
			AssertEx.AreEqual(closest, T.GetClosestPoint(point), 1e-5f, point.ToString());
			AssertEx.AreEqual(Vector3.Distance(point, closest), T.GetDistanceTo(point), 1e-5f, point.ToString());
		}
	}

	[TestMethod]
	public void Intersects_HalfSpace_And_Plane()
	{
		Assert.IsTrue(T.Intersects(new HalfSpace(Vector3.UnitX, new Vector3(1f, 0f, 0f))));
		Assert.IsTrue(T.Intersects(new HalfSpace(-Vector3.UnitX, new Vector3(1f, 0f, 0f))));
		Assert.IsFalse(T.Intersects(new HalfSpace(-Vector3.UnitX, new Vector3(4.5f, 0f, 0f))));
		Assert.IsTrue(T.Intersects(new Plane(Vector3.UnitX, new Vector3(1f, 0f, 0f))));
		Assert.IsTrue(T.Intersects(new Plane(Vector3.UnitZ, Vector3.Zero)));
		Assert.IsTrue(T.Intersects(new Plane(Vector3.UnitX, new Vector3(4f, 0f, 0f))));
		Assert.IsFalse(T.Intersects(new Plane(Vector3.UnitX, new Vector3(4.5f, 0f, 0f))));
		Assert.IsFalse(T.Intersects(new Plane(-Vector3.UnitZ, new Vector3(0f, 0f, 1f))));
	}

	[TestMethod]
	public void Intersects_Volumes()
	{
		Assert.IsTrue(T.Intersects(new AxisAlignedBox(new Vector3(0.5f, 0.5f, -1f), new Vector3(1f, 1f, 1f))));
		Assert.IsFalse(T.Intersects(new AxisAlignedBox(new Vector3(2.5f, 2.5f, -1f), new Vector3(3f, 3f, 1f))));
		Assert.IsTrue(T.Intersects(new OrientedBox(new Vector3(2.5f, 2.5f, 0f), Matrix3.Rotation(Axis.Z, SingleConstants.Pi/4f), new Vector3(0.8f, 0.1f, 1f))));
		Assert.IsFalse(T.Intersects(new OrientedBox(new Vector3(2.5f, 2.5f, 0f), Matrix3.Rotation(Axis.Z, SingleConstants.Pi/4f), new Vector3(0.1f, 0.8f, 1f))));
		Assert.IsTrue(T.Intersects(new Sphere(new Vector3(1f, 1f, 1f), 1f)));
		Assert.IsFalse(T.Intersects(new Sphere(new Vector3(1f, 1f, 1.1f), 1f)));
		Assert.IsTrue(T.Intersects(new Sphere(new Vector3(3f, 3f, 0f), 1.5f)));
		Assert.IsFalse(T.Intersects(new Sphere(new Vector3(3f, 3f, 0f), 1.4f)));
	}
}

[TestClass]
public sealed class Quadrilateral3Tests
{
	// Unit square in the z = 0 plane scaled by 2, counter-clockwise from +Z.
	private static readonly Quadrilateral3 Q = new Quadrilateral3(Vector3.Zero, new Vector3(2f, 0f, 0f), new Vector3(2f, 2f, 0f), new Vector3(0f, 2f, 0f));

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(Vector3.Zero, Q.Vertex0);
		Assert.AreEqual(new Vector3(2f, 0f, 0f), Q.Vertex1);
		Assert.AreEqual(new Vector3(2f, 2f, 0f), Q.Vertex2);
		Assert.AreEqual(new Vector3(0f, 2f, 0f), Q.Vertex3);
		Assert.AreEqual(Q, new Quadrilateral3(Q.GetVertices().ToArray()));
		Assert.AreEqual(8f, Q.Perimeter);
		Assert.AreEqual(4f, Q.Area);
		Assert.AreEqual(Vector3.UnitZ, Q.Normal);
		Assert.AreEqual(Vector3.UnitZ, Quadrilateral3.ComputeNormal(Q.Vertex0, Q.Vertex1, Q.Vertex2, Q.Vertex3));
		Assert.AreEqual(new AxisAlignedBox(Vector3.Zero, new Vector3(2f, 2f, 0f)), Q.GetCircumscribedBox());
		Assert.IsTrue(Q.IsFinite);
		Assert.IsFalse(new Quadrilateral3(Vector3.Zero, Vector3.Zero, Vector3.Zero, new Vector3(Single.NaN)).IsFinite);

		Quadrilateral3 q = Q;
		q.Vertex0 = Vector3.UnitX;
		q.Vertex1 = Vector3.UnitY;
		q.Vertex2 = Vector3.UnitZ;
		q.Vertex3 = Vector3.Zero;
		Assert.AreEqual(new Quadrilateral3(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ, Vector3.Zero), q);
	}

	[TestMethod]
	public void Area_OfGeneralPlanarQuadrilateral()
	{
		// Trapezoid with parallel sides 4 and 2, height 3 -> area 9.
		Quadrilateral3 trapezoid = new Quadrilateral3(Vector3.Zero, new Vector3(4f, 0f, 0f), new Vector3(3f, 3f, 0f), new Vector3(1f, 3f, 0f));
		AssertEx.AreEqual(9f, trapezoid.Area);
		AssertEx.AreEqual(Vector3.UnitZ, trapezoid.Normal);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Quadrilateral3 same = new Quadrilateral3(Vector3.Zero, new Vector3(2f, 0f, 0f), new Vector3(2f, 2f, 0f), new Vector3(0f, 2f, 0f));
		Assert.IsTrue(Q == same);
		Assert.IsFalse(Q != same);
		Assert.IsTrue(Q.Equals((object)same));
		Assert.IsFalse(Q.Equals((object)Quadrilateral3.Flip(Q)));
		Assert.AreEqual(Q.GetHashCode(), same.GetHashCode());
		Quadrilateral3 moved = Quadrilateral3.Translate(Q, new Vector3(0f, 0f, 0.01f));
		Assert.IsFalse(Q.ApproxEquals(moved, 1e-3f));
		Assert.IsTrue(Q.ApproxEquals(moved, 0.1f));
	}

	[TestMethod]
	public void Translate_Transform_Flip()
	{
		Vector3 offset = new Vector3(1f, 2f, 3f);
		Quadrilateral3 moved = Quadrilateral3.Translate(Q, offset);
		Assert.AreEqual(Q.Vertex2 + offset, moved.Vertex2);
		Quadrilateral3 q = Q;
		q.Translate(offset);
		Assert.AreEqual(moved, q);

		Matrix3 r = Matrix3.Rotation(Axis.X, SingleConstants.HalfPi);
		Quadrilateral3 rotated = Quadrilateral3.Transform(Q, r);
		AssertEx.AreEqual(-Vector3.UnitY, rotated.Normal);
		q = Q;
		q.Transform(r);
		Assert.AreEqual(rotated, q);
		AffineTransform at = new AffineTransform(r, offset);
		Quadrilateral3 transformed = Quadrilateral3.Transform(Q, at);
		AssertEx.AreEqual(Vector3.Transform(Q.Vertex3, at), transformed.Vertex3);
		q = Q;
		q.Transform(at);
		Assert.AreEqual(transformed, q);

		Quadrilateral3 flipped = Quadrilateral3.Flip(Q);
		Assert.AreEqual(new Quadrilateral3(Q.Vertex3, Q.Vertex2, Q.Vertex1, Q.Vertex0), flipped);
		Assert.AreEqual(-Vector3.UnitZ, flipped.Normal);
		q = Q;
		q.Flip();
		Assert.AreEqual(flipped, q);
	}
}

[TestClass]
public sealed class TetrahedronTests
{
	private static readonly Tetrahedron T = new Tetrahedron(Vector3.Zero, new Vector3(3f, 0f, 0f), new Vector3(0f, 3f, 0f), new Vector3(0f, 0f, 3f));

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(Vector3.Zero, T.Vertex0);
		Assert.AreEqual(new Vector3(3f, 0f, 0f), T.Vertex1);
		Assert.AreEqual(new Vector3(0f, 3f, 0f), T.Vertex2);
		Assert.AreEqual(new Vector3(0f, 0f, 3f), T.Vertex3);
		Assert.AreEqual(T, new Tetrahedron(T.GetVertices().ToArray()));
		Assert.AreEqual(new Vector3(0.75f), T.Centroid);
		Assert.AreEqual(4.5f, T.Volume);
		Assert.AreEqual(new AxisAlignedBox(Vector3.Zero, new Vector3(3f)), T.GetCircumscribedBox());
		Assert.IsTrue(T.IsFinite);
		Assert.IsFalse(new Tetrahedron(Vector3.Zero, Vector3.Zero, Vector3.Zero, new Vector3(Single.NaN)).IsFinite);

		Tetrahedron t = T;
		t.Vertex0 = Vector3.UnitX;
		t.Vertex1 = Vector3.UnitY;
		t.Vertex2 = Vector3.UnitZ;
		t.Vertex3 = Vector3.Zero;
		Assert.AreEqual(new Tetrahedron(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ, Vector3.Zero), t);
	}

	[TestMethod]
	public void Volume_IsOrientationIndependent()
	{
		Assert.AreEqual(T.Volume, Tetrahedron.Flip(T).Volume);
		Tetrahedron general = new Tetrahedron(new Vector3(1f, 1f, 1f), new Vector3(3f, 1f, 1f), new Vector3(1f, 4f, 1f), new Vector3(1f, 1f, 5f));
		AssertEx.AreEqual(4f, general.Volume);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Tetrahedron same = new Tetrahedron(Vector3.Zero, new Vector3(3f, 0f, 0f), new Vector3(0f, 3f, 0f), new Vector3(0f, 0f, 3f));
		Assert.IsTrue(T == same);
		Assert.IsFalse(T != same);
		Assert.IsTrue(T.Equals((object)same));
		Assert.IsFalse(T.Equals((object)Tetrahedron.Flip(T)));
		Assert.AreEqual(T.GetHashCode(), same.GetHashCode());
		Tetrahedron moved = Tetrahedron.Translate(T, new Vector3(0.01f, 0f, 0f));
		Assert.IsFalse(T.ApproxEquals(moved, 1e-3f));
		Assert.IsTrue(T.ApproxEquals(moved, 0.1f));
	}

	[TestMethod]
	public void Translate_Transform_Flip()
	{
		Vector3 offset = new Vector3(1f, 2f, 3f);
		Tetrahedron moved = Tetrahedron.Translate(T, offset);
		Assert.AreEqual(T.Centroid + offset, moved.Centroid);
		Tetrahedron t = T;
		t.Translate(offset);
		Assert.AreEqual(moved, t);

		Matrix3 s = Matrix3.Scaling(new Vector3(2f, 1f, 1f));
		Tetrahedron scaled = Tetrahedron.Transform(T, s);
		AssertEx.AreEqual(9f, scaled.Volume);
		t = T;
		t.Transform(s);
		Assert.AreEqual(scaled, t);
		AffineTransform at = new AffineTransform(Matrix3.Rotation(Axis.Y, 0.7f), offset);
		Tetrahedron transformed = Tetrahedron.Transform(T, at);
		AssertEx.AreEqual(T.Volume, transformed.Volume, 1e-4f);
		AssertEx.AreEqual(Vector3.Transform(T.Centroid, at), transformed.Centroid);
		t = T;
		t.Transform(at);
		Assert.AreEqual(transformed, t);

		Tetrahedron flipped = Tetrahedron.Flip(T);
		Assert.AreEqual(new Tetrahedron(T.Vertex1, T.Vertex0, T.Vertex2, T.Vertex3), flipped);
		t = T;
		t.Flip();
		Assert.AreEqual(flipped, t);
	}
}
