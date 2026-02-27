/*
 *  Name: AxisAlignedBox
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(AxisAlignedBoxConverter))]
	public struct AxisAlignedBox : IFormattable, IEquatable<AxisAlignedBox>
	{
        public static readonly AxisAlignedBox Empty =
			new AxisAlignedBox(new Vector3(Single.PositiveInfinity, Single.PositiveInfinity, Single.PositiveInfinity),
				new Vector3(Single.NegativeInfinity, Single.NegativeInfinity, Single.NegativeInfinity));

        public AxisAlignedBox(Vector3 minimum, Vector3 maximum)
        {
            minimum_ = minimum;
            maximum_ = maximum;
        }

		public AxisAlignedBox(Vector3 dimensions)
        {
			minimum_ = -0.5f*dimensions;
			maximum_ = 0.5f*dimensions;
        }

        public static bool operator ==(AxisAlignedBox lhs, AxisAlignedBox rhs)
        {
            return (lhs.minimum_ == rhs.minimum_) && (lhs.maximum_ == rhs.maximum_);
        }

        public static bool operator !=(AxisAlignedBox lhs, AxisAlignedBox rhs)
        {
            return (lhs.minimum_ != rhs.minimum_) || (lhs.maximum_ != rhs.maximum_);
        }

        public readonly override bool Equals(object other)
        {
            if (other is AxisAlignedBox rhs)
                return (minimum_ == rhs.minimum_) && (maximum_ == rhs.maximum_);

            return false;
        }

		public readonly bool Equals(AxisAlignedBox other)
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

		public static AxisAlignedBox Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new AxisAlignedBox(new Vector3(Functions.ParseSingle(m[0]), Functions.ParseSingle(m[1]), Functions.ParseSingle(m[2])),
				new Vector3(Functions.ParseSingle(m[3]), Functions.ParseSingle(m[4]), Functions.ParseSingle(m[5])));
		}

		public static AxisAlignedBox Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 6)
				throw new FormatException();

			return new AxisAlignedBox(new Vector3(Functions.ParseSingle(m[0], provider), Functions.ParseSingle(m[1], provider), Functions.ParseSingle(m[2], provider)),
				new Vector3(Functions.ParseSingle(m[3], provider), Functions.ParseSingle(m[4], provider), Functions.ParseSingle(m[5], provider)));
		}

		public static AxisAlignedBox FromOrientedBox(in OrientedBox b)
		{
			Vector3 hl = new Vector3(Math.Abs(b.halfDims_.x_ * b.basis_[0].x_) + Math.Abs(b.halfDims_.y_ * b.basis_[1].x_) + Math.Abs(b.halfDims_.z_ * b.basis_[2].x_),
				Math.Abs(b.halfDims_.x_ * b.basis_[0].y_) + Math.Abs(b.halfDims_.y_ * b.basis_[1].y_) + Math.Abs(b.halfDims_.z_ * b.basis_[2].y_),
				Math.Abs(b.halfDims_.x_ * b.basis_[0].z_) + Math.Abs(b.halfDims_.y_ * b.basis_[1].z_) + Math.Abs(b.halfDims_.z_ * b.basis_[2].z_));
			return new AxisAlignedBox(b.center_ - hl, b.center_ + hl);
		}

		[Browsable(false)]
		public readonly bool IsFinite => minimum_.IsFinite && maximum_.IsFinite;

		[Browsable(false)]
        public readonly bool IsEmpty => minimum_.AnyGreaterThan(maximum_);

		public Vector3 Minimum
        {
			readonly get => minimum_;
            set => minimum_ = value;
        }

        public Vector3 Maximum
        {
			readonly get => maximum_;
            set => maximum_ = value;
        }

        [Browsable(false)]
        public readonly Vector3 Dimensions => (maximum_ - minimum_);

        [Browsable(false)]
        public readonly Vector3 HalfDimensions => (maximum_ - minimum_)*0.5f;

        [Browsable(false)]
        public readonly Vector3 Center => (minimum_ + maximum_)*0.5f;

        [Browsable(false)]
        public readonly float Diagonal => Vector3.Distance(minimum_, maximum_);

		[Browsable(false)]
		public readonly float Area
		{
			get
			{
				Vector3 dim = maximum_ - minimum_;
				return 2f*(dim.x_*dim.y_ + dim.y_*dim.z_ + dim.z_*dim.x_);
			}
		}

		[Browsable(false)]
		public readonly float Volume
		{
			get
			{
				Vector3 dim = maximum_ - minimum_;
				return dim.x_*dim.y_*dim.z_;
			}
		}

		public readonly Sphere GetCircumscribedSphere()
		{
			Vector3 center = (minimum_ + maximum_)*0.5f;
			return new Sphere(center, Vector3.Distance(center, maximum_));
		}

		//public Vector3[] GetVertices()
		//{
		//    return new Vector3[8] { minimum_, new Vector3(maximum_.x_, minimum_.y_, minimum_.z_),
		//		new Vector3(minimum_.x_, maximum_.y_, minimum_.z_), new Vector3(maximum_.x_, maximum_.y_, minimum_.z_),
		//		new Vector3(minimum_.x_, minimum_.y_, maximum_.z_), new Vector3(maximum_.x_, minimum_.y_, maximum_.z_),
		//		new Vector3(minimum_.x_, maximum_.y_, maximum_.z_), maximum_ };
		//}

		//public HalfSpace[] GetHalfSpaces()
		//{
		//    return new HalfSpace[6] { new HalfSpace(new Vector3(-1f, 0f, 0f), new Vector3(minimum_.x_, 0f, 0f)),
		//		new HalfSpace(new Vector3(1f, 0f, 0f), new Vector3(maximum_.x_, 0f, 0f)),
		//		new HalfSpace(new Vector3(0f, -1f, 0f), new Vector3(0f, minimum_.y_, 0f)),
		//		new HalfSpace(new Vector3(0f, 1f, 0f), new Vector3(0f, maximum_.y_, 0f)),
		//		new HalfSpace(new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, minimum_.z_)),
		//		new HalfSpace(new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, maximum_.z_)) };
		//}

		public readonly IEnumerable<Vector3> GetVertices()
		{
			yield return minimum_;
	        yield return new Vector3(maximum_.x_, minimum_.y_, minimum_.z_);
	        yield return new Vector3(minimum_.x_, maximum_.y_, minimum_.z_);
	        yield return new Vector3(maximum_.x_, maximum_.y_, minimum_.z_);
	        yield return new Vector3(minimum_.x_, minimum_.y_, maximum_.z_);
	        yield return new Vector3(maximum_.x_, minimum_.y_, maximum_.z_);
	        yield return new Vector3(minimum_.x_, maximum_.y_, maximum_.z_);
	        yield return maximum_;
		}

		public readonly IEnumerable<HalfSpace> GetHalfSpaces()
		{
			yield return new HalfSpace(new Vector3(-1f, 0f, 0f), new Vector3(minimum_.x_, 0f, 0f));
			yield return new HalfSpace(new Vector3(1f, 0f, 0f), new Vector3(maximum_.x_, 0f, 0f));
			yield return new HalfSpace(new Vector3(0f, -1f, 0f), new Vector3(0f, minimum_.y_, 0f));
			yield return new HalfSpace(new Vector3(0f, 1f, 0f), new Vector3(0f, maximum_.y_, 0f));
			yield return new HalfSpace(new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, minimum_.z_));
			yield return new HalfSpace(new Vector3(0f, 0f, 1f), new Vector3(0f, 0f, maximum_.z_));
		}

        public static AxisAlignedBox Union(in AxisAlignedBox a, in AxisAlignedBox b)
        {
            return new AxisAlignedBox(Vector3.Min(a.minimum_, b.minimum_), Vector3.Max(a.maximum_, b.maximum_));
        }

        public static AxisAlignedBox Intersection(in AxisAlignedBox a, in AxisAlignedBox b)
        {
            return new AxisAlignedBox(Vector3.Max(a.minimum_, b.minimum_), Vector3.Min(a.maximum_, b.maximum_));
        }

        public void ExtendBy(Vector3 point)
        {
	        minimum_ = Vector3.Min(minimum_, point);
	        maximum_ = Vector3.Max(maximum_, point);
        }

        public void ExtendBy(in AxisAlignedBox box)
        {
	        minimum_ = Vector3.Min(minimum_, box.minimum_);
	        maximum_ = Vector3.Max(maximum_, box.maximum_);
        }

        public void ExtendBy(in Sphere sphere)
        {
            Vector3 r = new Vector3(sphere.radius_, sphere.radius_, sphere.radius_);
            ExtendBy(new AxisAlignedBox(sphere.center_ - r, sphere.center_ + r));
        }

		public void Inflate(Vector3 halfDims)
		{
			minimum_ -= halfDims;
			maximum_ += halfDims;
		}

		public void Translate(Vector3 offset)
		{
			minimum_ += offset;
			maximum_ += offset;
		}

		public static AxisAlignedBox Translate(AxisAlignedBox b, Vector3 offset)
		{
			b.Translate(offset);
			return b;
		}

		public void Transform(in AffineTransform at)
		{
			this = AxisAlignedBox.FromOrientedBox(OrientedBox.FromAxisAlignedBox(this, at));
		}

		public static AxisAlignedBox Transform(in AxisAlignedBox b, in AffineTransform at)
		{
			return AxisAlignedBox.FromOrientedBox(OrientedBox.FromAxisAlignedBox(b, at));
		}

		public readonly Vector3 GetClosestPoint(Vector3 point)
		{
			return Vector3.Min(Vector3.Max(point, minimum_), maximum_);
		}

		public readonly float GetDistanceTo(Vector3 point)
		{
			return Vector3.Distance(point, GetClosestPoint(point));
		}

		public readonly bool Contains(Vector3 point)
        {
	        return (minimum_.x_ <= point.x_) && (maximum_.x_ >= point.x_) && // #TODO SIMD
				(minimum_.y_ <= point.y_) && (maximum_.y_ >= point.y_) &&
		        (minimum_.z_ <= point.z_) && (maximum_.z_ >= point.z_);
        }

		public readonly bool Contains(in AxisAlignedBox box)
		{
			return (minimum_.x_ <= box.minimum_.x_) && (maximum_.x_ >= box.maximum_.x_) && // #TODO SIMD
				(minimum_.y_ <= box.minimum_.y_) && (maximum_.y_ >= box.maximum_.y_) &&
				(minimum_.z_ <= box.minimum_.z_) && (maximum_.z_ >= box.maximum_.z_);
		}

		public readonly bool Contains(in Sphere sphere)
		{
			Vector3 center = sphere.Center;
			float radius = sphere.Radius;
			return (minimum_.x_ <= (center.x_ - radius)) && (maximum_.x_ >= (center.x_ + radius)) && // #TODO SIMD
				(minimum_.y_ <= (center.y_ - radius)) && (maximum_.y_ >= (center.y_ + radius)) &&
				(minimum_.z_ <= (center.z_ - radius)) && (maximum_.z_ >= (center.z_ + radius));
		}

		public readonly bool Intersects(in Line3 line)
        {
			return FindIntersection(line).HasValue;
        }

        public readonly bool Intersects(in Ray3 ray)
        {
			return FindIntersection(ray).HasValue;
        }

		public readonly bool Intersects(in Segment3 segment)
		{
			return FindIntersection(segment).HasValue;
		}

		public readonly bool Intersects(in HalfSpace halfSpace)
		{
			Vector3 halfDims = (maximum_ - minimum_)*0.5f;
			Vector3 center = (minimum_ + maximum_)*0.5f;
			Vector3 normal = halfSpace.Normal;
			float r = Math.Abs(halfDims.x_*normal.X) + Math.Abs(halfDims.y_*normal.Y) + Math.Abs(halfDims.z_*normal.Z); // #TODO SIMD
			return ((Vector3.Dot(normal, center) + halfSpace.d_) <= r);
		}

		public readonly bool Intersects(in Plane plane)
		{
			Vector3 halfDims = (maximum_ - minimum_)*0.5f;
			Vector3 center = (minimum_ + maximum_)*0.5f;
			Vector3 normal = plane.Normal;
			float r = Math.Abs(halfDims.x_*normal.X) + Math.Abs(halfDims.y_*normal.Y) + Math.Abs(halfDims.z_*normal.Z); // #TODO SIMD
			return (Math.Abs(Vector3.Dot(normal, center) + plane.d_) <= r);
		}

		public readonly bool Intersects(in Triangle3 triangle)
		{
			Vector3 center = (minimum_ + maximum_)*0.5f;
			return IntersectAxisAlignedBoxTriangle((maximum_ - minimum_)*0.5f,
				triangle.Vertex0 - center, triangle.Vertex1 - center, triangle.Vertex2 - center);
		}

		public readonly bool Intersects(in AxisAlignedBox box)
        {
            return (minimum_.x_ <= box.maximum_.x_) && (maximum_.x_ >= box.minimum_.x_) && // #TODO SIMD
				(minimum_.y_ <= box.maximum_.y_) && (maximum_.y_ >= box.minimum_.y_) &&
                (minimum_.z_ <= box.maximum_.z_) && (maximum_.z_ >= box.minimum_.z_);
        }

		public readonly bool Intersects(in OrientedBox box)
		{
			return box.Intersects(this);
		}

		public readonly bool Intersects(in Sphere sphere)
		{
			return sphere.Intersects(this);
		}

        public readonly Interval? FindIntersection(in Line3 line)
        {
			Vector3 invDir = 1f/line.direction_;
            
            float tMin, tMax;
            if (line.direction_.x_ >= 0f)
            {
                tMin = (minimum_.x_ - line.origin_.x_)*invDir.x_;
                tMax = (maximum_.x_ - line.origin_.x_)*invDir.x_;
            }
            else
            {
                tMin = (maximum_.x_ - line.origin_.x_)*invDir.x_;
                tMax = (minimum_.x_ - line.origin_.x_)*invDir.x_;
            }
            
            float tyMin, tyMax;
            if (line.direction_.y_ >= 0f)
            {
                tyMin = (minimum_.y_ - line.origin_.y_)*invDir.y_;
                tyMax = (maximum_.y_ - line.origin_.y_)*invDir.y_;
            }
            else
            {
                tyMin = (maximum_.y_ - line.origin_.y_)*invDir.y_;
                tyMax = (minimum_.y_ - line.origin_.y_)*invDir.y_;
            }

			if ((tMin > tyMax) || (tyMin > tMax))
				return null;

            if (tyMin > tMin) 
				tMin = tyMin;
            if (tyMax < tMax) 
				tMax = tyMax;

            float tzMin, tzMax;
            if (line.direction_.z_ >= 0f)
            {
                tzMin = (minimum_.z_ - line.origin_.z_)*invDir.z_;
                tzMax = (maximum_.z_ - line.origin_.z_)*invDir.z_;
            }
            else
            {
                tzMin = (maximum_.z_ - line.origin_.z_)*invDir.z_;
                tzMax = (minimum_.z_ - line.origin_.z_)*invDir.z_;
            }

			if ((tMin > tzMax) || (tzMin > tMax))
				return null;

            if (tzMin > tMin) 
				tMin = tzMin;
            if (tzMax < tMax) 
				tMax = tzMax;

			if (tMin > tMax)
				return null;   // #TODO Check for Empty box

			return new Interval(tMin, tMax);
		}

		public readonly Interval? FindIntersection(in Ray3 ray)
		{
			Interval? intersection = FindIntersection(new Line3(ray.origin_, ray.direction_));

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

		public readonly Interval? FindIntersection(in Segment3 segment)
		{
			Interval? intersection = FindIntersection(new Line3(segment.start_, segment.end_ - segment.start_));

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

		internal static bool IntersectAxisAlignedBoxTriangle(/*Vector3 center,*/ Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/

			Vector3 e0 = v1 - v0;
			Vector3 e1 = v2 - v1;
			Vector3 e2 = v0 - v2;

			float fex = Math.Abs(e0.X);
			float fey = Math.Abs(e0.Y);
			float fez = Math.Abs(e0.Z);
			if (!TestAxisX01(e0.Z, e0.Y, fez, fey, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisY02(e0.Z, e0.X, fez, fex, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisZ12(e0.Y, e0.X, fey, fex, halfDims, v0, v1, v2))
				return false;
	
			fex = Math.Abs(e1.X);
			fey = Math.Abs(e1.Y);
			fez = Math.Abs(e1.Z);
			if (!TestAxisX01(e1.Z, e1.Y, fez, fey, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisY02(e1.Z, e1.X, fez, fex, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisZ0(e1.Y, e1.X, fey, fex, halfDims, v0, v1, v2))
				return false;
	
			fex = Math.Abs(e2.X);
			fey = Math.Abs(e2.Y);
			fez = Math.Abs(e2.Z);
			if (!TestAxisX2(e2.Z, e2.Y, fez, fey, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisY1(e2.Z, e2.X, fez, fex, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisZ12(e2.Y, e2.X, fey, fex, halfDims, v0, v1, v2))
				return false;

			float minimum = Math.Min(Math.Min(v0.X, v1.X), v2.X);
			float maximum = Math.Max(Math.Max(v0.X, v1.X), v2.X);
			if ((minimum > halfDims.X) || (maximum< -halfDims.X))
				return false;

			minimum = Math.Min(Math.Min(v0.Y, v1.Y), v2.Y);
			maximum = Math.Max(Math.Max(v0.Y, v1.Y), v2.Y);
			if ((minimum > halfDims.Y) || (maximum< -halfDims.Y))
				return false;

			minimum = Math.Min(Math.Min(v0.Z, v1.Z), v2.Z);
			maximum = Math.Max(Math.Max(v0.Z, v1.Z), v2.Z);
			if ((minimum > halfDims.Z) || (maximum< -halfDims.Z))
				return false;

			Vector3 normal = Vector3.Cross(e0, e1);
			//return (Math.Abs(Vector3.Dot(normal, v0)) <= Math.Abs(halfDims.X*normal.X) + Math.Abs(halfDims.Y*normal.Y) + Math.Abs(halfDims.Z*normal.Z));

			Vector3 minV = -halfDims;
			Vector3 maxV = halfDims;

			if (normal.X <= 0.0f)
				SwapX(ref minV, ref maxV);
			if (normal.Y <= 0.0f)
				SwapY(ref minV, ref maxV);
			if (normal.Z <= 0.0f)
				SwapZ(ref minV, ref maxV);

			minV -= v0;
			maxV -= v0;

			if (Vector3.Dot(normal, minV) > 0.0f) 
				return false;

			if (Vector3.Dot(normal, maxV) >= 0.0f) 
				return true;
	
			return false;
		}

		private static void SwapX(ref Vector3 a, ref Vector3 b)
		{
			float temp = a.X;
			a.X = b.X;
			b.X = temp;
		}

		private static void SwapY(ref Vector3 a, ref Vector3 b)
		{
			float temp = a.Y;
			a.Y = b.Y;
			b.Y = temp;
		}

		private static void SwapZ(ref Vector3 a, ref Vector3 b)
		{
			float temp = a.Z;
			a.Z = b.Z;
			b.Z = temp;
		}

		private static bool TestAxisX01(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)			   
		{
			float p0 = a*v0.Y - b*v0.Z;
			float p2 = a*v2.Y - b*v2.Z;

			float minimum, maximum;
			if (p0 < p2)
			{
				minimum = p0; 
				maximum = p2;
			} 
			else 
			{
				minimum = p2; 
				maximum = p0;
			}

			float rad = fa*halfDims.Y + fb*halfDims.Z;   
			return !((minimum > rad) || (maximum< -rad));
		}

		private static bool TestAxisX2(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			float p0 = a*v0.Y - b*v0.Z;
			float p1 = a*v1.Y - b*v1.Z;

			float minimum, maximum;
			if (p0 < p1)
			{
				minimum = p0;
				maximum = p1;
			}
			else
			{
				minimum = p1;
				maximum = p0;
			}

			float rad = fa*halfDims.Y + fb*halfDims.Z;
			return !((minimum > rad) || (maximum < -rad));
		}

		private static bool TestAxisY02(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			float p0 = -a*v0.X + b*v0.Z;
			float p2 = -a*v2.X + b*v2.Z;

			float minimum, maximum;
			if (p0 < p2)
			{
				minimum = p0;
				maximum = p2;
			}
			else
			{
				minimum = p2;
				maximum = p0;
			}

			float rad = fa*halfDims.X + fb*halfDims.Z;
			return !((minimum > rad) || (maximum < -rad));
		}

		private static bool TestAxisY1(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			float p0 = -a*v0.X + b*v0.Z;
			float p1 = -a*v1.X + b*v1.Z;

			float minimum, maximum;
			if (p0 < p1)
			{
				minimum = p0;
				maximum = p1;
			}
			else
			{
				minimum = p1;
				maximum = p0;
			}

			float rad = fa*halfDims.X + fb*halfDims.Z;
			return !((minimum > rad) || (maximum < -rad));
		}

		private static bool TestAxisZ12(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			float p1 = a*v1.X - b*v1.Y;
			float p2 = a*v2.X - b*v2.Y;

			float minimum, maximum;
			if (p2 < p1)
			{
				minimum = p2;
				maximum = p1;
			}
			else
			{
				minimum = p1;
				maximum = p2;
			}

			float rad = fa*halfDims.X + fb*halfDims.Y;
			return !((minimum > rad) || (maximum < -rad));
		}

		private static bool TestAxisZ0(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			float p0 = a*v0.X - b*v0.Y;
			float p1 = a*v1.X - b*v1.Y;

			float minimum, maximum;
			if (p0 < p1)
			{
				minimum = p0;
				maximum = p1;
			}
			else
			{
				minimum = p1;
				maximum = p0;
			}

			float rad = fa*halfDims.X + fb*halfDims.Y;
			return !((minimum > rad) || (maximum < -rad));
		}

		internal Vector3 minimum_;
		internal Vector3 maximum_;
	}
}
