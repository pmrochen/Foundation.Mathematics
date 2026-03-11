/*
 *  Name: Sphere
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Sphere : IFormattable, IEquatable<Sphere>
	{
        public Sphere(Vector3 center, float radius)
        {
            center_ = center;
            radius_ = radius;
        }

		public static bool operator ==(Sphere lhs, Sphere rhs)
		{
			return (lhs.center_ == rhs.center_) && (lhs.radius_ == rhs.radius_);
		}

		public static bool operator !=(Sphere lhs, Sphere rhs)
		{
			return (lhs.center_ != rhs.center_) || (lhs.radius_ != rhs.radius_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Sphere rhs)
				return (center_ == rhs.center_) && (radius_ == rhs.radius_);

			return false;
		}

		public readonly bool Equals(Sphere other)
		{
			return (center_ == other.center_) && (radius_ == other.radius_);
		}

		public readonly override int GetHashCode()
		{
			int hash = center_.GetHashCode();
			return ((hash << 5) + hash) ^ radius_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Sphere other, float tolerance)
		{
			return center_.ApproxEquals(other.center_, tolerance) &&
				(Math.Abs(other.radius_ - radius_) < tolerance);
		}

		public readonly bool ApproxEquals(in Sphere other)
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

		public static Sphere Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Sphere(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])), Single.Parse(m[3]));
		}

		public static Sphere Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Sphere(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
				Single.Parse(m[3], provider));
		}

		[Browsable(false)]
		public readonly bool IsFinite => center_.IsFinite && Functions.IsFinite(radius_);

		public Vector3 Center
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
		public readonly float SurfaceArea => 4f*SingleConstants.Pi*radius_*radius_;

		[Browsable(false)]
		public readonly float Volume => 4f*SingleConstants.Pi*radius_*radius_*radius_/3f;

		public readonly AxisAlignedBox GetCircumscribedBox()
		{
			Vector3 halfDims = new Vector3(radius_);
			return new AxisAlignedBox(center_ - halfDims, center_ + halfDims);
		}

		//public readonly OrientedBox GetCircumscribedBox()
		//{
		//	Vector3 halfDims = new Vector3(radius_);
		//	return new OrientedBox(center_, Matrix3.Identity, halfDims);
		//}

		public static Sphere FromEllipsoid(in Ellipsoid ellipsoid)
		{
			return new Sphere(ellipsoid.center_, ellipsoid.radii_.MaxComponent);
		}

		public void Translate(Vector3 offset)
		{
			center_ += offset;
		}

		public static Sphere Translate(Sphere sphere, Vector3 offset)
		{
			sphere.Translate(offset);
			return sphere;
		}

		public void Transform(in Matrix3 matrix, bool orthogonal = false)
		{
			if (!orthogonal)
				this = Sphere.FromEllipsoid(Ellipsoid.FromSphere(this, matrix, orthogonal));
		}

		public void Transform(in AffineTransform at, bool orthogonal = false)
		{
			if (orthogonal)
				center_.Transform(at);
			else
				this = Sphere.FromEllipsoid(Ellipsoid.FromSphere(this, at, orthogonal));
		}

		public static Sphere Transform(in Sphere sphere, in Matrix3 matrix, bool orthogonal = false)
		{
			if (orthogonal)
				return sphere;
			else
				return Sphere.FromEllipsoid(Ellipsoid.FromSphere(sphere, matrix, orthogonal));
		}

		public static Sphere Transform(in Sphere sphere, in AffineTransform at, bool orthogonal = false)
		{
			if (orthogonal)
				return new Sphere(Vector3.Transform(sphere.center_, at), sphere.radius_);
			else
				return Sphere.FromEllipsoid(Ellipsoid.FromSphere(sphere, at, orthogonal));
		}

		//public readonly Vector3 GetClosestPoint(Vector3 point)
		//{
		//  // #TODO
		//}

		public readonly float GetDistanceTo(Vector3 point)
		{
			return Math.Max(Vector3.Distance(point, center_) - radius_, 0f);
		}

		public readonly float GetSignedDistanceTo(Vector3 point)
		{
			return Vector3.Distance(point, center_) - radius_;
		}

		public readonly bool Contains(Vector3 point)
		{
			return (Vector3.DistanceSquared(point, center_) <= radius_*radius_);
		}

		public readonly bool Intersects(in HalfSpace halfSpace)
        {
	        return ((Vector3.Dot(halfSpace.Normal, center_) + halfSpace.d_) <= radius_);
        }

        public readonly bool Intersects(in Plane plane)
        {
        	return (Math.Abs(Vector3.Dot(plane.Normal, center_) + plane.d_) <= radius_);
        }

		public readonly bool Intersects(in Triangle3 triangle)
		{
			return (Distances.GetPointTriangleSquared(center_, triangle.vertex0_, triangle.vertex1_, triangle.vertex2_) <= radius_*radius_);
		}

		public readonly bool Intersects(in AxisAlignedBox box)
        {
			return Intersections.TestAxisAlignedBoxSphere(box.minimum_, box.maximum_, center_, radius_);
		}

		public readonly bool Intersects(in OrientedBox box)
		{
			//Matrix3 boxBasisT = Matrix3.Transpose(box.basis_);
			return Intersections.TestAxisAlignedBoxSphere(-box.halfDims_, box.halfDims_,
				box.basis_*(center_ - box.center_)/*(center_ - box.center_)*boxBasisT*/, radius_);
		}

        public readonly bool Intersects(in Sphere sphere)
        {
            float d = sphere.radius_ + radius_;
            return (Vector3.DistanceSquared(sphere.center_, center_) <= d*d);
        }
		
		public readonly bool Intersects(in Cone cone)
		{
			return cone.Intersects(this);
		}

		//public readonly bool Intersects(in OpenCone cone)
		//{
		//	return cone.Intersects(this);
		//}

		public readonly bool Intersects(in SymmetricFrustum frustum)
		{
			return frustum.Intersects(this);
		}

		internal Vector3 center_;
		internal float radius_;
	}
}
