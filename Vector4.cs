/*
 *  Name: Vector4
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	[Serializable]
	[TypeConverter(typeof(Vector4Converter))]
	public struct Vector4 : ISerializable, IFormattable, IEquatable<Vector4>
	{
#if SIMD
		public static readonly Vector4 Zero = new Vector4(System.Numerics.Vector4.Zero);
		public static readonly Vector4 UnitX = new Vector4(System.Numerics.Vector4.UnitX);
		public static readonly Vector4 UnitY = new Vector4(System.Numerics.Vector4.UnitY);
		public static readonly Vector4 UnitZ = new Vector4(System.Numerics.Vector4.UnitZ);
		public static readonly Vector4 UnitW = new Vector4(System.Numerics.Vector4.UnitW);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4(float scalar)
		{
			xyzw_ = new System.Numerics.Vector4(scalar);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4(float x, float y, float z, float w)
		{
			xyzw_ = new System.Numerics.Vector4(x, y, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4(Vector2 v, float z, float w)
		{
			xyzw_ = new System.Numerics.Vector4(v.xy_, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4(Vector2 xy, Vector2 zw)
		{
			xyzw_ = new System.Numerics.Vector4(xy.xy_, zw.xy_.X, zw.xy_.Y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4(Vector3 v, float w)
		{
			xyzw_ = new System.Numerics.Vector4(v.xyz_, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector4(float[] v)
		{
			xyzw_ = new System.Numerics.Vector4(v[0], v[1], v[2], v[3]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector4(System.Numerics.Vector4 v)
		{
			xyzw_ = v;
		}

		private Vector4(SerializationInfo info, StreamingContext context)
		{
			xyzw_ = new System.Numerics.Vector4(info.GetSingle("X"), info.GetSingle("Y"), info.GetSingle("Z"), info.GetSingle("W"));
		}

		public static implicit operator Vector4(Vector2 v)
		{
			return new Vector4(new System.Numerics.Vector4(v.xy_, 0f, 1f));
		}

		public static implicit operator Vector4(Vector3 v)
		{
			return new Vector4(new System.Numerics.Vector4(v.xyz_, 1f));
		}

		public float X 
		{ 
			readonly get { return xyzw_.X; } 
			set { xyzw_.X = value; } 
		}
		
		public float Y 
		{ 
			readonly get { return xyzw_.Y; } 
			set { xyzw_.Y = value; } 
		}
		
		public float Z 
		{ 
			readonly get { return xyzw_.Z; } 
			set { xyzw_.Z = value; } 
		}
		
		public float W 
		{ 
			readonly get { return xyzw_.W; } 
			set { xyzw_.W = value; } 
		}

		[Browsable(false)]
		public Vector2 XY
		{
			readonly get { return new Vector2(xyzw_.X, xyzw_.Y); }
			set { xyzw_ = new System.Numerics.Vector4(value.xy_, xyzw_.Z, xyzw_.W); }
		}

		[Browsable(false)]
		public Vector3 XYZ
		{
			readonly get { return new Vector3(xyzw_.X, xyzw_.Y, xyzw_.Z); }
			set { xyzw_ = new System.Numerics.Vector4(value.xyz_, xyzw_.W); }
		}

		[Browsable(false)]
		public float Magnitude
		{
			readonly get { return xyzw_.Length(); }
			set
			{
				float m = xyzw_.Length();
				if (m > 0f)
					this *= value/m;
			}
		}

		[Browsable(false)]
		public readonly float MagnitudeSquared
		{
			get { return xyzw_.LengthSquared(); }
		}

		[Browsable(false)]
		public float Length
		{
			readonly get { return xyzw_.Length(); }
			set
			{
				float m = xyzw_.Length();
				if (m > 0f)
					this *= value / m;
			}
		}

		[Browsable(false)]
		public readonly float LengthSquared
		{
			get { return xyzw_.LengthSquared(); }
		}

		public readonly override bool Equals(object other)
		{
			return (other is Vector4 rhs) && (xyzw_ == rhs.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Vector4 other)
		{
			return (xyzw_ == other.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector4 lhs, Vector4 rhs)
		{
			return (lhs.xyzw_ == rhs.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector4 lhs, Vector4 rhs)
		{
			return (lhs.xyzw_ != rhs.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator +(Vector4 v)
		{
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator -(Vector4 v)
		{
			return new Vector4(-v.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator +(Vector4 c, Vector4 d)
		{
			return new Vector4(c.xyzw_ + d.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator -(Vector4 c, Vector4 d)
		{
			return new Vector4(c.xyzw_ - d.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator *(Vector4 c, Vector4 d)
		{
			return new Vector4(c.xyzw_*d.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator *(Vector4 c, float f)
		{
			return new Vector4(c.xyzw_*f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator *(float f, Vector4 c)
		{
			return new Vector4(f*c.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator *(Vector4 v, in Matrix4 m)
		{
			return new Vector4(System.Numerics.Vector4.Dot(v.xyzw_, m.column0_),
				System.Numerics.Vector4.Dot(v.xyzw_, m.column1_),
				System.Numerics.Vector4.Dot(v.xyzw_, m.column2_),
				System.Numerics.Vector4.Dot(v.xyzw_, m.column3_));

			//System.Numerics.Vector4 vx = new System.Numerics.Vector4(v.xyzw_.X);
			//System.Numerics.Vector4 vy = new System.Numerics.Vector4(v.xyzw_.Y);
			//System.Numerics.Vector4 vz = new System.Numerics.Vector4(v.xyzw_.Z);
			//System.Numerics.Vector4 vw = new System.Numerics.Vector4(v.xyzw_.W);
			//return new Vector4(vx*m.row0_ + vy*m.row1_ + vz*m.row2_ + vw*m.row3_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator /(Vector4 c, Vector4 d)
		{
			return new Vector4(c.xyzw_/d.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator /(Vector4 c, float f)
		{
			return new Vector4(c.xyzw_/f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 operator /(float f, Vector4 c)
		{
			return new Vector4(new System.Numerics.Vector4(f)/c.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Abs(Vector4 v)
		{
			return new Vector4(System.Numerics.Vector4.Abs(v.xyzw_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Vector4 u, Vector4 v)
		{
			return System.Numerics.Vector4.Dot(u.xyzw_, v.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(Vector4 u, Vector4 v)
		{
			return System.Numerics.Vector4.Distance(u.xyzw_, v.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DistanceSquared(Vector4 u, Vector4 v)
		{
			return System.Numerics.Vector4.DistanceSquared(u.xyzw_, v.xyzw_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Min(Vector4 c, Vector4 d)
		{
			return new Vector4(System.Numerics.Vector4.Min(c.xyzw_, d.xyzw_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Max(Vector4 c, Vector4 d)
		{
			return new Vector4(System.Numerics.Vector4.Max(c.xyzw_, d.xyzw_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Clamp(Vector4 c, Vector4 low, Vector4 high)
		{
			return new Vector4(System.Numerics.Vector4.Clamp(c.xyzw_, low.xyzw_, high.xyzw_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Lerp(Vector4 c, Vector4 d, float t)
		{
			return new Vector4(System.Numerics.Vector4.Lerp(c.xyzw_, d.xyzw_, t));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Normalize(Vector4 v)
		{
			float m = v.xyzw_.Length();
			return (m > 0f) ? new Vector4(v.xyzw_/m) : v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector4 Transform(Vector4 v, in Matrix4 m)
		{
			return new Vector4(System.Numerics.Vector4.Dot(v.xyzw_, m.column0_),
				System.Numerics.Vector4.Dot(v.xyzw_, m.column1_),
				System.Numerics.Vector4.Dot(v.xyzw_, m.column2_),
				System.Numerics.Vector4.Dot(v.xyzw_, m.column3_));

			//System.Numerics.Vector4 vx = new System.Numerics.Vector4(v.xyzw_.X);
			//System.Numerics.Vector4 vy = new System.Numerics.Vector4(v.xyzw_.Y);
			//System.Numerics.Vector4 vz = new System.Numerics.Vector4(v.xyzw_.Z);
			//System.Numerics.Vector4 vw = new System.Numerics.Vector4(v.xyzw_.W);
			//return new Vector4(vx*m.row0_ + vy*m.row1_ + vz*m.row2_ + vw*m.row3_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Normalize()
		{
			float m = Magnitude;
			if (m > 0f)
				xyzw_ /= m;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Transform(in Matrix4 m)
		{
			xyzw_ = new System.Numerics.Vector4(System.Numerics.Vector4.Dot(xyzw_, m.column0_),
				System.Numerics.Vector4.Dot(xyzw_, m.column1_),
				System.Numerics.Vector4.Dot(xyzw_, m.column2_),
				System.Numerics.Vector4.Dot(xyzw_, m.column3_));

			//System.Numerics.Vector4 vx = new System.Numerics.Vector4(xyzw_.X);
			//System.Numerics.Vector4 vy = new System.Numerics.Vector4(xyzw_.Y);
			//System.Numerics.Vector4 vz = new System.Numerics.Vector4(xyzw_.Z);
			//System.Numerics.Vector4 vw = new System.Numerics.Vector4(xyzw_.W);
			//xyzw_ = vx*m.row0_ + vy*m.row1_ + vz*m.row2_ + vw*m.row3_;
		}

		internal readonly float x_ 
		{ 
			get { return xyzw_.X; } 
			/*set { xyzw_.X = value; }*/ 
		}
		
		internal readonly float y_ 
		{ 
			get { return xyzw_.Y; } 
			/*set { xyzw_.Y = value; }*/ 
		}
		
		internal readonly float z_
		{ 
			get { return xyzw_.Z; } 
			/*set { xyzw_.Z = value; }*/ 
		}
		
		internal readonly float w_ 
		{ 
			get { return xyzw_.W; } 
			/*set { xyzw_.W = value; }*/ 
		}

		internal System.Numerics.Vector4 xyzw_;
