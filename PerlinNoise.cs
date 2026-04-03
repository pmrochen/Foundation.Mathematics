/*
 *  Name: PerlinNoise
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	public static class PerlinNoise
	{
		public static float Get(float x)
		{
			float fx = MathF.Floor(x);
			int ix = (int)fx & 255;
			x -= fx;
			float u = Fade(x);

			int a = perm_[ix];
			int aa = perm_[a];
			int b = perm_[ix + 1];
			int ba = perm_[b];

			return Functions.Lerp(Grad(perm_[aa], x, 0f, 0f), Grad(perm_[ba], x - 1f, 0f, 0f), u);
		}

		public static double Get(double x)
		{
			double fx = Math.Floor(x);
			int ix = (int)fx & 255;
			x -= fx;
			double u = Fade(x);

			int a = perm_[ix];
			int aa = perm_[a];
			int b = perm_[ix + 1];
			int ba = perm_[b];

			return Functions.Lerp(Grad(perm_[aa], x, 0.0, 0.0), Grad(perm_[ba], x - 1.0, 0.0, 0.0), u);
		}

		public static float Get(float x, float y)
		{
			float fx = MathF.Floor(x);
			float fy = MathF.Floor(y);
			int ix = (int)fx & 255;
			int iy = (int)fy & 255;
			x -= fx;
			y -= fy;
			float u = Fade(x);
			float v = Fade(y);

			int a = perm_[ix] + iy;
			int aa = perm_[a];
			int ab = perm_[a + 1];
			int b = perm_[ix + 1] + iy;
			int ba = perm_[b];
			int bb = perm_[b + 1];

			return Functions.Lerp(Functions.Lerp(Grad(perm_[aa], x, y, 0f), Grad(perm_[ba], x - 1f, y, 0f), u),
						Functions.Lerp(Grad(perm_[ab], x, y - 1f, 0f), Grad(perm_[bb], x - 1f, y - 1f, 0f), u), v);
		}

		public static double Get(double x, double y)
		{
			double fx = Math.Floor(x);
			double fy = Math.Floor(y);
			int ix = (int)fx & 255;
			int iy = (int)fy & 255;
			x -= fx;
			y -= fy;
			double u = Fade(x);
			double v = Fade(y);

			int a = perm_[ix] + iy;
			int aa = perm_[a];
			int ab = perm_[a + 1];
			int b = perm_[ix + 1] + iy;
			int ba = perm_[b];
			int bb = perm_[b + 1];

			return Functions.Lerp(Functions.Lerp(Grad(perm_[aa], x, y, 0.0), Grad(perm_[ba], x - 1.0, y, 0.0), u),
						Functions.Lerp(Grad(perm_[ab], x, y - 1.0, 0.0), Grad(perm_[bb], x - 1.0, y - 1.0, 0.0), u), v);
		}

		public static float Get(float x, float y, float z)
		{
			float fx = MathF.Floor(x);
			float fy = MathF.Floor(y);
			float fz = MathF.Floor(z);
			int ix = (int)fx & 255;
			int iy = (int)fy & 255;
			int iz = (int)fz & 255;
			x -= fx;
			y -= fy;
			z -= fz;
			float u = Fade(x);
			float v = Fade(y);
			float w = Fade(z);

			int a = perm_[ix] + iy;
			int aa = perm_[a] + iz;
			int ab = perm_[a + 1] + iz;
			int b = perm_[ix + 1] + iy;
			int ba = perm_[b] + iz;
			int bb = perm_[b + 1] + iz;

			return Functions.Lerp(Functions.Lerp(Functions.Lerp(Grad(perm_[aa], x, y, z), Grad(perm_[ba], x - 1f, y, z), u),
							 Functions.Lerp(Grad(perm_[ab], x, y - 1f, z), Grad(perm_[bb], x - 1f, y - 1f, z), u), v),
						Functions.Lerp(Functions.Lerp(Grad(perm_[aa + 1], x, y, z - 1f), Grad(perm_[ba + 1], x - 1f, y, z - 1f), u),
							 Functions.Lerp(Grad(perm_[ab + 1], x, y - 1f, z - 1f), Grad(perm_[bb + 1], x - 1f, y - 1f, z - 1f), u), v), w);
		}

		public static double Get(double x, double y, double z)
		{
			double fx = Math.Floor(x);
			double fy = Math.Floor(y);
			double fz = Math.Floor(z);
			int ix = (int)fx & 255;
			int iy = (int)fy & 255;
			int iz = (int)fz & 255;
			x -= fx;
			y -= fy;
			z -= fz;
			double u = Fade(x);
			double v = Fade(y);
			double w = Fade(z);

			int a = perm_[ix] + iy;
			int aa = perm_[a] + iz;
			int ab = perm_[a + 1] + iz;
			int b = perm_[ix + 1] + iy;
			int ba = perm_[b] + iz;
			int bb = perm_[b + 1] + iz;

			return Functions.Lerp(Functions.Lerp(Functions.Lerp(Grad(perm_[aa], x, y, z), Grad(perm_[ba], x - 1.0, y, z), u),
							 Functions.Lerp(Grad(perm_[ab], x, y - 1.0, z), Grad(perm_[bb], x - 1.0, y - 1.0, z), u), v),
						Functions.Lerp(Functions.Lerp(Grad(perm_[aa + 1], x, y, z - 1.0), Grad(perm_[ba + 1], x - 1.0, y, z - 1.0), u),
							 Functions.Lerp(Grad(perm_[ab + 1], x, y - 1.0, z - 1.0), Grad(perm_[bb + 1], x - 1.0, y - 1.0, z - 1.0), u), v), w);
		}

		private static float Fade(float t) 
		{ 
			return t*t*t*(t*(t*6f - 15f) + 10f); 
		}

		private static double Fade(double t) 
		{ 
			return t*t*t*(t*(t*6.0 - 15.0) + 10.0); 
		}

		private static float Grad(int hash, float x, float y, float z) 
		{
			int h = hash & 15;
			float u = (h < 8) ? x : y;
			float v = (h < 4) ? y : (h == 12) || (h == 14) ? x : z;
			return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
		}

		private static double Grad(int hash, double x, double y, double z)
		{
			int h = hash & 15;
			double u = (h < 8) ? x : y;
			double v = (h < 4) ? y : (h == 12) || (h == 14) ? x : z;
			return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
		}

		private static readonly int[] perm_ =
		{
			151,160,137,91,90,15,
			131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
			190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
			88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
			77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
			102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
			135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
			5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
			223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
			129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
			251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
			49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
			138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
			151,160,137,91,90,15,
			131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
			190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
			88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
			77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
			102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
			135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
			5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
			223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
			129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
			251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
			49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
			138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
		};
	}
}
