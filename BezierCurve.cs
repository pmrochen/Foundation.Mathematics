/*
 *  Name: BezierCurve
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	/// <summary>
	/// 1D Cubic Bezier Curve.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(BezierCurveConverter))]
	public struct BezierCurve : ISerializable, IFormattable, IEquatable<BezierCurve> // #TODO SIMD
	{
		public static readonly BezierCurve Zero = new BezierCurve(0f, 0f, 0f, 0f);

		public BezierCurve(float p0, float p1, float p2, float p3)
		{
			p0_ = p0;
			p1_ = p1;
			p2_ = p2;
			p3_ = p3;
		}

		public BezierCurve(float[] points)
		{
			p0_ = points[0];
			p1_ = points[1];
			p2_ = points[2];
			p3_ = points[3];
		}

		private BezierCurve(SerializationInfo info, StreamingContext context)
		{
			p0_ = info.GetSingle("P0");
			p1_ = info.GetSingle("P1");
			p2_ = info.GetSingle("P2");
			p3_ = info.GetSingle("P3");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("P0", p0_);
			info.AddValue("P1", p1_);
			info.AddValue("P2", p2_);
			info.AddValue("P3", p3_);
		}

		public float ControlPoint0
		{
			readonly get => p0_;
			set => p0_ = value;
		}

		public float ControlPoint1
		{
			readonly get => p1_;
			set => p1_ = value;
		}

		public float ControlPoint2
		{
			readonly get => p2_;
			set => p2_ = value;
		}

		public float ControlPoint3
		{
			readonly get => p3_;
			set => p3_ = value;
		}

		public readonly override int GetHashCode()
		{
			int hash = p0_.GetHashCode();
			hash = ((hash << 5) + hash) ^ p1_.GetHashCode();
			hash = ((hash << 5) + hash) ^ p2_.GetHashCode();
			return ((hash << 5) + hash) ^ p3_.GetHashCode();
		}

		public readonly override bool Equals(object other)
		{
			if (other is BezierCurve rhs)
				return (p0_ == rhs.p0_) && (p1_ == rhs.p1_) && (p2_ == rhs.p2_) && (p3_ == rhs.p3_);

			return false;
		}

		public readonly bool Equals(BezierCurve other)
		{
			return (p0_ == other.p0_) && (p1_ == other.p1_) && (p2_ == other.p2_) && (p3_ == other.p3_);
		}

		public readonly bool ApproxEquals(BezierCurve curve, float tolerance)
		{
			return (Math.Abs(curve.p0_ - p0_) < tolerance) && (Math.Abs(curve.p1_ - p1_) < tolerance) &&
				(Math.Abs(curve.p2_ - p2_) < tolerance) && (Math.Abs(curve.p3_ - p3_) < tolerance);
		}

		public readonly bool ApproxEquals(BezierCurve curve)
		{
			return ApproxEquals(curve, 1e-6f);
		}

		public readonly float[] ToArray()
		{
			return new float[4] { p0_, p1_, p2_, p3_ };
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1} {2} {3}", p0_, p1_, p2_, p3_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3}", p0_, p1_, p2_, p3_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1} {2} {3}", 
				p0_.ToString(format), p1_.ToString(format), p2_.ToString(format), p3_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3}",
				p0_.ToString(format, provider), p1_.ToString(format, provider), p2_.ToString(format, provider), p3_.ToString(format, provider));
		}

		public static BezierCurve Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new BezierCurve(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]), Single.Parse(m[3]));
		}

		public static BezierCurve Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new BezierCurve(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
					Single.Parse(m[2], provider), Single.Parse(m[3], provider));
		}

		// #TODO ControlPoints[int index] get/set

		public static bool operator ==(BezierCurve lhs, BezierCurve rhs)
		{
			return (lhs.p0_ == rhs.p0_) && (lhs.p1_ == rhs.p1_) && (lhs.p2_ == rhs.p2_) && (lhs.p3_ == rhs.p3_);
		}

		public static bool operator !=(BezierCurve lhs, BezierCurve rhs)
		{
			return (lhs.p0_ != rhs.p0_) || (lhs.p1_ != rhs.p1_) || (lhs.p2_ != rhs.p2_) || (lhs.p3_ != rhs.p3_);
		}

		public readonly float Evaluate(float t) // #TODO SIMD
		{
			Vector4 bt = GetBasis(t);
			return p0_*bt.x_ + p1_*bt.y_ + p2_*bt.z_ + p3_*bt.w_;
		}

		public readonly float CalculateDerivative(float t) // #TODO SIMD
		{
			Vector4 dbt = GetDerivativeBasis(t);
			return p0_*dbt.x_ + p1_*dbt.y_ + p2_*dbt.z_ + p3_*dbt.w_;
		}

		public readonly float CalculateSpeed(float t)
		{
			return CalculateDerivative(t);
		}

		public readonly float CalculateLength()
		{
			return CalculateLength(1f);
		}

		public readonly float CalculateLength(float t)
		{
			if (t <= 0f)
				return 0f;

			BezierCurve curve = this;
			return Romberg.Estimate(0f, Math.Min(t, 1f), curve.CalculateSpeed, 8);
		}

		public readonly float? CalculateTime(float s, int nIterations)
		{
			if (s <= 0f)
				return 0f;

			float totalLen = CalculateLength(1f);
			if (s >= totalLen)
				return 1f;

			float time = s/totalLen;
			for (int i = 0; i < nIterations; i++)
			{
				float difference = CalculateLength(time) - s;
				if (Math.Abs(difference) < SingleConstants.Tolerance)
					return time;
				time -= difference/CalculateSpeed(time);
			}

			return null;
		}

		internal static Vector4 GetBasis(float t) 
		{
			float t2 = t*t;
			float omt = 1f - t;
			float omt2 = omt*omt;
			return new Vector4(omt*omt2, 3f*t*omt2, 3f*t2*omt, t*t2);
		}

		internal static Vector4 GetDerivativeBasis(float t)
		{
			float t2 = t*t;
			float omt = 1f - t;
			float omt2 = omt*omt;
			return new Vector4((-3f)*omt2, (-6f)*t*omt + 3f*omt2, (-3f)*t2 + 6f*t*omt, 3f*t2);
		}

		internal float p0_;
		internal float p1_;
		internal float p2_;
		internal float p3_;
	}
}
