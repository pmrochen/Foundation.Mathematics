/*
 *  Name: Ellipsoid
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	/// <summary>
	/// A closed type of quadric surface that is a higher dimensional analogue of an ellipse.
	/// </summary>
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Ellipsoid : IFormattable, IEquatable<Ellipsoid>
	{
		public Ellipsoid(Vector3 center, Matrix3 basis, Vector3 radii)
		{
			center_ = center;
			basis_ = basis;
			radii_ = radii;
		}

		public static bool operator ==(Ellipsoid lhs, Ellipsoid rhs)
		{
			return (lhs.center_ == rhs.center_) && (lhs.basis_ == rhs.basis_) && (lhs.radii_ == rhs.radii_);
		}

		public static bool operator !=(Ellipsoid lhs, Ellipsoid rhs)
		{
			return (lhs.center_ != rhs.center_) || (lhs.basis_ != rhs.basis_) || (lhs.radii_ != rhs.radii_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Ellipsoid rhs)
				return (center_ == rhs.center_) && (basis_ == rhs.basis_) && (radii_ == rhs.radii_);

			return false;
		}

		public readonly bool Equals(Ellipsoid other)
		{
			return (center_ == other.center_) && (basis_ == other.basis_) && (radii_ == other.radii_);
		}

		public readonly override int GetHashCode()
		{
			int hash = center_.GetHashCode();
			hash = ((hash << 5) + hash) ^ basis_.GetHashCode();
			return ((hash << 5) + hash) ^ radii_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Ellipsoid other, float tolerance)
		{
			return center_.ApproxEquals(other.center_, tolerance) &&
				basis_.ApproxEquals(other.basis_, tolerance) &&
				radii_.ApproxEquals(other.radii_, tolerance);
		}

		public readonly bool ApproxEquals(in Ellipsoid other)
		{
			return center_.ApproxEquals(other.center_) &&
				basis_.ApproxEquals(other.basis_) &&
				radii_.ApproxEquals(other.radii_);
		}

		public readonly override string ToString()
		{
			return String.Concat(center_.ToString(), " ", basis_.ToString(), " ", radii_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(center_.ToString(provider), " ", basis_.ToString(provider), " ", radii_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(center_.ToString(format), " ", basis_.ToString(format), " ", radii_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(center_.ToString(format, provider), " ", basis_.ToString(format, provider), " ", radii_.ToString(format, provider));
		}

		public static Ellipsoid Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 15)
				throw new FormatException();

			return new Ellipsoid(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
								 new Matrix3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5]),
											 Single.Parse(m[6]), Single.Parse(m[7]), Single.Parse(m[8]),
											 Single.Parse(m[9]), Single.Parse(m[10]), Single.Parse(m[11])),
								 new Vector3(Single.Parse(m[12]), Single.Parse(m[13]), Single.Parse(m[14])));
		}

		public static Ellipsoid Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 15)
				throw new FormatException();

			return new Ellipsoid(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
								 new Matrix3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider),
											 Single.Parse(m[6], provider), Single.Parse(m[7], provider), Single.Parse(m[8], provider),
											 Single.Parse(m[9], provider), Single.Parse(m[10], provider), Single.Parse(m[11], provider)),
								 new Vector3(Single.Parse(m[12], provider), Single.Parse(m[13], provider), Single.Parse(m[14], provider)));
		}

		public static Ellipsoid FromSphere(in Sphere s)
		{
			return new Ellipsoid(s.Center, Matrix3.Identity, new Vector3(s.radius_, s.radius_, s.radius_));
		}

		//public static implicit operator Ellipsoid(Sphere s)
		//{
		//    return FromSphere(s);
		//}

		[Browsable(false)]
		public readonly bool IsFinite => center_.IsFinite && basis_.IsFinite && radii_.IsFinite;

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

		public Vector3 Radii
		{
			readonly get => radii_;
			set => radii_ = value;
		}

		[Browsable(false)]
		public Vector3 Diameters
		{
			readonly get => radii_*2f;
			set => radii_ = value*0.5f;
		}

		//[Browsable(false)]
		//public readonly float Area; // #TODO

		[Browsable(false)]
		public readonly float Volume => 4f*SingleConstants.Pi*radii_.X*radii_.Y*radii_.Z/3f;

		public readonly Sphere GetCircumscribedSphere()
		{
			return new Sphere(center_, Math.Max(Math.Max(radii_.X, radii_.Y), radii_.Z));
		}

		public void Orthonormalize()
		{
			//if (System.Numerics.Vector.IsHardwareAccelerated)
			//{
				radii_ *= new Vector3(basis_[0].Magnitude, basis_[1].Magnitude, basis_[2].Magnitude);
			//}
			//else
			//{
			//	radii_.X *= basis_[0].Magnitude;
			//	radii_.Y *= basis_[1].Magnitude;
			//	radii_.Z *= basis_[2].Magnitude;
			//}

			basis_.Orthonormalize();
		}

		public static Ellipsoid Orthonormalize(Ellipsoid ellipsoid)
		{
			ellipsoid.Orthonormalize();
			return ellipsoid;
		}

		public void Translate(Vector3 offset)
		{
			center_ += offset;
		}

		public static Ellipsoid Translate(Ellipsoid ellipsoid, Vector3 offset)
		{
			ellipsoid.Translate(offset);
			return ellipsoid;
		}

		public void Transform(in AffineTransform at)
		{
			basis_.Concat(at.r_);
			center_.Transform(at);
			Orthonormalize();
		}

		public static Ellipsoid Transform(Ellipsoid ellipsoid, in AffineTransform at)
		{
			ellipsoid.Transform(at);
			return ellipsoid;
		}

		/// <summary>
		/// Evaluate the quadratic function.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public readonly float Evaluate(Vector3 point)
		{
			Vector3 diff = point - center_;
			float ratio0 = Vector3.Dot(basis_[0], diff)/radii_[0];
			float ratio1 = Vector3.Dot(basis_[1], diff)/radii_[1];
			float ratio2 = Vector3.Dot(basis_[2], diff)/radii_[2];
			return ratio0*ratio0 + ratio1*ratio1 + ratio2*ratio2 - 1f;
		}

		public readonly bool Contains(Vector3 point)
		{
			return (Evaluate(point) <= 0f);
		}

		public readonly bool Intersects(in Plane plane)
		{
			Matrix3 mInverse = InverseMatrix;
			float discr = Vector3.Dot(plane.Normal, plane.Normal*mInverse);
			return (plane.GetDistanceTo(center_) <= MathF.Sqrt(Math.Abs(discr)));
		}

		internal readonly Matrix3 Matrix
		{
			get
			{
				Vector3 ratio0 = basis_[0]/radii_[0];
				Vector3 ratio1 = basis_[1]/radii_[1];
				Vector3 ratio2 = basis_[2]/radii_[2];
				return Vector3.Tensor(ratio0, ratio0) + Vector3.Tensor(ratio1, ratio1) + Vector3.Tensor(ratio2, ratio2);
			}
		}

		internal readonly Matrix3 InverseMatrix
		{
			get
			{
				Vector3 ratio0 = basis_[0]*radii_[0];
				Vector3 ratio1 = basis_[1]*radii_[1];
				Vector3 ratio2 = basis_[2]*radii_[2];
				return Vector3.Tensor(ratio0, ratio0) + Vector3.Tensor(ratio1, ratio1) + Vector3.Tensor(ratio2, ratio2);
			}
		}

		internal Vector3 center_;
		internal Matrix3 basis_;
		internal Vector3 radii_;
	}
}
