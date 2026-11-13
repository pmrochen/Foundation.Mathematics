namespace Foundation.Mathematics.Tests;

[TestClass]
public sealed class CylinderTests
{
	// Vertical cylinder from y = 0 to y = 4 with radius 1.
	private static readonly Cylinder C = new Cylinder(new Vector3(0f, 2f, 0f), Vector3.UnitY, 4f, 1f);

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(new Vector3(0f, 2f, 0f), C.Center);
		Assert.AreEqual(Vector3.UnitY, C.Axis);
		Assert.AreEqual(4f, C.Height);
		Assert.AreEqual(2f, C.HalfHeight);
		Assert.AreEqual(1f, C.Radius);
		AssertEx.AreEqual(10f*MathF.PI, C.SurfaceArea);
		AssertEx.AreEqual(4f*MathF.PI, C.Volume);
		Assert.IsTrue(C.IsFinite);
		Assert.IsFalse(new Cylinder(Vector3.Zero, Vector3.UnitY, Single.NaN, 1f).IsFinite);
		Assert.AreEqual(C, new Cylinder(new Vector3(0f, 2f, 0f), Axis.Y, 4f, 1f));
		Cylinder fromPoints = new Cylinder(Vector3.Zero, new Vector3(0f, 4f, 0f), 1f);
		Assert.IsTrue(C.ApproxEquals(fromPoints));

		Cylinder c = C;
		c.Center = Vector3.Zero;
		c.Axis = Vector3.UnitX;
		c.Height = 2f;
		c.Radius = 0.5f;
		Assert.AreEqual(new Cylinder(Vector3.Zero, Vector3.UnitX, 2f, 0.5f), c);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		Cylinder same = new Cylinder(new Vector3(0f, 2f, 0f), Vector3.UnitY, 4f, 1f);
		Assert.IsTrue(C == same);
		Assert.IsFalse(C != same);
		Assert.IsTrue(C.Equals((object)same));
		Assert.IsFalse(C.Equals((object)new Capsule(C.Center, C.Axis, C.Height, C.Radius)));
		Assert.AreEqual(C.GetHashCode(), same.GetHashCode());
		Cylinder wider = new Cylinder(C.Center, C.Axis, C.Height, 1.01f);
		Assert.IsTrue(C != wider);
		Assert.IsFalse(C.ApproxEquals(wider));
		Assert.IsTrue(C.ApproxEquals(wider, 0.1f));
	}

	[TestMethod]
	public void Normalize_MovesAxisLengthIntoHeight()
	{
		Cylinder c = new Cylinder(Vector3.Zero, new Vector3(0f, 2f, 0f), 3f, 1f);
		Cylinder n = Cylinder.Normalize(c);
		Assert.AreEqual(Vector3.UnitY, n.Axis);
		Assert.AreEqual(6f, n.Height);
		c.Normalize();
		Assert.AreEqual(n, c);
	}

	[TestMethod]
	public void Translate()
	{
		Assert.AreEqual(new Vector3(1f, 3f, 1f), Cylinder.Translate(C, new Vector3(1f)).Center);
		Cylinder c = C;
		c.Translate(new Vector3(1f));
		Assert.AreEqual(new Vector3(1f, 3f, 1f), c.Center);
	}

	[TestMethod]
	public void Contains()
	{
		Assert.IsTrue(C.Contains(C.Center));
		Assert.IsTrue(C.Contains(new Vector3(0.9f, 0.1f, 0f)));
		Assert.IsTrue(C.Contains(new Vector3(0f, 4f, 1f)));
		Assert.IsFalse(C.Contains(new Vector3(0.8f, 2f, 0.8f)));
		Assert.IsFalse(C.Contains(new Vector3(0f, -0.1f, 0f)));
		Assert.IsFalse(C.Contains(new Vector3(0f, 4.1f, 0f)));

		Cylinder tilted = new Cylinder(Vector3.Zero, new Vector3(1f, 0f, 1f), 1f);
		Assert.IsTrue(tilted.Contains(new Vector3(0.5f, 0f, 0.5f)));
		Assert.IsTrue(tilted.Contains(new Vector3(0.5f, 0.9f, 0.5f)));
		Assert.IsFalse(tilted.Contains(new Vector3(1.1f, 0f, 1.1f)));
		Assert.IsFalse(tilted.Contains(new Vector3(0.5f, 1.1f, 0.5f)));
	}

	[TestMethod]
	public void CircumscribedBox_ContainsSurfaceAndIsTight()
	{
		Cylinder tilted = new Cylinder(new Vector3(1f, -1f, 2f), Vector3.Normalize(new Vector3(1f, 2f, -1f)), 3f, 0.5f);
		foreach (Cylinder c in new[] { C, tilted })
		{
			OrientedBox box = c.GetCircumscribedBox();
			AssertEx.IsOrthonormal(box.Basis);
			Assert.AreEqual(c.Center, box.Center);
			AssertEx.AreEqual(Math.Abs(Vector3.Dot(box.Basis.Row1, c.Axis)), 1f);
			AssertEx.AreEqual(new Vector3(c.Radius, c.HalfHeight, c.Radius), box.HalfDimensions);
			Matrix3 frame = Matrix3.FromForward(c.Axis);
			for (int i = 0; i < 16; i++)
			{
				float angle = i*SingleConstants.TwoPi/16f;
				Vector3 radial = c.Radius*(MathF.Cos(angle)*frame.Right + MathF.Sin(angle)*frame.Up);
				foreach (float h in new[] { -0.999f*c.HalfHeight, 0f, 0.999f*c.HalfHeight })
				{
					Vector3 p = c.Center + h*c.Axis + 0.999f*radial;
					Assert.IsTrue(c.Contains(p));
					Assert.IsTrue(box.Contains(p));
				}
			}
		}
	}

	[TestMethod]
	public void ToString_And_Parse_RoundTrip()
	{
		Assert.AreEqual(C, Cylinder.Parse(C.ToString(System.Globalization.CultureInfo.InvariantCulture), System.Globalization.CultureInfo.InvariantCulture));
	}
}

