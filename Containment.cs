/*
 *  Name: Containment
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	internal static class Containment
	{
		public static bool TestConePoint(Vector3 vertex, Vector3 axis, float height, float baseRadius, Vector3 point)
		{
			// http://www.geometrictools.com/

			Vector3 diff = point - vertex;
			float coneDist = Vector3.Dot(diff, axis);
			if ((coneDist < 0f) || (coneDist > height))
				return false;

			float coneRadius = coneDist*baseRadius/height;
			float orthDistSquared = (diff - coneDist*axis).LengthSquared;
			return (orthDistSquared <= coneRadius*coneRadius);
		}
	}
}
