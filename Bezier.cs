/*
 *  Name: Bezier
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	internal static class Bezier
	{
		public static Vector4 GetBasis(float t) 
		{
			float t2 = t*t;
			float omt = 1f - t;
			float omt2 = omt*omt;
			return new Vector4(omt*omt2, 3f*t*omt2, 3f*t2*omt, t*t2);
		}

		public static Vector4 GetDerivativeBasis(float t)
		{
			float t2 = t*t;
			float omt = 1f - t;
			float omt2 = omt*omt;
			return new Vector4((-3f)*omt2, (-6f)*t*omt + 3f*omt2, (-3f)*t2 + 6f*t*omt, 3f*t2);
		}
	}
}
