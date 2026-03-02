/*
 *  Name: Quadrilateral3
 *  Author: Pawel Mrochen
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Quadrilateral3 : IFormattable, IEquatable<Quadrilateral3>
	{
		public Quadrilateral3(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            vertex0_ = vertex0;
            vertex1_ = vertex1;
            vertex2_ = vertex2;
			vertex3_ = vertex3;
		}

		public Quadrilateral3(Vector3[] vertices) :
			this(vertices[0], vertices[1], vertices[2], vertices[3])
		{
		}

		public static bool operator ==(Quadrilateral3 lhs, Quadrilateral3 rhs)
		{
			return (lhs.vertex0_ == rhs.vertex0_) && (lhs.vertex1_ == rhs.vertex1_) && (lhs.vertex2_ == rhs.vertex2_) &&
				(lhs.vertex3_ == rhs.vertex3_);
		}

		public static bool operator !=(Quadrilateral3 lhs, Quadrilateral3 rhs)
		{
			return (lhs.vertex0_ != rhs.vertex0_) || (lhs.vertex1_ != rhs.vertex1_) || (lhs.vertex2_ != rhs.vertex2_) ||
				(lhs.vertex3_ != rhs.vertex3_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Quadrilateral3 rhs)
			{
				return (vertex0_ == rhs.vertex0_) && (vertex1_ == rhs.vertex1_) && (vertex2_ == rhs.vertex2_) &&
					(vertex3_ == rhs.vertex3_);
			}

			return false;
		}

		public readonly bool Equals(Quadrilateral3 other)
		{
			return (vertex0_ == other.vertex0_) && (vertex1_ == other.vertex1_) && (vertex2_ == other.vertex2_) &&
				(vertex3_ == other.vertex3_);
		}

		public readonly override int GetHashCode()
		{
			int hash = vertex0_.GetHashCode();
			hash = ((hash << 5) + hash) ^ vertex1_.GetHashCode();
			hash = ((hash << 5) + hash) ^ vertex2_.GetHashCode();
			return ((hash << 5) + hash) ^ vertex3_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Quadrilateral3 other, float tolerance)
		{
			return vertex0_.ApproxEquals(other.vertex0_, tolerance) &&
				vertex1_.ApproxEquals(other.vertex1_, tolerance) &&
				vertex2_.ApproxEquals(other.vertex2_, tolerance) &&
				vertex3_.ApproxEquals(other.vertex3_, tolerance);
		}

		public readonly bool ApproxEquals(in Quadrilateral3 other)
		{
			return vertex0_.ApproxEquals(other.vertex0_) &&
				vertex1_.ApproxEquals(other.vertex1_) &&
				vertex2_.ApproxEquals(other.vertex2_) &&
				vertex3_.ApproxEquals(other.vertex3_);
		}

		public readonly override string ToString()
		{
			return String.Concat(vertex0_.ToString(), " ", vertex1_.ToString(), " ", vertex2_.ToString(), " ", vertex3_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(vertex0_.ToString(provider), " ", vertex1_.ToString(provider), " ", vertex2_.ToString(provider), " ", 
				vertex3_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(vertex0_.ToString(format), " ", vertex1_.ToString(format), " ", vertex2_.ToString(format), " ", 
				vertex3_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(vertex0_.ToString(format, provider), " ", vertex1_.ToString(format, provider), " ", 
				vertex2_.ToString(format, provider), " ", vertex3_.ToString(format, provider));
		}

		public static Quadrilateral3 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 12)
				throw new FormatException();

			return new Quadrilateral3(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
				new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])),
				new Vector3(Single.Parse(m[6]), Single.Parse(m[7]), Single.Parse(m[8])),
				new Vector3(Single.Parse(m[9]), Single.Parse(m[10]), Single.Parse(m[11])));
		}

		public static Quadrilateral3 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 12)
				throw new FormatException();

			return new Quadrilateral3(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
				new Vector3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider)),
				new Vector3(Single.Parse(m[6], provider), Single.Parse(m[7], provider), Single.Parse(m[8], provider)),
				new Vector3(Single.Parse(m[9], provider), Single.Parse(m[10], provider), Single.Parse(m[11], provider)));
		}

		[Browsable(false)]
		public readonly bool IsFinite => vertex0_.IsFinite && vertex1_.IsFinite && vertex2_.IsFinite && vertex3_.IsFinite;

		public Vector3 Vertex0
        {
			readonly get => vertex0_;
            set => vertex0_ = value;
        }

        public Vector3 Vertex1
        {
			readonly get => vertex1_;
            set => vertex1_ = value;
        }

        public Vector3 Vertex2
        {
			readonly get => vertex2_;
            set => vertex2_ = value;
        }

		public Vector3 Vertex3
		{
			readonly get => vertex3_;
			set => vertex3_ = value;
		}

		public readonly IEnumerable<Vector3> GetVertices()
		{
			yield return vertex0_;
			yield return vertex1_;
			yield return vertex2_;
			yield return vertex3_;
		}

		[Browsable(false)]
		public readonly float Perimeter
		{
			get
			{
				return Vector3.Distance(vertex0_, vertex1_) + Vector3.Distance(vertex1_, vertex2_) +
					Vector3.Distance(vertex2_, vertex3_) + Vector3.Distance(vertex3_, vertex0_);
			}
		}

		[Browsable(false)]
		public readonly float Area
		{
			get
			{
				Vector3 p = vertex2_ - vertex0_;
				Vector3 q = vertex3_ - vertex1_;
				return Vector3.Cross(p, q).Magnitude*0.5f;
			}
		}

		[Browsable(false)]
		public readonly Vector3 Normal
		{
			get 
			{ 
				return Vector3.Normalize(Vector3.Cross(vertex1_ - vertex0_, vertex2_ - vertex0_) +
					Vector3.Cross(vertex2_ - vertex0_, vertex3_ - vertex0_)); 
			}
		}

		public readonly AxisAlignedBox GetCircumscribedBox()
		{
			return new AxisAlignedBox(Vector3.Min(Vector3.Min(Vector3.Min(vertex0_, vertex1_), vertex2_), vertex3_),
				Vector3.Max(Vector3.Max(Vector3.Max(vertex0_, vertex1_), vertex2_), vertex3_));
		}

		public static Vector3 ComputeNormal(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
		{
			return Vector3.Normalize(Vector3.Cross(vertex1 - vertex0, vertex2 - vertex0) +
				Vector3.Cross(vertex2 - vertex0, vertex3 - vertex0));
		}

		public void Translate(Vector3 offset)
		{
			vertex0_ += offset;
			vertex1_ += offset;
			vertex2_ += offset;
			vertex3_ += offset;
		}

		public static Quadrilateral3 Translate(Quadrilateral3 q, Vector3 offset)
		{
			q.Translate(offset);
			return q;
		}

		public void Transform(in AffineTransform at)
        {
            vertex0_.Transform(at);
            vertex1_.Transform(at);
            vertex2_.Transform(at);
			vertex3_.Transform(at);
		}

		public static Quadrilateral3 Transform(Quadrilateral3 q, in AffineTransform at)
		{
			q.Transform(at);
			return q;
		}

		public void Flip()
		{
			Vector3 v = vertex0_;
			vertex0_ = vertex3_;
			vertex3_ = v;
			v = vertex1_;
			vertex1_ = vertex2_;
			vertex2_ = v;
		}

		public static Quadrilateral3 Flip(Quadrilateral3 q)
		{
			q.Flip();
			return q;
		}

		internal Vector3 vertex0_;
		internal Vector3 vertex1_;
		internal Vector3 vertex2_;
		internal Vector3 vertex3_;
	}
}
