/*
 *  Name: IntVector2
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	[Serializable]
	[TypeConverter(typeof(IntVector2Converter))]
	public struct IntVector2 : ISerializable, IFormattable, IEquatable<IntVector2>
	{
		public static readonly IntVector2 Zero = new IntVector2(0, 0);

		public IntVector2(int scalar)
		{
			x_ = scalar;
			y_ = scalar;
		}

		public IntVector2(int x, int y)
		{
			x_ = x;
			y_ = y;
		}

		public IntVector2(int[] v)
		{
			x_ = v[0];
			y_ = v[1];
		}

		private IntVector2(SerializationInfo info, StreamingContext context)
		{
			x_ = info.GetInt32("X");
			y_ = info.GetInt32("Y");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("X", x_);
			info.AddValue("Y", y_);
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

		[Browsable(false)]
		public readonly int MinComponent => Math.Min(x_, y_);

		[Browsable(false)]
		public readonly int MaxComponent => Math.Max(x_, y_);

		public readonly override int GetHashCode()
		{
			int hash = x_.GetHashCode();
			return ((hash << 5) + hash) ^ y_.GetHashCode();
		}

		public readonly override bool Equals(object other)
		{
			if (other is IntVector2 rhs)
				return (x_ == rhs.x_) && (y_ == rhs.y_);
			
			return false;
		}

		public readonly bool Equals(IntVector2 other)
		{
			return (x_ == other.x_) && (y_ == other.y_);
		}

		public readonly bool AllLessThan(IntVector2 v)
		{
			return (x_ < v.x_) && (y_ < v.y_);
		}

		public readonly bool AllLessThanEqual(IntVector2 v)
		{
			return (x_ <= v.x_) && (y_ <= v.y_);
		}

		public readonly bool AllGreaterThan(IntVector2 v)
		{
			return (x_ > v.x_) && (y_ > v.y_);
		}

		public readonly bool AllGreaterThanEqual(IntVector2 v)
		{
			return (x_ >= v.x_) && (y_ >= v.y_);
		}

		public readonly bool AnyLessThan(IntVector2 v)
		{
			return (x_ < v.x_) || (y_ < v.y_);
		}

		public readonly bool AnyLessThanEqual(IntVector2 v)
		{
			return (x_ <= v.x_) || (y_ <= v.y_);
		}

		public readonly bool AnyGreaterThan(IntVector2 v)
		{
			return (x_ > v.x_) || (y_ > v.y_);
		}

		public readonly bool AnyGreaterThanEqual(IntVector4 v)
		{
			return (x_ >= v.x_) || (y_ >= v.y_);
		}

		public readonly int[] ToArray()
		{
			return new int[2] { x_, y_ };
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

		public static IntVector2 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 2)
				throw new FormatException();

			return new IntVector2(Int32.Parse(m[0]), Int32.Parse(m[1]));
		}

		public static IntVector2 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 2)
				throw new FormatException();

			return new IntVector2(Int32.Parse(m[0], provider), Int32.Parse(m[1], provider));
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

		public static explicit operator IntVector2(Vector2 v)
		{		
			return new IntVector2((int)v.x_, (int)v.y_);
		}

		public static IntVector2 Round(Vector2 v)
		{
			return new IntVector2((int)MathF.Round(v.x_), (int)MathF.Round(v.y_));
		}

		public static bool operator ==(IntVector2 lhs, IntVector2 rhs)
		{
			return (lhs.x_ == rhs.x_) && (lhs.y_ == rhs.y_);
		}

		public static bool operator !=(IntVector2 lhs, IntVector2 rhs)
		{
			return (lhs.x_ != rhs.x_) || (lhs.y_ != rhs.y_);
		}

		public static IntVector2 operator +(IntVector2 v)
		{
			return v;
		}

		public static IntVector2 operator -(IntVector2 v)
		{
			return new IntVector2(-v.x_, -v.y_);
		}

		public static IntVector2 operator +(IntVector2 u, IntVector2 v)
		{
			return new IntVector2(u.x_ + v.x_, u.y_ + v.y_);
		}

		public static IntVector2 operator -(IntVector2 u, IntVector2 v)
		{
			return new IntVector2(u.x_ - v.x_, u.y_ - v.y_);
		}

		public static IntVector2 operator *(IntVector2 u, IntVector2 v)
		{
			return new IntVector2(u.x_*v.x_, u.y_*v.y_);
		}

		public static IntVector2 operator /(IntVector2 u, IntVector2 v)
		{
			return new IntVector2(u.x_/v.x_, u.y_/v.y_);
		}

		public static IntVector2 operator *(IntVector2 v, int f)
		{
			return new IntVector2(v.x_*f, v.y_*f);
		}

		public static IntVector2 operator *(int f, IntVector2 v)
		{
			return new IntVector2(f*v.x_, f*v.y_);
		}

		public static IntVector2 operator /(IntVector2 v, int f)
		{
			return new IntVector2(v.x_/f, v.y_/f);
		}

		public static IntVector2 operator /(int f, IntVector2 v)
		{
			return new IntVector2(f/v.x_, f/v.y_);
		}

		public static IntVector2 Abs(IntVector2 v)
		{
			return new IntVector2(Math.Abs(v.x_), Math.Abs(v.y_));
		}

		public static int Sum(IntVector2 v)
		{
			return (v.x_ + v.y_);
		}

		public static IntVector2 Min(IntVector2 u, IntVector2 v)
		{
			return new IntVector2(Math.Min(u.x_, v.x_), Math.Min(u.y_, v.y_));
		}

		public static IntVector2 Max(IntVector2 u, IntVector2 v)
		{
			return new IntVector2(Math.Max(u.x_, v.x_), Math.Max(u.y_, v.y_));
		}

		public static IntVector2 Clamp(IntVector2 v, IntVector2 low, IntVector2 high)
		{
			return new IntVector2(Math.Clamp(v.x_, low.x_, high.x_), Math.Clamp(v.y_, low.y_, high.y_));
		}

		internal int x_;
		internal int y_;
	}
}
