/*
 *  Name: Vector3
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
	[StructLayout(LayoutKind.Sequential, Size = 16)]
#endif
	[Serializable]
	[TypeConverter(typeof(Vector3Converter))]
	public struct Vector3 : ISerializable, IFormattable, IEquatable<Vector3>
	{
#if SIMD
		public static readonly Vector3 Zero = new Vector3(System.Numerics.Vector3.Zero);
		public static readonly Vector3 UnitX = new Vector3(System.Numerics.Vector3.UnitX);
		public static readonly Vector3 UnitY = new Vector3(System.Numerics.Vector3.UnitY);
		public static readonly Vector3 UnitZ = new Vector3(System.Numerics.Vector3.UnitZ);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3(float scalar)
		{
			xyz_ = new System.Numerics.Vector3(scalar);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3(float x, float y, float z)
		{
			xyz_ = new System.Numerics.Vector3(x, y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3(Vector2 v, float z)
		{
			xyz_ = new System.Numerics.Vector3(v.xy_, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3(float[] v)
		{
			xyz_ = new System.Numerics.Vector3(v[0], v[1], v[2]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Vector3(System.Numerics.Vector3 v)
		{
			xyz_ = v;
		}

		private Vector3(SerializationInfo info, StreamingContext context)
		{
			xyz_ = new System.Numerics.Vector3(info.GetSingle("X"), info.GetSingle("Y"), info.GetSingle("Z"));
		}

		public static implicit operator Vector3(Vector2 v)
		{
			return new Vector3(new System.Numerics.Vector3(v.xy_, 0f));
		}

		public float X
		{
			readonly get => xyz_.X;
			set => xyz_.X = value;
		}

		public float Y
		{
			readonly get => xyz_.Y;
			set => xyz_.Y = value;
		}

		public float Z
		{
			readonly get => xyz_.Z;
			set => xyz_.Z = value;
		}

		[Browsable(false)]
		public Vector2 XY
		{
			readonly get => new Vector2(xy_);
			set => xyz_ = new System.Numerics.Vector3(value.xy_, xyz_.Z);
		}

		[Browsable(false)]
		public readonly Vector2 XZ => new Vector2(xz_);

		[Browsable(false)]
		public readonly Vector2 ZY => new Vector2(zy_);

		[Browsable(false)]
		public readonly Vector3 YZX => new Vector3(yzx_);

		[Browsable(false)]
		public readonly Vector3 ZXY => new Vector3(zxy_);

		[Browsable(false)]
		public float Magnitude
		{
			readonly get => xyz_.Length();
			set
			{
				float m = xyz_.Length();
				if (m > 0f)
					this *= value/m;
			}
		}

		[Browsable(false)]
		public readonly float MagnitudeSquared => xyz_.LengthSquared();

		[Browsable(false)]
		public float Length
		{
			readonly get => xyz_.Length();
			set
			{
				float m = xyz_.Length();
				if (m > 0f)
					this *= value/m;
			}
		}

		[Browsable(false)]
		public readonly float LengthSquared => xyz_.LengthSquared();

		public readonly override bool Equals(object other)
		{
			return (other is Vector3 rhs) && (xyz_ == rhs.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Vector3 other)
		{
			return (xyz_ == other.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector3 lhs, Vector3 rhs)
		{
			return (lhs.xyz_ == rhs.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector3 lhs, Vector3 rhs)
		{
			return (lhs.xyz_ != rhs.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator +(Vector3 v)
		{
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator -(Vector3 v)
		{
			return new Vector3(-v.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator +(Vector3 c, Vector3 d)
		{
			return new Vector3(c.xyz_ + d.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator -(Vector3 c, Vector3 d)
		{
			return new Vector3(c.xyz_ - d.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator *(Vector3 c, Vector3 d)
		{
			return new Vector3(c.xyz_*d.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator *(Vector3 c, float f)
		{
			return new Vector3(c.xyz_*f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator *(float f, Vector3 c)
		{
			return new Vector3(f*c.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator *(Vector3 v, in Matrix3 m)
		{
			//return new Vector3(System.Numerics.Vector3.Dot(v.xyz_, m.column0_),
			//	System.Numerics.Vector3.Dot(v.xyz_, m.column1_),
			//	System.Numerics.Vector3.Dot(v.xyz_, m.column2_));

			return new Vector3(v.xyz_.X*m.row0_ + v.xyz_.Y*m.row1_ + v.xyz_.Z*m.row2_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator /(Vector3 c, Vector3 d)
		{
			return new Vector3(c.xyz_/d.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator /(Vector3 c, float f)
		{
			return new Vector3(c.xyz_/f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 operator /(float f, Vector3 c)
		{
			return new Vector3(new System.Numerics.Vector3(f)/c.xyz_);
		}

		public float this[int index] // #TODO Use System.Numerics.Vector3 indexing operator
		{
			readonly get
			{
				switch (index)
				{
					case 0:
						return xyz_.X;
					case 1:
						return xyz_.Y;
					case 2:
						return xyz_.Z;
					default:
						throw new IndexOutOfRangeException();
				}
			}

			set
			{
				switch (index)
				{
					case 0:
						xyz_.X = value;
						break;
					case 1:
						xyz_.Y = value;
						break;
					case 2:
						xyz_.Z = value;
						break;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Abs(Vector3 v)
		{
			return new Vector3(System.Numerics.Vector3.Abs(v.xyz_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Vector3 u, Vector3 v)
		{
			return System.Numerics.Vector3.Dot(u.xyz_, v.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Cross(Vector3 u, Vector3 v)
		{
			return new Vector3(System.Numerics.Vector3.Cross(u.xyz_, v.xyz_));
		}

		public static Matrix3 Tensor(Vector3 u, Vector3 v)
		{
			return new Matrix3(u.xyz_.X*v.xyz_, u.xyz_.Y*v.xyz_, u.xyz_.Z*v.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(Vector3 u, Vector3 v)
		{
			return System.Numerics.Vector3.Distance(u.xyz_, v.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DistanceSquared(Vector3 u, Vector3 v)
		{
			return System.Numerics.Vector3.DistanceSquared(u.xyz_, v.xyz_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Min(Vector3 c, Vector3 d)
		{
			return new Vector3(System.Numerics.Vector3.Min(c.xyz_, d.xyz_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Max(Vector3 c, Vector3 d)
		{
			return new Vector3(System.Numerics.Vector3.Max(c.xyz_, d.xyz_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Clamp(Vector3 c, Vector3 low, Vector3 high)
		{
			return new Vector3(System.Numerics.Vector3.Clamp(c.xyz_, low.xyz_, high.xyz_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Lerp(Vector3 c, Vector3 d, float t)
		{
			return new Vector3(System.Numerics.Vector3.Lerp(c.xyz_, d.xyz_, t));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Normalize(Vector3 v)
		{
			float m = v.xyz_.Length();
			return (m > 0f) ? new Vector3(v.xyz_/m) : v;
		}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static Vector3 Scale(Vector3 v, Vector3 s)
		//{
		//	return new Vector3(v.xyz_*s.xyz_);
		//}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Transform(Vector3 v, in Matrix3 m)
		{
			//return new Vector3(System.Numerics.Vector3.Dot(v.xyz_, m.column0_),
			//	System.Numerics.Vector3.Dot(v.xyz_, m.column1_),
			//	System.Numerics.Vector3.Dot(v.xyz_, m.column2_));

			return new Vector3(v.xyz_.X*m.row0_ + v.xyz_.Y*m.row1_ + v.xyz_.Z*m.row2_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 Transform(Vector3 v, in AffineTransform t)
		{
			//return new Vector3(System.Numerics.Vector4.Dot(new System.Numerics.Vector4(v.xyz_, 1f), t.column0_),
			//	System.Numerics.Vector4.Dot(new System.Numerics.Vector4(v.xyz_, 1f), t.column1_),
			//	System.Numerics.Vector4.Dot(new System.Numerics.Vector4(v.xyz_, 1f), t.column2_));

			return new Vector3(v.xyz_.X*t.row0_ + v.xyz_.Y*t.row1_ + v.xyz_.Z*t.row2_ + t.row3_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Normalize()
		{
			float m = xyz_.Length();
			if (m > 0f)
				xyz_ /= m;
		}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public void Scale(Vector3 v)
		//{
		//	xyz_ *= v.xyz_;
		//}

		public static Vector3 Rotate(Vector3 v, Quaternion q)
		{
			System.Numerics.Vector3 qyzx = q.yzx_;
			System.Numerics.Vector3 qzxy = q.zxy_;
			System.Numerics.Vector3 t1 = qyzx*v.zxy_ - qzxy*v.yzx_;
			System.Numerics.Vector3 t2 = q.www_*t1 + qyzx*t1.ZXY() - qzxy*t1.YZX();
			return new Vector3(v.xyz_ + 2f*t2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Transform(in Matrix3 m)
		{
			//xyz_ = new System.Numerics.Vector3(System.Numerics.Vector3.Dot(xyz_, m.column0_),
			//	System.Numerics.Vector3.Dot(xyz_, m.column1_),
			//	System.Numerics.Vector3.Dot(xyz_, m.column2_));

			xyz_ = xyz_.X*m.row0_ + xyz_.Y*m.row1_ + xyz_.Z*m.row2_;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Transform(in AffineTransform t)
		{
			//xyz_ = new System.Numerics.Vector3(System.Numerics.Vector4.Dot(new System.Numerics.Vector4(xyz_, 1f), t.column0_),
			//	System.Numerics.Vector4.Dot(new System.Numerics.Vector4(xyz_, 1f), t.column1_),
			//	System.Numerics.Vector4.Dot(new System.Numerics.Vector4(xyz_, 1f), t.column2_));

			xyz_ = xyz_.X*t.row0_ + xyz_.Y*t.row1_ + xyz_.Z*t.row2_ + t.row3_;
		}

		internal readonly float x_ => xyz_.X;
		internal readonly float y_ => xyz_.Y;
		internal readonly float z_ => xyz_.Z;
		internal readonly System.Numerics.Vector2 xy_ => new System.Numerics.Vector2(xyz_.X, xyz_.Y);
		internal readonly System.Numerics.Vector2 xz_ => new System.Numerics.Vector2(xyz_.X, xyz_.Z);
		internal readonly System.Numerics.Vector2 zy_ => new System.Numerics.Vector2(xyz_.Z, xyz_.Y);
		internal readonly System.Numerics.Vector3 zyx_ => new System.Numerics.Vector3(xyz_.Z, xyz_.Y, xyz_.X);
		internal readonly System.Numerics.Vector3 yzx_ => new System.Numerics.Vector3(xyz_.Y, xyz_.Z, xyz_.X);
		internal readonly System.Numerics.Vector3 zxy_ => new System.Numerics.Vector3(xyz_.Z, xyz_.X, xyz_.Y);
		internal readonly System.Numerics.Vector4 xyzx_ => new System.Numerics.Vector4(xyz_, xyz_.X);
		internal readonly System.Numerics.Vector4 xyzmx_ => new System.Numerics.Vector4(xyz_, -xyz_.X);

		internal System.Numerics.Vector3 xyz_;
#else
		public static readonly Vector3 Zero = new Vector3(0f, 0f, 0f);
		public static readonly Vector3 UnitX = new Vector3(1f, 0f, 0f);
		public static readonly Vector3 UnitY = new Vector3(0f, 1f, 0f);
		public static readonly Vector3 UnitZ = new Vector3(0f, 0f, 1f);

		public Vector3(float scalar)
		{
			x_ = scalar;
			y_ = scalar;
			z_ = scalar;
		}

		public Vector3(float x, float y, float z)
		{
			x_ = x;
			y_ = y;
			z_ = z;
		}

		public Vector3(Vector2 v, float z)
		{
			x_ = v.x_;
			y_ = v.y_;
			z_ = z;
		}

		public Vector3(float[] v)
		{
			x_ = v[0];
			y_ = v[1];
			z_ = v[2];
		}

		private Vector3(SerializationInfo info, StreamingContext context)
		{
			x_ = info.GetSingle("X");
			y_ = info.GetSingle("Y");
			z_ = info.GetSingle("Z");
		}

		public static implicit operator Vector3(Vector2 v)
		{
			return new Vector3(v.x_, v.y_, 0f);
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

		[Browsable(false)]
		public Vector2 XY
		{
			readonly get => new Vector2(x_, y_);
			set 
			{ 
				x_ = value.x_; 
				y_ = value.y_; 
			}
		}

		[Browsable(false)]
		public readonly Vector2 XZ => new Vector2(x_, z_);

		[Browsable(false)]
		public readonly Vector2 ZY => new Vector2(z_, y_);

		[Browsable(false)]
		public readonly Vector3 YZX => new Vector3(y_, z_, x_);

		[Browsable(false)]
		public readonly Vector3 ZXY => new Vector3(z_, x_, y_);

		[Browsable(false)]
		public float Magnitude
		{
			readonly get => MathF.Sqrt(x_*x_ + y_*y_ + z_*z_);
			set
			{
				float m = MathF.Sqrt(x_*x_ + y_*y_ + z_*z_);
				if (m > 0f)
					this *= value/m;
			}
		}

		[Browsable(false)]
		public readonly float MagnitudeSquared => x_*x_ + y_*y_ + z_*z_;

		[Browsable(false)]
		public float Length
		{
			readonly get => MathF.Sqrt(x_*x_ + y_*y_ + z_*z_);
			set
			{
				float m = MathF.Sqrt(x_*x_ + y_*y_ + z_*z_);
				if (m > 0f)
					this *= value/m;
			}
		}

		[Browsable(false)]
		public readonly float LengthSquared => x_*x_ + y_*y_ + z_*z_;

		public readonly override bool Equals(object other)
		{
			if (other is Vector3 rhs)
				return (x_ == rhs.x_) && (y_ == rhs.y_) && (z_ == rhs.z_);
			
			return false;
		}

		public readonly bool Equals(Vector3 other)
		{
			return (x_ == other.x_) && (y_ == other.y_) && (z_ == other.z_);
		}
		
		public static bool operator ==(Vector3 lhs, Vector3 rhs)
		{
			return (lhs.x_ == rhs.x_) && (lhs.y_ == rhs.y_) && (lhs.z_ == rhs.z_);
		}

		public static bool operator !=(Vector3 lhs, Vector3 rhs)
		{
			return (lhs.x_ != rhs.x_) || (lhs.y_ != rhs.y_) || (lhs.z_ != rhs.z_);
		}

		public static Vector3 operator +(Vector3 v)
		{
			return v;
		}

		public static Vector3 operator -(Vector3 v)
		{
			return new Vector3(-v.x_, -v.y_, -v.z_);
		}

		public static Vector3 operator +(Vector3 u, Vector3 v)
		{
			return new Vector3(u.x_ + v.x_, u.y_ + v.y_, u.z_ + v.z_);
		}

		public static Vector3 operator -(Vector3 u, Vector3 v)
		{
			return new Vector3(u.x_ - v.x_, u.y_ - v.y_, u.z_ - v.z_);
		}

		public static Vector3 operator *(Vector3 u, Vector3 v)
		{
			return new Vector3(u.x_*v.x_, u.y_*v.y_, u.z_*v.z_);
		}

		public static Vector3 operator *(Vector3 v, float f)
		{
			return new Vector3(v.x_*f, v.y_*f, v.z_*f);
		}

		public static Vector3 operator *(float f, Vector3 v)
		{
			return new Vector3(f*v.x_, f*v.y_, f*v.z_);
		}

		public static Vector3 operator *(Vector3 v, in Matrix3 m)
		{
			return new Vector3(v.x_*m.m00_ + v.y_*m.m10_ + v.z_*m.m20_,
				v.x_*m.m01_ + v.y_*m.m11_ + v.z_*m.m21_,
				v.x_*m.m02_ + v.y_*m.m12_ + v.z_*m.m22_);
		}

		//public static Vector3 operator *(Matrix3 m, Vector3 v)
		//{
		//    return new Vector3(m.m00_*v.x_ + m.m01_*v.y_ + m.m02_*v.z_,
		//		m.m10_*v.x_ + m.m11_*v.y_ + m.m12_*v.z_,
		//		m.m20_*v.x_ + m.m21_*v.y_ + m.m22_*v.z_);
		//}

		public static Vector3 operator /(Vector3 u, Vector3 v)
		{
			return new Vector3(u.x_/v.x_, u.y_/v.y_, u.z_/v.z_);
		}

		public static Vector3 operator /(Vector3 v, float f)
		{
			return new Vector3(v.x_/f, v.y_/f, v.z_/f);
		}

		public static Vector3 operator /(float f, Vector3 v)
		{
			return new Vector3(f/v.x_, f/v.y_, f/v.z_);
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
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		public static Vector3 Abs(Vector3 v)
		{
			return new Vector3(MathF.Abs(v.x_), MathF.Abs(v.y_), MathF.Abs(v.z_));
		}

		public static float Dot(Vector3 u, Vector3 v)
		{
			return (u.x_*v.x_ + u.y_*v.y_ + u.z_*v.z_);
		}

		public static Vector3 Cross(Vector3 u, Vector3 v)
		{
			return new Vector3(u.y_*v.z_ - u.z_*v.y_, u.z_*v.x_ - u.x_*v.z_, u.x_*v.y_ - u.y_*v.x_);
		}

		public static Matrix3 Tensor(Vector3 u, Vector3 v)
		{
			return new Matrix3(u.x_*v.x_, u.x_*v.y_, u.x_*v.z_, u.y_*v.x_, u.y_*v.y_, u.y_*v.z_, u.z_*v.x_, u.z_*v.y_, u.z_*v.z_);
		}

		public static float Distance(Vector3 u, Vector3 v)
		{
			return (v - u).Magnitude;
		}

		public static float DistanceSquared(Vector3 u, Vector3 v)
		{
			return (v - u).MagnitudeSquared;
		}

		public static Vector3 Min(Vector3 u, Vector3 v)
		{
			return new Vector3(Math.Min(u.x_, v.x_), Math.Min(u.y_, v.y_), Math.Min(u.z_, v.z_));
		}

		public static Vector3 Max(Vector3 u, Vector3 v)
		{
			return new Vector3(Math.Max(u.x_, v.x_), Math.Max(u.y_, v.y_), Math.Max(u.z_, v.z_));
		}

		public static Vector3 Clamp(Vector3 v, Vector3 low, Vector3 high)
		{
			return new Vector3(Math.Clamp(v.x_, low.x_, high.x_), Math.Clamp(v.y_, low.y_, high.y_), 
				Math.Clamp(v.z_, low.z_, high.z_));
		}

		public static Vector3 Lerp(Vector3 u, Vector3 v, float t)
		{
			return new Vector3(u.x_ + t*(v.x_ - u.x_), u.y_ + t*(v.y_ - u.y_), u.z_ + t*(v.z_ - u.z_));
		}

		public static Vector3 /*Unit*/Normalize(Vector3 v)
		{
			float m = v.Magnitude;
			return (m > 0f) ? new Vector3(v.x_/m, v.y_/m, v.z_/m) : v;
		}

		//public static Vector3 Scale(Vector3 v, Vector3 s)
		//{
		//	return new Vector3(v.x_*s.x_, v.y_*s.y_, v.z_*s.z_);
		//}

		public static Vector3 Rotate(Vector3 v, Quaternion q)
		{
			float x1 = q.y_*v.z_ - q.z_*v.y_;
			float y1 = q.z_*v.x_ - q.x_*v.z_;
			float z1 = q.x_*v.y_ - q.y_*v.x_;
			float x2 = q.w_*x1 + q.y_*z1 - q.z_*y1;
			float y2 = q.w_*y1 + q.z_*x1 - q.x_*z1;
			float z2 = q.w_*z1 + q.x_*y1 - q.y_*x1;
			return new Vector3(v.x_ + 2f*x2, v.y_ + 2f*y2, v.z_ + 2f*z2);
		}

		public static Vector3 Transform(Vector3 v, in Matrix3 m)
		{
			return new Vector3(v.x_*m.m00_ + v.y_*m.m10_ + v.z_*m.m20_,
				v.x_*m.m01_ + v.y_*m.m11_ + v.z_*m.m21_,
				v.x_*m.m02_ + v.y_*m.m12_ + v.z_*m.m22_);
		}

        public static Vector3 Transform(Vector3 v, in AffineTransform t)
        {
            return new Vector3(v.x_*t.r_.m00_ + v.y_*t.r_.m10_ + v.z_*t.r_.m20_ + t.t_.x_,
				v.x_*t.r_.m01_ + v.y_*t.r_.m11_ + v.z_*t.r_.m21_ + t.t_.y_,
				v.x_*t.r_.m02_ + v.y_*t.r_.m12_ + v.z_*t.r_.m22_ + t.t_.z_);
        }

		public void Normalize()
		{
			float m = Magnitude;
			if (m > 0f)
			{
				x_ /= m;
				y_ /= m;
				z_ /= m;
			}
		}

		//public void Scale(Vector3 v)
		//{
		//	x_ *= v.x_;
		//	y_ *= v.y_;
		//	z_ *= v.z_;
		//}

		public void Transform(in Matrix3 m)
		{
			float x = x_, y = y_, z = z_;
			x_ = x*m.m00_ + y*m.m10_ + z*m.m20_;
			y_ = x*m.m01_ + y*m.m11_ + z*m.m21_;
			z_ = x*m.m02_ + y*m.m12_ + z*m.m22_;
		}

		public void Transform(in AffineTransform t)
		{
			float x = x_, y = y_, z = z_;
			x_ = x*t.r_.m00_ + y*t.r_.m10_ + z*t.r_.m20_ + t.t_.x_;
			y_ = x*t.r_.m01_ + y*t.r_.m11_ + z*t.r_.m21_ + t.t_.y_;
			z_ = x*t.r_.m02_ + y*t.r_.m12_ + z*t.r_.m22_ + t.t_.z_;
		}

		internal float x_;
		internal float y_;
		internal float z_;
