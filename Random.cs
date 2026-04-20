/*
 *  Name: Random
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	public static class Random
	{
		public static IRandomNumberGenerator<uint> Generator = new MersenneTwister();

		public static int GetInt32(int min, int max)
		{
			//if (min >= max)
			//	return min;

			return Math.Clamp(Scalar.Round(min - 0.5 + (max - min + 1.0)*GetDouble01()), min, max);
		}

		public static float GetSingle(float min, float max)
		{
			//if (min >= max)
			//	return min;

			return (float)(min + (max - min)*GetDouble01());
		}

		public static double GetDouble(double min, double max)
		{
			//if (min >= max)
			//	return min;

			return (min + (max - min)*GetDouble01());
		}

		public static float GetSingleRightOpen(float min, float max)
		{
			//if (min >= max)
			//	return min;

			return (float)(min + (max - min)*GetDouble01RightOpen());
		}

		public static double GetDoubleRightOpen(double min, double max)
		{
			//if (min >= max)
			//	return min;

			return (min + (max - min)*GetDouble01RightOpen());
		}

		public static Vector2 GetVectorOnUnitCircle()
		{
			float theta = GetSingleRightOpen(0f, SingleConstants.TwoPi);
			return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
		}

		public static Vector2 GetVectorOnUnitDisk()
		{
			Vector2 v;
			do
			{
				v = new Vector2(GetSingle(-1f, 1f), GetSingle(-1f, 1f));
			} while (v.MagnitudeSquared > 1f);

			return v;
		}

		public static Vector3 GetVectorInUnitSphere()
		{
			Vector3 v;
			do
			{
				v = new Vector3(GetSingle(-1f, 1f), GetSingle(-1f, 1f), GetSingle(-1f, 1f));
			} while (v.MagnitudeSquared > 1f);

			return v;
		}

		public static Vector3 GetVectorOnUnitSphere()
		{
			float z = GetSingle(-1f, 1f);
			float phi = GetSingleRightOpen(0f, SingleConstants.TwoPi);
			float s = MathF.Sqrt(1f - z*z);
			return new Vector3(s*(float)Math.Cos(phi), s*(float)Math.Sin(phi), z);
		}

		public static Vector3 GetVectorOnUnitSphericalCone(float halfAngle)
		{
			float z = GetSingle((float)Math.Cos(halfAngle), 1f);
			float phi = GetSingleRightOpen(0f, SingleConstants.TwoPi);
			float s = MathF.Sqrt(1f - z*z);
			return new Vector3(s*(float)Math.Cos(phi), s*(float)Math.Sin(phi), z);
		}

		public static Vector3 GetVectorOnUnitSphericalCone(Vector3 direction, float halfAngle)
		{
			Vector3 tangentX = Vector3.Normalize(Vector3.Cross((Math.Abs(direction.Z) < 0.999f) ? Vector3.UnitZ : Vector3.UnitX, direction));
			Vector3 tangentY = Vector3.Cross(direction, tangentX);
			float theta = (float)Math.Acos(GetSingle((float)Math.Cos(halfAngle), 1f));
			float phi = GetSingleRightOpen(0f, SingleConstants.TwoPi);
			return (float)Math.Sin(theta)*((float)Math.Cos(phi)*tangentX + (float)Math.Sin(phi)*tangentY) + (float)Math.Cos(theta)*direction;
		}

		public static Quaternion GetQuaternion()
		{
			float t1 = GetSingle(0f, SingleConstants.TwoPi);
			float t2 = GetSingle(0f, SingleConstants.TwoPi);
			float m = GetSingle(0f, 1f);
			float r1 = MathF.Sqrt(1f - m);
			float r2 = MathF.Sqrt(m);
			float s1 = (float)Math.Sin(t1);
			float c1 = (float)Math.Cos(t1);
			float s2 = (float)Math.Sin(t2);
			float c2 = (float)Math.Cos(t2);
			return new Quaternion(s1*r1, c1*r1, s2*r2, c2*r2);
		}

		public static Matrix3 GetRotationMatrix()
		{
			//float d = Math.Clamp(range, 0f, 1f);
			float theta = GetSingle(0f, SingleConstants.TwoPi/* *d*/);
			float phi = GetSingle(0f, SingleConstants.TwoPi);
			float m = GetSingle(0f, 2f/* *d*/);
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

		private static double GetDouble01()
		{
			return Generator.GetNext()/(double)(Generator.MaxValue - 1);
		}

		private static double GetDouble01RightOpen()
		{
			return Generator.GetNext()/(double)Generator.MaxValue;
		}
	}
}
