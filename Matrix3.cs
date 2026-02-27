/*
 *  Name: Matrix3
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
	[StructLayout(LayoutKind.Explicit, Size = 48)]
#endif
	[Serializable]
	[TypeConverter(typeof(Matrix3Converter))]
	public struct Matrix3 : ISerializable, IFormattable, IEquatable<Matrix3>
	{
#if SIMD
		public static readonly Matrix3 Zero = new Matrix3(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero,
			System.Numerics.Vector3.Zero);
		public static readonly Matrix3 Identity = new Matrix3(System.Numerics.Vector3.UnitX, System.Numerics.Vector3.UnitY,
			System.Numerics.Vector3.UnitZ);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
		{
			row0_ = new System.Numerics.Vector3(m00, m01, m02);
			row1_ = new System.Numerics.Vector3(m10, m11, m12);
			row2_ = new System.Numerics.Vector3(m20, m21, m22);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix3(Vector3 row0, Vector3 row1, Vector3 row2)
		{
			row0_ = row0.xyz_;
			row1_ = row1.xyz_;
			row2_ = row2.xyz_;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Matrix3(System.Numerics.Vector3 row0, System.Numerics.Vector3 row1, System.Numerics.Vector3 row2)
		{
			row0_ = row0;
			row1_ = row1;
			row2_ = row2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix3(float[] m)
		{
			row0_ = new System.Numerics.Vector3(m[0], m[1], m[2]);
			row1_ = new System.Numerics.Vector3(m[3], m[4], m[5]);
			row2_ = new System.Numerics.Vector3(m[6], m[7], m[8]);
		}

		private Matrix3(SerializationInfo info, StreamingContext context)
		{
			row0_ = new System.Numerics.Vector3(info.GetSingle("M00"), info.GetSingle("M01"), info.GetSingle("M02"));
			row1_ = new System.Numerics.Vector3(info.GetSingle("M10"), info.GetSingle("M11"), info.GetSingle("M12"));
			row2_ = new System.Numerics.Vector3(info.GetSingle("M20"), info.GetSingle("M21"), info.GetSingle("M22"));
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
		public float M02
		{
			readonly get => row0_.Z;
			set => row0_.Z = value;
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

		[Browsable(false)]
		public float M12
		{
			readonly get => row1_.Z;
			set => row1_.Z = value;
		}

		[Browsable(false)]
		public float M20
		{
			readonly get => row2_.X;
			set => row2_.X = value;
		}

		[Browsable(false)]
		public float M21
		{
			readonly get => row2_.Y;
			set => row2_.Y = value;
		}

		[Browsable(false)]
		public float M22
		{
			readonly get => row2_.Z;
			set => row2_.Z = value;
		}

		[Browsable(false)]
		public Vector3 Row0
		{
			readonly get => new Vector3(row0_);
			set => row0_ = value.xyz_;
		}

		[Browsable(false)]
		public Vector3 Row1
		{
			readonly get => new Vector3(row1_);
			set => row1_ = value.xyz_;
		}

		[Browsable(false)]
		public Vector3 Row2
		{
			readonly get => new Vector3(row2_);
			set => row2_ = value.xyz_;
		}

		[Browsable(false)]
		public readonly Vector3 Column0 => new Vector3(column0_);

		[Browsable(false)]
		public readonly Vector3 Column1 => new Vector3(column1_);

		[Browsable(false)]
		public readonly Vector3 Column2 => new Vector3(column2_);

		public Vector3 Right
		{
			readonly get => new Vector3(row0_);
			set => row0_ = value.xyz_;
		}

		public Vector3 Up
		{
			readonly get => new Vector3(row1_);
			set => row1_ = value.xyz_;
		}

		public Vector3 Forward
		{
			readonly get => new Vector3(row2_);
			set => row2_ = value.xyz_;
		}

		public readonly override bool Equals(object other)
		{
			if (other is Matrix3 rhs)
				return (row0_ == rhs.row0_) && (row1_ == rhs.row1_) && (row2_ == rhs.row2_);

			return false;
		}

		public readonly bool Equals(Matrix3 other)
		{
			return (row0_ == other.row0_) && (row1_ == other.row1_) && (row2_ == other.row2_);
		}

		public Vector3 this[int row]
		{
			readonly get
			{
				switch (row)
				{
					case 0:
						return new Vector3(row0_);
					case 1:
						return new Vector3(row1_);
					case 2:
						return new Vector3(row2_);
					default:
						throw new IndexOutOfRangeException();
				}
			}

			set
			{
				switch (row)
				{
					case 0:
						row0_ = value.xyz_;
						break;
					case 1:
						row1_ = value.xyz_;
						break;
					case 2:
						row2_ = value.xyz_;
						break;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		public float this[int row, int column] // #TODO Use System.Numerics.Vector3 indexing operator
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
							case 2:
								return row0_.Z;
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
							case 2:
								return row1_.Z;
							default:
								throw new IndexOutOfRangeException();
						}

					case 2:
						switch (column)
						{
							case 0:
								return row2_.X;
							case 1:
								return row2_.Y;
							case 2:
								return row2_.Z;
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
							case 2:
								row0_.Z = value;
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
							case 2:
								row1_.Z = value;
								break;
							default:
								throw new IndexOutOfRangeException();
						}
						break;

					case 2:
						switch (column)
						{
							case 0:
								row2_.X = value;
								break;
							case 1:
								row2_.Y = value;
								break;
							case 2:
								row2_.Z = value;
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

		public static implicit operator Matrix3(in Matrix2 m)
		{
			return new Matrix3(new System.Numerics.Vector3(m.row0_, 0f), new System.Numerics.Vector3(m.row1_, 0f),
				System.Numerics.Vector3.UnitZ);
		}

		public static bool operator ==(Matrix3 lhs, Matrix3 rhs)
		{
			return (lhs.row0_ == rhs.row0_) && (lhs.row1_ == rhs.row1_) && (lhs.row2_ == rhs.row2_);
		}

		public static bool operator !=(Matrix3 lhs, Matrix3 rhs)
		{
			return (lhs.row0_ != rhs.row0_) || (lhs.row1_ != rhs.row1_) || (lhs.row2_ != rhs.row2_);
		}

		public static Matrix3 operator +(Matrix3 m)
		{
			return m;
		}

		public static Matrix3 operator -(Matrix3 m)
		{
			return new Matrix3(-m.row0_, -m.row1_, -m.row2_);
		}

		public static Matrix3 operator +(Matrix3 m, Matrix3 n)
		{
			return new Matrix3(m.row0_ + n.row0_, m.row1_ + n.row1_, m.row2_ + n.row2_);
		}

		public static Matrix3 operator -(Matrix3 m, Matrix3 n)
		{
			return new Matrix3(m.row0_ - n.row0_, m.row1_ - n.row1_, m.row2_ - n.row2_);
		}

		public static Matrix3 operator *(Matrix3 m, float f)
		{
			return new Matrix3(m.row0_*f, m.row1_*f, m.row2_*f);
		}

		public static Matrix3 operator *(float f, Matrix3 m)
		{
			return new Matrix3(f*m.row0_, f*m.row1_, f*m.row2_);
		}

		public static Matrix3 operator *(Matrix3 m, Matrix3 n)
		{
			return new Matrix3(m.m00_*n.row0_ + m.m01_*n.row1_ + m.m02_*n.row2_,
				m.m10_*n.row0_ + m.m11_*n.row1_ + m.m12_*n.row2_,
				m.m20_*n.row0_ + m.m21_*n.row1_ + m.m22_*n.row2_);
		}

		public static Matrix3 Concat(in Matrix3 m, in Matrix3 n)
		{
			return new Matrix3(m.m00_*n.row0_ + m.m01_*n.row1_ + m.m02_*n.row2_,
				m.m10_*n.row0_ + m.m11_*n.row1_ + m.m12_*n.row2_,
				m.m20_*n.row0_ + m.m21_*n.row1_ + m.m22_*n.row2_);
		}

		public void PreScale(Vector3 v)
		{
			row0_ *= v.xyz_.X;
			row1_ *= v.xyz_.Y;
			row2_ *= v.xyz_.Z;
		}

		public void Scale(Vector3 v)
		{
			row0_ *= v.xyz_;
			row1_ *= v.xyz_;
			row2_ *= v.xyz_;
		}

		public static Matrix3 Transpose(in Matrix3 m)
		{
			return new Matrix3(m.column0_, m.column1_, m.column2_);
		}

		public void Transpose()
		{
			System.Numerics.Vector3 column0 = column0_;
			System.Numerics.Vector3 column1 = column1_;
			System.Numerics.Vector3 column2 = column2_;
			row0_ = column0;
			row1_ = column1;
			row2_ = column2;
		}
#else
		public static readonly Matrix3 Zero = new Matrix3(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		public static readonly Matrix3 Identity = new Matrix3(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

		public Matrix3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
		{
			m00_ = m00; m01_ = m01; m02_ = m02;
			m10_ = m10; m11_ = m11; m12_ = m12;
			m20_ = m20; m21_ = m21; m22_ = m22;
		}

		public Matrix3(Vector3 row0, Vector3 row1, Vector3 row2)
		{
			m00_ = row0.x_; m01_ = row0.y_; m02_ = row0.z_;
			m10_ = row1.x_; m11_ = row1.y_; m12_ = row1.z_;
			m20_ = row2.x_; m21_ = row2.y_; m22_ = row2.z_;
		}

		public Matrix3(float[] m)
		{
			m00_ = m[0]; m01_ = m[1]; m02_ = m[2];
			m10_ = m[3]; m11_ = m[4]; m12_ = m[5];
			m20_ = m[6]; m21_ = m[7]; m22_ = m[8];
		}

		private Matrix3(SerializationInfo info, StreamingContext context)
		{
			m00_ = info.GetSingle("M00"); m01_ = info.GetSingle("M01"); m02_ = info.GetSingle("M02");
			m10_ = info.GetSingle("M10"); m11_ = info.GetSingle("M11"); m12_ = info.GetSingle("M12");
			m20_ = info.GetSingle("M20"); m21_ = info.GetSingle("M21"); m22_ = info.GetSingle("M22");
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
		public float M02
		{
			readonly get => m02_;
			set => m02_ = value;
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

		[Browsable(false)]
		public float M12
		{
			readonly get => m12_;
			set => m12_ = value;
		}

		[Browsable(false)]
		public float M20
		{
			readonly get => m20_;
			set => m20_ = value;
		}

		[Browsable(false)]
		public float M21
		{
			readonly get => m21_;
			set => m21_ = value;
		}

		[Browsable(false)]
		public float M22
		{
			readonly get => m22_;
			set => m22_ = value;
		}

		[Browsable(false)]
		public Vector3 Row0
		{
			readonly get => new Vector3(m00_, m01_, m02_);
			set 
			{ 
				m00_ = value.x_; 
				m01_ = value.y_; 
				m02_ = value.z_; 
			}
		}

		[Browsable(false)]
		public Vector3 Row1
		{
			readonly get => new Vector3(m10_, m11_, m12_);
			set 
			{ 
				m10_ = value.x_; 
				m11_ = value.y_; 
				m12_ = value.z_; 
			}
		}

		[Browsable(false)]
		public Vector3 Row2
		{
			readonly get => new Vector3(m20_, m21_, m22_);
			set 
			{ 
				m20_ = value.x_; 
				m21_ = value.y_; 
				m22_ = value.z_; 
			}
		}

		[Browsable(false)]
		public readonly Vector3 Column0 => new Vector3(m00_, m10_, m20_);

		[Browsable(false)]
		public readonly Vector3 Column1 => new Vector3(m01_, m11_, m21_);

		[Browsable(false)]
		public readonly Vector3 Column2 => new Vector3(m02_, m12_, m22_);

		public Vector3 Right
		{
			readonly get => new Vector3(m00_, m01_, m02_);
			set { m00_ = value.x_; m01_ = value.y_; m02_ = value.z_; }
		}

		public Vector3 Up
		{
			readonly get => new Vector3(m10_, m11_, m12_);
			set { m10_ = value.x_; m11_ = value.y_; m12_ = value.z_; }
		}

		public Vector3 Forward
		{
			readonly get => new Vector3(m20_, m21_, m22_);
			set { m20_ = value.x_; m21_ = value.y_; m22_ = value.z_; }
		}

		public readonly override bool Equals(object other)
		{
			if (other is Matrix3 rhs)
			{
				return (m00_ == rhs.m00_) && (m01_ == rhs.m01_) && (m02_ == rhs.m02_) &&
					(m10_ == rhs.m10_) && (m11_ == rhs.m11_) && (m12_ == rhs.m12_) &&
					(m20_ == rhs.m20_) && (m21_ == rhs.m21_) && (m22_ == rhs.m22_);
			}
			
			return false;
		}

		public readonly bool Equals(Matrix3 other)
		{
			return (m00_ == other.m00_) && (m01_ == other.m01_) && (m02_ == other.m02_) &&
				(m10_ == other.m10_) && (m11_ == other.m11_) && (m12_ == other.m12_) &&
				(m20_ == other.m20_) && (m21_ == other.m21_) && (m22_ == other.m22_);
		}

		public Vector3 this[int row]
		{
			readonly get
			{
				switch (row)
				{
					case 0: 
						return new Vector3(m00_, m01_, m02_);
					case 1: 
						return new Vector3(m10_, m11_, m12_);
					case 2: 
						return new Vector3(m20_, m21_, m22_);
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
						m02_ = value.z_; 
						break;
					case 1: 
						m10_ = value.x_; 
						m11_ = value.y_; 
						m12_ = value.z_; 
						break;
					case 2: 
						m20_ = value.x_; 
						m21_ = value.y_; 
						m22_ = value.z_; 
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
							case 2: 
								return m02_;
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
							case 2: 
								return m12_;
							default: 
								throw new IndexOutOfRangeException();
						}

					case 2:
						switch (column)
						{
							case 0: 
								return m20_;
							case 1: 
								return m21_;
							case 2: 
								return m22_;
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
							case 2: 
								m02_ = value; 
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
							case 2: 
								m12_ = value; 
								break;
							default: 
								throw new IndexOutOfRangeException();
						}
						break;

					case 2:
						switch (column)
						{
							case 0: 
								m20_ = value; 
								break;
							case 1: 
								m21_ = value; 
								break;
							case 2: 
								m22_ = value; 
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

		public static implicit operator Matrix3(in Matrix2 m)
		{
			return new Matrix3(m.m00_, m.m01_, 0f,
				m.m10_, m.m11_, 0f,
				0f, 0f, 1f);
		}

		public static bool operator ==(Matrix3 lhs, Matrix3 rhs)
		{
			return (lhs.m00_ == rhs.m00_) && (lhs.m01_ == rhs.m01_) && (lhs.m02_ == rhs.m02_) &&
				(lhs.m10_ == rhs.m10_) && (lhs.m11_ == rhs.m11_) && (lhs.m12_ == rhs.m12_) &&
				(lhs.m20_ == rhs.m20_) && (lhs.m21_ == rhs.m21_) && (lhs.m22_ == rhs.m22_);
		}

		public static bool operator !=(Matrix3 lhs, Matrix3 rhs)
		{
			return (lhs.m00_ != rhs.m00_) || (lhs.m01_ != rhs.m01_) || (lhs.m02_ != rhs.m02_) ||
				(lhs.m10_ != rhs.m10_) || (lhs.m11_ != rhs.m11_) || (lhs.m12_ != rhs.m12_) ||
				(lhs.m20_ != rhs.m20_) || (lhs.m21_ != rhs.m21_) || (lhs.m22_ != rhs.m22_);
		}

		public static Matrix3 operator +(Matrix3 m)
		{
			return m;
		}

		public static Matrix3 operator -(Matrix3 m)
		{
			return new Matrix3(-m.m00_, -m.m01_, -m.m02_,
				-m.m10_, -m.m11_, -m.m12_,
				-m.m20_, -m.m21_, -m.m22_);
		}

		public static Matrix3 operator +(Matrix3 m, Matrix3 n)
		{
			return new Matrix3(m.m00_ + n.m00_, m.m01_ + n.m01_, m.m02_ + n.m02_,
				m.m10_ + n.m10_, m.m11_ + n.m11_, m.m12_ + n.m12_,
				m.m20_ + n.m20_, m.m21_ + n.m21_, m.m22_ + n.m22_);
		}

		public static Matrix3 operator -(Matrix3 m, Matrix3 n)
		{
			return new Matrix3(m.m00_ - n.m00_, m.m01_ - n.m01_, m.m02_ - n.m02_,
				m.m10_ - n.m10_, m.m11_ - n.m11_, m.m12_ - n.m12_,
				m.m20_ - n.m20_, m.m21_ - n.m21_, m.m22_ - n.m22_);
		}

		public static Matrix3 operator *(Matrix3 m, float f)
		{
			return new Matrix3(m.m00_*f, m.m01_*f, m.m02_*f,
				m.m10_*f, m.m11_*f, m.m12_*f,
				m.m20_*f, m.m21_*f, m.m22_*f);
		}

		public static Matrix3 operator *(float f, Matrix3 m)
		{
			return new Matrix3(f*m.m00_, f*m.m01_, f*m.m02_,
				f*m.m10_, f*m.m11_, f*m.m12_,
				f*m.m20_, f*m.m21_, f*m.m22_);
		}

		public static Matrix3 operator *(Matrix3 m, Matrix3 n)
		{
			return new Matrix3(m.m00_*n.m00_ + m.m01_*n.m10_ + m.m02_*n.m20_,
				m.m00_*n.m01_ + m.m01_*n.m11_ + m.m02_*n.m21_,
				m.m00_*n.m02_ + m.m01_*n.m12_ + m.m02_*n.m22_,
				m.m10_*n.m00_ + m.m11_*n.m10_ + m.m12_*n.m20_,
				m.m10_*n.m01_ + m.m11_*n.m11_ + m.m12_*n.m21_,
				m.m10_*n.m02_ + m.m11_*n.m12_ + m.m12_*n.m22_,
				m.m20_*n.m00_ + m.m21_*n.m10_ + m.m22_*n.m20_,
				m.m20_*n.m01_ + m.m21_*n.m11_ + m.m22_*n.m21_,
				m.m20_*n.m02_ + m.m21_*n.m12_ + m.m22_*n.m22_);
		}

		public static Matrix3 Concat(in Matrix3 m, in Matrix3 n)
		{
			return new Matrix3(m.m00_*n.m00_ + m.m01_*n.m10_ + m.m02_*n.m20_,
				m.m00_*n.m01_ + m.m01_*n.m11_ + m.m02_*n.m21_,
				m.m00_*n.m02_ + m.m01_*n.m12_ + m.m02_*n.m22_,
				m.m10_*n.m00_ + m.m11_*n.m10_ + m.m12_*n.m20_,
				m.m10_*n.m01_ + m.m11_*n.m11_ + m.m12_*n.m21_,
				m.m10_*n.m02_ + m.m11_*n.m12_ + m.m12_*n.m22_,
				m.m20_*n.m00_ + m.m21_*n.m10_ + m.m22_*n.m20_,
				m.m20_*n.m01_ + m.m21_*n.m11_ + m.m22_*n.m21_,
				m.m20_*n.m02_ + m.m21_*n.m12_ + m.m22_*n.m22_);
		}

		public void PreScale(Vector3 v)
		{
			m00_ *= v.x_; m01_ *= v.x_; m02_ *= v.x_;
			m10_ *= v.y_; m11_ *= v.y_; m12_ *= v.y_;
			m20_ *= v.z_; m21_ *= v.z_; m22_ *= v.z_;
		}

		public void Scale(Vector3 v)
		{
			m00_ *= v.x_; m01_ *= v.y_; m02_ *= v.z_;
			m10_ *= v.x_; m11_ *= v.y_; m12_ *= v.z_;
			m20_ *= v.x_; m21_ *= v.y_; m22_ *= v.z_;
		}

		public static Matrix3 Transpose(in Matrix3 m)
		{
			return new Matrix3(m.m00_, m.m10_, m.m20_, m.m01_, m.m11_, m.m21_, m.m02_, m.m12_, m.m22_);
		}

		public void Transpose()
		{
			float t = m01_; m01_ = m10_; m10_ = t;
			t = m02_; m02_ = m20_; m20_ = t;
			t = m12_; m12_ = m21_; m21_ = t;
		}
#endif

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("M00", m00_); info.AddValue("M01", m01_); info.AddValue("M02", m02_);
			info.AddValue("M10", m10_); info.AddValue("M11", m11_); info.AddValue("M12", m12_);
			info.AddValue("M20", m20_); info.AddValue("M21", m21_); info.AddValue("M22", m22_);
		}

		public readonly override int GetHashCode()
		{
			int hash = m00_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m01_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m02_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m10_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m11_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m12_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m20_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m21_.GetHashCode();
			return ((hash << 5) + hash) ^ m22_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Matrix3 m, float tolerance)
		{
			return (Math.Abs(m.m00_ - m00_) < tolerance) &&
				(Math.Abs(m.m01_ - m01_) < tolerance) &&
				(Math.Abs(m.m02_ - m02_) < tolerance) &&
				(Math.Abs(m.m10_ - m10_) < tolerance) &&
				(Math.Abs(m.m11_ - m11_) < tolerance) &&
				(Math.Abs(m.m12_ - m12_) < tolerance) &&
				(Math.Abs(m.m20_ - m20_) < tolerance) &&
				(Math.Abs(m.m21_ - m21_) < tolerance) &&
				(Math.Abs(m.m22_ - m22_) < tolerance);
		}

		public readonly bool ApproxEquals(in Matrix3 m)
		{
			return ApproxEquals(m, 1e-6f);
		}

		public readonly float[] ToArray()
		{
			return new float[9] { m00_, m01_, m02_, m10_, m11_, m12_, m20_, m21_, m22_ };
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",
				m00_, m01_, m02_, m10_, m11_, m12_, m20_, m21_, m22_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider,
				"{0} {1} {2} {3} {4} {5} {6} {7} {8}",
				m00_, m01_, m02_, m10_, m11_, m12_, m20_, m21_, m22_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",
				m00_.ToString(format), m01_.ToString(format), m02_.ToString(format),
				m10_.ToString(format), m11_.ToString(format), m12_.ToString(format),
				m20_.ToString(format), m21_.ToString(format), m22_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3} {4} {5} {6} {7} {8}",
				m00_.ToString(format, provider), m01_.ToString(format, provider), m02_.ToString(format, provider),
				m10_.ToString(format, provider), m11_.ToString(format, provider), m12_.ToString(format, provider),
				m20_.ToString(format, provider), m21_.ToString(format, provider), m22_.ToString(format, provider));
		}

		public static Matrix3 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 9)
				throw new FormatException();

			return new Matrix3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]),
				Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5]),
				Single.Parse(m[6]), Single.Parse(m[7]), Single.Parse(m[8]));
		}

		public static Matrix3 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 9)
				throw new FormatException();

			return new Matrix3(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
				Single.Parse(m[2], provider), Single.Parse(m[3], provider),
				Single.Parse(m[4], provider), Single.Parse(m[5], provider),
				Single.Parse(m[6], provider), Single.Parse(m[7], provider),
				Single.Parse(m[8], provider));
		}

		public static explicit operator Matrix3(Quaternion q)
		{
			return FromQuaternion(q);
		}

		public static Matrix3 FromForward(Vector3 forward)
        {
            float m = forward.Magnitude;
            if (m > 0f)
            {
                forward /= m;
                if ((1f - Math.Abs(forward.y_)) >= 1e-6f)
                {
                    Vector3 right = new Vector3(forward.z_, 0f, -forward.x_);
                    right.Normalize();
                    Vector3 up = Vector3.Cross(forward, right);
                    return new Matrix3(right, up, forward);
                }
                else
                {
					Vector3 up = (forward.y_ > 0f) ? -Vector3.UnitZ : Vector3.UnitZ;
					Vector3 right = Vector3.Cross(up, forward);
					right.Normalize();
					up = Vector3.Cross(forward, right);
                    return new Matrix3(right, up, forward);
                }
            }
            else
            {
                return Matrix3.Identity;
            }
        }

		public static Matrix3 FromUp(Vector3 up)
		{
			//Matrix3 m = Matrix3.FromForward(up);
			//return new Matrix3(m[0], m[2], -m[1]);

			float m = up.Magnitude;
			if (m > 0f)
			{
				up /= m;
				if ((1f - Math.Abs(up.y_)) >= 1e-6f)
				{
					Vector3 right = new Vector3(up.z_, 0f, -up.x_);
					right.Normalize();
					Vector3 forward = Vector3.Cross(right, up);
					return new Matrix3(right, up, forward);
				}
				else
				{
					Vector3 forward = (up.y_ > 0f) ? Vector3.UnitZ : -Vector3.UnitZ;
					Vector3 right = Vector3.Cross(up, forward);
					right.Normalize();
					forward = Vector3.Cross(right, up);
					return new Matrix3(right, up, forward);
				}
			}
			else
			{
				return Matrix3.Identity;
			}
		}

		public static Matrix3 FromForwardAndUp(Vector3 forward, Vector3 up)
		{
			float m = forward.Magnitude;
			if ((m > 0f) && (up.Magnitude > 0f))
			{
				forward /= m;
				Vector3 right = Vector3.Cross(up, forward);
				right.Normalize();
				up = Vector3.Cross(forward, right);
				return new Matrix3(right, up, forward);
			}
			else
			{
				return Matrix3.Identity;
			}
		}

		public static Matrix3 FromRightAndUp(Vector3 right, Vector3 up)
		{
			float m = right.Magnitude;
			if ((m > 0f) && (up.Magnitude > 0f))
			{
				right /= m;
				Vector3 forward = Vector3.Cross(right, up);
				forward.Normalize();
				up = Vector3.Cross(forward, right);
				return new Matrix3(right, up, forward);
			}
			else
			{
				return Matrix3.Identity;
			}
		}

		public static Matrix3 FromYawPitchRoll(YawPitchRoll ypr)
		{
			return Matrix3.Rotation(ypr);
		}

		public static Matrix3 FromEuler(Euler e)
		{
			return Matrix3.Rotation(e);
		}

		public static Matrix3 FromQuaternion(Quaternion q) // Input quaternion must be normalized
		{
			if (q != Quaternion.Identity)
			{
				//float nq = q.Norm;
				const float s = /*(nq > 0f) ? 2f/nq :*/ 2f;
				float xs = q.x_*s, ys = q.y_*s, zs = q.z_*s; // #TODO SIMD
				float wx = q.w_*xs, wy = q.w_*ys, wz = q.w_*zs;
				float xx = q.x_*xs, xy = q.x_*ys, xz = q.x_*zs;
				float yy = q.y_*ys, yz = q.y_*zs, zz = q.z_*zs;
				return new Matrix3(1f - (yy + zz), xy + wz, xz - wy,
					xy - wz, 1f - (xx + zz), yz + wx,
					xz + wy, yz - wx, 1f - (xx + yy));
			}

			return Matrix3.Identity;
		}

		public static Matrix3 Concat(in Matrix3 s, in Matrix3 t, in Matrix3 u)
		{
			return Concat(Concat(s, t), u);
		}

		public static Matrix3 Concat(in Matrix3 s, in Matrix3 t, in Matrix3 u, in Matrix3 v)
		{
			return Concat(Concat(Concat(s, t), u), v);
		}

		public static Matrix3 Concat(in Matrix3 s, in Matrix3 t, in Matrix3 u, in Matrix3 v, in Matrix3 w)
		{
			return Concat(Concat(Concat(Concat(s, t), u), v), w);
		}

		public static Matrix3 Concat(in Matrix3 s, in Matrix3 t, in Matrix3 u, in Matrix3 v, in Matrix3 w, in Matrix3 x)
		{
			return Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x);
		}

		public static Matrix3 Concat(in Matrix3 s, in Matrix3 t, in Matrix3 u, in Matrix3 v, in Matrix3 w, in Matrix3 x, in Matrix3 y)
		{
			return Concat(Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x), y);
		}

		public static Matrix3 Concat(in Matrix3 s, in Matrix3 t, in Matrix3 u, in Matrix3 v, in Matrix3 w, in Matrix3 x, in Matrix3 y, in Matrix3 z)
		{
			return Concat(Concat(Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x), y), z);
		}

		public void Concat(in Matrix3 m)
		{
			this = Concat(this, m);
		}

		public void PreConcat(in Matrix3 m)
		{
			this = Concat(m, this);
		}

		public static Matrix3 Rotation(Axis axis, float angle)
		{
			if (angle != 0f)
			{
				float sine = (float)Math.Sin(angle);
				float cosine = (float)Math.Cos(angle);

				switch (axis)
				{
					case Axis.X:
						return new Matrix3(1f, 0f, 0f, 0f, cosine, sine, 0f, -sine, cosine);
					case Axis.Y:
						return new Matrix3(cosine, 0f, -sine, 0f, 1f, 0f, sine, 0f, cosine);
					case Axis.Z:
						return new Matrix3(cosine, sine, 0f, -sine, cosine, 0f, 0f, 0f, 1f);
					default:
						return Zero;
				}
			}

			return Identity;
		}

		//public void Rotate(Axis axis, float angle)
		//{
		//	this *= Rotation(axis, angle);
		//}

		public static Matrix3 Rotation(Vector3 axis, float angle)
		{
			float m = axis.Magnitude;
			if ((m > 0f) && (angle != 0f))
			{
				axis /= m;
				float sine = (float)Math.Sin(angle);
				float cosine = (float)Math.Cos(angle);
				float mCos = 1f - cosine;
				float aa = axis.x_*axis.x_, bb = axis.y_*axis.y_, cc = axis.z_*axis.z_;
				float abm = axis.x_*axis.y_*mCos, acm = axis.x_*axis.z_*mCos, bcm = axis.y_*axis.z_*mCos;
				float aSin = axis.x_*sine, bSin = axis.y_*sine, cSin = axis.z_*sine;
				return new Matrix3(aa*mCos + cosine, abm + cSin, acm - bSin,
					abm - cSin, bb*mCos + cosine, bcm + aSin,
					acm + bSin, bcm - aSin, cc*mCos + cosine);
			}

			return Identity;
		}

		//public void Rotate(Vector3 axis, float angle)
		//{
		//	this *= Rotation(axis, angle);
		//}

		public static Matrix3 Rotation(YawPitchRoll ypr)
		{
			if ((ypr.yaw_ != 0f) || (ypr.pitch_ != 0f) || (ypr.roll_ != 0f))
			{
				float sinh = (float)Math.Sin(ypr.yaw_);
				float cosh = (float)Math.Cos(ypr.yaw_);
				float sinp = (float)Math.Sin(ypr.pitch_);
				float cosp = (float)Math.Cos(ypr.pitch_);
				float sinb = (float)Math.Sin(ypr.roll_);
				float cosb = (float)Math.Cos(ypr.roll_);
				float sinhsinp = sinh*sinp;
				float coshsinp = cosh*sinp;
				return new Matrix3(cosb*cosh + sinb*sinhsinp, sinb*cosp, sinb*coshsinp - cosb*sinh,
					cosb*sinhsinp - sinb*cosh, cosb*cosp, cosb*coshsinp + sinb*sinh,
					sinh*cosp, -sinp, cosh*cosp);
			}

			return Matrix3.Identity;
		}

		//public void Rotate(YawPitchRoll ypr)
		//{
		//	this *= Rotation(ypr);
		//}

		public static Matrix3 Rotation(Euler e)
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

				float ti = e[(f != 0) ? ((s != 0) ? i : k) : i];
				float tj = e[j];
				float th = e[(f != 0) ? i : ((s != 0) ? i : k)];

				if (f != 0)
				{
					float t = ti;
					ti = th;
					th = t;
				}

				if (n != 0)
				{
					ti = -ti;
					tj = -tj;
					th = -th;
				}

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

				Matrix3 m = new Matrix3();
				if (s != 0)
				{
					m[i, i] = cj; m[j, i] = sj*si; m[k, i] = sj*ci;
					m[i, j] = sj*sh; m[j, j] = -cj*ss + cc; m[k, j] = -cj*cs - sc;
					m[i, k] = -sj*ch; m[j, k] = cj*sc + cs; m[k, k] = cj*cc - ss;
				}
				else
				{
					m[i, i] = cj*ch; m[j, i] = sj*sc - cs; m[k, i] = sj*cc + ss;
					m[i, j] = cj*sh; m[j, j] = sj*ss + cc; m[k, j] = sj*cs - sc;
					m[i, k] = -sj; m[j, k] = cj*si; m[k, k] = cj*ci;
				}

				return m;
			}

			return Matrix3.Identity;
		}

		//public void Rotate(Euler e)
		//{
		//	this *= Rotation(e);
		//}

		public static Matrix3 Rotation(Quaternion q) // #TODO SIMD
		{
			if (q != Quaternion.Identity)
			{
				float nq = q.Norm;
				float s = (nq > 0f) ? 2f/nq : 2f;
				float xs = q.x_*s, ys = q.y_*s, zs = q.z_*s;
				float wx = q.w_*xs, wy = q.w_*ys, wz = q.w_*zs;
				float xx = q.x_*xs, xy = q.x_*ys, xz = q.x_*zs;
				float yy = q.y_*ys, yz = q.y_*zs, zz = q.z_*zs;
				return new Matrix3(1f - (yy + zz), xy + wz, xz - wy,
					xy - wz, 1f - (xx + zz), yz + wx,
					xz + wy, yz - wx, 1f - (xx + yy));
			}

			return Matrix3.Identity;
		}

		//public void Rotate(Quaternion q)
		//{
		//	this *= Rotation(q);
		//}

		public static Matrix3 Scaling(Vector3 v)
		{
			return new Matrix3(v.x_, 0f, 0f, 0f, v.y_, 0f, 0f, 0f, v.z_);
		}

		public static Matrix3 Scaling(float s)
		{
			return new Matrix3(s, 0f, 0f, 0f, s, 0f, 0f, 0f, s);
		}

		public static Matrix3 ScalingRotation(Vector3 s, Axis axis, float angle)
		{
			Matrix3 r = Matrix3.Rotation(axis, angle);
			r.PreScale(s);
			return r;
		}

		public static Matrix3 ScalingRotation(Vector3 s, Vector3 axis, float angle)
		{
			Matrix3 r = Matrix3.Rotation(axis, angle);
			r.PreScale(s);
			return r;
		}

		public static Matrix3 ScalingRotation(Vector3 s, YawPitchRoll ypr)
		{
			Matrix3 r = Matrix3.Rotation(ypr);
			r.PreScale(s);
			return r;
		}

		public static Matrix3 ScalingRotation(Vector3 s, Euler e)
		{
			Matrix3 r = Matrix3.Rotation(e);
			r.PreScale(s);
			return r;
		}

		public static Matrix3 ScalingRotation(Vector3 s, Quaternion q)
		{
			Matrix3 r = Matrix3.Rotation(q);
			r.PreScale(s);
			return r;
		}

		public static Matrix3 Shearing(float xy, float xz, float yx, float yz, float zx, float zy)
		{
			return new Matrix3(1f, xy, xz, yx, 1f, yz, zx, zy, 1f);
		}

		//public void Shear(float xy, float xz, float yx, float yz, float zx, float zy)
		//{
		//	this *= new Matrix3(1f, xy, xz, yx, 1f, yz, zx, zy, 1f);
		//}

		public static Matrix3 Adjoint(in Matrix3 m)
		{
			return new Matrix3(new Matrix2(m.m11_, m.m12_, m.m21_, m.m22_).Determinant,
				-(new Matrix2(m.m01_, m.m02_, m.m21_, m.m22_).Determinant),
				 new Matrix2(m.m01_, m.m02_, m.m11_, m.m12_).Determinant,
				-(new Matrix2(m.m10_, m.m12_, m.m20_, m.m22_).Determinant),
				 new Matrix2(m.m00_, m.m02_, m.m20_, m.m22_).Determinant,
				-(new Matrix2(m.m00_, m.m02_, m.m10_, m.m12_).Determinant),
				 new Matrix2(m.m10_, m.m11_, m.m20_, m.m21_).Determinant,
				-(new Matrix2(m.m00_, m.m01_, m.m20_, m.m21_).Determinant),
				 new Matrix2(m.m00_, m.m01_, m.m10_, m.m11_).Determinant);
		}

		public static Matrix3 Inverse(in Matrix3 m)
		{
			float a = m.m11_*m.m22_ - m.m12_*m.m21_, b = m.m12_*m.m20_ - m.m10_*m.m22_, c = m.m10_*m.m21_ - m.m11_*m.m20_;
            float d = m.m02_*m.m21_ - m.m01_*m.m22_, e = m.m00_*m.m22_ - m.m02_*m.m20_, f = m.m01_*m.m20_ - m.m00_*m.m21_;
            float g = m.m01_*m.m12_ - m.m02_*m.m11_, h = m.m02_*m.m10_ - m.m00_*m.m12_, i = m.m00_*m.m11_ - m.m01_*m.m10_;
			float n = 1f/(m.m00_*a + m.m01_*b + m.m02_*c);
#if SIMD
			return new Matrix3(new System.Numerics.Vector3(a, d, g)*n, new System.Numerics.Vector3(b, e, h)*n,
				new System.Numerics.Vector3(c, f, i)*n);
#else
			return new Matrix3(a*n, d*n, g*n, b*n, e*n, h*n, c*n, f*n, i*n);
#endif
		}

		public static Matrix3 InverseTranspose(in Matrix3 m)
		{
			float a = m.m11_*m.m22_ - m.m12_*m.m21_, b = m.m12_*m.m20_ - m.m10_*m.m22_, c = m.m10_*m.m21_ - m.m11_*m.m20_;
			float d = m.m02_*m.m21_ - m.m01_*m.m22_, e = m.m00_*m.m22_ - m.m02_*m.m20_, f = m.m01_*m.m20_ - m.m00_*m.m21_;
			float g = m.m01_*m.m12_ - m.m02_*m.m11_, h = m.m02_*m.m10_ - m.m00_*m.m12_, i = m.m00_*m.m11_ - m.m01_*m.m10_;
			float n = 1f/(m.m00_*a + m.m01_*b + m.m02_*c);
#if SIMD
			return new Matrix3(new System.Numerics.Vector3(a, b, c)*n, new System.Numerics.Vector3(d, e, f)*n,
				new System.Numerics.Vector3(g, h, i)*n);
#else
			return new Matrix3(a*n, b*n, c*n, d*n, e*n, f*n, g*n, h*n, i*n);
#endif
		}

		/// <summary>
		/// Stabilized Gram-Schmidt orthonormalization.
		/// </summary>
		public static Matrix3 Orthonormalize(in Matrix3 m)
		{
			Vector3 v1 = m.Row0;
			Vector3 v2 = m.Row1;
			Vector3 v3 = m.Row2;
			v2 -= Vector3.Project(v2, v1);
			v3 -= Vector3.Project(v3, v1);
			v3 -= Vector3.Project(v3, v2);
			return new Matrix3(Vector3.Normalize(v1), Vector3.Normalize(v2), Vector3.Normalize(v3));
		}

		public void Invert()
		{
			float a = m11_*m22_ - m12_*m21_, b = m12_*m20_ - m10_*m22_, c = m10_*m21_ - m11_*m20_;
			float d = m02_*m21_ - m01_*m22_, e = m00_*m22_ - m02_*m20_, f = m01_*m20_ - m00_*m21_;
			float g = m01_*m12_ - m02_*m11_, h = m02_*m10_ - m00_*m12_, i = m00_*m11_ - m01_*m10_;
			float n = 1f/(m00_*a + m01_*b + m02_*c);
#if SIMD
			row0_ = new System.Numerics.Vector3(a, d, g)*n;
			row1_ = new System.Numerics.Vector3(b, e, h)*n;
			row2_ = new System.Numerics.Vector3(c, f, i)*n;
#else
			m00_ = a*n; m01_ = d*n; m02_ = g*n;
			m10_ = b*n; m11_ = e*n; m12_ = h*n;
			m20_ = c*n; m21_ = f*n; m22_ = i*n;
#endif
		}

		/// <summary>
		/// Stabilized Gram-Schmidt orthonormalization.
		/// </summary>
		public void Orthonormalize()
		{
			Vector3 v1 = Row0;
			Vector3 v2 = Row1;
			Vector3 v3 = Row2;
			v2 -= Vector3.Project(v2, v1);
			v3 -= Vector3.Project(v3, v1);
			v3 -= Vector3.Project(v3, v2);
			Row0 = Vector3.Normalize(v1);
			Row1 = Vector3.Normalize(v2);
			Row2 = Vector3.Normalize(v3);
		}

		[Browsable(false)]
		public readonly bool IsFinite
		{
			get
			{
				return Functions.IsFinite(m00_) && Functions.IsFinite(m01_) && Functions.IsFinite(m02_) &&
					Functions.IsFinite(m10_) && Functions.IsFinite(m11_) && Functions.IsFinite(m12_) &&
					Functions.IsFinite(m20_) && Functions.IsFinite(m21_) && Functions.IsFinite(m22_);
			}
		}

		[Browsable(false)]
		public readonly float Trace => m00_ + m11_ + m22_;

		[Browsable(false)]
		public readonly float Determinant
		{
			get => m00_*(m11_*m22_ - m12_*m21_) - m01_*(m10_*m22_ - m12_*m20_) + m02_*(m10_*m21_ - m11_*m20_);
		}

		[Browsable(false)]
		public readonly Vector3 ScaleComponent => new Vector3(Row0.Magnitude, Row1.Magnitude, Row2.Magnitude);

