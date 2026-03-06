/*
 *  Name: Tetrahedron
 *  Author: Pawel Mrochen
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Tetrahedron : IFormattable, IEquatable<Tetrahedron>
	{
		public Tetrahedron(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            vertex0_ = vertex0;
            vertex1_ = vertex1;
            vertex2_ = vertex2;
			vertex3_ = vertex3;
		}

		public Tetrahedron(Vector3[] vertices) :
			this(vertices[0], vertices[1], vertices[2], vertices[3])
		{
		}

		public static bool operator ==(Tetrahedron lhs, Tetrahedron rhs)
		{
			return (lhs.vertex0_ == rhs.vertex0_) && (lhs.vertex1_ == rhs.vertex1_) && (lhs.vertex2_ == rhs.vertex2_) && 
				(lhs.vertex3_ == rhs.vertex3_);
		}

		public static bool operator !=(Tetrahedron lhs, Tetrahedron rhs)
		{
			return (lhs.vertex0_ != rhs.vertex0_) || (lhs.vertex1_ != rhs.vertex1_) || (lhs.vertex2_ != rhs.vertex2_) || 
				(lhs.vertex3_ != rhs.vertex3_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Tetrahedron rhs)
			{
				return (vertex0_ == rhs.vertex0_) && (vertex1_ == rhs.vertex1_) && (vertex2_ == rhs.vertex2_) && 
					(vertex3_ == rhs.vertex3_);
			}

			return false;
		}

		public readonly bool Equals(Tetrahedron other)
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

		public readonly bool ApproxEquals(in Tetrahedron other, float tolerance)
		{
			return vertex0_.ApproxEquals(other.vertex0_, tolerance) &&
				vertex1_.ApproxEquals(other.vertex1_, tolerance) &&
				vertex2_.ApproxEquals(other.vertex2_, tolerance) &&
				vertex3_.ApproxEquals(other.vertex3_, tolerance);
		}

		public readonly bool ApproxEquals(in Tetrahedron other)
		{
			return vertex0_.ApproxEquals(other.vertex0_) &&
				vertex1_.ApproxEquals(other.vertex1_) &&
				vertex2_.ApproxEquals(other.vertex2_) &&
				vertex3_.ApproxEquals(other.vertex3_);
		}

		public readonly override string ToString()
		{
			return String.Concat(vertex0_.ToString(), " ", vertex1_.ToString(), " ", vertex2_.ToString(), 
				" ", vertex3_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(vertex0_.ToString(provider), " ", vertex1_.ToString(provider), " ", vertex2_.ToString(provider), 
				" ", vertex3_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(vertex0_.ToString(format), " ", vertex1_.ToString(format), " ", vertex2_.ToString(format), 
				" ", vertex3_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(vertex0_.ToString(format, provider), " ", vertex1_.ToString(format, provider), 
				" ", vertex2_.ToString(format, provider), " ", vertex3_.ToString(format, provider));
		}

		public static Tetrahedron Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 12)
				throw new FormatException();

			return new Tetrahedron(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
				new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])),
				new Vector3(Single.Parse(m[6]), Single.Parse(m[7]), Single.Parse(m[8])),
				new Vector3(Single.Parse(m[9]), Single.Parse(m[10]), Single.Parse(m[11])));
		}

		public static Tetrahedron Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 12)
				throw new FormatException();

			return new Tetrahedron(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
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
		public readonly Vector3 Centroid => (vertex0_ + vertex1_ + vertex2_ + vertex3_)*0.25f;

		//[Browsable(false)] // #TODO
		//public readonly float SurfaceArea;

		[Browsable(false)]
		public readonly float Volume
		{
			get => Math.Abs(Vector3.Dot(vertex0_ - vertex3_, Vector3.Cross(vertex1_ - vertex3_, vertex2_ - vertex3_)))/6.0f;
		}

		public readonly AxisAlignedBox GetCircumscribedBox()
		{
			return new AxisAlignedBox(Vector3.Min(Vector3.Min(Vector3.Min(vertex0_, vertex1_), vertex2_), vertex3_),
				Vector3.Max(Vector3.Max(Vector3.Max(vertex0_, vertex1_), vertex2_), vertex3_));
		}

		public void Translate(Vector3 offset)
		{
			vertex0_ += offset;
			vertex1_ += offset;
			vertex2_ += offset;
			vertex3_ += offset;
		}

		public static Tetrahedron Translate(Tetrahedron tetrahedron, Vector3 offset)
		{
			tetrahedron.Translate(offset);
			return tetrahedron;
		}

		public void Transform(in Matrix3 matrix)
		{
			vertex0_.Transform(matrix);
			vertex1_.Transform(matrix);
			vertex2_.Transform(matrix);
			vertex3_.Transform(matrix);
		}

		public void Transform(in AffineTransform at)
        {
            vertex0_.Transform(at);
            vertex1_.Transform(at);
            vertex2_.Transform(at);
			vertex3_.Transform(at);
		}

		public static Tetrahedron Transform(Tetrahedron tetrahedron, in Matrix3 matrix)
		{
			tetrahedron.Transform(matrix);
			return tetrahedron;
		}

		public static Tetrahedron Transform(Tetrahedron tetrahedron, in AffineTransform at)
		{
			tetrahedron.Transform(at);
			return tetrahedron;
		}

		public void Flip()
		{
			Vector3 v = vertex0_;
			vertex0_ = vertex1_;
			vertex1_ = v;
		}

		public static Tetrahedron Flip(Tetrahedron t)
		{
			t.Flip();
			return t;
		}

		internal Vector3 vertex0_;
		internal Vector3 vertex1_;
		internal Vector3 vertex2_;
		internal Vector3 vertex3_;
	}
}
