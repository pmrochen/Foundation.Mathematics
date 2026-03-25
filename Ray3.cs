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

		public void Translate(Vector3 offset)
		{
			origin_ += offset;
		}

		public static Ray3 Translate(Ray3 ray, Vector3 offset)
		{
			ray.Translate(offset);
			return ray;
		}

		public void Transform(in Matrix3 matrix)
		{
			origin_.Transform(matrix);
			direction_.Transform(matrix);
		}

		public void Transform(in AffineTransform at)
        {
			origin_.Transform(at);
			direction_.Transform(at.r_);
        }

		public static Ray3 Transform(Ray3 ray, in Matrix3 matrix)
		{
			ray.Transform(matrix);
			return ray;
		}

		public static Ray3 Transform(Ray3 ray, in AffineTransform at)
		{
			ray.Transform(at);
			return ray;
		}

		public void Normalize()
		{
			direction_.Normalize();
		}

		public static Ray3 Normalize(Ray3 ray)
		{
			ray.Normalize();
			return ray;
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
			return Intersections.TestRayEllipsoid(origin_, direction_, ellipsoid.Center, ellipsoid.Matrix);
		}

		public readonly float? FindIntersection(in Plane plane)
		{
			float? result = Intersections.FindLinePlane(origin_, direction_, plane.Normal, plane.D);
			return (result.HasValue && (result.Value >= 0f)) ? result : null;
		}

		public readonly float? FindIntersection(in Triangle3 triangle)
		{
			//return Intersections.FindRayTriangle(origin_, direction_, triangle.vertex0_, triangle.vertex1_, triangle.vertex2_);
			float? result = Intersections.FindLineTriangle(origin_, direction_, triangle.vertex0_, triangle.vertex1_, triangle.vertex2_);
			return (result.HasValue && (result.Value >= 0f)) ? result : null;
		}

		public readonly Interval? FindIntersection(in AxisAlignedBox box)
		{
			Interval? intersection = Intersections.FindLineAxisAlignedBox(origin_, direction_, box.minimum_, box.maximum_);

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
			Interval? intersection = Intersections.FindLineOrientedBox(origin_, direction_, box.center_, box.basis_, box.halfDims_);

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
			Interval? intersection = Intersections.FindLineSphere(origin_, direction_, sphere.center_, sphere.radius_);

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
			return Intersections.FindRayEllipsoid(origin_, direction_, ellipsoid.Center, ellipsoid.Matrix);
		}

		//public readonly Vector2? FindIntersectionPoint(in Plane plane)
		//{
		//}

		internal Vector3 origin_;
		internal Vector3 direction_;
	}
}
