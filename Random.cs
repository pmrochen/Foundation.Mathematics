/*
 *  Name: Random
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	public static class Random
	{
		public static IRandomNumberGenerator<uint> DefaultGenerator = new MersenneTwister();

		public static int GetInt32(int min, int max)
		{
			return GetInt32(min, max, DefaultGenerator);
		}

		public static int GetInt32(int min, int max, IRandomNumberGenerator<uint> generator)
		{
			return Math.Clamp(Scalar.Round(min - 0.5 + ((max - min) + 1.0)*GetDouble01(generator)), min, max);
		}

		public static float GetSingle(float min, float max)
		{
			return GetSingle(min, max, DefaultGenerator);
		}

		public static float GetSingle(float min, float max, IRandomNumberGenerator<uint> generator)
		{
			return (float)(min + ((double)max - min)*GetDouble01(generator));
		}

		/// <summary>
		/// Returns a random number in [min, max]. The range must be no wider than Double.MaxValue (max - min must not
		/// overflow); it is not checked and the result is unspecified otherwise.
		/// </summary>
		public static double GetDouble(double min, double max)
		{
			return GetDouble(min, max, DefaultGenerator);
		}

		/// <summary>
		/// Returns a random number in [min, max]. The range must be no wider than Double.MaxValue (max - min must not
		/// overflow); it is not checked and the result is unspecified otherwise.
		/// </summary>
		public static double GetDouble(double min, double max, IRandomNumberGenerator<uint> generator)
		{
			return min + (max - min)*GetDouble01(generator);
		}

		/// <summary>
		/// Returns a random number in [min, max), rounded towards min. Requires min &lt; max; the range is not checked
		/// and the result is unspecified otherwise.
		/// </summary>
		public static float GetSingleRightOpen(float min, float max)
		{
			return GetSingleRightOpen(min, max, DefaultGenerator);
		}

		/// <summary>
		/// Returns a random number in [min, max), rounded towards min. Requires min &lt; max; the range is not checked
		/// and the result is unspecified otherwise.
		/// </summary>
		public static float GetSingleRightOpen(float min, float max, IRandomNumberGenerator<uint> generator)
		{
			// The closed [0, 1] variant is used on purpose, see GetDoubleRightOpen.
			float result = FloorToSingle(min + ((double)max - min)*GetDouble01(generator));
			return (result < max) ? result : MathF.BitDecrement(max);
		}

		/// <summary>
		/// Returns a random number in [min, max). Requires min &lt; max and a range no wider than Double.MaxValue
		/// (max - min must not overflow); the range is not checked and the result is unspecified otherwise.
		/// </summary>
		public static double GetDoubleRightOpen(double min, double max)
		{
			return GetDoubleRightOpen(min, max, DefaultGenerator);
		}

		/// <summary>
		/// Returns a random number in [min, max). Requires min &lt; max and a range no wider than Double.MaxValue
		/// (max - min must not overflow); the range is not checked and the result is unspecified otherwise.
		/// </summary>
		public static double GetDoubleRightOpen(double min, double max, IRandomNumberGenerator<uint> generator)
		{
			// The closed [0, 1] variant is used on purpose: only an exact 1 maps to max, which is then moved to the
			// largest value below max, so the gap to max is a single bit regardless of the size of the range.
			double result = min + (max - min)*GetDouble01(generator);
			return (result < max) ? result : Math.BitDecrement(max);
		}

		public static float GetSingle01()
		{
			return GetSingle01(DefaultGenerator);
		}

		public static float GetSingle01(IRandomNumberGenerator<uint> generator)
		{
			return (float)GetDouble01(generator);
		}

		public static double GetDouble01()
		{
			return GetDouble01(DefaultGenerator);
		}

		public static double GetDouble01(IRandomNumberGenerator<uint> generator)
		{
			return generator.GetNext()/(double)generator.MaxValue;
		}

		public static float GetSingle01RightOpen()
		{
			return GetSingle01RightOpen(DefaultGenerator);
		}

		public static float GetSingle01RightOpen(IRandomNumberGenerator<uint> generator)
		{
			return FloorToSingle(GetDouble01RightOpen(generator));
		}

		public static double GetDouble01RightOpen()
		{
			return GetDouble01RightOpen(DefaultGenerator);
		}

		public static double GetDouble01RightOpen(IRandomNumberGenerator<uint> generator)
		{
			return generator.GetNext()/((double)generator.MaxValue + 1.0);
		}

		public static Vector2 GetVectorOnUnitCircle()
		{
			return GetVectorOnUnitCircle(DefaultGenerator);
		}

		public static Vector2 GetVectorOnUnitCircle(IRandomNumberGenerator<uint> generator)
		{
			float theta = GetSingleRightOpen(0f, SingleConstants.TwoPi, generator);
			return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
		}

		public static Vector2 GetVectorOnUnitDisk()
		{
			return GetVectorOnUnitDisk(DefaultGenerator);
		}

		public static Vector2 GetVectorOnUnitDisk(IRandomNumberGenerator<uint> generator)
		{
			Vector2 v;
			do
			{
				v = new Vector2(GetSingle(-1f, 1f, generator), GetSingle(-1f, 1f, generator));
			} while (v.MagnitudeSquared > 1f);

			return v;
		}

		public static Vector3 GetVectorInUnitSphere()
		{
			return GetVectorInUnitSphere(DefaultGenerator);
		}

		public static Vector3 GetVectorInUnitSphere(IRandomNumberGenerator<uint> generator)
		{
			Vector3 v;
			do
			{
				v = new Vector3(GetSingle(-1f, 1f, generator), GetSingle(-1f, 1f, generator), GetSingle(-1f, 1f, generator));
			} while (v.MagnitudeSquared > 1f);

			return v;
		}

		public static Vector3 GetVectorOnUnitSphere()
		{
			return GetVectorOnUnitSphere(DefaultGenerator);
		}

		public static Vector3 GetVectorOnUnitSphere(IRandomNumberGenerator<uint> generator)
		{
			float z = GetSingle(-1f, 1f, generator);
			float phi = GetSingleRightOpen(0f, SingleConstants.TwoPi, generator);
			float s = MathF.Sqrt(1f - z*z);
			return new Vector3(s*(float)Math.Cos(phi), s*(float)Math.Sin(phi), z);
		}

		public static Vector3 GetVectorOnUnitSphericalCone(float halfAngle)
		{
			return GetVectorOnUnitSphericalCone(halfAngle, DefaultGenerator);
		}

		public static Vector3 GetVectorOnUnitSphericalCone(float halfAngle, IRandomNumberGenerator<uint> generator)
		{
			float z = GetSingle((float)Math.Cos(halfAngle), 1f, generator);
			float phi = GetSingleRightOpen(0f, SingleConstants.TwoPi, generator);
			float s = MathF.Sqrt(1f - z*z);
			return new Vector3(s*(float)Math.Cos(phi), s*(float)Math.Sin(phi), z);
		}

		public static Vector3 GetVectorOnUnitSphericalCone(Vector3 direction, float halfAngle)
		{
			return GetVectorOnUnitSphericalCone(direction, halfAngle, DefaultGenerator);
		}

		public static Vector3 GetVectorOnUnitSphericalCone(Vector3 direction, float halfAngle, IRandomNumberGenerator<uint> generator)
		{
			Vector3 tangentX = Vector3.Normalize(Vector3.Cross((Math.Abs(direction.Z) < 0.999f) ? Vector3.UnitZ : Vector3.UnitX, direction));
			Vector3 tangentY = Vector3.Cross(direction, tangentX);
			float theta = (float)Math.Acos(GetSingle((float)Math.Cos(halfAngle), 1f, generator));
			float phi = GetSingleRightOpen(0f, SingleConstants.TwoPi, generator);
			return (float)Math.Sin(theta)*((float)Math.Cos(phi)*tangentX + (float)Math.Sin(phi)*tangentY) + (float)Math.Cos(theta)*direction;
		}

		public static Matrix3 GetRotationMatrix()
		{
			return GetRotationMatrix(DefaultGenerator);
		}

		public static Matrix3 GetRotationMatrix(IRandomNumberGenerator<uint> generator)
		{
			//float d = Math.Clamp(range, 0f, 1f);
			float theta = GetSingle(0f, SingleConstants.TwoPi/* *d*/, generator);
			float phi = GetSingle(0f, SingleConstants.TwoPi, generator);
			float m = GetSingle(0f, 2f/* *d*/, generator);
			float r = MathF.Sqrt(m);
			float sp = (float)Math.Sin(phi);
			float cp = (float)Math.Cos(phi);
			float vx = sp*r;
			float vy = cp*r;
			float vz = MathF.Sqrt(2f - m);
			float st = (float)Math.Sin(theta);
			float ct = (float)Math.Cos(theta);
			float sx = vx*ct - vy*st;
			float sy = vx*st + vy*ct;
			return new Matrix3(vx*sx - ct, vx*sy - st, vx*vz, vy*sx + st, vy*sy - ct, vy*vz, vz*sx, vz*sy, 1f - m);
		}

		public static Quaternion GetQuaternion()
		{
			return GetQuaternion(DefaultGenerator);
		}

		public static Quaternion GetQuaternion(IRandomNumberGenerator<uint> generator)
		{
			float t1 = GetSingle(0f, SingleConstants.TwoPi, generator);
			float t2 = GetSingle(0f, SingleConstants.TwoPi, generator);
			float m = GetSingle01(generator);
			float r1 = MathF.Sqrt(1f - m);
			float r2 = MathF.Sqrt(m);
			float s1 = (float)Math.Sin(t1);
			float c1 = (float)Math.Cos(t1);
			float s2 = (float)Math.Sin(t2);
			float c2 = (float)Math.Cos(t2);
			return new Quaternion(s1*r1, c1*r1, s2*r2, c2*r2);
		}

		/// <summary>
		/// Converts to single precision rounding towards negative infinity. The default conversion rounds to the
		/// nearest float, which can turn a value just below an exclusive upper bound into the bound itself.
		/// </summary>
		internal static float FloorToSingle(double x)
		{
			float f = (float)x;
			return (f > x) ? MathF.BitDecrement(f) : f;
		}

		/// <summary>
		/// Converts to single precision rounding towards positive infinity.
		/// </summary>
		internal static float CeilingToSingle(double x)
		{
			float f = (float)x;
			return (f < x) ? MathF.BitIncrement(f) : f;
		}
	}
}
