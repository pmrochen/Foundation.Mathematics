/*
 *  Name: Vector2
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
	[TypeConverter(typeof(Vector2Converter))]
	public struct Vector2 : ISerializable, IFormattable, IEquatable<Vector2>
	{
		public static readonly Vector2 PositiveInfinity = new Vector2(Single.PositiveInfinity);
		public static readonly Vector2 NegativeInfinity = new Vector2(Single.NegativeInfinity);
#if SIMD
		public static readonly Vector2 Zero = new Vector2(System.Numerics.Vector2.Zero);
		public static readonly Vector2 UnitX = new Vector2(System.Numerics.Vector2.UnitX);
		public static readonly Vector2 UnitY = new Vector2(System.Numerics.Vector2.UnitY);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2(float scalar)
		{
			xy_ = new System.Numerics.Vector2(scalar);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2(float x, float y)
		{
			xy_ = new System.Numerics.Vector2(x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector2(float[] v)
		{
			xy_ = new System.Numerics.Vector2(v[0], v[1]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Vector2(System.Numerics.Vector2 v)
		{
			xy_ = v;
		}

		private Vector2(SerializationInfo info, StreamingContext context)
		{
			xy_ = new System.Numerics.Vector2(info.GetSingle("X"), info.GetSingle("Y"));
		}

		public float X
		{
			readonly get => xy_.X;
			set => xy_.X = value;
		}

		public float Y
		{
			readonly get => xy_.Y;
			set => xy_.Y = value;
		}

		[Browsable(false)]
		public float Magnitude
		{
			readonly get => xy_.Length();
			set
			{
				float m = xy_.Length();
				if (m > 0f)
					this *= value/m;
			}
		}

		[Browsable(false)]
		public readonly float MagnitudeSquared => xy_.LengthSquared();

		[Browsable(false)]
		public float Length
		{
			readonly get => xy_.Length();
			set
			{
				float m = xy_.Length();
				if (m > 0f)
					this *= value/m;
			}
		}

		[Browsable(false)]
		public readonly float LengthSquared => xy_.LengthSquared();

		public readonly override bool Equals(object other)
		{
			return (other is Vector2 rhs) && (xy_ == rhs.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Vector2 other)
		{
			return (xy_ == other.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Vector2 lhs, Vector2 rhs)
		{
			return (lhs.xy_ == rhs.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector2 lhs, Vector2 rhs)
		{
			return (lhs.xy_ != rhs.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator +(Vector2 v)
		{
			return v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator -(Vector2 v)
		{
			return new Vector2(-v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator +(Vector2 c, Vector2 d)
		{
			return new Vector2(c.xy_ + d.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator -(Vector2 c, Vector2 d)
		{
			return new Vector2(c.xy_ - d.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(Vector2 c, Vector2 d)
		{
			return new Vector2(c.xy_*d.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(Vector2 c, float f)
		{
			return new Vector2(c.xy_*f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(float f, Vector2 c)
		{
			return new Vector2(f*c.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(Vector2 v, in Matrix2 m)
		{
			//return new Vector2(System.Numerics.Vector2.Dot(v.xy_, m.column0_), System.Numerics.Vector2.Dot(v.xy_, m.column1_));
			return new Vector2(v.xy_.X*m.row0_ + v.xy_.Y*m.row1_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator *(in Matrix2 m, Vector2 v)
		{
			return new Vector2(System.Numerics.Vector2.Dot(m.row0_, v.xy_), System.Numerics.Vector2.Dot(m.row1_, v.xy_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator /(Vector2 c, Vector2 d)
		{
			return new Vector2(c.xy_/d.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator /(Vector2 c, float f)
		{
			return new Vector2(c.xy_/f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 operator /(float f, Vector2 c)
		{
			return new Vector2(new System.Numerics.Vector2(f)/c.xy_);
		}

		public float this[int index] // #TODO Use System.Numerics.Vector2 indexing operator
		{
			readonly get
			{
				switch (index)
				{
					case 0:
						return xy_.X;
					case 1:
						return xy_.Y;
					default:
						throw new IndexOutOfRangeException();
				}
			}

			set
			{
				switch (index)
				{
					case 0:
						xy_.X = value;
						break;
					case 1:
						xy_.Y = value;
						break;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Abs(Vector2 v)
		{
			return new Vector2(System.Numerics.Vector2.Abs(v.xy_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(Vector2 u, Vector2 v)
		{
			return System.Numerics.Vector2.Dot(u.xy_, v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(Vector2 u, Vector2 v)
		{
			return System.Numerics.Vector2.Distance(u.xy_, v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float DistanceSquared(Vector2 u, Vector2 v)
		{
			return System.Numerics.Vector2.DistanceSquared(u.xy_, v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Min(Vector2 c, Vector2 d)
		{
			return new Vector2(System.Numerics.Vector2.Min(c.xy_, d.xy_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Max(Vector2 c, Vector2 d)
		{
			return new Vector2(System.Numerics.Vector2.Max(c.xy_, d.xy_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Clamp(Vector2 c, Vector2 low, Vector2 high)
		{
			return new Vector2(System.Numerics.Vector2.Clamp(c.xy_, low.xy_, high.xy_));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Lerp(Vector2 c, Vector2 d, float t)
		{
			return new Vector2(System.Numerics.Vector2.Lerp(c.xy_, d.xy_, t));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Normalize(Vector2 v)
		{
			float m = v.xy_.Length();
			return (m > 0f) ? new Vector2(v.xy_/m) : v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector2 Transform(Vector2 v, in Matrix2 m)
		{
			//return new Vector2(System.Numerics.Vector2.Dot(v.xy_, m.column0_),
			//	System.Numerics.Vector2.Dot(v.xy_, m.column1_));

			return new Vector2(v.xy_.X*m.row0_ + v.xy_.Y*m.row1_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Normalize()
		{
			float m = xy_.Length();
			if (m > 0f)
				xy_ /= m;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Transform(in Matrix2 m)
		{
			//xy_ = new System.Numerics.Vector2(System.Numerics.Vector2.Dot(xy_, m.column0_),
			//	System.Numerics.Vector2.Dot(xy_, m.column1_));

			xy_ = xy_.X*m.row0_ + xy_.Y*m.row1_;
		}

		internal readonly float x_ => xy_.X;
		internal readonly float y_ => xy_.Y;
		internal readonly System.Numerics.Vector2 yx_ => new System.Numerics.Vector2(xy_.Y, xy_.X);

		internal System.Numerics.Vector2 xy_;
#else
		public static readonly Vector2 Zero = new Vector2(0f, 0f);
		public static readonly Vector2 UnitX = new Vector2(1f, 0f);
		public static readonly Vector2 UnitY = new Vector2(0f, 1f);

		public Vector2(float scalar)
		{
			x_ = scalar;
			y_ = scalar;
		}

		public Vector2(float x, float y)
		{
			x_ = x;
			y_ = y;
		}

		public Vector2(float[] v)
		{
			x_ = v[0];
			y_ = v[1];
		}

		private Vector2(SerializationInfo info, StreamingContext context)
		{
			x_ = info.GetSingle("X");
			y_ = info.GetSingle("Y");
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

		[Browsable(false)]
		public float Magnitude
		{
			readonly get => MathF.Sqrt(x_*x_ + y_*y_);
			set
			{
				float m = MathF.Sqrt(x_*x_ + y_*y_);
				if (m > 0f) 
					this *= value/m;
			}
		}

		[Browsable(false)]
		public readonly float MagnitudeSquared => x_*x_ + y_*y_;

		[Browsable(false)]
		public float Length
		{
			readonly get => MathF.Sqrt(x_*x_ + y_*y_);
			set
			{
				float m = MathF.Sqrt(x_*x_ + y_*y_);
				if (m > 0f)
					this *= value/m;
			}
		}

		[Browsable(false)]
		public readonly float LengthSquared => x_*x_ + y_*y_;

		public readonly override bool Equals(object other)
		{
			if (other is Vector2 rhs)
				return (x_ == rhs.x_) && (y_ == rhs.y_);
			
			return false;
		}

		public readonly bool Equals(Vector2 other)
		{
			return (x_ == other.x_) && (y_ == other.y_);
		}
		
		public static bool operator ==(Vector2 lhs, Vector2 rhs)
		{
			return (lhs.x_ == rhs.x_) && (lhs.y_ == rhs.y_);
		}

		public static bool operator !=(Vector2 lhs, Vector2 rhs)
		{
			return (lhs.x_ != rhs.x_) || (lhs.y_ != rhs.y_);
		}

		public static Vector2 operator +(Vector2 v)
		{
			return v;
		}

		public static Vector2 operator -(Vector2 v)
		{
			return new Vector2(-v.x_, -v.y_);
		}

		public static Vector2 operator +(Vector2 u, Vector2 v)
		{
			return new Vector2(u.x_ + v.x_, u.y_ + v.y_);
		}

		public static Vector2 operator -(Vector2 u, Vector2 v)
		{
			return new Vector2(u.x_ - v.x_, u.y_ - v.y_);
		}

		public static Vector2 operator *(Vector2 u, Vector2 v)
		{
			return new Vector2(u.x_*v.x_, u.y_*v.y_);
		}

		public static Vector2 operator *(Vector2 v, float f)
		{
			return new Vector2(v.x_*f, v.y_*f);
		}

		public static Vector2 operator *(float f, Vector2 v)
		{
			return new Vector2(f*v.x_, f*v.y_);
		}

		public static Vector2 operator *(Vector2 v, in Matrix2 m)
		{
			return new Vector2(v.x_*m.m00_ + v.y_*m.m10_, v.x_*m.m01_ + v.y_*m.m11_);
		}

		public static Vector2 operator *(in Matrix2 m, Vector2 v)
		{
		    return new Vector2(m.m00_*v.x_ + m.m01_*v.y_, m.m10_*v.x_ + m.m11_*v.y_);
		}

		public static Vector2 operator /(Vector2 u, Vector2 v)
		{
			return new Vector2(u.x_/v.x_, u.y_/v.y_);
		}

		public static Vector2 operator /(Vector2 v, float f)
		{
			return new Vector2(v.x_/f, v.y_/f);
		}

		public static Vector2 operator /(float f, Vector2 v)
		{
			return new Vector2(f/v.x_, f/v.y_);
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
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		public static Vector2 Abs(Vector2 v)
		{
			return new Vector2(MathF.Abs(v.x_), MathF.Abs(v.y_));
		}

		public static float Dot(Vector2 u, Vector2 v)
		{
			return (u.x_*v.x_ + u.y_*v.y_);
		}

		public static float Distance(Vector2 u, Vector2 v)
		{
			return (v - u).Magnitude;
		}

		public static float DistanceSquared(Vector2 u, Vector2 v)
		{
			return (v - u).MagnitudeSquared;
		}

		public static Vector2 Min(Vector2 u, Vector2 v)
		{
			return new Vector2(Math.Min(u.x_, v.x_), Math.Min(u.y_, v.y_));
		}

		public static Vector2 Max(Vector2 u, Vector2 v)
		{
			return new Vector2(Math.Max(u.x_, v.x_), Math.Max(u.y_, v.y_));
		}

		public static Vector2 Clamp(Vector2 v, Vector2 low, Vector2 high)
		{
			return new Vector2(Math.Clamp(v.x_, low.x_, high.x_), Math.Clamp(v.y_, low.y_, high.y_));
		}

		public static Vector2 Lerp(Vector2 u, Vector2 v, float t)
		{
			return new Vector2(u.x_ + t*(v.x_ - u.x_), u.y_ + t*(v.y_ - u.y_));
		}

		public static Vector2 /*Unit*/Normalize(Vector2 v)
		{
			float m = v.Magnitude;
			return (m > 0f) ? new Vector2(v.x_/m, v.y_/m) : v;
		}

		public static Vector2 Transform(Vector2 v, in Matrix2 m)
		{
			return new Vector2(v.x_*m.m00_ + v.y_*m.m10_, v.x_*m.m01_ + v.y_*m.m11_);
		}

		public void Normalize()
		{
			float m = Magnitude;
			if (m > 0f)
			{
				x_ /= m;
				y_ /= m;
			}
		}

		public void Transform(in Matrix2 m)
		{
			float x = x_, y = y_;
			x_ = x*m.m00_ + y*m.m10_;
			y_ = x*m.m01_ + y*m.m11_;
		}

		internal float x_;
		internal float y_;
