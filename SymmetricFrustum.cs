/*
 *  Name: SymmetricFrustum
 *  Author: Pawel Mrochen
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct SymmetricFrustum : IFormattable, IEquatable<SymmetricFrustum>
	{
		public SymmetricFrustum(Vector3 origin, Matrix3 basis, Vector2 halfDims, Interval depthRange)
		{
			origin_ = origin;
			basis_ = basis;
			halfDims_ = halfDims;
			depthRange_ = depthRange;
		}

		public SymmetricFrustum(Vector3 origin, Matrix3 basis, Vector2 halfDims, float depthMin, float depthMax)
        {
			origin_ = origin;
			basis_ = basis;
			halfDims_ = halfDims;
			depthRange_ = new Interval(depthMin, depthMax);
        }

		public static bool operator ==(SymmetricFrustum lhs, SymmetricFrustum rhs)
		{
			return (lhs.origin_ == rhs.origin_) && (lhs.basis_ == rhs.basis_) && (lhs.halfDims_ == rhs.halfDims_) &&
				(lhs.depthRange_ == rhs.depthRange_);
		}

		public static bool operator !=(SymmetricFrustum lhs, SymmetricFrustum rhs)
		{
			return (lhs.origin_ != rhs.origin_) || (lhs.basis_ != rhs.basis_) || (lhs.halfDims_ != rhs.halfDims_) ||
				(lhs.depthRange_ != rhs.depthRange_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is SymmetricFrustum rhs)
			{
				return (origin_ == rhs.origin_) && (basis_ == rhs.basis_) && (halfDims_ == rhs.halfDims_) &&
					(depthRange_ == rhs.depthRange_);
			}

			return false;
		}

		public readonly bool Equals(SymmetricFrustum other)
		{
			return (origin_ == other.origin_) && (basis_ == other.basis_) && (halfDims_ == other.halfDims_) &&
				(depthRange_ == other.depthRange_);
		}

		public readonly override int GetHashCode()
		{
			int hash = origin_.GetHashCode();
			hash = ((hash << 5) + hash) ^ basis_.GetHashCode();
			hash = ((hash << 5) + hash) ^ halfDims_.GetHashCode();
			return ((hash << 5) + hash) ^ depthRange_.GetHashCode();
		}

		public readonly bool ApproxEquals(in SymmetricFrustum other, float tolerance)
		{
			return origin_.ApproxEquals(other.origin_, tolerance) &&
				basis_.ApproxEquals(other.basis_, tolerance) &&
				halfDims_.ApproxEquals(other.halfDims_, tolerance) &&
				depthRange_.ApproxEquals(other.depthRange_, tolerance);
		}

		public readonly bool ApproxEquals(in SymmetricFrustum other)
		{
			return origin_.ApproxEquals(other.origin_) &&
				basis_.ApproxEquals(other.basis_) &&
				halfDims_.ApproxEquals(other.halfDims_) &&
				depthRange_.ApproxEquals(other.depthRange_);
		}

		public readonly override string ToString()
		{
			return String.Concat(origin_.ToString(), " ", basis_.ToString(), " ", halfDims_.ToString(), " ",
				depthRange_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(origin_.ToString(provider), " ", basis_.ToString(provider), " ", halfDims_.ToString(provider), " ",
				depthRange_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(origin_.ToString(format), " ", basis_.ToString(format), " ", halfDims_.ToString(format), " ",
				depthRange_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(origin_.ToString(format, provider), " ", basis_.ToString(format, provider), " ", halfDims_.ToString(format, provider), " ",
				depthRange_.ToString(format, provider));
		}

		public static SymmetricFrustum Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 16)
				throw new FormatException();

			return new SymmetricFrustum(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
							   new Matrix3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5]),
										   Single.Parse(m[6]), Single.Parse(m[7]), Single.Parse(m[8]),
										   Single.Parse(m[9]), Single.Parse(m[10]), Single.Parse(m[11])),
							   new Vector2(Single.Parse(m[12]), Single.Parse(m[13])),
							   new Interval(Single.Parse(m[14]), Single.Parse(m[15])));
		}

		public static SymmetricFrustum Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 16)
				throw new FormatException();

			return new SymmetricFrustum(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
							   new Matrix3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider),
										   Single.Parse(m[6], provider), Single.Parse(m[7], provider), Single.Parse(m[8], provider),
										   Single.Parse(m[9], provider), Single.Parse(m[10], provider), Single.Parse(m[11], provider)),
							   new Vector2(Single.Parse(m[12], provider), Single.Parse(m[13], provider)),
							   new Interval(Single.Parse(m[14], provider), Single.Parse(m[15], provider)));
		}

		[Browsable(false)]
		public readonly bool IsFinite
		{
			get
			{
				return origin_.IsFinite && basis_.IsFinite && halfDims_.IsFinite && depthRange_.IsFinite;
			}
		}

		//[Browsable(false)]
		//public readonly bool IsEmpty => halfDims_.AnyLessThanEqual(Vector2.Zero) || (depthRange_.minimum_ >= depthRange_.maximum_);

		public Vector3 Origin
		{
			readonly get => origin_;
			set => origin_ = value;
		}

		public Matrix3 Basis
		{
			readonly get => basis_;
			set => basis_ = value;
		}

		public Vector2 HalfDimensions
		{
			readonly get => halfDims_;
			set => halfDims_ = value;
		}

		[Browsable(false)]
		public Vector2 Dimensions
		{
			readonly get => halfDims_*2f;
			set => halfDims_ = value*0.5f;
		}

		[Browsable(false)]
		public Vector2 BaseHalfDimensions
		{
			readonly get => halfDims_*(depthRange_.maximum_/depthRange_.minimum_);
			set => halfDims_ = value*(depthRange_.minimum_/depthRange_.maximum_);
		}

		[Browsable(false)]
		public Vector2 BaseDimensions
		{
			readonly get => halfDims_*(2f*depthRange_.maximum_/depthRange_.minimum_);
			set => halfDims_ = value*(0.5f*depthRange_.minimum_/depthRange_.maximum_);
		}

		public Interval DepthRange
		{
			readonly get => depthRange_;
			set => depthRange_ = value;
		}

		[Browsable(false)]
		public float MinDepth
		{
			readonly get => depthRange_.minimum_;
			set => depthRange_.minimum_ = value;
		}

		[Browsable(false)]
		public float MaxDepth
		{
			readonly get => depthRange_.maximum_;
			set => depthRange_.maximum_ = value;
		}

		[Browsable(false)]
		public readonly float DepthRatio => depthRange_.maximum_/depthRange_.minimum_;

		public readonly IEnumerable<Vector3> GetVertices()
		{
			AffineTransform m = new AffineTransform(basis_, origin_);
			float depthRatio = depthRange_.maximum_/depthRange_.minimum_;
			yield return Vector3.Transform(new Vector3(-halfDims_, depthRange_.minimum_), m);
			yield return Vector3.Transform(new Vector3(halfDims_.x_, -halfDims_.y_, depthRange_.minimum_), m);
			yield return Vector3.Transform(new Vector3(-halfDims_.x_, halfDims_.y_, depthRange_.minimum_), m);
			yield return Vector3.Transform(new Vector3(halfDims_, depthRange_.minimum_), m);
			yield return Vector3.Transform(new Vector3(-halfDims_*depthRatio, depthRange_.maximum_), m);
			yield return Vector3.Transform(new Vector3(halfDims_.x_*depthRatio, -halfDims_.y_*depthRatio, depthRange_.maximum_), m);
			yield return Vector3.Transform(new Vector3(-halfDims_.x_*depthRatio, halfDims_.y_*depthRatio, depthRange_.maximum_), m);
			yield return Vector3.Transform(new Vector3(halfDims_*depthRatio, depthRange_.maximum_), m);
		}

		public readonly IEnumerable<HalfSpace> GetHalfSpaces()
		{
			AffineTransform m = new AffineTransform(basis_, origin_);
			Vector3 bottomLeft = Vector3.Transform(new Vector3(-halfDims_, depthRange_.minimum_), m);
			Vector3 bottomRight = Vector3.Transform(new Vector3(halfDims_.x_, -halfDims_.y_, depthRange_.minimum_), m);
			Vector3 topLeft = Vector3.Transform(new Vector3(-halfDims_.x_, halfDims_.y_, depthRange_.minimum_), m);
			Vector3 topRight = Vector3.Transform(new Vector3(halfDims_, depthRange_.minimum_), m);

			bool flip = (basis_.Determinant < 0f);
			yield return flip ? HalfSpace.FromTriangle(origin_, topLeft, bottomLeft) : HalfSpace.FromTriangle(origin_, bottomLeft, topLeft);
			yield return flip ? HalfSpace.FromTriangle(origin_, bottomRight, topRight) : HalfSpace.FromTriangle(origin_, topRight, bottomRight);
			yield return flip ? HalfSpace.FromTriangle(origin_, bottomLeft, bottomRight) : HalfSpace.FromTriangle(origin_, bottomRight, bottomLeft);
			yield return flip ? HalfSpace.FromTriangle(origin_, topRight, topLeft) : HalfSpace.FromTriangle(origin_, topLeft, topRight);

			yield return new HalfSpace(-basis_[2], depthRange_.minimum_*basis_[2] + origin_);
			if (depthRange_.maximum_ < Single.MaxValue)
				yield return new HalfSpace(basis_[2], depthRange_.maximum_*basis_[2] + origin_);
		}

		public readonly OrientedBox GetCircumscribedBox()
		{
			Vector2 baseHalfDims = halfDims_*(depthRange_.maximum_/depthRange_.minimum_);
			return new OrientedBox(origin_ + ((depthRange_.minimum_ + depthRange_.maximum_)*0.5f)*basis_[2], basis_, 
				new Vector3(baseHalfDims.x_, baseHalfDims.y_, (depthRange_.maximum_ - depthRange_.minimum_)*0.5f));
		}

		public readonly Sphere GetCircumscribedSphere()
		{
			Vector2 baseHalfDims = halfDims_*(depthRange_.maximum_/depthRange_.minimum_);
			float coneRadiusSq = baseHalfDims.MagnitudeSquared;
			float depthMaxSq = depthRange_.maximum_*depthRange_.maximum_;
			if (depthMaxSq > coneRadiusSq)
			{
				float slantSquared = coneRadiusSq + depthMaxSq;
				float sphereRadius = slantSquared/(2f*depthRange_.maximum_);
				return new Sphere(origin_ + sphereRadius*basis_[2], sphereRadius);
			}
			else
			{
				return new Sphere(origin_ + depthRange_.maximum_*basis_[2], MathF.Sqrt(coneRadiusSq));
			}
		}

		public readonly Cone GetCircumscribedCone()
		{
			Vector2 baseHalfDims = halfDims_*(depthRange_.maximum_/depthRange_.minimum_);
			return new Cone(origin_, basis_[2], depthRange_.maximum_, baseHalfDims.Magnitude);
		}

        public void Orthonormalize()
        {
#if SIMD
			halfDims_ *= new Vector2(basis_[0].Magnitude, basis_[1].Magnitude);
#else
			halfDims_.X *= basis_[0].Magnitude;
			halfDims_.Y *= basis_[1].Magnitude;
#endif
			float zLength = basis_[2].Magnitude;
			depthRange_.minimum_ *= zLength;
			depthRange_.maximum_ *= zLength;
			basis_.Orthonormalize();
		}

		public static SymmetricFrustum Orthonormalize(SymmetricFrustum frustum)
		{
			frustum.Orthonormalize();
			return frustum;
		}

		public void Translate(Vector3 offset)
		{
			origin_ += offset;
		}

		public static SymmetricFrustum Translate(SymmetricFrustum frustum, Vector3 offset)
		{
			frustum.Translate(offset);
			return frustum;
		}

		public readonly Vector3 GetClosestPoint(Vector3 point)
		{
			Vector3 closestPoint;
			Distances.GetPointSymmetricFrustumSquared(point, origin_, basis_, halfDims_, depthRange_.minimum_, depthRange_.maximum_, 
				out closestPoint);
			return closestPoint;
		}

		public readonly float GetDistanceTo(Vector3 point)
		{
			return Distances.GetPointSymmetricFrustum(point, origin_, basis_, halfDims_, depthRange_.minimum_, depthRange_.maximum_);
		}

		//public readonly float GetDistanceSquaredTo(Vector3 point)
		//{
		//	return Distances.GetPointSymmetricFrustumSquared(point, origin_, basis_, halfDims_, depthRange_.minimum_, depthRange_.maximum_);
		//}

		public readonly bool Contains(Vector3 point)
		{
			foreach (HalfSpace hs in GetHalfSpaces())
			{
				if (!hs.Contains(point))
					return false;
			}

			return true;
		}

		public readonly bool Intersects(in AxisAlignedBox box)
		{
			foreach (HalfSpace hs in GetHalfSpaces())
			{
				if (!box.Intersects(hs))
					return false;
			}

			return true;
		}

		public readonly bool Intersects(in OrientedBox box)
		{
			foreach (HalfSpace hs in GetHalfSpaces())
			{
				if (!box.Intersects(hs))
					return false;
			}

			return true;
		}

		public readonly bool Intersects(in Sphere sphere)
		{
			return (Distances.GetPointSymmetricFrustumSquared(sphere.center_, origin_, basis_, halfDims_,
				depthRange_.minimum_, depthRange_.maximum_) <= sphere.radius_*sphere.radius_);

			//foreach (HalfSpace hs in GetHalfSpaces())
			//{
			//	if (!sphere.Intersects(hs))
			//		return false;
			//}

			//return true;
		}

		internal Vector3 origin_;
		internal Matrix3 basis_;
		internal Vector2 halfDims_;
		internal Interval depthRange_;
	}
}
