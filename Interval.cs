/*
 *  Name: Interval
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(IntervalConverter))]
	public struct Interval : IFormattable, IEquatable<Interval>
	{
		public static readonly Interval Empty = new Interval(Single.PositiveInfinity, Single.NegativeInfinity);
		public static readonly Interval Zero = new Interval(0f, 0f);

		public Interval(float value)
		{
			minimum_ = value;
			maximum_ = value;
		}

		public Interval(float minimum, float maximum)
		{
			minimum_ = minimum;
			maximum_ = maximum;
		}

		public static bool operator ==(Interval lhs, Interval rhs)
		{
			return (lhs.minimum_ == rhs.minimum_) && (lhs.maximum_ == rhs.maximum_);
		}

		public static bool operator !=(Interval lhs, Interval rhs)
		{
			return (lhs.minimum_ != rhs.minimum_) || (lhs.maximum_ != rhs.maximum_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Interval rhs)
				return (minimum_ == rhs.minimum_) && (maximum_ == rhs.maximum_);

			return false;
		}

		public readonly bool Equals(Interval other)
		{
			return (minimum_ == other.minimum_) && (maximum_ == other.maximum_);
		}

		public readonly override int GetHashCode()
		{
			int hash = minimum_.GetHashCode();
			return ((hash << 5) + hash) ^ maximum_.GetHashCode();
		}

		public readonly bool ApproxEquals(Interval other, float tolerance)
		{
			return (Math.Abs(other.minimum_ - minimum_) < tolerance) &&
				(Math.Abs(other.maximum_ - maximum_) < tolerance);
		}

		public readonly bool ApproxEquals(Interval other)
		{
			return ApproxEquals(other, 1e-6f);
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1}", minimum_, maximum_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1}", minimum_, maximum_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1}", minimum_.ToString(format), maximum_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1}", minimum_.ToString(format, provider), maximum_.ToString(format, provider));
		}

		public static Interval Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 2)
				throw new FormatException();

			return new Interval(Functions.ParseSingle(m[0]), Functions.ParseSingle(m[1]));
		}

		public static Interval Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 2)
				throw new FormatException();

			return new Interval(Functions.ParseSingle(m[0], provider), Functions.ParseSingle(m[1], provider));
		}

		[Browsable(false)]
		public readonly bool IsFinite => Functions.IsFinite(minimum_) && Functions.IsFinite(maximum_);

		[Browsable(false)]
		public readonly bool IsEmpty => (minimum_ > maximum_);

		public float Minimum
		{
			readonly get => minimum_;
			set => minimum_ = value;
		}

		public float Maximum
		{
			readonly get => maximum_;
			set => maximum_ = value;
		}

		[Browsable(false)]
		public float Length
		{
			readonly get => (maximum_ - minimum_);

			set
			{
				float center = (minimum_ + maximum_)*0.5f;
				minimum_ = center - 0.5f*value;
				maximum_ = center + 0.5f*value;
			}
		}

		[Browsable(false)]
		public float Center
		{
			readonly get => (minimum_ + maximum_)*0.5f;

			set
			{
				float diff = value - (minimum_ + maximum_)*0.5f;
				minimum_ += diff;
				maximum_ += diff;
			}
		}

		public static Interval Union(Interval a, Interval b)
		{
			return new Interval(Math.Min(a.minimum_, b.minimum_), Math.Max(a.maximum_, b.maximum_));
		}

		public static Interval Intersection(Interval a, Interval b)
		{
			return new Interval(Math.Max(a.minimum_, b.minimum_), Math.Min(a.maximum_, b.maximum_));
		}

		public void ExtendBy(float value)
		{
			minimum_ = Math.Min(minimum_, value);
			maximum_ = Math.Max(maximum_, value);
		}

		public void ExtendBy(Interval interval)
		{
			minimum_ = Math.Min(minimum_, interval.minimum_);
			maximum_ = Math.Max(maximum_, interval.maximum_);
		}

		public readonly bool Contains(float value)
		{
			return (minimum_ <= value) && (maximum_ >= value);
		}

		public readonly bool Contains(Interval interval)
		{
			return (minimum_ <= interval.minimum_) && (maximum_ >= interval.maximum_);
		}

		public readonly bool Intersects(Interval interval)
		{
			return (minimum_ <= interval.maximum_) && (maximum_ >= interval.minimum_);
		}

		public readonly Interval? FindIntersection(Interval interval)
		{
			if ((maximum_ < interval.minimum_) || (minimum_ > interval.maximum_))
			{
				return null;
			}
			else if (maximum_ > interval.minimum_)
			{
				if (minimum_ < interval.maximum_)
				{
					return new Interval((minimum_ < interval.minimum_) ? interval.minimum_ : minimum_,
						(maximum_ > interval.maximum_) ? interval.maximum_ : maximum_);
				}
				else
				{
					return new Interval(minimum_);
				}
			}
			else
			{
				return new Interval(maximum_);
			}
		}

		internal float minimum_;
		internal float maximum_;
	}
}