#endif

#if SIMD && NET10_0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool AllLessThan(Vector2 v) 
		{
			return Vector2.LessThanAll(xy_, v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool AllLessThanEqual(Vector2 v) 
		{ 
			return Vector2.LessThanOrEqualAll(xy_, v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool AllGreaterThan(Vector2 v) 
		{ 
			return Vector2.GreaterThanAll(xy_, v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool AllGreaterThanEqual(Vector2 v) 
		{ 
			return Vector2.GreaterThanOrEqualAll(xy_, v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool AnyLessThan(Vector2 v) 
		{ 
			return Vector2.LessThanAny(xy_, v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool AnyLessThanEqual(Vector2 v) 
		{ 
			return Vector2.LessThanOrEqualAny(xy_, v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool AnyGreaterThan(Vector2 v) 
		{
			return Vector2.GreaterThanAny(xy_, v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool AnyGreaterThanEqual(Vector2 v) 
		{ 
			return Vector2.GreaterThanOrEqualAny(xy_, v.xy_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Sum(Vector2 v)
		{
			return System.Numerics.Vector2.Sum(v.xy_);
		}
#else
		public readonly bool AllLessThan(Vector2 v)
		{
			return (x_ < v.x_) && (y_ < v.y_);
		}

		public readonly bool AllLessThanEqual(Vector2 v)
		{
			return (x_ <= v.x_) && (y_ <= v.y_);
		}

		public readonly bool AllGreaterThan(Vector2 v)
		{
			return (x_ > v.x_) && (y_ > v.y_);
		}

		public readonly bool AllGreaterThanEqual(Vector2 v)
		{
			return (x_ >= v.x_) && (y_ >= v.y_);
		}

		public readonly bool AnyLessThan(Vector2 v)
		{
			return (x_ < v.x_) || (y_ < v.y_);
		}

		public readonly bool AnyLessThanEqual(Vector2 v)
		{
			return (x_ <= v.x_) || (y_ <= v.y_);
		}

		public readonly bool AnyGreaterThan(Vector2 v)
		{
			return (x_ > v.x_) || (y_ > v.y_);
		}

		public readonly bool AnyGreaterThanEqual(Vector4 v)
		{
			return (x_ >= v.x_) || (y_ >= v.y_);
		}

		public static float Sum(Vector2 v)
		{
			return (v.x_ + v.y_);
		}
#endif

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("X", x_);
			info.AddValue("Y", y_);
		}

		public static explicit operator Vector2(IntVector2 v)
		{
			return new Vector2((float)v.x_, (float)v.y_);
		}

		[Browsable(false)]
		public readonly Vector2 YX => new Vector2(y_, x_);

		[Browsable(false)]
		public readonly bool IsFinite => Functions.IsFinite(x_) && Functions.IsFinite(y_);

		[Browsable(false)]
		public readonly Axis MajorAxis => (MathF.Abs(y_) > MathF.Abs(x_)) ? Axis.Y : Axis.X;

		[Browsable(false)]
		public readonly float MinComponent => Math.Min(x_, y_); // #TODO SIMD

		[Browsable(false)]
		public readonly float MaxComponent => Math.Max(x_, y_); // #TODO SIMD

		public readonly override int GetHashCode()
		{
			//return HashCode.Combine(x_, y_);
			int hash = x_.GetHashCode();
			return ((hash << 5) + hash) ^ y_.GetHashCode();
		}

		public readonly bool ApproxEquals(Vector2 v, float tolerance) // #TODO SIMD
		{
			return (Math.Abs(v.x_ - x_) < tolerance) && 
				(Math.Abs(v.y_ - y_) < tolerance);
		}

		public readonly bool ApproxEquals(Vector2 v)
		{
			return ApproxEquals(v, 1e-6f);
		}

		public readonly float[] ToArray()
		{
			return new float[2] { x_, y_ };
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1}", x_, y_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1}", x_, y_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1}", x_.ToString(format), y_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1}", x_.ToString(format, provider), y_.ToString(format, provider));
		}

		public static Vector2 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 2)
				throw new FormatException();

			return new Vector2(Single.Parse(m[0]), Single.Parse(m[1]));
		}

		public static Vector2 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 2)
				throw new FormatException();

			return new Vector2(Single.Parse(m[0], provider), Single.Parse(m[1], provider));
		}

		public static Vector2 FromAxis(Axis axis)
		{
			return new Vector2((axis == Axis.X) ? 1f : 0f, (axis == Axis.Y) ? 1f : 0f);
		}

		public static float Cross(Vector2 u, Vector2 v)
		{
			return u.x_*v.y_ - u.y_*v.x_;
		}

		public static float Angle(Vector2 u, Vector2 v)
		{
			float q = u.Magnitude*v.Magnitude;
			if (q > 0f)
				return (float)Math.Acos(Math.Clamp(Vector2.Dot(u, v)/q, -1f, 1f));
			else
				return (Vector2.Dot(u, v) >= 0f) ? 0f : SingleConstants.Pi;
			
			//u.Normalize();
			//v.Normalize();
			//return (float)Math.Acos(Math.Clamp(Vector2.Dot(u, v), -1f, 1f));
		}

		public static Vector2 Slerp(Vector2 u, Vector2 v, float t)
		{
			float dp = Vector2.Dot(u, v);
			if ((1f - dp) < 1e-6f)
				return Vector2.Normalize(u + t*(v - u));

			dp = Math.Clamp(dp, -1f, 1f);
			float theta = (float)Math.Acos(dp)*t;
			Vector2 c = Vector2.Normalize(v - u*dp);
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
		public static Vector2 TruncateLength(Vector2 v, float maxLength)
		{
			float maxLengthSquared = maxLength*maxLength;
			float vecLengthSquared = v.LengthSquared;
			return (vecLengthSquared <= maxLengthSquared) ? v : v*(maxLength/MathF.Sqrt(vecLengthSquared));
		}

		public static Vector2 Rotate(Vector2 v, float angle)
		{
			if (angle != 0f)
			{
				float sine = (float)Math.Sin(angle);
				float cosine = (float)Math.Cos(angle);
				float vx = v.x_, vy = v.y_;
				return new Vector2(vx*cosine - vy*sine, vy*cosine + vx*sine);
			}

			return v;
		}

		/// <summary>
		/// Projects vector a onto vector u.
		/// </summary>
		public static Vector2 Project(Vector2 a, Vector2 u)
		{
			float uSqM = u.MagnitudeSquared;
			return (uSqM > 0f) ? ((Vector2.Dot(a, u)/uSqM)*u) : Vector2.Zero;
		}

		/// <summary>
		/// Find a vector that is perpendicular to a vector.
		/// </summary>
		public static Vector2 Perpendicular(Vector2 v)
		{
			return new Vector2(-v.y_, v.x_);
		}

		/// <summary>
		/// Return component of vector parallel to a unit basis vector
		/// (assumes <paramref name="unitBasis"/> has unit magnitude).
		/// </summary>
		/// <param name="v"></param>
		/// <param name="unitBasis"></param>
		/// <returns></returns>
		public static Vector2 ParallelComponent(Vector2 v, Vector2 unitBasis)
		{
			float projection = Vector2.Dot(v, unitBasis);
			return unitBasis*projection;
		}

		/// <summary>
		/// Return component of vector perpendicular to a unit basis vector
		/// (assumes <paramref name="unitBasis"/> has unit magnitude).
		/// </summary>
		/// <param name="v"></param>
		/// <param name="unitBasis"></param>
		/// <returns></returns>
		public static Vector2 PerpendicularComponent(Vector2 v, Vector2 unitBasis)
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

		public void Rotate(float angle)
		{
			if (angle != 0f)
				this = Vector2.Rotate(this, angle);
		}

		//public void Project(Vector2 v)
		//{
		//	this = Vector2.Project(this, v);
		//}
	}
}
