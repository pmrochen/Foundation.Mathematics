/*
 *  Name: IRandomNumberGenerator
 *  Author: Pawel Mrochen
 */

using System;

namespace Foundation.Mathematics
{
	public interface IRandomNumberGenerator<T>
	{
		public T MaxValue { get; }
		public T GetNext();
	}
}
