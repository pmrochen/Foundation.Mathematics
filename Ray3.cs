/*
 *  Name: Ray3
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Ray3 : IFormattable, IEquatable<Ray3>
	{
        public Ray3(Vector3 origin, Vector3 direction)
        {
            origin_ = origin;
            direction_ = direction;
        }

		public static bool operator ==(Ray3 lhs, Ray3 rhs)
		{
			return (lhs.origin_ == rhs.origin_) && (lhs.direction_ == rhs.direction_);
		}

		public static bool operator !=(Ray3 lhs, Ray3 rhs)
		{
			return (lhs.origin_ != rhs.origin_) || (lhs.direction_ != rhs.direction_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Ray3 rhs)
				return (origin_ == rhs.origin_) && (direction_ == rhs.direction_);

			return false;
		}

		public readonly bool Equals(Ray3 other)
		{
			return (origin_ == other.origin_) && (direction_ == other.direction_);
		}

		public readonly override int GetHashCode()
		{
			int hash = origin_.GetHashCode();
			return ((hash << 5) + hash) ^ direction_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Ray3 other, float tolerance)
		{
			return origin_.ApproxEquals(other.origin_, tolerance) &&
				direction_.ApproxEquals(other.direction_, tolerance);
		}

		public readonly bool ApproxEquals(in Ray3 other)
		{
			return origin_.ApproxEquals(other.origin_) &&
				direction_.ApproxEquals(other.direction_);
		}

		public readonly override string ToString()
		{
			return String.Concat(origin_.ToString(), " ", direction_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(origin_.ToString(provider), " ", direction_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(origin_.ToString(format), " ", direction_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(origin_.ToString(format, provider), " ", direction_.ToString(format, provider));
		}

		public static Ray3 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new Ray3(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
					new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])));
		}

		public static Ray3 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new Ray3(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
					new Vector3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider)));
		}

		public static Ray3 FromLine(in Line3 l)
		{
			return new Ray3(l.origin_, l.direction_);
		}

		//public static Ray3 FromSegment(in Segment3 s)
		//{
		//	return new Ray3(s.Start, s.Direction);
		//}

		//public static implicit operator Ray3(Line3 l)
		//{
		//    return FromLine(l);
		//}

		public Vector3 Origin
        {
			readonly get => origin_;
            set => origin_ = value;
        }

        public Vector3 Direction
        {
			readonly get => direction_;
            set => direction_ = value;
        }

        public void Normalize()
        {
            direction_.Normalize();
        }

		public static Ray3 Normalize(Ray3 r)
		{
			r.Normalize();
			return r;
		}

        public void Transform(in AffineTransform t)
        {
			origin_.Transform(t);
			direction_.Transform(t.r_);
        }

		public static Ray3 Transform(Ray3 r, in AffineTransform t)
		{
			r.Transform(t);
			return r;
		}

		public readonly Vector3 Evaluate(float t)
		{
			return (origin_ + t*direction_);
		}

		public readonly Vector3 GetClosestPoint(Vector3 point)
		{
			return Math.Max(Vector3.Dot(point - origin_, direction_), 0f)*direction_ + origin_;
		}

		public readonly Vector3 GetClosestPoint(Vector3 point, bool normalized)
		{
			return normalized ?
				Math.Max(Vector3.Dot(point - origin_, direction_), 0f)*direction_ + origin_ :
				Math.Max(Vector3.Dot(point - origin_, direction_)/Vector3.Dot(direction_, direction_), 0f)*direction_ + origin_;
		}

		public readonly float GetDistanceTo(Vector3 point)
		{
			return Vector3.Distance(GetClosestPoint(point), point);
		}

		public readonly float GetDistanceTo(Vector3 point, bool normalized)
		{
			return Vector3.Distance(GetClosestPoint(point, normalized), point);
		}

		public readonly bool Intersects(in Plane plane)
		{
			return FindIntersection(plane).HasValue;
		}

		public readonly bool Intersects(in Triangle3 triangle)
		{
			return FindIntersection(triangle).HasValue;
		}

		public readonly bool Intersects(in AxisAlignedBox box)
		{
			return FindIntersection(box).HasValue;
		}

		public readonly bool Intersects(in OrientedBox box)
		{
			return FindIntersection(box).HasValue;
		}

		public readonly bool Intersects(in Sphere sphere)
		{
			return FindIntersection(sphere).HasValue;
		}

		public readonly bool Intersects(in Ellipsoid ellipsoid)
		{
			Matrix3 m = ellipsoid.Matrix;
			Vector3 diff = origin_ - ellipsoid.Center;
			Vector3 matDir = direction_*m;
			Vector3 matDiff = diff*m;
			float a2 = Vector3.Dot(direction_, matDir);
			float a1 = Vector3.Dot(direction_, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			float discr = a1*a1 - a0*a2;
			if (discr < 0f)
				return false;
			if (a0 <= 0f)
				return true;

			return (a1 < 0f);
		}

		public readonly float? FindIntersection(in Plane plane)
		{
			float? t = new Line3(origin_, direction_).FindIntersection(plane);
			return (t.HasValue && (t.Value >= 0f)) ? t : null;
		}

		public readonly float? FindIntersection(in Triangle3 triangle)
		{
			// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/

			Vector3 edge1 = triangle.vertex1_ - triangle.vertex0_;
			Vector3 edge2 = triangle.vertex2_ - triangle.vertex0_;
			Vector3 pvec = Vector3.Cross(direction_, edge2);
			float det = Vector3.Dot(edge1, pvec);
			if (Math.Abs(det) < 1e-6f)
				return null;

			float invDet = 1f/det;
			Vector3 tvec = origin_ - triangle.vertex0_;
			float u = Vector3.Dot(tvec, pvec)*invDet;
			if ((u < 0f) || (u > 1f))
				return null;

			Vector3 qvec = Vector3.Cross(tvec, edge1);
			float v = Vector3.Dot(direction_, qvec)*invDet;
			if ((v < 0f) || ((u + v) > 1f))
				return null;

			float t = Vector3.Dot(edge2, qvec)*invDet;
			if (t >= 0f)
				return t;
			else
				return null;
		}

		public readonly Interval? FindIntersection(in AxisAlignedBox box)
		{
			Interval? intersection = new Line3(origin_, direction_).FindIntersection(box);

			if (intersection.HasValue && (intersection.Value.Maximum >= 0f))
			{
				Interval interval = intersection.Value;
				if (interval.Minimum != interval.Maximum)
					interval.Minimum = Math.Max(interval.Minimum, 0f);

				return interval;
			}
			else
			{
				return null;
			}
		}

		public readonly Interval? FindIntersection(in OrientedBox box)
		{
			Interval? intersection = new Line3(origin_, direction_).FindIntersection(box);

			if (intersection.HasValue && (intersection.Value.Maximum >= 0f))
			{
				Interval interval = intersection.Value;
				if (interval.Minimum != interval.Maximum)
					interval.Minimum = Math.Max(interval.Minimum, 0f);

				return interval;
			}
			else
			{
				return null;
			}
		}

		public readonly Interval? FindIntersection(in Sphere sphere)
		{
			Interval? intersection = new Line3(origin_, direction_).FindIntersection(sphere);

			if (intersection.HasValue && (intersection.Value.Maximum >= 0f))
			{
				Interval interval = intersection.Value;
				if (interval.Minimum != interval.Maximum)
					interval.Minimum = Math.Max(interval.Minimum, 0f);

				return interval;
			}
			else
			{
				return null;
			}
		}

		public readonly Interval? FindIntersection(in Ellipsoid ellipsoid)
		{
			Matrix3 m = ellipsoid.Matrix;
			Vector3 diff = origin_ - ellipsoid.Center;
			Vector3 matDir = direction_*m;
			Vector3 matDiff = diff*m;
			float a2 = Vector3.Dot(direction_, matDir);
			float a1 = Vector3.Dot(direction_, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			float discr = a1*a1 - a0*a2;
			if (discr < 0f)
			{
				return null;
			}
			else if (discr > 0f)
			{
				float root = MathF.Sqrt(discr);
				float inv = 1f/a2;
				float t0 = (-a1 - root)*inv;
				float t1 = (-a1 + root)*inv;

				if (t0 >= 0f)
					return new Interval(t0, t1);
				else if (t1 >= 0f)
					return new Interval(t1);
				else
					return null;
			}
			else
			{
				float t0 = -a1/a2;
				if (t0 >= 0f)
					return new Interval(t0);
				else
					return null;
			}
		}

		//public readonly Vector2? FindIntersectionPoint(in Plane plane)
		//{
		//}

		internal Vector3 origin_;
		internal Vector3 direction_;
	}
}
