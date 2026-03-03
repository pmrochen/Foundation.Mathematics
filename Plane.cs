/*
 *  Name: Plane
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Foundation.Mathematics
{
	[Serializable]
	[TypeConverter(typeof(PlaneConverter))]
	public struct Plane : ISerializable, IFormattable, IEquatable<Plane>
	{
#if SIMD
		public static readonly Plane Empty = new Plane(System.Numerics.Vector4.Zero);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(float a, float b, float c, float d)
		{
			abcd_ = new System.Numerics.Vector4(a, b, c, d);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Plane(Vector3 normal, float constant)
		{
			abcd_ = new System.Numerics.Vector4(normal.xyz_, constant);
		}

		public Plane(Vector3 normal, Vector3 point)
		{
			abcd_ = new System.Numerics.Vector4(normal.xyz_, -Vector3.Dot(normal, point));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Plane(System.Numerics.Vector4 v)
		{
			abcd_ = v;
		}

		private Plane(SerializationInfo info, StreamingContext context)
		{
			abcd_ = new System.Numerics.Vector4(info.GetSingle("A"), info.GetSingle("B"), info.GetSingle("C"), info.GetSingle("D"));
		}

		public static implicit operator Plane(HalfSpace hs)
		{
			return new Plane(hs.abcd_);
		}

		public float A
		{
			readonly get => abcd_.X;
			set => abcd_.X = value;
		}

		public float B
		{
			readonly get => abcd_.Y;
			set => abcd_.Y = value;
		}

		public float C
		{
			readonly get => abcd_.Z;
			set => abcd_.Z = value;
		}

		public float D
		{
			readonly get => abcd_.W;
			set => abcd_.W = value;
		}

		[Browsable(false)]
		public Vector3 Normal
		{
			readonly get => new Vector3(abc_);
			set => abcd_ = new System.Numerics.Vector4(value.xyz_, abcd_.W);
		}

		[Browsable(false)]
		public float Constant
		{
			readonly get => abcd_.W;
			set => abcd_.W = value;
		}

		[Browsable(false)]
		public readonly bool IsEmpty => (abc_ == System.Numerics.Vector3.Zero);

		public readonly override bool Equals(object other)
		{
			return (other is Plane rhs) && (abcd_ == rhs.abcd_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool Equals(Plane other)
		{
			return (abcd_ == other.abcd_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Plane lhs, Plane rhs)
		{
			return (lhs.abcd_ == rhs.abcd_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Plane lhs, Plane rhs)
		{
			return (lhs.abcd_ != rhs.abcd_);
		}

		public static Plane FromHalfSpace(in HalfSpace hs)
		{
			return new Plane(hs.abcd_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Plane Normalize(Plane p)
		{
			float m = p.abc_.Length();
			return (m > 0f) ? new Plane(p.abcd_/m) : p;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Plane Flip(Plane p)
		{
			return new Plane(-p.abcd_);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Normalize()
		{
			float m = abc_.Length();
			if (m > 0f)
				abcd_ /= m;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Flip()
		{
			abcd_ = -abcd_;
		}

		internal readonly float a_ => abcd_.X;
		internal readonly float b_ => abcd_.Y;
		internal readonly float c_ => abcd_.Z;
		internal readonly float d_ => abcd_.W;
		internal readonly System.Numerics.Vector3 abc_ => new System.Numerics.Vector3(abcd_.X, abcd_.Y, abcd_.Z);

		internal System.Numerics.Vector4 abcd_;
#else
		public static readonly Plane Empty = new Plane(0f, 0f, 0f, 0f);

        public Plane(float a, float b, float c, float d)
        {
            a_ = a;
            b_ = b;
            c_ = c;
            d_ = d;
        }

        public Plane(Vector3 normal, float constant)
        {
            a_ = normal.x_;
            b_ = normal.y_;
            c_ = normal.z_;
            d_ = constant;
        }

        public Plane(Vector3 normal, Vector3 point)
        {
            a_ = normal.x_;
            b_ = normal.y_;
            c_ = normal.z_;
            d_ = -Vector3.Dot(normal, point);
        }

		private Plane(SerializationInfo info, StreamingContext context)
		{
			a_ = info.GetSingle("A");
			b_ = info.GetSingle("B");
			c_ = info.GetSingle("C");
			d_ = info.GetSingle("D");
		}

		public static implicit operator Plane(HalfSpace hs)
		{
			return new Plane(hs.a_, hs.b_, hs.c_, hs.d_);
		}

		public float A
		{
			readonly get => a_;
			set => a_ = value;
		}

		public float B
		{
			readonly get => b_;
			set => b_ = value;
		}

		public float C
		{
			readonly get => c_;
			set => c_ = value;
		}

		public float D
		{
			readonly get => d_;
			set => d_ = value;
		}

        [Browsable(false)]
        public Vector3 Normal
        {
			readonly get => new Vector3(a_, b_, c_);
            set 
			{ 
				a_ = value.x_; 
				b_ = value.y_; 
				c_ = value.z_; 
			}
        }

        [Browsable(false)]
        public float Constant
        {
			readonly get => d_;
            set => d_ = value;
        }

		[Browsable(false)]
		public readonly bool IsEmpty => (a_ == 0f) && (b_ == 0f) && (c_ == 0f);

		public readonly override bool Equals(object other)
		{
			if (other is Plane rhs)
				return (a_ == rhs.a_) && (b_ == rhs.b_) && (c_ == rhs.c_) && (d_ == rhs.d_);

			return false;
		}

		public readonly bool Equals(Plane other)
		{
			return (a_ == other.a_) && (b_ == other.b_) && (c_ == other.c_) && (d_ == other.d_);
		}

		public static bool operator ==(Plane lhs, Plane rhs)
		{
			return (lhs.a_ == rhs.a_) && (lhs.b_ == rhs.b_) && (lhs.c_ == rhs.c_) && (lhs.d_ == rhs.d_);
		}

		public static bool operator !=(Plane lhs, Plane rhs)
		{
			return (lhs.a_ != rhs.a_) || (lhs.b_ != rhs.b_) || (lhs.c_ != rhs.c_) || (lhs.d_ != rhs.d_);
		}

		public static Plane FromHalfSpace(in HalfSpace hs)
		{
			return new Plane(hs.a_, hs.b_, hs.c_, hs.d_);
		}

		public static Plane Normalize(Plane p)
		{
			float m = p.Normal.Magnitude;
			return (m > 0f) ? new Plane(p.a_/m, p.b_/m, p.c_/m, p.d_/m) : p;
		}

		public static Plane Flip(Plane p)
		{
			return new Plane(-p.a_, -p.b_, -p.c_, -p.d_);
		}

        public void Normalize()
        {
			float m = Normal.Magnitude;
			if (m > 0f)
			{
				a_ /= m;
				b_ /= m;
				c_ /= m;
				d_ /= m;
			}
        }

		public void Flip()
		{
			a_ = -a_;
			b_ = -b_;
			c_ = -c_;
			d_ = -d_;
		}

		internal float a_;
		internal float b_;
		internal float c_;
		internal float d_;
#endif

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("A", a_);
			info.AddValue("B", b_);
			info.AddValue("C", c_);
			info.AddValue("D", d_);
		}

		public readonly override int GetHashCode()
		{
			int hash = a_.GetHashCode();
			hash = ((hash << 5) + hash) ^ b_.GetHashCode();
			hash = ((hash << 5) + hash) ^ c_.GetHashCode();
			return ((hash << 5) + hash) ^ d_.GetHashCode();
		}

		public readonly bool ApproxEquals(Plane p, float tolerance) // #TODO SIMD
		{
			return (Math.Abs(p.a_ - a_) < tolerance) &&
				(Math.Abs(p.b_ - b_) < tolerance) &&
				(Math.Abs(p.c_ - c_) < tolerance) &&
				(Math.Abs(p.d_ - d_) < tolerance);
		}

		public readonly bool ApproxEquals(Plane p)
		{
			return ApproxEquals(p, 1e-6f);
		}

		public readonly override string ToString()
		{
			return String.Format("{0} {1} {2} {3}", a_, b_, c_, d_);
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3}", a_, b_, c_, d_);
		}

		public readonly string ToString(string format)
		{
			return String.Format("{0} {1} {2} {3}", a_.ToString(format), b_.ToString(format), c_.ToString(format), d_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Format(provider, "{0} {1} {2} {3}",
				a_.ToString(format, provider), b_.ToString(format, provider), c_.ToString(format, provider), d_.ToString(format, provider));
		}

		public static Plane Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Plane(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2]), Single.Parse(m[3]));
		}

		public static Plane Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 4)
				throw new FormatException();

			return new Plane(Single.Parse(m[0], provider), Single.Parse(m[1], provider),
				Single.Parse(m[2], provider), Single.Parse(m[3], provider));
		}

		public static Plane FromTriangle(in Triangle3 t)
		{
			return new Plane(Vector3.Normalize(Vector3.Cross(t.vertex1_ - t.vertex0_, t.vertex2_ - t.vertex0_)), t.vertex0_);
		}

		public static Plane FromTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
		{
			return new Plane(Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0)), v0);
		}

		public static Plane Translate(Plane p, Vector3 offset)
		{
			p.Translate(offset);
			return p;
		}

		//public static Plane Transform(Plane p, in AffineTransform t)
		//{
		//    p.Transform(t);
		//    return p;
		//}

		public void Translate(Vector3 offset)
		{
			Vector3 n = Normal;
			Vector3 p = -Constant*n;
			Constant = -Vector3.Dot(n, p + offset);
		}

		//public void Transform(in AffineTransform t) // #TODO
		//{
		//}

		/// <summary>
		/// Returns a point reflected in normalized plane.
		/// </summary>
		public readonly Vector3 Reflect(Vector3 point)
		{
			return (Normal*(-2f*(Vector3.Dot(Normal, point) + d_)) + point);
		}

		/// <summary>
		/// Returns distance from a point to normalized plane.
		/// </summary>
		public readonly float GetDistanceTo(Vector3 point)
		{
			return Math.Abs((Vector3.Dot(Normal, point) + d_));
		}

		public readonly float GetDistanceTo(Vector3 point, bool normalized)
		{
			return normalized ?
				Math.Abs((Vector3.Dot(Normal, point) + d_)) :
				Math.Abs((Vector3.Dot(Normal, point) + d_)/Normal.Magnitude);
		}

		/// <summary>
		/// Returns signed distance from a point to normalized plane.
		/// </summary>
		public readonly float GetSignedDistanceTo(Vector3 point)
		{
			return (Vector3.Dot(Normal, point) + d_);
		}

		public readonly float GetSignedDistanceTo(Vector3 point, bool normalized)
		{
			return normalized ? 
				(Vector3.Dot(Normal, point) + d_) :
				(Vector3.Dot(Normal, point) + d_)/Normal.Magnitude;
		}

		public readonly bool Contains(Vector3 point)
		{
			return (Math.Abs(Vector3.Dot(Normal, point) + d_) < 1e-6f);
		}

		public readonly bool Intersects(in Triangle3 triangle)
		{
			return triangle.Intersects(this);
		}

		public readonly bool Intersects(in AxisAlignedBox box)
		{
			return box.Intersects(this);
		}

		public readonly bool Intersects(in OrientedBox box)
		{
			return box.Intersects(this);
		}

		public readonly bool Intersects(in Sphere sphere)
		{
			return sphere.Intersects(this);
		}

		public readonly bool Intersects(in Ellipsoid ellipsoid)
		{
			return ellipsoid.Intersects(this);
		}
	}
}
