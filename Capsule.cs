/*
 *  Name: Capsule
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Capsule : IFormattable, IEquatable<Capsule>
	{
        public Capsule(Vector3 center, Vector3 axis, float height, float radius)
        {
            center_ = center;
            axis_ = axis;
			height_ = height;
            radius_ = radius;
        }

		public Capsule(Vector3 center, Axis axis, float height, float radius)
		{
			center_ = center;
			axis_ = Vector3.FromAxis(axis);
			height_ = height;
			radius_ = radius;
		}

		public Capsule(Vector3 point0, Vector3 point1, float radius)
		{
			center_ = (point0 + point1)*0.5f;
			axis_ = Vector3.Normalize(point1 - point0);
			height_ = Vector3.Distance(point0, point1);
			radius_ = radius;
		}

		public static bool operator ==(Capsule lhs, Capsule rhs)
		{
			return (lhs.center_ == rhs.center_) && (lhs.axis_ == rhs.axis_) && (lhs.height_ == rhs.height_) && (lhs.radius_ == rhs.radius_);
		}

		public static bool operator !=(Capsule lhs, Capsule rhs)
		{
			return (lhs.center_ != rhs.center_) || (lhs.axis_ != rhs.axis_) || (lhs.height_ != rhs.height_) || (lhs.radius_ != rhs.radius_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Capsule rhs)
				return (center_ == rhs.center_) && (axis_ == rhs.axis_) && (height_ == rhs.height_) && (radius_ == rhs.radius_);

			return false;
		}

		public readonly bool Equals(Capsule other)
		{
			return (center_ == other.center_) && (axis_ == other.axis_) && (height_ == other.height_) && (radius_ == other.radius_);
		}

		public readonly override int GetHashCode()
		{
			int hash = center_.GetHashCode();
			hash = ((hash << 5) + hash) ^ axis_.GetHashCode();
			hash = ((hash << 5) + hash) ^ height_.GetHashCode();
			return ((hash << 5) + hash) ^ radius_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Capsule other, float tolerance)
		{
			return center_.ApproxEquals(other.center_, tolerance) &&
				axis_.ApproxEquals(other.axis_, tolerance) &&
				(Math.Abs(other.height_ - height_) < tolerance) &&
				(Math.Abs(other.radius_ - radius_) < tolerance);
		}

		public readonly bool ApproxEquals(in Capsule other)
		{
			return center_.ApproxEquals(other.center_) &&
				axis_.ApproxEquals(other.axis_) &&
				(Math.Abs(other.height_ - height_) < 1e-6f) &&
				(Math.Abs(other.radius_ - radius_) < 1e-6f);
		}

		public readonly override string ToString()
		{
			return String.Concat(center_.ToString(), " ", axis_.ToString(), " ", height_.ToString(), " ", radius_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(center_.ToString(provider), " ", axis_.ToString(provider), " ", height_.ToString(provider), " ", radius_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(center_.ToString(format), " ", axis_.ToString(format), " ", height_.ToString(format), " ", radius_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(center_.ToString(format, provider), " ", axis_.ToString(format, provider), " ", height_.ToString(format, provider), " ", radius_.ToString(format, provider));
		}

		public static Capsule Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 8)
				throw new FormatException();

			return new Capsule(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
				new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])),
				Single.Parse(m[6]), Single.Parse(m[7]));
		}

		public static Capsule Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 8)
				throw new FormatException();

			return new Capsule(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
				new Vector3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider)),
				Single.Parse(m[6], provider), Single.Parse(m[7], provider));
		}

		[Browsable(false)]
		public readonly bool IsFinite
		{
			get => center_.IsFinite && axis_.IsFinite && Scalar.IsFinite(height_) && Scalar.IsFinite(radius_);
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

		public float Height/*CylinderHeight*/
		{
			readonly get => height_;
			set => height_ = value;
		}

		[Browsable(false)]
		public readonly float HalfHeight => height_*0.5f;

		[Browsable(false)]
		public float TotalHeight
		{
			readonly get => height_ + radius_*2f;
			set => height_ = value - radius_*2f;
		}

        public float Radius
        {
			readonly get => radius_;
            set => radius_ = value;
        }

		[Browsable(false)]
		public readonly float SurfaceArea => SingleConstants.TwoPi*radius_*(2f*radius_ + height_);

		[Browsable(false)]
		public readonly float Volume => SingleConstants.Pi*radius_*radius_*(4f*radius_/3f + height_);

		public readonly OrientedBox GetCircumscribedBox()
		{
			Matrix3 matrix = Matrix3.FromForward(axis_);
			return new OrientedBox(Center, new Matrix3(matrix[0], matrix[2], -matrix[1]), 
				new Vector3(radius_, height_*0.5f + radius_, radius_));
		}

        public void Normalize()
        {
			float m = axis_.Magnitude;
			if (m > 0f)
			{
				axis_ /= m;
				height_ *= m;
			} 
        }

		public static Capsule Normalize(Capsule capsule)
		{
			capsule.Normalize();
			return capsule;
		}

		public void Translate(Vector3 offset)
		{
			center_ += offset;
		}

		public static Capsule Translate(Capsule capsule, Vector3 offset)
		{
			capsule.Translate(offset);
			return capsule;
		}

		internal Vector3 center_;
		internal Vector3 axis_;
		internal float height_;
		internal float radius_;
	}
}
