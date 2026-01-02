/*
 *  Name: Rational
 *  Source: 
 */

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Globalization;

namespace Foundation.Mathematics
{
	/// <summary>
	/// Represents a rational number.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(RationalConverter))]
	public readonly struct Rational : IConvertible, IComparable, IComparable<Rational>, IEquatable<Rational>, IFormattable, ISerializable
	{
		public static readonly Rational Zero = new Rational(0, 1);
		public static readonly Rational One = new Rational(1, 1);

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="numerator">The numerator of the rational number.</param>
		/// <param name="denominator">The denominator of the rational number.</param>
		public Rational(int numerator, int denominator)
		{
			numerator_ = numerator;
			denominator_ = denominator;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="numerator">The numerator of the rational number.</param>
		/// <param name="denominator">The denominator of the rational number.</param>
		/// <param name="reduce">Determines if should reduce by greatest common divisor.</param>
		public Rational(int numerator, int denominator, bool reduce)
		{
			numerator_ = numerator;
			denominator_ = denominator;

			if (reduce)
				Reduce(ref numerator_, ref denominator_);
		}

		private Rational(SerializationInfo info, StreamingContext context)
		{
			numerator_ = info.GetInt32("Numerator");
			denominator_ = info.GetInt32("Denominator");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Numerator", numerator_);
			info.AddValue("Denominator", denominator_);
		}

		public override int GetHashCode()
		{
			int hash = 0x1fb8d67d;
			hash = (-1521134295*hash) + numerator_.GetHashCode();
			return (-1521134295*hash) + denominator_.GetHashCode();
		}

		public override bool Equals(object other)
		{
			return (CompareTo(other) == 0);
		}

		public bool Equals(Rational other)
		{
			return (CompareTo(other) == 0);
		}

		public override string ToString()
		{
			return String.Concat(numerator_.ToString(), delimiter_, denominator_.ToString());
		}

		public string ToString(string format)
		{
			return String.Concat(numerator_.ToString(format), delimiter_, denominator_.ToString(format));
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return String.Concat(numerator_.ToString(format, provider), delimiter_, denominator_.ToString(format, provider));
		}

		public float ToSingle()
		{
			return (float)numerator_/(float)denominator_;
		}

		public double ToDouble()
		{
			return (double)numerator_/(double)denominator_;
		}

		public decimal ToDecimal()
		{
			decimal denominator = (decimal)denominator_;
			if (denominator == Decimal.Zero)
				return Decimal.MaxValue;
			return (decimal)numerator_/denominator;
		}

		/// <summary>
		/// Converts the string representation of a number to its <see cref="Rational"/> equivalent.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static Rational Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			if (str.Length == 0)
				throw new FormatException();

			string[] parts = str.Split(Rational.delimiters_, 2, StringSplitOptions.RemoveEmptyEntries);
			int numerator = Int32.Parse(parts[0]);
			int denominator;
			if (parts.Length > 1)
				denominator = Int32.Parse(parts[1]);
			else
				denominator = 1;

			return new Rational(numerator, denominator);
		}

		/// <summary>
		/// Converts the string representation of a number to its <see cref="Rational"/> equivalent.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="provider"></param>
		/// <returns></returns>
		public static Rational Parse(string str, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			if (str.Length == 0)
				throw new FormatException();

			string[] parts = str.Split(Rational.delimiters_, 2, StringSplitOptions.RemoveEmptyEntries);
			int numerator = Int32.Parse(parts[0], provider);
			int denominator;
			if (parts.Length > 1)
				denominator = Int32.Parse(parts[1], provider);
			else
				denominator = 1;

			return new Rational(numerator, denominator);
		}

		/// <summary>
		/// Converts the string representation of a number to its <see cref="Rational"/> equivalent.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="style"></param>
		/// <returns></returns>
		public static Rational Parse(string str, NumberStyles style)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			if (str.Length == 0)
				throw new FormatException();

			string[] parts = str.Split(Rational.delimiters_, 2, StringSplitOptions.RemoveEmptyEntries);
			int numerator = Int32.Parse(parts[0], style);
			int denominator;
			if (parts.Length > 1)
				denominator = Int32.Parse(parts[1], style);
			else
				denominator = 1;

			return new Rational(numerator, denominator);
		}

		/// <summary>
		/// Converts the string representation of a number to its <see cref="Rational"/> equivalent.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="style"></param>
		/// <param name="provider"></param>
		/// <returns></returns>
		public static Rational Parse(string str, NumberStyles style, IFormatProvider provider)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			if (str.Length == 0)
				throw new FormatException();

