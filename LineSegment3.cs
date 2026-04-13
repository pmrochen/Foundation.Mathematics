/*
 *  Name: LineSegment3
 *  Author: Pawel Mrochen
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct LineSegment3 : IFormattable, IEquatable<LineSegment3>
	{
		public LineSegment3(Vector3 start, Vector3 end)
		{
			start_ = start;
			end_ = end;
		}

		//public LineSegment3(Vector3[] endPoints) :
		//	this(endPoints[0], endPoints[1])
		//{
		//}

		public static bool operator ==(LineSegment3 lhs, LineSegment3 rhs)
		{
			return (lhs.start_ == rhs.start_) && (lhs.end_ == rhs.end_);
		}

		public static bool operator !=(LineSegment3 lhs, LineSegment3 rhs)
		{
			return (lhs.start_ != rhs.start_) || (lhs.end_ != rhs.end_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is LineSegment3 rhs)
				return (start_ == rhs.start_) && (end_ == rhs.end_);

			return false;
		}

		public readonly bool Equals(LineSegment3 other)
		{
			return (start_ == other.start_) && (end_ == other.end_);
		}

		public readonly override int GetHashCode()
		{
			int hash = start_.GetHashCode();
			return ((hash << 5) + hash) ^ end_.GetHashCode();
		}

		public readonly bool ApproxEquals(in LineSegment3 other, float tolerance)
		{
			return start_.ApproxEquals(other.start_, tolerance) &&
				end_.ApproxEquals(other.end_, tolerance);
		}

		public readonly bool ApproxEquals(in LineSegment3 other)
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

		public static LineSegment3 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new LineSegment3(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
				new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])));
		}

		public static LineSegment3 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new LineSegment3(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
				new Vector3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider)));
		}

		public static LineSegment3 FromLine(in Line3 l)
		{
			return new LineSegment3(l.origin_, l.origin_ + l.direction_);
		}

		public static LineSegment3 FromLine(in Line3 l, Interval interval)
		{
			return new LineSegment3(l.Evaluate(interval.Minimum), l.Evaluate(interval.Maximum));
		}

		public static LineSegment3 FromRay(in Ray3 r)
		{
			return new LineSegment3(r.origin_, r.origin_ + r.direction_);
		}

		public static LineSegment3 FromRay(in Ray3 r, Interval interval)
		{
			return new LineSegment3(r.Evaluate(interval.Minimum), r.Evaluate(interval.Maximum));
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

		public readonly IEnumerable<Vector3> GetEndpoints()
		{
			yield return start_;
			yield return end_;
		}

		public void Translate(Vector3 offset)
		{
			start_ += offset;
			end_ += offset;
		}

		public static LineSegment3 Translate(LineSegment3 segment, Vector3 offset)
		{
			segment.Translate(offset);
			return segment;
		}

		public void Transform(in Matrix3 matrix)
		{
			start_.Transform(matrix);
			end_.Transform(matrix);
		}

		public void Transform(in AffineTransform at)
        {
			start_.Transform(at);
			end_.Transform(at);
        }

		public static LineSegment3 Transform(LineSegment3 segment, in Matrix3 matrix)
		{
			segment.Transform(matrix);
			return segment;
		}

		public static LineSegment3 Transform(LineSegment3 segment, in AffineTransform at)
		{
			segment.Transform(at);
			return segment;
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
			return Intersections.TestLineSegmentEllipsoid(start_, end_, ellipsoid.Center, ellipsoid.Matrix);
		}

		public readonly float? FindIntersection(in Plane plane)
		{
			float? result = Intersections.FindLinePlane(start_, end_ - start_, plane.Normal, plane.D);
			return (result.HasValue && (result.Value >= 0f) && (result.Value <= 1f)) ? result : null;
		}

		public readonly float? FindIntersection(in Triangle3 triangle)
		{
			//return Intersections.FindLineSegmentTriangle(start_, end_, triangle.vertex0_, triangle.vertex1_, triangle.vertex2_);
			float? result = Intersections.FindLineTriangle(start_, end_ - start_, triangle.vertex0_, triangle.vertex1_, triangle.vertex2_);
			return (result.HasValue && (result.Value >= 0f) && (result.Value <= 1f)) ? result : null;
		}

		public readonly Interval? FindIntersection(in AxisAlignedBox box)
		{
			Interval? intersection = Intersections.FindLineAxisAlignedBox(start_, end_ - start_, box.minimum_, box.maximum_);

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
			Interval? intersection = Intersections.FindLineOrientedBox(start_, end_ - start_, box.center_, box.basis_, box.halfDims_);

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
			Interval? intersection = Intersections.FindLineSphere(start_, end_ - start_, sphere.center_, sphere.radius_);

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
			return Intersections.FindLineSegmentEllipsoid(start_, end_, ellipsoid.Center, ellipsoid.Matrix);
		}

		//public readonly Vector2? FindIntersectionPoint(in Plane plane)
		//{
		//}

		internal Vector3 start_;
		internal Vector3 end_;
	}
}
