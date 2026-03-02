/*
 *  Name: Circle2
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Circle2 : IFormattable, IEquatable<Circle2>
	{
        public Circle2(Vector2 center, float radius)
        {
            center_ = center;
            radius_ = radius;
        }

		public static bool operator ==(Circle2 lhs, Circle2 rhs)
		{
			return (lhs.center_ == rhs.center_) && (lhs.radius_ == rhs.radius_);
		}

		public static bool operator !=(Circle2 lhs, Circle2 rhs)
		{
			return (lhs.center_ != rhs.center_) || (lhs.radius_ != rhs.radius_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Circle2 rhs)
				return (center_ == rhs.center_) && (radius_ == rhs.radius_);

			return false;
		}

		public readonly bool Equals(Circle2 other)
		{
			return (center_ == other.center_) && (radius_ == other.radius_);
		}

		public readonly override int GetHashCode()
		{
			int hash = center_.GetHashCode();
			return ((hash << 5) + hash) ^ radius_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Circle2 other, float tolerance)
		{
			return center_.ApproxEquals(other.center_, tolerance) &&
				(Math.Abs(other.radius_ - radius_) < tolerance);
		}

		public readonly bool ApproxEquals(in Circle2 other)
		{
			return center_.ApproxEquals(other.center_) &&
				(Math.Abs(other.radius_ - radius_) < 1e-6f);
		}

		public readonly override string ToString()
		{
			return String.Concat(center_.ToString(), " ", radius_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(center_.ToString(provider), " ", radius_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(center_.ToString(format), " ", radius_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(center_.ToString(format, provider), " ", radius_.ToString(format, provider));
		}

		public static Circle2 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 3)
				throw new FormatException();

			return new Circle2(new Vector2(Single.Parse(m[0]), Single.Parse(m[1])),
					Single.Parse(m[2]));
		}

		public static Circle2 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 3)
				throw new FormatException();

			return new Circle2(new Vector2(Single.Parse(m[0], provider), Single.Parse(m[1], provider)),
					Single.Parse(m[2], provider));
		}

		[Browsable(false)]
		public readonly bool IsFinite => center_.IsFinite && Functions.IsFinite(radius_);

		public Vector2 Center
        {
			readonly get => center_;
            set => center_ = value;
        }

        public float Radius
        {
			readonly get => radius_;
            set => radius_ = value;
        }

        [Browsable(false)]
        public float Diameter
        {
			readonly get => radius_*2f;
            set => radius_ = value*0.5f;
        }

		[Browsable(false)]
		public readonly float Circumference => 2f*SingleConstants.Pi*radius_;

		[Browsable(false)]
		public readonly float Area => SingleConstants.Pi*radius_*radius_;

		public readonly AxisAlignedRectangle GetCircumscribedRectangle()
		{
			Vector2 h = new Vector2(radius_, radius_);
			return new AxisAlignedRectangle(center_ - h, center_ + h);
		}

		//public static Circle2 FromEllipse(in Ellipse2 ellipse)
		//{
		//	return new Circle2(ellipse.center_, Math.Max(ellipse.radii_.X, ellipse.radii_.Y));
		//}

		//public readonly Vector2 GetClosestPoint(Vector2 point)
		//{
		//  // #TODO
		//}

		public readonly float GetDistanceTo(Vector2 point)
		{
			return Math.Max(Vector2.Distance(point, center_) - radius_, 0f);
		}

		public readonly float GetSignedDistanceTo(Vector2 point)
		{
			return Vector2.Distance(point, center_) - radius_;
		}

		public readonly bool Contains(Vector2 point)
		{
			return (Vector2.DistanceSquared(point, center_) <= radius_*radius_);
		}

		public readonly bool Intersects(in Line2 line)
        {
			Vector2 diff = line.origin_ - center_;
			float a = Vector2.Dot(line.direction_, line.direction_);
			float b = 2f*Vector2.Dot(line.direction_, diff);
			float c = Vector2.Dot(diff, diff) - radius_*radius_;
			return !((b*b - 4f*a*c) < 0f);
        }

        public readonly bool Intersects(in Ray2 ray)
        {
			return FindIntersection(ray).HasValue;
        }

		public readonly bool Intersects(in Segment2 segment)
		{
			return FindIntersection(segment).HasValue;
		}

		public readonly bool Intersects(in AxisAlignedRectangle rectangle)
        {
			float d = 0f;

			if (center_.X < rectangle.minimum_.X)
			{
				float s = center_.X - rectangle.minimum_.X;
				d += s*s;
			}
			else if (center_.X > rectangle.maximum_.X)
			{
				float s = center_.X - rectangle.maximum_.X;
				d += s*s;
			}

			if (center_.Y < rectangle.minimum_.Y)
			{
				float s = center_.Y - rectangle.minimum_.Y;
				d += s*s;
			}
			else if (center_.Y > rectangle.maximum_.Y)
			{
				float s = center_.Y - rectangle.maximum_.Y;
				d += s*s;
			}

			return (d <= radius_*radius_);
		}

		public readonly bool Intersects(in Circle2 circle)
        {
            float d = circle.radius_ + radius_;
            return (Vector2.DistanceSquared(circle.center_, center_) <= d*d);
        }

		public readonly Interval? FindIntersection(in Line2 line)
		{
			Vector2 diff = line.origin_ - center_;
			float a = Vector2.Dot(line.direction_, line.direction_);
			float b = 2f*Vector2.Dot(line.direction_, diff);
			float c = Vector2.Dot(diff, diff) - radius_*radius_;
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

		internal Vector2 center_;
		internal float radius_;
	}
}
