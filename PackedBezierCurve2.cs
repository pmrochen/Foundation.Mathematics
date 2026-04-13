/*
 *  Name: PackedBezierCurve2
 *  Description:
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	/// <summary>
	/// 2D Cubic Bezier Curve where P0 is (0, 0) and P3 is (1, 1).
	/// </summary>
	//[Serializable]
	[TypeConverter(typeof(PackedBezierCurve2Converter))]
	public struct PackedBezierCurve2 : IFormattable, IEquatable<PackedBezierCurve2>
	{
		public static readonly PackedBezierCurve2 Zero = new PackedBezierCurve2(Vector2.Zero, Vector2.Zero);
		public static readonly PackedBezierCurve2 Linear/*Identity*/ = new PackedBezierCurve2(Vector2.Zero, new Vector2(1f));

		public PackedBezierCurve2(Vector2 p1, Vector2 p2)
		{
			p1_ = p1;
			p2_ = p2;
		}

		public PackedBezierCurve2(Vector2[] points)
		{
			p1_ = points[0];
			p2_ = points[1];
		}

		//private PackedBezierCurve2(SerializationInfo info, StreamingContext context)
		//{
		//    p1_ = info.GetSingle("P1");
		//    p2_ = info.GetSingle("P2");
		//}

		//void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		//{
		//    info.AddValue("P1", p1_);
		//    info.AddValue("P2", p2_);
		//}

		[Browsable(false)]
		public readonly Vector2 ControlPoint0 => Vector2.Zero;

		public Vector2 ControlPoint1
		{
			readonly get => p1_;
			set => p1_ = value;
		}

		public Vector2 ControlPoint2
		{
			readonly get => p2_;
			set => p2_ = value;
		}

		[Browsable(false)]
		public readonly Vector2 ControlPoint3 => new Vector2(1f);

		public readonly override int GetHashCode()
		{
			int hash = p1_.GetHashCode();
			return ((hash << 5) + hash) ^ p2_.GetHashCode();
		}

		public readonly override bool Equals(object other)
		{
			if (other is PackedBezierCurve2 rhs)
				return (p1_ == rhs.p1_) && (p2_ == rhs.p2_);

			return false;
		}

		public readonly bool Equals(PackedBezierCurve2 other)
		{
			return (p1_ == other.p1_) && (p2_ == other.p2_);
		}

		public readonly bool ApproxEquals(PackedBezierCurve2 curve, float tolerance)
		{
			return p1_.ApproxEquals(curve.p1_, tolerance) && p2_.ApproxEquals(curve.p2_, tolerance);
		}

		public readonly bool ApproxEquals(PackedBezierCurve2 curve)
		{
			return ApproxEquals(curve, 1e-6f);
		}

		public readonly Vector2[] ToArray()
		{
			return new Vector2[2] { p1_, p2_ };
		}

		public readonly override string ToString()
		{
			return String.Concat(p1_.ToString(), " ", p2_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(p1_.ToString(provider), " ", p2_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(p1_.ToString(format), " ", p2_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(p1_.ToString(format, provider), " ", p2_.ToString(format, provider));
		}

		public static PackedBezierCurve2 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new PackedBezierCurve2(new Vector2(Single.Parse(m[0]), Single.Parse(m[1])),
				new Vector2(Single.Parse(m[2]), Single.Parse(m[3])));
		}

		public static PackedBezierCurve2 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new PackedBezierCurve2(new Vector2(Single.Parse(m[0], provider), Single.Parse(m[1], provider)),
				new Vector2(Single.Parse(m[2], provider), Single.Parse(m[3], provider)));
		}

		public static explicit operator PackedBezierCurve2(in BezierCurve2 curve)
		{
			return FromBezierCurve(curve);
		}

		// #TODO ControlPoints[int index] get/set
		//public Vector2[] GetControlPoints()
		//{
		//    return new Vector2[4] { p0_, p1_, p2_, p3_ };
		//}

		public static bool operator ==(PackedBezierCurve2 lhs, PackedBezierCurve2 rhs)
		{
			return (lhs.p1_ == rhs.p1_) && (lhs.p2_ == rhs.p2_);
		}

		public static bool operator !=(PackedBezierCurve2 lhs, PackedBezierCurve2 rhs)
		{
			return (lhs.p1_ != rhs.p1_) || (lhs.p2_ != rhs.p2_);
		}

		public static PackedBezierCurve2 FromBezierCurve(BezierCurve2 curve)
		{
			if ((curve.p0_ != Vector2.Zero) || (curve.p1_ != new Vector2(1f)))
				throw new ArgumentException();
			
			return new PackedBezierCurve2(curve.p1_, curve.p2_);
		}

		public readonly Vector2 Evaluate(float t) // #TODO SIMD
		{
			Vector4 bt = BezierCurve.GetBasis(t);
			return new Vector2(p1_.x_*bt.y_ + p2_.x_*bt.z_ + bt.w_, p1_.y_*bt.y_ + p2_.y_*bt.z_ + bt.w_);
		}

		public readonly float EvaluateX(float t) // #TODO SIMD
		{
			Vector4 bt = BezierCurve.GetBasis(t);
			return p1_.x_*bt.y_ + p2_.x_*bt.z_ + bt.w_;
		}

		public readonly float EvaluateY(float t) // #TODO SIMD
		{
			Vector4 bt = BezierCurve.GetBasis(t);
			return p1_.y_*bt.y_ + p2_.y_*bt.z_ + bt.w_;
		}

		public readonly Vector2 CalculateDerivative(float t) // #TODO SIMD
		{
			Vector4 dbt = BezierCurve.GetDerivativeBasis(t);
			return new Vector2(p1_.x_*dbt.y_ + p2_.x_*dbt.z_ + dbt.w_, p1_.y_*dbt.y_ + p2_.y_*dbt.z_ + dbt.w_);
		}

		private readonly float CalculateDerivativeX(float t) // #TODO SIMD
		{
			Vector4 dbt = BezierCurve.GetDerivativeBasis(t);
			return p1_.x_*dbt.y_ + p2_.x_*dbt.z_ + dbt.w_;
		}

		public readonly Vector2 CalculateTangent(float t)
		{
			return Vector2.Normalize(CalculateDerivative(t));
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

			PackedBezierCurve2 curve = this;
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

		public readonly float FindYAtX(float x)
		{
			return EvaluateY(FindTAtX(x));
		}

		private readonly float FindTAtX(float x)
		{
			// https://github.com/gre/bezier-easing/blob/master/src/index.js

			const int nSteps = 20/*10*/;
			const float stepSize = 1f/(float)nSteps;

			int currentSample = 1;
			for (; (currentSample < nSteps) && (EvaluateX(currentSample*stepSize) <= x); currentSample++)
				;
			currentSample--;
			
			float intervalStart = currentSample*stepSize;
			float currentValue = EvaluateX(currentSample*stepSize);
			float dist = (x - currentValue)/(EvaluateX((currentSample + 1)*stepSize) - currentValue);
			float guessForT = intervalStart + dist*stepSize;

			const float minSlope = 0.001f;
			float initialSlope = CalculateDerivativeX(guessForT);
			if (initialSlope >= minSlope)
				return NewtonRaphsonIterate(x, guessForT);
			else if (Math.Abs(initialSlope) < SingleConstants.Tolerance)
				return guessForT;
			else
				return BinarySubdivide(x, intervalStart, intervalStart + stepSize);
		}

		private readonly float BinarySubdivide(float x, float a, float b)
		{
			const float precision = SingleConstants.Tolerance;
			const int nIterationsMax = 10;

			float currentX, currentT;
			int i = 0;
			do
			{
				currentT = a + (b - a)*0.5f;
				currentX = EvaluateX(currentT) - x;
				if (currentX > 0.0)
					b = currentT;
				else
					a = currentT;
			} while ((Math.Abs(currentX) > precision) && (++i < nIterationsMax));

			return currentT;
		}

		private readonly float NewtonRaphsonIterate(float x, float t)
		{
			const int nIterations = 6/*4*/;

			for (int i = 0; i < nIterations; i++)
			{
				float currentSlope = CalculateDerivativeX(t);
				if (Math.Abs(currentSlope) < SingleConstants.Tolerance)
					return t;
				
				float currentX = EvaluateX(t) - x;
				t -= currentX/currentSlope;
			}

			return t;
		}

		internal Vector2 p1_;
		internal Vector2 p2_;
	}
}