[TestClass]
public sealed class ConeTests
{
	// Apex at origin opening along +Z; height 4, base radius 3.
	private static readonly Cone C = new Cone(Vector3.Zero, Vector3.UnitZ, 4f, 3f);

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(Vector3.Zero, C.Vertex);
		Assert.AreEqual(Vector3.UnitZ, C.Axis);
		Assert.AreEqual(4f, C.Height);
		Assert.AreEqual(2f, C.HalfHeight);
		Assert.AreEqual(3f, C.BaseRadius);
		Assert.AreEqual(new Vector3(0f, 0f, 4f), C.BaseCenter);
		AssertEx.AreEqual(MathF.Atan(0.75f), C.HalfAngle);
		AssertEx.AreEqual(2f*MathF.Atan(0.75f), C.Angle);
		AssertEx.AreEqual(SingleConstants.TwoPi*(1f - 0.8f), C.SolidAngle);
		Assert.AreEqual(5f, C.SlantHeight);
		AssertEx.AreEqual(MathF.PI*3f*(3f + 5f), C.SurfaceArea);
		AssertEx.AreEqual(12f*MathF.PI, C.Volume);
		Assert.IsTrue(C.IsFinite);
		Assert.IsFalse(new Cone(Vector3.Zero, Vector3.UnitZ, 1f, Single.PositiveInfinity).IsFinite);
		Assert.IsTrue(C.ApproxEquals(new Cone(Vector3.Zero, new Vector3(0f, 0f, 4f), 3f)));
	}

	[TestMethod]
	public void Setters()
	{
		Cone c = C;
		c.BaseCenter = new Vector3(0f, 2f, 0f);
		Assert.AreEqual(Vector3.UnitY, c.Axis);
		Assert.AreEqual(2f, c.Height);
		c.HalfAngle = SingleConstants.Pi/4f;
		AssertEx.AreEqual(2f, c.BaseRadius);
		c.Angle = SingleConstants.HalfPi/2f;
		AssertEx.AreEqual(2f*MathF.Tan(SingleConstants.Pi/8f), c.BaseRadius);
		c.Vertex = Vector3.UnitX;
		c.Axis = -Vector3.UnitX;
		c.Height = 1f;
		c.BaseRadius = 0.5f;
		Assert.AreEqual(new Cone(Vector3.UnitX, -Vector3.UnitX, 1f, 0.5f), c);
	}

	[TestMethod]
	public void Equality_Normalize_Translate()
	{
		Cone same = new Cone(Vector3.Zero, Vector3.UnitZ, 4f, 3f);
		Assert.IsTrue(C == same);
		Assert.IsFalse(C != same);
		Assert.IsTrue(C.Equals((object)same));
		Assert.AreEqual(C.GetHashCode(), same.GetHashCode());
		Assert.IsFalse(C.ApproxEquals(new Cone(Vector3.Zero, Vector3.UnitZ, 4.01f, 3f)));
		Assert.IsTrue(C.ApproxEquals(new Cone(Vector3.Zero, Vector3.UnitZ, 4.01f, 3f), 0.1f));

		Cone c = new Cone(Vector3.Zero, new Vector3(0f, 0f, 2f), 2f, 3f);
		Assert.AreEqual(C, Cone.Normalize(c));
		c.Normalize();
		Assert.AreEqual(C, c);

		Assert.AreEqual(new Vector3(1f), Cone.Translate(C, new Vector3(1f)).Vertex);
		c = C;
		c.Translate(new Vector3(1f));
		Assert.AreEqual(new Vector3(1f), c.Vertex);
	}

	[TestMethod]
	public void Contains()
	{
		Assert.IsTrue(C.Contains(new Vector3(0f, 0f, 2f)));
		Assert.IsTrue(C.Contains(new Vector3(1.4f, 0f, 2f)));
		Assert.IsFalse(C.Contains(new Vector3(1.6f, 0f, 2f)));
		Assert.IsTrue(C.Contains(new Vector3(0f, 2.9f, 4f)));
		Assert.IsFalse(C.Contains(new Vector3(0f, 0f, 4.1f)));
		Assert.IsFalse(C.Contains(new Vector3(0f, 0f, -0.1f)));
		Assert.IsTrue(C.Contains(Vector3.Zero));
	}

	[TestMethod]
	public void CircumscribedBox_And_Sphere()
	{
		OrientedBox box = C.GetCircumscribedBox();
		AssertEx.AreEqual(new Vector3(0f, 0f, 2f), box.Center);
		AssertEx.AreEqual(new Vector3(3f, 2f, 3f), box.HalfDimensions);
		AssertEx.AreEqual(1f, Math.Abs(Vector3.Dot(box.Basis.Row1, C.Axis)));
		Assert.IsTrue(box.Contains(new Vector3(2.9f, 0f, 3.9f)));
		Assert.IsTrue(box.Contains(new Vector3(0f, 0f, 0.01f)));

		// Tall cone: vertex and base rim lie on the sphere.
		Sphere s = C.GetCircumscribedSphere();
		AssertEx.AreEqual(25f/8f, s.Radius);
		AssertEx.AreEqual(new Vector3(0f, 0f, 25f/8f), s.Center);
		AssertEx.AreEqual(s.Radius, Vector3.Distance(s.Center, C.Vertex));
		AssertEx.AreEqual(s.Radius, Vector3.Distance(s.Center, new Vector3(3f, 0f, 4f)));

		// Wide cone: base circle is the great circle.
		Cone wide = new Cone(Vector3.Zero, Vector3.UnitZ, 1f, 3f);
		Sphere ws = wide.GetCircumscribedSphere();
		Assert.AreEqual(new Sphere(new Vector3(0f, 0f, 1f), 3f), ws);
	}

	[TestMethod]
	public void Intersects_Sphere()
	{
		Assert.IsTrue(C.Intersects(new Sphere(new Vector3(0f, 0f, 2f), 0.5f)));
		Assert.IsTrue(C.Intersects(new Sphere(new Vector3(0f, 0f, -0.5f), 0.6f)));   // touches the apex
		Assert.IsFalse(C.Intersects(new Sphere(new Vector3(0f, 0f, -1f), 0.9f)));    // behind the apex
		Assert.IsTrue(C.Intersects(new Sphere(new Vector3(0f, 0f, 5f), 1.1f)));      // touches the base
		Assert.IsFalse(C.Intersects(new Sphere(new Vector3(0f, 0f, 5f), 0.9f)));     // beyond the base
		// Lateral surface: distance from (3, 0, 0) to the line z = 4x/3 is 2.4.
		Assert.IsTrue(C.Intersects(new Sphere(new Vector3(3f, 0f, 0f), 2.5f)));
		Assert.IsFalse(C.Intersects(new Sphere(new Vector3(3f, 0f, 0f), 2.3f)));
		Assert.AreEqual(C.Intersects(new Sphere(new Vector3(3f, 0f, 0f), 2.5f)), new Sphere(new Vector3(3f, 0f, 0f), 2.5f).Intersects(C));
	}
}

