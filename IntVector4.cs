/*
 *  Name: IntVector4
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	[Serializable]
	[TypeConverter(typeof(IntVector4Converter))]
	public struct IntVector4 : ISerializable, IFormattable, IEquatable<IntVector4>
	{
		public static readonly IntVector4 Zero = new IntVector4(0, 0, 0, 0);

		public IntVector4(int scalar)
		{
			x_ = scalar;
			y_ = scalar;
			z_ = scalar;
			w_ = scalar;
		}

		public IntVector4(int x, int y, int z, int w)
		{
			x_ = x;
			y_ = y;
			z_ = z;
			w_ = w;
		}

		public IntVector4(IntVector2 v, int z, int w)
		{
			x_ = v.x_;
			y_ = v.y_;
			z_ = z;
			w_ = w;
		}

		public IntVector4(IntVector2 xy, IntVector2 zw)
		{
			x_ = xy.x_;
			y_ = xy.y_;
			z_ = zw.x_;
			w_ = zw.y_;
		}

		public IntVector4(IntVector3 v, int w)
		{
			x_ = v.x_;
			y_ = v.y_;
			z_ = v.z_;
			w_ = w;
		}

		public IntVector4(int[] v)
		{
			x_ = v[0];
			y_ = v[1];
			z_ = v[2];
			w_ = v[3];
		}

		private IntVector4(SerializationInfo info, StreamingContext context)
		{
			x_ = info.GetInt32("X");
			y_ = info.GetInt32("Y");
			z_ = info.GetInt32("Z");
			w_ = info.GetInt32("W");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("X", x_);
			info.AddValue("Y", y_);
			info.AddValue("Z", z_);
			info.AddValue("W", w_);
		}

		public int X
		{
			readonly get => x_;
			set => x_ = value;
		}

		public int Y
		{
			readonly get => y_;
			set => y_ = value;
		}

		public int Z
		{
			readonly get => z_;
			set => z_ = value;
		}

		public int W
		{
			readonly get => w_;
			set => w_ = value;
		}

		[Browsable(false)]
		public IntVector2 XY
		{
			readonly get => new IntVector2(x_, y_);
			set 
			{ 
				x_ = value.x_; 
				y_ = value.y_; 
			}
		}

		[Browsable(false)]
		public IntVector3 XYZ
		{
			readonly get => new IntVector3(x_, y_, z_);
			set 
			{ 
				x_ = value.x_; 
				y_ = value.y_; 
				z_ = value.z_; 
			}
		}

		public readonly override int GetHashCode()
		{
			int hash = x_.GetHashCode();
			hash = ((hash << 5) + hash) ^ y_.GetHashCode();
			hash = ((hash << 5) + hash) ^ z_.GetHashCode();
			return ((hash << 5) + hash) ^ w_.GetHashCode();
		}

		public readonly override bool Equals(object other)
		{
			if (other is IntVector4 rhs)
				return (x_ == rhs.x_) && (y_ == rhs.y_) && (z_ == rhs.z_) && (w_ == rhs.w_);
			
			return false;
		}

		public readonly bool Equals(IntVector4 other)
		{
			return (x_ == other.x_) && (y_ == other.y_) && (z_ == other.z_) && (w_ == other.w_);
		}

		public readonly int[] ToArray()
		{
			return new int[4] { x_, y_, z_, w_ };
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

		public static IntVector4 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new IntVector4(Int32.Parse(m[0]), Int32.Parse(m[1]), Int32.Parse(m[2]),
				Int32.Parse(m[3]));
		}

		public static IntVector4 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new IntVector4(Int32.Parse(m[0], provider), Int32.Parse(m[1], provider),
				Int32.Parse(m[2], provider), Int32.Parse(m[3], provider));
		}

		public int this[int index]
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

		//public static implicit operator IntVector4(IntVector2 v)
		//{
		//    return new IntVector4(v.x_, v.y_, 0, 1);
		//}

		//public static implicit operator IntVector4(IntVector3 v)
		//{
		//    return new IntVector4(v.x_, v.y_, v.z_, 1);
		//}

		public static explicit operator IntVector4(Vector4 v)
		{
			return new IntVector4((int)v.x_, (int)v.y_, (int)v.z_, (int)v.w_);
		}

		public static IntVector4 Round(Vector4 v)
		{
			return new IntVector4((int)MathF.Round(v.x_), (int)MathF.Round(v.y_), (int)MathF.Round(v.z_), (int)MathF.Round(v.w_));
		}
		
		public static bool operator ==(IntVector4 lhs, IntVector4 rhs)
		{
			return (lhs.x_ == rhs.x_) && (lhs.y_ == rhs.y_) && (lhs.z_ == rhs.z_) && (lhs.w_ == rhs.w_);
		}

		public static bool operator !=(IntVector4 lhs, IntVector4 rhs)
		{
			return (lhs.x_ != rhs.x_) || (lhs.y_ != rhs.y_) || (lhs.z_ != rhs.z_) || (lhs.w_ != rhs.w_);
		}

		public static IntVector4 operator +(IntVector4 v)
		{
			return v;
		}

		public static IntVector4 operator -(IntVector4 v)
		{
			return new IntVector4(-v.x_, -v.y_, -v.z_, -v.w_);
		}

		public static IntVector4 operator +(IntVector4 u, IntVector4 v)
		{
			return new IntVector4(u.x_ + v.x_, u.y_ + v.y_, u.z_ + v.z_, u.w_ + v.w_);
		}

		public static IntVector4 operator -(IntVector4 u, IntVector4 v)
		{
			return new IntVector4(u.x_ - v.x_, u.y_ - v.y_, u.z_ - v.z_, u.w_ - v.w_);
		}

		public static IntVector4 operator *(IntVector4 u, IntVector4 v)
		{
			return new IntVector4(u.x_*v.x_, u.y_*v.y_, u.z_*v.z_, u.w_*v.w_);
		}

		public static IntVector4 operator /(IntVector4 u, IntVector4 v)
		{
			return new IntVector4(u.x_/v.x_, u.y_/v.y_, u.z_/v.z_, u.w_/v.w_);
		}

		public static IntVector4 operator *(IntVector4 v, int f)
		{
			return new IntVector4(v.x_*f, v.y_*f, v.z_*f, v.w_*f);
		}

		public static IntVector4 operator *(int f, IntVector4 v)
		{
			return new IntVector4(f*v.x_, f*v.y_, f*v.z_, f*v.w_);
		}

		public static IntVector4 operator /(IntVector4 v, int f)
		{
			return new IntVector4(v.x_/f, v.y_/f, v.z_/f, v.w_/f);
		}

		public static IntVector4 operator /(int f, IntVector4 v)
		{
			return new IntVector4(f/v.x_, f/v.y_, f/v.z_, f/v.w_);
		}

		public static IntVector4 Min(IntVector4 u, IntVector4 v)
		{
			return new IntVector4(Math.Min(u.x_, v.x_), Math.Min(u.y_, v.y_), Math.Min(u.z_, v.z_),
				Math.Min(u.w_, v.w_));
		}

		public static IntVector4 Max(IntVector4 u, IntVector4 v)
		{
			return new IntVector4(Math.Max(u.x_, v.x_), Math.Max(u.y_, v.y_), Math.Max(u.z_, v.z_),
				Math.Max(u.w_, v.w_));
		}

		public static IntVector4 Clamp(IntVector4 v, IntVector4 low, IntVector4 high)
		{
			return new IntVector4(Math.Clamp(v.x_, low.x_, high.x_), Math.Clamp(v.y_, low.y_, high.y_),
				Math.Clamp(v.z_, low.z_, high.z_), Math.Clamp(v.w_, low.w_, high.w_));
		}

		internal int x_;
		internal int y_;
		internal int z_;
		internal int w_;
	}
}
