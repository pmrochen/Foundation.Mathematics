/*
 *  Name: Axes
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;

namespace Foundation.Mathematics
{
	[Flags]
	public enum Axes // 1 << Axis
	{
		None = 0,
		X = 1,
		Y = 2,
		[Browsable(false)] XY = 3,
		Z = 4,
		[Browsable(false)] XZ = 5,
		[Browsable(false)] YZ = 6,
		[Browsable(false)] XYZ = 7,
		W = 8,
		[Browsable(false)] XW = 9,
		[Browsable(false)] YW = 10,
		[Browsable(false)] XYW = 11,
		[Browsable(false)] ZW = 12,
		[Browsable(false)] XZW = 13,
		[Browsable(false)] YZW = 14,
		[Browsable(false)] XYZW = 15
	}
}