[TestClass]
public sealed class CapsuleTests
{
	private static readonly Capsule C = new Capsule(new Vector3(0f, 2f, 0f), Vector3.UnitY, 4f, 1f);

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(new Vector3(0f, 2f, 0f), C.Center);
		Assert.AreEqual(Vector3.UnitY, C.Axis);
		Assert.AreEqual(4f, C.Height);
		Assert.AreEqual(2f, C.HalfHeight);
		Assert.AreEqual(6f, C.TotalHeight);
		Assert.AreEqual(1f, C.Radius);
		AssertEx.AreEqual(12f*MathF.PI, C.SurfaceArea);
		AssertEx.AreEqual(MathF.PI*(4f/3f + 4f), C.Volume);
		Assert.IsTrue(C.IsFinite);
		Assert.IsFalse(new Capsule(Vector3.Zero, new Vector3(Single.NaN), 1f, 1f).IsFinite);
		Assert.AreEqual(C, new Capsule(new Vector3(0f, 2f, 0f), Axis.Y, 4f, 1f));
		Assert.IsTrue(C.ApproxEquals(new Capsule(Vector3.Zero, new Vector3(0f, 4f, 0f), 1f)));

		Capsule c = C;
		c.TotalHeight = 10f;
		Assert.AreEqual(8f, c.Height);
		c.Center = Vector3.Zero;
		c.Axis = Vector3.UnitZ;
		c.Height = 1f;
		c.Radius = 2f;
		Assert.AreEqual(new Capsule(Vector3.Zero, Vector3.UnitZ, 1f, 2f), c);
	}

	[TestMethod]
	public void Equality_Normalize_Translate()
	{
		Capsule same = new Capsule(new Vector3(0f, 2f, 0f), Vector3.UnitY, 4f, 1f);
		Assert.IsTrue(C == same);
		Assert.IsFalse(C != same);
		Assert.IsTrue(C.Equals((object)same));
		Assert.AreEqual(C.GetHashCode(), same.GetHashCode());
		Assert.IsFalse(C.ApproxEquals(new Capsule(C.Center, C.Axis, C.Height, 1.01f)));
		Assert.IsTrue(C.ApproxEquals(new Capsule(C.Center, C.Axis, C.Height, 1.01f), 0.1f));

		Capsule c = new Capsule(C.Center, new Vector3(0f, 4f, 0f), 1f, 1f);
		Assert.AreEqual(C, Capsule.Normalize(c));
		c.Normalize();
		Assert.AreEqual(C, c);
		Assert.AreEqual(new Vector3(1f, 3f, 1f), Capsule.Translate(C, new Vector3(1f)).Center);
		c.Translate(new Vector3(1f));
		Assert.AreEqual(new Vector3(1f, 3f, 1f), c.Center);
	}

	[TestMethod]
	public void CircumscribedBox_IncludesHemispheres()
	{
		OrientedBox box = C.GetCircumscribedBox();
		Assert.AreEqual(C.Center, box.Center);
		AssertEx.AreEqual(new Vector3(1f, 3f, 1f), box.HalfDimensions);
		Assert.IsTrue(box.Contains(new Vector3(0f, 4.99f, 0f)));
		Assert.IsTrue(box.Contains(new Vector3(0f, -0.99f, 0f)));
	}
}

