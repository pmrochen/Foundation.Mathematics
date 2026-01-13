/*
 *  Name: PackedTransform
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	[Serializable]
	[TypeConverter(typeof(PackedTransformConverter))]
	public struct PackedTransform : ISerializable, IFormattable, IEquatable<PackedTransform>
	{
		public static readonly PackedTransform Zero = new PackedTransform(Vector4.Zero, Quaternion.Zero);
		public static readonly PackedTransform Identity = new PackedTransform(Vector4.UnitW, Quaternion.Identity);

		public PackedTransform(Vector4 ts, Quaternion q)
		{
			ts_ = ts;
			q_ = q;
		}

		public PackedTransform(Vector3 t, Quaternion q, float s)
		{
			ts_ = new Vector4(t, s);
			q_ = q;
		}

        private PackedTransform(SerializationInfo info, StreamingContext context)
		{
			Vector3 t = (Vector3)info.GetValue("Translation", typeof(Vector3));
			q_ = (Quaternion)info.GetValue("Rotation", typeof(Quaternion));
			ts_ = new Vector4(t, (float)info.GetValue("Scale", typeof(float)));
		}

		//public static explicit operator PackedTransform(in AffineTransform t)
		//{
		//}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
			info.AddValue("Translation", ts_.XYZ, typeof(Vector3));
			info.AddValue("Rotation", q_, typeof(Quaternion));
			info.AddValue("Scale", ts_.W, typeof(float));
		}

		public Vector3 Translation
		{
			readonly get => ts_.XYZ;
			set => ts_.XYZ = value;
		}

		public Quaternion Rotation
		{
			readonly get => q_;
			set => q_ = value;
		}

		public float Scale
		{
			readonly get => ts_.W;
			set => ts_.W = value;
		}

		[Browsable(false)]
		public Vector4 TranslationScale
		{
			readonly get => ts_;
			set => ts_ = value;
		}

		public readonly override int GetHashCode()
		{
			int hash = ts_.GetHashCode();
			return ((hash << 5) + hash) ^ q_.GetHashCode();
		}

		public readonly override bool Equals(object other)
		{
			if (other is PackedTransform rhs)
				return (ts_ == rhs.ts_) && (q_ == rhs.q_);
			
			return false;
		}

		public readonly bool Equals(PackedTransform other)
		{
			return (ts_ == other.ts_) && (q_ == other.q_);
		}

		public static bool operator ==(PackedTransform lhs, PackedTransform rhs)
		{
			return (lhs.ts_ == rhs.ts_) && (lhs.q_ == rhs.q_);
		}

		public static bool operator !=(PackedTransform lhs, PackedTransform rhs)
		{
			return (lhs.ts_ != rhs.ts_) || (lhs.q_ != rhs.q_);
		}

		public readonly bool ApproxEquals(in PackedTransform t, float tolerance)
		{
			return ts_.ApproxEquals(t.ts_, tolerance) && q_.ApproxEquals(t.q_, tolerance);
		}

		public readonly bool ApproxEquals(in PackedTransform t)
		{
			return ApproxEquals(t, 1e-6f);
		}

		//public static AffineTransform FromAffineTransform(AffineTransform t)
		//{
		//}

		public readonly override string ToString()
		{
			return String.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
				ts_.x_, ts_.y_, ts_.z_, ts_.w_, q_.x_, q_.y_, q_.z_, q_.w_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3} {4} {5} {6} {7}",
				ts_.x_, ts_.y_, ts_.z_, ts_.w_, q_.x_, q_.y_, q_.z_, q_.w_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
				ts_.x_.ToString(format), ts_.y_.ToString(format), ts_.z_.ToString(format),
				ts_.w_.ToString(format), q_.x_.ToString(format), q_.y_.ToString(format), 
				q_.z_.ToString(format), q_.w_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3} {4} {5} {6} {7}",
				ts_.x_.ToString(format, provider), ts_.y_.ToString(format, provider), ts_.z_.ToString(format, provider),
				ts_.w_.ToString(format, provider), q_.x_.ToString(format, provider), q_.y_.ToString(format, provider),
				q_.z_.ToString(format, provider), q_.w_.ToString(format, provider));
		}

		public static PackedTransform Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 8)
				throw new FormatException();

			return new PackedTransform(new Vector4(Single.Parse(m[0]), Single.Parse(m[1]), 
				Single.Parse(m[2]),	Single.Parse(m[3])),
				new Quaternion(Single.Parse(m[4]), Single.Parse(m[5]),
				Single.Parse(m[6]), Single.Parse(m[7])));
		}

		public static PackedTransform Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 8)
				throw new FormatException();

			return new PackedTransform(new Vector4(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
				Single.Parse(m[2], provider), Single.Parse(m[3], provider)),
				new Quaternion(Single.Parse(m[4], provider), Single.Parse(m[5], provider),
				Single.Parse(m[6], provider), Single.Parse(m[7], provider)));
		}

		internal Vector4 ts_;
		internal Quaternion q_;
	}
}
