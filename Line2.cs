/*
 *  Name: Line2
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Line2 : IFormattable, IEquatable<Line2>
	{
        public Line2(Vector2 origin, Vector2 direction)
        {
            origin_ = origin;
            direction_ = direction;
        }

		public Line2(Vector2 origin, float inclinationAngle)
		{
			origin_ = origin;
			direction_ = new Vector2((float)Math.Cos(inclinationAngle), (float)Math.Sin(inclinationAngle));
		}

		public static bool operator ==(Line2 lhs, Line2 rhs)
		{
			return (lhs.origin_ == rhs.origin_) && (lhs.direction_ == rhs.direction_);
		}

		public static bool operator !=(Line2 lhs, Line2 rhs)
		{
			return (lhs.origin_ != rhs.origin_) || (lhs.direction_ != rhs.direction_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Line2 rhs)
				return (origin_ == rhs.origin_) && (direction_ == rhs.direction_);

			return false;
		}

		public readonly bool Equals(Line2 other)
		{
			return (origin_ == other.origin_) && (direction_ == other.direction_);
		}

		public readonly override int GetHashCode()
		{
			int hash = origin_.GetHashCode();
			return ((hash << 5) + hash) ^ direction_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Line2 other, float tolerance)
		{
			return origin_.ApproxEquals(other.origin_, tolerance) &&
				direction_.ApproxEquals(other.direction_, tolerance);
		}

		public readonly bool ApproxEquals(in Line2 other)
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

		public static Line2 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Line2(new Vector2(Single.Parse(m[0]), Single.Parse(m[1])),
				new Vector2(Single.Parse(m[2]), Single.Parse(m[3])));
		}

		public static Line2 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Line2(new Vector2(Single.Parse(m[0], provider), Single.Parse(m[1], provider)),
				new Vector2(Single.Parse(m[2], provider), Single.Parse(m[3], provider)));
		}

		public static Line2 FromRay(in Ray2 r)
		{
			return new Line2(r.origin_, r.direction_);
		}

		//public static Line2 FromSegment(in Segment2 s)
		//{
		//	return new Line2(s.Start, s.Direction);
		//}

		//public static explicit operator Line2(Ray2 r)
		//{
		//    return FromRay(r);
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

		public static Line2 Normalize(Line2 l)
		{
			l.Normalize();
			return l;
		}

		public readonly Vector2 Evaluate(float t)
		{ 
			return (origin_ + t*direction_);
		}

		public readonly bool IsParallel(in Line2 line)
		{
			return (Math.Abs(Vector2.Cross(direction_, line.direction_)) < SingleConstants.Tolerance);
		}

		public readonly bool IsCoincident(in Line2 line)
		{
			return (Math.Abs(Vector2.Cross(direction_, line.direction_)) < SingleConstants.Tolerance) &&
				(Math.Abs(Vector2.Cross(Vector2.Normalize(line.origin_ - origin_), direction_)) < SingleConstants.Tolerance);
		}

		public readonly Vector2 GetClosestPoint(Vector2 point)
		{
			return Vector2.Dot(point - origin_, direction_)*direction_ + origin_;
		}

		public readonly Vector2 GetClosestPoint(Vector2 point, bool normalized)
		{
			return normalized ?
				Vector2.Dot(point - origin_, direction_)*direction_ + origin_ :
				(Vector2.Dot(point - origin_, direction_)/Vector2.Dot(direction_, direction_))*direction_ + origin_;
		}

		public readonly float GetDistanceTo(Vector2 point)
		{
			return Vector2.Distance(GetClosestPoint(point), point);
		}

		public readonly float GetDistanceTo(Vector2 point, bool normalized)
		{
			return Vector2.Distance(GetClosestPoint(point, normalized), point);
		}

		/// <summary>
		/// Returns signed distance from a point to normalized line.
		/// </summary>
		public readonly float GetSignedDistanceTo(Vector2 point)
		{ 
			return Vector2.Cross(origin_ - point, direction_);
		}

		/// <summary>
		/// Returns distance between normalized lines.
		/// </summary>
		public readonly float GetDistanceTo(in Line2 line)
		{
			return Math.Abs(GetSignedDistanceTo(line));
		}

		/// <summary>
		/// Returns signed distance between normalized lines.
		/// </summary>
		public readonly float GetSignedDistanceTo(in Line2 line)
		{
			return (Math.Abs(Vector2.Cross(direction_, line.direction_)) < SingleConstants.Tolerance) ?
				Vector2.Cross((origin_ + Vector2.Dot(line.origin_ - origin_, direction_)*direction_) - line.origin_, direction_) :
				0f;
		}

		public readonly bool Intersects(in Line2 line) 
		{
			return !(Math.Abs(Vector2.Cross(direction_, line.direction_)) < SingleConstants.Tolerance) ||
				(Math.Abs(Vector2.Cross(Vector2.Normalize(line.origin_ - origin_), direction_)) < SingleConstants.Tolerance);
		}

		public readonly bool Intersects(in Segment2 segment) 
		{
			return FindIntersection(segment).HasValue;
		}

		public readonly bool Intersects(in AxisAlignedRectangle rectangle)
		{
			return FindIntersection(rectangle).HasValue;
		}

		public readonly bool Intersects(in Circle2 circle)
		{
			Vector2 diff = origin_ - circle.center_;
			float a = Vector2.Dot(direction_, direction_);
			float b = 2f*Vector2.Dot(direction_, diff);
			float c = Vector2.Dot(diff, diff) - circle.radius_*circle.radius_;
			return !((b*b - 4f*a*c) < 0f);
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
			//float u = Vector2.Cross(line.origin_ - origin_, direction_)/d1CrossD2;
			return t;
		}

		public readonly float? FindIntersection(in Segment2 segment)
		{
			float d1CrossD2 = Vector2.Cross(direction_, segment.end_ - segment.start_);
			if (Math.Abs(d1CrossD2) < SingleConstants.Tolerance)
			{
				if (!(Math.Abs(Vector2.Cross(Vector2.Normalize(segment.start_ - origin_), direction_)) < SingleConstants.Tolerance))
					return null;
				return 0f; // collinear
			}

			//float t = Vector2.Cross(segment.start_ - origin_, segment.end_ - segment.start_)/d1CrossD2;
			float u = Vector2.Cross(segment.start_ - origin_, direction_)/d1CrossD2;
			if ((u >= 0f) && (u <= 1f))
				return /*t*/Vector2.Cross(segment.start_ - origin_, segment.end_ - segment.start_)/d1CrossD2;
			return null;
		}

		public readonly Interval? FindIntersection(in AxisAlignedRectangle rectangle)
		{
			Vector2 invDir = 1f/direction_;

			float tMin, tMax;
			if (direction_.x_ >= 0f)
			{
				tMin = (rectangle.minimum_.x_ - origin_.x_)*invDir.x_;
				tMax = (rectangle.maximum_.x_ - origin_.x_)*invDir.x_;
			}
			else
			{
				tMin = (rectangle.maximum_.x_ - origin_.x_)*invDir.x_;
				tMax = (rectangle.minimum_.x_ - origin_.x_)*invDir.x_;
			}

			float tyMin, tyMax;
			if (direction_.y_ >= 0f)
			{
				tyMin = (rectangle.minimum_.y_ - origin_.y_)*invDir.y_;
				tyMax = (rectangle.maximum_.y_ - origin_.y_)*invDir.y_;
			}
			else
			{
				tyMin = (rectangle.maximum_.y_ - origin_.y_)*invDir.y_;
				tyMax = (rectangle.minimum_.y_ - origin_.y_)*invDir.y_;
			}

			if ((tMin > tyMax) || (tyMin > tMax))
				return null;

			if (tyMin > tMin)
				tMin = tyMin;
			if (tyMax < tMax)
				tMax = tyMax;

			if (tMin > tMax)
				return null;

			return new Interval(tMin, tMax);
		}

		public readonly Interval? FindIntersection(in Circle2 circle)
		{
			Vector2 diff = origin_ - circle.center_;
			float a = Vector2.Dot(direction_, direction_);
			float b = 2f*Vector2.Dot(direction_, diff);
			float c = Vector2.Dot(diff, diff) - circle.radius_*circle.radius_;
			float delta = b*b - 4f*a*c;

			if (delta < 0f)
			{
				return null;
			}
			else if (delta > 0f)
			{
				delta = MathF.Sqrt(delta);
				a = 0.5f/a;
				return new Interval((-b - delta)*a, (-b + delta)*a);
			}
			else // delta == 0
			{
				return new Interval(-b*0.5f/a);
			}
		}

		//public readonly Vector2? FindIntersectionPoint(in Line2 line)
		//{
		//}

		//public readonly Vector2? FindIntersectionPoint(in Segment2 segment)
		//{
		//}

		internal Vector2 origin_;
		internal Vector2 direction_;
	}
}
