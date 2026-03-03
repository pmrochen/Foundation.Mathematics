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

		public static OrientedBox FromAxisAlignedBox(in AxisAlignedBox b, in AffineTransform at)
		{
			return OrientedBox.Orthonormalize(new OrientedBox(Vector3.Transform(b.Center, at), at.r_, b.HalfDimensions));
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
		public readonly float Area
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

		//public readonly AxisAlignedBox GetCircumscribedBox()
		//{
		//	Vector3 h = Vector3.Abs(halfDims_.x_*basis_.Row0) + Vector3.Abs(halfDims_.y_*basis_.Row1) + 
		//		Vector3.Abs(halfDims_.z_*basis_.Row2);
		//	return new AxisAlignedBox(center_ - h, center_ + h);
		//}

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

		public static OrientedBox Orthonormalize(OrientedBox b)
		{
			b.Orthonormalize();
			return b;
		}

		public void Translate(Vector3 offset)
		{
			center_ += offset;
		}

		public static OrientedBox Translate(OrientedBox b, Vector3 offset)
		{
			b.Translate(offset);
			return b;
		}

		public void Transform(in AffineTransform at)
		{
			basis_.Concat(at.r_);
			center_.Transform(at);
			Orthonormalize();
		}

		public static OrientedBox Transform(OrientedBox b, in AffineTransform at)
		{
			b.Transform(at);
			return b;
		}

		public readonly Vector3 GetClosestPoint(Vector3 point)
		{
			Matrix3 basisTranspose = Matrix3.Transpose(basis_);
			point = (point - center_)*basisTranspose;
			return Vector3.Clamp(point, -halfDims_, halfDims_)*basis_ + center_;
		}

		public readonly float GetDistanceTo(Vector3 point)
		{
			return Vector3.Distance(point, GetClosestPoint(point));
		}

		public readonly bool Contains(Vector3 point)
		{
			Matrix3 basisTranspose = Matrix3.Transpose(basis_);
			point = (point - center_)*basisTranspose;
			return (-halfDims_).AllLessThanEqual(point) && halfDims_.AllGreaterThanEqual(point);
		}

		public readonly bool Intersects(in HalfSpace halfSpace)
		{
			Vector3 normal = halfSpace.Normal;
#if SIMD
			Matrix3 basisTranspose = Matrix3.Transpose(basis_);
			float r = Vector3.Sum(Vector3.Abs(halfDims_*(normal*basisTranspose)));
#else
			float r = Math.Abs(halfDims_.x_*Vector3.Dot(normal, basis_.Row0)) +
				Math.Abs(halfDims_.y_*Vector3.Dot(normal, basis_.Row1)) +
				Math.Abs(halfDims_.z_*Vector3.Dot(normal, basis_.Row2));
#endif
			return ((Vector3.Dot(normal, center_) + halfSpace.D) <= r);
		}

		public readonly bool Intersects(in Plane plane)
		{
			Vector3 normal = plane.Normal;
#if SIMD
			Matrix3 basisTranspose = Matrix3.Transpose(basis_);
			float r = Vector3.Sum(Vector3.Abs(halfDims_*(normal*basisTranspose)));
#else
			float r = Math.Abs(halfDims_.x_*Vector3.Dot(normal, basis_.Row0)) +
				Math.Abs(halfDims_.y_*Vector3.Dot(normal, basis_.Row1)) +
				Math.Abs(halfDims_.z_*Vector3.Dot(normal, basis_.Row2));
#endif
			return (Math.Abs(Vector3.Dot(normal, center_) + plane.D) <= r);
		}

		public readonly bool Intersects(in Triangle3 triangle)
		{
			Matrix3 basisTranspose = Matrix3.Transpose(basis_);
			return AxisAlignedBox.IntersectAxisAlignedBoxTriangle(halfDims_, (triangle.Vertex0 - center_)*basisTranspose, 
				(triangle.Vertex1 - center_)*basisTranspose, (triangle.Vertex2 - center_)*basisTranspose);
		}

		public readonly bool Intersects(in AxisAlignedBox box)
		{
			return IntersectOrientedBoxOrientedBox(center_, basis_, halfDims_, box.Center, Matrix3.Identity, box.HalfDimensions);
		}

		public readonly bool Intersects(in OrientedBox box)
		{
			return IntersectOrientedBoxOrientedBox(center_, basis_, halfDims_, box.center_, box.basis_, box.halfDims_);
		}

		public readonly bool Intersects(in Sphere sphere)
		{
			return sphere.Intersects(this);
		}

		public readonly bool Intersects(in SymmetricFrustum frustum)
		{
			return frustum.Intersects(this);
		}

		private static bool IntersectOrientedBoxOrientedBox(Vector3 centerA, in Matrix3 akA, Vector3 afEA, Vector3 centerB, 
			in Matrix3 akB, Vector3 afEB)
		{
			// https://www.geometrictools.com/

			const float cutoff = 1f - 1e-6f;
			bool existsParallelPair = false;
			Vector3 kD = centerB - centerA;
			Vector3 aafC0 = Vector3.Zero;
			Vector3 aafC1 = Vector3.Zero;
			Vector3 aafC2 = Vector3.Zero;
			Vector3 aafAbsC0 = Vector3.Zero;
			Vector3 aafAbsC1 = Vector3.Zero;
			Vector3 aafAbsC2 = Vector3.Zero;
			Vector3 afAD = Vector3.Zero;

			for (int i = 0; i < 3; i++)
			{
				aafC0[i] = Vector3.Dot(akA[0], akB[i]);
				aafAbsC0[i] = Math.Abs(aafC0[i]);
				if (aafAbsC0[i] > cutoff)
					existsParallelPair = true;
			}

			afAD.X = Vector3.Dot(akA[0], kD);
			float fR = Math.Abs(afAD.x_);
			float fR1 = afEB.x_*aafAbsC0.x_ + afEB.y_*aafAbsC0.y_ + afEB.z_*aafAbsC0.z_;
			float fR01 = afEA.x_ + fR1;
			if (fR > fR01)
				return false;

			for (int i = 0; i < 3; i++)
			{
				aafC1[i] = Vector3.Dot(akA[1], akB[i]);
				aafAbsC1[i] = Math.Abs(aafC1[i]);
				if (aafAbsC1[i] > cutoff)
					existsParallelPair = true;
			}

			afAD.Y = Vector3.Dot(akA[1], kD);
			fR = Math.Abs(afAD.y_);
			fR1 = afEB.x_*aafAbsC1.x_ + afEB.y_*aafAbsC1.y_ + afEB.z_*aafAbsC1.z_;
			fR01 = afEA.y_ + fR1;
			if (fR > fR01)
				return false;

			for (int i = 0; i < 3; i++)
			{
				aafC2[i] = Vector3.Dot(akA[2], akB[i]);
				aafAbsC2[i] = Math.Abs(aafC2[i]);
				if (aafAbsC2[i] > cutoff)
					existsParallelPair = true;
			}

			afAD.Z = Vector3.Dot(akA[2], kD);
			fR = Math.Abs(afAD.z_);
			fR1 = afEB.x_*aafAbsC2.x_ + afEB.y_*aafAbsC2.y_ + afEB.z_*aafAbsC2.z_;
			fR01 = afEA.z_ + fR1;
			if (fR > fR01)
				return false;

			fR = Math.Abs(Vector3.Dot(akB[0], kD));
			float fR0 = afEA.x_*aafAbsC0.x_ + afEA.y_*aafAbsC1.x_ + afEA.z_*aafAbsC2.x_;
			fR01 = fR0 + afEB.x_;
			if (fR > fR01)
				return false;

			fR = Math.Abs(Vector3.Dot(akB[1], kD));
			fR0 = afEA.x_*aafAbsC0.y_ + afEA.y_*aafAbsC1.y_ + afEA.z_*aafAbsC2.y_;
			fR01 = fR0 + afEB.y_;
			if (fR > fR01)
				return false;

			fR = Math.Abs(Vector3.Dot(akB[2], kD));
			fR0 = afEA.x_*aafAbsC0.z_ + afEA.y_*aafAbsC1.z_ + afEA.z_*aafAbsC2.z_;
			fR01 = fR0 + afEB.z_;
			if (fR > fR01)
				return false;

			if (existsParallelPair)
				return true;

			fR = Math.Abs(afAD.z_*aafC1.x_ - afAD.y_*aafC2.x_);
			fR0 = afEA.y_*aafAbsC2.x_ + afEA.z_*aafAbsC1.x_;
			fR1 = afEB.y_*aafAbsC0.z_ + afEB.z_*aafAbsC0.y_;
			fR01 = fR0 + fR1;
			if (fR > fR01)
				return false;

			fR = Math.Abs(afAD.z_*aafC1.y_ - afAD.y_*aafC2.y_);
			fR0 = afEA.y_*aafAbsC2.y_ + afEA.z_*aafAbsC1.y_;
			fR1 = afEB.x_*aafAbsC0.z_ + afEB.z_*aafAbsC0.x_;
			fR01 = fR0 + fR1;
			if (fR > fR01)
				return false;

			fR = Math.Abs(afAD.z_*aafC1.z_ - afAD.y_*aafC2.z_);
			fR0 = afEA.y_*aafAbsC2.z_ + afEA.z_*aafAbsC1.z_;
			fR1 = afEB.x_*aafAbsC0.y_ + afEB.y_*aafAbsC0.x_;
			fR01 = fR0 + fR1;
			if (fR > fR01)
				return false;

			fR = Math.Abs(afAD.x_*aafC2.x_ - afAD.z_*aafC0.x_);
			fR0 = afEA.x_*aafAbsC2.x_ + afEA.z_*aafAbsC0.x_;
			fR1 = afEB.y_*aafAbsC1.z_ + afEB.z_*aafAbsC1.y_;
			fR01 = fR0 + fR1;
			if (fR > fR01)
				return false;

			fR = Math.Abs(afAD.x_*aafC2.y_ - afAD.z_*aafC0.y_);
			fR0 = afEA.x_*aafAbsC2.y_ + afEA.z_*aafAbsC0.y_;
			fR1 = afEB.x_*aafAbsC1.z_ + afEB.z_*aafAbsC1.x_;
			fR01 = fR0 + fR1;
			if (fR > fR01)
				return false;

			fR = Math.Abs(afAD.x_*aafC2.z_ - afAD.z_*aafC0.z_);
			fR0 = afEA.x_*aafAbsC2.z_ + afEA.z_*aafAbsC0.z_;
			fR1 = afEB.x_*aafAbsC1.y_ + afEB.y_*aafAbsC1.x_;
			fR01 = fR0 + fR1;
			if (fR > fR01)
				return false;

			fR = Math.Abs(afAD.y_*aafC0.x_ - afAD.x_*aafC1.x_);
			fR0 = afEA.x_*aafAbsC1.x_ + afEA.y_*aafAbsC0.x_;
			fR1 = afEB.y_*aafAbsC2.z_ + afEB.z_*aafAbsC2.y_;
			fR01 = fR0 + fR1;
			if (fR > fR01)
				return false;

			fR = Math.Abs(afAD.y_*aafC0.y_ - afAD.x_*aafC1.y_);
			fR0 = afEA.x_*aafAbsC1.y_ + afEA.y_*aafAbsC0.y_;
			fR1 = afEB.x_*aafAbsC2.z_ + afEB.z_*aafAbsC2.x_;
			fR01 = fR0 + fR1;
			if (fR > fR01)
				return false;

			fR = Math.Abs(afAD.y_*aafC0.z_ - afAD.x_*aafC1.z_);
			fR0 = afEA.x_*aafAbsC1.z_ + afEA.y_*aafAbsC0.z_;
			fR1 = afEB.x_*aafAbsC2.y_ + afEB.y_*aafAbsC2.x_;
			fR01 = fR0 + fR1;
			if (fR > fR01)
				return false;

			return true;
		}

		internal Vector3 center_;
		internal Matrix3 basis_;
		internal Vector3 halfDims_;
	}
}
