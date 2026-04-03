/*
 *  Name: Romberg
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	public delegate float SingleFunctionDelegate(float x);
	public delegate double DoubleFunctionDelegate(double x);

	public static class Romberg
	{
		public static float Estimate(float a, float b, SingleFunctionDelegate f, int order)
		{
			float h = b - a;
			float[,] rom = new float[2, order];
			rom[0, 0] = 0.5f*h*(f(a) + f(b));
			for (int i0 = 2, ip0 = 1; i0 <= order; i0++, ip0 *= 2, h *= 0.5f)
			{
				float sum = 0f;
				for (int i1 = 1; i1 <= ip0; i1++)
					sum += f(a + h*(i1 - 0.5f));

				rom[1, 0] = 0.5f*(rom[0, 0] + h*sum);
				for (int i2 = 1, ip2 = 4; i2 < i0; i2++, ip2 *= 4)
					rom[1, i2] = (ip2*rom[1, i2 - 1] - rom[0, i2 - 1])/(ip2 - 1);

				for (int i1 = 0; i1 < i0; i1++)
					rom[0, i1] = rom[1, i1];
			}

			return rom[0, order - 1];
		}

		public static double Estimate(double a, double b, DoubleFunctionDelegate f, int order)
		{
			double h = b - a;
			double[,] rom = new double[2, order];
			rom[0, 0] = 0.5*h*(f(a) + f(b));
			for (int i0 = 2, ip0 = 1; i0 <= order; i0++, ip0 *= 2, h *= 0.5)
			{
				double sum = 0.0;
				for (int i1 = 1; i1 <= ip0; i1++)
					sum += f(a + h*(i1 - 0.5));

				rom[1, 0] = 0.5*(rom[0, 0] + h*sum);
				for (int i2 = 1, ip2 = 4; i2 < i0; i2++, ip2 *= 4)
					rom[1, i2] = (ip2*rom[1, i2 - 1] - rom[0, i2 - 1]) / (ip2 - 1);

				for (int i1 = 0; i1 < i0; i1++)
					rom[0, i1] = rom[1, i1];
			}

			return rom[0, order - 1];
		}
	}
}
