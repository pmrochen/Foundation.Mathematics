/*
 *  Name: Torus
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Torus : IFormattable, IEquatable<Torus>
	{
        public Torus(Vector3 center, Vector3 axis, float majorRadius, float minorRadius)
        {
            center_ = center;
            axis_ = axis;
			majorRadius_ = majorRadius;
            minorRadius_ = minorRadius;
        }

		public Torus(Vector3 center, Axis axis, float majorRadius, float minorRadius)
		{
			center_ = center;
			axis_ = Vector3.FromAxis(axis);
			majorRadius_ = majorRadius;
			minorRadius_ = minorRadius;
		}

		public static bool operator ==(Torus lhs, Torus rhs)
		{
			return (lhs.center_ == rhs.center_) && (lhs.axis_ == rhs.axis_) && (lhs.majorRadius_ == rhs.majorRadius_) && 
				(lhs.minorRadius_ == rhs.minorRadius_);
		}

		public static bool operator !=(Torus lhs, Torus rhs)
		{
			return (lhs.center_ != rhs.center_) || (lhs.axis_ != rhs.axis_) || (lhs.majorRadius_ != rhs.majorRadius_) || 
				(lhs.minorRadius_ != rhs.minorRadius_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Torus rhs)
			{
				return (center_ == rhs.center_) && (axis_ == rhs.axis_) && (majorRadius_ == rhs.majorRadius_) && 
					(minorRadius_ == rhs.minorRadius_);
			}

			return false;
		}

		public readonly bool Equals(Torus other)
		{
			return (center_ == other.center_) && (axis_ == other.axis_) && (majorRadius_ == other.majorRadius_) && 
				(minorRadius_ == other.minorRadius_);
		}

		public readonly override int GetHashCode()
		{
			int hash = center_.GetHashCode();
			hash = ((hash << 5) + hash) ^ axis_.GetHashCode();
			hash = ((hash << 5) + hash) ^ majorRadius_.GetHashCode();
			return ((hash << 5) + hash) ^ minorRadius_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Torus other, float tolerance)
		{
			return center_.ApproxEquals(other.center_, tolerance) &&
				axis_.ApproxEquals(other.axis_, tolerance) &&
				(Math.Abs(other.majorRadius_ - majorRadius_) < tolerance) &&
				(Math.Abs(other.minorRadius_ - minorRadius_) < tolerance);
		}

		public readonly bool ApproxEquals(in Torus other)
		{
			return center_.ApproxEquals(other.center_) &&
				axis_.ApproxEquals(other.axis_) &&
				(Math.Abs(other.majorRadius_ - majorRadius_) < 1e-6f) &&
				(Math.Abs(other.minorRadius_ - minorRadius_) < 1e-6f);
		}

		public readonly override string ToString()
		{
			return String.Concat(center_.ToString(), " ", axis_.ToString(), " ", majorRadius_.ToString(), " ", minorRadius_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(center_.ToString(provider), " ", axis_.ToString(provider), " ", majorRadius_.ToString(provider), " ", minorRadius_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(center_.ToString(format), " ", axis_.ToString(format), " ", majorRadius_.ToString(format), " ", minorRadius_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(center_.ToString(format, provider), " ", axis_.ToString(format, provider), " ", majorRadius_.ToString(format, provider), " ", minorRadius_.ToString(format, provider));
		}

		public static Torus Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 8)
				throw new FormatException();

			return new Torus(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
				new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])),
				Single.Parse(m[6]), Single.Parse(m[7]));
		}

		public static Torus Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 8)
				throw new FormatException();

			return new Torus(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
				new Vector3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider)),
				Single.Parse(m[6], provider), Single.Parse(m[7], provider));
		}

		[Browsable(false)]
		public readonly bool IsFinite
		{
			get => center_.IsFinite && axis_.IsFinite && Functions.IsFinite(majorRadius_) && Functions.IsFinite(minorRadius_);
		}

		public Vector3 Center
        {
			readonly get => center_;
            set => center_ = value;
        }

        public Vector3 Axis
        {
			readonly get => axis_;
            set => axis_ = value;
        }

		public float MajorRadius
		{
			readonly get => majorRadius_;
			set => majorRadius_ = value;
		}

        public float MinorRadius
        {
			readonly get => minorRadius_;
            set => minorRadius_ = value;
        }

		[Browsable(false)]
		public readonly float InnerRadius => majorRadius_ - minorRadius_;

		[Browsable(false)]
		public readonly float OuterRadius => majorRadius_ + minorRadius_;

		[Browsable(false)]
		public readonly float AspectRatio => majorRadius_/minorRadius_;

		[Browsable(false)]
		public readonly float SurfaceArea => 4f*SingleConstants.PiSquared*majorRadius_*minorRadius_;

		[Browsable(false)]
		public readonly float Volume => 2f*SingleConstants.PiSquared*majorRadius_*minorRadius_*minorRadius_;

		public readonly OrientedBox GetCircumscribedBox()
		{
			Matrix3 m = Matrix3.FromForward(axis_);
			return new OrientedBox(Center, new Matrix3(m[0], m[2], -m[1]), 
				new Vector3(majorRadius_ + minorRadius_, minorRadius_, majorRadius_ + minorRadius_));
		}

		public void Translate(Vector3 offset)
		{
			center_ += offset;
		}

		public static Torus Translate(Torus torus, Vector3 offset)
		{
			torus.Translate(offset);
			return torus;
		}

		internal Vector3 center_;
		internal Vector3 axis_;
		internal float majorRadius_;
		internal float minorRadius_;
	}
}
