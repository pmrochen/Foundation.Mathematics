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
        public static readonly AxisAlignedBox Empty = new AxisAlignedBox(Vector3.PositiveInfinity, Vector3.NegativeInfinity);

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

		public static AxisAlignedBox FromOrientedBox(in OrientedBox box)
		{
			Vector3 halfDims = Vector3.Abs(box.halfDims_.x_*box.basis_.Row0) + Vector3.Abs(box.halfDims_.y_*box.basis_.Row1) + 
				Vector3.Abs(box.halfDims_.z_*box.basis_.Row2);
			return new AxisAlignedBox(box.center_ - halfDims, box.center_ + halfDims);
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
		public readonly float SurfaceArea
		{
			get
			{
				Vector3 dim = maximum_ - minimum_;
				return 2f*(dim.x_*dim.y_ + dim.y_*dim.z_ + dim.z_*dim.x_); // 2f*Vector3.Dot(dim, dim.YZX)
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
			yield return new HalfSpace(-Vector3.UnitX, new Vector3(minimum_.x_, 0f, 0f));
			yield return new HalfSpace(Vector3.UnitX, new Vector3(maximum_.x_, 0f, 0f));
			yield return new HalfSpace(-Vector3.UnitY, new Vector3(0f, minimum_.y_, 0f));
			yield return new HalfSpace(Vector3.UnitY, new Vector3(0f, maximum_.y_, 0f));
			yield return new HalfSpace(-Vector3.UnitZ, new Vector3(0f, 0f, minimum_.z_));
			yield return new HalfSpace(Vector3.UnitZ, new Vector3(0f, 0f, maximum_.z_));
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
            Vector3 radius = new Vector3(sphere.radius_);
            ExtendBy(new AxisAlignedBox(sphere.center_ - radius, sphere.center_ + radius));
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

		public static AxisAlignedBox Translate(AxisAlignedBox box, Vector3 offset)
		{
			box.Translate(offset);
			return box;
		}

		public void Transform(in Matrix3 matrix, bool orthogonal = false)
		{
			this = AxisAlignedBox.FromOrientedBox(OrientedBox.FromAxisAlignedBox(this, matrix, orthogonal));
		}

		public void Transform(in AffineTransform at, bool orthogonal = false)
		{
			this = AxisAlignedBox.FromOrientedBox(OrientedBox.FromAxisAlignedBox(this, at, orthogonal));
		}

		public static AxisAlignedBox Transform(in AxisAlignedBox box, in Matrix3 matrix, bool orthogonal = false)
		{
			return AxisAlignedBox.FromOrientedBox(OrientedBox.FromAxisAlignedBox(box, matrix, orthogonal));
		}

		public static AxisAlignedBox Transform(in AxisAlignedBox box, in AffineTransform at, bool orthogonal = false)
		{
			return AxisAlignedBox.FromOrientedBox(OrientedBox.FromAxisAlignedBox(box, at, orthogonal));
		}

		public readonly Vector3 GetClosestPoint(Vector3 point)
		{
			return Vector3.Clamp(point, minimum_, maximum_);
		}

		public readonly float GetDistanceTo(Vector3 point)
		{
			return Vector3.Distance(point, GetClosestPoint(point));
		}

		public readonly bool Contains(Vector3 point)
        {
			return minimum_.AllLessThanEqual(point) && maximum_.AllGreaterThanEqual(point);
        }

		public readonly bool Contains(in AxisAlignedBox box)
		{
			return minimum_.AllLessThanEqual(box.minimum_) && maximum_.AllGreaterThanEqual(box.maximum_);
		}

		public readonly bool Contains(in Sphere sphere)
		{
			Vector3 radius = new Vector3(sphere.Radius);
			return minimum_.AllLessThanEqual(sphere.Center - radius) && maximum_.AllGreaterThanEqual(sphere.Center + radius);
		}

		public readonly bool Intersects(in HalfSpace halfSpace)
		{
			return Intersections.TestAxisAlignedBoxHalfSpace((minimum_ + maximum_)*0.5f, (maximum_ - minimum_)*0.5f, 
				halfSpace.Normal, halfSpace.D);
		}

		public readonly bool Intersects(in Plane plane)
		{
			return Intersections.TestAxisAlignedBoxPlane((minimum_ + maximum_)*0.5f, (maximum_ - minimum_)*0.5f, 
				plane.Normal, plane.D);
		}

		public readonly bool Intersects(in Triangle3 triangle)
		{
			return Intersections.TestAxisAlignedBoxTriangle((minimum_ + maximum_)*0.5f, (maximum_ - minimum_)*0.5f,
				triangle.Vertex0, triangle.Vertex1, triangle.Vertex2);
		}

		public readonly bool Intersects(in AxisAlignedBox box)
        {
			return minimum_.AllLessThanEqual(box.maximum_) && maximum_.AllGreaterThanEqual(box.minimum_);
        }

		public readonly bool Intersects(in OrientedBox box)
		{
			return box.Intersects(this);
		}

		public readonly bool Intersects(in Sphere sphere)
		{
			return Intersections.TestAxisAlignedBoxSphere(minimum_, maximum_, sphere.center_, sphere.radius_);
		}

		public readonly bool Intersects(in SymmetricFrustum frustum)
		{
			return frustum.Intersects(this);
		}

		internal Vector3 minimum_;
		internal Vector3 maximum_;
	}
}
