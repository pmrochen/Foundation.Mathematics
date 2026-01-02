/*
 *  Name: YawPitchRoll
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	[Serializable]
	[TypeConverter(typeof(YawPitchRollConverter))]
	public struct YawPitchRoll : ISerializable, IFormattable, IEquatable<YawPitchRoll>
	{
		public static readonly YawPitchRoll Zero = new YawPitchRoll(0f, 0f, 0f);

		public YawPitchRoll(float yaw, float pitch, float roll)
		{
			yaw_ = yaw;
			pitch_ = pitch;
			roll_ = roll;
		}

		private YawPitchRoll(SerializationInfo info, StreamingContext context)
		{
			yaw_ = info.GetSingle("Yaw");
			pitch_ = info.GetSingle("Pitch");
			roll_ = info.GetSingle("Roll");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Yaw", yaw_);
			info.AddValue("Pitch", pitch_);
			info.AddValue("Roll", roll_);
		}

		[TypeConverter(typeof(DegreeConverter))]
		public float Yaw
		{
			readonly get => yaw_;
			set => yaw_ = value;
		}

		[TypeConverter(typeof(DegreeConverter))]
		public float Pitch
		{
			readonly get => pitch_;
			set => pitch_ = value;
		}

		[TypeConverter(typeof(DegreeConverter))]
		public float Roll
		{
			readonly get => roll_;
			set => roll_ = value;
		}

		[Browsable(false)]
		public readonly bool IsFinite => Functions.IsFinite(yaw_) && Functions.IsFinite(pitch_) && Functions.IsFinite(roll_);

		public readonly override int GetHashCode()
		{
			int hash = yaw_.GetHashCode();
			hash = ((hash << 5) + hash) ^ pitch_.GetHashCode();
			return ((hash << 5) + hash) ^ roll_.GetHashCode();
		}

		public readonly override bool Equals(object other)
		{
			if (other is YawPitchRoll rhs)
				return (yaw_ == rhs.yaw_) && (pitch_ == rhs.pitch_) && (roll_ == rhs.roll_);
			
			return false;
		}

		public readonly bool Equals(YawPitchRoll other)
		{
			return (yaw_ == other.yaw_) && (pitch_ == other.pitch_) && (roll_ == other.roll_);
		}

		public readonly bool ApproxEquals(YawPitchRoll ypr, float tolerance)
		{
			return (Math.Abs(ypr.yaw_ - yaw_) < tolerance) && (Math.Abs(ypr.pitch_ - pitch_) < tolerance) &&
				(Math.Abs(ypr.roll_ - roll_) < tolerance);
		}

		public readonly bool ApproxEquals(YawPitchRoll ypr)
		{
			return ApproxEquals(ypr, 1e-6f);
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1} {2}", yaw_, pitch_, roll_);
		}

		public readonly string ToString(bool degrees)
		{
			if (degrees)
				return String.Format("{0} {1} {2}", Functions.Degrees(yaw_), Functions.Degrees(pitch_), Functions.Degrees(roll_));
			else
				return String.Format("{0} {1} {2}", yaw_, pitch_, roll_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2}", yaw_, pitch_, roll_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1} {2}", 
				yaw_.ToString(format), pitch_.ToString(format), roll_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2}",
				yaw_.ToString(format, provider), pitch_.ToString(format, provider), roll_.ToString(format, provider));
		}

		public static YawPitchRoll Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 3)
				throw new FormatException();

			return new YawPitchRoll(Single.Parse(m[0]),	Single.Parse(m[1]), Single.Parse(m[2]));
		}

		public static YawPitchRoll Parse(string str, bool degrees)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 3)
				throw new FormatException();

			if (degrees)
			{
				return new YawPitchRoll(Functions.Radians(Single.Parse(m[0])), Functions.Radians(Single.Parse(m[1])),
					Functions.Radians(Single.Parse(m[2])));
			}
			else
			{
				return new YawPitchRoll(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]));
			}
		}

		public static YawPitchRoll Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 3)
				throw new FormatException();

			return new YawPitchRoll(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
				Single.Parse(m[2], provider));
		}

		public static YawPitchRoll Parse(string str, IFormatProvider provider, bool degrees)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 3)
				throw new FormatException();

			if (degrees)
			{
				return new YawPitchRoll(Functions.Radians(Single.Parse(m[0], provider)), Functions.Radians(Single.Parse(m[1], provider)),
					Functions.Radians(Single.Parse(m[2], provider)));
			}
			else
			{
				return new YawPitchRoll(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
					Single.Parse(m[2], provider));
			}
		}

		public static explicit operator YawPitchRoll(Euler e)
		{
			return FromEuler(e);
		}
		
		public static bool operator ==(YawPitchRoll lhs, YawPitchRoll rhs)
		{
			return (lhs.yaw_ == rhs.yaw_) && (lhs.pitch_ == rhs.pitch_) && (lhs.roll_ == rhs.roll_);
		}

		public static bool operator !=(YawPitchRoll lhs, YawPitchRoll rhs)
		{
			return (lhs.yaw_ != rhs.yaw_) || (lhs.pitch_ != rhs.pitch_) || (lhs.roll_ != rhs.roll_);
		}

		public static YawPitchRoll operator +(YawPitchRoll ypr)
		{
			return ypr;
		}

		public static YawPitchRoll operator -(YawPitchRoll ypr)
		{
			return new YawPitchRoll(-ypr.yaw_, -ypr.pitch_, -ypr.roll_);
		}

		public static YawPitchRoll operator +(YawPitchRoll ypr1, YawPitchRoll ypr2)
		{
			return new YawPitchRoll(ypr1.yaw_ + ypr2.yaw_, ypr1.pitch_ + ypr2.pitch_, ypr1.roll_ + ypr2.roll_);
		}

		public static YawPitchRoll operator -(YawPitchRoll ypr1, YawPitchRoll ypr2)
		{
			return new YawPitchRoll(ypr1.yaw_ - ypr2.yaw_, ypr1.pitch_ - ypr2.pitch_, ypr1.roll_ - ypr2.roll_);
		}

		public static YawPitchRoll operator *(YawPitchRoll ypr, float f)
		{
			return new YawPitchRoll(ypr.yaw_*f, ypr.pitch_*f, ypr.roll_*f);
		}

		public static YawPitchRoll operator *(float f, YawPitchRoll ypr)
		{
			return new YawPitchRoll(f*ypr.yaw_, f*ypr.pitch_, f*ypr.roll_);
		}

		public static YawPitchRoll operator /(YawPitchRoll ypr, float f)
		{
			return new YawPitchRoll(ypr.yaw_/f, ypr.pitch_/f, ypr.roll_/f);
		}

		public static YawPitchRoll operator /(float f, YawPitchRoll ypr)
		{
			return new YawPitchRoll(f/ypr.yaw_, f/ypr.pitch_, f/ypr.roll_);
		}

		public static YawPitchRoll FromEuler(Euler e)
		{
			if (e.order_ == EulerOrder.ZXY)
				return new YawPitchRoll(e.y_, e.x_, e.z_);
			else if (e.order_ != EulerOrder.Unspecified)
				return FromMatrix(Matrix3.Rotation(e));
			else
				return Zero;
		}

		public static YawPitchRoll FromQuaternion(Quaternion q)
		{
			// Using Matrix3.Rotation() instead of Matrix3.FromQuaternion()
			// - the former does not assume that the input quaternion is normalized.
			return FromMatrix(Matrix3.Rotation(q));
		}

		//[Obsolete("Use YawPitchRoll.FromMatrix(Matrix3)")]
		//public static YawPitchRoll FromBasis(Matrix3 m)
		//{
		//    return FromMatrix(m);
		//}

		public static YawPitchRoll FromMatrix/*3*/(in Matrix3 m)
		{
			Euler e = Euler.FromMatrix(m, EulerOrder.ZXY);
			return new YawPitchRoll(e.y_, e.x_, e.z_);
		}

		public static YawPitchRoll FromForward(Vector3 forward)
        {
            float h = 0f, p = 0f;
			float m = forward.Magnitude;
            if (m > 0f)
            {
				forward /= m;
				p = (float)Math.Asin(Math.Clamp(-forward.y_, -1f, 1f));
				if ((1f - Math.Abs(forward.y_)) >= 1e-6f)
	            {
		            float cosP = (float)Math.Cos(p);
					h = (float)Math.Acos(Math.Clamp(forward.z_/cosP, -1f, 1f));
					if (forward.x_ < 0f) 
						h = -h;
	            }
            }

            return new YawPitchRoll(h, p, 0f);
        }

		public static YawPitchRoll FromForward(Vector3 forward, float roll)
		{
			float h = 0f, p = 0f;
			float m = forward.Magnitude;
			if (m > 0f)
			{
				forward /= m;
				p = (float)Math.Asin(Math.Clamp(-forward.y_, -1f, 1f));
				if ((1f - Math.Abs(forward.y_)) >= 1e-6f)
				{
					float cosp = (float)Math.Cos(p);
					h = (float)Math.Acos(Math.Clamp(forward.z_/cosp, -1f, 1f));
					if (forward.x_ < 0f) 
						h = -h;
				}
			}

			return new YawPitchRoll(h, p, roll);
		}

		public readonly Vector3 ToForward()
		{
			float sinH = (float)Math.Sin(yaw_);
			float cosH = (float)Math.Cos(yaw_);
			float sinP = (float)Math.Sin(pitch_);
			float cosP = (float)Math.Cos(pitch_);
			return new Vector3(sinH*cosP, -sinP, cosH*cosP);
		}

		// #TODO internal System.Numerics.Vector3 v_;
		internal float yaw_;
		internal float pitch_;
		internal float roll_;
	}
}
