/*
 *  Name: Random
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	public static class Random
	{
		public static MersenneTwister Generator = new MersenneTwister();

		public static int GetNextInt32(int min, int max)
		{
			return Generator.GetNext(min, max);
		}

		public static float GetNextSingle(float min, float max)
		{
			return Generator.GetNext(min, max);
		}

		public static double GetNextDouble(double min, double max)
		{
			return Generator.GetNext(min, max);
		}

		public static Vector2 GetNextVectorOnUnitCircle()
		{
			float theta = GetNextSingle(0f, SingleConstants.TwoPi - SingleConstants.Tolerance);
			return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
		}

		public static Vector2 GetNextVectorOnUnitDisk()
		{
			Vector2 v;
			do
			{
				v = new Vector2(GetNextSingle(-1f, 1f), GetNextSingle(-1f, 1f));
			} while (v.MagnitudeSquared > 1f);

			return v;
		}

		public static Vector3 GetNextVectorInUnitSphere()
		{
			Vector3 v;
			do
			{
				v = new Vector3(GetNextSingle(-1f, 1f), GetNextSingle(-1f, 1f), GetNextSingle(-1f, 1f));
			} while (v.MagnitudeSquared > 1f);

			return v;
		}

		public static Vector3 GetNextVectorOnUnitSphere()
		{
			float z = GetNextSingle(-1f, 1f);
			float phi = GetNextSingle(0f, SingleConstants.TwoPi - SingleConstants.Tolerance/*Epsilon*/);
			float s = MathF.Sqrt(1f - z*z);
			return new Vector3(s*(float)Math.Cos(phi), s*(float)Math.Sin(phi), z);
		}

		public static Vector3 GetNextVectorOnUnitSphericalCone(float halfAngle)
		{
			float z = GetNextSingle((float)Math.Cos(halfAngle), 1f);
			float phi = GetNextSingle(0f, SingleConstants.TwoPi - SingleConstants.Tolerance/*Epsilon*/);
			float s = MathF.Sqrt(1f - z*z);
			return new Vector3(s*(float)Math.Cos(phi), s*(float)Math.Sin(phi), z);
		}

		public static Vector3 GetNextVectorOnUnitSphericalCone(Vector3 direction, float halfAngle)
		{
			Vector3 tangentX = Vector3.Normalize(Vector3.Cross((Math.Abs(direction.Z) < 0.999f) ? Vector3.UnitZ : Vector3.UnitX, direction));
			Vector3 tangentY = Vector3.Cross(direction, tangentX);
			float theta = (float)Math.Acos(GetNextSingle((float)Math.Cos(halfAngle), 1f));
			float phi = GetNextSingle(0f, SingleConstants.TwoPi - SingleConstants.Tolerance/*Epsilon*/);
			return (float)Math.Sin(theta)*((float)Math.Cos(phi)*tangentX + (float)Math.Sin(phi)*tangentY) + (float)Math.Cos(theta)*direction;
		}

		public static Quaternion GetNextQuaternion()
		{
			float t1 = GetNextSingle(0f, SingleConstants.TwoPi);
			float t2 = GetNextSingle(0f, SingleConstants.TwoPi);
			float m = GetNextSingle(0f, 1f);
			float r1 = MathF.Sqrt(1f - m);
			float r2 = MathF.Sqrt(m);
			float s1 = (float)Math.Sin(t1);
			float c1 = (float)Math.Cos(t1);
			float s2 = (float)Math.Sin(t2);
			float c2 = (float)Math.Cos(t2);
			return new Quaternion(s1*r1, c1*r1, s2*r2, c2*r2);
		}

		public static Matrix3 GetNextRotationMatrix()
		{
			//float d = Math.Clamp(range, 0f, 1f);
			float theta = GetNextSingle(0f, SingleConstants.TwoPi/* *d*/);
			float phi = GetNextSingle(0f, SingleConstants.TwoPi);
			float m = GetNextSingle(0f, 2f/* *d*/);
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
	}
}
