/*
 *  Name: BezierCurve4
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	/// <summary>
	/// 4D Cubic Bezier Curve.
	/// </summary>
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct BezierCurve4 : IFormattable, IEquatable<BezierCurve4>
	{
		public static readonly BezierCurve4 Zero = new BezierCurve4(Vector4.Zero, Vector4.Zero, Vector4.Zero, Vector4.Zero);

		public BezierCurve4(Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3)
		{
			p0_ = p0;
			p1_ = p1;
			p2_ = p2;
			p3_ = p3;
		}

		public BezierCurve4(Vector4[] points)
		{
			p0_ = points[0];
			p1_ = points[1];
			p2_ = points[2];
			p3_ = points[3];
		}

		//private BezierCurve4(SerializationInfo info, StreamingContext context)
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

		public Vector4 ControlPoint0
		{
			readonly get => p0_;
			set => p0_ = value;
		}

		public Vector4 ControlPoint1
		{
			readonly get => p1_;
			set => p1_ = value;
		}

		public Vector4 ControlPoint2
		{
			readonly get => p2_;
			set => p2_ = value;
		}

		public Vector4 ControlPoint3
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
			if (other is BezierCurve4 rhs)
				return (p0_ == rhs.p0_) && (p1_ == rhs.p1_) && (p2_ == rhs.p2_) && (p3_ == rhs.p3_);

			return false;
		}

		public readonly bool Equals(BezierCurve4 other)
		{
			return (p0_ == other.p0_) && (p1_ == other.p1_) && (p2_ == other.p2_) && (p3_ == other.p3_);
		}

		public readonly bool ApproxEquals(BezierCurve4 curve, float tolerance)
		{
			return p0_.ApproxEquals(curve.p0_, tolerance) && p1_.ApproxEquals(curve.p1_, tolerance) &&
				p2_.ApproxEquals(curve.p2_, tolerance) && p3_.ApproxEquals(curve.p3_, tolerance);
		}

		public readonly bool ApproxEquals(BezierCurve4 curve)
		{
			return ApproxEquals(curve, 1e-6f);
		}

		public readonly Vector4[] ToArray()
		{
			return new Vector4[4] { p0_, p1_, p2_, p3_ };
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

		public static BezierCurve4 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 16)
				throw new FormatException();

			return new BezierCurve4(new Vector4(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]), Single.Parse(m[3])),
				new Vector4(Single.Parse(m[4]), Single.Parse(m[5]), Single.Parse(m[6]), Single.Parse(m[7])),
				new Vector4(Single.Parse(m[8]), Single.Parse(m[9]), Single.Parse(m[10]), Single.Parse(m[11])),
				new Vector4(Single.Parse(m[12]), Single.Parse(m[13]), Single.Parse(m[14]), Single.Parse(m[15])));
		}

		public static BezierCurve4 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 16)
				throw new FormatException();

			return new BezierCurve4(new Vector4(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider), Single.Parse(m[3], provider)),
				new Vector4(Single.Parse(m[4], provider), Single.Parse(m[5], provider), Single.Parse(m[6], provider), Single.Parse(m[7], provider)),
				new Vector4(Single.Parse(m[8], provider), Single.Parse(m[9], provider), Single.Parse(m[10], provider), Single.Parse(m[11], provider)),
				new Vector4(Single.Parse(m[12], provider), Single.Parse(m[13], provider), Single.Parse(m[14], provider), Single.Parse(m[15], provider)));
		}

		// #TODO ControlPoints[int index] get/set
		//public Vector4[] GetControlPoints()
		//{
		//    return new Vector4[4] { p0_, p1_, p2_, p3_ };
		//}

		public static bool operator ==(BezierCurve4 lhs, BezierCurve4 rhs)
		{
			return (lhs.p0_ == rhs.p0_) && (lhs.p1_ == rhs.p1_) && (lhs.p2_ == rhs.p2_) && (lhs.p3_ == rhs.p3_);
		}

		public static bool operator !=(BezierCurve4 lhs, BezierCurve4 rhs)
		{
			return (lhs.p0_ != rhs.p0_) || (lhs.p1_ != rhs.p1_) || (lhs.p2_ != rhs.p2_) || (lhs.p3_ != rhs.p3_);
		}

		public void Translate(Vector4 offset)
		{
			p0_ += offset;
			p1_ += offset;
			p2_ += offset;
			p3_ += offset;
		}

		public static BezierCurve4 Translate(BezierCurve4 curve, Vector4 offset)
		{
			curve.Translate(offset);
			return curve;
		}

		public void Transform(in Matrix4 matrix)
		{
			p0_.Transform(matrix);
			p1_.Transform(matrix);
			p2_.Transform(matrix);
			p3_.Transform(matrix);
		}

		public static BezierCurve4 Transform(BezierCurve4 curve, in Matrix4 matrix)
		{
			curve.Transform(matrix);
			return curve;
		}

		public readonly Vector4 Evaluate(float t) // #TODO SIMD
		{
			Vector4 bt = BezierCurve.GetBasis(t);
			return new Vector4(p0_.x_*bt.x_ + p1_.x_*bt.y_ + p2_.x_*bt.z_ + p3_.x_*bt.w_,
				p0_.y_*bt.x_ + p1_.y_*bt.y_ + p2_.y_*bt.z_ + p3_.y_*bt.w_,
				p0_.z_*bt.x_ + p1_.z_*bt.y_ + p2_.z_*bt.z_ + p3_.z_*bt.w_,
				p0_.w_*bt.x_ + p1_.w_*bt.y_ + p2_.w_*bt.z_ + p3_.w_*bt.w_);
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

		public readonly float EvaluateW(float t) // #TODO SIMD
		{
			Vector4 bt = BezierCurve.GetBasis(t);
			return p0_.w_*bt.x_ + p1_.w_*bt.y_ + p2_.w_*bt.z_ + p3_.w_*bt.w_;
		}

		public readonly Vector4 CalculateDerivative(float t) // #TODO SIMD
		{
			Vector4 dbt = BezierCurve.GetDerivativeBasis(t);
			return new Vector4(p0_.x_*dbt.x_ + p1_.x_*dbt.y_ + p2_.x_*dbt.z_ + p3_.x_*dbt.w_,
				p0_.y_*dbt.x_ + p1_.y_*dbt.y_ + p2_.y_*dbt.z_ + p3_.y_*dbt.w_,
				p0_.z_*dbt.x_ + p1_.z_*dbt.y_ + p2_.z_*dbt.z_ + p3_.z_*dbt.w_,
				p0_.w_*dbt.x_ + p1_.w_*dbt.y_ + p2_.w_*dbt.z_ + p3_.w_*dbt.w_);
		}

		public readonly Vector4 CalculateTangent(float t)
		{
			return Vector4.Normalize(CalculateDerivative(t));
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

			BezierCurve4 curve = this;
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

		internal Vector4 p0_;
		internal Vector4 p1_;
		internal Vector4 p2_;
		internal Vector4 p3_;
	}
}