#if SIMD
		internal readonly float m00_ => row0_.X;
		internal readonly float m01_ => row0_.Y;
		internal readonly float m02_ => row0_.Z;
		internal readonly float m10_ => row1_.X;
		internal readonly float m11_ => row1_.Y;
		internal readonly float m12_ => row1_.Z;
		internal readonly float m20_ => row2_.X;
		internal readonly float m21_ => row2_.Y;
		internal readonly float m22_ => row2_.Z;
		internal readonly System.Numerics.Vector3 column0_ => new System.Numerics.Vector3(row0_.X, row1_.X, row2_.X);
		internal readonly System.Numerics.Vector3 column1_ => new System.Numerics.Vector3(row0_.Y, row1_.Y, row2_.Y);
		internal readonly System.Numerics.Vector3 column2_ => new System.Numerics.Vector3(row0_.Z, row1_.Z, row2_.Z);

		[FieldOffset(0)] internal System.Numerics.Vector3 row0_;
		[FieldOffset(16)] internal System.Numerics.Vector3 row1_;
		[FieldOffset(32)] internal System.Numerics.Vector3 row2_;
#else
		internal float m00_, m01_, m02_;
		internal float m10_, m11_, m12_;
		internal float m20_, m21_, m22_;
#endif

		private static readonly int[] safe_ = { 0, 1, 2, 0 };
		private static readonly int[] next_ = { 1, 2, 0, 1 };
	}
}
