/*
 *  Name: Matrix2
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
#if SIMD
	[StructLayout(LayoutKind.Explicit, Size = 32)]
#endif
	[Serializable]
	[TypeConverter(typeof(Matrix2Converter))]
	public struct Matrix2 : ISerializable, IFormattable, IEquatable<Matrix2>
	{
#if SIMD
		public static readonly Matrix2 Zero = new Matrix2(System.Numerics.Vector2.Zero, System.Numerics.Vector2.Zero);
		public static readonly Matrix2 Identity = new Matrix2(System.Numerics.Vector2.UnitX, System.Numerics.Vector2.UnitY);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix2(float m00, float m01, float m10, float m11)
		{
			row0_ = new System.Numerics.Vector2(m00, m01);
			row1_ = new System.Numerics.Vector2(m10, m11);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix2(Vector2 row0, Vector2 row1)
		{
			row0_ = row0.xy_;
			row1_ = row1.xy_;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Matrix2(System.Numerics.Vector2 row0, System.Numerics.Vector2 row1)
		{
			row0_ = row0;
			row1_ = row1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix2(float[] m)
		{
			row0_ = new System.Numerics.Vector2(m[0], m[1]);
			row1_ = new System.Numerics.Vector2(m[2], m[3]);
		}

		private Matrix2(SerializationInfo info, StreamingContext context)
		{
			row0_ = new System.Numerics.Vector2(info.GetSingle("M00"), info.GetSingle("M01"));
			row1_ = new System.Numerics.Vector2(info.GetSingle("M10"), info.GetSingle("M11"));
		}

		[Browsable(false)]
		public float M00
		{
			readonly get => row0_.X;
			set => row0_.X = value;
		}

		[Browsable(false)]
		public float M01
		{
			readonly get => row0_.Y;
			set => row0_.Y = value;
		}

		[Browsable(false)]
		public float M10
		{
			readonly get => row1_.X;
			set => row1_.X = value;
		}

		[Browsable(false)]
		public float M11
		{
			readonly get => row1_.Y;
			set => row1_.Y = value;
		}

		public Vector2 Row0
		{
			readonly get => new Vector2(row0_);
			set => row0_ = value.xy_;
		}

		public Vector2 Row1
		{
			readonly get => new Vector2(row1_);
			set => row1_ = value.xy_;
		}

		[Browsable(false)]
		public readonly Vector2 Column0 => new Vector2(column0_);

		[Browsable(false)]
		public readonly Vector2 Column1 => new Vector2(column1_);

		public readonly override bool Equals(object other)
		{
			if (other is Matrix2 rhs)
				return (row0_ == rhs.row0_) && (row1_ == rhs.row1_);

			return false;
		}

		public readonly bool Equals(Matrix2 other)
		{
			return (row0_ == other.row0_) && (row1_ == other.row1_);
		}

		public Vector2 this[int row]
		{
			readonly get
			{
				switch (row)
				{
					case 0:
						return new Vector2(row0_);
					case 1:
						return new Vector2(row1_);
					default:
						throw new IndexOutOfRangeException();
				}
			}

			set
			{
				switch (row)
				{
					case 0:
						row0_ = value.xy_;
						break;
					case 1:
						row1_ = value.xy_;
						break;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		public float this[int row, int column] // #TODO Use System.Numerics.Vector2 indexing operator
		{
			readonly get
			{
				switch (row)
				{
					case 0:
						switch (column)
						{
							case 0:
								return row0_.X;
							case 1:
								return row0_.Y;
							default:
								throw new IndexOutOfRangeException();
						}

					case 1:
						switch (column)
						{
							case 0:
								return row1_.X;
							case 1:
								return row1_.Y;
							default:
								throw new IndexOutOfRangeException();
						}

					default:
						throw new IndexOutOfRangeException();
				}
			}

			set
			{
				switch (row)
				{
					case 0:
						switch (column)
						{
							case 0:
								row0_.X = value;
								break;
							case 1:
								row0_.Y = value;
								break;
							default:
								throw new IndexOutOfRangeException();
						}
						break;

					case 1:
						switch (column)
						{
							case 0:
								row1_.X = value;
								break;
							case 1:
								row1_.Y = value;
								break;
							default:
								throw new IndexOutOfRangeException();
						}
						break;

					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		public static bool operator ==(Matrix2 lhs, Matrix2 rhs)
		{
			return (lhs.row0_ == rhs.row0_) && (lhs.row1_ == rhs.row1_);
		}

		public static bool operator !=(Matrix2 lhs, Matrix2 rhs)
		{
			return (lhs.row0_ != rhs.row0_) || (lhs.row1_ != rhs.row1_);
		}

		public static Matrix2 operator +(Matrix2 m)
		{
			return m;
		}

		public static Matrix2 operator -(Matrix2 m)
		{
			return new Matrix2(-m.row0_, -m.row1_);
		}

		public static Matrix2 operator +(Matrix2 m, Matrix2 n)
		{
			return new Matrix2(m.row0_ + n.row0_, m.row1_ + n.row1_);
		}

		public static Matrix2 operator -(Matrix2 m, Matrix2 n)
		{
			return new Matrix2(m.row0_ - n.row0_, m.row1_ - n.row1_);
		}

		public static Matrix2 operator *(Matrix2 m, float f)
		{
			return new Matrix2(m.row0_*f, m.row1_*f);
		}

		public static Matrix2 operator *(float f, Matrix2 m)
		{
			return new Matrix2(f*m.row0_, f*m.row1_);
		}

		public static Matrix2 operator *(Matrix2 m, Matrix2 n)
		{
			return new Matrix2(m.m00_*n.row0_ + m.m01_*n.row1_,
				m.m10_*n.row0_ + m.m11_*n.row1_);
		}

		public static Matrix2 Concat(in Matrix2 m, in Matrix2 n)
		{
			return new Matrix2(m.m00_*n.row0_ + m.m01_*n.row1_,
				m.m10_*n.row0_ + m.m11_*n.row1_);
		}

		public void PreScale(Vector2 v)
		{
			row0_ *= v.xy_.X;
			row1_ *= v.xy_.Y;
		}

		public void Scale(Vector2 v)
		{
			row0_ *= v.xy_;
			row1_ *= v.xy_;
		}

		public static Matrix2 Transpose(in Matrix2 m)
		{
			return new Matrix2(m.column0_, m.column1_);
		}

		public void Transpose()
		{
			System.Numerics.Vector2 column0 = column0_;
			System.Numerics.Vector2 column1 = column1_;
			row0_ = column0;
			row1_ = column1;
		}
#else
		public static readonly Matrix2 Zero = new Matrix2(0f, 0f, 0f, 0f);
		public static readonly Matrix2 Identity = new Matrix2(1f, 0f, 0f, 1f);

		public Matrix2(float m00, float m01, float m10, float m11)
		{
			m00_ = m00; m01_ = m01;
			m10_ = m10; m11_ = m11;
		}

		public Matrix2(Vector2 row0, Vector2 row1)
		{
			m00_ = row0.x_; m01_ = row0.y_;
			m10_ = row1.x_; m11_ = row1.y_;
		}

		public Matrix2(float[] m)
		{
			m00_ = m[0]; m01_ = m[1];
			m10_ = m[2]; m11_ = m[3];
		}

		private Matrix2(SerializationInfo info, StreamingContext context)
		{
			m00_ = info.GetSingle("M00"); m01_ = info.GetSingle("M01");
			m10_ = info.GetSingle("M10"); m11_ = info.GetSingle("M11");
		}

		[Browsable(false)]
		public float M00
		{
			readonly get => m00_;
			set => m00_ = value;
		}

		[Browsable(false)]
		public float M01
		{
			readonly get => m01_;
			set => m01_ = value;
		}

		[Browsable(false)]
		public float M10
		{
			readonly get => m10_;
			set => m10_ = value;
		}

		[Browsable(false)]
		public float M11
		{
			readonly get => m11_;
			set => m11_ = value;
		}

		public Vector2 Row0
		{
			readonly get => new Vector2(m00_, m01_);
			set
			{ 
				m00_ = value.x_; 
				m01_ = value.y_; 
			}
		}

		public Vector2 Row1
		{
			readonly get => new Vector2(m10_, m11_);
			set 
			{ 
				m10_ = value.x_; 
				m11_ = value.y_; 
			}
		}

		[Browsable(false)]
		public readonly Vector2 Column0 => new Vector2(m00_, m10_);

		[Browsable(false)]
		public readonly Vector2 Column1 => new Vector2(m01_, m11_);

		public readonly override bool Equals(object other)
		{
			if (other is Matrix2 rhs)
			{
				return (m00_ == rhs.m00_) && (m01_ == rhs.m01_) &&
					(m10_ == rhs.m10_) && (m11_ == rhs.m11_);
			}

			return false;
		}

		public readonly bool Equals(Matrix2 other)
		{
			return (m00_ == other.m00_) && (m01_ == other.m01_) &&
				(m10_ == other.m10_) && (m11_ == other.m11_);
		}

		public Vector2 this[int row]
		{
			readonly get
			{
				switch (row)
				{
					case 0: 
						return new Vector2(m00_, m01_);
					case 1: 
						return new Vector2(m10_, m11_);
					default: 
						throw new IndexOutOfRangeException();
				}
			}

			set
			{
				switch (row)
				{
					case 0: 
						m00_ = value.x_; 
						m01_ = value.y_; 
						break;
					case 1: 
						m10_ = value.x_; 
						m11_ = value.y_; 
						break;
					default: 
						throw new IndexOutOfRangeException();
				}
			}
		}

		public float this[int row, int column]
		{
			readonly get
			{
				switch (row)
				{
					case 0:
						switch (column)
						{
							case 0: 
								return m00_;
							case 1: 
								return m01_;
							default: 
								throw new IndexOutOfRangeException();
						}

					case 1:
						switch (column)
						{
							case 0: 
								return m10_;
							case 1: 
								return m11_;
							default: 
								throw new IndexOutOfRangeException();
						}

					default: 
						throw new IndexOutOfRangeException();
				}
			}

			set
			{
				switch (row)
				{
					case 0:
						switch (column)
						{
							case 0: 
								m00_ = value; 
								break;
							case 1: 
								m01_ = value; 
								break;
							default: 
								throw new IndexOutOfRangeException();
						}
						break;

					case 1:
						switch (column)
						{
							case 0: 
								m10_ = value; 
								break;
							case 1: 
								m11_ = value; 
								break;
							default: 
								throw new IndexOutOfRangeException();
						}
						break;

					default: 
						throw new IndexOutOfRangeException();
				}
			}
		}

		public static bool operator ==(Matrix2 lhs, Matrix2 rhs)
		{
			return (lhs.m00_ == rhs.m00_) && (lhs.m01_ == rhs.m01_) &&
				(lhs.m10_ == rhs.m10_) && (lhs.m11_ == rhs.m11_);
		}

		public static bool operator !=(Matrix2 lhs, Matrix2 rhs)
		{
			return (lhs.m00_ != rhs.m00_) || (lhs.m01_ != rhs.m01_) ||
				(lhs.m10_ != rhs.m10_) || (lhs.m11_ != rhs.m11_);
		}

		public static Matrix2 operator +(Matrix2 m)
		{
			return m;
		}

		public static Matrix2 operator -(Matrix2 m)
		{
			return new Matrix2(-m.m00_, -m.m01_,
				-m.m10_, -m.m11_);
		}

		public static Matrix2 operator +(Matrix2 m, Matrix2 n)
		{
			return new Matrix2(m.m00_ + n.m00_, m.m01_ + n.m01_,
				m.m10_ + n.m10_, m.m11_ + n.m11_);
		}

		public static Matrix2 operator -(Matrix2 m, Matrix2 n)
		{
			return new Matrix2(m.m00_ - n.m00_, m.m01_ - n.m01_,
				m.m10_ - n.m10_, m.m11_ - n.m11_);
		}

		public static Matrix2 operator *(Matrix2 m, float f)
		{
			return new Matrix2(m.m00_*f, m.m01_*f,
				m.m10_*f, m.m11_*f);
		}

		public static Matrix2 operator *(float f, Matrix2 m)
		{
			return new Matrix2(f*m.m00_, f*m.m01_,
				f*m.m10_, f*m.m11_);
		}

		public static Matrix2 operator *(Matrix2 m, Matrix2 n)
		{
			return new Matrix2(m.m00_*n.m00_ + m.m01_*n.m10_,
				m.m00_*n.m01_ + m.m01_*n.m11_,
				m.m10_*n.m00_ + m.m11_*n.m10_,
				m.m10_*n.m01_ + m.m11_*n.m11_);
		}

		public static Matrix2 Concat(in Matrix2 m, in Matrix2 n)
		{
			return new Matrix2(m.m00_*n.m00_ + m.m01_*n.m10_,
				m.m00_*n.m01_ + m.m01_*n.m11_,
				m.m10_*n.m00_ + m.m11_*n.m10_,
				m.m10_*n.m01_ + m.m11_*n.m11_);
		}

		public void PreScale(Vector2 v)
		{
			m00_ *= v.x_; m01_ *= v.x_;
			m10_ *= v.y_; m11_ *= v.y_;
		}

		public void Scale(Vector2 v)
		{
			m00_ *= v.x_; m01_ *= v.y_;
			m10_ *= v.x_; m11_ *= v.y_;
		}

		public static Matrix2 Transpose(in Matrix2 m)
		{
			return new Matrix2(m.m00_, m.m10_, m.m01_, m.m11_);
		}

		public void Transpose()
		{
			float t = m01_; 
			m01_ = m10_; 
			m10_ = t;
		}
#endif

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("M00", m00_); info.AddValue("M01", m01_);
			info.AddValue("M10", m10_); info.AddValue("M11", m11_);
		}

		public readonly override int GetHashCode()
		{
			int hash = m00_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m01_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m10_.GetHashCode();
			return ((hash << 5) + hash) ^ m11_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Matrix2 m, float tolerance)
		{
			return (Math.Abs(m.m00_ - m00_) < tolerance) &&
				(Math.Abs(m.m01_ - m01_) < tolerance) &&
				(Math.Abs(m.m10_ - m10_) < tolerance) &&
				(Math.Abs(m.m11_ - m11_) < tolerance);
		}

		public readonly bool ApproxEquals(in Matrix2 m)
		{
			return ApproxEquals(m, 1e-6f);
		}

		public readonly float[] ToArray()
		{
			return new float[4] { m00_, m01_, m10_, m11_ };
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1} {2} {3}", m00_, m01_, m10_, m11_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3}", m00_, m01_, m10_, m11_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1} {2} {3}", 
				m00_.ToString(format), m01_.ToString(format), m10_.ToString(format), m11_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3}",
				m00_.ToString(format, provider), m01_.ToString(format, provider), m10_.ToString(format, provider), m11_.ToString(format, provider));
		}

		public static Matrix2 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Matrix2(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]), Single.Parse(m[3]));
		}

		public static Matrix2 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Matrix2(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
				Single.Parse(m[2], provider), Single.Parse(m[3], provider));
		}

		public static Matrix2 Concat(in Matrix2 s, in Matrix2 t, in Matrix2 u)
		{
			return Concat(Concat(s, t), u);
		}

		public static Matrix2 Concat(in Matrix2 s, in Matrix2 t, in Matrix2 u, in Matrix2 v)
		{
			return Concat(Concat(Concat(s, t), u), v);
		}

		public static Matrix2 Concat(in Matrix2 s, in Matrix2 t, in Matrix2 u, in Matrix2 v, in Matrix2 w)
		{
			return Concat(Concat(Concat(Concat(s, t), u), v), w);
		}

		public static Matrix2 Concat(in Matrix2 s, in Matrix2 t, in Matrix2 u, in Matrix2 v, in Matrix2 w, in Matrix2 x)
		{
			return Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x);
		}

		public static Matrix2 Concat(in Matrix2 s, in Matrix2 t, in Matrix2 u, in Matrix2 v, in Matrix2 w, in Matrix2 x, in Matrix2 y)
		{
			return Concat(Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x), y);
		}

		public static Matrix2 Concat(in Matrix2 s, in Matrix2 t, in Matrix2 u, in Matrix2 v, in Matrix2 w, in Matrix2 x, in Matrix2 y, in Matrix2 z)
		{
			return Concat(Concat(Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x), y), z);
		}

		public void Concat(in Matrix2 m)
		{
			this = Concat(this, m);
		}

		public void PreConcat(in Matrix2 m)
		{
			this = Concat(m, this);
		}

		public static Matrix2 Rotation(float angle)
		{
			if (angle != 0f)
			{
				float sine = (float)Math.Sin(angle);
				float cosine = (float)Math.Cos(angle);
				return new Matrix2(cosine, sine, -sine, cosine);
			}

			return Identity;
		}

		public static Matrix2 Scaling(Vector2 v)
		{
			return new Matrix2(v.x_, 0f, 0f, v.y_);
		}

		public static Matrix2 Scaling(float s)
		{
			return new Matrix2(s, 0f, 0f, s);
		}

		public static Matrix2 ScalingRotation(Vector2 s, float angle)
		{
			Matrix2 r = Matrix2.Rotation(angle);
			r.PreScale(s);
			return r;
		}

		public static Matrix2 Shearing(float xy, float yx)
		{
			return new Matrix2(1f, xy, yx, 1f);
		}

		public static Matrix2 Adjoint(in Matrix2 m)
		{
			return new Matrix2(m.m11_, -m.m10_, -m.m01_, m.m00_);
		}

		public static Matrix2 Inverse(in Matrix2 m)
		{
			float a = m.m00_, b = m.m01_;
			float c = m.m10_, d = m.m11_;
			float n = 1f/(a*d - b*c);
#if SIMD
			return new Matrix2(new System.Numerics.Vector2(d, -b)*n, new System.Numerics.Vector2(-c, a)*n);
#else
			return new Matrix2(d*n, -b*n, -c*n, a*n);
#endif
		}

		public static Matrix2 InverseTranspose(in Matrix2 m)
		{
			float a = m.m00_, b = m.m01_;
			float c = m.m10_, d = m.m11_;
			float n = 1f/(a*d - b*c);
#if SIMD
			return new Matrix2(new System.Numerics.Vector2(d, -b)*n, new System.Numerics.Vector2(-c, a)*n);
#else
			return new Matrix2(d*n, -c*n, -b*n, a*n);
#endif
		}

		/// <summary>
		/// Stabilized Gram-Schmidt orthonormalization.
		/// </summary>
		public static Matrix2 Orthonormalize(in Matrix2 m)
		{
			Vector2 v1 = m.Row0;
			Vector2 v2 = m.Row1;
			v2 -= Vector2.Project(v2, v1);
			return new Matrix2(Vector2.Normalize(v1), Vector2.Normalize(v2));
		}

		public void Invert()
		{
			float a = m00_, b = m01_;
			float c = m10_, d = m11_;
			float n = 1f/(a*d - b*c);
#if SIMD
			row0_ = new System.Numerics.Vector2(d, -b)*n;
			row1_ = new System.Numerics.Vector2(-c, a)*n;
#else
			m00_ = d*n; m01_ = -b*n;
			m10_ = -c*n; m11_ = a*n;
#endif
		}

		/// <summary>
		/// Stabilized Gram-Schmidt orthonormalization.
		/// </summary>
		public void Orthonormalize()
		{
			Vector2 v1 = Row0;
			Vector2 v2 = Row1;
			v2 -= Vector2.Project(v2, v1);
			Row0 = Vector2.Normalize(v1);
			Row1 = Vector2.Normalize(v2);
		}

		[Browsable(false)]
		public readonly bool IsFinite
		{
			get => Scalar.IsFinite(m00_) && Scalar.IsFinite(m01_) && Scalar.IsFinite(m10_) && Scalar.IsFinite(m11_);
		}

		[Browsable(false)]
		public readonly float Trace => m00_ + m11_;

		[Browsable(false)]
		public readonly float Determinant => m00_*m11_ - m01_*m10_;

		[Browsable(false)]
		public readonly Vector2 ScaleComponent => new Vector2(Row0.Magnitude, Row1.Magnitude);

#if SIMD
		internal readonly float m00_ => row0_.X;
		internal readonly float m01_ => row0_.Y;
		internal readonly float m10_ => row1_.X;
		internal readonly float m11_ => row1_.Y;
		internal readonly System.Numerics.Vector2 column0_ => new System.Numerics.Vector2(row0_.X, row1_.X);
		internal readonly System.Numerics.Vector2 column1_ => new System.Numerics.Vector2(row0_.Y, row1_.Y);

		[FieldOffset(0)] internal System.Numerics.Vector2 row0_;
		[FieldOffset(16)] internal System.Numerics.Vector2 row1_;
#else
		internal float m00_, m01_;
		internal float m10_, m11_;
#endif
	}
}
