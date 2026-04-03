/*
 *  Name: Cone
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Cone : IFormattable, IEquatable<Cone>
	{
		public Cone(Vector3 vertex, Vector3 axis, float height, float radius)
		{
			vertex_ = vertex;
			axis_ = axis;
			height_ = height;
			radius_ = radius;
		}

		//public Cone(Vector3 vertex, Axis axis, float direction, float height, float radius)
		//{
		//    vertex_ = vertex;
		//    axis_ = Vector3.FromAxis(axis)*direction;
		//    height_ = height;
		//    radius_ = radius;
		//}

		public Cone(Vector3 vertex, Vector3 baseCenter, float radius)
		{
			vertex_ = vertex;
			axis_ = Vector3.Normalize(baseCenter - vertex);
			height_ = Vector3.Distance(vertex, baseCenter);
			radius_ = radius;
		}

		public static bool operator ==(Cone lhs, Cone rhs)
		{
			return (lhs.vertex_ == rhs.vertex_) && (lhs.axis_ == rhs.axis_) && (lhs.height_ == rhs.height_) && (lhs.radius_ == rhs.radius_);
		}

		public static bool operator !=(Cone lhs, Cone rhs)
		{
			return (lhs.vertex_ != rhs.vertex_) || (lhs.axis_ != rhs.axis_) || (lhs.height_ != rhs.height_) || (lhs.radius_ != rhs.radius_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Cone rhs)
				return (vertex_ == rhs.vertex_) && (axis_ == rhs.axis_) && (height_ == rhs.height_) && (radius_ == rhs.radius_);

			return false;
		}

		public readonly bool Equals(Cone other)
		{
			return (vertex_ == other.vertex_) && (axis_ == other.axis_) && (height_ == other.height_) && (radius_ == other.radius_);
		}

		public readonly override int GetHashCode()
		{
			int hash = vertex_.GetHashCode();
			hash = ((hash << 5) + hash) ^ axis_.GetHashCode();
			hash = ((hash << 5) + hash) ^ height_.GetHashCode();
			return ((hash << 5) + hash) ^ radius_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Cone other, float tolerance)
		{
			return vertex_.ApproxEquals(other.vertex_, tolerance) &&
				axis_.ApproxEquals(other.axis_, tolerance) &&
				(Math.Abs(other.height_ - height_) < tolerance) &&
				(Math.Abs(other.radius_ - radius_) < tolerance);
		}

		public readonly bool ApproxEquals(in Cone other)
		{
			return vertex_.ApproxEquals(other.vertex_) &&
				axis_.ApproxEquals(other.axis_) &&
				(Math.Abs(other.height_ - height_) < 1e-6f) &&
				(Math.Abs(other.radius_ - radius_) < 1e-6f);
		}

		public readonly override string ToString()
		{
			return String.Concat(vertex_.ToString(), " ", axis_.ToString(), " ", height_.ToString(), " ", radius_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(vertex_.ToString(provider), " ", axis_.ToString(provider), " ", height_.ToString(provider), " ", radius_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(vertex_.ToString(format), " ", axis_.ToString(format), " ", height_.ToString(format), " ", radius_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(vertex_.ToString(format, provider), " ", axis_.ToString(format, provider), " ", height_.ToString(format, provider), " ", radius_.ToString(format, provider));
		}

		public static Cone Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 8)
				throw new FormatException();

			return new Cone(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
				new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])),
				Single.Parse(m[6]), Single.Parse(m[7]));
		}

		public static Cone Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 8)
				throw new FormatException();

			return new Cone(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
				new Vector3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider)),
				Single.Parse(m[6], provider), Single.Parse(m[7], provider));
		}

		[Browsable(false)]
		public readonly bool IsFinite
		{
			get => vertex_.IsFinite && axis_.IsFinite && Scalar.IsFinite(height_) && Scalar.IsFinite(radius_);
		}

		public Vector3 Vertex
		{
			readonly get => vertex_;
			set => vertex_ = value;
		}

		public Vector3 Axis
		{
			readonly get => axis_;
			set => axis_ = value;
		}

		public float Height
		{
			readonly get => height_;
			set => height_ = value;
		}

		[Browsable(false)]
		public readonly float HalfHeight => height_*0.5f;

		[Browsable(false)]
		public Vector3 BaseCenter
		{
			readonly get => vertex_ + height_*axis_;
			set
			{
				axis_ = Vector3.Normalize(value - vertex_);
				height_ = Vector3.Distance(vertex_, value);
			}
		}

		public float BaseRadius
		{
			readonly get => radius_;
			set => radius_ = value;
		}

		[Browsable(false)]
		public float HalfAngle
		{
			readonly get => (float)Math.Atan(radius_/Height);
			set => radius_ = height_*(float)Math.Tan(value);
		}

		[Browsable(false)]
		public float Angle
		{
			readonly get => HalfAngle*2f;
			set => HalfAngle = value*0.5f;
		}

		[Browsable(false)]
		public readonly float SolidAngle => SingleConstants.TwoPi*(1f - (float)Math.Cos(HalfAngle));

		[Browsable(false)]
		public readonly float SlantHeight => MathF.Sqrt(radius_*radius_ + height_*height_);

		[Browsable(false)]
		public readonly float SurfaceArea => SingleConstants.Pi*radius_*(radius_ + MathF.Sqrt(radius_*radius_ + height_*height_));

		[Browsable(false)]
		public readonly float Volume => SingleConstants.Pi*radius_*radius_*height_/3f;

		public readonly OrientedBox GetCircumscribedBox()
		{
			Matrix3 matrix = Matrix3.FromForward(-axis_);
			Vector3 center = vertex_ + (height_*0.5f)*axis_;
			return new OrientedBox(center, new Matrix3(matrix[0], matrix[2], -matrix[1]), new Vector3(radius_, height_*0.5f, radius_));
		}

		public readonly Sphere GetCircumscribedSphere()
		{
			if (height_ > radius_)
			{
				float slantSquared = radius_*radius_ + height_*height_;
				float sphereRadius = slantSquared/(2f*height_);
				return new Sphere(vertex_ + sphereRadius*axis_, sphereRadius);
			}
			else
			{
				return new Sphere(vertex_ + height_*axis_, radius_);
			}
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

		public static Cone Normalize(Cone cone)
		{
			cone.Normalize();
			return cone;
		}

		public void Translate(Vector3 offset)
		{
			vertex_ += offset;
		}

		public static Cone Translate(Cone cone, Vector3 offset)
		{
			cone.Translate(offset);
			return cone;
		}

		public readonly bool Contains(Vector3 point)
		{
			return Containment.TestConePoint(vertex_, axis_, height_, radius_, point);
		}

		public readonly bool Intersects(in Sphere sphere)
		{
			return Intersections.TestConeSphere(vertex_, axis_, height_, radius_, sphere.center_, sphere.radius_);
		}

		internal Vector3 vertex_;
		internal Vector3 axis_;
		internal float height_;
		internal float radius_;
	}
}