[TestClass]
public sealed class TorusTests
{
	private static readonly Torus T = new Torus(new Vector3(1f, 2f, 3f), Vector3.UnitY, 3f, 1f);

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(new Vector3(1f, 2f, 3f), T.Center);
		Assert.AreEqual(Vector3.UnitY, T.Axis);
		Assert.AreEqual(3f, T.MajorRadius);
		Assert.AreEqual(1f, T.MinorRadius);
		Assert.AreEqual(2f, T.InnerRadius);
		Assert.AreEqual(4f, T.OuterRadius);
		Assert.AreEqual(3f, T.AspectRatio);
		AssertEx.AreEqual(12f*MathF.PI*MathF.PI, T.SurfaceArea, 1e-4f);
		AssertEx.AreEqual(6f*MathF.PI*MathF.PI, T.Volume, 1e-4f);
		Assert.IsTrue(T.IsFinite);
		Assert.IsFalse(new Torus(Vector3.Zero, Vector3.UnitY, Single.NaN, 1f).IsFinite);
		Assert.AreEqual(T, new Torus(new Vector3(1f, 2f, 3f), Axis.Y, 3f, 1f));

		Torus t = T;
		t.Center = Vector3.Zero;
		t.Axis = Vector3.UnitZ;
		t.MajorRadius = 5f;
		t.MinorRadius = 0.5f;
		Assert.AreEqual(new Torus(Vector3.Zero, Vector3.UnitZ, 5f, 0.5f), t);
	}

	[TestMethod]
	public void Equality_Translate()
	{
		Torus same = new Torus(new Vector3(1f, 2f, 3f), Vector3.UnitY, 3f, 1f);
		Assert.IsTrue(T == same);
		Assert.IsFalse(T != same);
		Assert.IsTrue(T.Equals((object)same));
		Assert.AreEqual(T.GetHashCode(), same.GetHashCode());
		Assert.IsFalse(T.ApproxEquals(new Torus(T.Center, T.Axis, 3.01f, 1f)));
		Assert.IsTrue(T.ApproxEquals(new Torus(T.Center, T.Axis, 3.01f, 1f), 0.1f));
		Assert.AreEqual(new Vector3(2f, 3f, 4f), Torus.Translate(T, new Vector3(1f)).Center);
		Torus t = T;
		t.Translate(new Vector3(1f));
		Assert.AreEqual(new Vector3(2f, 3f, 4f), t.Center);
	}

	[TestMethod]
	public void CircumscribedBox()
	{
		Torus tilted = new Torus(Vector3.Zero, Vector3.Normalize(new Vector3(1f, 1f, 0f)), 3f, 1f);
		foreach (Torus t in new[] { T, tilted })
		{
			OrientedBox box = t.GetCircumscribedBox();
			Assert.AreEqual(t.Center, box.Center);
			AssertEx.AreEqual(new Vector3(4f, 1f, 4f), box.HalfDimensions);
			AssertEx.AreEqual(1f, Math.Abs(Vector3.Dot(box.Basis.Row1, t.Axis)));
			AssertEx.IsOrthonormal(box.Basis);
		}
	}
}

