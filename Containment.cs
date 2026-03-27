/*
 *  Name: Containment
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	internal static class Containment
	{
		public static bool TestCylinderPoint(Vector3 center, Vector3 axis, float height, float radius, Vector3 point)
		{
			Vector3 pd = point - (center - (height*0.5f)*axis);
			float d = Vector3.Dot(pd, height*axis);
			float lengthSq = height*height;
			if ((d < 0f) || (d > lengthSq))
				return false;

			return ((Vector3.Dot(pd, pd) - d*d/lengthSq) <= radius*radius);
		}

		public static bool TestConePoint(Vector3 vertex, Vector3 axis, float height, float radius, Vector3 point)
		{
			Vector3 diff = point - vertex;
			float d = Vector3.Dot(diff, axis);
			if ((d < 0f) || (d > height))
				return false;

			float r = d*radius/height;
			return (Vector3.DistanceSquared(diff, d*axis) <= r*r);
		}
	}
}
