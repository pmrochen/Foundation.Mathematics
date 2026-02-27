/*
 *  Name: IntVector3
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	[Serializable]
	[TypeConverter(typeof(IntVector3Converter))]
	public struct IntVector3 : ISerializable, IFormattable, IEquatable<IntVector3>
	{
		public static readonly IntVector3 Zero = new IntVector3(0, 0, 0);

		public IntVector3(int scalar)
		{
			x_ = scalar;
			y_ = scalar;
			z_ = scalar;
		}

		public IntVector3(int x, int y, int z)
		{
			x_ = x;
			y_ = y;
			z_ = z;
		}

		public IntVector3(IntVector2 v, int z)
		{
			x_ = v.x_;
			y_ = v.y_;
			z_ = z;
		}

		public IntVector3(int[] v)
		{
			x_ = v[0];
			y_ = v[1];
			z_ = v[2];
		}

		private IntVector3(SerializationInfo info, StreamingContext context)
		{
			x_ = info.GetInt32("X");
			y_ = info.GetInt32("Y");
			z_ = info.GetInt32("Z");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("X", x_);
			info.AddValue("Y", y_);
			info.AddValue("Z", z_);
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
		public readonly IntVector2 XZ => new IntVector2(x_, z_);

		[Browsable(false)]
		public readonly IntVector2 ZY => new IntVector2(z_, y_);

		public readonly override int GetHashCode()
		{
			int hash = x_.GetHashCode();
			hash = ((hash << 5) + hash) ^ y_.GetHashCode();
			return ((hash << 5) + hash) ^ z_.GetHashCode();
		}

		public readonly override bool Equals(object other)
		{
			if (other is IntVector3 rhs)
				return (x_ == rhs.x_) && (y_ == rhs.y_) && (z_ == rhs.z_);
			
			return false;
		}

		public readonly bool Equals(IntVector3 other)
		{
			return (x_ == other.x_) && (y_ == other.y_) && (z_ == other.z_);
		}

		public readonly int[] ToArray()
		{
			return new int[3] { x_, y_, z_ };
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

		public static IntVector3 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 3)
				throw new FormatException();

			return new IntVector3(Int32.Parse(m[0]), Int32.Parse(m[1]), Int32.Parse(m[2]));
		}

		public static IntVector3 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 3)
				throw new FormatException();

			return new IntVector3(Int32.Parse(m[0], provider), Int32.Parse(m[1], provider), Int32.Parse(m[2], provider));
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

		//public static implicit operator IntVector3(IntVector2 v)
		//{
		//    return new IntVector3(v.x_, v.y_, 0);
		//}

		public static explicit operator IntVector3(Vector3 v)
		{
			return new IntVector3((int)v.x_, (int)v.y_, (int)v.z_);
		}

		public static IntVector3 Round(Vector3 v)
		{
			return new IntVector3((int)MathF.Round(v.x_), (int)MathF.Round(v.y_), (int)MathF.Round(v.z_));
		}
		
		public static bool operator ==(IntVector3 lhs, IntVector3 rhs)
		{
			return (lhs.x_ == rhs.x_) && (lhs.y_ == rhs.y_) && (lhs.z_ == rhs.z_);
		}

		public static bool operator !=(IntVector3 lhs, IntVector3 rhs)
		{
			return (lhs.x_ != rhs.x_) || (lhs.y_ != rhs.y_) || (lhs.z_ != rhs.z_);
		}

		public static IntVector3 operator +(IntVector3 v)
		{
			return v;
		}

		public static IntVector3 operator -(IntVector3 v)
		{
			return new IntVector3(-v.x_, -v.y_, -v.z_);
		}

		public static IntVector3 operator +(IntVector3 u, IntVector3 v)
		{
			return new IntVector3(u.x_ + v.x_, u.y_ + v.y_, u.z_ + v.z_);
		}

		public static IntVector3 operator -(IntVector3 u, IntVector3 v)
		{
			return new IntVector3(u.x_ - v.x_, u.y_ - v.y_, u.z_ - v.z_);
		}

		public static IntVector3 operator *(IntVector3 u, IntVector3 v)
		{
			return new IntVector3(u.x_*v.x_, u.y_*v.y_, u.z_*v.z_);
		}

		public static IntVector3 operator /(IntVector3 u, IntVector3 v)
		{
			return new IntVector3(u.x_/v.x_, u.y_/v.y_, u.z_/v.z_);
		}

		public static IntVector3 operator *(IntVector3 v, int f)
		{
			return new IntVector3(v.x_*f, v.y_*f, v.z_*f);
		}

		public static IntVector3 operator *(int f, IntVector3 v)
		{
			return new IntVector3(f*v.x_, f*v.y_, f*v.z_);
		}

		public static IntVector3 operator /(IntVector3 v, int f)
		{
			return new IntVector3(v.x_/f, v.y_/f, v.z_/f);
		}

		public static IntVector3 operator /(int f, IntVector3 v)
		{
			return new IntVector3(f/v.x_, f/v.y_, f/v.z_);
		}

		public static IntVector3 Abs(IntVector3 v)
		{
			return new IntVector3(Math.Abs(v.x_), Math.Abs(v.y_), Math.Abs(v.z_));
		}

		public static int Sum(IntVector3 v)
		{
			return (v.x_ + v.y_ + v.z_);
		}

		public static IntVector3 Min(IntVector3 u, IntVector3 v)
		{
			return new IntVector3(Math.Min(u.x_, v.x_), Math.Min(u.y_, v.y_), Math.Min(u.z_, v.z_));
		}

		public static IntVector3 Max(IntVector3 u, IntVector3 v)
		{
			return new IntVector3(Math.Max(u.x_, v.x_), Math.Max(u.y_, v.y_), Math.Max(u.z_, v.z_));
		}

		public static IntVector3 Clamp(IntVector3 v, IntVector3 low, IntVector3 high)
		{
			return new IntVector3(Math.Clamp(v.x_, low.x_, high.x_), Math.Clamp(v.y_, low.y_, high.y_),
				Math.Clamp(v.z_, low.z_, high.z_));
		}

		internal int x_;
		internal int y_;
		internal int z_;
	}
}