[TestClass]
public sealed class SymmetricFrustumTests
{
	// Looks down +Z from the origin; near plane z = 1 with half extents (1, 0.5), far plane z = 4.
	private static readonly SymmetricFrustum F = new SymmetricFrustum(Vector3.Zero, Matrix3.Identity, new Vector2(1f, 0.5f), 1f, 4f);

	[TestMethod]
	public void Constructors_And_Properties()
	{
		Assert.AreEqual(Vector3.Zero, F.Origin);
		Assert.AreEqual(Matrix3.Identity, F.Basis);
		Assert.AreEqual(new Vector2(1f, 0.5f), F.HalfDimensions);
		Assert.AreEqual(new Vector2(2f, 1f), F.Dimensions);
		Assert.AreEqual(new Vector2(4f, 2f), F.BaseHalfDimensions);
		Assert.AreEqual(new Vector2(8f, 4f), F.BaseDimensions);
		Assert.AreEqual(new Interval(1f, 4f), F.DepthRange);
		Assert.AreEqual(1f, F.MinDepth);
		Assert.AreEqual(4f, F.MaxDepth);
		Assert.AreEqual(4f, F.DepthRatio);
		Assert.IsTrue(F.IsFinite);
		Assert.AreEqual(F, new SymmetricFrustum(Vector3.Zero, Matrix3.Identity, new Vector2(1f, 0.5f), new Interval(1f, 4f)));
	}

