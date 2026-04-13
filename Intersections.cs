/*
 *  Name: Intersections
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	internal static class Intersections
	{
		public static bool TestLineLine(Vector2 originA, Vector2 directionA, Vector2 originB, Vector2 directionB)
		{
			return !(Math.Abs(Vector2.Cross(directionA, directionB)) < SingleConstants.Tolerance) ||
				(Math.Abs(Vector2.Cross(Vector2.Normalize(originB - originA), directionA)) < SingleConstants.Tolerance);
		}

		public static float? FindLineLine(Vector2 originA, Vector2 directionA, Vector2 originB, Vector2 directionB)
		{
			// http://www.geometrictools.com/

			Vector2 diff = originB - originA;
			float d1CrossD2 = Vector2.Cross(directionA, directionB);
			if (Math.Abs(d1CrossD2) < SingleConstants.Tolerance)
			{
				if (!(Math.Abs(Vector2.Cross(Vector2.Normalize(diff), directionA)) < SingleConstants.Tolerance))
					return null;
				return 0f; // collinear
			}

			return Vector2.Cross(diff, directionB)/d1CrossD2;
		}

		public static float? FindLineRay(Vector2 originA, Vector2 directionA, Vector2 originB, Vector2 directionB)
		{
			// http://www.geometrictools.com/

			Vector2 diff = originA - originB;
			float d2CrossD1 = Vector2.Cross(directionB, directionA);
			if (Math.Abs(d2CrossD1) < SingleConstants.Tolerance)
			{
				if (!(Math.Abs(Vector2.Cross(Vector2.Normalize(diff), directionB)) < SingleConstants.Tolerance))
					return null;
				return 0f; // collinear
			}

			float t = Vector2.Cross(diff, directionA)/d2CrossD1;
			if (t >= 0f)
				return t;
			return null;
		}

		public static float? FindLineLineSegment(Vector2 origin, Vector2 direction, Vector2 start, Vector2 end)
		{
			// http://www.geometrictools.com/

			Vector2 directionB = end - start;
			Vector2 diff = origin - start;
			float d2CrossD1 = Vector2.Cross(directionB, direction);
			if (Math.Abs(d2CrossD1) < SingleConstants.Tolerance)
			{
				if (!(Math.Abs(Vector2.Cross(Vector2.Normalize(diff), directionB)) < SingleConstants.Tolerance))
					return null;
				return 0f; // collinear
			}

			float t = Vector2.Cross(diff, direction)/d2CrossD1;
			if ((t >= 0f) && (t <= 1f))
				return t;
			return null;
		}

		public static float? FindLineSegmentLineSegment(Vector2 startA, Vector2 endA, Vector2 startB, Vector2 endB)
		{
			// http://www.geometrictools.com/

			Vector2 directionA = endA - startA;
			Vector2 directionB = endB - startB;
			Vector2 diff = startB - startA;
			float d1CrossD2 = Vector2.Cross(directionA, directionB);
			if (Math.Abs(d1CrossD2) < SingleConstants.Tolerance)
			{
				if (!(Math.Abs(Vector2.Cross(Vector2.Normalize(diff), directionA)) < SingleConstants.Tolerance))
					return null;

				float d1DotD1 = Vector2.Dot(directionA, directionA);
				float d1DotD2 = Vector2.Dot(directionA, directionB);
				float t0 = Vector2.Dot(diff, directionA)/d1DotD1;
				float t1 = t0 + d1DotD2/d1DotD1;
				if (d1DotD2 < 0f)
				{
					float s = t0;
					t0 = t1;
					t1 = s;
				}

				if ((t0 <= 1f) && (t1 >= 0f))
					return Math.Max(t0, 0f);
				return null;
			}

			float t = Vector2.Cross(diff, directionB)/d1CrossD2;
			float u = Vector2.Cross(diff, directionA)/d1CrossD2;
			if ((t >= 0f) && (t <= 1f) && (u >= 0f) && (u <= 1f))
				return t;
			return null;
		}

		public static bool TestLinePlane(Vector3 direction, Vector3 normal)
		{
			return (Math.Abs(Vector3.Dot(normal, direction)) >= 1e-6f);
		}

		public static float? FindLinePlane(Vector3 origin, Vector3 direction, Vector3 normal, float constant)
		{
			float nDotD = Vector3.Dot(normal, direction);
			if (Math.Abs(nDotD) < SingleConstants.Tolerance)
				return null;

			return (-constant - Vector3.Dot(normal, origin))/nDotD;
		}

		public static float? FindLineTriangle(Vector3 origin, Vector3 direction, Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
		{
			// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/

			Vector3 edge1 = vertex1 - vertex0;
			Vector3 edge2 = vertex2 - vertex0;
			Vector3 pVec = Vector3.Cross(direction, edge2);
			float det = Vector3.Dot(edge1, pVec);
			if (Math.Abs(det) < 1e-6f)
				return null;

			float invDet = 1f/det;
			Vector3 tVec = origin - vertex0;
			float u = Vector3.Dot(tVec, pVec)*invDet;
			if ((u < 0f) || (u > 1f))
				return null;

			Vector3 qVec = Vector3.Cross(tVec, edge1);
			float v = Vector3.Dot(direction, qVec)*invDet;
			if ((v < 0f) || ((u + v) > 1f))
				return null;

			return Vector3.Dot(edge2, qVec)*invDet;
		}

		//public static float? FindRayTriangle(Vector3 origin, Vector3 direction, Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
		//{
		//	// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/

		//	Vector3 edge1 = vertex1 - vertex0;
		//	Vector3 edge2 = vertex2 - vertex0;
		//	Vector3 pVec = Vector3.Cross(direction, edge2);
		//	float det = Vector3.Dot(edge1, pVec);
		//	if (Math.Abs(det) < 1e-6f)
		//		return null;

		//	float invDet = 1f/det;
		//	Vector3 tVec = origin - vertex0;
		//	float u = Vector3.Dot(tVec, pVec)*invDet;
		//	if ((u < 0f) || (u > 1f))
		//		return null;

		//	Vector3 qVec = Vector3.Cross(tVec, edge1);
		//	float v = Vector3.Dot(direction, qVec)*invDet;
		//	if ((v < 0f) || ((u + v) > 1f))
		//		return null;

		//	float t = Vector3.Dot(edge2, qVec)*invDet;
		//	if (t >= 0f)
		//		return t;
		//	else
		//		return null;
		//}

		//public static float? FindLineSegmentTriangle(Vector3 start, Vector3 end, Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
		//{
		//	// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/

		//	Vector3 edge1 = vertex1 - vertex0;
		//	Vector3 edge2 = vertex2 - vertex0;
		//	Vector3 direction = end - start;
		//	Vector3 pVec = Vector3.Cross(direction, edge2);
		//	float det = Vector3.Dot(edge1, pVec);
		//	if (Math.Abs(det) < 1e-6f)
		//		return null;

		//	float invDet = 1f/det;
		//	Vector3 tVec = start - vertex0;
		//	float u = Vector3.Dot(tVec, pVec)*invDet;
		//	if ((u < 0f) || (u > 1f))
		//		return null;

		//	Vector3 qVec = Vector3.Cross(tVec, edge1);
		//	float v = Vector3.Dot(direction, qVec)*invDet;
		//	if ((v < 0f) || ((u + v) > 1f))
		//		return null;

		//	float t = Vector3.Dot(edge2, qVec)*invDet;
		//	if ((t >= 0f) && (t <= 1f))
		//		return t;
		//	else
		//		return null;
		//}

		public static Interval? FindLineAxisAlignedRectangle(Vector2 origin, Vector2 direction, Vector2 minimum, Vector2 maximum) // #TODO SIMD
		{
			Vector2 invDir = 1f/direction;

			float tMin, tMax;
			if (invDir/*direction*/.x_ >= 0f)
			{
				tMin = (minimum.x_ - origin.x_)*invDir.x_;
				tMax = (maximum.x_ - origin.x_)*invDir.x_;
			}
			else
			{
				tMin = (maximum.x_ - origin.x_)*invDir.x_;
				tMax = (minimum.x_ - origin.x_)*invDir.x_;
			}

			float tyMin, tyMax;
			if (invDir/*direction*/.y_ >= 0f)
			{
				tyMin = (minimum.y_ - origin.y_)*invDir.y_;
				tyMax = (maximum.y_ - origin.y_)*invDir.y_;
			}
			else
			{
				tyMin = (maximum.y_ - origin.y_)*invDir.y_;
				tyMax = (minimum.y_ - origin.y_)*invDir.y_;
			}

			if ((tMin > tyMax) || (tyMin > tMax))
				return null;

			if (tyMin > tMin)
				tMin = tyMin;
			if (tyMax < tMax)
				tMax = tyMax;

			if (tMin > tMax)
				return null;
			else
				return new Interval(tMin, tMax);
		}

		public static Interval? FindLineAxisAlignedBox(Vector3 origin, Vector3 direction, Vector3 minimum, Vector3 maximum) // #TODO SIMD
		{
			Vector3 invDir = 1f/direction;

			float tMin, tMax;
			if (invDir/*direction*/.x_ >= 0f)
			{
				tMin = (minimum.x_ - origin.x_)*invDir.x_;
				tMax = (maximum.x_ - origin.x_)*invDir.x_;
			}
			else
			{
				tMin = (maximum.x_ - origin.x_)*invDir.x_;
				tMax = (minimum.x_ - origin.x_)*invDir.x_;
			}

			float tyMin, tyMax;
			if (invDir/*direction*/.y_ >= 0f)
			{
				tyMin = (minimum.y_ - origin.y_)*invDir.y_;
				tyMax = (maximum.y_ - origin.y_)*invDir.y_;
			}
			else
			{
				tyMin = (maximum.y_ - origin.y_)*invDir.y_;
				tyMax = (minimum.y_ - origin.y_)*invDir.y_;
			}

			if ((tMin > tyMax) || (tyMin > tMax))
				return null;

			if (tyMin > tMin)
				tMin = tyMin;
			if (tyMax < tMax)
				tMax = tyMax;

			float tzMin, tzMax;
			if (invDir/*direction*/.z_ >= 0f)
			{
				tzMin = (minimum.z_ - origin.z_)*invDir.z_;
				tzMax = (maximum.z_ - origin.z_)*invDir.z_;
			}
			else
			{
				tzMin = (maximum.z_ - origin.z_)*invDir.z_;
				tzMax = (minimum.z_ - origin.z_)*invDir.z_;
			}

			if ((tMin > tzMax) || (tzMin > tMax))
				return null;

			if (tzMin > tMin)
				tMin = tzMin;
			if (tzMax < tMax)
				tMax = tzMax;

			if (tMin > tMax)
				return null;
			else
				return new Interval(tMin, tMax);
		}

		public static Interval? FindLineOrientedBox(Vector3 origin, Vector3 direction, Vector3 center, in Matrix3 basis, 
			Vector3 halfDims)
		{
			//Matrix3 basisTranspose = Matrix3.Transpose(basis);
			return FindLineAxisAlignedBox(basis*(origin - center)/*(origin - center)*basisTranspose*/,
				basis*direction/*direction*basisTranspose*/, -halfDims, halfDims);
		}

		public static bool TestLineCircle(Vector2 origin, Vector2 direction, Vector2 center, float radius)
		{
			Vector2 diff = origin - center;
			float a = Vector2.Dot(direction, direction);
			float b = 2f*Vector2.Dot(direction, diff);
			float c = Vector2.Dot(diff, diff) - radius*radius;
			return !((b*b - 4f*a*c) < 0f);
		}

		public static Interval? FindLineCircle(Vector2 origin, Vector2 direction, Vector2 center, float radius)
		{
			Vector2 diff = origin - center;
			float a = Vector2.Dot(direction, direction);
			float b = 2f*Vector2.Dot(direction, diff);
			float c = Vector2.Dot(diff, diff) - radius*radius;
			float delta = b*b - 4f*a*c;

			if (delta < 0f)
			{
				return null;
			}
			else if (delta > 0f)
			{
				delta = MathF.Sqrt(delta);
				a = 0.5f/a;
				return new Interval((-b - delta)*a, (-b + delta)*a);
			}
			else // delta == 0
			{
				return new Interval(-b*0.5f/a);
			}
		}

		public static bool TestLineSphere(Vector3 origin, Vector3 direction, Vector3 center, float radius)
		{
			Vector3 diff = origin - center;
			float a = Vector3.Dot(direction, direction);
			float b = 2f*Vector3.Dot(direction, diff);
			float c = Vector3.Dot(diff, diff) - radius*radius;
			return !((b*b - 4f*a*c) < 0f);
		}

		public static Interval? FindLineSphere(Vector3 origin, Vector3 direction, Vector3 center, float radius)
		{
			Vector3 diff = origin - center;
			float a = Vector3.Dot(direction, direction);
			float b = 2f*Vector3.Dot(direction, diff);
			float c = Vector3.Dot(diff, diff) - radius*radius;
			float delta = b*b - 4f*a*c;

			if (delta < 0f)
			{
				return null;
			}
			else if (delta > 0f)
			{
				delta = MathF.Sqrt(delta);
				a = 0.5f/a;
				return new Interval((-b - delta)*a, (-b + delta)*a);
			}
			else // delta == 0
			{
				return new Interval(-b*0.5f/a);
			}
		}

		public static bool TestLineEllipsoid(Vector3 origin, Vector3 direction, Vector3 center, in Matrix3 matrix)
		{
			// http://www.geometrictools.com/

			Vector3 diff = origin - center;
			Vector3 matDir = direction*matrix;
			Vector3 matDiff = diff*matrix;
			float a2 = Vector3.Dot(direction, matDir);
			float a1 = Vector3.Dot(direction, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			float discr = (a1*a1 - a0*a2);
			return (discr >= 0f);
		}

		public static Interval? FindLineEllipsoid(Vector3 origin, Vector3 direction, Vector3 center, in Matrix3 matrix)
		{
			// http://www.geometrictools.com/

			Vector3 diff = origin - center;
			Vector3 matDir = direction*matrix;
			Vector3 matDiff = diff*matrix;
			float a2 = Vector3.Dot(direction, matDir);
			float a1 = Vector3.Dot(direction, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			float discr = a1*a1 - a0*a2;
			if (discr < 0f)
			{
				return null;
			}
			else if (discr > 1e-6f)
			{
				float root = MathF.Sqrt(discr);
				float inv = 1f/a2;
				return new Interval((-a1 - root)*inv, (-a1 + root)*inv);
			}
			else
			{
				return new Interval(-a1/a2);
			}
		}

		public static bool TestRayEllipsoid(Vector3 origin, Vector3 direction, Vector3 center, in Matrix3 matrix)
		{
			// http://www.geometrictools.com/

			Vector3 diff = origin - center;
			Vector3 matDir = direction*matrix;
			Vector3 matDiff = diff*matrix;
			float a2 = Vector3.Dot(direction, matDir);
			float a1 = Vector3.Dot(direction, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			float discr = a1*a1 - a0*a2;
			if (discr < 0f)
				return false;
			if (a0 <= 0f)
				return true;

			return (a1 < 0f);
		}

		public static Interval? FindRayEllipsoid(Vector3 origin, Vector3 direction, Vector3 center, in Matrix3 matrix)
		{
			// http://www.geometrictools.com/

			Vector3 diff = origin - center;
			Vector3 matDir = direction*matrix;
			Vector3 matDiff = diff*matrix;
			float a2 = Vector3.Dot(direction, matDir);
			float a1 = Vector3.Dot(direction, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			float discr = a1*a1 - a0*a2;
			if (discr < 0f)
			{
				return null;
			}
			else if (discr > 0f)
			{
				float root = MathF.Sqrt(discr);
				float inv = 1f/a2;
				float t0 = (-a1 - root)*inv;
				float t1 = (-a1 + root)*inv;

				if (t0 >= 0f)
					return new Interval(t0, t1);
				else if (t1 >= 0f)
					return new Interval(t1);
				else
					return null;
			}
			else
			{
				float t0 = -a1/a2;
				if (t0 >= 0f)
					return new Interval(t0);
				else
					return null;
			}
		}

		public static bool TestLineSegmentEllipsoid(Vector3 start, Vector3 end, Vector3 center, in Matrix3 matrix)
		{
			// http://www.geometrictools.com/

			Vector3 direction = end - start;
			Vector3 diff = (start + end)*0.5f - center;
			Vector3 matDir = direction*matrix;
			Vector3 matDiff = diff*matrix;
			float a2 = Vector3.Dot(direction, matDir);
			float a1 = Vector3.Dot(direction, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			float discr = a1*a1 - a0*a2;
			if (discr < 0f)
				return false;
			if (a0 <= 0f)
				return true;

			const float e = 0.5f;
			if (a1 >= 0f)
			{
				if (((a0 + e*(-2f*a1 + a2*e)) <= 0f) || ((a1 - a2*e) < 0f))
					return true;
			}
			else
			{
				if (((a0 + e*(2f*a1 + a2*e)) <= 0f) || ((a1 + a2*e) < 0f))
					return true;
			}

			return false;
		}

		public static Interval? FindLineSegmentEllipsoid(Vector3 start, Vector3 end, Vector3 center, in Matrix3 matrix)
		{
			// http://www.geometrictools.com/

			Vector3 direction = end - start;
			Vector3 diff = (start + end)*0.5f - center;
			Vector3 matDir = direction*matrix;
			Vector3 matDiff = diff*matrix;
			float a2 = Vector3.Dot(direction, matDir);
			float a1 = Vector3.Dot(direction, matDiff);
			float a0 = Vector3.Dot(diff, matDiff) - 1f;

			const float e = 0.5f;
			float discr = a1*a1 - a0*a2;
			if (discr < 0f)
			{
				return null;
			}
			else if (discr > 1e-6f)
			{
				float root = MathF.Sqrt(discr);
				float inv = 1f/a2;
				float t0 = (-a1 - root)*inv;
				float t1 = (-a1 + root)*inv;

				Interval? intersection = new Interval(t0, t1).FindIntersection(new Interval(-e, e));
				if (intersection.HasValue)
					return new Interval(intersection.Value.Minimum + e, intersection.Value.Maximum + e);
				else
					return null;
			}
			else
			{
				float t0 = -a1/a2;
				if (Math.Abs(t0) <= e)
					return new Interval(t0 + e);
				else
					return null;
			}
		}

		public static bool TestAxisAlignedBoxHalfSpace(Vector3 center, Vector3 halfDims, Vector3 normal, float constant)
		{
			float r = Vector3.Sum(Vector3.Abs(halfDims*normal));
			return ((Vector3.Dot(normal, center) + constant) <= r);
		}

		public static bool TestOrientedBoxHalfSpace(Vector3 center, in Matrix3 basis, Vector3 halfDims, Vector3 normal, float constant)
		{
#if SIMD
			//Matrix3 basisTranspose = Matrix3.Transpose(basis);
			//float r = Vector3.Sum(Vector3.Abs(halfDims*(normal*basisTranspose)));
			float r = Vector3.Sum(Vector3.Abs(halfDims*(basis*normal)));
#else
			float r = Math.Abs(halfDims.x_*Vector3.Dot(normal, basis.Row0)) +
				Math.Abs(halfDims.y_*Vector3.Dot(normal, basis.Row1)) +
				Math.Abs(halfDims.z_*Vector3.Dot(normal, basis.Row2));
#endif
			return ((Vector3.Dot(normal, center) + constant) <= r);
		}

		public static bool TestAxisAlignedBoxPlane(Vector3 center, Vector3 halfDims, Vector3 normal, float constant)
		{
			float r = Vector3.Sum(Vector3.Abs(halfDims*normal));
			return (Math.Abs(Vector3.Dot(normal, center) + constant) <= r);
		}

		public static bool TestOrientedBoxPlane(Vector3 center, in Matrix3 basis, Vector3 halfDims, Vector3 normal, float constant)
		{
#if SIMD
			//Matrix3 basisTranspose = Matrix3.Transpose(basis);
			//float r = Vector3.Sum(Vector3.Abs(halfDims*(normal*basisTranspose)));
			float r = Vector3.Sum(Vector3.Abs(halfDims*(basis*normal)));
#else
			float r = Math.Abs(halfDims.x_*Vector3.Dot(normal, basis.Row0)) +
				Math.Abs(halfDims.y_*Vector3.Dot(normal, basis.Row1)) +
				Math.Abs(halfDims.z_*Vector3.Dot(normal, basis.Row2));
#endif
			return (Math.Abs(Vector3.Dot(normal, center) + constant) <= r);
		}

		public static bool TestAxisAlignedBoxTriangle(Vector3 center, Vector3 halfDims, Vector3 vertex0, Vector3 vertex1, 
			Vector3 vertex2)
		{
			// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/code/

			// Move everything so that the boxcenter is in (0,0,0)
			Vector3 v0 = vertex0 - center;
			Vector3 v1 = vertex1 - center;
			Vector3 v2 = vertex2 - center;

			// Compute triangle edges
			Vector3 e0 = v1 - v0;
			Vector3 e1 = v2 - v1;
			Vector3 e2 = v0 - v2;

			float fex = Math.Abs(e0.X);
			float fey = Math.Abs(e0.Y);
			float fez = Math.Abs(e0.Z);
			if (!TestAxisX01(e0.Z, e0.Y, fez, fey, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisY02(e0.Z, e0.X, fez, fex, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisZ12(e0.Y, e0.X, fey, fex, halfDims, v0, v1, v2))
				return false;
	
			fex = Math.Abs(e1.X);
			fey = Math.Abs(e1.Y);
			fez = Math.Abs(e1.Z);
			if (!TestAxisX01(e1.Z, e1.Y, fez, fey, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisY02(e1.Z, e1.X, fez, fex, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisZ0(e1.Y, e1.X, fey, fex, halfDims, v0, v1, v2))
				return false;
	
			fex = Math.Abs(e2.X);
			fey = Math.Abs(e2.Y);
			fez = Math.Abs(e2.Z);
			if (!TestAxisX2(e2.Z, e2.Y, fez, fey, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisY1(e2.Z, e2.X, fez, fex, halfDims, v0, v1, v2))
				return false;
			if (!TestAxisZ12(e2.Y, e2.X, fey, fex, halfDims, v0, v1, v2))
				return false;

			float minimum = Math.Min(Math.Min(v0.X, v1.X), v2.X);
			float maximum = Math.Max(Math.Max(v0.X, v1.X), v2.X);
			if ((minimum > halfDims.X) || (maximum< -halfDims.X))
				return false;

			minimum = Math.Min(Math.Min(v0.Y, v1.Y), v2.Y);
			maximum = Math.Max(Math.Max(v0.Y, v1.Y), v2.Y);
			if ((minimum > halfDims.Y) || (maximum< -halfDims.Y))
				return false;

			minimum = Math.Min(Math.Min(v0.Z, v1.Z), v2.Z);
			maximum = Math.Max(Math.Max(v0.Z, v1.Z), v2.Z);
			if ((minimum > halfDims.Z) || (maximum< -halfDims.Z))
				return false;

			Vector3 normal = Vector3.Cross(e0, e1);
			//return (Math.Abs(Vector3.Dot(normal, v0)) <= Math.Abs(halfDims.X*normal.X) + Math.Abs(halfDims.Y*normal.Y) + Math.Abs(halfDims.Z*normal.Z));

			Vector3 minV = -halfDims;
			Vector3 maxV = halfDims;

			if (normal.X <= 0.0f)
				SwapX(ref minV, ref maxV);
			if (normal.Y <= 0.0f)
				SwapY(ref minV, ref maxV);
			if (normal.Z <= 0.0f)
				SwapZ(ref minV, ref maxV);

			minV -= v0;
			maxV -= v0;

			if (Vector3.Dot(normal, minV) > 0.0f) 
				return false;
			if (Vector3.Dot(normal, maxV) >= 0.0f) 
				return true;
	
			return false;
		}

		public static bool TestOrientedBoxTriangle(Vector3 center, in Matrix3 basis, Vector3 halfDims, Vector3 vertex0, Vector3 vertex1,
			Vector3 vertex2)
		{
			//Matrix3 basisTranspose = Matrix3.Transpose(basis);
			//return TestAxisAlignedBoxTriangle(Vector3.Zero, halfDims, (vertex0 - center)*basisTranspose, 
			//	(vertex1 - center)*basisTranspose, (vertex2 - center)*basisTranspose);
			return TestAxisAlignedBoxTriangle(Vector3.Zero, halfDims, basis*(vertex0 - center),
				basis*(vertex1 - center), basis*(vertex2 - center));
		}

		public static bool TestAxisAlignedRectangleCircle(Vector2 minimum, Vector2 maximum, Vector2 center, float radius)
		{
			float d = 0f;

			if (center.X < minimum.X)
			{
				float s = center.X - minimum.X;
				d += s*s;
			}
			else if (center.X > maximum.X)
			{
				float s = center.X - maximum.X;
				d += s*s;
			}

			if (center.Y < minimum.Y)
			{
				float s = center.Y - minimum.Y;
				d += s*s;
			}
			else if (center.Y > maximum.Y)
			{
				float s = center.Y - maximum.Y;
				d += s*s;
			}

			return (d <= radius*radius);
		}

		public static bool TestAxisAlignedBoxSphere(Vector3 minimum, Vector3 maximum, Vector3 center, float radius)
		{
			float d = 0f;

			if (center.X < minimum.X)
			{
				float s = center.X - minimum.X;
				d += s*s;
			}
			else if (center.X > maximum.X)
			{
				float s = center.X - maximum.X;
				d += s*s;
			}

			if (center.Y < minimum.Y)
			{
				float s = center.Y - minimum.Y;
				d += s*s;
			}
			else if (center.Y > maximum.Y)
			{
				float s = center.Y - maximum.Y;
				d += s*s;
			}

			if (center.Z < minimum.Z)
			{
				float s = center.Z - minimum.Z;
				d += s*s;
			}
			else if (center.Z > maximum.Z)
			{
				float s = center.Z - maximum.Z;
				d += s*s;
			}

			return (d <= radius*radius);
		}

		public static bool TestOrientedBoxAxisAlignedBox(Vector3 centerA, in Matrix3 basisA, Vector3 halfDimsA,
			Vector3 centerB, Vector3 halfDimsB)
		{
			return TestOrientedBoxOrientedBox(centerA, basisA, halfDimsA, centerB, Matrix3.Identity, halfDimsB);
		}

		public static bool TestOrientedBoxOrientedBox(Vector3 centerA, in Matrix3 basisA, Vector3 halfDimsA, Vector3 centerB,
			in Matrix3 basisB, Vector3 halfDimsB)
		{
			// https://www.geometrictools.com/

			const float cutoff = 1f - 1e-6f;
			bool existsParallelPair = false;
			Vector3 d = centerB - centerA;
			Vector3 c0 = Vector3.Zero;
			Vector3 c1 = Vector3.Zero;
			Vector3 c2 = Vector3.Zero;
			Vector3 absC0 = Vector3.Zero;
			Vector3 absC1 = Vector3.Zero;
			Vector3 absC2 = Vector3.Zero;
			Vector3 aD = Vector3.Zero;

			for (int i = 0; i < 3; i++)
			{
				c0[i] = Vector3.Dot(basisA[0], basisB[i]);
				absC0[i] = Math.Abs(c0[i]);
				if (absC0[i] > cutoff)
					existsParallelPair = true;
			}

			aD.X = Vector3.Dot(basisA[0], d);
			float r = Math.Abs(aD.x_);
			float r1 = Vector3.Dot(halfDimsB, absC0);
			float r01 = halfDimsA.x_ + r1;
			if (r > r01)
				return false;

			for (int i = 0; i < 3; i++)
			{
				c1[i] = Vector3.Dot(basisA[1], basisB[i]);
				absC1[i] = Math.Abs(c1[i]);
				if (absC1[i] > cutoff)
					existsParallelPair = true;
			}

			aD.Y = Vector3.Dot(basisA[1], d);
			r = Math.Abs(aD.y_);
			r1 = Vector3.Dot(halfDimsB, absC1);
			r01 = halfDimsA.y_ + r1;
			if (r > r01)
				return false;

			for (int i = 0; i < 3; i++)
			{
				c2[i] = Vector3.Dot(basisA[2], basisB[i]);
				absC2[i] = Math.Abs(c2[i]);
				if (absC2[i] > cutoff)
					existsParallelPair = true;
			}

			aD.Z = Vector3.Dot(basisA[2], d);
			r = Math.Abs(aD.z_);
			r1 = Vector3.Dot(halfDimsB, absC2);
			r01 = halfDimsA.z_ + r1;
			if (r > r01)
				return false;

			r = Math.Abs(Vector3.Dot(basisB[0], d));
			float r0 = halfDimsA.x_*absC0.x_ + halfDimsA.y_*absC1.x_ + halfDimsA.z_*absC2.x_;
			r01 = r0 + halfDimsB.x_;
			if (r > r01)
				return false;

			r = Math.Abs(Vector3.Dot(basisB[1], d));
			r0 = halfDimsA.x_*absC0.y_ + halfDimsA.y_*absC1.y_ + halfDimsA.z_*absC2.y_;
			r01 = r0 + halfDimsB.y_;
			if (r > r01)
				return false;

			r = Math.Abs(Vector3.Dot(basisB[2], d));
			r0 = halfDimsA.x_*absC0.z_ + halfDimsA.y_*absC1.z_ + halfDimsA.z_*absC2.z_;
			r01 = r0 + halfDimsB.z_;
			if (r > r01)
				return false;

			if (existsParallelPair)
				return true;

			r = Math.Abs(aD.z_*c1.x_ - aD.y_*c2.x_);
			r0 = halfDimsA.y_*absC2.x_ + halfDimsA.z_*absC1.x_;
			r1 = halfDimsB.y_*absC0.z_ + halfDimsB.z_*absC0.y_;
			r01 = r0 + r1;
			if (r > r01)
				return false;

			r = Math.Abs(aD.z_*c1.y_ - aD.y_*c2.y_);
			r0 = halfDimsA.y_*absC2.y_ + halfDimsA.z_*absC1.y_;
			r1 = halfDimsB.x_*absC0.z_ + halfDimsB.z_*absC0.x_;
			r01 = r0 + r1;
			if (r > r01)
				return false;

			r = Math.Abs(aD.z_*c1.z_ - aD.y_*c2.z_);
			r0 = halfDimsA.y_*absC2.z_ + halfDimsA.z_*absC1.z_;
			r1 = halfDimsB.x_*absC0.y_ + halfDimsB.y_*absC0.x_;
			r01 = r0 + r1;
			if (r > r01)
				return false;

			r = Math.Abs(aD.x_*c2.x_ - aD.z_*c0.x_);
			r0 = halfDimsA.x_*absC2.x_ + halfDimsA.z_*absC0.x_;
			r1 = halfDimsB.y_*absC1.z_ + halfDimsB.z_*absC1.y_;
			r01 = r0 + r1;
			if (r > r01)
				return false;

			r = Math.Abs(aD.x_*c2.y_ - aD.z_*c0.y_);
			r0 = halfDimsA.x_*absC2.y_ + halfDimsA.z_*absC0.y_;
			r1 = halfDimsB.x_*absC1.z_ + halfDimsB.z_*absC1.x_;
			r01 = r0 + r1;
			if (r > r01)
				return false;

			r = Math.Abs(aD.x_*c2.z_ - aD.z_*c0.z_);
			r0 = halfDimsA.x_*absC2.z_ + halfDimsA.z_*absC0.z_;
			r1 = halfDimsB.x_*absC1.y_ + halfDimsB.y_*absC1.x_;
			r01 = r0 + r1;
			if (r > r01)
				return false;

			r = Math.Abs(aD.y_*c0.x_ - aD.x_*c1.x_);
			r0 = halfDimsA.x_*absC1.x_ + halfDimsA.y_*absC0.x_;
			r1 = halfDimsB.y_*absC2.z_ + halfDimsB.z_*absC2.y_;
			r01 = r0 + r1;
			if (r > r01)
				return false;

			r = Math.Abs(aD.y_*c0.y_ - aD.x_*c1.y_);
			r0 = halfDimsA.x_*absC1.y_ + halfDimsA.y_*absC0.y_;
			r1 = halfDimsB.x_*absC2.z_ + halfDimsB.z_*absC2.x_;
			r01 = r0 + r1;
			if (r > r01)
				return false;

			r = Math.Abs(aD.y_*c0.z_ - aD.x_*c1.z_);
			r0 = halfDimsA.x_*absC1.z_ + halfDimsA.y_*absC0.z_;
			r1 = halfDimsB.x_*absC2.y_ + halfDimsB.y_*absC2.x_;
			r01 = r0 + r1;
			if (r > r01)
				return false;

			return true;
		}

		public static bool TestOrientedBoxSphere(Vector3 centerA, in Matrix3 basisA, Vector3 halfDimsA, Vector3 centerB, float radiusB)
		{
			//Matrix3 boxBasisT = Matrix3.Transpose(basisA);
			return TestAxisAlignedBoxSphere(-halfDimsA, halfDimsA,
				basisA*(centerB - centerA)/*(centerB - centerA)*boxBasisT*/, radiusB);
		}

		public static bool TestEllipsoidPlane(Vector3 center, in Matrix3 inverseMatrix, Vector3 normal, float constant)
		{
			return (Distances.GetPointPlaneSquared(center, normal, constant) <= Math.Abs(Vector3.Dot(normal, normal*inverseMatrix)));
		}

		public static bool TestConeSphere(Vector3 vertex, Vector3 axis, float height, float baseRadius, Vector3 center, float radius)
		{
			// http://www.geometrictools.com/

			float slantHeight = MathF.Sqrt(baseRadius*baseRadius + height*height);
			float sinAngle = baseRadius/slantHeight;
			float cosAngle = height/slantHeight;
			float invSin = slantHeight/baseRadius; // 1f/sinAngle;
			float cosSqr = cosAngle*cosAngle;
			Vector3 cmV = center - vertex;
			Vector3 d = cmV + (radius*invSin)*axis;
			float dSqrLen = d.LengthSquared;
			float e = Vector3.Dot(d, axis);

			if ((e > 0f) && (e*e >= dSqrLen*cosSqr))
			{
				float sinSqr = sinAngle*sinAngle;
				dSqrLen = cmV.LengthSquared;
				e = -Vector3.Dot(cmV, axis);
				if ((e > 0f) && (e*e >= dSqrLen*sinSqr) && (dSqrLen > radius*radius))
					return false;
				if (/*(height < Single.MaxValue) &&*/ !((Vector3.Dot(axis, center) - Vector3.Dot(axis, vertex + height*axis)) <= radius))
					return false;

				return true;
			}

			return false;
		}

		private static void SwapX(ref Vector3 a, ref Vector3 b)
		{
			float temp = a.X;
			a.X = b.X;
			b.X = temp;
		}

		private static void SwapY(ref Vector3 a, ref Vector3 b)
		{
			float temp = a.Y;
			a.Y = b.Y;
			b.Y = temp;
		}

		private static void SwapZ(ref Vector3 a, ref Vector3 b)
		{
			float temp = a.Z;
			a.Z = b.Z;
			b.Z = temp;
		}

		private static bool TestAxisX01(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)			   
		{
			float p0 = a*v0.Y - b*v0.Z;
			float p2 = a*v2.Y - b*v2.Z;

			float minimum, maximum;
			if (p0 < p2)
			{
				minimum = p0; 
				maximum = p2;
			} 
			else 
			{
				minimum = p2; 
				maximum = p0;
			}

			float rad = fa*halfDims.Y + fb*halfDims.Z;   
			return !((minimum > rad) || (maximum< -rad));
		}

		private static bool TestAxisX2(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			float p0 = a*v0.Y - b*v0.Z;
			float p1 = a*v1.Y - b*v1.Z;

			float minimum, maximum;
			if (p0 < p1)
			{
				minimum = p0;
				maximum = p1;
			}
			else
			{
				minimum = p1;
				maximum = p0;
			}

			float rad = fa*halfDims.Y + fb*halfDims.Z;
			return !((minimum > rad) || (maximum < -rad));
		}

		private static bool TestAxisY02(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			float p0 = -a*v0.X + b*v0.Z;
			float p2 = -a*v2.X + b*v2.Z;

			float minimum, maximum;
			if (p0 < p2)
			{
				minimum = p0;
				maximum = p2;
			}
			else
			{
				minimum = p2;
				maximum = p0;
			}

			float rad = fa*halfDims.X + fb*halfDims.Z;
			return !((minimum > rad) || (maximum < -rad));
		}

		private static bool TestAxisY1(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			float p0 = -a*v0.X + b*v0.Z;
			float p1 = -a*v1.X + b*v1.Z;

			float minimum, maximum;
			if (p0 < p1)
			{
				minimum = p0;
				maximum = p1;
			}
			else
			{
				minimum = p1;
				maximum = p0;
			}

			float rad = fa*halfDims.X + fb*halfDims.Z;
			return !((minimum > rad) || (maximum < -rad));
		}

		private static bool TestAxisZ12(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			float p1 = a*v1.X - b*v1.Y;
			float p2 = a*v2.X - b*v2.Y;

			float minimum, maximum;
			if (p2 < p1)
			{
				minimum = p2;
				maximum = p1;
			}
			else
			{
				minimum = p1;
				maximum = p2;
			}

			float rad = fa*halfDims.X + fb*halfDims.Y;
			return !((minimum > rad) || (maximum < -rad));
		}

		private static bool TestAxisZ0(float a, float b, float fa, float fb, Vector3 halfDims, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			float p0 = a*v0.X - b*v0.Y;
			float p1 = a*v1.X - b*v1.Y;

			float minimum, maximum;
			if (p0 < p1)
			{
				minimum = p0;
				maximum = p1;
			}
			else
			{
				minimum = p1;
				maximum = p0;
			}

			float rad = fa*halfDims.X + fb*halfDims.Y;
			return !((minimum > rad) || (maximum < -rad));
		}
	}
}
