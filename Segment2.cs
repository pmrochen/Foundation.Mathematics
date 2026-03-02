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

		public readonly float? FindIntersection(in Line2 line) 
		{
			Vector2 direction = end_ - start_;
			float d1CrossD2 = Vector2.Cross(direction, line.direction_);
			if (Math.Abs(d1CrossD2) < SingleConstants.Tolerance)
			{
				if (!(Math.Abs(Vector2.Cross(Vector2.Normalize(line.origin_ - start_), direction)) < SingleConstants.Tolerance))
					return null;
				return 0f; // collinear
			}

			float t = Vector2.Cross(line.origin_ - start_, line.direction_)/d1CrossD2;
			if ((t >= 0f) && (t <= 1f))
				return t;
			return null;
		}

		public readonly float? FindIntersection(in Segment2 segment) 
		{
			Vector2 direction = end_ - start_;
			float d1CrossD2 = Vector2.Cross(direction, segment.end_ - segment.start_);
			if (Math.Abs(d1CrossD2) < SingleConstants.Tolerance)
			{
				if (!(Math.Abs(Vector2.Cross(Vector2.Normalize(segment.start_ - start_), direction)) < SingleConstants.Tolerance))
					return null;

				float d1DotD1 = Vector2.Dot(direction, direction);
				float d1DotD2 = Vector2.Dot(direction, segment.end_ - segment.start_);
				float t0 = Vector2.Dot(segment.start_ - start_, direction)/d1DotD1;
				float t1 = t0 + d1DotD2/d1DotD1;
				if (d1DotD2 < 0f)
				{
					float s = t0;
					t0 = t1;
					t1 = s;
				}

				if ((t0 <= 1f) && (t1 >= 0f))
					return Math.Max(t0, 0f);
				return null;
			}

			float t = Vector2.Cross(segment.start_ - start_, segment.end_ - segment.start_)/d1CrossD2;
			float u = Vector2.Cross(segment.start_ - start_, direction)/d1CrossD2;
			if ((t >= 0f) && (t <= 1f) && (u >= 0f) && (u <= 1f))
				return t;
			return null;
		}

		//public readonly Vector2? FindIntersectionPoint(in Line2 line) // #TODO
		//{
		//}

		//public readonly Vector2? FindIntersectionPoint(in Segment2 segment) // #TODO
		//{
		//}

		internal Vector2 start_;
		internal Vector2 end_;
	}
}
