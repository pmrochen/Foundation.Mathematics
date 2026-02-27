/*
 *  Name: AffineTransform
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	[Serializable]
	[TypeConverter(typeof(AffineTransformConverter))]
	public struct AffineTransform : ISerializable, IFormattable, IEquatable<AffineTransform>
	{
#if SIMD
		public static readonly AffineTransform Zero = new AffineTransform(System.Numerics.Vector3.Zero, 
			System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero);
		public static readonly AffineTransform Identity = new AffineTransform(System.Numerics.Vector3.UnitX, 
			System.Numerics.Vector3.UnitY, System.Numerics.Vector3.UnitZ, System.Numerics.Vector3.Zero);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AffineTransform(float r00, float r01, float r02, float r10, float r11, float r12, float r20, float r21, float r22,
			float tx, float ty, float tz)
		{
			r_ = new Matrix3(r00, r01, r02, r10, r11, r12, r20, r21, r22);
			t_ = new Vector3(tx, ty, tz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AffineTransform(Vector3 row0, Vector3 row1, Vector3 row2, Vector3 t)
		{
			r_ = new Matrix3(row0, row1, row2);
			t_ = t;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal AffineTransform(System.Numerics.Vector3 row0, System.Numerics.Vector3 row1, System.Numerics.Vector3 row2,
			System.Numerics.Vector3 t)
		{
			r_ = new Matrix3(row0, row1, row2);
			t_ = new Vector3(t);
		}
#else
		public static readonly AffineTransform Zero = new AffineTransform(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		public static readonly AffineTransform Identity = new AffineTransform(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f);

		public AffineTransform(float r00, float r01, float r02, float r10, float r11, float r12, float r20, float r21, float r22,
			float tx, float ty, float tz)
		{
			r_.m00_ = r00; r_.m01_ = r01; r_.m02_ = r02;
			r_.m10_ = r10; r_.m11_ = r11; r_.m12_ = r12;
			r_.m20_ = r20; r_.m21_ = r21; r_.m22_ = r22;
			t_.x_ = tx; t_.y_ = ty; t_.z_ = tz;
		}

		public AffineTransform(Vector3 row0, Vector3 row1, Vector3 row2, Vector3 t)
		{
			r_.m00_ = row0.x_; r_.m01_ = row0.y_; r_.m02_ = row0.z_;
			r_.m10_ = row1.x_; r_.m11_ = row1.y_; r_.m12_ = row1.z_;
			r_.m20_ = row2.x_; r_.m21_ = row2.y_; r_.m22_ = row2.z_;
			t_.x_ = t.x_; t_.y_ = t.y_; t_.z_ = t.z_;
		}
#endif

		public AffineTransform(in Matrix3 r, Vector3 t)
		{
			r_ = r;
			t_ = t;
		}

		//public AffineTransform(in Matrix3 r)
		//{
		//	r_ = r;
		//	t_ = Vector3.Zero;
		//}

		//public AffineTransform(Vector3 t)
		//{
		//	r_ = Matrix3.Identity;
		//	t_ = t;
		//}

		private AffineTransform(SerializationInfo info, StreamingContext context)
		{
			r_ = (Matrix3)info.GetValue("Basis", typeof(Matrix3));
			t_ = (Vector3)info.GetValue("Origin", typeof(Vector3));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
			info.AddValue("Basis", r_, typeof(Matrix3));
			info.AddValue("Origin", t_, typeof(Vector3));
        }

        [Browsable(false)]
		public Matrix3 Basis
		{
			readonly get => r_;
			set => r_ = value;
		}

		public Vector3 Right
		{
			readonly get => r_.Row0;
			set => r_.Right = value;
		}

		public Vector3 Up
		{
			readonly get => r_.Up;
			set => r_.Up = value;
		}

		public Vector3 Forward
		{
			readonly get => r_.Forward;
			set => r_.Forward = value;
		}

		public Vector3 Origin
		{
			readonly get => t_;
			set => t_ = value;
		}

		public readonly override int GetHashCode()
		{
			int hash = r_.GetHashCode();
			return ((hash << 5) + hash) ^ t_.GetHashCode();
		}

		public readonly override bool Equals(object other)
		{
			if (other is AffineTransform rhs)
				return (r_ == rhs.r_) && (t_ == rhs.t_);
			
			return false;
		}

		public readonly bool Equals(AffineTransform other)
		{
			return (r_ == other.r_) && (t_ == other.t_);
		}

		public readonly bool ApproxEquals(in AffineTransform t, float tolerance)
		{
			return r_.ApproxEquals(t.r_, tolerance) && t_.ApproxEquals(t.t_, tolerance);
		}

		public readonly bool ApproxEquals(in AffineTransform t)
		{
			return r_.ApproxEquals(t.r_) && t_.ApproxEquals(t.t_);
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
				r_.m00_, r_.m01_, r_.m02_, r_.m10_, r_.m11_, r_.m12_, r_.m20_, r_.m21_, r_.m22_, t_.x_, t_.y_, t_.z_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider,
				"{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
				r_.m00_, r_.m01_, r_.m02_, r_.m10_, r_.m11_, r_.m12_, r_.m20_, r_.m21_, r_.m22_, t_.x_, t_.y_, t_.z_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
				r_.m00_.ToString(format), r_.m01_.ToString(format), r_.m02_.ToString(format),
				r_.m10_.ToString(format), r_.m11_.ToString(format), r_.m12_.ToString(format),
				r_.m20_.ToString(format), r_.m21_.ToString(format), r_.m22_.ToString(format),
				t_.x_.ToString(format), t_.y_.ToString(format), t_.z_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}",
				r_.m00_.ToString(format, provider), r_.m01_.ToString(format, provider), r_.m02_.ToString(format, provider),
				r_.m10_.ToString(format, provider), r_.m11_.ToString(format, provider), r_.m12_.ToString(format, provider),
				r_.m20_.ToString(format, provider), r_.m21_.ToString(format, provider), r_.m22_.ToString(format, provider),
				t_.x_.ToString(format, provider), t_.y_.ToString(format, provider), t_.z_.ToString(format, provider));
		}

		public static AffineTransform Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 12)
				throw new FormatException();

			return new AffineTransform(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]),
				Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5]),
				Single.Parse(m[6]), Single.Parse(m[7]), Single.Parse(m[8]),
				Single.Parse(m[9]), Single.Parse(m[10]), Single.Parse(m[11]));
		}

		public static AffineTransform Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 12)
				throw new FormatException();

			return new AffineTransform(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
				Single.Parse(m[2], provider), Single.Parse(m[3], provider),
				Single.Parse(m[4], provider), Single.Parse(m[5], provider),
				Single.Parse(m[6], provider), Single.Parse(m[7], provider),
				Single.Parse(m[8], provider), Single.Parse(m[9], provider),
				Single.Parse(m[10], provider), Single.Parse(m[11], provider));
		}

		public static implicit operator AffineTransform(in Matrix2 m)
		{
			return new AffineTransform((Matrix3)m, Vector3.Zero);
		}

		public static implicit operator AffineTransform(in Matrix3 m)
		{
			return new AffineTransform(m, Vector3.Zero);
		}

		public static implicit operator AffineTransform(in PackedTransform transform)
		{
			return FromPackedTransform(transform);
		}

		//public static explicit operator AffineTransform(Vector3 v)
		//{
		//    return new AffineTransform(Matrix3.Identity, v);
		//}

		public static bool operator ==(AffineTransform lhs, AffineTransform rhs)
		{
			return (lhs.r_ == rhs.r_) && (lhs.t_ == rhs.t_);
		}

		public static bool operator !=(AffineTransform lhs, AffineTransform rhs)
		{
			return (lhs.r_ != rhs.r_) || (lhs.t_ != rhs.t_);
		}

		public static AffineTransform FromPackedTransform(in PackedTransform transform)
		{
			Matrix3 r = Matrix3.FromQuaternion(transform.Rotation);
			r *= transform.Scale;
			return new AffineTransform(r, transform.Translation);
		}

		public static AffineTransform FromForward(Vector3 forward)
		{
			return new AffineTransform(Matrix3.FromForward(forward), Vector3.Zero);
		}

		public static AffineTransform FromUp(Vector3 up)
		{
			return new AffineTransform(Matrix3.FromUp(up), Vector3.Zero);
		}

		public static AffineTransform FromForwardAndUp(Vector3 forward, Vector3 up)
		{
			return new AffineTransform(Matrix3.FromForwardAndUp(forward, up), Vector3.Zero);
		}

		public static AffineTransform FromRightAndUp(Vector3 right, Vector3 up)
		{
			return new AffineTransform(Matrix3.FromRightAndUp(right, up), Vector3.Zero);
		}

		public static AffineTransform FromOriginAndTarget(Vector3 origin, Vector3 target)
		{
			return new AffineTransform(Matrix3.FromForward(target - origin), origin);
		}

		public static AffineTransform FromOriginTargetAndUp(Vector3 origin, Vector3 target, Vector3 up)
		{
			return new AffineTransform(Matrix3.FromForwardAndUp(target - origin, up), origin);
		}

		public static AffineTransform FromOriginAndForward(Vector3 origin, Vector3 forward)
		{
			return new AffineTransform(Matrix3.FromForward(forward), origin);
		}

		public static AffineTransform FromOriginForwardAndUp(Vector3 origin, Vector3 forward, Vector3 up)
		{
			return new AffineTransform(Matrix3.FromForwardAndUp(forward, up), origin);
		}

		public static AffineTransform FromOriginRightAndUp(Vector3 origin, Vector3 right, Vector3 up)
		{
			return new AffineTransform(Matrix3.FromRightAndUp(right, up), origin);
		}

		public static AffineTransform Concat(in AffineTransform s, in AffineTransform t)
		{
#if SIMD
			return new AffineTransform(Matrix3.Concat(s.r_, t.r_), Vector3.Transform(s.t_, t));
#else
			return new AffineTransform(s.r_.m00_*t.r_.m00_ + s.r_.m01_*t.r_.m10_ + s.r_.m02_*t.r_.m20_,
				s.r_.m00_*t.r_.m01_ + s.r_.m01_*t.r_.m11_ + s.r_.m02_*t.r_.m21_,
				s.r_.m00_*t.r_.m02_ + s.r_.m01_*t.r_.m12_ + s.r_.m02_*t.r_.m22_,
				s.r_.m10_*t.r_.m00_ + s.r_.m11_*t.r_.m10_ + s.r_.m12_*t.r_.m20_,
				s.r_.m10_*t.r_.m01_ + s.r_.m11_*t.r_.m11_ + s.r_.m12_*t.r_.m21_,
				s.r_.m10_*t.r_.m02_ + s.r_.m11_*t.r_.m12_ + s.r_.m12_*t.r_.m22_,
				s.r_.m20_*t.r_.m00_ + s.r_.m21_*t.r_.m10_ + s.r_.m22_*t.r_.m20_,
				s.r_.m20_*t.r_.m01_ + s.r_.m21_*t.r_.m11_ + s.r_.m22_*t.r_.m21_,
				s.r_.m20_*t.r_.m02_ + s.r_.m21_*t.r_.m12_ + s.r_.m22_*t.r_.m22_,
				s.t_.x_*t.r_.m00_ + s.t_.y_*t.r_.m10_ + s.t_.z_*t.r_.m20_ + t.t_.x_,
				s.t_.x_*t.r_.m01_ + s.t_.y_*t.r_.m11_ + s.t_.z_*t.r_.m21_ + t.t_.y_,
				s.t_.x_*t.r_.m02_ + s.t_.y_*t.r_.m12_ + s.t_.z_*t.r_.m22_ + t.t_.z_);
#endif
		}

		public static AffineTransform Concat(in AffineTransform s, in AffineTransform t, in AffineTransform u)
		{
            return Concat(Concat(s, t), u);
        }

		public static AffineTransform Concat(in AffineTransform s, in AffineTransform t, in AffineTransform u,
			in AffineTransform v)
		{
            return Concat(Concat(Concat(s, t), u), v);
        }

		public static AffineTransform Concat(in AffineTransform s, in AffineTransform t, in AffineTransform u,
			in AffineTransform v, in AffineTransform w)
		{
            return Concat(Concat(Concat(Concat(s, t), u), v), w);
        }

		public static AffineTransform Concat(in AffineTransform s, in AffineTransform t, in AffineTransform u,
			in AffineTransform v, in AffineTransform w, in AffineTransform x)
		{
            return Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x);
        }

		public static AffineTransform Concat(in AffineTransform s, in AffineTransform t, in AffineTransform u,
			in AffineTransform v, in AffineTransform w, in AffineTransform x, in AffineTransform y)
		{
            return Concat(Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x), y);
        }

		public static AffineTransform Concat(in AffineTransform s, in AffineTransform t, in AffineTransform u,
			in AffineTransform v, in AffineTransform w, in AffineTransform x, in AffineTransform y, in AffineTransform z)
		{
            return Concat(Concat(Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x), y), z);
        }

		public void Concat(in AffineTransform t)
		{
			this = Concat(this, t);
		}

		public void PreConcat(in AffineTransform t)
		{
			this = Concat(t, this);
		}

		public static AffineTransform Rotation(Axis axis, float angle)
		{
			return new AffineTransform(Matrix3.Rotation(axis, angle), Vector3.Zero);
		}

		//public void Rotate(Axis axis, float angle)
		//{
		//	this = Concat(this, new AffineTransform(Matrix3.Rotation(axis, angle), Vector3.Zero));
		//}

		public static AffineTransform Rotation(Vector3 axis, float angle)
		{
			return new AffineTransform(Matrix3.Rotation(axis, angle), Vector3.Zero);
		}

		//public void Rotate(Vector3 axis, float angle)
		//{
		//	this = Concat(this, new AffineTransform(Matrix3.Rotation(axis, angle), Vector3.Zero));
		//}

		public static AffineTransform Rotation(YawPitchRoll ypr)
		{
			return new AffineTransform(Matrix3.Rotation(ypr), Vector3.Zero);
		}

		//public void Rotate(YawPitchRoll ypr)
		//{
		//	this = Concat(this, new AffineTransform(Matrix3.Rotation(ypr), Vector3.Zero));
		//}

		public static AffineTransform Rotation(Euler e)
		{
			return new AffineTransform(Matrix3.Rotation(e), Vector3.Zero);
		}

		//public void Rotate(Euler e)
		//{
		//	this = Concat(this, new AffineTransform(Matrix3.Rotation(e), Vector3.Zero));
		//}

		public static AffineTransform Rotation(Quaternion q)
		{
			return new AffineTransform(Matrix3.Rotation(q), Vector3.Zero);
		}

		//public void Rotate(Quaternion q)
		//{
		//	this = Concat(this, new AffineTransform(Matrix3.Rotation(q), Vector3.Zero));
		//}

		public static AffineTransform Scaling(Vector3 v)
		{
#if SIMD
			return new AffineTransform(Matrix3.Scaling(v), Vector3.Zero);
#else
			return new AffineTransform(v.x_, 0f, 0f, 0f, v.y_, 0f, 0f, 0f, v.z_, 0f, 0f, 0f);
#endif
		}

		public static AffineTransform Scaling(float s)
		{
#if SIMD
			return new AffineTransform(Matrix3.Scaling(s), Vector3.Zero);
#else
			return new AffineTransform(s, 0f, 0f, 0f, s, 0f, 0f, 0f, s, 0f, 0f, 0f);
#endif
		}

		public void PreScale(Vector3 v)
		{
			r_.PreScale(v);
		}

		public void Scale(Vector3 v)
		{
			r_.Scale(v);
#if SIMD
			t_ *= v;
#else
			t_.x_ *= v.x_; 
			t_.y_ *= v.y_; 
			t_.z_ *= v.z_;
#endif
		}

		public static AffineTransform Translation(Vector3 v)
		{
#if SIMD
			return new AffineTransform(Matrix3.Identity, v);
#else
			return new AffineTransform(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f, v.x_, v.y_, v.z_);
#endif
		}

		public void Translate(Vector3 v)
		{
#if SIMD
			t_ += v;
#else
			t_.x_ += v.x_;
			t_.y_ += v.y_;
			t_.z_ += v.z_;
#endif
		}

		public static AffineTransform RotationTranslation(Axis axis, float angle, Vector3 t)
		{
			return new AffineTransform(Matrix3.Rotation(axis, angle), t);
		}

		public static AffineTransform RotationTranslation(Vector3 axis, float angle, Vector3 t)
		{
			return new AffineTransform(Matrix3.Rotation(axis, angle), t);
		}

		public static AffineTransform RotationTranslation(YawPitchRoll ypr, Vector3 t)
		{
			return new AffineTransform(Matrix3.Rotation(ypr), t);
		}

		public static AffineTransform RotationTranslation(Euler e, Vector3 t)
		{
			return new AffineTransform(Matrix3.Rotation(e), t);
		}

		public static AffineTransform RotationTranslation(Quaternion q, Vector3 t)
		{
			return new AffineTransform(Matrix3.Rotation(q), t);
		}

		public static AffineTransform ScalingRotation(Vector3 s, Axis axis, float angle)
		{
			return new AffineTransform(Matrix3.ScalingRotation(s, axis, angle), Vector3.Zero);
		}

		public static AffineTransform ScalingRotation(Vector3 s, Vector3 axis, float angle)
		{
			return new AffineTransform(Matrix3.ScalingRotation(s, axis, angle), Vector3.Zero);
		}

		public static AffineTransform ScalingRotation(Vector3 s, YawPitchRoll ypr)
		{
			return new AffineTransform(Matrix3.ScalingRotation(s, ypr), Vector3.Zero);
		}

		public static AffineTransform ScalingRotation(Vector3 s, Euler e)
		{
			return new AffineTransform(Matrix3.ScalingRotation(s, e), Vector3.Zero);
		}

		public static AffineTransform ScalingRotation(Vector3 s, Quaternion q)
		{
			return new AffineTransform(Matrix3.ScalingRotation(s, q), Vector3.Zero);
		}

		public static AffineTransform ScalingTranslation(Vector3 s, Vector3 t)
		{
#if SIMD
			return new AffineTransform(Matrix3.Scaling(s), t);
#else
			return new AffineTransform(s.x_, 0f, 0f, 0f, s.y_, 0f, 0f, 0f, s.z_, t.x_, t.y_, t.z_);
#endif
		}

		public static AffineTransform ScalingRotationTranslation(Vector3 s, Axis axis, float angle, Vector3 t)
		{
			return new AffineTransform(Matrix3.ScalingRotation(s, axis, angle), t);
		}

		public static AffineTransform ScalingRotationTranslation(Vector3 s, Vector3 axis, float angle, Vector3 t)
		{
			return new AffineTransform(Matrix3.ScalingRotation(s, axis, angle), t);
		}

		public static AffineTransform ScalingRotationTranslation(Vector3 s, YawPitchRoll ypr, Vector3 t)
		{
			return new AffineTransform(Matrix3.ScalingRotation(s, ypr), t);
		}

		public static AffineTransform ScalingRotationTranslation(Vector3 s, Euler e, Vector3 t)
		{
			return new AffineTransform(Matrix3.ScalingRotation(s, e), t);
		}

		public static AffineTransform ScalingRotationTranslation(Vector3 s, Quaternion q, Vector3 t)
		{
			return new AffineTransform(Matrix3.ScalingRotation(s, q), t);
		}

		public static AffineTransform Shearing(float xy, float xz, float yx, float yz, float zx, float zy)
		{
			return new AffineTransform(1f, xy, xz, yx, 1f, yz, zx, zy, 1f, 0f, 0f, 0f);
		}

		//public void Shear(float xy, float xz, float yx, float yz, float zx, float zy)
		//{
		//	this = Concat(this, new AffineTransform(1f, xy, xz, yx, 1f, yz, zx, zy, 1f, 0f, 0f, 0f));
		//}

		public static AffineTransform Reflection(in Plane plane)
		{
			Plane p = Plane.Normalize(plane);
#if SIMD
			System.Numerics.Vector3 n = p.abc_;
			System.Numerics.Vector4 t = -2f*p.abcd_;
			return new AffineTransform(t.X*n + System.Numerics.Vector3.UnitX,
				t.Y*n + System.Numerics.Vector3.UnitY,
				t.Z*n + System.Numerics.Vector3.UnitZ,
				t.W*n);
#else
			return new AffineTransform(-2f*p.a_*p.a_ + 1f, -2f*p.b_*p.a_, -2f*p.c_*p.a_,
				-2f*p.a_*p.b_, -2f*p.b_*p.b_ + 1f, -2f*p.c_*p.b_,
				-2f*p.a_*p.c_, -2f*p.b_*p.c_, -2f*p.c_*p.c_ + 1f,
				-2f*p.a_*p.d_, -2f*p.b_*p.d_, -2f*p.c_*p.d_);
#endif
		}

		public static AffineTransform Inverse(in AffineTransform t)
		{
			Matrix3 r = Matrix3.Inverse(t.r_);
			return new AffineTransform(r, -(t.t_*r));
		}

		public static AffineTransform InverseOrthogonal(in AffineTransform t)
		{
			Matrix3 r = Matrix3.Transpose(t.r_);
			return new AffineTransform(r, -(t.t_*r));
		}

		/// <summary>
		/// Stabilized Gram-Schmidt orthonormalization.
		/// </summary>
		public static AffineTransform Orthonormalize(in AffineTransform t)
		{
			return new AffineTransform(Matrix3.Orthonormalize(t.r_), t.t_);
		}

		public void Invert()
		{
			r_.Invert();
			t_ = -(t_*r_);
		}

		public void InvertOrthogonal()
		{
			r_.Transpose();
			t_ = -(t_*r_);
		}

		/// <summary>
		/// Stabilized Gram-Schmidt orthonormalization.
		/// </summary>
		public void Orthonormalize()
        {
            r_.Orthonormalize();
        }

		[Browsable(false)]
		public readonly bool IsFinite => r_.IsFinite && t_.IsFinite;

		[Browsable(false)]
		public readonly Vector3 ScaleComponent => r_.ScaleComponent;

#if SIMD
		internal readonly System.Numerics.Vector4 column0_ => new System.Numerics.Vector4(r_.row0_.X, r_.row1_.X, r_.row2_.X, t_.xyz_.X);
		internal readonly System.Numerics.Vector4 column1_ => new System.Numerics.Vector4(r_.row0_.Y, r_.row1_.Y, r_.row2_.Y, t_.xyz_.Y);
		internal readonly System.Numerics.Vector4 column2_ => new System.Numerics.Vector4(r_.row0_.Z, r_.row1_.Z, r_.row2_.Z, t_.xyz_.Z);
		internal readonly System.Numerics.Vector3 row0_ => r_.row0_;
		internal readonly System.Numerics.Vector3 row1_ => r_.row1_;
		internal readonly System.Numerics.Vector3 row2_ => r_.row2_;
		internal readonly System.Numerics.Vector3 row3_ => t_.xyz_;
#endif

		internal Matrix3 r_;
		internal Vector3 t_;
	}
}
