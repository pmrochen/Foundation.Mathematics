/*
 *  Name: Vector3Extensions
 *  Author: Pawel Mrochen
 */

using System.Runtime.CompilerServices;

namespace Foundation.Mathematics
{
	internal static class Vector3Extensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static System.Numerics.Vector3 ZYX(this System.Numerics.Vector3 v)
		{
			return new System.Numerics.Vector3(v.Z, v.Y, v.X);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static System.Numerics.Vector3 YZX(this System.Numerics.Vector3 v)
		{
			return new System.Numerics.Vector3(v.Y, v.Z, v.X);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static System.Numerics.Vector3 ZXY(this System.Numerics.Vector3 v)
		{
			return new System.Numerics.Vector3(v.Z, v.X, v.Y);
		}
	}
}
