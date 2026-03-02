/*
 *  Name: Segment3
 *  Author: Pawel Mrochen
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Segment3 : IFormattable, IEquatable<Segment3>
	{
		public Segment3(Vector3 start, Vector3 end)
		{
			start_ = start;
			end_ = end;
		}

		//public Segment3(Vector3[] endPoints) :
		//	this(endPoints[0], endPoints[1])
		//{
		//}

		public static bool operator ==(Segment3 lhs, Segment3 rhs)
		{
			return (lhs.start_ == rhs.start_) && (lhs.end_ == rhs.end_);
		}

		public static bool operator !=(Segment3 lhs, Segment3 rhs)
		{
			return (lhs.start_ != rhs.start_) || (lhs.end_ != rhs.end_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Segment3 rhs)
				return (start_ == rhs.start_) && (end_ == rhs.end_);

			return false;
		}

		public readonly bool Equals(Segment3 other)
		{
			return (start_ == other.start_) && (end_ == other.end_);
		}

		public readonly override int GetHashCode()
		{
			int hash = start_.GetHashCode();
			return ((hash << 5) + hash) ^ end_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Segment3 other, float tolerance)
		{
			return start_.ApproxEquals(other.start_, tolerance) &&
				end_.ApproxEquals(other.end_, tolerance);
		}

		public readonly bool ApproxEquals(in Segment3 other)
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

		public static Segment3 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new Segment3(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
				new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])));
		}

		public static Segment3 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new Segment3(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
				new Vector3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider)));
		}

		public static Segment3 FromLine(in Line3 l)
		{
			return new Segment3(l.origin_, l.origin_ + l.direction_);
		}

		public static Segment3 FromLine(in Line3 l, Interval interval)
		{
			return new Segment3(l.Evaluate(interval.Minimum), l.Evaluate(interval.Maximum));
		}

		public static Segment3 FromRay(in Ray3 r)
		{
			return new Segment3(r.origin_, r.origin_ + r.direction_);
		}

		public static Segment3 FromRay(in Ray3 r, Interval interval)
		{
			return new Segment3(r.Evaluate(interval.Minimum), r.Evaluate(interval.Maximum));
		}

		[Browsable(false)]
		public readonly bool IsFinite => start_.IsFinite && end_.IsFinite;

		public Vector3 Start
		{
			readonly get => start_;
			set => start_ = value;
		}

		public Vector3 End
        {
			readonly get => end_;
            set => end_ = value;
        }

		[Browsable(false)]
		public readonly Vector3 /*Normalized*/Direction => Vector3.Normalize(end_ - start_);

		[Browsable(false)]
		public readonly float Length => Vector3.Distance(start_, end_);

		[Browsable(false)]
		public readonly Vector3 Center => Vector3.Lerp(start_, end_, 0.5f);

		//public Vector3[] GetEndPoints()
		//{
		//    return new Vector3[2] { start_, end_ };
		//}

		public readonly IEnumerable<Vector3> GetEndPoints()
		{
			yield return start_;
			yield return end_;
		}

		public void Translate(Vector3 offset)
		{
			start_ += offset;
			end_ += offset;
		}

		public static Segment3 Translate(Segment3 s, Vector3 offset)
		{
			s.Translate(offset);
			return s;
		}

		public void Transform(in AffineTransform t)
        {
			start_.Transform(t);
			end_.Transform(t);
        }

		public static Segment3 Transform(Segment3 s, in AffineTransform t)
		{
			s.Transform(t);
			return s;
		}

		public readonly Vector3 Evaluate(float t)
		{
			return Vector3.Lerp(start_, end_, t);
		}

		public readonly Vector3 GetClosestPoint(Vector3 point)
		{
			Vector3 direction = end_ - start_;
			return Math.Clamp(Vector3.Dot(point - start_, direction)/Vector3.Dot(direction, direction), 0f, 1f)*direction + start_;
		}

		public readonly float GetDistanceTo(Vector3 point)
		{
			return Vector3.Distance(GetClosestPoint(point), point);
		}

		public readonly bool Intersects(in HalfSpace halfSpace)
		{
			return halfSpace.Contains(start_) || halfSpace.Contains(end_);
		}

		public readonly bool Intersects(in Plane plane)
		{
			return FindIntersection(plane).HasValue;
		}

		public readonly float? FindIntersection(in Plane plane)
		{
			float? t = new Line3(start_, end_ - start_).FindIntersection(plane);
			return (t.HasValue && (t.Value >= 0f) && (t.Value <= 1f)) ? t : null;
		}

		//public readonly Vector2? FindIntersectionPoint(in Plane plane)
		//{
		//}

		internal Vector3 start_;
		internal Vector3 end_;
	}
}
