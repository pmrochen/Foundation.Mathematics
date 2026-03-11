/*
 *  Name: Segment2
 *  Author: Pawel Mrochen
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Segment2 : IFormattable, IEquatable<Segment2>
	{
        public Segment2(Vector2 start, Vector2 end)
        {
            start_ = start;
            end_ = end;
        }

		public Segment2(Vector2[] endPoints) :
			this(endPoints[0], endPoints[1])
		{
		}

		public static bool operator ==(Segment2 lhs, Segment2 rhs)
		{
			return (lhs.start_ == rhs.start_) && (lhs.end_ == rhs.end_);
		}

		public static bool operator !=(Segment2 lhs, Segment2 rhs)
		{
			return (lhs.start_ != rhs.start_) || (lhs.end_ != rhs.end_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Segment2 rhs)
				return (start_ == rhs.start_) && (end_ == rhs.end_);

			return false;
		}

		public readonly bool Equals(Segment2 other)
		{
			return (start_ == other.start_) && (end_ == other.end_);
		}

		public readonly override int GetHashCode()
		{
			int hash = start_.GetHashCode();
			return ((hash << 5) + hash) ^ end_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Segment2 other, float tolerance)
		{
			return start_.ApproxEquals(other.start_, tolerance) &&
				end_.ApproxEquals(other.end_, tolerance);
		}

		public readonly bool ApproxEquals(in Segment2 other)
		{
			return start_.ApproxEquals(other.start_) &&
				end_.ApproxEquals(other.end_);
		}

		public readonly override string ToString()
		{
			return String.Concat(start_.ToString(), " ", end_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(start_.ToString(provider), " ", end_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(start_.ToString(format), " ", end_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(start_.ToString(format, provider), " ", end_.ToString(format, provider));
		}

		public static Segment2 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Segment2(new Vector2(Single.Parse(m[0]), Single.Parse(m[1])),
				new Vector2(Single.Parse(m[2]), Single.Parse(m[3])));
		}

		public static Segment2 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Segment2(new Vector2(Single.Parse(m[0], provider), Single.Parse(m[1], provider)),
				new Vector2(Single.Parse(m[2], provider), Single.Parse(m[3], provider)));
		}

		public static Segment2 FromLine(in Line2 l)
		{
			return new Segment2(l.origin_, l.origin_ + l.direction_);
		}

		public static Segment2 FromLine(in Line2 l, Interval interval)
		{
			return new Segment2(l.Evaluate(interval.Minimum), l.Evaluate(interval.Maximum));
		}

		public static Segment2 FromRay(in Ray2 r)
		{
			return new Segment2(r.origin_, r.origin_ + r.direction_);
		}

		public static Segment2 FromRay(in Ray2 r, Interval interval)
		{
			return new Segment2(r.Evaluate(interval.Minimum), r.Evaluate(interval.Maximum));
		}

		[Browsable(false)]
		public readonly bool IsFinite => start_.IsFinite && end_.IsFinite;

		public Vector2 Start
		{
			readonly get => start_;
			set => start_ = value;
		}

		public Vector2 End
        {
			readonly get => end_;
            set => end_ = value;
        }

		[Browsable(false)]
		public readonly Vector2 /*Normalized*/Direction => Vector2.Normalize(end_ - start_);

		[Browsable(false)]
		public readonly float Length => Vector2.Distance(start_, end_);

		[Browsable(false)]
		public readonly Vector2 Center => Vector2.Lerp(start_, end_, 0.5f);

		[Browsable(false)]
		public readonly float Slope => (end_.Y - start_.Y)/(end_.X - start_.X);

		[Browsable(false)]
		public readonly float InclinationAngle => (float)Math.Atan2(end_.Y - start_.Y, end_.X - start_.X);

		public readonly IEnumerable<Vector2> GetEndPoints()
		{
			yield return start_;
			yield return end_;
		}

		public void Translate(Vector2 offset)
		{
			start_ += offset;
			end_ += offset;
		}

		public static Segment2 Translate(Segment2 segment, Vector2 offset)
		{
			segment.Translate(offset);
			return segment;
		}

		public readonly Vector2 Evaluate(float t)
		{
			return Vector2.Lerp(start_, end_, t);
		}

		public readonly Vector2 GetClosestPoint(Vector2 point)
		{
			Vector2 direction = end_ - start_;
			return Math.Clamp(Vector2.Dot(point - start_, direction)/Vector2.Dot(direction, direction), 0f, 1f)*direction + start_;
		}

		public readonly float GetDistanceTo(Vector2 point)
		{
			return Vector2.Distance(GetClosestPoint(point), point);
		}

		public readonly bool Intersects(in Line2 line)
		{
			return FindIntersection(line).HasValue;
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
			return FindIntersection(circle).HasValue;
		}

		public readonly float? FindIntersection(in Line2 line)
		{
			return Intersections.FindLineSegment(line.origin_, line.direction_, start_, end_);
		}

		public readonly float? FindIntersection(in Segment2 segment)
		{
			return Intersections.FindSegmentSegment(start_, end_, segment.start_, segment.end_);
		}

		public readonly Interval? FindIntersection(in AxisAlignedRectangle rectangle)
		{
			Interval? intersection = Intersections.FindLineAxisAlignedRectangle(start_, end_ - start_, 
				rectangle.minimum_, rectangle.maximum_);

			if (intersection.HasValue && (intersection.Value.Maximum >= 0f) && (intersection.Value.Minimum <= 1f))
			{
				Interval interval = intersection.Value;
				if (interval.Minimum != interval.Maximum)
				{
					interval.Minimum = Math.Max(interval.Minimum, 0f);
					interval.Maximum = Math.Min(interval.Maximum, 1f);
				}

				return interval;
			}
			else
			{
				return null;
			}
		}

		public readonly Interval? FindIntersection(in Circle2 circle)
		{
			Interval? intersection = Intersections.FindLineCircle(start_, end_ - start_, circle.center_, circle.radius_);

			if (intersection.HasValue && (intersection.Value.Maximum >= 0f) && (intersection.Value.Minimum <= 1f))
			{
				Interval interval = intersection.Value;
				if (interval.Minimum != interval.Maximum)
				{
					interval.Minimum = Math.Max(interval.Minimum, 0f);
					interval.Maximum = Math.Min(interval.Maximum, 1f);
				}

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

		//public readonly Vector2? FindIntersectionPoint(in Segment2 segment)
		//{
		//}

		internal Vector2 start_;
		internal Vector2 end_;
	}
}
