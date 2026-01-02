/*
 *  Name: EulerOrder
 *  Author: Pawel Mrochen
 */

namespace Foundation.Mathematics
{
	public enum EulerOrder
	{
		Unspecified = 0,
		XYZ = ((((((0 << 1) | 0) << 1) | 0) << 1) | 1),
		XZY = ((((((0 << 1) | 1) << 1) | 0) << 1) | 1),
		YZX = ((((((1 << 1) | 0) << 1) | 0) << 1) | 1),
		YXZ = ((((((1 << 1) | 1) << 1) | 0) << 1) | 1),
		ZXY = ((((((2 << 1) | 0) << 1) | 0) << 1) | 1),	// YawPitchRoll
		ZYX = ((((((2 << 1) | 1) << 1) | 0) << 1) | 1)
	}
}