	[TestMethod]
	public void Setters()
	{
		SymmetricFrustum f = F;
		f.Dimensions = new Vector2(4f, 2f);
		Assert.AreEqual(new Vector2(2f, 1f), f.HalfDimensions);
		f.BaseHalfDimensions = new Vector2(4f, 2f);
		Assert.AreEqual(new Vector2(1f, 0.5f), f.HalfDimensions);
		f.BaseDimensions = new Vector2(16f, 8f);
		Assert.AreEqual(new Vector2(2f, 1f), f.HalfDimensions);
		f.HalfDimensions = new Vector2(3f);
		Assert.AreEqual(new Vector2(3f), f.HalfDimensions);
		f.MinDepth = 2f;
		f.MaxDepth = 10f;
		Assert.AreEqual(new Interval(2f, 10f), f.DepthRange);
		f.DepthRange = new Interval(0.5f, 1f);
		Assert.AreEqual(0.5f, f.MinDepth);
		f.Origin = Vector3.UnitX;
		f.Basis = Matrix3.Rotation(Axis.Y, 1f);
		Assert.AreEqual(Vector3.UnitX, f.Origin);
		Assert.AreEqual(Matrix3.Rotation(Axis.Y, 1f), f.Basis);
	}

	[TestMethod]
	public void Equality_ApproxEquals_HashCode()
	{
		SymmetricFrustum same = new SymmetricFrustum(Vector3.Zero, Matrix3.Identity, new Vector2(1f, 0.5f), 1f, 4f);
		Assert.IsTrue(F == same);
		Assert.IsFalse(F != same);
		Assert.IsTrue(F.Equals((object)same));
		Assert.AreEqual(F.GetHashCode(), same.GetHashCode());
		SymmetricFrustum deeper = new SymmetricFrustum(Vector3.Zero, Matrix3.Identity, new Vector2(1f, 0.5f), 1f, 4.01f);
		Assert.IsTrue(F != deeper);
		Assert.IsFalse(F.ApproxEquals(deeper, 1e-3f));
		Assert.IsTrue(F.ApproxEquals(deeper, 0.1f));
	}

	[TestMethod]
	public void Vertices()
	{
		Vector3[] vertices = F.GetVertices().ToArray();
		Assert.HasCount(8, vertices);
		Vector3[] expected =
		{
			new Vector3(-1f, -0.5f, 1f), new Vector3(1f, -0.5f, 1f), new Vector3(-1f, 0.5f, 1f), new Vector3(1f, 0.5f, 1f),
			new Vector3(-4f, -2f, 4f), new Vector3(4f, -2f, 4f), new Vector3(-4f, 2f, 4f), new Vector3(4f, 2f, 4f),
		};
		for (int i = 0; i < expected.Length; i++)
			AssertEx.AreEqual(expected[i], vertices[i]);

		SymmetricFrustum moved = new SymmetricFrustum(new Vector3(1f, 2f, 3f), Matrix3.Rotation(Axis.Y, 0.5f), F.HalfDimensions, F.DepthRange);
		AffineTransform at = new AffineTransform(moved.Basis, moved.Origin);
		Vector3[] movedVertices = moved.GetVertices().ToArray();
		for (int i = 0; i < expected.Length; i++)
			AssertEx.AreEqual(Vector3.Transform(expected[i], at), movedVertices[i]);
	}

