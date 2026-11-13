using System.Globalization;

namespace Foundation.Mathematics.Tests;

/// <summary>
/// Approximate-equality assertions for the library's floating point types.
/// </summary>
internal static class AssertEx
{
	public const float Tolerance = 1e-5f;

	public static void AreEqual(float expected, float actual, float tolerance = Tolerance, string message = null)
	{
		if (!(Math.Abs(expected - actual) <= tolerance))
			Assert.Fail($"Expected <{expected:R}> but was <{actual:R}> (tolerance {tolerance}). {message}");
	}

	public static void AreEqual(Vector2 expected, Vector2 actual, float tolerance = Tolerance, string message = null)
	{
		if (!expected.ApproxEquals(actual, tolerance))
			Assert.Fail($"Expected <{Format(expected)}> but was <{Format(actual)}> (tolerance {tolerance}). {message}");
	}

	public static void AreEqual(Vector3 expected, Vector3 actual, float tolerance = Tolerance, string message = null)
	{
		if (!expected.ApproxEquals(actual, tolerance))
			Assert.Fail($"Expected <{Format(expected)}> but was <{Format(actual)}> (tolerance {tolerance}). {message}");
	}

	public static void AreEqual(Vector4 expected, Vector4 actual, float tolerance = Tolerance, string message = null)
	{
		if (!expected.ApproxEquals(actual, tolerance))
			Assert.Fail($"Expected <{Format(expected)}> but was <{Format(actual)}> (tolerance {tolerance}). {message}");
	}

	public static void AreEqual(Quaternion expected, Quaternion actual, float tolerance = Tolerance, string message = null)
	{
		if (!expected.ApproxEquals(actual, tolerance))
			Assert.Fail($"Expected <{Format(expected)}> but was <{Format(actual)}> (tolerance {tolerance}). {message}");
	}

	/// <summary>Asserts that two quaternions represent the same rotation (q and -q are equivalent).</summary>
	public static void AreSameRotation(Quaternion expected, Quaternion actual, float tolerance = Tolerance, string message = null)
	{
		if (!expected.ApproxEquals(actual, tolerance) && !expected.ApproxEquals(-actual, tolerance))
			Assert.Fail($"Expected rotation <{Format(expected)}> but was <{Format(actual)}> (tolerance {tolerance}). {message}");
	}

	public static void AreEqual(in Matrix2 expected, in Matrix2 actual, float tolerance = Tolerance, string message = null)
	{
		if (!expected.ApproxEquals(actual, tolerance))
			Assert.Fail($"Expected <{expected.ToString(CultureInfo.InvariantCulture)}> but was <{actual.ToString(CultureInfo.InvariantCulture)}> (tolerance {tolerance}). {message}");
	}

	public static void AreEqual(in Matrix3 expected, in Matrix3 actual, float tolerance = Tolerance, string message = null)
	{
		if (!expected.ApproxEquals(actual, tolerance))
			Assert.Fail($"Expected <{expected.ToString(CultureInfo.InvariantCulture)}> but was <{actual.ToString(CultureInfo.InvariantCulture)}> (tolerance {tolerance}). {message}");
	}

	public static void AreEqual(in Matrix4 expected, in Matrix4 actual, float tolerance = Tolerance, string message = null)
	{
		if (!expected.ApproxEquals(actual, tolerance))
			Assert.Fail($"Expected <{expected.ToString(CultureInfo.InvariantCulture)}> but was <{actual.ToString(CultureInfo.InvariantCulture)}> (tolerance {tolerance}). {message}");
	}

	public static void AreEqual(in AffineTransform expected, in AffineTransform actual, float tolerance = Tolerance, string message = null)
	{
		if (!expected.ApproxEquals(actual, tolerance))
			Assert.Fail($"Expected <{expected.ToString(CultureInfo.InvariantCulture)}> but was <{actual.ToString(CultureInfo.InvariantCulture)}> (tolerance {tolerance}). {message}");
	}

	public static void AreEqual(Interval expected, Interval? actual, float tolerance = Tolerance, string message = null)
	{
		if (!actual.HasValue)
			Assert.Fail($"Expected <{expected.ToString(CultureInfo.InvariantCulture)}> but was <null>. {message}");
		if (!expected.ApproxEquals(actual.Value, tolerance))
			Assert.Fail($"Expected <{expected.ToString(CultureInfo.InvariantCulture)}> but was <{actual.Value.ToString(CultureInfo.InvariantCulture)}> (tolerance {tolerance}). {message}");
	}

	public static void IsOrthonormal(in Matrix3 m, float tolerance = Tolerance)
	{
		AreEqual(Matrix3.Identity, Matrix3.Concat(m, Matrix3.Transpose(m)), tolerance, "Matrix is not orthonormal.");
	}

	private static string Format(Vector2 v) => v.ToString("R", CultureInfo.InvariantCulture);
	private static string Format(Vector3 v) => v.ToString("R", CultureInfo.InvariantCulture);
	private static string Format(Vector4 v) => v.ToString("R", CultureInfo.InvariantCulture);
	private static string Format(Quaternion q) => q.ToString("R", CultureInfo.InvariantCulture);
}

/// <summary>
/// Temporarily replaces the current thread culture (for APIs that parse/format without a provider).
/// </summary>
internal sealed class CultureScope : IDisposable
{
	private readonly CultureInfo previousCulture_;

	public CultureScope(string name) : this(CultureInfo.GetCultureInfo(name))
	{
	}

	public CultureScope(CultureInfo culture)
	{
		previousCulture_ = CultureInfo.CurrentCulture;
		CultureInfo.CurrentCulture = culture;
	}

	public static CultureScope Invariant() => new CultureScope(CultureInfo.InvariantCulture);

	public void Dispose()
	{
		CultureInfo.CurrentCulture = previousCulture_;
	}
}

/// <summary>
/// Deterministic sample data for property-style tests.
/// </summary>
internal static class Samples
{
	public static readonly Vector3[] Vectors =
	{
		new Vector3(1f, 2f, 3f),
		new Vector3(-0.5f, 0.25f, 4f),
		new Vector3(3f, -1f, -2f),
		new Vector3(0f, 0f, 1f),
		new Vector3(-2.5f, 7f, 0.5f),
	};

	public static readonly Vector3[] Axes =
	{
		Vector3.UnitX,
		Vector3.UnitY,
		Vector3.UnitZ,
		Vector3.Normalize(new Vector3(1f, 1f, 1f)),
		Vector3.Normalize(new Vector3(-0.3f, 0.8f, 0.2f)),
	};

	public static readonly float[] Angles = { 0.3f, -1.1f, 2.5f, SingleConstants.HalfPi, -3f };

	/// <summary>A well-conditioned, non-orthogonal 3x3 matrix.</summary>
	public static readonly Matrix3 GeneralMatrix3 = new Matrix3(2f, -1f, 0.5f, 0.3f, 1.5f, -0.7f, 1f, 0.2f, 3f);

	/// <summary>A well-conditioned 4x4 matrix with a non-trivial last column.</summary>
	public static readonly Matrix4 GeneralMatrix4 = new Matrix4(
		2f, -1f, 0.5f, 0.1f,
		0.3f, 1.5f, -0.7f, 0.2f,
		1f, 0.2f, 3f, -0.3f,
		0.5f, -2f, 1f, 1.2f);

	public static IEnumerable<Quaternion> UnitQuaternions()
	{
		for (int i = 0; i < Axes.Length; i++)
			yield return Quaternion.FromAxisAngle(Axes[i], Angles[i]);
	}
}
