/*
 *  Name: Scalar
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	public static class Scalar
	{
		public static bool IsFinite(float x) { return !Single.IsInfinity(x) && !Single.IsNaN(x); }
		public static bool IsFinite(double x) { return !Double.IsInfinity(x) && !Double.IsNaN(x); }
		public static float Log2(float x) { return MathF.Log(x)/0.693147180559945309417f; }
		public static double Log2(double x) { return Math.Log(x)/0.693147180559945309417; }
		public static float Square(float x) { return x*x; }
		public static double Square(double x) { return x*x; }
		public static int Sign(int x) { return (x > 0) ? 1 : ((x < 0) ? -1 : 0); }
		public static float Sign(float x) { return (x > 0f) ? 1f : ((x < 0f) ? -1f : 0f); }
		public static double Sign(double x) { return (x > 0.0) ? 1.0 : ((x < 0.0) ? -1.0 : 0.0); }
        public static int Round(float x) { return (x >= 0f) ? (int)(x + 0.5f) : (int)(x - 0.5f); }
        public static int Round(double x) { return (x >= 0.0) ? (int)(x + 0.5) : (int)(x - 0.5); }
		public static int Truncate(float x) { return (x >= 0f) ? (int)x : -(int)(-x); }
		public static int Truncate(double x) { return (x >= 0.0) ? (int)x : -(int)(-x); }
		public static float Fractional(float x) { return x - MathF.Floor(x); }
		public static double Fractional(double x) { return x - Math.Floor(x); }
		public static float Radians(float x) { return x*0.01745329251994329547f; }
		public static double Radians(double x) { return x*0.01745329251994329547; }
		public static float Degrees(float x) { return x*57.29577951308232286465f; }
		public static double Degrees(double x) { return x*57.29577951308232286465; }
		public static bool ApproxEquals(float x, float y, float tolerance) { return (Math.Abs(y - x) < tolerance); }
		public static bool ApproxEquals(double x, double y, double tolerance) { return (Math.Abs(y - x) < tolerance); }
		public static bool ApproxEquals(float x, float y) { return ApproxEquals(x, y, 1e-6f); }
		public static bool ApproxEquals(double x, double y) { return ApproxEquals(x, y, 1e-15); }
		public static float Lerp(float a, float b, float t) { return a + t*(b - a); }
		public static double Lerp(double a, double b, double t) { return a + t*(b - a); }
		public static float Step(float a, float t) { return ((t >= a) ? 1f : 0f); }
		public static double Step(double a, double t) { return ((t >= a) ? 1.0 : 0.0); }
		public static float Pulse(float a, float b, float t) { return (Step(a, t) - Step(b, t)); }
		public static double Pulse(double a, double b, double t) { return (Step(a, t) - Step(b, t)); }
		
		public static float BoxStep(float a, float b, float t)
		{
			if (t <= a) 
				return 0f;
			if (t >= b) 
				return 1f;
			return (t - a)/(b - a);
		}
		
		public static double BoxStep(double a, double b, double t)
		{
			if (t <= a) 
				return 0.0;
			if (t >= b) 
				return 1.0;
			return (t - a)/(b - a);
		}
		
		public static float SmoothStep(float a, float b, float t)
		{
			if (t <= a) 
				return 0f;
			if (t >= b) 
				return 1f;
			t = (t - a)/(b - a);
			return t*t*(3f - 2f*t);
		}
		
		public static double SmoothStep(double a, double b, double t)
		{
			if (t <= a) 
				return 0.0;
			if (t >= b) 
				return 1.0;
			t = (t - a)/(b - a);
			return t*t*(3.0 - 2.0*t);
		}

		public static float SmootherStep(float a, float b, float t)
		{
			if (t <= a)
				return 0f;
			if (t >= b)
				return 1f;
			t = (t - a)/(b - a);
			return t*t*t*(t*(t*6f - 15f) + 10f);
		}

		public static double SmootherStep(double a, double b, double t)
		{
			if (t <= a)
				return 0.0;
			if (t >= b)
				return 1.0;
			t = (t - a)/(b - a);
			return t*t*t*(t*(t*6.0 - 15.0) + 10.0);
		}
	}
}
