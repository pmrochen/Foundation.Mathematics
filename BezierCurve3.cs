/*
 *  Name: BezierCurve3
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	/// <summary>
	/// 3D Cubic Bezier Curve.
	/// </summary>
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct BezierCurve3 : IFormattable, IEquatable<BezierCurve3>
	{
		public static readonly BezierCurve3 Zero = new BezierCurve3(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);

		public BezierCurve3(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			p0_ = p0;
			p1_ = p1;
			p2_ = p2;
			p3_ = p3;
		}

		public BezierCurve3(Vector3[] points)
		{
			p0_ = points[0];
			p1_ = points[1];
			p2_ = points[2];
			p3_ = points[3];
		}

		//private BezierCurve3(SerializationInfo info, StreamingContext context)
		//{
		//    p0_ = info.GetSingle("P0");
		//    p1_ = info.GetSingle("P1");
		//    p2_ = info.GetSingle("P2");
		//    p3_ = info.GetSingle("P3");
		//}

		//void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		//{
		//    info.AddValue("P0", p0_);
		//    info.AddValue("P1", p1_);
		//    info.AddValue("P2", p2_);
		//    info.AddValue("P3", p3_);
		//}

		public Vector3 ControlPoint0
		{
			readonly get => p0_;
			set => p0_ = value;
		}

		public Vector3 ControlPoint1
		{
			readonly get => p1_;
			set => p1_ = value;
		}

		public Vector3 ControlPoint2
		{
			readonly get => p2_;
			set => p2_ = value;
		}

		public Vector3 ControlPoint3
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
			if (other is BezierCurve3 rhs)
				return (p0_ == rhs.p0_) && (p1_ == rhs.p1_) && (p2_ == rhs.p2_) && (p3_ == rhs.p3_);

			return false;
		}

		public readonly bool Equals(BezierCurve3 other)
		{
			return (p0_ == other.p0_) && (p1_ == other.p1_) && (p2_ == other.p2_) && (p3_ == other.p3_);
		}

		public readonly bool ApproxEquals(BezierCurve3 curve, float tolerance)
		{
			return p0_.ApproxEquals(curve.p0_, tolerance) && p1_.ApproxEquals(curve.p1_, tolerance) &&
				p2_.ApproxEquals(curve.p2_, tolerance) && p3_.ApproxEquals(curve.p3_, tolerance);
		}

		public readonly bool ApproxEquals(BezierCurve3 curve)
		{
			return ApproxEquals(curve, 1e-6f);
		}

		public readonly Vector3[] ToArray()
		{
			return new Vector3[4] { p0_, p1_, p2_, p3_ };
		}

		public readonly override string ToString()
		{
			return String.Concat(p0_.ToString(), " ", p1_.ToString(), " ", p2_.ToString(), " ", p3_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(p0_.ToString(provider), " ", p1_.ToString(provider), " ", p2_.ToString(provider), " ", p3_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(p0_.ToString(format), " ", p1_.ToString(format), " ", p2_.ToString(format), " ", p3_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(p0_.ToString(format, provider), " ", p1_.ToString(format, provider), " ", p2_.ToString(format, provider), " ", p3_.ToString(format, provider));
		}

		public static BezierCurve3 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 12)
				throw new FormatException();

			return new BezierCurve3(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
				new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])),
				new Vector3(Single.Parse(m[6]), Single.Parse(m[7]), Single.Parse(m[8])),
				new Vector3(Single.Parse(m[9]), Single.Parse(m[10]), Single.Parse(m[11])));
		}

		public static BezierCurve3 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 12)
				throw new FormatException();

			return new BezierCurve3(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
				new Vector3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider)),
				new Vector3(Single.Parse(m[6], provider), Single.Parse(m[7], provider), Single.Parse(m[8], provider)),
				new Vector3(Single.Parse(m[9], provider), Single.Parse(m[10], provider), Single.Parse(m[11], provider)));
		}

		// #TODO ControlPoints[int index] get/set
		//public Vector3[] GetControlPoints()
		//{
		//    return new Vector3[4] { p0_, p1_, p2_, p3_ };
		//}

		public static bool operator ==(BezierCurve3 lhs, BezierCurve3 rhs)
		{
			return (lhs.p0_ == rhs.p0_) && (lhs.p1_ == rhs.p1_) && (lhs.p2_ == rhs.p2_) && (lhs.p3_ == rhs.p3_);
		}

		public static bool operator !=(BezierCurve3 lhs, BezierCurve3 rhs)
		{
			return (lhs.p0_ != rhs.p0_) || (lhs.p1_ != rhs.p1_) || (lhs.p2_ != rhs.p2_) || (lhs.p3_ != rhs.p3_);
		}

		public readonly Vector3 Evaluate(float t) // #TODO SIMD
		{
			Vector4 bt = BezierCurve.GetBasis(t);
			return new Vector3(p0_.x_*bt.x_ + p1_.x_*bt.y_ + p2_.x_*bt.z_ + p3_.x_*bt.w_,
				p0_.y_*bt.x_ + p1_.y_*bt.y_ + p2_.y_*bt.z_ + p3_.y_*bt.w_,
				p0_.z_*bt.x_ + p1_.z_*bt.y_ + p2_.z_*bt.z_ + p3_.z_*bt.w_);
		}

		public readonly float EvaluateX(float t) // #TODO SIMD
		{
			Vector4 bt = BezierCurve.GetBasis(t);
			return p0_.x_*bt.x_ + p1_.x_*bt.y_ + p2_.x_*bt.z_ + p3_.x_*bt.w_;
		}

		public readonly float EvaluateY(float t) // #TODO SIMD
		{
			Vector4 bt = BezierCurve.GetBasis(t);
			return p0_.y_*bt.x_ + p1_.y_*bt.y_ + p2_.y_*bt.z_ + p3_.y_*bt.w_;
		}

		public readonly float EvaluateZ(float t) // #TODO SIMD
		{
			Vector4 bt = BezierCurve.GetBasis(t);
			return p0_.z_*bt.x_ + p1_.z_*bt.y_ + p2_.z_*bt.z_ + p3_.z_*bt.w_;
		}

		public readonly Vector3 CalculateDerivative(float t) // #TODO SIMD
		{
			Vector4 dbt = BezierCurve.GetDerivativeBasis(t);
			return new Vector3(p0_.x_*dbt.x_ + p1_.x_*dbt.y_ + p2_.x_*dbt.z_ + p3_.x_*dbt.w_,
				p0_.y_*dbt.x_ + p1_.y_*dbt.y_ + p2_.y_*dbt.z_ + p3_.y_*dbt.w_,
				p0_.z_*dbt.x_ + p1_.z_*dbt.y_ + p2_.z_*dbt.z_ + p3_.z_*dbt.w_);
		}

		public readonly Vector3 CalculateTangent(float t)
		{
			return Vector3.Normalize(CalculateDerivative(t));
		}

		public readonly float CalculateSpeed(float t)
		{
			return CalculateDerivative(t).Magnitude;
		}

		public readonly float CalculateLength()
		{
			return CalculateLength(1f);
		}

		public readonly float CalculateLength(float t)
		{
			if (t <= 0f) 
				return 0f;

			BezierCurve3 curve = this;
			return Romberg.Estimate(0f, Math.Min(t, 1f), delegate(float x) { return curve.CalculateSpeed(x); }, 8);
		}

		public readonly float CalculateTime(float s, int nIterations)
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
				if (Math.Abs(difference) < 1e-6f) 
					return time;
				time -= difference/CalculateSpeed(time);
			}

			return Single.PositiveInfinity;
		}

		internal Vector3 p0_;
		internal Vector3 p1_;
		internal Vector3 p2_;
		internal Vector3 p3_;
	}
}
