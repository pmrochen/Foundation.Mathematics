/*
 *  Name: Matrix4
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	[Serializable]
	[TypeConverter(typeof(Matrix4Converter))]
	public struct Matrix4 : ISerializable, IFormattable, IEquatable<Matrix4>
	{
#if SIMD
		public static readonly Matrix4 Zero = new Matrix4(System.Numerics.Vector4.Zero, System.Numerics.Vector4.Zero,
			System.Numerics.Vector4.Zero, System.Numerics.Vector4.Zero);
		public static readonly Matrix4 Identity = new Matrix4(System.Numerics.Vector4.UnitX, System.Numerics.Vector4.UnitY,
			System.Numerics.Vector4.UnitZ, System.Numerics.Vector4.UnitW);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13,
			float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
		{
			row0_ = new System.Numerics.Vector4(m00, m01, m02, m03);
			row1_ = new System.Numerics.Vector4(m10, m11, m12, m13);
			row2_ = new System.Numerics.Vector4(m20, m21, m22, m23);
			row3_ = new System.Numerics.Vector4(m30, m31, m32, m33);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
		{
			row0_ = row0.xyzw_;
			row1_ = row1.xyzw_;
			row2_ = row2.xyzw_;
			row3_ = row3.xyzw_;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Matrix4(System.Numerics.Vector4 row0, System.Numerics.Vector4 row1, System.Numerics.Vector4 row2,
			System.Numerics.Vector4 row3)
		{
			row0_ = row0;
			row1_ = row1;
			row2_ = row2;
			row3_ = row3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Matrix4(float[] m)
		{
			row0_ = new System.Numerics.Vector4(m[0], m[1], m[2], m[3]);
			row1_ = new System.Numerics.Vector4(m[4], m[5], m[6], m[7]);
			row2_ = new System.Numerics.Vector4(m[8], m[9], m[10], m[11]);
			row3_ = new System.Numerics.Vector4(m[12], m[13], m[14], m[15]);
		}

		private Matrix4(SerializationInfo info, StreamingContext context)
		{
			row0_ = new System.Numerics.Vector4(info.GetSingle("M00"), info.GetSingle("M01"), info.GetSingle("M02"),
				info.GetSingle("M03"));
			row1_ = new System.Numerics.Vector4(info.GetSingle("M10"), info.GetSingle("M11"), info.GetSingle("M12"),
				info.GetSingle("M13"));
			row2_ = new System.Numerics.Vector4(info.GetSingle("M20"), info.GetSingle("M21"), info.GetSingle("M22"),
				info.GetSingle("M23"));
			row3_ = new System.Numerics.Vector4(info.GetSingle("M30"), info.GetSingle("M31"), info.GetSingle("M32"),
				info.GetSingle("M33"));
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
		public float M03
		{
			readonly get => row0_.W;
			set => row0_.W = value;
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
		public float M13
		{
			readonly get => row1_.W;
			set => row1_.W = value;
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
		public float M23
		{
			readonly get => row2_.W;
			set => row2_.W = value;
		}

		[Browsable(false)]
		public float M30
		{
			readonly get => row3_.X;
			set => row3_.X = value;
		}

		[Browsable(false)]
		public float M31
		{
			readonly get => row3_.Y;
			set => row3_.Y = value;
		}

		[Browsable(false)]
		public float M32
		{
			readonly get => row3_.Z;
			set => row3_.Z = value;
		}

		[Browsable(false)]
		public float M33
		{
			readonly get => row3_.W;
			set => row3_.W = value;
		}

		public Vector4 Row0
		{
			readonly get => new Vector4(row0_);
			set => row0_ = value.xyzw_;
		}

		public Vector4 Row1
		{
			readonly get => new Vector4(row1_);
			set => row1_ = value.xyzw_;
		}

		public Vector4 Row2
		{
			readonly get => new Vector4(row2_);
			set => row2_ = value.xyzw_;
		}

		public Vector4 Row3
		{
			readonly get => new Vector4(row3_);
			set => row3_ = value.xyzw_;
		}

		[Browsable(false)]
		public readonly Vector4 Column0 => new Vector4(column0_);

		[Browsable(false)]
		public readonly Vector4 Column1 => new Vector4(column1_);

		[Browsable(false)]
		public readonly Vector4 Column2 => new Vector4(column2_);

		[Browsable(false)]
		public readonly Vector4 Column3 => new Vector4(column3_);

		public readonly override bool Equals(object other)
		{
			if (other is Matrix4 rhs)
				return (row0_ == rhs.row0_) && (row1_ == rhs.row1_) && (row2_ == rhs.row2_) && (row3_ == rhs.row3_);

			return false;
		}

		public readonly bool Equals(Matrix4 other)
		{
			return (row0_ == other.row0_) && (row1_ == other.row1_) && (row2_ == other.row2_) && (row3_ == other.row3_);
		}

		public Vector4 this[int row]
		{
			readonly get
			{
				switch (row)
				{
					case 0:
						return new Vector4(row0_);
					case 1:
						return new Vector4(row1_);
					case 2:
						return new Vector4(row2_);
					case 3:
						return new Vector4(row3_);
					default:
						throw new IndexOutOfRangeException();
				}
			}

			set
			{
				switch (row)
				{
					case 0:
						row0_ = value.xyzw_;
						break;
					case 1:
						row1_ = value.xyzw_;
						break;
					case 2:
						row2_ = value.xyzw_;
						break;
					case 3:
						row3_ = value.xyzw_;
						break;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		public float this[int row, int column] // #TODO Use System.Numerics.Vector4 indexing operator
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
							case 3:
								return row0_.W;
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
							case 3:
								return row1_.W;
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
							case 3:
								return row2_.W;
							default:
								throw new IndexOutOfRangeException();
						}

					case 3:
						switch (column)
						{
							case 0:
								return row3_.X;
							case 1:
								return row3_.Y;
							case 2:
								return row3_.Z;
							case 3:
								return row3_.W;
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
							case 3:
								row0_.W = value;
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
							case 3:
								row1_.W = value;
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
							case 3:
								row2_.W = value;
								break;
							default:
								throw new IndexOutOfRangeException();
						}
						break;

					case 3:
						switch (column)
						{
							case 0:
								row3_.X = value;
								break;
							case 1:
								row3_.Y = value;
								break;
							case 2:
								row3_.Z = value;
								break;
							case 3:
								row3_.W = value;
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

		public static implicit operator Matrix4(in Matrix2 m)
		{
			return new Matrix4(new System.Numerics.Vector4(m.row0_, 0f, 0f), new System.Numerics.Vector4(m.row1_, 0f, 0f),
				System.Numerics.Vector4.UnitZ, System.Numerics.Vector4.UnitW);
		}

		public static implicit operator Matrix4(in Matrix3 m)
		{
			return new Matrix4(new System.Numerics.Vector4(m.row0_, 0f), new System.Numerics.Vector4(m.row1_, 0f),
				new System.Numerics.Vector4(m.row2_, 0f), System.Numerics.Vector4.UnitW);
		}

		public static implicit operator Matrix4(in AffineTransform t)
		{
			return new Matrix4(new System.Numerics.Vector4(t.row0_, 0f), new System.Numerics.Vector4(t.row1_, 0f),
				new System.Numerics.Vector4(t.row2_, 0f), new System.Numerics.Vector4(t.row3_, 1f));
		}

		public static bool operator ==(Matrix4 lhs, Matrix4 rhs)
		{
			return (lhs.row0_ == rhs.row0_) && (lhs.row1_ == rhs.row1_) && (lhs.row2_ == rhs.row2_) && (lhs.row3_ == rhs.row3_);
		}

		public static bool operator !=(Matrix4 lhs, Matrix4 rhs)
		{
			return (lhs.row0_ != rhs.row0_) || (lhs.row1_ != rhs.row1_) || (lhs.row2_ != rhs.row2_) || (lhs.row3_ != rhs.row3_);
		}

		public static Matrix4 operator +(Matrix4 m)
		{
			return m;
		}

		public static Matrix4 operator -(Matrix4 m)
		{
			return new Matrix4(-m.row0_, -m.row1_, -m.row2_, -m.row3_);
		}

		public static Matrix4 operator +(Matrix4 m, Matrix4 n)
		{
			return new Matrix4(m.row0_ + n.row0_, m.row1_ + n.row1_, m.row2_ + n.row2_, m.row3_ + n.row3_);
		}

		public static Matrix4 operator -(Matrix4 m, Matrix4 n)
		{
			return new Matrix4(m.row0_ - n.row0_, m.row1_ - n.row1_, m.row2_ - n.row2_, m.row3_ - n.row3_);
		}

		public static Matrix4 operator *(Matrix4 m, float f)
		{
			return new Matrix4(m.row0_*f, m.row1_*f, m.row2_*f, m.row3_*f);
		}

		public static Matrix4 operator *(float f, Matrix4 m)
		{
			return new Matrix4(f*m.row0_, f*m.row1_, f*m.row2_, f*m.row3_);
		}

		public static Matrix4 operator *(Matrix4 m, Matrix4 n)
		{
			return new Matrix4(m.m00_*n.row0_ + m.m01_*n.row1_ + m.m02_*n.row2_ + m.m03_*n.row3_,
				m.m10_*n.row0_ + m.m11_*n.row1_ + m.m12_*n.row2_ + m.m13_*n.row3_,
				m.m20_*n.row0_ + m.m21_*n.row1_ + m.m22_*n.row2_ + m.m23_*n.row3_,
				m.m30_*n.row0_ + m.m31_*n.row1_ + m.m32_*n.row2_ + m.m33_*n.row3_);
		}

		public static Matrix4 FromAffineTransform(in AffineTransform t)
		{
			return new Matrix4(new System.Numerics.Vector4(t.row0_, 0f), new System.Numerics.Vector4(t.row1_, 0f),
				new System.Numerics.Vector4(t.row2_, 0f), new System.Numerics.Vector4(t.row3_, 1f));
		}

		public static Matrix4 Concat(in Matrix4 m, in Matrix4 n)
		{
			return new Matrix4(m.m00_*n.row0_ + m.m01_*n.row1_ + m.m02_*n.row2_ + m.m03_*n.row3_,
				m.m10_*n.row0_ + m.m11_*n.row1_ + m.m12_*n.row2_ + m.m13_*n.row3_,
				m.m20_*n.row0_ + m.m21_*n.row1_ + m.m22_*n.row2_ + m.m23_*n.row3_,
				m.m30_*n.row0_ + m.m31_*n.row1_ + m.m32_*n.row2_ + m.m33_*n.row3_);
		}

		public void PreScale(Vector3 v)
		{
			row0_ *= v.xyz_.X; 
			row1_ *= v.xyz_.Y; 
			row2_ *= v.xyz_.Z;
			//row3_ *= 1f;
		}

		public void Scale(Vector3 v)
		{
			System.Numerics.Vector4 s = new System.Numerics.Vector4(v.xyz_, 1f);
			row0_ *= s;
			row1_ *= s;
			row2_ *= s;
			row3_ *= s;
		}

		public static Matrix4 Translation(Vector3 v)
		{
			return new Matrix4(System.Numerics.Vector4.UnitX, System.Numerics.Vector4.UnitY, System.Numerics.Vector4.UnitZ,
				new System.Numerics.Vector4(v.xyz_, 1f));
		}

		public void Translate(Vector3 v)
		{
			System.Numerics.Vector4 t = new System.Numerics.Vector4(v.xyz_, 0f);
			row0_ += row0_.W*t;
			row1_ += row1_.W*t;
			row2_ += row2_.W*t;
			row3_ += row3_.W*t;
		}

		public static Matrix4 Transpose(in Matrix4 m)
		{
			return new Matrix4(m.column0_, m.column1_, m.column2_, m.column3_);
		}

		public void Transpose()
		{
			System.Numerics.Vector4 column0 = column0_;
			System.Numerics.Vector4 column1 = column1_;
			System.Numerics.Vector4 column2 = column2_;
			System.Numerics.Vector4 column3 = column3_;
			row0_ = column0;
			row1_ = column1;
			row2_ = column2;
			row3_ = column3;
		}
#else
		public static readonly Matrix4 Zero = new Matrix4(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		public static readonly Matrix4 Identity = new Matrix4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);

		public Matrix4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13,
			float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
		{
			m00_ = m00; m01_ = m01; m02_ = m02; m03_ = m03;
			m10_ = m10; m11_ = m11; m12_ = m12; m13_ = m13;
			m20_ = m20; m21_ = m21; m22_ = m22; m23_ = m23;
			m30_ = m30; m31_ = m31; m32_ = m32; m33_ = m33;
		}

		public Matrix4(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
		{
			m00_ = row0.x_; m01_ = row0.y_; m02_ = row0.z_; m03_ = row0.w_;
			m10_ = row1.x_; m11_ = row1.y_; m12_ = row1.z_; m13_ = row1.w_;
			m20_ = row2.x_; m21_ = row2.y_; m22_ = row2.z_; m23_ = row2.w_;
			m30_ = row3.x_; m31_ = row3.y_; m32_ = row3.z_; m33_ = row3.w_;
		}

		public Matrix4(float[] m)
		{
			m00_ = m[0]; m01_ = m[1]; m02_ = m[2]; m03_ = m[3];
			m10_ = m[4]; m11_ = m[5]; m12_ = m[6]; m13_ = m[7];
			m20_ = m[8]; m21_ = m[9]; m22_ = m[10]; m23_ = m[11];
			m30_ = m[12]; m31_ = m[13]; m32_ = m[14]; m33_ = m[15];
		}

		private Matrix4(SerializationInfo info, StreamingContext context)
		{
			m00_ = info.GetSingle("M00"); m01_ = info.GetSingle("M01"); m02_ = info.GetSingle("M02"); m03_ = info.GetSingle("M03");
			m10_ = info.GetSingle("M10"); m11_ = info.GetSingle("M11"); m12_ = info.GetSingle("M12"); m13_ = info.GetSingle("M13");
			m20_ = info.GetSingle("M20"); m21_ = info.GetSingle("M21"); m22_ = info.GetSingle("M22"); m23_ = info.GetSingle("M23");
			m30_ = info.GetSingle("M30"); m31_ = info.GetSingle("M31"); m32_ = info.GetSingle("M32"); m33_ = info.GetSingle("M33");
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
		public float M03
		{
			readonly get => m03_;
			set => m03_ = value;
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
		public float M13
		{
			readonly get => m13_;
			set => m13_ = value;
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
		public float M23
		{
			readonly get => m23_;
			set => m23_ = value;
		}

		[Browsable(false)]
		public float M30
		{
			readonly get => m30_;
			set => m30_ = value;
		}

		[Browsable(false)]
		public float M31
		{
			readonly get => m31_;
			set => m31_ = value;
		}

		[Browsable(false)]
		public float M32
		{
			readonly get => m32_;
			set => m32_ = value;
		}

		[Browsable(false)]
		public float M33
		{
			readonly get => m33_;
			set => m33_ = value;
		}

		public Vector4 Row0
		{
			readonly get => new Vector4(m00_, m01_, m02_, m03_);
			set 
			{ 
				m00_ = value.x_; 
				m01_ = value.y_; 
				m02_ = value.z_; 
				m03_ = value.w_; 
			}
		}

		public Vector4 Row1
		{
			readonly get => new Vector4(m10_, m11_, m12_, m13_);
			set 
			{ 
				m10_ = value.x_; 
				m11_ = value.y_; 
				m12_ = value.z_; 
				m13_ = value.w_; 
			}
		}

		public Vector4 Row2
		{
			readonly get => new Vector4(m20_, m21_, m22_, m23_);
			set 
			{ 
				m20_ = value.x_; 
				m21_ = value.y_; 
				m22_ = value.z_; 
				m23_ = value.w_; 
			}
		}

		public Vector4 Row3
		{
			readonly get => new Vector4(m30_, m31_, m32_, m33_);
			set 
			{ 
				m30_ = value.x_; 
				m31_ = value.y_; 
				m32_ = value.z_; 
				m33_ = value.w_; 
			}
		}

		[Browsable(false)]
		public readonly Vector4 Column0 => new Vector4(m00_, m10_, m20_, m30_);

		[Browsable(false)]
		public readonly Vector4 Column1 => new Vector4(m01_, m11_, m21_, m31_);

		[Browsable(false)]
		public readonly Vector4 Column2 => new Vector4(m02_, m12_, m22_, m32_);

		[Browsable(false)]
		public readonly Vector4 Column3 => new Vector4(m03_, m13_, m23_, m33_);

		public readonly override bool Equals(object other)
		{
			if (other is Matrix4 rhs)
			{
				return (m00_ == rhs.m00_) && (m01_ == rhs.m01_) && (m02_ == rhs.m02_) && (m03_ == rhs.m03_) &&
					(m10_ == rhs.m10_) && (m11_ == rhs.m11_) && (m12_ == rhs.m12_) && (m13_ == rhs.m13_) &&
					(m20_ == rhs.m20_) && (m21_ == rhs.m21_) && (m22_ == rhs.m22_) && (m23_ == rhs.m23_) &&
					(m30_ == rhs.m30_) && (m31_ == rhs.m31_) && (m32_ == rhs.m32_) && (m33_ == rhs.m33_);
			}
			
			return false;
		}

		public readonly bool Equals(Matrix4 other)
		{
			return (m00_ == other.m00_) && (m01_ == other.m01_) && (m02_ == other.m02_) && (m03_ == other.m03_) &&
				(m10_ == other.m10_) && (m11_ == other.m11_) && (m12_ == other.m12_) && (m13_ == other.m13_) &&
				(m20_ == other.m20_) && (m21_ == other.m21_) && (m22_ == other.m22_) && (m23_ == other.m23_) &&
				(m30_ == other.m30_) && (m31_ == other.m31_) && (m32_ == other.m32_) && (m33_ == other.m33_);
		}

		public Vector4 this[int row]
		{
			readonly get
			{
				switch (row)
				{
					case 0: 
						return new Vector4(m00_, m01_, m02_, m03_);
					case 1: 
						return new Vector4(m10_, m11_, m12_, m13_);
					case 2: 
						return new Vector4(m20_, m21_, m22_, m23_);
					case 3: 
						return new Vector4(m30_, m31_, m32_, m33_);
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
						m03_ = value.w_; 
						break;
					case 1: 
						m10_ = value.x_; 
						m11_ = value.y_; 
						m12_ = value.z_; 
						m13_ = value.w_; 
						break;
					case 2: 
						m20_ = value.x_; 
						m21_ = value.y_; 
						m22_ = value.z_; 
						m23_ = value.w_; 
						break;
					case 3: 
						m30_ = value.x_; 
						m31_ = value.y_; 
						m32_ = value.z_; 
						m33_ = value.w_; 
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
							case 3: 
								return m03_;
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
							case 3: 
								return m13_;
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
							case 3: 
								return m23_;
							default: 
								throw new IndexOutOfRangeException();
						}

					case 3:
						switch (column)
						{
							case 0: 
								return m30_;
							case 1: 
								return m31_;
							case 2: 
								return m32_;
							case 3: 
								return m33_;
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
							case 3: 
								m03_ = value; 
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
							case 3:
								m13_ = value;
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
							case 3:
								m23_ = value;
								break;
							default:
								throw new IndexOutOfRangeException();
						}
						break;

					case 3:
						switch (column)
						{
							case 0:
								m30_ = value;
								break;
							case 1:
								m31_ = value;
								break;
							case 2:
								m32_ = value;
								break;
							case 3:
								m33_ = value;
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

		public static implicit operator Matrix4(in Matrix2 m)
		{
			return new Matrix4(m.m00_, m.m01_, 0f, 0f,
				m.m10_, m.m11_, 0f, 0f,
				0f, 0f, 1f, 0f,
				0f, 0f, 0f, 1f);
		}

		public static implicit operator Matrix4(in Matrix3 m)
		{
			return new Matrix4(m.m00_, m.m01_, m.m02_, 0f,
				m.m10_, m.m11_, m.m12_, 0f,
				m.m20_, m.m21_, m.m22_, 0f,
				0f, 0f, 0f, 1f);
		}

        public static implicit operator Matrix4(in AffineTransform t)
        {
			return new Matrix4(t.r_.m00_, t.r_.m01_, t.r_.m02_, 0f,
				t.r_.m10_, t.r_.m11_, t.r_.m12_, 0f,
				t.r_.m20_, t.r_.m21_, t.r_.m22_, 0f,
				t.t_.x_, t.t_.y_, t.t_.z_, 1f);
        }

		public static bool operator ==(Matrix4 lhs, Matrix4 rhs)
		{
			return (lhs.m00_ == rhs.m00_) && (lhs.m01_ == rhs.m01_) && (lhs.m02_ == rhs.m02_) && (lhs.m03_ == rhs.m03_) &&
				(lhs.m10_ == rhs.m10_) && (lhs.m11_ == rhs.m11_) && (lhs.m12_ == rhs.m12_) && (lhs.m13_ == rhs.m13_) &&
				(lhs.m20_ == rhs.m20_) && (lhs.m21_ == rhs.m21_) && (lhs.m22_ == rhs.m22_) && (lhs.m23_ == rhs.m23_) &&
				(lhs.m30_ == rhs.m30_) && (lhs.m31_ == rhs.m31_) && (lhs.m32_ == rhs.m32_) && (lhs.m33_ == rhs.m33_);
		}

		public static bool operator !=(Matrix4 lhs, Matrix4 rhs)
		{
			return (lhs.m00_ != rhs.m00_) || (lhs.m01_ != rhs.m01_) || (lhs.m02_ != rhs.m02_) || (lhs.m03_ != rhs.m03_) ||
				(lhs.m10_ != rhs.m10_) || (lhs.m11_ != rhs.m11_) || (lhs.m12_ != rhs.m12_) || (lhs.m13_ != rhs.m13_) ||
				(lhs.m20_ != rhs.m20_) || (lhs.m21_ != rhs.m21_) || (lhs.m22_ != rhs.m22_) || (lhs.m23_ != rhs.m23_) ||
				(lhs.m30_ != rhs.m30_) || (lhs.m31_ != rhs.m31_) || (lhs.m32_ != rhs.m32_) || (lhs.m33_ != rhs.m33_);
		}

		public static Matrix4 operator +(Matrix4 m)
		{
			return m;
		}

		public static Matrix4 operator -(Matrix4 m)
		{
			return new Matrix4(-m.m00_, -m.m01_, -m.m02_, -m.m03_,
				-m.m10_, -m.m11_, -m.m12_, -m.m13_,
				-m.m20_, -m.m21_, -m.m22_, -m.m23_,
				-m.m30_, -m.m31_, -m.m32_, -m.m33_);
		}

		public static Matrix4 operator +(Matrix4 m, Matrix4 n)
		{
			return new Matrix4(m.m00_ + n.m00_, m.m01_ + n.m01_, m.m02_ + n.m02_, m.m03_ + n.m03_,
				m.m10_ + n.m10_, m.m11_ + n.m11_, m.m12_ + n.m12_, m.m13_ + n.m13_,
				m.m20_ + n.m20_, m.m21_ + n.m21_, m.m22_ + n.m22_, m.m23_ + n.m23_,
				m.m30_ + n.m30_, m.m31_ + n.m31_, m.m32_ + n.m32_, m.m33_ + n.m33_);
		}

		public static Matrix4 operator -(Matrix4 m, Matrix4 n)
		{
			return new Matrix4(m.m00_ - n.m00_, m.m01_ - n.m01_, m.m02_ - n.m02_, m.m03_ - n.m03_,
				m.m10_ - n.m10_, m.m11_ - n.m11_, m.m12_ - n.m12_, m.m13_ - n.m13_,
				m.m20_ - n.m20_, m.m21_ - n.m21_, m.m22_ - n.m22_, m.m23_ - n.m23_,
				m.m30_ - n.m30_, m.m31_ - n.m31_, m.m32_ - n.m32_, m.m33_ - n.m33_);
		}

		public static Matrix4 operator *(Matrix4 m, float f)
		{
			return new Matrix4(m.m00_*f, m.m01_*f, m.m02_*f, m.m03_*f,
				m.m10_*f, m.m11_*f, m.m12_*f, m.m13_*f,
				m.m20_*f, m.m21_*f, m.m22_*f, m.m23_*f,
				m.m30_*f, m.m31_*f, m.m32_*f, m.m33_*f);
		}

		public static Matrix4 operator *(float f, Matrix4 m)
		{
			return new Matrix4(f*m.m00_, f*m.m01_, f*m.m02_, f*m.m03_,
				f*m.m10_, f*m.m11_, f*m.m12_, f*m.m13_,
				f*m.m20_, f*m.m21_, f*m.m22_, f*m.m23_,
				f*m.m30_, f*m.m31_, f*m.m32_, f*m.m33_);
		}

		public static Matrix4 operator *(Matrix4 m, Matrix4 n)
		{
			return new Matrix4(m.m00_*n.m00_ + m.m01_*n.m10_ + m.m02_*n.m20_ + m.m03_*n.m30_,
				m.m00_*n.m01_ + m.m01_*n.m11_ + m.m02_*n.m21_ + m.m03_*n.m31_,
				m.m00_*n.m02_ + m.m01_*n.m12_ + m.m02_*n.m22_ + m.m03_*n.m32_,
				m.m00_*n.m03_ + m.m01_*n.m13_ + m.m02_*n.m23_ + m.m03_*n.m33_,
				m.m10_*n.m00_ + m.m11_*n.m10_ + m.m12_*n.m20_ + m.m13_*n.m30_,
				m.m10_*n.m01_ + m.m11_*n.m11_ + m.m12_*n.m21_ + m.m13_*n.m31_,
				m.m10_*n.m02_ + m.m11_*n.m12_ + m.m12_*n.m22_ + m.m13_*n.m32_,
				m.m10_*n.m03_ + m.m11_*n.m13_ + m.m12_*n.m23_ + m.m13_*n.m33_,
				m.m20_*n.m00_ + m.m21_*n.m10_ + m.m22_*n.m20_ + m.m23_*n.m30_,
				m.m20_*n.m01_ + m.m21_*n.m11_ + m.m22_*n.m21_ + m.m23_*n.m31_,
				m.m20_*n.m02_ + m.m21_*n.m12_ + m.m22_*n.m22_ + m.m23_*n.m32_,
				m.m20_*n.m03_ + m.m21_*n.m13_ + m.m22_*n.m23_ + m.m23_*n.m33_,
				m.m30_*n.m00_ + m.m31_*n.m10_ + m.m32_*n.m20_ + m.m33_*n.m30_,
				m.m30_*n.m01_ + m.m31_*n.m11_ + m.m32_*n.m21_ + m.m33_*n.m31_,
				m.m30_*n.m02_ + m.m31_*n.m12_ + m.m32_*n.m22_ + m.m33_*n.m32_,
				m.m30_*n.m03_ + m.m31_*n.m13_ + m.m32_*n.m23_ + m.m33_*n.m33_);
		}

		public static Matrix4 FromAffineTransform(in AffineTransform t)
		{
			return new Matrix4(t.r_.m00_, t.r_.m01_, t.r_.m02_, 0f,
				t.r_.m10_, t.r_.m11_, t.r_.m12_, 0f,
				t.r_.m20_, t.r_.m21_, t.r_.m22_, 0f,
				t.t_.x_, t.t_.y_, t.t_.z_, 1f);
		}

		public static Matrix4 Concat(in Matrix4 m, in Matrix4 n)
		{
			return new Matrix4(m.m00_*n.m00_ + m.m01_*n.m10_ + m.m02_*n.m20_ + m.m03_*n.m30_,
				m.m00_*n.m01_ + m.m01_*n.m11_ + m.m02_*n.m21_ + m.m03_*n.m31_,
				m.m00_*n.m02_ + m.m01_*n.m12_ + m.m02_*n.m22_ + m.m03_*n.m32_,
				m.m00_*n.m03_ + m.m01_*n.m13_ + m.m02_*n.m23_ + m.m03_*n.m33_,
				m.m10_*n.m00_ + m.m11_*n.m10_ + m.m12_*n.m20_ + m.m13_*n.m30_,
				m.m10_*n.m01_ + m.m11_*n.m11_ + m.m12_*n.m21_ + m.m13_*n.m31_,
				m.m10_*n.m02_ + m.m11_*n.m12_ + m.m12_*n.m22_ + m.m13_*n.m32_,
				m.m10_*n.m03_ + m.m11_*n.m13_ + m.m12_*n.m23_ + m.m13_*n.m33_,
				m.m20_*n.m00_ + m.m21_*n.m10_ + m.m22_*n.m20_ + m.m23_*n.m30_,
				m.m20_*n.m01_ + m.m21_*n.m11_ + m.m22_*n.m21_ + m.m23_*n.m31_,
				m.m20_*n.m02_ + m.m21_*n.m12_ + m.m22_*n.m22_ + m.m23_*n.m32_,
				m.m20_*n.m03_ + m.m21_*n.m13_ + m.m22_*n.m23_ + m.m23_*n.m33_,
				m.m30_*n.m00_ + m.m31_*n.m10_ + m.m32_*n.m20_ + m.m33_*n.m30_,
				m.m30_*n.m01_ + m.m31_*n.m11_ + m.m32_*n.m21_ + m.m33_*n.m31_,
				m.m30_*n.m02_ + m.m31_*n.m12_ + m.m32_*n.m22_ + m.m33_*n.m32_,
				m.m30_*n.m03_ + m.m31_*n.m13_ + m.m32_*n.m23_ + m.m33_*n.m33_);
		}

		public void PreScale(Vector3 v)
		{
			m00_ *= v.x_; m01_ *= v.x_; m02_ *= v.x_; m03_ *= v.x_;
			m10_ *= v.y_; m11_ *= v.y_; m12_ *= v.y_; m13_ *= v.y_;
			m20_ *= v.z_; m21_ *= v.z_; m22_ *= v.z_; m23_ *= v.z_;
		}

		public void Scale(Vector3 v)
		{
			m00_ *= v.x_; m01_ *= v.y_; m02_ *= v.z_;
			m10_ *= v.x_; m11_ *= v.y_; m12_ *= v.z_;
			m20_ *= v.x_; m21_ *= v.y_; m22_ *= v.z_;
			m30_ *= v.x_; m31_ *= v.y_; m32_ *= v.z_;
		}

		public static Matrix4 Translation(Vector3 v)
		{
			return new Matrix4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, v.x_, v.y_, v.z_, 1f);
		}

		public void Translate(Vector3 v)
		{
			m00_ += m03_*v.x_; m01_ += m03_*v.y_; m02_ += m03_*v.z_;
			m10_ += m13_*v.x_; m11_ += m13_*v.y_; m12_ += m13_*v.z_;
			m20_ += m23_*v.x_; m21_ += m23_*v.y_; m22_ += m23_*v.z_;
			m30_ += m33_*v.x_; m31_ += m33_*v.y_; m32_ += m33_*v.z_;
		}

		public static Matrix4 Transpose(in Matrix4 m)
		{
			return new Matrix4(m.m00_, m.m10_, m.m20_, m.m30_,
				m.m01_, m.m11_, m.m21_, m.m31_,
				m.m02_, m.m12_, m.m22_, m.m32_,
				m.m03_, m.m13_, m.m23_, m.m33_);
		}

		public void Transpose()
		{
			float t = m01_; m01_ = m10_; m10_ = t;
			t = m02_; m02_ = m20_; m20_ = t;
			t = m12_; m12_ = m21_; m21_ = t;
			t = m03_; m03_ = m30_; m30_ = t;
			t = m13_; m13_ = m31_; m31_ = t;
			t = m23_; m23_ = m32_; m32_ = t;
		}
#endif

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("M00", m00_); info.AddValue("M01", m01_); info.AddValue("M02", m02_); info.AddValue("M03", m03_);
			info.AddValue("M10", m10_); info.AddValue("M11", m11_); info.AddValue("M12", m12_); info.AddValue("M13", m13_);
			info.AddValue("M20", m20_); info.AddValue("M21", m21_); info.AddValue("M22", m22_); info.AddValue("M23", m23_);
			info.AddValue("M30", m30_); info.AddValue("M31", m31_); info.AddValue("M32", m32_); info.AddValue("M33", m33_);
		}

		public readonly override int GetHashCode()
		{
			int hash = m00_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m01_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m02_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m03_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m10_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m11_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m12_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m13_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m20_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m21_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m22_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m23_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m30_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m31_.GetHashCode();
			hash = ((hash << 5) + hash) ^ m32_.GetHashCode();
			return ((hash << 5) + hash) ^ m33_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Matrix4 m, float tolerance)
		{
			return (Math.Abs(m.m00_ - m00_) < tolerance) &&
				(Math.Abs(m.m01_ - m01_) < tolerance) &&
				(Math.Abs(m.m02_ - m02_) < tolerance) &&
				(Math.Abs(m.m03_ - m03_) < tolerance) &&
				(Math.Abs(m.m10_ - m10_) < tolerance) &&
				(Math.Abs(m.m11_ - m11_) < tolerance) &&
				(Math.Abs(m.m12_ - m12_) < tolerance) &&
				(Math.Abs(m.m13_ - m13_) < tolerance) &&
				(Math.Abs(m.m20_ - m20_) < tolerance) &&
				(Math.Abs(m.m21_ - m21_) < tolerance) &&
				(Math.Abs(m.m22_ - m22_) < tolerance) &&
				(Math.Abs(m.m23_ - m23_) < tolerance) &&
				(Math.Abs(m.m30_ - m30_) < tolerance) &&
				(Math.Abs(m.m31_ - m31_) < tolerance) &&
				(Math.Abs(m.m32_ - m32_) < tolerance) &&
				(Math.Abs(m.m33_ - m33_) < tolerance);
		}

		public readonly bool ApproxEquals(in Matrix4 m)
		{
			return ApproxEquals(m, 1e-6f);
		}

		public readonly float[] ToArray()
		{
			return new float[16] { m00_, m01_, m02_, m03_, m10_, m11_, m12_, m13_, m20_, m21_, m22_, m23_, m30_, m31_, m32_, m33_ };
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15}",
				m00_, m01_, m02_, m03_, m10_, m11_, m12_, m13_, m20_, m21_, m22_, m23_, m30_, m31_, m32_, m33_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider,
				"{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15}",
				m00_, m01_, m02_, m03_, m10_, m11_, m12_, m13_, m20_, m21_, m22_, m23_, m30_, m31_, m32_, m33_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15}",
				m00_.ToString(format), m01_.ToString(format), m02_.ToString(format), m03_.ToString(format),
				m10_.ToString(format), m11_.ToString(format), m12_.ToString(format), m13_.ToString(format),
				m20_.ToString(format), m21_.ToString(format), m22_.ToString(format), m23_.ToString(format),
				m30_.ToString(format), m31_.ToString(format), m32_.ToString(format), m33_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13} {14} {15}",
				m00_.ToString(format, provider), m01_.ToString(format, provider), m02_.ToString(format, provider), m03_.ToString(format, provider),
				m10_.ToString(format, provider), m11_.ToString(format, provider), m12_.ToString(format, provider), m13_.ToString(format, provider),
				m20_.ToString(format, provider), m21_.ToString(format, provider), m22_.ToString(format, provider), m23_.ToString(format, provider),
				m30_.ToString(format, provider), m31_.ToString(format, provider), m32_.ToString(format, provider), m33_.ToString(format, provider));
		}

		public static Matrix4 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 16)
				throw new FormatException();

			return new Matrix4(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]),
				Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5]),
				Single.Parse(m[6]), Single.Parse(m[7]), Single.Parse(m[8]),
				Single.Parse(m[9]), Single.Parse(m[10]), Single.Parse(m[11]),
				Single.Parse(m[12]), Single.Parse(m[13]), Single.Parse(m[14]),
				Single.Parse(m[15]));
		}

		public static Matrix4 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 16)
				throw new FormatException();

			return new Matrix4(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
				Single.Parse(m[2], provider), Single.Parse(m[3], provider),
				Single.Parse(m[4], provider), Single.Parse(m[5], provider),
				Single.Parse(m[6], provider), Single.Parse(m[7], provider),
				Single.Parse(m[8], provider), Single.Parse(m[9], provider),
				Single.Parse(m[10], provider), Single.Parse(m[11], provider),
				Single.Parse(m[12], provider), Single.Parse(m[13], provider),
				Single.Parse(m[14], provider), Single.Parse(m[15], provider));
		}

		public static Matrix4 Concat(in Matrix4 s, in Matrix4 t, in Matrix4 u)
		{
			return Concat(Concat(s, t), u);
		}

		public static Matrix4 Concat(in Matrix4 s, in Matrix4 t, in Matrix4 u, in Matrix4 v)
		{
			return Concat(Concat(Concat(s, t), u), v);
		}

		public static Matrix4 Concat(in Matrix4 s, in Matrix4 t, in Matrix4 u, in Matrix4 v, in Matrix4 w)
		{
			return Concat(Concat(Concat(Concat(s, t), u), v), w);
		}

		public static Matrix4 Concat(in Matrix4 s, in Matrix4 t, in Matrix4 u, in Matrix4 v, in Matrix4 w, in Matrix4 x)
		{
			return Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x);
		}

		public static Matrix4 Concat(in Matrix4 s, in Matrix4 t, in Matrix4 u, in Matrix4 v, in Matrix4 w, in Matrix4 x, in Matrix4 y)
		{
			return Concat(Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x), y);
		}

		public static Matrix4 Concat(in Matrix4 s, in Matrix4 t, in Matrix4 u, in Matrix4 v, in Matrix4 w, in Matrix4 x, in Matrix4 y, in Matrix4 z)
		{
			return Concat(Concat(Concat(Concat(Concat(Concat(Concat(s, t), u), v), w), x), y), z);
		}

		public void Concat(in Matrix4 m)
		{
			this = Concat(this, m);
		}

		public void PreConcat(in Matrix4 m)
		{
			this = Concat(m, this);
		}

		public static Matrix4 Rotation(Axis axis, float angle)
		{
			return (Matrix4)Matrix3.Rotation(axis, angle);
		}

		//public void Rotate(Axis axis, float angle)
		//{
		//	this *= (Matrix4)Matrix3.Rotation(axis, angle);
		//}

		public static Matrix4 Rotation(Vector3 axis, float angle)
		{
			return (Matrix4)Matrix3.Rotation(axis, angle);
		}

		//public void Rotate(Vector3 axis, float angle)
		//{
		//	this *= (Matrix4)Matrix3.Rotation(axis, angle);
		//}

		public static Matrix4 Rotation(YawPitchRoll ypr)
		{
			return (Matrix4)Matrix3.Rotation(ypr);
		}

		//public void Rotate(YawPitchRoll ypr)
		//{
		//	this *= (Matrix4)Matrix3.Rotation(ypr);
		//}

		public static Matrix4 Rotation(Euler e)
		{
			return (Matrix4)Matrix3.Rotation(e);
		}

		//public void Rotate(Euler e)
		//{
		//	this *= (Matrix4)Matrix3.Rotation(e);
		//}

		public static Matrix4 Rotation(Quaternion q)
		{
			return (Matrix4)Matrix3.Rotation(q);
		}

		//public void Rotate(Quaternion q)
		//{
		//	this *= (Matrix4)Matrix3.Rotation(q);
		//}

		public static Matrix4 Scaling(Vector3 v)
		{
			return new Matrix4(v.x_, 0f, 0f, 0f, 0f, v.y_, 0f, 0f, 0f, 0f, v.z_, 0f, 0f, 0f, 0f, 1f);
		}

		public static Matrix4 Scaling(float s)
		{
			return new Matrix4(s, 0f, 0f, 0f, 0f, s, 0f, 0f, 0f, 0f, s, 0f, 0f, 0f, 0f, 1f);
		}

		public static Matrix4 RotationTranslation(Axis axis, float angle, Vector3 t)
		{
			Matrix4 m = (Matrix4)Matrix3.Rotation(axis, angle);
#if SIMD
			m.row3_ = new System.Numerics.Vector4(t.xyz_, 1f);
#else
			m.m30_ = t.x_; m.m31_ = t.y_; m.m32_ = t.z_;
#endif
			return m;
		}

		public static Matrix4 RotationTranslation(Vector3 axis, float angle, Vector3 t)
		{
			Matrix4 m = (Matrix4)Matrix3.Rotation(axis, angle);
#if SIMD
			m.row3_ = new System.Numerics.Vector4(t.xyz_, 1f);
#else
			m.m30_ = t.x_; m.m31_ = t.y_; m.m32_ = t.z_;
#endif
			return m;
		}

		public static Matrix4 RotationTranslation(YawPitchRoll ypr, Vector3 t)
		{
			Matrix4 m = (Matrix4)Matrix3.Rotation(ypr);
#if SIMD
			m.row3_ = new System.Numerics.Vector4(t.xyz_, 1f);
#else
			m.m30_ = t.x_; m.m31_ = t.y_; m.m32_ = t.z_;
#endif
			return m;
		}

		public static Matrix4 RotationTranslation(Euler e, Vector3 t)
		{
			Matrix4 m = (Matrix4)Matrix3.Rotation(e);
#if SIMD
			m.row3_ = new System.Numerics.Vector4(t.xyz_, 1f);
#else
			m.m30_ = t.x_; m.m31_ = t.y_; m.m32_ = t.z_;
#endif
			return m;
		}

		public static Matrix4 RotationTranslation(Quaternion q, Vector3 t)
		{
			Matrix4 m = (Matrix4)Matrix3.Rotation(q);
#if SIMD
			m.row3_ = new System.Numerics.Vector4(t.xyz_, 1f);
#else
			m.m30_ = t.x_; m.m31_ = t.y_; m.m32_ = t.z_;
#endif
			return m;
		}

		public static Matrix4 ScalingRotation(Vector3 s, Axis axis, float angle)
		{
			return (Matrix4)Matrix3.ScalingRotation(s, axis, angle);
		}

		public static Matrix4 ScalingRotation(Vector3 s, Vector3 axis, float angle)
		{
			return (Matrix4)Matrix3.ScalingRotation(s, axis, angle);
		}

		public static Matrix4 ScalingRotation(Vector3 s, YawPitchRoll ypr)
		{
			return (Matrix4)Matrix3.ScalingRotation(s, ypr);
		}

		public static Matrix4 ScalingRotation(Vector3 s, Euler e)
		{
			return (Matrix4)Matrix3.ScalingRotation(s, e);
		}

		public static Matrix4 ScalingRotation(Vector3 s, Quaternion q)
		{
			return (Matrix4)Matrix3.ScalingRotation(s, q);
		}

		public static Matrix4 ScalingTranslation(Vector3 s, Vector3 t)
		{
			return new Matrix4(s.x_, 0f, 0f, 0f, 0f, s.y_, 0f, 0f, 0f, 0f, s.z_, 0f, t.x_, t.y_, t.z_, 1f);
		}

		public static Matrix4 ScalingRotationTranslation(Vector3 s, Axis axis, float angle, Vector3 t)
		{
			Matrix4 m = (Matrix4)Matrix3.ScalingRotation(s, axis, angle);
#if SIMD
			m.row3_ = new System.Numerics.Vector4(t.xyz_, 1f);
#else
			m.m30_ = t.x_; m.m31_ = t.y_; m.m32_ = t.z_;
#endif
			return m;
		}

		public static Matrix4 ScalingRotationTranslation(Vector3 s, Vector3 axis, float angle, Vector3 t)
		{
			Matrix4 m = (Matrix4)Matrix3.ScalingRotation(s, axis, angle);
#if SIMD
			m.row3_ = new System.Numerics.Vector4(t.xyz_, 1f);
#else
			m.m30_ = t.x_; m.m31_ = t.y_; m.m32_ = t.z_;
#endif
			return m;
		}

		public static Matrix4 ScalingRotationTranslation(Vector3 s, YawPitchRoll ypr, Vector3 t)
		{
			Matrix4 m = (Matrix4)Matrix3.ScalingRotation(s, ypr);
#if SIMD
			m.row3_ = new System.Numerics.Vector4(t.xyz_, 1f);
#else
			m.m30_ = t.x_; m.m31_ = t.y_; m.m32_ = t.z_;
#endif
			return m;
		}

		public static Matrix4 ScalingRotationTranslation(Vector3 s, Euler e, Vector3 t)
		{
			Matrix4 m = (Matrix4)Matrix3.ScalingRotation(s, e);
#if SIMD
			m.row3_ = new System.Numerics.Vector4(t.xyz_, 1f);
#else
			m.m30_ = t.x_; m.m31_ = t.y_; m.m32_ = t.z_;
#endif
			return m;
		}

		public static Matrix4 ScalingRotationTranslation(Vector3 s, Quaternion q, Vector3 t)
		{
			Matrix4 m = (Matrix4)Matrix3.ScalingRotation(s, q);
#if SIMD
			m.row3_ = new System.Numerics.Vector4(t.xyz_, 1f);
#else
			m.m30_ = t.x_; m.m31_ = t.y_; m.m32_ = t.z_;
#endif
			return m;
		}

		public static Matrix4 Shearing(float xy, float xz, float yx, float yz, float zx, float zy)
		{
			return new Matrix4(1f, xy, xz, 0f, yx, 1f, yz, 0f, zx, zy, 1f, 0f, 0f, 0f, 0f, 1f);
		}

		//public void Shear(float xy, float xz, float yx, float yz, float zx, float zy)
		//{
		//	this *= new Matrix4(1f, xy, xz, 0f, yx, 1f, yz, 0f, zx, zy, 1f, 0f, 0f, 0f, 0f, 1f);
		//}

		public static Matrix4 Reflection(in Plane plane)
		{
			Plane p = Plane.Normalize(plane);
#if SIMD
			System.Numerics.Vector3 n = p.abc_;
			System.Numerics.Vector4 t = -2f*p.abcd_;
			return new Matrix4(new System.Numerics.Vector4(t.X*n, 0f) + System.Numerics.Vector4.UnitX,
				new System.Numerics.Vector4(t.Y*n, 0f) + System.Numerics.Vector4.UnitY,
				new System.Numerics.Vector4(t.Z*n, 0f) + System.Numerics.Vector4.UnitZ,
				new System.Numerics.Vector4(t.W*n, 1f));
#else
			return new Matrix4(-2f*p.a_*p.a_ + 1f, -2f*p.b_*p.a_, -2f*p.c_*p.a_, 0f,
				-2f*p.a_*p.b_, -2f*p.b_*p.b_ + 1f, -2f*p.c_*p.b_, 0f,
				-2f*p.a_*p.c_, -2f*p.b_*p.c_, -2f*p.c_*p.c_ + 1f, 0f,
				-2f*p.a_*p.d_, -2f*p.b_*p.d_, -2f*p.c_*p.d_, 1f);
#endif
		}

		public static Matrix4 Adjoint(in Matrix4 m)
		{
			return new Matrix4(new Matrix3(m.m11_, m.m21_, m.m31_, m.m12_, m.m22_, m.m32_, m.m13_, m.m23_, m.m33_).Determinant,
				-(new Matrix3(m.m01_, m.m21_, m.m31_, m.m02_, m.m22_, m.m32_, m.m03_, m.m23_, m.m33_).Determinant),
				 new Matrix3(m.m01_, m.m11_, m.m31_, m.m02_, m.m12_, m.m32_, m.m03_, m.m13_, m.m33_).Determinant,
				-(new Matrix3(m.m01_, m.m11_, m.m21_, m.m02_, m.m12_, m.m22_, m.m03_, m.m13_, m.m23_).Determinant),
				-(new Matrix3(m.m10_, m.m20_, m.m30_, m.m12_, m.m22_, m.m32_, m.m13_, m.m23_, m.m33_).Determinant),
				 new Matrix3(m.m00_, m.m20_, m.m30_, m.m02_, m.m22_, m.m32_, m.m03_, m.m23_, m.m33_).Determinant,
				-(new Matrix3(m.m00_, m.m10_, m.m30_, m.m02_, m.m12_, m.m32_, m.m03_, m.m13_, m.m33_).Determinant),
				 new Matrix3(m.m00_, m.m10_, m.m20_, m.m02_, m.m12_, m.m22_, m.m03_, m.m13_, m.m23_).Determinant,
				 new Matrix3(m.m10_, m.m20_, m.m30_, m.m11_, m.m21_, m.m31_, m.m13_, m.m23_, m.m33_).Determinant,
				-(new Matrix3(m.m00_, m.m20_, m.m30_, m.m01_, m.m21_, m.m31_, m.m03_, m.m23_, m.m33_).Determinant),
				 new Matrix3(m.m00_, m.m10_, m.m30_, m.m01_, m.m11_, m.m31_, m.m03_, m.m13_, m.m33_).Determinant,
				-(new Matrix3(m.m00_, m.m10_, m.m20_, m.m01_, m.m11_, m.m21_, m.m03_, m.m13_, m.m23_).Determinant),
				-(new Matrix3(m.m10_, m.m20_, m.m30_, m.m11_, m.m21_, m.m31_, m.m12_, m.m22_, m.m32_).Determinant),
				 new Matrix3(m.m00_, m.m20_, m.m30_, m.m01_, m.m21_, m.m31_, m.m02_, m.m22_, m.m32_).Determinant,
				-(new Matrix3(m.m00_, m.m10_, m.m30_, m.m01_, m.m11_, m.m31_, m.m02_, m.m12_, m.m32_).Determinant),
				 new Matrix3(m.m00_, m.m10_, m.m20_, m.m01_, m.m11_, m.m21_, m.m02_, m.m12_, m.m22_).Determinant);
		}

		public static Matrix4 Inverse(in Matrix4 m)
		{
			float di = m.m00_;
			float d = 1f/di;
			float m00 = d;
			float m10 = m.m10_*(-d);
			float m20 = m.m20_*(-d);
			float m30 = m.m30_*(-d);
			float m01 = m.m01_*d;
			float m02 = m.m02_*d;
			float m03 = m.m03_*d;
			float m11 = m.m11_ + m10*m01*di;
			float m12 = m.m12_ + m10*m02*di;
			float m13 = m.m13_ + m10*m03*di;
			float m21 = m.m21_ + m20*m01*di;
			float m22 = m.m22_ + m20*m02*di;
			float m23 = m.m23_ + m20*m03*di;
			float m31 = m.m31_ + m30*m01*di;
			float m32 = m.m32_ + m30*m02*di;
			float m33 = m.m33_ + m30*m03*di;

			di = m11;
			d = 1f/di;
			m11 = d;
			m01 *= -d;
			m21 *= -d;
			m31 *= -d;
			m10 *= d;
			m12 *= d;
			m13 *= d;
			m00 += m01*m10*di;
			m02 += m01*m12*di;
			m03 += m01*m13*di;
			m20 += m21*m10*di;
			m22 += m21*m12*di;
			m23 += m21*m13*di;
			m30 += m31*m10*di;
			m32 += m31*m12*di;
			m33 += m31*m13*di;

			di = m22;
			d = 1f/di;
			m22 = d;
			m02 *= -d;
			m12 *= -d;
			m32 *= -d;
			m20 *= d;
			m21 *= d;
			m23 *= d;
			m00 += m02*m20*di;
			m01 += m02*m21*di;
			m03 += m02*m23*di;
			m10 += m12*m20*di;
			m11 += m12*m21*di;
			m13 += m12*m23*di;
			m30 += m32*m20*di;
			m31 += m32*m21*di;
			m33 += m32*m23*di;

			di = m33;
			d = 1f/di;
			m33 = d;
			m03 *= -d;
			m13 *= -d;
			m23 *= -d;
			m30 *= d;
			m31 *= d;
			m32 *= d;
			m00 += m03*m30*di;
			m01 += m03*m31*di;
			m02 += m03*m32*di;
			m10 += m13*m30*di;
			m11 += m13*m31*di;
			m12 += m13*m32*di;
			m20 += m23*m30*di;
			m21 += m23*m31*di;
			m22 += m23*m32*di;

			return new Matrix4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
		}

		public static Matrix4 InverseTranspose(in Matrix4 m)
		{
			float di = m.m00_;
			float d = 1f/di;
			float m00 = d;
			float m10 = m.m10_*(-d);
			float m20 = m.m20_*(-d);
			float m30 = m.m30_*(-d);
			float m01 = m.m01_*d;
			float m02 = m.m02_*d;
			float m03 = m.m03_*d;
			float m11 = m.m11_ + m10*m01*di;
			float m12 = m.m12_ + m10*m02*di;
			float m13 = m.m13_ + m10*m03*di;
			float m21 = m.m21_ + m20*m01*di;
			float m22 = m.m22_ + m20*m02*di;
			float m23 = m.m23_ + m20*m03*di;
			float m31 = m.m31_ + m30*m01*di;
			float m32 = m.m32_ + m30*m02*di;
			float m33 = m.m33_ + m30*m03*di;

			di = m11;
			d = 1f/di;
			m11 = d;
			m01 *= -d;
			m21 *= -d;
			m31 *= -d;
			m10 *= d;
			m12 *= d;
			m13 *= d;
			m00 += m01*m10*di;
			m02 += m01*m12*di;
			m03 += m01*m13*di;
			m20 += m21*m10*di;
			m22 += m21*m12*di;
			m23 += m21*m13*di;
			m30 += m31*m10*di;
			m32 += m31*m12*di;
			m33 += m31*m13*di;

			di = m22;
			d = 1f/di;
			m22 = d;
			m02 *= -d;
			m12 *= -d;
			m32 *= -d;
			m20 *= d;
			m21 *= d;
			m23 *= d;
			m00 += m02*m20*di;
			m01 += m02*m21*di;
			m03 += m02*m23*di;
			m10 += m12*m20*di;
			m11 += m12*m21*di;
			m13 += m12*m23*di;
			m30 += m32*m20*di;
			m31 += m32*m21*di;
			m33 += m32*m23*di;

			di = m33;
			d = 1f/di;
			m33 = d;
			m03 *= -d;
			m13 *= -d;
			m23 *= -d;
			m30 *= d;
			m31 *= d;
			m32 *= d;
			m00 += m03*m30*di;
			m01 += m03*m31*di;
			m02 += m03*m32*di;
			m10 += m13*m30*di;
			m11 += m13*m31*di;
			m12 += m13*m32*di;
			m20 += m23*m30*di;
			m21 += m23*m31*di;
			m22 += m23*m32*di;

			return new Matrix4(m00, m10, m20, m30, m01, m11, m21, m31, m02, m12, m22, m32, m03, m13, m23, m33);
		}

		public void Invert()
		{
			float di = m00_;
			float d = 1f/di;
			float m00 = d;
			float m10 = m10_*(-d);
			float m20 = m20_*(-d);
			float m30 = m30_*(-d);
			float m01 = m01_*d;
			float m02 = m02_*d;
			float m03 = m03_*d;
			float m11 = m11_ + m10*m01*di;
			float m12 = m12_ + m10*m02*di;
			float m13 = m13_ + m10*m03*di;
			float m21 = m21_ + m20*m01*di;
			float m22 = m22_ + m20*m02*di;
			float m23 = m23_ + m20*m03*di;
			float m31 = m31_ + m30*m01*di;
			float m32 = m32_ + m30*m02*di;
			float m33 = m33_ + m30*m03*di;

			di = m11;
			d = 1f/di;
			m11 = d;
			m01 *= -d;
			m21 *= -d;
			m31 *= -d;
			m10 *= d;
			m12 *= d;
			m13 *= d;
			m00 += m01*m10*di;
			m02 += m01*m12*di;
			m03 += m01*m13*di;
			m20 += m21*m10*di;
			m22 += m21*m12*di;
			m23 += m21*m13*di;
			m30 += m31*m10*di;
			m32 += m31*m12*di;
			m33 += m31*m13*di;

			di = m22;
			d = 1f/di;
			m22 = d;
			m02 *= -d;
			m12 *= -d;
			m32 *= -d;
			m20 *= d;
			m21 *= d;
			m23 *= d;
			m00 += m02*m20*di;
			m01 += m02*m21*di;
			m03 += m02*m23*di;
			m10 += m12*m20*di;
			m11 += m12*m21*di;
			m13 += m12*m23*di;
			m30 += m32*m20*di;
			m31 += m32*m21*di;
			m33 += m32*m23*di;

			di = m33;
			d = 1f/di;
			m33 = d;
			m03 *= -d;
			m13 *= -d;
			m23 *= -d;
			m30 *= d;
			m31 *= d;
			m32 *= d;
			m00 += m03*m30*di;
			m01 += m03*m31*di;
			m02 += m03*m32*di;
			m10 += m13*m30*di;
			m11 += m13*m31*di;
			m12 += m13*m32*di;
			m20 += m23*m30*di;
			m21 += m23*m31*di;
			m22 += m23*m32*di;

#if SIMD
			row0_ = new System.Numerics.Vector4(m00, m01, m02, m03);
			row1_ = new System.Numerics.Vector4(m10, m11, m12, m13);
			row2_ = new System.Numerics.Vector4(m20, m21, m22, m23);
			row3_ = new System.Numerics.Vector4(m30, m31, m32, m33);
#else
			m00_ = m00; m01_ = m01; m02_ = m02; m03_ = m03;
			m10_ = m10; m11_ = m11; m12_ = m12; m13_ = m13;
			m20_ = m20; m21_ = m21; m22_ = m22; m23_ = m23;
			m30_ = m30; m31_ = m31; m32_ = m32; m33_ = m33;
#endif
		}

		[Browsable(false)]
		public readonly float Trace => m00_ + m11_ + m22_ + m33_;

		[Browsable(false)]
		public readonly float Determinant
		{
		    get
			{
				float q = new Matrix3(m11_, m21_, m31_, m12_, m22_, m32_, m13_, m23_, m33_).Determinant;
				float r = new Matrix3(m10_, m20_, m30_, m12_, m22_, m32_, m13_, m23_, m33_).Determinant;
				float s = new Matrix3(m10_, m20_, m30_, m11_, m21_, m31_, m13_, m23_, m33_).Determinant;
				float t = new Matrix3(m10_, m20_, m30_, m11_, m21_, m31_, m12_, m22_, m32_).Determinant;
				return m00_*q - m01_*r + m02_*s - m03_*t;
			}
		}

#if SIMD
		internal readonly float m00_ => row0_.X;
		internal readonly float m01_ => row0_.Y;
		internal readonly float m02_ => row0_.Z;
		internal readonly float m03_ => row0_.W;
		internal readonly float m10_ => row1_.X;
		internal readonly float m11_ => row1_.Y;
		internal readonly float m12_ => row1_.Z;
		internal readonly float m13_ => row1_.W;
		internal readonly float m20_ => row2_.X;
		internal readonly float m21_ => row2_.Y;
		internal readonly float m22_ => row2_.Z;
		internal readonly float m23_ => row2_.W;
		internal readonly float m30_ => row3_.X;
		internal readonly float m31_ => row3_.Y;
		internal readonly float m32_ => row3_.Z;
		internal readonly float m33_ => row3_.W;
		internal readonly System.Numerics.Vector4 column0_ => new System.Numerics.Vector4(row0_.X, row1_.X, row2_.X, row3_.X);
		internal readonly System.Numerics.Vector4 column1_ => new System.Numerics.Vector4(row0_.Y, row1_.Y, row2_.Y, row3_.Y);
		internal readonly System.Numerics.Vector4 column2_ => new System.Numerics.Vector4(row0_.Z, row1_.Z, row2_.Z, row3_.Z);
		internal readonly System.Numerics.Vector4 column3_ => new System.Numerics.Vector4(row0_.W, row1_.W, row2_.W, row3_.W);

		internal System.Numerics.Vector4 row0_;
		internal System.Numerics.Vector4 row1_;
		internal System.Numerics.Vector4 row2_;
		internal System.Numerics.Vector4 row3_;
#else
		internal float m00_, m01_, m02_, m03_;
		internal float m10_, m11_, m12_, m13_;
		internal float m20_, m21_, m22_, m23_;
		internal float m30_, m31_, m32_, m33_;
#endif
	}
}
