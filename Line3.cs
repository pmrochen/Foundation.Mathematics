/*
 *  Name: Line3
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Line3 : IFormattable, IEquatable<Line3>
	{
        public Line3(Vector3 origin, Vector3 direction)
        {
            origin_ = origin;
            direction_ = direction;
        }

		public static bool operator ==(Line3 lhs, Line3 rhs)
		{
			return (lhs.origin_ == rhs.origin_) && (lhs.direction_ == rhs.direction_);
		}

		public static bool operator !=(Line3 lhs, Line3 rhs)
		{
			return (lhs.origin_ != rhs.origin_) || (lhs.direction_ != rhs.direction_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Line3 rhs)
				return (origin_ == rhs.origin_) && (direction_ == rhs.direction_);

			return false;
		}

		public readonly bool Equals(Line3 other)
		{
			return (origin_ == other.origin_) && (direction_ == other.direction_);
		}

		public readonly override int GetHashCode()
		{
			int hash = origin_.GetHashCode();
			return ((hash << 5) + hash) ^ direction_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Line3 other, float tolerance)
		{
			return origin_.ApproxEquals(other.origin_, tolerance) &&
				direction_.ApproxEquals(other.direction_, tolerance);
		}

		public readonly bool ApproxEquals(in Line3 other)
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

		public static Line3 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new Line3(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
				new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])));
		}

		public static Line3 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new Line3(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
				new Vector3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider)));
		}

		public static Line3 FromRay(in Ray3 r)
		{
			return new Line3(r.origin_, r.direction_);
		}

		//public static Line3 FromSegment(in Segment3 s)
		//{
		//	return new Line3(s.Start, s.Direction);
		//}

		//public static explicit operator Line3(Ray3 r)
		//{
		//    return FromRay(r);
		//}

		public Vector3 Origin
        {
			readonly get => origin_;
            set => origin_ = value;
        }

        public Vector3 Direction
        {
			readonly get => direction_;
            set => direction_ = value;
        }

		public void Translate(Vector3 offset)
		{
			origin_ += offset;
		}

		public static Line3 Translate(Line3 line, Vector3 offset)
		{
			line.Translate(offset);
			return line;
		}

		public void Transform(in Matrix3 matrix)
		{
			origin_.Transform(matrix);
			direction_.Transform(matrix);
		}

		public void Transform(in AffineTransform at)
        {
			origin_.Transform(at);
			direction_.Transform(at.r_);
        }

		public static Line3 Transform(Line3 line, in Matrix3 matrix)
		{
			line.Transform(matrix);
			return line;
		}

		public static Line3 Transform(Line3 line, in AffineTransform at)
		{
			line.Transform(at);
			return line;
		}

		public void Normalize()
		{
			direction_.Normalize();
		}

		public static Line3 Normalize(Line3 line)
		{
			line.Normalize();
			return line;
		}

		public readonly Vector3 Evaluate(float t)
		{ 
			return (origin_ + t*direction_);
		}

		public readonly Vector3 GetClosestPoint(Vector3 point)
		{
			return Vector3.Dot(point - origin_, direction_)*direction_ + origin_;
		}

		public readonly Vector3 GetClosestPoint(Vector3 point, bool normalized)
		{
			return normalized ?
				Vector3.Dot(point - origin_, direction_)*direction_ + origin_ :
				(Vector3.Dot(point - origin_, direction_)/Vector3.Dot(direction_, direction_))*direction_ + origin_;
		}

		public readonly float GetDistanceTo(Vector3 point)
		{
			return Vector3.Distance(GetClosestPoint(point), point);
		}

		public readonly float GetDistanceTo(Vector3 point, bool normalized)
		{
			return Vector3.Distance(GetClosestPoint(point, normalized), point);
		}

		// #TODO
		//public readonly float GetDistanceTo(Line3 line)
		//{
		// 	return Math.Abs(Vector3.Dot(Vector3.Cross(direction_, line.direction_), (line.origin_ - origin_)));
		//}

		public readonly bool Intersects(in Plane plane)
		{
			return Intersections.TestLinePlane(direction_, plane.Normal);
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
			return Intersections.TestLineSphere(origin_, direction_, sphere.center_, sphere.radius_);
		}

		public readonly bool Intersects(in Ellipsoid ellipsoid)
		{
			return Intersections.TestLineEllipsoid(origin_, direction_, ellipsoid.Center, ellipsoid.Matrix);
		}

		public readonly float? FindIntersection(in Plane plane)
		{
			return Intersections.FindLinePlane(origin_, direction_, plane.Normal, plane.D);
		}

		public readonly float? FindIntersection(in Triangle3 triangle)
		{
			return Intersections.FindLineTriangle(origin_, direction_, triangle.vertex0_, triangle.vertex1_, triangle.vertex2_);
		}

		public readonly Interval? FindIntersection(in AxisAlignedBox box)
		{
			return Intersections.FindLineAxisAlignedBox(origin_, direction_, box.minimum_, box.maximum_);
		}

		public readonly Interval? FindIntersection(in OrientedBox box)
		{
			//Matrix3 basisTranspose = Matrix3.Transpose(box.basis_);
			return Intersections.FindLineAxisAlignedBox(box.basis_*(origin_ - box.center_)/*(origin_ - box.center_)*basisTranspose*/,
				box.basis_*direction_/*direction_*basisTranspose*/, -box.halfDims_, box.halfDims_);
		}

		public readonly Interval? FindIntersection(in Sphere sphere)
		{
			return Intersections.FindLineSphere(origin_, direction_, sphere.center_, sphere.radius_);
		}

		public readonly Interval? FindIntersection(in Ellipsoid ellipsoid)
		{
			return Intersections.FindLineEllipsoid(origin_, direction_, ellipsoid.Center, ellipsoid.Matrix);
		}

		//public readonly Vector2? FindIntersectionPoint(in Plane plane)
		//{
		//}

		internal Vector3 origin_;
		internal Vector3 direction_;
	}
}
