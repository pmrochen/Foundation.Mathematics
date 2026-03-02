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
        public SymmetricFrustum(Vector3 origin, Matrix3 basis, Vector2 halfDims, float depthMin, float depthMax)
        {
			origin_ = origin;
			basis_ = basis;
			halfDims_ = halfDims;
			depthMin_ = depthMin;
			depthMax_ = depthMax;
        }

		public static bool operator ==(SymmetricFrustum lhs, SymmetricFrustum rhs)
		{
			return (lhs.origin_ == rhs.origin_) && (lhs.basis_ == rhs.basis_) && (lhs.halfDims_ == rhs.halfDims_) &&
				(lhs.depthMin_ == rhs.depthMin_) && (lhs.depthMax_ == rhs.depthMax_);
		}

		public static bool operator !=(SymmetricFrustum lhs, SymmetricFrustum rhs)
		{
			return (lhs.origin_ != rhs.origin_) || (lhs.basis_ != rhs.basis_) || (lhs.halfDims_ != rhs.halfDims_) ||
				(lhs.depthMin_ != rhs.depthMin_) || (lhs.depthMax_ != rhs.depthMax_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is SymmetricFrustum rhs)
			{
				return (origin_ == rhs.origin_) && (basis_ == rhs.basis_) && (halfDims_ == rhs.halfDims_) &&
					(depthMin_ == rhs.depthMin_) && (depthMax_ == rhs.depthMax_);
			}

			return false;
		}

		public readonly bool Equals(SymmetricFrustum other)
		{
			return (origin_ == other.origin_) && (basis_ == other.basis_) && (halfDims_ == other.halfDims_) &&
				(depthMin_ == other.depthMin_) && (depthMax_ == other.depthMax_);
		}

		public readonly override int GetHashCode()
		{
			int hash = origin_.GetHashCode();
			hash = ((hash << 5) + hash) ^ basis_.GetHashCode();
			hash = ((hash << 5) + hash) ^ halfDims_.GetHashCode();
			hash = ((hash << 5) + hash) ^ depthMin_.GetHashCode();
			return ((hash << 5) + hash) ^ depthMax_.GetHashCode();
		}

		public readonly bool ApproxEquals(in SymmetricFrustum other, float tolerance)
		{
			return origin_.ApproxEquals(other.origin_, tolerance) &&
				basis_.ApproxEquals(other.basis_, tolerance) &&
				halfDims_.ApproxEquals(other.halfDims_, tolerance) &&
				(Math.Abs(other.depthMin_ - depthMin_) < tolerance) &&
				(Math.Abs(other.depthMax_ - depthMax_) < tolerance);
		}

		public readonly bool ApproxEquals(in SymmetricFrustum other)
		{
			return origin_.ApproxEquals(other.origin_) &&
				basis_.ApproxEquals(other.basis_) &&
				halfDims_.ApproxEquals(other.halfDims_) &&
				(Math.Abs(other.depthMin_ - depthMin_) < 1e-6f) &&
				(Math.Abs(other.depthMax_ - depthMax_) < 1e-6f);
		}

		public readonly override string ToString()
		{
			return String.Concat(origin_.ToString(), " ", basis_.ToString(), " ", halfDims_.ToString(), " ",
				depthMin_.ToString(), " ", depthMax_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(origin_.ToString(provider), " ", basis_.ToString(provider), " ", halfDims_.ToString(provider), " ",
				depthMin_.ToString(provider), " ", depthMax_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(origin_.ToString(format), " ", basis_.ToString(format), " ", halfDims_.ToString(format), " ",
				depthMin_.ToString(format), " ", depthMax_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(origin_.ToString(format, provider), " ", basis_.ToString(format, provider), " ", halfDims_.ToString(format, provider), " ",
				depthMin_.ToString(format, provider), " ", depthMax_.ToString(format, provider));
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
							   Single.Parse(m[14]), Single.Parse(m[15]));
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
							   Single.Parse(m[14], provider), Single.Parse(m[15], provider));
		}

		[Browsable(false)]
		public readonly bool IsFinite
		{
			get
			{
				return origin_.IsFinite && basis_.IsFinite && halfDims_.IsFinite && Functions.IsFinite(depthMin_) &&
					Functions.IsFinite(depthMax_);
			}
		}

		[Browsable(false)]
		public readonly bool IsEmpty => (halfDims_.x_ <= 0f) || (halfDims_.y_ <= 0f) || (depthMin_ >= depthMax_);

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
			readonly get => halfDims_*(depthMax_/depthMin_);
			set => halfDims_ = value*(depthMin_/depthMax_);
		}

		[Browsable(false)]
		public Vector2 BaseDimensions
		{
			readonly get => halfDims_*(2f*depthMax_/depthMin_);
			set => halfDims_ = value*(0.5f*depthMin_/depthMax_);
		}

		public float MinDepth
		{
			readonly get => depthMin_;
			set => depthMin_ = value;
		}

		public float MaxDepth
		{
			readonly get => depthMax_;
			set => depthMax_ = value;
		}

		public readonly IEnumerable<Vector3> GetVertices()
		{
			AffineTransform m = new AffineTransform(basis_, origin_);
			float depthRatio = depthMax_/depthMin_;
			yield return Vector3.Transform(new Vector3(-halfDims_.x_, -halfDims_.y_, depthMin_), m);
			yield return Vector3.Transform(new Vector3(halfDims_.x_, -halfDims_.y_, depthMin_), m);
			yield return Vector3.Transform(new Vector3(-halfDims_.x_, halfDims_.y_, depthMin_), m);
			yield return Vector3.Transform(new Vector3(halfDims_.x_, halfDims_.y_, depthMin_), m);
			yield return Vector3.Transform(new Vector3(-halfDims_.x_*depthRatio, -halfDims_.y_*depthRatio, depthMax_), m);
			yield return Vector3.Transform(new Vector3(halfDims_.x_*depthRatio, -halfDims_.y_*depthRatio, depthMax_), m);
			yield return Vector3.Transform(new Vector3(-halfDims_.x_*depthRatio, halfDims_.y_*depthRatio, depthMax_), m);
			yield return Vector3.Transform(new Vector3(halfDims_.x_*depthRatio, halfDims_.y_*depthRatio, depthMax_), m);
		}

		public readonly IEnumerable<HalfSpace> GetHalfSpaces()
		{
			AffineTransform m = new AffineTransform(basis_, origin_);
			Vector3 bottomLeft = Vector3.Transform(new Vector3(-halfDims_.x_, -halfDims_.y_, depthMin_), m);
			Vector3 bottomRight = Vector3.Transform(new Vector3(halfDims_.x_, -halfDims_.y_, depthMin_), m);
			Vector3 topLeft = Vector3.Transform(new Vector3(-halfDims_.x_, halfDims_.y_, depthMin_), m);
			Vector3 topRight = Vector3.Transform(new Vector3(halfDims_.x_, halfDims_.y_, depthMin_), m);

			bool flip = (basis_.Determinant < 0f);
			yield return flip ? HalfSpace.FromTriangle(origin_, topLeft, bottomLeft) : HalfSpace.FromTriangle(origin_, bottomLeft, topLeft);
			yield return flip ? HalfSpace.FromTriangle(origin_, bottomRight, topRight) : HalfSpace.FromTriangle(origin_, topRight, bottomRight);
			yield return flip ? HalfSpace.FromTriangle(origin_, bottomLeft, bottomRight) : HalfSpace.FromTriangle(origin_, bottomRight, bottomLeft);
			yield return flip ? HalfSpace.FromTriangle(origin_, topRight, topLeft) : HalfSpace.FromTriangle(origin_, topLeft, topRight);

			yield return new HalfSpace(-basis_[2], depthMin_*basis_[2] + origin_);
			if (depthMax_ < Single.MaxValue)
				yield return new HalfSpace(basis_[2], depthMax_*basis_[2] + origin_);
		}

		public readonly OrientedBox GetCircumscribedBox()
		{
			Vector2 baseHalfDims = halfDims_*(depthMax_/depthMin_);
			return new OrientedBox(origin_ + ((depthMin_ + depthMax_)*0.5f)*basis_[2], basis_, 
				new Vector3(baseHalfDims.x_, baseHalfDims.y_, (depthMax_ - depthMin_)*0.5f));
		}

		public readonly Sphere GetCircumscribedSphere()
		{
			Vector2 baseHalfDims = halfDims_*(depthMax_/depthMin_);
			float coneRadiusSq = baseHalfDims.MagnitudeSquared;
			float depthMaxSq = depthMax_*depthMax_;
			if (depthMaxSq > coneRadiusSq)
			{
				float slantSquared = coneRadiusSq + depthMaxSq;
				float sphereRadius = slantSquared/(2f*depthMax_);
				return new Sphere(origin_ + sphereRadius*basis_[2], sphereRadius);
			}
			else
			{
				float coneRadius = MathF.Sqrt(coneRadiusSq);
				return new Sphere(origin_ + depthMax_*basis_[2], coneRadius);
			}
		}

		public readonly Cone GetCircumscribedCone()
		{
			Vector2 baseHalfDims = halfDims_*(depthMax_/depthMin_);
			float coneRadius = baseHalfDims.Magnitude;
			return new Cone(origin_, basis_[2], depthMax_, coneRadius);
		}

		//public readonly OpenCone GetCircumscribedOpenCone()
		//{
		//	Vector2 baseHalfDims = halfDims_*(depthMax_/depthMin_);
		//	float coneRadius = baseHalfDims.Magnitude;
		//	return new OpenCone(origin_, basis_[2], depthMax_, (float)Math.Atan(coneRadius/depthMax_));
		//}

        public void Orthonormalize()
        {
			//if (System.Numerics.Vector.IsHardwareAccelerated)
			//{
				halfDims_ *= new Vector2(basis_[0].Magnitude, basis_[1].Magnitude);
			//}
			//else
			//{
			//	halfDims_.X *= basis_[0].Magnitude;
			//	halfDims_.Y *= basis_[1].Magnitude;
			//}

			float zLength = basis_[2].Magnitude;
			depthMin_ *= zLength;
			depthMax_ *= zLength;
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

		//public readonly Vector3 GetClosestPoint(Vector3 point) // #TODO
		//{
		//}

		//public readonly float GetDistanceTo(Vector3 point) // #TODO
		//{
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
			foreach (HalfSpace hs in GetHalfSpaces())
			{
				if (!sphere.Intersects(hs))
					return false;
			}

			return true;
		}

		internal Vector3 origin_;
		internal Matrix3 basis_;
		internal Vector2 halfDims_;
		internal float depthMin_;
		internal float depthMax_;
	}
}
