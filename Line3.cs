/*
 *  Name: Line3
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Line3 : IFormattable, IEquatable<Line3>
	{
        public Line3(Vector3 origin, Vector3 direction)
        {
            origin_ = origin;
            direction_ = direction;
        }

		public static bool operator ==(Line3 lhs, Line3 rhs)
		{
			return (lhs.origin_ == rhs.origin_) && (lhs.direction_ == rhs.direction_);
		}

		public static bool operator !=(Line3 lhs, Line3 rhs)
		{
			return (lhs.origin_ != rhs.origin_) || (lhs.direction_ != rhs.direction_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Line3 rhs)
				return (origin_ == rhs.origin_) && (direction_ == rhs.direction_);

			return false;
		}

		public readonly bool Equals(Line3 other)
		{
			return (origin_ == other.origin_) && (direction_ == other.direction_);
		}

		public readonly override int GetHashCode()
		{
			int hash = origin_.GetHashCode();
			return ((hash << 5) + hash) ^ direction_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Line3 other, float tolerance)
		{
			return origin_.ApproxEquals(other.origin_, tolerance) &&
				direction_.ApproxEquals(other.direction_, tolerance);
		}

		public readonly bool ApproxEquals(in Line3 other)
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

		public static Line3 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new Line3(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
				new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])));
		}

		public static Line3 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new Line3(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
				new Vector3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider)));
		}

		public static Line3 FromRay(in Ray3 r)
		{
			return new Line3(r.origin_, r.direction_);
		}

		//public static Line3 FromSegment(in Segment3 s)
		//{
		//	return new Line3(s.Start, s.Direction);
		//}

		//public static explicit operator Line3(Ray3 r)
		//{
		//    return FromRay(r);
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

		public static Line3 Normalize(Line3 l)
		{
			l.Normalize();
			return l;
		}

		public void Translate(Vector3 offset)
		{
			origin_ += offset;
		}

		public static Line3 Translate(Line3 l, Vector3 offset)
		{
			l.Translate(offset);
			return l;
		}

		public void Transform(in AffineTransform t)
        {
			origin_.Transform(t);
			direction_.Transform(t.r_);
        }

		public static Line3 Transform(Line3 l, in AffineTransform t)
		{
			l.Transform(t);
			return l;
		}

		public readonly Vector3 Evaluate(float t)
		{ 
			return (origin_ + t*direction_);
		}

		public readonly Vector3 GetClosestPoint(Vector3 point)
		{
			return Vector3.Dot(point - origin_, direction_)*direction_ + origin_;
		}

		public readonly Vector3 GetClosestPoint(Vector3 point, bool normalized)
		{
			return normalized ?
				Vector3.Dot(point - origin_, direction_)*direction_ + origin_ :
				(Vector3.Dot(point - origin_, direction_)/Vector3.Dot(direction_, direction_))*direction_ + origin_;
		}

		public readonly float GetDistanceTo(Vector3 point)
		{
			return Vector3.Distance(GetClosestPoint(point), point);
		}

		public readonly float GetDistanceTo(Vector3 point, bool normalized)
		{
			return Vector3.Distance(GetClosestPoint(point, normalized), point);
		}

		// #TODO
		//public readonly float GetDistanceTo(Line3 line)
		//{
		// 	return Math.Abs(Vector3.Dot(Vector3.Cross(direction_, line.direction_), (line.origin_ - origin_)));
		//}

		public readonly bool Intersects(in Plane plane)
		{
			return (Math.Abs(Vector3.Dot(plane.Normal, direction_)) >= 1e-6f);
		}

		public readonly float? FindIntersection(in Plane plane)
		{
			float nd = Vector3.Dot(plane.Normal, direction_);
			if (Math.Abs(nd) < SingleConstants.Tolerance)
				return null;
			return (-plane.D - Vector3.Dot(plane.Normal, origin_))/nd;
		}

		//public readonly Vector2? FindIntersectionPoint(in Plane plane)
		//{
		//}

		internal Vector3 origin_;
		internal Vector3 direction_;
	}
}
