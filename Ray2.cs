/*
 *  Name: Ray2
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Ray2 : IFormattable, IEquatable<Ray2>
	{
        public Ray2(Vector2 origin, Vector2 direction)
        {
            origin_ = origin;
            direction_ = direction;
        }

		public static bool operator ==(Ray2 lhs, Ray2 rhs)
		{
			return (lhs.origin_ == rhs.origin_) && (lhs.direction_ == rhs.direction_);
		}

		public static bool operator !=(Ray2 lhs, Ray2 rhs)
		{
			return (lhs.origin_ != rhs.origin_) || (lhs.direction_ != rhs.direction_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Ray2 rhs)
				return (origin_ == rhs.origin_) && (direction_ == rhs.direction_);

			return false;
		}

		public readonly bool Equals(Ray2 other)
		{
			return (origin_ == other.origin_) && (direction_ == other.direction_);
		}

		public readonly override int GetHashCode()
		{
			int hash = origin_.GetHashCode();
			return ((hash << 5) + hash) ^ direction_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Ray2 other, float tolerance)
		{
			return origin_.ApproxEquals(other.origin_, tolerance) &&
				direction_.ApproxEquals(other.direction_, tolerance);
		}

		public readonly bool ApproxEquals(in Ray2 other)
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

		public static Ray2 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Ray2(new Vector2(Single.Parse(m[0]), Single.Parse(m[1])),
					new Vector2(Single.Parse(m[2]), Single.Parse(m[3])));
		}

		public static Ray2 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Ray2(new Vector2(Single.Parse(m[0], provider), Single.Parse(m[1], provider)),
					new Vector2(Single.Parse(m[2], provider), Single.Parse(m[3], provider)));
		}

		public static Ray2 FromLine(in Line2 l)
		{
			return new Ray2(l.origin_, l.direction_);
		}

		//public static Ray2 FromSegment(in Segment2 s)
		//{
		//	return new Ray2(s.Start, s.Direction);
		//}

		//public static implicit operator Ray2(Line2 l)
		//{
		//    return FromLine(l);
		//}

		public Vector2 Origin
        {
			readonly get => origin_;
            set => origin_ = value;
        }

        public Vector2 Direction
        {
			readonly get => direction_;
            set => direction_ = value;
        }

		[Browsable(false)]
		public readonly float Slope => direction_.Y/direction_.X;

		[Browsable(false)]
		public readonly float InclinationAngle => (float)Math.Atan2(direction_.Y, direction_.X);

		public void Normalize()
        {
            direction_.Normalize();
        }

		public static Ray2 Normalize(Ray2 r)
		{
			r.Normalize();
			return r;
		}

		public readonly Vector2 Evaluate(float t)
		{
			return (origin_ + t*direction_);
		}

		public readonly Vector2 GetClosestPoint(Vector2 point)
		{
			return Math.Max(Vector2.Dot(point - origin_, direction_), 0f)*direction_ + origin_;
		}

		public readonly Vector2 GetClosestPoint(Vector2 point, bool normalized)
		{
			return normalized ?
				Math.Max(Vector2.Dot(point - origin_, direction_), 0f)*direction_ + origin_ :
				Math.Max(Vector2.Dot(point - origin_, direction_)/Vector2.Dot(direction_, direction_), 0f)*direction_ + origin_;
		}

		public readonly float GetDistanceTo(Vector2 point)
		{
			return Vector2.Distance(GetClosestPoint(point), point);
		}

		public readonly float GetDistanceTo(Vector2 point, bool normalized)
		{
			return Vector2.Distance(GetClosestPoint(point, normalized), point);
		}

		public readonly bool Intersects(in Line2 line)
		{
			return FindIntersection(line).HasValue;
		}

		public readonly bool Intersects(in AxisAlignedRectangle rectangle)
		{
			return FindIntersection(rectangle).HasValue;
		}

		public readonly bool Intersects(in Circle2 circle)
		{
			return FindIntersection(circle).HasValue;
		}

		public readonly float? FindIntersection(in Line2 line) 
		{
			float d1CrossD2 = Vector2.Cross(direction_, line.direction_);
			if (Math.Abs(d1CrossD2) < SingleConstants.Tolerance)
			{
				if (!(Math.Abs(Vector2.Cross(Vector2.Normalize(line.origin_ - origin_), direction_)) < SingleConstants.Tolerance))
					return null;
				return 0f; // collinear
			}

			float t = Vector2.Cross(line.origin_ - origin_, line.direction_)/d1CrossD2;
			if (t >= 0f) 
				return t;
			return null;
		}

		public readonly Interval? FindIntersection(in AxisAlignedRectangle rectangle)
		{
			Interval? intersection = new Line2(origin_, direction_).FindIntersection(rectangle);

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

		public readonly Interval? FindIntersection(in Circle2 circle)
		{
			Interval? intersection = new Line2(origin_, direction_).FindIntersection(circle);

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

		//public readonly Vector2? FindIntersectionPoint(in Line2 line)
		//{
		//}

		internal Vector2 origin_;
		internal Vector2 direction_;
	}
}