#endif

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("X", x_);
			info.AddValue("Y", y_);
			info.AddValue("Z", z_);
		}

		public static explicit operator Vector3(IntVector3 v)
		{
			return new Vector3((float)v.x_, (float)v.y_, (float)v.z_);
		}

		[Browsable(false)]
		public readonly bool IsFinite => Functions.IsFinite(x_) && Functions.IsFinite(y_) && Functions.IsFinite(z_);

		[Browsable(false)]
		public readonly Axis MajorAxis
		{
			get
			{
				Axis axis = Axis.X;
				if (MathF.Abs(y_) > MathF.Abs(x_))
					axis = Axis.Y;
				if (MathF.Abs(z_) > MathF.Abs((axis == Axis.X) ? x_ : y_))
					axis = Axis.Z;
				return axis;
			}
		}

		public readonly override int GetHashCode()
		{
			//return HashCode.Combine(x_, y_, z_);
			int hash = x_.GetHashCode();
			hash = ((hash << 5) + hash) ^ y_.GetHashCode();
			return ((hash << 5) + hash) ^ z_.GetHashCode();
		}

		public readonly bool ApproxEquals(Vector3 v, float tolerance) // #TODO SIMD
		{
			return (Math.Abs(v.x_ - x_) < tolerance) &&
				(Math.Abs(v.y_ - y_) < tolerance) &&
				(Math.Abs(v.z_ - z_) < tolerance);
		}

		public readonly bool ApproxEquals(Vector3 v)
		{
			return ApproxEquals(v, 1e-6f);
		}

		public readonly bool AllLessThan(Vector3 v)
		{
			return (x_ < v.x_) && (y_ < v.y_) && (z_ < v.z_);
		}

		public readonly bool AllLessThanEqual(Vector3 v)
		{
			return (x_ <= v.x_) && (y_ <= v.y_) && (z_ <= v.z_);
		}

		public readonly bool AllGreaterThan(Vector3 v)
		{
			return (x_ > v.x_) && (y_ > v.y_) && (z_ > v.z_);
		}

		public readonly bool AllGreaterThanEqual(Vector3 v)
		{
			return (x_ >= v.x_) && (y_ >= v.y_) && (z_ >= v.z_);
		}

		public readonly bool AnyLessThan(Vector3 v)
		{
			return (x_ < v.x_) || (y_ < v.y_) || (z_ < v.z_);
		}

		public readonly bool AnyLessThanEqual(Vector3 v)
		{
			return (x_ <= v.x_) || (y_ <= v.y_) || (z_ <= v.z_);
		}

		public readonly bool AnyGreaterThan(Vector3 v)
		{
			return (x_ > v.x_) || (y_ > v.y_) || (z_ > v.z_);
		}

		public readonly bool AnyGreaterThanEqual(Vector3 v)
		{
			return (x_ >= v.x_) || (y_ >= v.y_) || (z_ >= v.z_);
		}

		public readonly float[] ToArray()
		{
			return new float[3] { x_, y_, z_ };
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1} {2}", x_, y_, z_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2}", x_, y_, z_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1} {2}",
				x_.ToString(format), y_.ToString(format), z_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2}",
				x_.ToString(format, provider), y_.ToString(format, provider), z_.ToString(format, provider));
		}

		public static Vector3 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 3)
				throw new FormatException();

			return new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]));
		}

		public static Vector3 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 3)
				throw new FormatException();

			return new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider));
		}

		public static Vector3 FromAxis(Axis axis)
		{
			return new Vector3((axis == Axis.X) ? 1f : 0f, (axis == Axis.Y) ? 1f : 0f, (axis == Axis.Z) ? 1f : 0f);
		}

		public static float Angle(Vector3 u, Vector3 v)
		{
			float q = u.Magnitude*v.Magnitude;
			if (q > 0f)
				return (float)Math.Acos(Math.Clamp(Vector3.Dot(u, v)/q, -1f, 1f));
			else
				return (Vector3.Dot(u, v) >= 0f) ? 0f : SingleConstants.Pi;
			
			//u.Normalize();
			//v.Normalize();
			//return (float)Math.Acos(Math.Clamp(Vector3.Dot(u, v), -1f, 1f));
		}

		public static Vector3 Slerp(Vector3 u, Vector3 v, float t)
		{
			float dp = Vector3.Dot(u, v);
			if ((1f - dp) < 1e-6f)
				return Vector3.Normalize(u + t*(v - u));

			dp = Math.Clamp(dp, -1f, 1f);
			float theta = (float)Math.Acos(dp)*t;
			Vector3 c = Vector3.Normalize(v - u*dp);
			return (u*(float)Math.Cos(theta) + c*(float)Math.Sin(theta));
		}

		/// <summary>
		/// Clamps the length of a given vector to <paramref name="maxLength"/>.
		/// </summary>
		/// <remarks>
		/// If the vector is shorter its value is returned unaltered, if the vector 
		/// is longer the value returned has length of maxLength and is parallel 
		/// to the original input.
		/// </remarks>
		/// <param name="v"></param>
		/// <param name="maxLength"></param>
		/// <returns></returns>
		public static Vector3 TruncateLength(Vector3 v, float maxLength)
		{
			float maxLengthSquared = maxLength*maxLength;
			float vecLengthSquared = v.LengthSquared;
			return (vecLengthSquared <= maxLengthSquared) ? v : v*(maxLength/MathF.Sqrt(vecLengthSquared));
		}

		public static Vector3 Rotate(Vector3 v, Axis axis, float angle)
		{
			if (angle != 0f)
			{
				float sine = (float)Math.Sin(angle);
				float cosine = (float)Math.Cos(angle);

				switch (axis)
				{
					case Axis.X:
					{
						float vy = v.y_, vz = v.z_;
						return new Vector3(v.x_, vy*cosine - vz*sine, vz*cosine + vy*sine);
					}

					case Axis.Y:
					{
						float vx = v.x_, vz = v.z_;
						return new Vector3(vx*cosine + vz*sine, v.y_, vz*cosine - vx*sine);
					}

					case Axis.Z:
					{
						float vx = v.x_, vy = v.y_;
						return new Vector3(vx*cosine - vy*sine, vy*cosine + vx*sine, v.z_);
					}
				}
			}

			return v;
		}

		/// <summary>
		/// Returns a reflection vector.
		/// </summary>
		/// <param name="i">Entering ray direction (incident vector).</param>
		/// <param name="n">Normalized surface normal.</param>
		/// <returns>Reflected vector of length that is equal to the length of <paramref name="i"/>.</returns>
		public static Vector3 Reflect(Vector3 i, Vector3 n)
		{
			return (i - 2f*Vector3.Dot(i, n)*n);
		}

		/// <summary>
		/// Returns a refraction vector.
		/// </summary>
		/// <param name="i">Entering ray direction.</param>
		/// <param name="n">Normalized surface normal.</param>
		/// <param name="etaRatio">Ratio of the index of refraction in the medium containing
		///			   			   the incident ray to that of the medium being entered
		///						   (vacuum - 1.0, air - 1.0003, water - 1.3333,
		///						   glass - 1.5, plastic - 1.5, diamond - 2.417).</param>
		/// <returns>Refracted vector of length that is equal to the length of <paramref name="i"/>.</returns>
		public static Vector3 Refract(Vector3 i, Vector3 n, float etaRatio)
		{
			float cosI = Vector3.Dot(-i, n);
			float cosT2 = 1f - etaRatio*etaRatio*(1f - cosI*cosI);
			if (cosT2 <= 0f) 
				return Vector3.Zero;

			return etaRatio*i + ((etaRatio*cosI - MathF.Sqrt(Math.Abs(cosT2)))*n);
		}

		/// <summary>
		/// Projects vector <paramref name="a"/> onto vector <paramref name="u"/>.
		/// </summary>
		public static Vector3 Project(Vector3 a, Vector3 u)
		{
			float uSqM = u.MagnitudeSquared;
			return (uSqM > 0f) ? ((Vector3.Dot(a, u)/uSqM)*u) : Vector3.Zero;
		}

		///// <summary>
		///// Projects vector <paramref name="v"/> onto normalized plane <paramref name="p"/>.
		///// </summary>
		//public static Vector3 Project(Vector3 v, in Plane p)
		//{
		//	return v - Vector3.Dot(v, p.Normal)*p.Normal;
		//}

		/// <summary>
		/// Find a vector that is perpendicular to a vector.
		/// </summary>
		/// <remarks>
		/// Achieved by constructing a vector from the segment vector with the following properties:
		/// (Segment vector is any vector parallel to the line segment.)
		/// <list type="number">
		/// <item><description>Component with the smallest index is set to 0</description></item>
		/// <item><description>The remaing two component are copied into the new vector but are swapped in position</description></item>
		/// <item><description>One of the copied components is multiplied by -1.0 (has it's sign flipped)</description></item>
		/// </list>
		/// <para>leaving: (for example)</para>
		/// <para><c>segmentVector = (x, y, z)</c>, with let's say y as the smallest component</para>
		/// <para><c>segmentNormal = (-z, 0 , x)</c> or <c>(z, 0, -x)</c>, either will do.</para>
		/// <para>This will in fact be a normal as (in the example) the dot product will be zero.</para>
		/// <para>i.e. <c>x*-z + y*0 + z*x = 0</c></para>
		/// </remarks>
		public static Vector3 Perpendicular(Vector3 v)
		{
			int min = 0, a = 1, b = 2;

			if (Math.Abs(v.y_) < Math.Abs(v.x_))
			{
				a = min;
				min = 1;
			}

			if (Math.Abs(v.z_) < Math.Abs(v[min]))
			{
				b = min;
				min = 2;
			}

			Vector3 u = new Vector3();
			u[min] = 0f;
			u[a] = v[b];
			u[b] = -v[a];
			return u*(v.Magnitude/u.Magnitude);
		}

		/// <summary>
		/// Return component of vector parallel to a unit basis vector
		/// (assumes <paramref name="unitBasis"/> has unit magnitude).
		/// </summary>
		/// <param name="v"></param>
		/// <param name="unitBasis"></param>
		/// <returns></returns>
		public static Vector3 ParallelComponent(Vector3 v, Vector3 unitBasis)
		{
			float projection = Vector3.Dot(v, unitBasis);
			return unitBasis*projection;
		}

		/// <summary>
		/// Return component of vector perpendicular to a unit basis vector
		/// (assumes <paramref name="unitBasis"/> has unit magnitude).
		/// </summary>
		/// <param name="v"></param>
		/// <param name="unitBasis"></param>
		/// <returns></returns>
		public static Vector3 PerpendicularComponent(Vector3 v, Vector3 unitBasis)
		{
			return v - ParallelComponent(v, unitBasis);
		}

		/// <summary>
		/// Clamps the length of a given vector to <paramref name="maxLength"/>.
		/// </summary>
		/// <remarks>
		/// If the vector is shorter its value is leaved unaltered, if the vector 
		/// is longer its value has length of maxLength and is parallel to the
		/// original value.
		/// </remarks>
		/// <param name="maxLength"></param>
		/// <returns></returns>
		public void TruncateLength(float maxLength)
		{
			float maxLengthSquared = maxLength*maxLength;
			float vecLengthSquared = LengthSquared;
			if (vecLengthSquared > maxLengthSquared)
				this *= (maxLength/MathF.Sqrt(vecLengthSquared));
		}

		public void Rotate(Axis axis, float angle)
		{
			if (angle != 0f)
				this = Vector3.Rotate(this, axis, angle);
		}

		public void Rotate(Quaternion q)
		{
			this = Vector3.Rotate(this, q);
		}

		//public void Reflect(Vector3 n)
		//{
		//	this = Vector3.Reflect(this, n);
		//}

		//public void Refract(Vector3 n, float etaRatio)
		//{
		//	this = Vector3.Refract(this, n, etaRatio);
		//}

		//public void Project(Vector3 v)
		//{
		//	this = Vector3.Project(this, v);
		//}
	}
}
