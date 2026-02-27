/*
 *  Name: Triangle3
 *  Author: Pawel Mrochen
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	//[Serializable]
	[TypeConverter(typeof(ValueTypeConverter))]
	public struct Triangle3 : IFormattable, IEquatable<Triangle3>
	{
        public Triangle3(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
        {
            vertex0_ = vertex0;
            vertex1_ = vertex1;
            vertex2_ = vertex2;
        }

		public Triangle3(Vector3[] vertices) :
			this(vertices[0], vertices[1], vertices[2])
		{
		}

		public static bool operator ==(Triangle3 lhs, Triangle3 rhs)
		{
			return (lhs.vertex0_ == rhs.vertex0_) && (lhs.vertex1_ == rhs.vertex1_) && (lhs.vertex2_ == rhs.vertex2_);
		}

		public static bool operator !=(Triangle3 lhs, Triangle3 rhs)
		{
			return (lhs.vertex0_ != rhs.vertex0_) || (lhs.vertex1_ != rhs.vertex1_) || (lhs.vertex2_ != rhs.vertex2_);
		}

		public readonly override bool Equals(object other)
		{
			if (other is Triangle3 rhs)
				return (vertex0_ == rhs.vertex0_) && (vertex1_ == rhs.vertex1_) && (vertex2_ == rhs.vertex2_);

			return false;
		}

		public readonly bool Equals(Triangle3 other)
		{
			return (vertex0_ == other.vertex0_) && (vertex1_ == other.vertex1_) && (vertex2_ == other.vertex2_);
		}

		public readonly override int GetHashCode()
		{
			int hash = vertex0_.GetHashCode();
			hash = ((hash << 5) + hash) ^ vertex1_.GetHashCode();
			return ((hash << 5) + hash) ^ vertex2_.GetHashCode();
		}

		public readonly bool ApproxEquals(in Triangle3 other, float tolerance)
		{
			return vertex0_.ApproxEquals(other.vertex0_, tolerance) &&
				vertex1_.ApproxEquals(other.vertex1_, tolerance) &&
				vertex2_.ApproxEquals(other.vertex2_, tolerance);
		}

		public readonly bool ApproxEquals(in Triangle3 other)
		{
			return vertex0_.ApproxEquals(other.vertex0_) &&
				vertex1_.ApproxEquals(other.vertex1_) &&
				vertex2_.ApproxEquals(other.vertex2_);
		}

		public readonly override string ToString()
		{
			return String.Concat(vertex0_.ToString(), " ", vertex1_.ToString(), " ", vertex2_.ToString());
		}

		public readonly string ToString(IFormatProvider provider)
		{
			return String.Concat(vertex0_.ToString(provider), " ", vertex1_.ToString(provider), " ", vertex2_.ToString(provider));
		}

		public readonly string ToString(string format)
		{
			return String.Concat(vertex0_.ToString(format), " ", vertex1_.ToString(format), " ", vertex2_.ToString(format));
		}

		public readonly string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(vertex0_.ToString(format, provider), " ", vertex1_.ToString(format, provider), " ", vertex2_.ToString(format, provider));
		}

		public static Triangle3 Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 9)
				throw new FormatException();

			return new Triangle3(new Vector3(Single.Parse(m[0]), Single.Parse(m[1]), Single.Parse(m[2])),
				new Vector3(Single.Parse(m[3]), Single.Parse(m[4]), Single.Parse(m[5])),
				new Vector3(Single.Parse(m[6]), Single.Parse(m[7]), Single.Parse(m[8])));
		}

		public static Triangle3 Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] m = str.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
			if (m.Length != 9)
				throw new FormatException();

			return new Triangle3(new Vector3(Single.Parse(m[0], provider), Single.Parse(m[1], provider), Single.Parse(m[2], provider)),
				new Vector3(Single.Parse(m[3], provider), Single.Parse(m[4], provider), Single.Parse(m[5], provider)),
				new Vector3(Single.Parse(m[6], provider), Single.Parse(m[7], provider), Single.Parse(m[8], provider)));
		}

		[Browsable(false)]
		public readonly bool IsFinite => vertex0_.IsFinite && vertex1_.IsFinite && vertex2_.IsFinite;

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

		//public Vector3[] GetVertices()
		//{
		//    return new Vector3[3] { vertex0_, vertex1_, vertex2_ };
		//}

		public readonly IEnumerable<Vector3> GetVertices()
		{
			yield return vertex0_;
			yield return vertex1_;
			yield return vertex2_;
		}

		[Browsable(false)]
		public readonly float Perimeter
		{
			get
			{
				return Vector3.Distance(vertex0_, vertex1_) + Vector3.Distance(vertex1_, vertex2_) + 
					Vector3.Distance(vertex2_, vertex0_);
			}
		}

		[Browsable(false)]
		public readonly float Area
		{
			get
			{
				Vector3 v1 = Vector3.Cross(vertex0_, vertex1_);
				Vector3 v2 = Vector3.Cross(vertex1_, vertex2_);
				Vector3 v3 = Vector3.Cross(vertex2_, vertex0_);
				return (v1 + v2 + v3).Magnitude*0.5f;
			}
		}

		[Browsable(false)]
		public readonly Vector3 Normal => Vector3.Normalize(Vector3.Cross(vertex1_ - vertex0_, vertex2_ - vertex0_)); 

		public readonly AxisAlignedBox GetCircumscribedBox()
		{
			return new AxisAlignedBox(Vector3.Min(Vector3.Min(vertex0_, vertex1_), vertex2_),
				Vector3.Max(Vector3.Max(vertex0_, vertex1_), vertex2_));
		}

		public readonly Sphere GetCircumscribedSphere()
		{
			float d1 = Vector3.Dot(vertex2_ - vertex0_, vertex1_ - vertex0_);
			float d2 = Vector3.Dot(vertex2_ - vertex1_, vertex0_ - vertex1_);
			float d3 = Vector3.Dot(vertex0_ - vertex2_, vertex1_ - vertex2_);

			float c1 = d2*d3;
			float c2 = d3*d1;
			float c3 = d1*d2;
			float invC = 1f/(c1 + c2 + c3);

			return new Sphere((vertex0_*(c2 + c3) + vertex1_*(c3 + c1) + vertex2_*(c1 + c2))*invC*0.5f,
				0.5f*MathF.Sqrt((d1 + d2)*(d2 + d3)*(d3 + d1)*invC));
		}

		public static Vector3 ComputeNormal(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
		{
			return Vector3.Normalize(Vector3.Cross(vertex1 - vertex0, vertex2 - vertex0));
		}

		public readonly Matrix3 GetTangentSpaceBasis(Vector2 uv0, Vector2 uv1, Vector2 uv2, bool weightedByArea)
		{
			Vector3 tangent, bitangent;

			Vector3 e1 = vertex1_ - vertex0_;
			Vector3 e2 = vertex2_ - vertex0_;
			Vector3 normal = Vector3.Normalize(Vector3.Cross(e1, e2));

			Vector2 delta1 = uv1 - uv0;
			Vector2 delta2 = uv2 - uv0;
			float d = Vector2.Cross(delta1, delta2);

			if (/*!Single.IsNaN(d) &&*/ (d != 0f))
			{
				tangent = Vector3.Normalize(e1*(delta2.Y/d) + e2*(-delta1.Y/d));
				bitangent = Vector3.Normalize(e1*(-delta2.X/d) + e2*(delta1.X/d));

				if (weightedByArea)
				{
					float area = Math.Abs(d)*0.5f;
					tangent *= area; // tangent and bitangent become weighted by the uv triangle size
					bitangent *= area;
				}
			}
			else
			{
				tangent = Vector3.Zero;
				bitangent = Vector3.Zero;
			}

			return new Matrix3(tangent, bitangent, normal);
		}

		public static Matrix3 ComputeTangentSpaceBasis(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2,
			Vector2 uv0, Vector2 uv1, Vector2 uv2, bool weightedByArea)
		{
			Vector3 tangent, bitangent;

			Vector3 e1 = vertex1 - vertex0;
			Vector3 e2 = vertex2 - vertex0;
			Vector3 normal = Vector3.Normalize(Vector3.Cross(e1, e2));

			Vector2 delta1 = uv1 - uv0;
			Vector2 delta2 = uv2 - uv0;
			float d = Vector2.Cross(delta1, delta2);

			if (/*!Single.IsNaN(d) &&*/ (d != 0f))
			{
				tangent = Vector3.Normalize(e1*(delta2.Y/d) + e2*(-delta1.Y/d));
				bitangent = Vector3.Normalize(e1*(-delta2.X/d) + e2*(delta1.X/d));

				if (weightedByArea)
				{
					float area = Math.Abs(d)*0.5f;
					tangent *= area; // tangent and bitangent become weighted by the uv triangle size
					bitangent *= area;
				}
			}
			else
			{
				tangent = Vector3.Zero;
				bitangent = Vector3.Zero;
			}

			return new Matrix3(tangent, bitangent, normal);
		}

		public void Translate(Vector3 offset)
		{
			vertex0_ += offset;
			vertex1_ += offset;
			vertex2_ += offset;
		}

		public static Triangle3 Translate(Triangle3 t, Vector3 offset)
		{
			t.Translate(offset);
			return t;
		}

		public void Transform(in AffineTransform at)
        {
            vertex0_.Transform(at);
            vertex1_.Transform(at);
            vertex2_.Transform(at);
        }

		public static Triangle3 Transform(Triangle3 t, in AffineTransform at)
		{
			t.Transform(at);
			return t;
		}

		public void Flip()
		{
			Vector3 v = vertex0_;
			vertex0_ = vertex2_;
			vertex2_ = v;
		}

		public static Triangle3 Flip(Triangle3 t)
		{
			t.Flip();
			return t;
		}

		public readonly Vector3 Evaluate(float v, float w)
		{
			return vertex0_*(1f - v - w) + vertex1_*v + vertex2_*w;
		}

		public readonly Vector3 Evaluate(float u, float v, float w)
		{
			return vertex0_*u + vertex1_*v + vertex2_*w;
		}

		public readonly Vector3 GetBarycentricCoords(Vector3 point)
		{
			Vector3 edge1 = vertex1_ - vertex0_;
			Vector3 edge2 = vertex2_ - vertex0_;
			float m = 1f/Vector3.Cross(edge1, edge2).MagnitudeSquared;
			float v = MathF.Sqrt(Vector3.Cross(edge2, point - vertex2_).MagnitudeSquared*m);
			float w = MathF.Sqrt(Vector3.Cross(edge1, point - vertex1_).MagnitudeSquared*m);
			return new Vector3(1f - v - w, v, w);
		}

		public readonly Vector3 GetClosestPoint(Vector3 point)
		{
			Vector3 closestPoint;
			DistanceSquaredPointTriangle(point, vertex0_, vertex1_, vertex2_, out closestPoint);
			return closestPoint;
		}

		public readonly float GetDistanceTo(Vector3 point)
		{
			Vector3 closestPoint;
			return MathF.Sqrt(DistanceSquaredPointTriangle(point, vertex0_, vertex1_, vertex2_, out closestPoint));
		}

		public readonly float GetDistanceSquaredTo(Vector3 point)
		{
			Vector3 closestPoint;
			return DistanceSquaredPointTriangle(point, vertex0_, vertex1_, vertex2_, out closestPoint);
		}

		//public readonly bool Intersects(Plane plane)
		//{
		//	// #TODO Check if all vertices are on one side
		//	float d0 = plane.GetSignedDistanceTo(vertex0_);
		//	float d1 = plane.GetSignedDistanceTo(vertex1_);
		//	float d2 = plane.GetSignedDistanceTo(vertex2_);
		//	return !(((d0 < 0f) && (d1 < 0f) && (d2 < 0f)) || ((d0 > 0f) && (d1 > 0f) && (d2 > 0f)));
		//}

		public readonly bool Intersects(in HalfSpace halfSpace)
		{
			return halfSpace.Contains(vertex0_) || halfSpace.Contains(vertex1_) || halfSpace.Contains(vertex2_);
		}

		public readonly bool Intersects(in Line3 line)
        {
            return FindIntersection(line).HasValue;
        }

        public readonly bool Intersects(in Ray3 ray)
        {
			return FindIntersection(ray).HasValue;
        }

        public readonly bool Intersects(in Segment3 segment)
        {
			return FindIntersection(segment).HasValue;
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
			return (GetDistanceSquaredTo(sphere.Center) <= sphere.Radius*sphere.Radius);
		}

		public readonly float? FindIntersection(in Line3 line)
        {
			// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/

			Vector3 edge1 = vertex1_ - vertex0_;
            Vector3 edge2 = vertex2_ - vertex0_;
            Vector3 pvec = Vector3.Cross(line.direction_, edge2);
	        float det = Vector3.Dot(edge1, pvec);
			if (Math.Abs(det) < 1e-6f)
				return null;

	        float invDet = 1f/det;
            Vector3 tvec = line.origin_ - vertex0_;
	        float u = Vector3.Dot(tvec, pvec)*invDet;
			if ((u < 0f) || (u > 1f))
				return null;

	        Vector3 qvec = Vector3.Cross(tvec, edge1);
	        float v = Vector3.Dot(line.direction_, qvec)*invDet;
			if ((v < 0f) || ((u + v) > 1f))
				return null;

	        return Vector3.Dot(edge2, qvec)*invDet;
        }

		public readonly float? FindIntersection(in Ray3 ray)
		{
			// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/

			Vector3 edge1 = vertex1_ - vertex0_;
			Vector3 edge2 = vertex2_ - vertex0_;
			Vector3 pvec = Vector3.Cross(ray.direction_, edge2);
			float det = Vector3.Dot(edge1, pvec);
			if (Math.Abs(det) < 1e-6f)
				return null;

			float invDet = 1f/det;
			Vector3 tvec = ray.origin_ - vertex0_;
			float u = Vector3.Dot(tvec, pvec)*invDet;
			if ((u < 0f) || (u > 1f))
				return null;

			Vector3 qvec = Vector3.Cross(tvec, edge1);
			float v = Vector3.Dot(ray.direction_, qvec)*invDet;
			if ((v < 0f) || ((u + v) > 1f))
				return null;

			float t = Vector3.Dot(edge2, qvec)*invDet;
			if (t >= 0f)
				return t;
			else
				return null;
		}

		public readonly float? FindIntersection(in Segment3 segment)
        {
			// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/

			Vector3 edge1 = vertex1_ - vertex0_;
			Vector3 edge2 = vertex2_ - vertex0_;
			Vector3 direction = segment.end_ - segment.start_;
			Vector3 pvec = Vector3.Cross(direction, edge2);
			float det = Vector3.Dot(edge1, pvec);
			if (Math.Abs(det) < 1e-6f)
				return null;

			float invDet = 1f/det;
			Vector3 tvec = segment.start_ - vertex0_;
			float u = Vector3.Dot(tvec, pvec)*invDet;
			if ((u < 0f) || (u > 1f))
				return null;

			Vector3 qvec = Vector3.Cross(tvec, edge1);
			float v = Vector3.Dot(direction, qvec)*invDet;
			if ((v < 0f) || ((u + v) > 1f))
				return null;

			float t = Vector3.Dot(edge2, qvec)*invDet;
			if ((t >= 0f) && (t <= 1f))
				return t;
			else
				return null;
		}

		private static float DistanceSquaredPointTriangle(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2, out Vector3 closestPoint)
		{
			// http://www.geometrictools.com/

			Vector3 diff = v0 - point;
			Vector3 edge0 = v1 - v0;
			Vector3 edge1 = v2 - v0;
			float a00 = edge0.LengthSquared;
			float a01 = Vector3.Dot(edge0, edge1);
			float a11 = edge1.LengthSquared;
			float b0 = Vector3.Dot(diff, edge0);
			float b1 = Vector3.Dot(diff, edge1);
			float c = diff.LengthSquared;
			float det = Math.Abs(a00*a11 - a01*a01);
			float s = a01*b1 - a11*b0;
			float t = a01*b0 - a00*b1;
			float sqrDistance;

			if (s + t <= det)
			{
				if (s < 0.0f)
				{
					if (t < 0.0f)
					{
						if (b0 < 0.0f)
						{
							t = 0.0f;
							if (-b0 >= a00)
							{
								s = 1.0f;
								sqrDistance = a00 + (2.0f)*b0 + c;
							}
							else
							{
								s = -b0/a00;
								sqrDistance = b0*s + c;
							}
						}
						else
						{
							s = 0.0f;
							if (b1 >= 0.0f)
							{
								t = 0.0f;
								sqrDistance = c;
							}
							else if (-b1 >= a11)
							{
								t = 1.0f;
								sqrDistance = a11 + 2.0f*b1 + c;
							}
							else
							{
								t = -b1/a11;
								sqrDistance = b1*t + c;
							}
						}
					}
					else
					{
						s = 0.0f;
						if (b1 >= 0.0f)
						{
							t = 0.0f;
							sqrDistance = c;
						}
						else if (-b1 >= a11)
						{
							t = 1.0f;
							sqrDistance = a11 + 2.0f*b1 + c;
						}
						else
						{
							t = -b1/a11;
							sqrDistance = b1*t + c;
						}
					}
				}
				else if (t < 0.0f)
				{
					t = 0.0f;
					if (b0 >= 0.0f)
					{
						s = 0.0f;
						sqrDistance = c;
					}
					else if (-b0 >= a00)
					{
						s = 1.0f;
						sqrDistance = a00 + 2.0f*b0 + c;
					}
					else
					{
						s = -b0/a00;
						sqrDistance = b0*s + c;
					}
				}
				else
				{
					float invDet = 1.0f/det;
					s *= invDet;
					t *= invDet;
					sqrDistance = s*(a00*s + a01*t + 2.0f*b0) + t*(a01*s + a11*t + 2.0f*b1) + c;
				}
			}
			else
			{
				if (s < 0.0f)
				{
					float tmp0 = a01 + b0;
					float tmp1 = a11 + b1;
					if (tmp1 > tmp0)
					{
						float numer = tmp1 - tmp0;
						float denom = a00 - 2.0f*a01 + a11;
						if (numer >= denom)
						{
							s = 1.0f;
							t = 0.0f;
							sqrDistance = a00 + 2.0f*b0 + c;
						}
						else
						{
							s = numer/denom;
							t = 1.0f - s;
							sqrDistance = s*(a00*s + a01*t + 2.0f*b0) + t*(a01*s + a11*t + 2.0f*b1) + c;
						}
					}
					else
					{
						s = 0.0f;
						if (tmp1 <= 0.0f)
						{
							t = 1.0f;
							sqrDistance = a11 + 2.0f*b1 + c;
						}
						else if (b1 >= 0.0f)
						{
							t = 0.0f;
							sqrDistance = c;
						}
						else
						{
							t = -b1/a11;
							sqrDistance = b1*t + c;
						}
					}
				}
				else if (t < 0.0f)
				{
					float tmp0 = a01 + b1;
					float tmp1 = a00 + b0;
					if (tmp1 > tmp0)
					{
						float numer = tmp1 - tmp0;
						float denom = a00 - 2.0f*a01 + a11;
						if (numer >= denom)
						{
							t = 1.0f;
							s = 0.0f;
							sqrDistance = a11 + 2.0f*b1 + c;
						}
						else
						{
							t = numer/denom;
							s = 1.0f - t;
							sqrDistance = s*(a00*s + a01*t + 2.0f*b0) + t*(a01*s + a11*t + 2.0f*b1) + c;
						}
					}
					else
					{
						t = 0.0f;
						if (tmp1 <= 0.0f)
						{
							s = 1.0f;
							sqrDistance = a00 + 2.0f*b0 + c;
						}
						else if (b0 >= 0.0f)
						{
							s = 0.0f;
							sqrDistance = c;
						}
						else
						{
							s = -b0/a00;
							sqrDistance = b0*s + c;
						}
					}
				}
				else
				{
					float numer = a11 + b1 - a01 - b0;
					if (numer <= 0.0f)
					{
						s = 0.0f;
						t = 1.0f;
						sqrDistance = a11 + 2.0f*b1 + c;
					}
					else
					{
						float denom = a00 - 2.0f*a01 + a11;
						if (numer >= denom)
						{
							s = 1.0f;
							t = 0.0f;
							sqrDistance = a00 + 2.0f*b0 + c;
						}
						else
						{
							s = numer/denom;
							t = 1.0f - s;
							sqrDistance = s*(a00*s + a01*t + 2.0f*b0) + t*(a01*s + a11*t + 2.0f*b1) + c;
						}
					}
				}
			}

			if (sqrDistance < 0.0f)
				sqrDistance = 0.0f;

			closestPoint = v0 + s*edge0 + t*edge1;
			//triangleBary[1] = s;
			//triangleBary[2] = t;
			//triangleBary[0] = 1.0f - s - t;
    
			return sqrDistance;
		}

		internal Vector3 vertex0_;
		internal Vector3 vertex1_;
		internal Vector3 vertex2_;
	}
}
