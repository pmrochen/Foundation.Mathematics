/*
 *  Name: AxisAlignedRectangle
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(AxisAlignedRectangleConverter))]
	public struct AxisAlignedRectangle : IFormattable, IEquatable<AxisAlignedRectangle>
	{
		public static readonly AxisAlignedRectangle Empty =
			new AxisAlignedRectangle(new Vector2(Single.PositiveInfinity, Single.PositiveInfinity),
				new Vector2(Single.NegativeInfinity, Single.NegativeInfinity));

		public AxisAlignedRectangle(Vector2 minimum, Vector2 maximum)
		{
			minimum_ = minimum;
			maximum_ = maximum;
		}

		public AxisAlignedRectangle(Vector2 dimensions)
		{
			minimum_ = -0.5f*dimensions;
			maximum_ = 0.5f*dimensions;
		}

		public static bool operator ==(AxisAlignedRectangle lhs, AxisAlignedRectangle rhs)
		{
			return (lhs.minimum_ == rhs.minimum_) && (lhs.maximum_ == rhs.maximum_);
		}

		public static bool operator !=(AxisAlignedRectangle lhs, AxisAlignedRectangle rhs)
		{
			return (lhs.minimum_ != rhs.minimum_) || (lhs.maximum_ != rhs.maximum_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is AxisAlignedRectangle rhs)
				return (minimum_ == rhs.minimum_) && (maximum_ == rhs.maximum_);

			return false;
		}

		public readonly bool Equals(AxisAlignedRectangle other)
		{
			return (minimum_ == other.minimum_) && (maximum_ == other.maximum_);
		}

		public readonly override int GetHashCode()
		{
			int hash = minimum_.GetHashCode();
			return ((hash << 5) + hash) ^ maximum_.GetHashCode();
		}

		public readonly bool ApproxEquals(in AxisAlignedRectangle other, float tolerance)
		{
			return minimum_.ApproxEquals(other.minimum_, tolerance) &&
				maximum_.ApproxEquals(other.maximum_, tolerance);
		}

		public readonly bool ApproxEquals(in AxisAlignedRectangle other)
		{
			return minimum_.ApproxEquals(other.minimum_) &&
				maximum_.ApproxEquals(other.maximum_);
		}

		public readonly override string ToString()
		{
			return String.Concat(minimum_.ToString(), " ", maximum_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(minimum_.ToString(provider), " ", maximum_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(minimum_.ToString(format), " ", maximum_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(minimum_.ToString(format, provider), " ", maximum_.ToString(format, provider));
		}

		public static AxisAlignedRectangle Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new AxisAlignedRectangle(new Vector2(Functions.ParseSingle(m[0]), Functions.ParseSingle(m[1])),
				new Vector2(Functions.ParseSingle(m[2]), Functions.ParseSingle(m[3])));
		}

		public static AxisAlignedRectangle Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new AxisAlignedRectangle(new Vector2(Functions.ParseSingle(m[0], provider), Functions.ParseSingle(m[1], provider)),
				new Vector2(Functions.ParseSingle(m[2], provider), Functions.ParseSingle(m[3], provider)));
		}

		[Browsable(false)]
		public readonly bool IsFinite => minimum_.IsFinite && maximum_.IsFinite;

		[Browsable(false)]
		public readonly bool IsEmpty => minimum_.AnyGreaterThan(maximum_);

		public Vector2 Minimum
		{
			readonly get => minimum_;
			set => minimum_ = value;
		}

		public Vector2 Maximum
		{
			readonly get => maximum_;
			set => maximum_ = value;
		}

		[Browsable(false)]
		public readonly Vector2 Dimensions => (maximum_ - minimum_);

		[Browsable(false)]
		public readonly Vector2 HalfDimensions => (maximum_ - minimum_)*0.5f;

		[Browsable(false)]
		public readonly Vector2 Center => (minimum_ + maximum_)*0.5f;

		[Browsable(false)]
		public readonly float Diagonal => Vector2.Distance(minimum_, maximum_);

		[Browsable(false)]
		public readonly float Perimeter
		{
			get
			{
				Vector2 dim = maximum_ - minimum_;
				return 2f*dim.x_ + 2f*dim.y_;
			}
		}

		[Browsable(false)]
		public readonly float Area
		{
			get
			{
				Vector2 dim = maximum_ - minimum_;
				return dim.x_*dim.y_;
			}
		}

		public readonly IEnumerable<Vector2> GetVertices()
		{
			yield return minimum_;
	        yield return new Vector2(maximum_.x_, minimum_.y_);
	        yield return new Vector2(minimum_.x_, maximum_.y_);
	        yield return maximum_;
		}

		public static AxisAlignedRectangle Union(in AxisAlignedRectangle a, in AxisAlignedRectangle b)
		{
			return new AxisAlignedRectangle(Vector2.Min(a.minimum_, b.minimum_), Vector2.Max(a.maximum_, b.maximum_));
		}

		public static AxisAlignedRectangle Intersection(in AxisAlignedRectangle a, in AxisAlignedRectangle b)
		{
			return new AxisAlignedRectangle(Vector2.Max(a.minimum_, b.minimum_), Vector2.Min(a.maximum_, b.maximum_));
		}

		public void ExtendBy(Vector2 point)
		{
			minimum_ = Vector2.Min(minimum_, point);
			maximum_ = Vector2.Max(maximum_, point);
		}

		public void ExtendBy(in AxisAlignedRectangle rectangle)
		{
			minimum_ = Vector2.Min(minimum_, rectangle.minimum_);
			maximum_ = Vector2.Max(maximum_, rectangle.maximum_);
		}

		public void Inflate(Vector2 halfDims)
		{
			minimum_ -= halfDims;
			maximum_ += halfDims;
		}

		public void Translate(Vector2 offset)
		{
			minimum_ += offset;
			maximum_ += offset;
		}

		public readonly Vector2 GetClosestPoint(Vector2 point)
		{
			return Vector2.Clamp(point, minimum_, maximum_);
		}

		public readonly float GetDistanceTo(Vector2 point)
		{
			return Vector2.Distance(point, GetClosestPoint(point));
		}

		public readonly bool Contains(Vector2 point)
		{
			return minimum_.AllLessThanEqual(point) && maximum_.AllGreaterThanEqual(point);
		}

		public readonly bool Contains(in AxisAlignedRectangle rectangle)
		{
			return minimum_.AllLessThanEqual(rectangle.minimum_) && maximum_.AllGreaterThanEqual(rectangle.maximum_.x_);
		}

		public readonly bool Contains(in Circle2 circle)
		{
			Vector2 center = circle.Center;
#if SIMD
			Vector2 radius = new Vector2(sphere.Radius);
			return minimum_.AllLessThanEqual(center - radius) && maximum_.AllGreaterThanEqual(center + radius);
#else
			float radius = circle.Radius;
			return (minimum_.x_ <= (center.x_ - radius)) && (maximum_.x_ >= (center.x_ + radius)) &&
				(minimum_.y_ <= (center.y_ - radius)) && (maximum_.y_ >= (center.y_ + radius));
#endif
		}

		public readonly bool Intersects(in AxisAlignedRectangle rectangle)
		{
			return minimum_.AllLessThanEqual(rectangle.maximum_) && maximum_.AllGreaterThanEqual(rectangle.minimum_);
		}

		public readonly bool Intersects(in Line2 line)
        {
			return FindIntersection(line).HasValue;
        }

        public readonly bool Intersects(in Ray2 ray)
        {
			return FindIntersection(ray).HasValue;
        }

		public readonly bool Intersects(in Segment2 segment)
		{
			return FindIntersection(segment).HasValue;
		}

		public readonly bool Intersects(in Circle2 circle)
		{
			return circle.Intersects(this);
		}

		public readonly Interval? FindIntersection(in Line2 line)
		{
			// #TODO
		}

		public readonly Interval? FindIntersection(in Ray2 ray)
		{
			Interval? intersection = FindIntersection(new Line2(ray.origin_, ray.direction_));

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

		public readonly Interval? FindIntersection(in Segment2 segment)
		{
			Interval? intersection = FindIntersection(new Line2(segment.start_, segment.end_ - segment.start_));

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

		internal Vector2 minimum_;
		internal Vector2 maximum_;
	}
}
