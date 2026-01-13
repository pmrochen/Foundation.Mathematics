/*
 *  Name: Quaternion
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	[Serializable]
	[TypeConverter(typeof(QuaternionConverter))]
	public struct Quaternion : ISerializable, IFormattable, IEquatable<Quaternion>
	{
#if SIMD
		public static readonly Quaternion Zero = new Quaternion(System.Numerics.Vector4.Zero);
		public static readonly Quaternion Identity = new Quaternion(System.Numerics.Vector4.UnitW);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Quaternion(float x, float y, float z, float w)
		{
			xyzw_ = new System.Numerics.Vector4(x, y, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Quaternion(float scalar)
		{
			xyzw_ = new System.Numerics.Vector4(System.Numerics.Vector3.Zero, scalar);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Quaternion(Vector3 vector)
		{
			xyzw_ = new System.Numerics.Vector4(vector.xyz_, 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Quaternion(Vector3 vector, float scalar)
		{
			xyzw_ = new System.Numerics.Vector4(vector.xyz_, scalar);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Quaternion(float[] q)
		{
			xyzw_ = new System.Numerics.Vector4(q[0], q[1], q[2], q[3]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Quaternion(System.Numerics.Vector4 v)
		{
			xyzw_ = v;
		}

		private Quaternion(SerializationInfo info, StreamingContext context)
		{
			xyzw_ = new System.Numerics.Vector4(info.GetSingle("X"), info.GetSingle("Y"), info.GetSingle("Z"), info.GetSingle("W"));
		}

		public float X
		{
			readonly get => xyzw_.X;
			set => xyzw_.X = value;
		}

		public float Y
		{
			readonly get => xyzw_.Y;
			set => xyzw_.Y = value;
		}

		public float Z
		{
			readonly get => xyzw_.Z;
			set => xyzw_.Z = value;
		}

		public float W
		{
			readonly get => xyzw_.W;
			set => xyzw_.W = value;
		}

		[Browsable(false)]
		public Vector3 /*Imaginary*/Vector
		{
			readonly get => new Vector3(xyz_);
			set => xyzw_ = new System.Numerics.Vector4(value.xyz_, xyzw_.W);
		}

		[Browsable(false)]
		public float /*Real*/Scalar
		{
			readonly get => xyzw_.W;
			set => xyzw_.W = value;
		}

		[Browsable(false)]
		public readonly float AbsoluteValue => xyzw_.Length();

		[Browsable(false)]
		public readonly float Norm => xyzw_.LengthSquared();

		[Browsable(false)]
		public readonly float Magnitude => xyzw_.Length();

		[Browsable(false)]
		public readonly float MagnitudeSquared => xyzw_.LengthSquared();

		public readonly override bool Equals(object other)
		{
			return (other is Quaternion rhs) && (xyzw_ == rhs.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Quaternion other)
		{
			return (xyzw_ == other.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Quaternion lhs, Quaternion rhs)
		{
			return (lhs.xyzw_ == rhs.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Quaternion lhs, Quaternion rhs)
		{
			return (lhs.xyzw_ != rhs.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion operator +(Quaternion q)
		{
			return q;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion operator -(Quaternion q)
		{
			return new Quaternion(-q.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion operator +(Quaternion q, Quaternion r)
		{
			return new Quaternion(q.xyzw_ + r.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion operator -(Quaternion q, Quaternion r)
		{
			return new Quaternion(q.xyzw_ - r.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion operator *(Quaternion q, float f)
		{
			return new Quaternion(q.xyzw_*f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion operator *(float f, Quaternion q)
		{
			return new Quaternion(f*q.xyzw_);
		}

		public static Quaternion operator *(Quaternion q, Vector3 v) // #TODO SIMD
		{
			return new Quaternion(q.w_*v.x_ + q.y_*v.z_ - q.z_*v.y_,
				q.w_*v.y_ + q.z_*v.x_ - q.x_*v.z_,
				q.w_*v.z_ + q.x_*v.y_ - q.y_*v.x_,
				-(q.x_*v.x_ + q.y_*v.y_ + q.z_*v.z_));
		}

		public static Quaternion operator *(Vector3 v, Quaternion q) // #TODO SIMD
		{
			return new Quaternion(q.w_*v.x_ + q.z_*v.y_ - q.y_*v.z_,
				q.w_*v.y_ + q.x_*v.z_ - q.z_*v.x_,
				q.w_*v.z_ + q.y_*v.x_ - q.x_*v.y_,
				-(q.x_*v.x_ + q.y_*v.y_ + q.z_*v.z_));
		}

		public static Quaternion operator *(Quaternion q, Quaternion r) // #TODO SIMD
		{
			return new Quaternion(q.w_*r.x_ + q.x_*r.w_ + q.y_*r.z_ - q.z_*r.y_,
				q.w_*r.y_ - q.x_*r.z_ + q.y_*r.w_ + q.z_*r.x_,
				q.w_*r.z_ + q.x_*r.y_ - q.y_*r.x_ + q.z_*r.w_,
				q.w_*r.w_ - q.x_*r.x_ - q.y_*r.y_ - q.z_*r.z_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion operator /(Quaternion q, float f)
		{
			return new Quaternion(q.xyzw_/f);
		}

		public static Quaternion operator /(float f, Quaternion q)
		{
			return f*Inverse(q);
		}

		public float this[int index] // #TODO Use System.Numerics.Vector4 indexing operator
		{
			readonly get
			{
				switch (index)
				{
					case 0:
						return xyzw_.X;
					case 1:
						return xyzw_.Y;
					case 2:
						return xyzw_.Z;
					case 3:
						return xyzw_.W;
					default:
						throw new IndexOutOfRangeException();
				}
			}

			set
			{
				switch (index)
				{
					case 0:
						xyzw_.X = value;
						break;
					case 1:
						xyzw_.Y = value;
						break;
					case 2:
						xyzw_.Z = value;
						break;
					case 3:
						xyzw_.W = value;
						break;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Quaternion q, Quaternion r)
		{
			return System.Numerics.Vector4.Dot(q.xyzw_, r.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Quaternion Normalize(Quaternion q)
		{
			float m = q.xyzw_.Length();
			return (m > 0f) ? new Quaternion(q.xyzw_/m) : q;
		}

		public static Quaternion Conjugate(Quaternion q)
		{
			return new Quaternion(new System.Numerics.Vector4(-q.xyz_, q.w_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Normalize()
		{
			float m = xyzw_.Length();
			if (m > 0f)
				xyzw_ /= m;
		}

		public void Conjugate()
		{
			xyzw_ = new System.Numerics.Vector4(-xyz_, w_);
		}

		internal readonly float x_ => xyzw_.X;
		internal readonly float y_ => xyzw_.Y;
		internal readonly float z_ => xyzw_.Z;
		internal readonly float w_ => xyzw_.W;
		internal readonly System.Numerics.Vector3 xyz_ => new System.Numerics.Vector3(xyzw_.X, xyzw_.Y, xyzw_.Z);
		internal readonly System.Numerics.Vector4 wzyx_ => new System.Numerics.Vector4(xyzw_.W, xyzw_.Z, xyzw_.Y, xyzw_.X);

		internal System.Numerics.Vector4 xyzw_;
#else
		public static readonly Quaternion Zero = new Quaternion(0f, 0f, 0f, 0f);
		public static readonly Quaternion Identity = new Quaternion(0f, 0f, 0f, 1f);

		public Quaternion(float x, float y, float z, float w)
		{
			x_ = x;
			y_ = y;
			z_ = z;
			w_ = w;
		}

		public Quaternion(float scalar)
		{
			x_ = 0f;
			y_ = 0f;
			z_ = 0f;
			w_ = scalar;
		}

		public Quaternion(Vector3 vector)
		{
			x_ = vector.x_;
			y_ = vector.y_;
			z_ = vector.z_;
			w_ = 0f;
		}

		public Quaternion(Vector3 vector, float scalar)
		{
			x_ = vector.x_;
			y_ = vector.y_;
			z_ = vector.z_;
			w_ = scalar;
		}

		public Quaternion(float[] q)
		{
			x_ = q[0];
			y_ = q[1];
			z_ = q[2];
			w_ = q[3];
		}

		private Quaternion(SerializationInfo info, StreamingContext context)
		{
			x_ = info.GetSingle("X");
			y_ = info.GetSingle("Y");
			z_ = info.GetSingle("Z");
			w_ = info.GetSingle("W");
		}

		public float X
		{
			readonly get => x_;
			set => x_ = value;
		}

		public float Y
		{
			readonly get => y_;
			set => y_ = value;
		}

		public float Z
		{
			readonly get => z_;
			set => z_ = value;
		}

		public float W
		{
			readonly get => w_;
			set => w_ = value;
		}

		[Browsable(false)]
		public Vector3 /*Imaginary*/Vector
		{
			readonly get => new Vector3(x_, y_, z_);
			set 
			{ 
				x_ = value.x_; 
				y_ = value.y_; 
				z_ = value.z_; 
			}
		}

		[Browsable(false)]
		public float /*Real*/Scalar
		{
			readonly get => w_;
			set => w_ = value;
		}

		[Browsable(false)]
		public readonly float AbsoluteValue => MathF.Sqrt(x_*x_ + y_*y_ + z_*z_ + w_*w_);

		[Browsable(false)]
		public readonly float Norm => x_*x_ + y_*y_ + z_*z_ + w_*w_;

		[Browsable(false)]
		public readonly float Magnitude => MathF.Sqrt(x_*x_ + y_*y_ + z_*z_ + w_*w_);

		[Browsable(false)]
		public readonly float MagnitudeSquared => x_*x_ + y_*y_ + z_*z_ + w_*w_;

		public readonly override bool Equals(object other)
		{
			if (other is Quaternion rhs)
				return (x_ == rhs.x_) && (y_ == rhs.y_) && (z_ == rhs.z_) && (w_ == rhs.w_);
			
			return false;
		}

		public readonly bool Equals(Quaternion other)
		{
			return (x_ == other.x_) && (y_ == other.y_) && (z_ == other.z_) && (w_ == other.w_);
		}

		public static bool operator ==(Quaternion lhs, Quaternion rhs)
		{
			return (lhs.x_ == rhs.x_) && (lhs.y_ == rhs.y_) && (lhs.z_ == rhs.z_) && (lhs.w_ == rhs.w_);
		}

		public static bool operator !=(Quaternion lhs, Quaternion rhs)
		{
			return (lhs.x_ != rhs.x_) || (lhs.y_ != rhs.y_) || (lhs.z_ != rhs.z_) || (lhs.w_ != rhs.w_);
		}

		public static Quaternion operator +(Quaternion q)
		{
			return q;
		}

		public static Quaternion operator -(Quaternion q)
		{
			return new Quaternion(-q.x_, -q.y_, -q.z_, -q.w_);
		}

		public static Quaternion operator +(Quaternion q, Quaternion r)
		{
			return new Quaternion(q.x_ + r.x_, q.y_ + r.y_, q.z_ + r.z_, q.w_ + r.w_);
		}

		public static Quaternion operator -(Quaternion q, Quaternion r)
		{
			return new Quaternion(q.x_ - r.x_, q.y_ - r.y_, q.z_ - r.z_, q.w_ - r.w_);
		}

		public static Quaternion operator *(Quaternion q, float f)
		{
			return new Quaternion(q.x_*f, q.y_*f, q.z_*f, q.w_*f);
		}

		public static Quaternion operator *(float f, Quaternion q)
		{
			return new Quaternion(f*q.x_, f*q.y_, f*q.z_, f*q.w_);
		}

		public static Quaternion operator *(Quaternion q, Vector3 v)
		{
			return new Quaternion(q.w_*v.x_ + q.y_*v.z_ - q.z_*v.y_,
				q.w_*v.y_ + q.z_*v.x_ - q.x_*v.z_,
				q.w_*v.z_ + q.x_*v.y_ - q.y_*v.x_,
				-(q.x_*v.x_ + q.y_*v.y_ + q.z_*v.z_));
		}

		public static Quaternion operator *(Vector3 v, Quaternion q)
		{
			return new Quaternion(q.w_*v.x_ + q.z_*v.y_ - q.y_*v.z_,
				q.w_*v.y_ + q.x_*v.z_ - q.z_*v.x_,
				q.w_*v.z_ + q.y_*v.x_ - q.x_*v.y_,
				-(q.x_*v.x_ + q.y_*v.y_ + q.z_*v.z_));
		}

		public static Quaternion operator *(Quaternion q, Quaternion r)
		{
			return new Quaternion(q.w_*r.x_ + q.x_*r.w_ + q.y_*r.z_ - q.z_*r.y_,
				q.w_*r.y_ - q.x_*r.z_ + q.y_*r.w_ + q.z_*r.x_,
				q.w_*r.z_ + q.x_*r.y_ - q.y_*r.x_ + q.z_*r.w_,
				q.w_*r.w_ - q.x_*r.x_ - q.y_*r.y_ - q.z_*r.z_);
		}

		public static Quaternion operator /(Quaternion q, float f)
		{
			return new Quaternion(q.x_/f, q.y_/f, q.z_/f, q.w_/f);
		}

		public static Quaternion operator /(float f, Quaternion q)
		{
			return f*Inverse(q);
		}

		public float this[int index]
		{
			readonly get
			{
				switch (index)
				{
					case 0:
						return x_;
					case 1:
						return y_;
					case 2:
						return z_;
					case 3:
						return w_;
					default:
						throw new IndexOutOfRangeException();
				}
			}

			set
			{
				switch (index)
				{
					case 0:
						x_ = value;
						break;
					case 1:
						y_ = value;
						break;
					case 2:
						z_ = value;
						break;
					case 3:
						w_ = value;
						break;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		public static float Dot(Quaternion q, Quaternion r)
		{
			return (q.x_*r.x_ + q.y_*r.y_ + q.z_*r.z_ + q.w_*r.w_);
		}

		public static Quaternion Normalize(Quaternion q)
		{
			float m = q.Magnitude;
			return (m > 0f) ? new Quaternion(q.x_/m, q.y_/m, q.z_/m, q.w_/m) : q;
		}
		
		public static Quaternion Conjugate(Quaternion q)
		{
			return new Quaternion(-q.x_, -q.y_, -q.z_, q.w_);
		}

		public void Normalize()
		{
			float m = Magnitude;
			if (m > 0f)
			{
				x_ /= m;
				y_ /= m;
				z_ /= m;
				w_ /= m;
			}
		}

		public void Conjugate()
		{
			x_ = -x_;
			y_ = -y_;
			z_ = -z_;
		}

		internal float x_;
		internal float y_;
		internal float z_;
		internal float w_;
#endif

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("X", x_);
			info.AddValue("Y", y_);
			info.AddValue("Z", z_);
			info.AddValue("W", w_);
		}

		public static explicit operator Quaternion(Vector3 vector)
		{
			return new Quaternion(vector);
		}

		public static explicit operator Quaternion(float scalar)
		{
			return new Quaternion(scalar);
		}

		public static explicit operator Quaternion(in Matrix3 m)
		{
			return FromMatrix(m);
		}

		[Browsable(false)]
		public readonly Vector3 Axis
		{
			get
			{
				float cosI = 1f - w_*w_;
				if (cosI <= 0f)
					return Vector3.Zero;
				float m = MathF.Sqrt(cosI);
				if (m > 0f)
					return /*Imaginary*/Vector/m;
				else
					return Vector3.Zero;
			}
		}

		[Browsable(false)]
		public readonly float Angle => 2f*(float)Math.Acos(Math.Clamp(w_, -1f, 1f));

		[Browsable(false)]
		public readonly bool IsFinite
		{
			get => Functions.IsFinite(x_) && Functions.IsFinite(y_) && Functions.IsFinite(z_) && Functions.IsFinite(w_);
		}

		public readonly override int GetHashCode()
		{
			int hash = x_.GetHashCode();
			hash = ((hash << 5) + hash) ^ y_.GetHashCode();
			hash = ((hash << 5) + hash) ^ z_.GetHashCode();
			return ((hash << 5) + hash) ^ w_.GetHashCode();
		}

		public readonly bool ApproxEquals(Quaternion q, float tolerance)
		{
			return (Math.Abs(q.x_ - x_) < tolerance) && (Math.Abs(q.y_ - y_) < tolerance) && (Math.Abs(q.z_ - z_) < tolerance) &&
				(Math.Abs(q.w_ - w_) < tolerance);
		}

		public readonly bool ApproxEquals(Quaternion q)
		{
			return ApproxEquals(q, 1e-6f);
		}

		public readonly float[] ToArray()
		{
			return new float[4] { x_, y_, z_, w_ };
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1} {2} {3}", x_, y_, z_, w_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3}", x_, y_, z_, w_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1} {2} {3}",
				x_.ToString(format), y_.ToString(format), z_.ToString(format), w_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3}",
				x_.ToString(format, provider), y_.ToString(format, provider), z_.ToString(format, provider), w_.ToString(format, provider));
		}

		public static Quaternion Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Quaternion(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]), Single.Parse(m[3]));
		}

		public static Quaternion Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Quaternion(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
				Single.Parse(m[2], provider), Single.Parse(m[3], provider));
		}

		public static Quaternion FromMatrix/*3*/(in Matrix3 m)
		{
			float tr = m.m00_ + m.m11_ + m.m22_;
			if (tr > 0f)
			{
				float s = MathF.Sqrt(tr + 1f);
				float t = 0.5f/s;
				return new Quaternion((m.m12_ - m.m21_)*t, (m.m20_ - m.m02_)*t, (m.m01_ - m.m10_)*t, 0.5f*s);
			}
			else
			{
				int i = 0;
				if (m.m11_ > m.m00_)
					i = 1;
				if (m.m22_ > m[i][i])
					i = 2;
				int j = next_[i];
				int k = next_[j];
				float s = MathF.Sqrt(m[i][i] - (m[j][j] + m[k][k]) + 1f);
				float t = 0.5f/s;

				Quaternion q = new Quaternion();
				q[i] = 0.5f*s;
				q[j] = (m[i][j] + m[j][i])*t;
				q[k] = (m[i][k] + m[k][i])*t;
				q.W = (m[j][k] - m[k][j])*t;
				return q;
			}
		}

		public static Quaternion FromYawPitchRoll(YawPitchRoll ypr)
		{
			if ((ypr.yaw_ != 0f) || (ypr.pitch_ != 0f) || (ypr.roll_ != 0f))
			{
				float ti = ypr.roll_*0.5f;
				float tj = ypr.pitch_*0.5f;
				float th = ypr.yaw_*0.5f;

				float si = (float)Math.Sin(ti);
				float ci = (float)Math.Cos(ti);
				float sj = (float)Math.Sin(tj);
				float cj = (float)Math.Cos(tj);
				float sh = (float)Math.Sin(th);
				float ch = (float)Math.Cos(th);

				float cc = ci*ch;
				float cs = ci*sh;
				float sc = si*ch;
				float ss = si*sh;

				return new Quaternion(cj*ss + sj*cc, cj*cs - sj*sc, cj*sc - sj*cs, cj*cc + sj*ss);
			}

			return Identity;
		}

		public static Quaternion FromEuler(Euler e)
		{
			if (((e.x_ != 0f) || (e.y_ != 0f) || (e.z_ != 0f)) && (e.order_ != EulerOrder.Unspecified))
			{
				int o = (int)e.order_;
				int f = o & 1; o >>= 1;
				int s = o & 1; o >>= 1;
				int n = o & 1; o >>= 1;
				int i = safe_[o & 3];
				int j = next_[i + n];
				int k = next_[i + 1 - n];

				float ti = e[(f != 0) ? ((s != 0) ? i : k) : i]*0.5f;
				float tj = e[j]*0.5f;
				float th = e[(f != 0) ? i : ((s != 0) ? i : k)]*0.5f;

				if (f != 0)
				{
					float t = ti;
					ti = th;
					th = t;
				}

				if (n != 0)
					tj = -tj;

				float si = (float)Math.Sin(ti);
				float ci = (float)Math.Cos(ti);
				float sj = (float)Math.Sin(tj);
				float cj = (float)Math.Cos(tj);
				float sh = (float)Math.Sin(th);
				float ch = (float)Math.Cos(th);

				float cc = ci*ch;
				float cs = ci*sh;
				float sc = si*ch;
				float ss = si*sh;

				Quaternion q = new Quaternion();
				if (s != 0)
				{
					q[i] = cj*(cs + sc);
					q[j] = sj*(cc + ss);
					q[k] = sj*(cs - sc);
					q.W = cj*(cc - ss);
				}
				else
				{
					q[i] = cj*sc - sj*cs;
					q[j] = cj*ss + sj*cc;
					q[k] = cj*cs - sj*sc;
					q.W = cj*cc + sj*ss;
				}

				if (n != 0)
					q[j] = -q[j];
				return q;
			}

			return Identity;
		}

		public static Quaternion FromAxisAngle(Vector3 axis, float angle)
		{
			float m = axis.Magnitude;
			if ((m > 0f) && (angle != 0f))
			{
				axis /= m;
				float half = angle*0.5f;
				float sinHalf = (float)Math.Sin(half);
				float cosHalf = (float)Math.Cos(half);
#if SIMD
				return new Quaternion(axis*sinHalf, cosHalf);
#else
				return new Quaternion(axis.x_*sinHalf, axis.y_*sinHalf, axis.z_*sinHalf, cosHalf);
#endif
			}

			return Identity;
		}

		public static Quaternion Concat(Quaternion q, Quaternion r)
		{
			return r*q;
		}

		public static Quaternion Concat(Quaternion q, Quaternion r, Quaternion u)
		{
			return Concat(Concat(q, r), u);
		}

		public static Quaternion Concat(Quaternion q, Quaternion r, Quaternion u, Quaternion v)
		{
			return Concat(Concat(Concat(q, r), u), v);
		}

		public static Quaternion Concat(Quaternion q, Quaternion r, Quaternion u, Quaternion v, Quaternion w)
		{
			return Concat(Concat(Concat(Concat(q, r), u), v), w);
		}

		public static Quaternion Concat(Quaternion q, Quaternion r, Quaternion u, Quaternion v, Quaternion w, Quaternion x)
		{
			return Concat(Concat(Concat(Concat(Concat(q, r), u), v), w), x);
		}

		public static Quaternion Concat(Quaternion q, Quaternion r, Quaternion u, Quaternion v, Quaternion w, Quaternion x, Quaternion y)
		{
			return Concat(Concat(Concat(Concat(Concat(Concat(q, r), u), v), w), x), y);
		}

		public static Quaternion Concat(Quaternion q, Quaternion r, Quaternion u, Quaternion v, Quaternion w, Quaternion x, Quaternion y, Quaternion z)
		{
			return Concat(Concat(Concat(Concat(Concat(Concat(Concat(q, r), u), v), w), x), y), z);
		}

		public static Quaternion Rotate(Quaternion q, Quaternion r)
		{
			return r*q*Conjugate(r);
		}

		public static Quaternion Inverse(Quaternion q)
		{
			return Conjugate(q)/q.Norm;
		}

		public static Quaternion Lerp(Quaternion q, Quaternion r, float t)
		{
			float cosTheta = Dot(q, r);

			float t0 = 1f - t;
			float t1 = t;
			if (/*shortestArc &&*/ (cosTheta < 0f))
			{
				// If r is on the oposite hemisphere use -r instead of r
				t1 = -t1;
			}

#if SIMD
			return /*Quaternion.Normalize*/(t0*q + t1*r);
#else
			return /*Quaternion.Normalize*/(new Quaternion(t0*q.x_ + t1*r.x_, t0*q.y_ + t1*r.y_, t0*q.z_ + t1*r.z_,
				t0*q.w_ + t1*r.w_));
#endif
		}

		public static Quaternion Slerp(Quaternion q, Quaternion r, float t)
		{
			float cosTheta = Dot(q, r);

			float signOfT1 = 1f;
			if (/*shortestArc &&*/ (cosTheta < 0f))
			{
				// If r is on the oposite hemisphere use -r instead of r
				cosTheta = -cosTheta;
				signOfT1 = -1f;
			}

			float t0, t1;
			if ((1f - cosTheta) < 1e-6f)
			{
				// If q is nearly the same as r we just linearly interpolate
				t0 = 1f - t;
				t1 = t;
			}
			else
			{
				cosTheta = Math.Clamp(cosTheta, -1f, 1f);
				float theta = (float)Math.Acos(cosTheta);
				//if (Math.Abs(theta) < 1e-6f) // if theta == 0 then return q
				//	return q;
				float iSinTheta = 1f/MathF.Sqrt(1f - cosTheta*cosTheta); // instead of 1/sin(theta)
				//if (Math.Abs(sinTheta) < 1e-6f) // if theta*2 = 180 degrees then result is undefined
				//	return q*0.5f + r*0.5f;
				float tTheta = t*theta;
				t0 = (float)Math.Sin(theta - tTheta) * iSinTheta;
				t1 = (float)Math.Sin(tTheta) * iSinTheta;
			}

			t1 *= signOfT1;
#if SIMD
			return /*Quaternion.Normalize*/(t0*q + t1*r);
#else
			return /*Quaternion.Normalize*/(new Quaternion(t0*q.x_ + t1*r.x_, t0*q.y_ + t1*r.y_, t0*q.z_ + t1*r.z_,
				t0*q.w_ + t1*r.w_));
#endif
		}

		public static Quaternion Arc(Vector3 u, Vector3 v)
		{
			Vector3 v1 = Vector3.Normalize(u);
			Vector3 v2 = Vector3.Normalize(v);
			Vector3 h = Vector3.Normalize(v1 + v2);
			return new Quaternion(Vector3.Cross(v1, h), Vector3.Dot(v1, h));
		}

		public void Concat(Quaternion q)
		{
			this = q*this;
		}

		public void PreConcat(Quaternion q)
		{
			this = this*q;
		}

		public void Rotate(Quaternion r)
		{
			this = r*this*Conjugate(r);
		}

		public void Invert()
		{
			this = Conjugate(this)/Norm;
		}

		private static readonly int[] safe_ = { 0, 1, 2, 0 };
		private static readonly int[] next_ = { 1, 2, 0, 1 };
	}
}