	[TestMethod]
	public void HalfSpaces_And_Contains()
	{
		Assert.HasCount(6, F.GetHalfSpaces().ToArray());
		Vector3[] inside = { new Vector3(0f, 0f, 2f), new Vector3(1.9f, 0f, 2f), new Vector3(0f, 0.9f, 2f), new Vector3(3.9f, 1.9f, 3.99f) };
		Vector3[] outside = { new Vector3(0f, 0f, 0.5f), new Vector3(0f, 0f, 4.5f), new Vector3(2.1f, 0f, 2f), new Vector3(0f, 1.1f, 2f), new Vector3(-2.1f, 0f, 2f), new Vector3(0f, -1.1f, 2f) };
		foreach (Vector3 p in inside)
			Assert.IsTrue(F.Contains(p), p.ToString());
		foreach (Vector3 p in outside)
			Assert.IsFalse(F.Contains(p), p.ToString());

		// A mirrored basis (negative determinant) must describe the same volume.
		SymmetricFrustum mirrored = new SymmetricFrustum(Vector3.Zero, Matrix3.Scaling(new Vector3(-1f, 1f, 1f)), F.HalfDimensions, F.DepthRange);
		foreach (Vector3 p in inside)
			Assert.IsTrue(mirrored.Contains(p), p.ToString());
		foreach (Vector3 p in outside)
			Assert.IsFalse(mirrored.Contains(p), p.ToString());

		// Without a far plane only five half-spaces are produced.
		SymmetricFrustum infinite = new SymmetricFrustum(Vector3.Zero, Matrix3.Identity, F.HalfDimensions, 1f, Single.MaxValue);
		Assert.HasCount(5, infinite.GetHalfSpaces().ToArray());
		Assert.IsTrue(infinite.Contains(new Vector3(0f, 0f, 1000f)));
	}

	[TestMethod]
	public void CircumscribedVolumes_ContainAllVertices()
	{
		SymmetricFrustum longFrustum = new SymmetricFrustum(new Vector3(1f, 1f, 1f), Matrix3.Rotation(Axis.X, 0.4f), new Vector2(0.2f, 0.1f), 1f, 10f);
		foreach (SymmetricFrustum f in new[] { F, longFrustum })
		{
			OrientedBox box = f.GetCircumscribedBox();
			Sphere sphere = f.GetCircumscribedSphere();
			Cone cone = f.GetCircumscribedCone();
			Vector3 tipCenter = f.Origin + f.MinDepth*f.Basis.Row2;
			foreach (Vector3 v in f.GetVertices())
			{
				Vector3 shrunk = v + 1e-3f*(tipCenter - v); // nudge inwards to avoid boundary round-off
				Assert.IsTrue(box.Contains(shrunk), v.ToString());
				Assert.IsTrue(Vector3.Distance(v, sphere.Center) <= sphere.Radius + 1e-4f, v.ToString());
				Assert.IsTrue(cone.Contains(shrunk), v.ToString());
			}
			Assert.IsTrue(Vector3.Distance(f.Origin, sphere.Center) <= sphere.Radius + 1e-4f);
		}
		AssertEx.AreEqual(new Vector3(4f, 2f, 1.5f), F.GetCircumscribedBox().HalfDimensions);
		AssertEx.AreEqual(new Vector3(0f, 0f, 2.5f), F.GetCircumscribedBox().Center);
		AssertEx.AreEqual(MathF.Sqrt(20f), F.GetCircumscribedSphere().Radius);
		AssertEx.AreEqual(5.25f, longFrustum.GetCircumscribedSphere().Radius, 1e-4f);
		Cone c = F.GetCircumscribedCone();
		Assert.AreEqual(F.Origin, c.Vertex);
		Assert.AreEqual(4f, c.Height);
		AssertEx.AreEqual(MathF.Sqrt(20f), c.BaseRadius);
	}

