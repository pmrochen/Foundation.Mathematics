/*
 *  Name: Euler
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	[Serializable]
	[TypeConverter(typeof(EulerConverter))]
	public struct Euler : ISerializable, IFormattable, IEquatable<Euler>
	{
		public static readonly Euler Zero = new Euler(0f, 0f, 0f, EulerOrder.Unspecified);
		public static readonly Euler ZeroXYZ = new Euler(0f, 0f, 0f, EulerOrder.XYZ);
		public static readonly Euler ZeroXZY = new Euler(0f, 0f, 0f, EulerOrder.XZY);
		public static readonly Euler ZeroYZX = new Euler(0f, 0f, 0f, EulerOrder.YZX);
		public static readonly Euler ZeroYXZ = new Euler(0f, 0f, 0f, EulerOrder.YXZ);
		public static readonly Euler ZeroZXY = new Euler(0f, 0f, 0f, EulerOrder.ZXY);
		public static readonly Euler ZeroZYX = new Euler(0f, 0f, 0f, EulerOrder.ZYX);

		public Euler(EulerOrder order)
		{
			x_ = 0f;
			y_ = 0f;
			z_ = 0f;
			order_ = order;
		}

		public Euler(float x, float y, float z, EulerOrder order)
		{
			x_ = x;
			y_ = y;
			z_ = z;
			order_ = order;
		}

		public Euler(Euler e, EulerOrder order)
		{
			x_ = e.x_;
			y_ = e.y_;
			z_ = e.z_;
			order_ = order;
		}

		public Euler(float[] e, EulerOrder order)
		{
			x_ = e[0];
			y_ = e[1];
			z_ = e[2];
			order_ = order;
		}

		private Euler(SerializationInfo info, StreamingContext context)
		{
			x_ = info.GetSingle("X");
			y_ = info.GetSingle("Y");
			z_ = info.GetSingle("Z");
			order_ = (EulerOrder)info.GetInt32("Order");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("X", x_);
			info.AddValue("Y", y_);
			info.AddValue("Z", z_);
			info.AddValue("Order", (int)order_);
		}

		[TypeConverter(typeof(DegreeConverter))]
		public float X
		{
			readonly get => x_;
			set => x_ = value;
		}

		[TypeConverter(typeof(DegreeConverter))]
		public float Y
		{
			readonly get => y_;
			set => y_ = value;
		}

		[TypeConverter(typeof(DegreeConverter))]
		public float Z
		{
			readonly get => z_;
			set => z_ = value;
		}

		public EulerOrder Order
		{
			readonly get => order_;
			set => order_ = value;
		}

		[Browsable(false)]
		public readonly bool IsFinite => Scalar.IsFinite(x_) && Scalar.IsFinite(y_) && Scalar.IsFinite(z_);

		public readonly override int GetHashCode()
		{
			int hash = x_.GetHashCode();
			hash = ((hash << 5) + hash) ^ y_.GetHashCode();
			hash = ((hash << 5) + hash) ^ z_.GetHashCode();
			return ((hash << 5) + hash) ^ order_.GetHashCode();
		}

		public readonly override bool Equals(object other)
		{
			if (other is Euler rhs)
				return (order_ == rhs.order_) && (x_ == rhs.x_) && (y_ == rhs.y_) && (z_ == rhs.z_);

			return false;
		}

		public readonly bool Equals(Euler other)
		{
			return (order_ == other.order_) && (x_ == other.x_) && (y_ == other.y_) && (z_ == other.z_);
		}

		public readonly bool ApproxEquals(Euler e, float tolerance)
		{
			return (order_ == e.order_) && (Math.Abs(e.x_ - x_) < tolerance) && (Math.Abs(e.y_ - y_) < tolerance) && 
				(Math.Abs(e.z_ - z_) < tolerance);
		}

		public readonly bool ApproxEquals(Euler e)
		{
			return ApproxEquals(e, 1e-6f);
		}

		public readonly float[] ToArray()
		{
			return new float[3] { x_, y_, z_ };
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1} {2} {3}", x_, y_, z_, order_.ToString());
		}

		public readonly string ToString(bool degrees)
		{
			if (degrees)
				return String.Format("{0} {1} {2} {3}", Scalar.Degrees(x_), Scalar.Degrees(y_), Scalar.Degrees(z_), order_.ToString());
			else
				return String.Format("{0} {1} {2} {3}", x_, y_, z_, order_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3}", x_, y_, z_, order_.ToString());
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1} {2} {3}",
				x_.ToString(format), y_.ToString(format), z_.ToString(format), order_.ToString(/*format*/));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3}",
				x_.ToString(format, provider), y_.ToString(format, provider), z_.ToString(format, provider), order_.ToString(/*format*/));
		}

		public static Euler Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Euler(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]),
				(EulerOrder)Enum.Parse(typeof(EulerOrder), m[3], true));
		}

		public static Euler Parse(string str, bool degrees)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			if (degrees)
			{
				return new Euler(Scalar.Radians(Single.Parse(m[0])), Scalar.Radians(Single.Parse(m[1])),
					Scalar.Radians(Single.Parse(m[2])), (EulerOrder)Enum.Parse(typeof(EulerOrder), m[3], true));
			}
			else
			{
				return new Euler(Single.Parse(m[0]), Single.Parse(m[1]),
					Single.Parse(m[2]), (EulerOrder)Enum.Parse(typeof(EulerOrder), m[3], true));
			}
		}

		public static Euler Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Euler(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
				Single.Parse(m[2], provider), (EulerOrder)Enum.Parse(typeof(EulerOrder), m[3], true));
		}

		public static Euler Parse(string str, IFormatProvider provider, bool degrees)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			if (degrees)
			{
				return new Euler(Scalar.Radians(Single.Parse(m[0], provider)), Scalar.Radians(Single.Parse(m[1], provider)),
					Scalar.Radians(Single.Parse(m[2], provider)), (EulerOrder)Enum.Parse(typeof(EulerOrder), m[3], true));
			}
			else
			{
				return new Euler(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
					Single.Parse(m[2], provider), (EulerOrder)Enum.Parse(typeof(EulerOrder), m[3], true));
			}
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

		public static implicit operator Euler(YawPitchRoll ypr)
		{
			return FromYawPitchRoll(ypr);
		}

		public static bool operator ==(Euler lhs, Euler rhs)
		{
			return (lhs.order_ == rhs.order_) && (lhs.x_ == rhs.x_) && (lhs.y_ == rhs.y_) && (lhs.z_ == rhs.z_);
		}

		public static bool operator !=(Euler lhs, Euler rhs)
		{
			return (lhs.order_ != rhs.order_) || (lhs.x_ != rhs.x_) || (lhs.y_ != rhs.y_) || (lhs.z_ != rhs.z_);
		}

		public static Euler operator +(Euler e)
		{
			return e;
		}

		public static Euler operator -(Euler e)
		{
			return new Euler(-e.x_, -e.y_, -e.z_, e.order_);
		}

		public static Euler operator +(Euler e1, Euler e2)
		{
			if (e1.order_ != e2.order_) 
				throw new InvalidOperationException();

			return new Euler(e1.x_ + e2.x_, e1.y_ + e2.y_, e1.z_ + e2.z_, e1.order_);
		}

		public static Euler operator -(Euler e1, Euler e2)
		{
			if (e1.order_ != e2.order_) 
				throw new InvalidOperationException();

			return new Euler(e1.x_ - e2.x_, e1.y_ - e2.y_, e1.z_ - e2.z_, e1.order_);
		}

		public static Euler operator *(Euler e, float f)
		{
			return new Euler(e.x_*f, e.y_*f, e.z_*f, e.order_);
		}

		public static Euler operator *(float f, Euler e)
		{
			return new Euler(f*e.x_, f*e.y_, f*e.z_, e.order_);
		}

		public static Euler operator /(Euler e, float f)
		{
			return new Euler(e.x_/f, e.y_/f, e.z_/f, e.order_);
		}

		public static Euler operator /(float f, Euler e)
		{
			return new Euler(f/e.x_, f/e.y_, f/e.z_, e.order_);
		}

		public static Euler FromYawPitchRoll(YawPitchRoll ypr)
		{
			return new Euler(ypr.pitch_, ypr.yaw_, ypr.roll_, EulerOrder.ZXY);
		}

		public static Euler FromQuaternion(Quaternion q, EulerOrder order)
		{
			return (q != Quaternion.Identity) ? FromMatrix(Matrix3.Rotation(q), order) : new Euler(0f, 0f, 0f, order);
		}

		public static Euler FromMatrix/*3*/(in Matrix3 m, EulerOrder order)
		{
			if (order != EulerOrder.Unspecified)
			{
				int o = (int)order;
				int f = o & 1; o >>= 1;
				int s = o & 1; o >>= 1;
				int n = o & 1; o >>= 1;
				int i = safe_[o & 3];
				int j = next_[i + n];
				int k = next_[i + 1 - n];

				float ti, tj, th;
				if (s != 0)
				{
					float sy = MathF.Sqrt(m[j][i]*m[j][i] + m[k][i]*m[k][i]);
					if (sy > 1.90735e-6f/*1e-6f*/)
					{
						ti = (float)Math.Atan2(m[j][i], m[k][i]);
						tj = (float)Math.Atan2(sy, m[i][i]);
						th = (float)Math.Atan2(m[i][j], -m[i][k]);
					}
					else
					{
						ti = (float)Math.Atan2(-m[k][j], m[j][j]);
						tj = (float)Math.Atan2(sy, m[i][i]);
						th = 0f;
					}
				}
				else
				{
					float cy = MathF.Sqrt(m[i][i] * m[i][i] + m[i][j] * m[i][j]);
					if (cy > 1.90735e-6f/*1e-6f*/)
					{
						ti = (float)Math.Atan2(m[j][k], m[k][k]);
						tj = (float)Math.Atan2(-m[i][k], cy);
						th = (float)Math.Atan2(m[i][j], m[i][i]);
					}
					else
					{
						ti = (float)Math.Atan2(-m[k][j], m[j][j]);
						tj = (float)Math.Atan2(-m[i][k], cy);
						th = 0f;
					}
				}

				if (n != 0)
				{
					ti = -ti;
					tj = -tj;
					th = -th;
				}

				if (f != 0)
				{
					float t = ti;
					ti = th;
					th = t;
				}

				Euler e = new Euler(0f, 0f, 0f, order);
				e[(f != 0) ? ((s != 0) ? i : k) : i] = ti;
				e[j] = tj;
				e[(f != 0) ? i : ((s != 0) ? i : k)] = th;
				return e;
			}

			return Zero;
		}

		internal float x_;
		internal float y_;
		internal float z_;
		internal EulerOrder order_;

		private static readonly int[] safe_ = { 0, 1, 2, 0 };
		private static readonly int[] next_ = { 1, 2, 0, 1 };
	}
}
