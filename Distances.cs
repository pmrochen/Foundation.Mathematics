/*
 *  Name: Distances
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	internal static class Distances
	{
		public static float GetPointHalfSpace(Vector3 point, Vector3 normal, float constant)
		{
			return Math.Max((Vector3.Dot(normal, point) + constant)/normal.Magnitude, 0f);
		}

		public static float GetPointNormalizedHalfSpace(Vector3 point, Vector3 normal, float constant)
		{
			return Math.Max(Vector3.Dot(normal, point) + constant, 0f);
		}

		public static float GetPointPlane(Vector3 point, Vector3 normal, float constant)
		{
			return Math.Abs((Vector3.Dot(normal, point) + constant)/normal.Magnitude);
		}

		public static float GetPointNormalizedPlane(Vector3 point, Vector3 normal, float constant)
		{
			return Math.Abs(Vector3.Dot(normal, point) + constant);
		}

		public static float GetPointPlaneSigned(Vector3 point, Vector3 normal, float constant)
		{
			return (Vector3.Dot(normal, point) + constant)/normal.Magnitude;
		}

		public static float GetPointNormalizedPlaneSigned(Vector3 point, Vector3 normal, float constant)
		{
			return Vector3.Dot(normal, point) + constant;
		}

		public static float GetPointPlaneSquared(Vector3 point, Vector3 normal, float constant)
		{
			float d = (Vector3.Dot(normal, point) + constant)/normal.Magnitude;
			return d*d;
		}

		public static float GetPointNormalizedPlaneSquared(Vector3 point, Vector3 normal, float constant)
		{
			float d = Vector3.Dot(normal, point) + constant;
			return d*d;
		}

		public static float GetPointTriangle(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			Vector3 closestPoint;
			return MathF.Sqrt(GetPointTriangleSquared(point, v0, v1, v2, out closestPoint));
		}

		public static float GetPointTriangleSquared(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			Vector3 closestPoint;
			return GetPointTriangleSquared(point, v0, v1, v2, out closestPoint);
		}

		public static float GetPointTriangleSquared(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2, out Vector3 closestPoint)
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
					// Minimum at interior point
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

			// Account for numerical round-off error
			if (sqrDistance < 0.0f)
				sqrDistance = 0.0f;

			closestPoint = v0 + s*edge0 + t*edge1;
			//triangleBary[1] = s;
			//triangleBary[2] = t;
			//triangleBary[0] = 1.0f - s - t;
    
			return sqrDistance;
		}
	}
}