#else
		public static readonly Vector4 Zero = new Vector4(0f, 0f, 0f, 0f);
		public static readonly Vector4 UnitX = new Vector4(1f, 0f, 0f, 0f);
		public static readonly Vector4 UnitY = new Vector4(0f, 1f, 0f, 0f);
		public static readonly Vector4 UnitZ = new Vector4(0f, 0f, 1f, 0f);
		public static readonly Vector4 UnitW = new Vector4(0f, 0f, 0f, 1f);

		public Vector4(float scalar)
		{
			x_ = scalar;
			y_ = scalar;
			z_ = scalar;
			w_ = scalar;
		}

		public Vector4(float x, float y, float z, float w)
		{
			x_ = x;
			y_ = y;
			z_ = z;
			w_ = w;
		}

		public Vector4(Vector2 v, float z, float w)
		{
			x_ = v.x_;
			y_ = v.y_;
			z_ = z;
			w_ = w;
		}

		public Vector4(Vector2 xy, Vector2 zw)
		{
			x_ = xy.x_;
			y_ = xy.y_;
			z_ = zw.x_;
			w_ = zw.y_;
		}

		public Vector4(Vector3 v, float w)
		{
			x_ = v.x_;
			y_ = v.y_;
			z_ = v.z_;
			w_ = w;
		}

		public Vector4(float[] v)
		{
			x_ = v[0];
			y_ = v[1];
			z_ = v[2];
			w_ = v[3];
		}

		private Vector4(SerializationInfo info, StreamingContext context)
		{
			x_ = info.GetSingle("X");
			y_ = info.GetSingle("Y");
			z_ = info.GetSingle("Z");
			w_ = info.GetSingle("W");
		}

		public static implicit operator Vector4(Vector2 v)
		{
			return new Vector4(v.x_, v.y_, 0f, 1f);
		}

		public static implicit operator Vector4(Vector3 v)
		{
			return new Vector4(v.x_, v.y_, v.z_, 1f);
		}

		public float X
		{
			readonly get { return x_; }
			set { x_ = value; }
		}

		public float Y
		{
			readonly get { return y_; }
			set { y_ = value; }
		}

		public float Z
		{
			readonly get { return z_; }
			set { z_ = value; }
		}

		public float W
		{
			readonly get { return w_; }
			set { w_ = value; }
		}

		[Browsable(false)]
		public Vector2 XY
		{
			readonly get { return new Vector2(x_, y_); }
			set { x_ = value.x_; y_ = value.y_; }
		}

		[Browsable(false)]
		public Vector3 XYZ
		{
			readonly get { return new Vector3(x_, y_, z_); }
			set { x_ = value.x_; y_ = value.y_; z_ = value.z_; }
		}

		[Browsable(false)]
		public float Magnitude
		{
			readonly get { return MathF.Sqrt(x_*x_ + y_*y_ + z_*z_ + w_*w_); }
			set
			{
				float m = MathF.Sqrt(x_*x_ + y_*y_ + z_*z_ + w_*w_);
				if (m > 0f) 
					this *= value/m;
			}
		}

		[Browsable(false)]
		public readonly float MagnitudeSquared
		{
			get { return (x_*x_ + y_*y_ + z_*z_ + w_*w_); }
		}

		[Browsable(false)]
		public float Length
		{
			readonly get { return MathF.Sqrt(x_*x_ + y_*y_ + z_*z_ + w_*w_); }
			set
			{
				float m = MathF.Sqrt(x_*x_ + y_*y_ + z_*z_ + w_*w_);
				if (m > 0f)
					this *= value/m;
			}
		}

		[Browsable(false)]
		public readonly float LengthSquared
		{
			get { return (x_*x_ + y_*y_ + z_*z_ + w_*w_); }
		}

		public readonly override bool Equals(object other)
		{
			if (other is Vector4 rhs)
				return (x_ == rhs.x_) && (y_ == rhs.y_) && (z_ == rhs.z_) && (w_ == rhs.w_);
			
			return false;
		}

		public readonly bool Equals(Vector4 other)
		{
			return (x_ == other.x_) && (y_ == other.y_) && (z_ == other.z_) && (w_ == other.w_);
		}
		
		public static bool operator==(Vector4 lhs, Vector4 rhs)
		{
			return (lhs.x_ == rhs.x_) && (lhs.y_ == rhs.y_) && (lhs.z_ == rhs.z_) && (lhs.w_ == rhs.w_);
		}

		public static bool operator!=(Vector4 lhs, Vector4 rhs)
		{
			return (lhs.x_ != rhs.x_) || (lhs.y_ != rhs.y_) || (lhs.z_ != rhs.z_) || (lhs.w_ != rhs.w_);
		}

		public static Vector4 operator+(Vector4 v)
		{
			return v;
		}

		public static Vector4 operator-(Vector4 v)
		{
			return new Vector4(-v.x_, -v.y_, -v.z_, -v.w_);
		}

		public static Vector4 operator+(Vector4 u, Vector4 v)
		{
			return new Vector4(u.x_ + v.x_, u.y_ + v.y_, u.z_ + v.z_, u.w_ + v.w_);
		}

		public static Vector4 operator-(Vector4 u, Vector4 v)
		{
			return new Vector4(u.x_ - v.x_, u.y_ - v.y_, u.z_ - v.z_, u.w_ - v.w_);
		}

		public static Vector4 operator *(Vector4 u, Vector4 v)
		{
			return new Vector4(u.x_*v.x_, u.y_*v.y_, u.z_*v.z_, u.w_*v.w_);
		}

		public static Vector4 operator*(Vector4 v, float f)
		{
			return new Vector4(v.x_*f, v.y_*f, v.z_*f, v.w_*f);
		}

		public static Vector4 operator*(float f, Vector4 v)
		{
			return new Vector4(f*v.x_, f*v.y_, f*v.z_, f*v.w_);
		}

		public static Vector4 operator*(Vector4 v, in Matrix4 m)
		{
			return new Vector4(v.x_*m.m00_ + v.y_*m.m10_ + v.z_*m.m20_ + v.w_*m.m30_,
				v.x_*m.m01_ + v.y_*m.m11_ + v.z_*m.m21_ + v.w_*m.m31_,
				v.x_*m.m02_ + v.y_*m.m12_ + v.z_*m.m22_ + v.w_*m.m32_,
				v.x_*m.m03_ + v.y_*m.m13_ + v.z_*m.m23_ + v.w_*m.m33_);
		}

		//public static Vector4 operator *(Matrix4 m, Vector4 v)
		//{
		//    return new Vector4(m.m00_*v.x_ + m.m01_*v.y_ + m.m02_*v.z_ + m.m03_*v.w_,
		//		m.m10_*v.x_ + m.m11_*v.y_ + m.m12_*v.z_ + m.m13_*v.w_,
		//		m.m20_*v.x_ + m.m21_*v.y_ + m.m22_*v.z_ + m.m23_*v.w_,
		//		m.m30_*v.x_ + m.m31_*v.y_ + m.m32_*v.z_ + m.m33_*v.w_);
		//}

		public static Vector4 operator /(Vector4 u, Vector4 v)
		{
			return new Vector4(u.x_/v.x_, u.y_/v.y_, u.z_/v.z_, u.w_/v.w_);
		}

		public static Vector4 operator/(Vector4 v, float f)
		{
			return new Vector4(v.x_/f, v.y_/f, v.z_/f, v.w_/f);
		}

		public static Vector4 operator/(float f, Vector4 v)
		{
			return new Vector4(f/v.x_, f/v.y_, f/v.z_, f/v.w_);
		}

		public static Vector4 Abs(Vector4 v)
		{
			return new Vector4(MathF.Abs(v.x_), MathF.Abs(v.y_), MathF.Abs(v.z_), MathF.Abs(v.w_));
		}

		public static float Dot(Vector4 u, Vector4 v)
		{
			return (u.x_*v.x_ + u.y_*v.y_ + u.z_*v.z_ + u.w_*v.w_);
		}

		public static float Distance(Vector4 u, Vector4 v)
		{
			return (v - u).Magnitude;
		}

		public static float DistanceSquared(Vector4 u, Vector4 v)
		{
			return (v - u).MagnitudeSquared;
		}

		public static Vector4 Min(Vector4 u, Vector4 v)
		{
			return new Vector4(Math.Min(u.x_, v.x_), Math.Min(u.y_, v.y_), Math.Min(u.z_, v.z_),
				Math.Min(u.w_, v.w_));
		}

		public static Vector4 Max(Vector4 u, Vector4 v)
		{
			return new Vector4(Math.Max(u.x_, v.x_), Math.Max(u.y_, v.y_), Math.Max(u.z_, v.z_),
				Math.Max(u.w_, v.w_));
		}

		public static Vector4 Clamp(Vector4 v, Vector4 low, Vector4 high)
		{
			return new Vector4(Math.Clamp(v.x_, low.x_, high.x_), Math.Clamp(v.y_, low.y_, high.y_), 
				Math.Clamp(v.z_, low.z_, high.z_),	Math.Clamp(v.w_, low.w_, high.w_));
		}

		public static Vector4 Lerp(Vector4 u, Vector4 v, float t)
		{
			return new Vector4(u.x_ + t*(v.x_ - u.x_), u.y_ + t*(v.y_ - u.y_), u.z_ + t*(v.z_ - u.z_),
				u.w_ + t*(v.w_ - u.w_));
		}

		public static Vector4 /*Unit*/Normalize(Vector4 v)
		{
			float m = v.Magnitude;
			return (m > 0f) ? new Vector4(v.x_/m, v.y_/m, v.z_/m, v.w_/m) : v;
		}

		public static Vector4 Transform(Vector4 v, in Matrix4 m)
		{
			return new Vector4(v.x_*m.m00_ + v.y_*m.m10_ + v.z_*m.m20_ + v.w_*m.m30_,
				v.x_*m.m01_ + v.y_*m.m11_ + v.z_*m.m21_ + v.w_*m.m31_,
				v.x_*m.m02_ + v.y_*m.m12_ + v.z_*m.m22_ + v.w_*m.m32_,
				v.x_*m.m03_ + v.y_*m.m13_ + v.z_*m.m23_ + v.w_*m.m33_);
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

		public void Transform(in Matrix4 m)
		{
			float x = x_, y = y_, z = z_, w = w_;
			x_ = x*m.m00_ + y*m.m10_ + z*m.m20_ + w*m.m30_;
			y_ = x*m.m01_ + y*m.m11_ + z*m.m21_ + w*m.m31_;
			z_ = x*m.m02_ + y*m.m12_ + z*m.m22_ + w*m.m32_;
			w_ = x*m.m03_ + y*m.m13_ + z*m.m23_ + w*m.m33_;
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
						X = value;
						break;
					case 1:
						Y = value;
						break;
					case 2:
						Z = value;
						break;
					case 3:
						W = value;
						break;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		public static explicit operator Vector4(IntVector4 v)
		{
			return new Vector4((float)v.x_, (float)v.y_, (float)v.z_, (float)v.w_);
		}

		[Browsable(false)]
		public readonly bool IsFinite
		{
			get { return Functions.IsFinite(x_) && Functions.IsFinite(y_) && Functions.IsFinite(z_) && Functions.IsFinite(w_); }
		}

		public readonly override int GetHashCode()
		{
			//return HashCode.Combine(x_, y_, z_, w_);
			int hash = x_.GetHashCode();
			hash = ((hash << 5) + hash) ^ y_.GetHashCode();
			hash = ((hash << 5) + hash) ^ z_.GetHashCode();
			return ((hash << 5) + hash) ^ w_.GetHashCode();
		}

		public readonly bool ApproxEquals(Vector4 v, float tolerance)
		{
			return (Math.Abs(v.x_ - x_) < tolerance) &&
				(Math.Abs(v.y_ - y_) < tolerance) &&
				(Math.Abs(v.z_ - z_) < tolerance) &&
				(Math.Abs(v.w_ - w_) < tolerance);
		}

		public readonly bool ApproxEquals(Vector4 v)
		{
			return ApproxEquals(v, 1e-6f);
		}

		public readonly bool AllLessThan(Vector4 v) 
		{ 
			return (x_ < v.x_) && (y_ < v.y_) && (z_ < v.z_) && (w_ < v.w_); 
		}

		public readonly bool AllLessThanEqual(Vector4 v) 
		{ 
			return (x_ <= v.x_) && (y_ <= v.y_) && (z_ <= v.z_) && (w_ <= v.w_); 
		}

		public readonly bool AllGreaterThan(Vector4 v) 
		{ 
			return (x_ > v.x_) && (y_ > v.y_) && (z_ > v.z_) && (w_ > v.w_); 
		}

		public readonly bool AllGreaterThanEqual(Vector4 v) 
		{ 
			return (x_ >= v.x_) && (y_ >= v.y_) && (z_ >= v.z_) && (w_ >= v.w_); 
		}

		public readonly bool AnyLessThan(Vector4 v) 
		{ 
			return (x_ < v.x_) || (y_ < v.y_) || (z_ < v.z_) || (w_ < v.w_); 
		}

		public readonly bool AnyLessThanEqual(Vector4 v) 
		{ 
			return (x_ <= v.x_) || (y_ <= v.y_) || (z_ <= v.z_) || (w_ <= v.w_); 
		}

		public readonly bool AnyGreaterThan(Vector4 v) 
		{
			return (x_ > v.x_) || (y_ > v.y_) || (z_ > v.z_) || (w_ > v.w_); 
		}

		public readonly bool AnyGreaterThanEqual(Vector4 v) 
		{ 
			return (x_ >= v.x_) || (y_ >= v.y_) || (z_ >= v.z_) || (w_ >= v.w_); 
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

		public static Vector4 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Vector4(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]), Single.Parse(m[3]));
		}

		public static Vector4 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Vector4(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider),
				Single.Parse(m[3], provider));
		}

		public static Vector4 FromAxis(Axis axis)
		{
			return new Vector4((axis == Axis.X) ? 1f : 0f, (axis == Axis.Y) ? 1f : 0f, (axis == Axis.Z) ? 1f : 0f, (axis == Axis.W) ? 1f : 0f);
		}

		public static Vector4 Cross(Vector4 u, Vector4 v, Vector4 w)
		{
			float d1 = v.z_*w.w_ - v.w_*w.z_;
			float d2 = v.y_*w.w_ - v.w_*w.y_;
			float d3 = v.y_*w.z_ - v.z_*w.y_;
			float d4 = v.x_*w.w_ - v.w_*w.x_;
			float d5 = v.x_*w.z_ - v.z_*w.x_;
			float d6 = v.x_*w.y_ - v.y_*w.x_;
			return new Vector4(-u.y_*d1 + u.z_*d2 - u.w_*d3, u.x_*d1 - u.z_*d4 + u.w_*d5, 
				-u.x_*d2 + u.y_*d4 - u.w_*d6, u.x_*d3 - u.y_*d5 + u.z_*d6);
		}

		public static Matrix4 Tensor(Vector4 u, Vector4 v)
		{
			return new Matrix4(u.x_*v.x_, u.x_*v.y_, u.x_*v.z_, u.x_*v.w_, u.y_*v.x_, u.y_*v.y_, u.y_*v.z_, u.y_*v.w_,
				u.z_*v.x_, u.z_*v.y_, u.z_*v.z_, u.z_*v.w_, u.w_*v.x_, u.w_*v.y_, u.w_*v.z_, u.w_*v.w_);
		}

		public static float Angle(Vector4 u, Vector4 v)
		{
			float q = u.Magnitude*v.Magnitude;
			if (q > 0f)
				return (float)Math.Acos(Math.Clamp(Vector4.Dot(u, v)/q, -1f, 1f));
			else
				return (Vector4.Dot(u, v) >= 0f) ? 0f : SingleConstants.Pi;
			
			//u.Normalize();
			//v.Normalize();
			//return (float)Math.Acos(Math.Clamp(Vector4.Dot(u, v), -1f, 1f));
		}

		public static Vector4 Slerp(Vector4 u, Vector4 v, float t)
		{
			float dp = Vector4.Dot(u, v);
			if ((1f - dp) < 1e-6f)
				return Vector4.Normalize(u + t*(v - u));

			dp = Math.Clamp(dp, -1f, 1f);
			float theta = (float)Math.Acos(dp)*t;
			Vector4 c = Vector4.Normalize(v - u*dp);
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
		public static Vector4 TruncateLength(Vector4 v, float maxLength)
		{
			float maxLengthSquared = maxLength*maxLength;
			float vecLengthSquared = v.LengthSquared;
			return (vecLengthSquared <= maxLengthSquared) ? v : v*(maxLength/MathF.Sqrt(vecLengthSquared));
		}

		/// <summary>
		/// Projects vector <paramref name="a"/> onto vector <paramref name="u"/>.
		/// </summary>
		public static Vector4 Project(Vector4 a, Vector4 u)
		{
			float uSqM = u.MagnitudeSquared;
			return (uSqM > 0f) ? ((Vector4.Dot(a, u)/uSqM)*u) : Vector4.Zero;
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

		//public void Project(Vector4 v)
		//{
		//	this = Vector4.Project(this, v);
		//}
	}
}