	[TestMethod]
	public void Orthonormalize_And_Translate()
	{
		SymmetricFrustum f = new SymmetricFrustum(Vector3.Zero, Matrix3.Scaling(new Vector3(2f, 3f, 4f)), new Vector2(1f), 1f, 2f);
		SymmetricFrustum o = SymmetricFrustum.Orthonormalize(f);
		AssertEx.AreEqual(Matrix3.Identity, o.Basis);
		AssertEx.AreEqual(new Vector2(2f, 3f), o.HalfDimensions);
		AssertEx.AreEqual(new Interval(4f, 8f), o.DepthRange);
		f.Orthonormalize();
		Assert.IsTrue(o.ApproxEquals(f, 1e-6f));

		Assert.AreEqual(new Vector3(1f, 2f, 3f), SymmetricFrustum.Translate(F, new Vector3(1f, 2f, 3f)).Origin);
		SymmetricFrustum t = F;
		t.Translate(new Vector3(1f, 2f, 3f));
		Assert.AreEqual(new Vector3(1f, 2f, 3f), t.Origin);
	}

	[TestMethod]
	public void ClosestPoint_And_Distance()
	{
		(Vector3 point, Vector3 closest)[] cases =
		{
			(new Vector3(0f, 0f, 2f), new Vector3(0f, 0f, 2f)),           // inside
			(new Vector3(0f, 0f, 6f), new Vector3(0f, 0f, 4f)),           // beyond far plane
			(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f)),           // before near plane
			(new Vector3(10f, 0f, 4f), new Vector3(4f, 0f, 4f)),          // far edge
			(new Vector3(3f, 0f, 2f), new Vector3(2.5f, 0f, 2.5f)),       // side face x = z
			(new Vector3(0.2f, 0.1f, -3f), new Vector3(0.2f, 0.1f, 1f)),  // near face
		};
		foreach (var (point, closest) in cases)
		{
			AssertEx.AreEqual(closest, F.GetClosestPoint(point), 1e-5f, point.ToString());
			AssertEx.AreEqual(Vector3.Distance(point, closest), F.GetDistanceTo(point), 1e-5f, point.ToString());
		}
	}

	[TestMethod]
	public void Intersects()
	{
		Assert.IsTrue(F.Intersects(new Sphere(new Vector3(0f, 0f, 6f), 2.1f)));
		Assert.IsFalse(F.Intersects(new Sphere(new Vector3(0f, 0f, 6f), 1.9f)));
		Assert.IsTrue(F.Intersects(new Sphere(new Vector3(3f, 0f, 2f), 0.75f)));
		Assert.IsFalse(F.Intersects(new Sphere(new Vector3(3f, 0f, 2f), 0.65f)));
		Assert.IsTrue(new Sphere(new Vector3(0f, 0f, 2f), 0.1f).Intersects(F));

		AxisAlignedBox inside = new AxisAlignedBox(new Vector3(-0.1f, -0.1f, 2f), new Vector3(0.1f, 0.1f, 3f));
		AxisAlignedBox beyond = new AxisAlignedBox(new Vector3(-1f, -1f, 5f), new Vector3(1f, 1f, 6f));
		AxisAlignedBox straddling = new AxisAlignedBox(new Vector3(-10f, -10f, 3f), new Vector3(10f, 10f, 3.5f));
		Assert.IsTrue(F.Intersects(inside));
		Assert.IsFalse(F.Intersects(beyond));
		Assert.IsTrue(F.Intersects(straddling));
		Assert.IsTrue(inside.Intersects(F));
		Assert.IsFalse(beyond.Intersects(F));

		OrientedBox obbInside = OrientedBox.FromAxisAlignedBox(inside, Matrix3.Rotation(Axis.Z, 0.3f), true);
		OrientedBox obbBehind = new OrientedBox(new Vector3(0f, 0f, -2f), Matrix3.Rotation(Axis.Y, 0.3f), new Vector3(0.5f));
		Assert.IsTrue(F.Intersects(obbInside));
		Assert.IsFalse(F.Intersects(obbBehind));
		Assert.IsTrue(obbInside.Intersects(F));
		Assert.IsFalse(obbBehind.Intersects(F));
	}
}
