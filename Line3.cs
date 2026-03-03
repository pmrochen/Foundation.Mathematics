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

        public void Normalize()
        {
            direction_.Normalize();
        }

		public static Line3 Normalize(Line3 l)
		{
			l.Normalize();
			return l;
		}

		public void Translate(Vector3 offset)
		{
			origin_ += offset;
		}

		public static Line3 Translate(Line3 l, Vector3 offset)
		{
			l.Translate(offset);
			return l;
		}

		public void Transform(in AffineTransform t)
        {
			origin_.Transform(t);
			direction_.Transform(t.r_);
        }

		public static Line3 Transform(Line3 l, in AffineTransform t)
		{
			l.Transform(t);
			return l;
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
			return (Math.Abs(Vector3.Dot(plane.Normal, direction_)) >= 1e-6f);
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
			Vector3 diff = origin_ - sphere.center_;
			float a = Vector3.Dot(direction_, direction_);
			float b = 2f*Vector3.Dot(direction_, diff);
			float c = Vector3.Dot(diff, diff) - sphere.radius_*sphere.radius_;
			return !((b*b - 4f*a*c) < 0f);
		}

		public readonly bool Intersects(in Ellipsoid ellipsoid)
		{
			Matrix3 m = ellipsoid.Matrix;
			Vector3 diff = origin_ - ellipsoid.Center;
			Vector3 matDir = direction_*m;
			Vector3 matDiff = diff*m;
			float a2 = Vector3.Dot(direction_, matDir);
			float a1 = Vector3.Dot(direction_, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			float discr = a1*a1 - a0*a2;
			return (discr >= 0f);
		}

		public readonly float? FindIntersection(in Plane plane)
		{
			float nd = Vector3.Dot(plane.Normal, direction_);
			if (Math.Abs(nd) < SingleConstants.Tolerance)
				return null;
			return (-plane.D - Vector3.Dot(plane.Normal, origin_))/nd;
		}

		public readonly float? FindIntersection(in Triangle3 triangle)
		{
			// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/

			Vector3 edge1 = triangle.vertex1_ - triangle.vertex0_;
			Vector3 edge2 = triangle.vertex2_ - triangle.vertex0_;
			Vector3 pvec = Vector3.Cross(direction_, edge2);
			float det = Vector3.Dot(edge1, pvec);
			if (Math.Abs(det) < 1e-6f)
				return null;

			float invDet = 1f/det;
			Vector3 tvec = origin_ - triangle.vertex0_;
			float u = Vector3.Dot(tvec, pvec)*invDet;
			if ((u < 0f) || (u > 1f))
				return null;

			Vector3 qvec = Vector3.Cross(tvec, edge1);
			float v = Vector3.Dot(direction_, qvec)*invDet;
			if ((v < 0f) || ((u + v) > 1f))
				return null;

			return Vector3.Dot(edge2, qvec)*invDet;
		}

		public readonly Interval? FindIntersection(in AxisAlignedBox box)
		{
			Vector3 invDir = 1f/direction_;

			float tMin, tMax;
			if (direction_.x_ >= 0f)
			{
				tMin = (box.minimum_.x_ - origin_.x_)*invDir.x_;
				tMax = (box.maximum_.x_ - origin_.x_)*invDir.x_;
			}
			else
			{
				tMin = (box.maximum_.x_ - origin_.x_)*invDir.x_;
				tMax = (box.minimum_.x_ - origin_.x_)*invDir.x_;
			}

			float tyMin, tyMax;
			if (direction_.y_ >= 0f)
			{
				tyMin = (box.minimum_.y_ - origin_.y_)*invDir.y_;
				tyMax = (box.maximum_.y_ - origin_.y_)*invDir.y_;
			}
			else
			{
				tyMin = (box.maximum_.y_ - origin_.y_)*invDir.y_;
				tyMax = (box.minimum_.y_ - origin_.y_)*invDir.y_;
			}

			if ((tMin > tyMax) || (tyMin > tMax))
				return null;

			if (tyMin > tMin)
				tMin = tyMin;
			if (tyMax < tMax)
				tMax = tyMax;

			float tzMin, tzMax;
			if (direction_.z_ >= 0f)
			{
				tzMin = (box.minimum_.z_ - origin_.z_)*invDir.z_;
				tzMax = (box.maximum_.z_ - origin_.z_)*invDir.z_;
			}
			else
			{
				tzMin = (box.maximum_.z_ - origin_.z_)*invDir.z_;
				tzMax = (box.minimum_.z_ - origin_.z_)*invDir.z_;
			}

			if ((tMin > tzMax) || (tzMin > tMax))
				return null;

			if (tzMin > tMin)
				tMin = tzMin;
			if (tzMax < tMax)
				tMax = tzMax;

			if (tMin > tMax)
				return null;

			return new Interval(tMin, tMax);
		}

		public readonly Interval? FindIntersection(in OrientedBox box)
		{
			Matrix3 basisTranspose = Matrix3.Transpose(box.basis_);
			Vector3 boxMin = -box.halfDims_;
			Vector3 boxMax = box.halfDims_;
			Vector3 pos = (origin_ - box.center_)*basisTranspose;
			Vector3 invDir = 1f/(direction_*basisTranspose);

			float tMin, tMax;
			if (invDir.x_ >= 0f)
			{
				tMin = (boxMin.x_ - pos.x_)*invDir.x_;
				tMax = (boxMax.x_ - pos.x_)*invDir.x_;
			}
			else
			{
				tMin = (boxMax.x_ - pos.x_)*invDir.x_;
				tMax = (boxMin.x_ - pos.x_)*invDir.x_;
			}

			float tyMin, tyMax;
			if (invDir.y_ >= 0f)
			{
				tyMin = (boxMin.y_ - pos.y_)*invDir.y_;
				tyMax = (boxMax.y_ - pos.y_)*invDir.y_;
			}
			else
			{
				tyMin = (boxMax.y_ - pos.y_)*invDir.y_;
				tyMax = (boxMin.y_ - pos.y_)*invDir.y_;
			}

			if ((tMin > tyMax) || (tyMin > tMax))
				return null;

			if (tyMin > tMin)
				tMin = tyMin;
			if (tyMax < tMax)
				tMax = tyMax;

			float tzMin, tzMax;
			if (invDir.z_ >= 0f)
			{
				tzMin = (boxMin.z_ - pos.z_)*invDir.z_;
				tzMax = (boxMax.z_ - pos.z_)*invDir.z_;
			}
			else
			{
				tzMin = (boxMax.z_ - pos.z_)*invDir.z_;
				tzMax = (boxMin.z_ - pos.z_)*invDir.z_;
			}

			if ((tMin > tzMax) || (tzMin > tMax))
				return null;

			if (tzMin > tMin)
				tMin = tzMin;
			if (tzMax < tMax)
				tMax = tzMax;

			if (tMin > tMax)
				return null;

			return new Interval(tMin, tMax);
		}

		public readonly Interval? FindIntersection(in Sphere sphere)
		{
			Vector3 diff = origin_ - sphere.center_;
			float a = Vector3.Dot(direction_, direction_);
			float b = 2f*Vector3.Dot(direction_, diff);
			float c = Vector3.Dot(diff, diff) - sphere.radius_*sphere.radius_;
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

		public readonly Interval? FindIntersection(in Ellipsoid ellipsoid)
		{
			Matrix3 m = ellipsoid.Matrix;
			Vector3 diff = origin_ - ellipsoid.Center;
			Vector3 matDir = direction_*m;
			Vector3 matDiff = diff*m;
			float a2 = Vector3.Dot(direction_, matDir);
			float a1 = Vector3.Dot(direction_, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			float discr = a1*a1 - a0*a2;
			if (discr < 0f)
			{
				return null;
			}
			else if (discr > 1e-6f)
			{
				float root = MathF.Sqrt(discr);
				float inv = 1f/a2;
				return new Interval((-a1 - root)*inv, (-a1 + root)*inv);
			}
			else
			{
				return new Interval(-a1/a2);
			}
		}

		//public readonly Vector2? FindIntersectionPoint(in Plane plane)
		//{
		//}

		internal Vector3 origin_;
		internal Vector3 direction_;
	}
}
