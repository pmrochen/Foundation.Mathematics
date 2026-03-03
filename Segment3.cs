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
			Matrix3 m = ellipsoid.Matrix;
			Vector3 direction = end_ - start_;
			Vector3 diff = Vector3.Lerp(start_, end_, 0.5f) - ellipsoid.Center;
			Vector3 matDir = direction*m;
			Vector3 matDiff = diff*m;
			float a2 = Vector3.Dot(direction, matDir);
			float a1 = Vector3.Dot(direction, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			float discr = a1*a1 - a0*a2;
			if (discr < 0f)
				return false;
			if (a0 <= 0f)
				return true;

			const float e = 0.5f;
			if (a1 >= 0f)
			{
				float q = a0 + e*(-2f*a1 + a2*e);
				if (q <= 0f)
					return true;

				float qder = a1 - a2*e;
				if (qder < 0f)
					return true;
			}
			else
			{
				float q = a0 + e*(2f*a1 + a2*e);
				if (q <= 0f)
					return true;

				float qder = a1 + a2*e;
				if (qder < 0f)
					return true;
			}

			return false;
		}

		public readonly float? FindIntersection(in Plane plane)
		{
			float? t = new Line3(start_, end_ - start_).FindIntersection(plane);
			return (t.HasValue && (t.Value >= 0f) && (t.Value <= 1f)) ? t : null;
		}

		public readonly float? FindIntersection(in Triangle3 triangle)
		{
			// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/

			Vector3 edge1 = triangle.vertex1_ - triangle.vertex0_;
			Vector3 edge2 = triangle.vertex2_ - triangle.vertex0_;
			Vector3 direction = end_ - start_;
			Vector3 pvec = Vector3.Cross(direction, edge2);
			float det = Vector3.Dot(edge1, pvec);
			if (Math.Abs(det) < 1e-6f)
				return null;

			float invDet = 1f/det;
			Vector3 tvec = start_ - triangle.vertex0_;
			float u = Vector3.Dot(tvec, pvec)*invDet;
			if ((u < 0f) || (u > 1f))
				return null;

			Vector3 qvec = Vector3.Cross(tvec, edge1);
			float v = Vector3.Dot(direction, qvec)*invDet;
			if ((v < 0f) || ((u + v) > 1f))
				return null;

			float t = Vector3.Dot(edge2, qvec)*invDet;
			if ((t >= 0f) && (t <= 1f))
				return t;
			else
				return null;
		}

		public readonly Interval? FindIntersection(in AxisAlignedBox box)
		{
			Interval? intersection = new Line3(start_, end_ - start_).FindIntersection(box);

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

		public readonly Interval? FindIntersection(in OrientedBox box)
		{
			Interval? intersection = new Line3(start_, end_ - start_).FindIntersection(box);

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

		public readonly Interval? FindIntersection(in Sphere sphere)
		{
			Interval? intersection = new Line3(start_, end_ - start_).FindIntersection(sphere);

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

		public readonly Interval? FindIntersection(in Ellipsoid ellipsoid)
		{
			Matrix3 m = ellipsoid.Matrix;
			Vector3 direction = end_ - start_;
			Vector3 diff = Vector3.Lerp(start_, end_, 0.5f) - ellipsoid.Center;
			Vector3 matDir = direction*m;
			Vector3 matDiff = diff*m;
			float a2 = Vector3.Dot(direction, matDir);
			float a1 = Vector3.Dot(direction, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			const float e = 0.5f;
			float discr = a1*a1 - a0*a2;
			if (discr < 0f)
			{
				return null;
			}
			else if (discr > 1e-6f)
			{
				float root = MathF.Sqrt(discr);
				float inv = 1f/a2;
				float t0 = (-a1 - root)*inv;
				float t1 = (-a1 + root)*inv;

				Interval? intersection = new Interval(t0, t1).FindIntersection(new Interval(-e, e));
				if (intersection.HasValue)
					return new Interval(intersection.Value.Minimum + e, intersection.Value.Maximum + e);
				else
					return null;
			}
			else
			{
				float t0 = -a1/a2;
				if (Math.Abs(t0) <= e)
					return new Interval(t0 + e);
				else
					return null;
			}
		}

		//public readonly Vector2? FindIntersectionPoint(in Plane plane)
		//{
		//}

		internal Vector3 start_;
		internal Vector3 end_;
	}
}
