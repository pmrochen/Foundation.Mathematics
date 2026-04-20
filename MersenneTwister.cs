/*
 *  Name: Mersenne Twister
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	/// <summary>
	/// Mersenne Twister random number generator.
	/// </summary>
	public sealed class MersenneTwister : IRandomNumberGenerator<uint>
	{
		public MersenneTwister()
		{
			Init(new int[4] { 0x123, 0x234, 0x345, 0x456 });
		}

		public MersenneTwister(int seed)
		{
			Init(seed);
		}

		public MersenneTwister(int[] key)
		{
			if (key != null)
				Init(key);
		}

		uint IRandomNumberGenerator<uint>.MaxValue => ~0U;

		uint IRandomNumberGenerator<uint>.GetNext()
		{
			return GetUInt32();
		}

		public int GetInt31()
		{
			return (int)(GetUInt32() >> 1);
		}

		public int GetInt32()
		{
			return (int)GetUInt32();
		}

		public uint GetUInt32()
		{
			uint y = 0;

			if (mti_ >= n_)
			{
				if (mti_ == n_ + 1)
					Init(5489);

				int kk = 0;

				for (; kk < n_ - m_; kk++)
				{
					y = (mt_[kk] & upperMask_) | (mt_[kk + 1] & loweMask_);
					mt_[kk] = mt_[kk + m_] ^ (y >> 1) ^ magm01_[y & 0x1u];
				}

				for (; kk < n_ - 1; kk++)
				{
					y = (mt_[kk] & upperMask_) | (mt_[kk + 1] & loweMask_);
					mt_[kk] = mt_[kk + (m_ - n_)] ^ (y >> 1) ^ magm01_[y & 0x1u];
				}

				y = (mt_[n_ - 1] & upperMask_) | (mt_[0] & loweMask_);
				mt_[n_ - 1] = mt_[m_ - 1] ^ (y >> 1) ^ magm01_[y & 0x1u];

				mti_ = 0;
			}

			y = mt_[mti_++];
			y ^= (y >> 11);
			y ^= (y << 7) & 0x9d2c5680u;
			y ^= (y << 15) & 0xefc60000u;
			y ^= (y >> 18);
			return y;
		}

		public float GetSingle01()
		{
			return (float)(GetUInt32()*(1.0/4294967295.0));
		}

		public float GetSingle01RightOpen()
		{
			return (float)(GetUInt32()*(1.0/4294967296.0));
		}

		public float GetSingle01Open()
		{
			return (float)((GetUInt32() + 0.5)*(1.0/4294967296.0));
		}

		public double GetDouble01()
		{
			return GetUInt32()*(1.0/4294967295.0);
		}

		public double GetDouble01RightOpen()
		{
			return GetUInt32()*(1.0/4294967296.0);
		}

		public double GetDouble01Open()
		{
			return (GetUInt32() + 0.5)*(1.0/4294967296.0);
		}

		public double GetDouble01RightOpen53Bits()
		{
			uint a = GetUInt32() >> 5;
			uint b = GetUInt32() >> 6;
			return (a*67108864.0 + b)*(1.0/9007199254740992.0);
		}

		private void Init(int seed)
		{
			mt_[0] = (uint)seed;
			mti_ = n_;

			for (int mti = 1; mti < n_; mti++)
			{
				mt_[mti] = (1812433253u*(mt_[mti - 1] ^ (mt_[mti - 1] >> 30)) + (uint)mti);
			}
		}

		private void Init(int[] key)
		{
			Init(19650218);

			int i = 1;
			int j = 0;

			for (int k = (n_ > key.Length) ? n_ : key.Length; k > 0; k--)
			{
				mt_[i] = (mt_[i] ^ ((mt_[i - 1] ^ (mt_[i - 1] >> 30))*1664525u)) + (uint)key[j] + (uint)j;
				i++;
				j++;
				
				if (i >= n_)
				{
					mt_[0] = mt_[n_ - 1];
					i = 1;
				}
				
				if (j >= key.Length)
					j = 0;
			}

			for (int k = n_ - 1; k > 0; k--)
			{
				mt_[i] = (mt_[i] ^ ((mt_[i - 1] ^ (mt_[i - 1] >> 30))*1566083941u)) - (uint)i;
				i++;
				
				if (i >= n_)
				{
					mt_[0] = mt_[n_ - 1];
					i = 1;
				}
			}

			mt_[0] = 0x80000000u;
		}

		private const int n_ = 624;
		private const int m_ = 397;
		private const uint matrixA_ = 0x9908b0dfu;
		private const uint upperMask_ = 0x80000000u;
		private const uint loweMask_ = 0x7fffffffu;

		private static readonly uint[] magm01_ = new uint[2] { 0x0u, matrixA_ };

		private uint[] mt_ = new uint[624];
		private int mti_ = n_ + 1;
	}
}
