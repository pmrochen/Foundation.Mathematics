/*
 *  Name: OrientedBox
 *  Author: Pawel Mrochen
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct OrientedBox : IFormattable, IEquatable<OrientedBox>
	{
        public OrientedBox(Vector3 center, Matrix3 basis, Vector3 halfDims)
        {
			center_ = center;
			basis_ = basis;
            halfDims_ = halfDims;
        }

		public static bool operator ==(OrientedBox lhs, OrientedBox rhs)
		{
			return (lhs.center_ == rhs.center_) && (lhs.basis_ == rhs.basis_) && (lhs.halfDims_ == rhs.halfDims_);
		}

		public static bool operator !=(OrientedBox lhs, OrientedBox rhs)
		{
			return (lhs.center_ != rhs.center_) || (lhs.basis_ != rhs.basis_) || (lhs.halfDims_ != rhs.halfDims_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is OrientedBox rhs)
				return (center_ == rhs.center_) && (basis_ == rhs.basis_) && (halfDims_ == rhs.halfDims_);

			return false;
		}

		public readonly bool Equals(OrientedBox other)
		{
			return (center_ == other.center_) && (basis_ == other.basis_) && (halfDims_ == other.halfDims_);
		}

		public readonly override int GetHashCode()
		{
			int hash = center_.GetHashCode();
			hash = ((hash << 5) + hash) ^ basis_.GetHashCode();
			return ((hash << 5) + hash) ^ halfDims_.GetHashCode();
		}

		public readonly bool ApproxEquals(in OrientedBox other, float tolerance)
		{
			return center_.ApproxEquals(other.center_, tolerance) &&
				basis_.ApproxEquals(other.basis_, tolerance) &&
				halfDims_.ApproxEquals(other.halfDims_, tolerance);
		}

		public readonly bool ApproxEquals(in OrientedBox other)
		{
			return center_.ApproxEquals(other.center_) &&
				basis_.ApproxEquals(other.basis_) &&
				halfDims_.ApproxEquals(other.halfDims_);
		}

		public readonly override string ToString()
		{
			return String.Concat(center_.ToString(), " ", basis_.ToString(), " ", halfDims_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(center_.ToString(provider), " ", basis_.ToString(provider), " ", halfDims_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(center_.ToString(format), " ", basis_.ToString(format), " ", halfDims_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(center_.ToString(format, provider), " ", basis_.ToString(format, provider), " ", halfDims_.ToString(format, provider));
		}

		public static OrientedBox Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 15)
				throw new FormatException();

			return new OrientedBox(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
						   new Matrix3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5]),
									   Single.Parse(m[6]), Single.Parse(m[7]), Single.Parse(m[8]),
									   Single.Parse(m[9]), Single.Parse(m[10]), Single.Parse(m[11])),
						   new Vector3(Single.Parse(m[12]), Single.Parse(m[13]), Single.Parse(m[14])));
		}

		public static OrientedBox Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 15)
				throw new FormatException();

			return new OrientedBox(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
						   new Matrix3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider),
									   Single.Parse(m[6], provider), Single.Parse(m[7], provider), Single.Parse(m[8], provider),
									   Single.Parse(m[9], provider), Single.Parse(m[10], provider), Single.Parse(m[11], provider)),
						   new Vector3(Single.Parse(m[12], provider), Single.Parse(m[13], provider), Single.Parse(m[14], provider)));
		}

		public static OrientedBox FromAxisAlignedBox(in AxisAlignedBox b)
		{
			return new OrientedBox(b.Center, Matrix3.Identity, b.HalfDimensions);
		}

		public static OrientedBox FromAxisAlignedBox(in AxisAlignedBox b, in Matrix3 matrix, bool orthogonal = false)
		{
			OrientedBox box = new OrientedBox(Vector3.Transform(b.Center, matrix), matrix, b.HalfDimensions);
			if (!orthogonal)
				box.Orthonormalize();
			return box;
		}

		public static OrientedBox FromAxisAlignedBox(in AxisAlignedBox b, in AffineTransform at, bool orthogonal = false)
		{
			OrientedBox box = new OrientedBox(Vector3.Transform(b.Center, at), at.r_, b.HalfDimensions);
			if (!orthogonal)
				box.Orthonormalize();
			return box;
		}

		//public static implicit operator OrientedBox(AxisAlignedBox b)
		//{
		//    return FromAxisAlignedBox(b);
		//}

		[Browsable(false)]
		public readonly bool IsFinite => center_.IsFinite && basis_.IsFinite && halfDims_.IsFinite;

		public Vector3 Center
		{
			readonly get => center_;
			set => center_ = value;
		}

		public Matrix3 Basis
		{
			readonly get => basis_;
			set => basis_ = value;
		}

		public Vector3 HalfDimensions
		{
			readonly get => halfDims_;
			set => halfDims_ = value;
		}

		[Browsable(false)]
		public Vector3 Dimensions
		{
			readonly get => halfDims_*2f;
			set => halfDims_ = value*0.5f;
		}

		[Browsable(false)]
		public readonly float Diagonal => halfDims_.Magnitude*2f;

		[Browsable(false)]
		public readonly float SurfaceArea
		{
			get
			{
				//if ((halfDims_.x_ < 0f) || (halfDims_.y_ < 0f) || (halfDims_.z_ < 0f))
				//	return 0f;
				Vector3 dim = halfDims_*2f;
				return 2f*(dim.x_*dim.y_ + dim.y_*dim.z_ + dim.z_*dim.x_);
			}
		}

		[Browsable(false)]
		public readonly float Volume
		{
			get
			{
				//if ((halfDims_.x_ < 0f) || (halfDims_.y_ < 0f) || (halfDims_.z_ < 0f))
				//	return 0f;
				Vector3 dim = halfDims_*2f;
				return dim.x_*dim.y_*dim.z_;
			}
		}

		public readonly AxisAlignedBox GetCircumscribedBox()
		{
			Vector3 halfDims = Vector3.Abs(halfDims_.x_*basis_.Row0) + Vector3.Abs(halfDims_.y_*basis_.Row1) +
				Vector3.Abs(halfDims_.z_*basis_.Row2);
			return new AxisAlignedBox(center_ - halfDims, center_ + halfDims);
		}

		public readonly Sphere GetCircumscribedSphere()
		{
			//if ((halfDims_.x_ < 0f) || (halfDims_.y_ < 0f) || (halfDims_.z_ < 0f))
			//	return new Sphere();
			return new Sphere(center_, halfDims_.Magnitude);
		}

		public readonly IEnumerable<Vector3> GetVertices()
		{
			AffineTransform m = new AffineTransform(basis_, center_);
			yield return Vector3.Transform(new Vector3(-halfDims_.x_, -halfDims_.y_, -halfDims_.z_), m);
	        yield return Vector3.Transform(new Vector3(halfDims_.x_, -halfDims_.y_, -halfDims_.z_), m);
	        yield return Vector3.Transform(new Vector3(-halfDims_.x_, halfDims_.y_, -halfDims_.z_), m);
	        yield return Vector3.Transform(new Vector3(halfDims_.x_, halfDims_.y_, -halfDims_.z_), m);
	        yield return Vector3.Transform(new Vector3(-halfDims_.x_, -halfDims_.y_, halfDims_.z_), m);
	        yield return Vector3.Transform(new Vector3(halfDims_.x_, -halfDims_.y_, halfDims_.z_), m);
	        yield return Vector3.Transform(new Vector3(-halfDims_.x_, halfDims_.y_, halfDims_.z_), m);
	        yield return Vector3.Transform(new Vector3(halfDims_.x_, halfDims_.y_, halfDims_.z_), m);
		}

		public readonly IEnumerable<HalfSpace> GetHalfSpaces()
		{
			yield return new HalfSpace(-basis_.Row0, -halfDims_.x_*basis_.Row0 + center_);
			yield return new HalfSpace(basis_.Row0, halfDims_.x_*basis_.Row0 + center_);
			yield return new HalfSpace(-basis_.Row1, -halfDims_.y_*basis_.Row1 + center_);
			yield return new HalfSpace(basis_.Row1, halfDims_.y_*basis_.Row1 + center_);
			yield return new HalfSpace(-basis_.Row2, -halfDims_.z_*basis_.Row2 + center_);
			yield return new HalfSpace(basis_.Row2, halfDims_.z_*basis_.Row2 + center_);
		}

		public void Orthonormalize()
		{
#if SIMD
			halfDims_ *= new Vector3(basis_.Row0.Magnitude, basis_.Row1.Magnitude, basis_.Row2.Magnitude);
#else
			halfDims_.X *= basis_.Row0.Magnitude;
			halfDims_.Y *= basis_.Row1.Magnitude;
			halfDims_.Z *= basis_.Row2.Magnitude;
#endif
			basis_.Orthonormalize();
		}

		public static OrientedBox Orthonormalize(OrientedBox box)
		{
			box.Orthonormalize();
			return box;
		}

		public void Translate(Vector3 offset)
		{
			center_ += offset;
		}

		public static OrientedBox Translate(OrientedBox box, Vector3 offset)
		{
			box.Translate(offset);
			return box;
		}

		public void Transform(in Matrix3 matrix, bool orthogonal = false)
		{
			basis_.Concat(matrix);
			center_.Transform(matrix);
			if (!orthogonal)
				Orthonormalize();
		}

		public void Transform(in AffineTransform at, bool orthogonal = false)
		{
			basis_.Concat(at.r_);
			center_.Transform(at);
			if (!orthogonal)
				Orthonormalize();
		}

		public static OrientedBox Transform(OrientedBox box, in Matrix3 matrix, bool orthogonal = false)
		{
			box.Transform(matrix, orthogonal);
			return box;
		}

		public static OrientedBox Transform(OrientedBox box, in AffineTransform at, bool orthogonal = false)
		{
			box.Transform(at, orthogonal);
			return box;
		}

		public readonly Vector3 GetClosestPoint(Vector3 point)
		{
			//Matrix3 basisTranspose = Matrix3.Transpose(basis_);
			//point = (point - center_)*basisTranspose;
			point = basis_*(point - center_);
			return Vector3.Clamp(point, -halfDims_, halfDims_)*basis_ + center_;
		}

		public readonly float GetDistanceTo(Vector3 point)
		{
			return Vector3.Distance(point, GetClosestPoint(point));
		}

		public readonly bool Contains(Vector3 point)
		{
			//Matrix3 basisTranspose = Matrix3.Transpose(basis_);
			//point = (point - center_)*basisTranspose;
			point = basis_*(point - center_);
			return (-halfDims_).AllLessThanEqual(point) && halfDims_.AllGreaterThanEqual(point);
		}

		public readonly bool Intersects(in HalfSpace halfSpace)
		{
			return Intersections.TestOrientedBoxHalfSpace(center_, basis_, halfDims_, halfSpace.Normal, halfSpace.D);
		}

		public readonly bool Intersects(in Plane plane)
		{
			return Intersections.TestOrientedBoxPlane(center_, basis_, halfDims_, plane.Normal, plane.D);
		}

		public readonly bool Intersects(in Triangle3 triangle)
		{
			return Intersections.TestOrientedBoxTriangle(center_, basis_, halfDims_, triangle.vertex0_, triangle.vertex1_, 
				triangle.vertex2_);
		}

		public readonly bool Intersects(in AxisAlignedBox box)
		{
			return Intersections.TestOrientedBoxAxisAlignedBox(center_, basis_, halfDims_, box.Center, box.HalfDimensions);
		}

		public readonly bool Intersects(in OrientedBox box)
		{
			return Intersections.TestOrientedBoxOrientedBox(center_, basis_, halfDims_, box.center_, box.basis_, box.halfDims_);
		}

		public readonly bool Intersects(in Sphere sphere)
		{
			return Intersections.TestOrientedBoxSphere(center_, basis_, halfDims_, sphere.center_, sphere.radius_);
		}

		public readonly bool Intersects(in SymmetricFrustum frustum)
		{
			return frustum.Intersects(this);
		}

		internal Vector3 center_;
		internal Matrix3 basis_;
		internal Vector3 halfDims_;
	}
}