			string[] parts = str.Split(Rational.delimiters_, 2, StringSplitOptions.RemoveEmptyEntries);
			int numerator = Int32.Parse(parts[0], style, provider);
			int denominator;
			if (parts.Length > 1)
				denominator = Int32.Parse(parts[1], style, provider);
			else
				denominator = 1;

			return new Rational(numerator, denominator);
		}

		/// <summary>
		/// Converts the string representation of a number to its <see cref="Rational"/> equivalent.
		/// A return value indicates whether the conversion succeeded or failed.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="rational"></param>
		/// <returns></returns>
		public static bool TryParse(string str, out Rational rational)
		{
			if (String.IsNullOrEmpty(str))
			{
				rational = new Rational();
				return false;
			}
			
			int numerator, denominator;
			string[] parts = str.Split(Rational.delimiters_, 2, StringSplitOptions.RemoveEmptyEntries);
			if (!Int32.TryParse(parts[0], out numerator))
			{
				rational = new Rational();
				return false;
			}

			if (parts.Length > 1)
			{
				if (!Int32.TryParse(parts[1], out denominator))
				{
					rational = new Rational();
					return false;
				}
			}
			else
			{
				denominator = 1;
			}

			rational = new Rational(numerator, denominator);
			return (parts.Length == 2);
		}

		/// <summary>
		/// Converts the string representation of a number to its <see cref="Rational"/> equivalent.
		/// A return value indicates whether the conversion succeeded or failed.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="style"></param>
		/// <param name="provider"></param>
		/// <param name="rational"></param>
		/// <returns></returns>
		public static bool TryParse(string str, NumberStyles style, IFormatProvider provider, out Rational rational)
		{
			if (String.IsNullOrEmpty(str))
			{
				rational = new Rational();
				return false;
			}

			int numerator, denominator;
			string[] parts = str.Split(Rational.delimiters_, 2, StringSplitOptions.RemoveEmptyEntries);
			if (!Int32.TryParse(parts[0], style, provider, out numerator))
			{
				rational = new Rational();
				return false;
			}

			if (parts.Length > 1)
			{
				if (!Int32.TryParse(parts[1], style, provider, out denominator))
				{
					rational = new Rational();
					return false;
				}
			}
			else
			{
				denominator = 1;
			}

			rational = new Rational(numerator, denominator);
			return (parts.Length == 2);
		}

		public static implicit operator Rational(int value)
		{
			return new Rational(value, 1);
		}

		public static explicit operator Rational(float value)
		{
			return Approximate(value);
		}

		public static explicit operator Rational(double value)
		{
			return Approximate(value);
		}

		public static explicit operator Rational(decimal value)
		{
			return Approximate(value);
		}

		public static explicit operator float(Rational value)
		{
			return (float)value.numerator_/(float)value.denominator_;
		}

		public static explicit operator double(Rational value)
		{
			return (double)value.numerator_/(double)value.denominator_;
		}

		public static explicit operator decimal(Rational value)
		{
			decimal denominator = (decimal)value.denominator_;
			if (denominator == Decimal.Zero)
				return Decimal.MaxValue;
			return (decimal)value.numerator_/denominator;
		}

		public static Rational operator +(Rational r)
		{
			return r;
		}

		public static Rational operator -(Rational r)
		{
			int numerator = (int)-Convert.ToDecimal(r.numerator_);
			return new Rational(numerator, r.denominator_);
		}

		public static Rational operator +(Rational r1, Rational r2)
		{
			decimal n1 = Convert.ToDecimal(r1.numerator_);
			decimal d1 = Convert.ToDecimal(r1.denominator_);
			decimal n2 = Convert.ToDecimal(r2.numerator_);
			decimal d2 = Convert.ToDecimal(r2.denominator_);

			decimal denominator = LowestCommonDenominator(d1, d2);
			if (denominator > d1)
				n1 *= (denominator/d1);
			if (denominator > d2)
				n2 *= (denominator/d2);

			decimal numerator = n1 + n2;
			return new Rational((int)numerator, (int)denominator);
		}

		public static Rational operator -(Rational r1, Rational r2)
		{
			return r1 + (-r2);
		}

		public static Rational operator *(Rational r1, Rational r2)
		{
			decimal numerator = Convert.ToDecimal(r1.numerator_)*Convert.ToDecimal(r2.numerator_);
			decimal denominator = Convert.ToDecimal(r1.denominator_)*Convert.ToDecimal(r2.denominator_);
			return new Rational((int)numerator, (int)denominator);
		}

		public static Rational operator /(Rational r1, Rational r2)
		{
			return r1*new Rational(r2.denominator_, r2.numerator_);
		}

		public static bool operator <(Rational r1, Rational r2)
		{
			return (r1.CompareTo(r2) < 0);
		}

		public static bool operator <=(Rational r1, Rational r2)
		{
			return (r1.CompareTo(r2) <= 0);
		}

		public static bool operator >(Rational r1, Rational r2)
		{
			return (r1.CompareTo(r2) > 0);
		}

		public static bool operator >=(Rational r1, Rational r2)
		{
			return (r1.CompareTo(r2) >= 0);
		}

		public static bool operator ==(Rational r1, Rational r2)
		{
			return (r1.CompareTo(r2) == 0);
		}

		public static bool operator !=(Rational r1, Rational r2)
		{
			return (r1.CompareTo(r2) != 0);
		}

		/// <summary>
		/// Gets the numerator of the rational number.
		/// </summary>
		public readonly int Numerator => numerator_;

		/// <summary>
		/// Gets the denominator of the rational number.
		/// </summary>
		public readonly int Denominator => denominator_;

		/// <summary>
		/// Approximate the floating point value accurate to a precision of 0.000001.
		/// </summary>
		/// <param name="value">Floating point value to approximate.</param>
		/// <returns>An approximation of the value as a rational number.</returns>
		public static Rational Approximate(float value)
		{
			return Approximate((decimal)value);
		}

		/// <summary>
		/// Approximate the floating point value accurate to a precision of 0.000001.
		/// </summary>
		/// <param name="value">Floating point value to approximate.</param>
		/// <returns>An approximation of the value as a rational number.</returns>
		public static Rational Approximate(double value)
		{
			return Approximate((decimal)value);
		}

		/// <summary>
		/// Approximate the decimal value accurate to a precision of 0.000001.
		/// </summary>
		/// <param name="value">Decimal value to approximate.</param>
		/// <returns>An approximation of the value as a rational number.</returns>
		public static Rational Approximate(decimal value)
		{
			return Approximate(value, 0.000001m);
		}

		/// <summary>
		/// Approximate the floating point value accurate to a certain precision.
		/// </summary>
		/// <param name="value">Floating point value to approximate.</param>
		/// <param name="epsilon">Maximum precision to converge.</param>
		/// <returns>An approximation of the value as a rational number.</returns>
		public static Rational Approximate(float value, float epsilon)
		{
			return Approximate((decimal)value, (decimal)epsilon);
		}

		/// <summary>
		/// Approximate the floating point value accurate to a certain precision.
		/// </summary>
		/// <param name="value">Floating point value to approximate.</param>
		/// <param name="epsilon">Maximum precision to converge.</param>
		/// <returns>An approximation of the value as a rational number.</returns>
		public static Rational Approximate(double value, double epsilon)
		{
			return Approximate((decimal)value, (decimal)epsilon);
		}

		/// <summary>
		/// Approximate the decimal value accurate to a certain precision.
		/// </summary>
		/// <param name="value">Decimal value to approximate.</param>
		/// <param name="epsilon">Maximum precision to converge.</param>
		/// <returns>An approximation of the value as a rational number.</returns>
		public static Rational Approximate(decimal value, decimal epsilon)
		{
			decimal numerator = Decimal.Truncate(value);
			decimal denominator = Decimal.One;
			decimal fraction = Decimal.Divide(numerator, denominator);
			decimal maxValue = (decimal)Int32.MaxValue;

			while ((Math.Abs(fraction - value) > epsilon) && (denominator < maxValue) && (numerator < maxValue))
			{
				if (fraction < value)
				{
					numerator++;
				}
				else
				{
					denominator++;

					decimal temp = Math.Round(Decimal.Multiply(value, denominator));
					if (temp > maxValue)
					{
						denominator--;
						break;
					}

					numerator = temp;
				}

				fraction = Decimal.Divide(numerator, denominator);
			}

			return new Rational((int)numerator, (int)denominator);
		}

		/// <summary>
		/// Finds the greatest common divisor and reduces the fraction by this amount.
		/// </summary>
		/// <returns>The reduced rational.</returns>
		public Rational Reduce()
		{
			int numerator = numerator_;
			int denominator = denominator_;
			Reduce(ref numerator, ref denominator);
			return new Rational(numerator, denominator);
		}

		/// <summary>
		/// Finds the greatest common divisor and reduces the fraction by this amount.
		/// </summary>
		/// <returns>The reduced rational.</returns>
		private static void Reduce(ref int numerator, ref int denominator)
		{
			bool reduced = false;

			decimal n = Convert.ToDecimal(numerator);
			decimal d = Convert.ToDecimal(denominator);

			// greatest common divisor
			decimal gcd = GreatestCommonDivisor(n, d);
			if ((gcd != Decimal.One) && (gcd != Decimal.Zero))
			{
				reduced = true;
				n /= gcd;
				d /= gcd;
			}

			// cancel out signs
			if (d < Decimal.Zero)
			{
				reduced = true;
				n = -n;
				d = -d;
			}

			if (reduced)
			{
				numerator = (int)n;
				denominator = (int)d;
			}
		}

		/// <summary>
		/// Lowest Common Denominator.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static decimal LowestCommonDenominator(decimal a, decimal b)
		{
			if ((a == Decimal.Zero) && (b == Decimal.Zero))
				return Decimal.Zero;

			return (a*b)/GreatestCommonDivisor(a, b);
		}

		/// <summary>
		/// Greatest Common Divisor.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static decimal GreatestCommonDivisor(decimal a, decimal b)
		{
			if (a < Decimal.Zero)
				a = -a;
			if (b < Decimal.Zero)
				b = -b;

			while (a != b)
			{
				if (a == Decimal.Zero)
					return b;
				if (b == Decimal.Zero)
					return a;

				if (a > b)
					a %= b;
				else
					b %= a;
			}

			return a;
		}

		#region IConvertible
		public string ToString(IFormatProvider provider)
		{
			return String.Concat(numerator_.ToString(provider), delimiter_, denominator_.ToString(provider));
		}

		public float ToSingle(IFormatProvider provider)
		{
			return ((IConvertible)numerator_).ToSingle(provider)/((IConvertible)denominator_).ToSingle(provider);
		}

		public double ToDouble(IFormatProvider provider)
		{
			return ((IConvertible)numerator_).ToDouble(provider)/((IConvertible)denominator_).ToDouble(provider);
		}

		public decimal ToDecimal(IFormatProvider provider)
		{
			decimal denominator = ((IConvertible)denominator_).ToDecimal(provider);
			if (denominator == Decimal.Zero)
				return Decimal.MaxValue;
			return ((IConvertible)numerator_).ToDecimal(provider)/denominator;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return ((IConvertible)ToDecimal(provider)).ToBoolean(provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return ((IConvertible)ToDecimal(provider)).ToByte(provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return ((IConvertible)ToDecimal(provider)).ToChar(provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return ((IConvertible)ToDecimal(provider)).ToInt16(provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return ((IConvertible)ToDecimal(provider)).ToInt32(provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return ((IConvertible)ToDecimal(provider)).ToInt64(provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return ((IConvertible)ToDecimal(provider)).ToSByte(provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return ((IConvertible)ToDecimal(provider)).ToUInt16(provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return ((IConvertible)ToDecimal(provider)).ToUInt32(provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return ((IConvertible)ToDecimal(provider)).ToUInt64(provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return new DateTime(((IConvertible)this).ToInt64(provider));
		}

		TypeCode IConvertible.GetTypeCode()
		{
			return numerator_.GetTypeCode();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType == null)
				throw new ArgumentNullException("conversionType");

			if (conversionType == GetType())
				return this;

			return Convert.ChangeType(this, conversionType, provider);
		}
		#endregion

		#region IComparable
		/// <summary>
		/// Compares this instance to a specified System.Object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			if (obj is Rational)
			{
				// differentiate between a real zero and a divide by zero
				// work around divide by zero value to get meaningful comparisons
				Rational other = (Rational)obj;
				if (Convert.ToDecimal(denominator_) == Decimal.Zero)
				{
					if (Convert.ToDecimal(other.denominator_) == Decimal.Zero)
					{
						return Convert.ToDecimal(numerator_).CompareTo(Convert.ToDecimal(other.numerator_));
					}
					else if (Convert.ToDecimal(other.numerator_) == Decimal.Zero)
					{
						return Convert.ToDecimal(denominator_).CompareTo(Convert.ToDecimal(other.denominator_));
					}
				}
				else if (Convert.ToDecimal(other.denominator_) == Decimal.Zero)
				{
					if (Convert.ToDecimal(numerator_) == Decimal.Zero)
					{
						return Convert.ToDecimal(denominator_).CompareTo(Convert.ToDecimal(other.denominator_));
					}
				}
			}

			return Convert.ToDecimal(this).CompareTo(Convert.ToDecimal(obj));
		}
		#endregion

		#region IComparable<Rational>
		/// <summary>
		/// Compares this instance to another Rational instance.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(Rational obj)
		{
			return Decimal.Compare(Convert.ToDecimal(this), Convert.ToDecimal(obj));
		}
		#endregion

		private const char delimiter_ = '/';
		private static readonly char[] delimiters_ = new char[] { delimiter_ };

		private readonly int numerator_;
		private readonly int denominator_;
	}
}
