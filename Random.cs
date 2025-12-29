/*
 *  Name: Random
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	public static class Random
	{
		public static int GetNext/*Int32*/(int min, int max)
		{
			return Default.GetNext(min, max);
		}

		public static float GetNext/*Single*/(float min, float max)
		{
			return Default.GetNext(min, max);
		}

		public static double GetNext/*Double*/(double min, double max)
		{
			return Default.GetNext(min, max);
		}

		public static Vector2 GetNextVectorOnUnitCircle()
		{
			float theta = GetNext/*Single*/(0f, SingleConstants.TwoPi - SingleConstants.Tolerance);
			return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
		}

		public static Vector2 GetNextVectorOnUnitDisk()
		{
			Vector2 v;
			do
			{
				v = new Vector2(GetNext/*Single*/(-1f, 1f), GetNext/*Single*/(-1f, 1f));
			} while (v.MagnitudeSquared > 1f);

			return v;
		}

		public static Vector3 GetNextVectorInUnitSphere()
		{
			Vector3 v;
			do
			{
				v = new Vector3(GetNext/*Single*/(-1f, 1f), GetNext/*Single*/(-1f, 1f), GetNext/*Single*/(-1f, 1f));
			} while (v.MagnitudeSquared > 1f);

			return v;
		}

		public static Vector3 GetNextVectorOnUnitSphere()
		{
			float z = GetNext/*Single*/(-1f, 1f);
			float phi = GetNext/*Single*/(0f, SingleConstants.TwoPi - SingleConstants.Tolerance/*Epsilon*/);
			float s = MathF.Sqrt(1f - z*z);
			return new Vector3(s*(float)Math.Cos(phi), s*(float)Math.Sin(phi), z);
		}

		public static Vector3 GetNextVectorOnUnitSphericalCone(float halfAngle)
		{
			float z = GetNext/*Single*/((float)Math.Cos(halfAngle), 1f);
			float phi = GetNext/*Single*/(0f, SingleConstants.TwoPi - SingleConstants.Tolerance/*Epsilon*/);
			float s = MathF.Sqrt(1f - z*z);
			return new Vector3(s*(float)Math.Cos(phi), s*(float)Math.Sin(phi), z);
		}

		public static Vector3 GetNextVectorOnUnitSphericalCone(Vector3 direction, float halfAngle)
		{
			Vector3 tangentX = Vector3.Normalize(Vector3.Cross((Math.Abs(direction.Z) < 0.999f) ? Vector3.UnitZ : Vector3.UnitX, direction));
			Vector3 tangentY = Vector3.Cross(direction, tangentX);
			float theta = (float)Math.Acos(GetNext/*Single*/((float)Math.Cos(halfAngle), 1f));
			float phi = GetNext/*Single*/(0f, SingleConstants.TwoPi - SingleConstants.Tolerance/*Epsilon*/);
			return (float)Math.Sin(theta)*((float)Math.Cos(phi)*tangentX + (float)Math.Sin(phi)*tangentY) + (float)Math.Cos(theta)*direction;
		}
	}
}
