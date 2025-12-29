/*
 *  Name: DoubleConverter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class DoubleConverter : TypeConverter
	{
		//public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		//{
		//	return true;
		//}

		//public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		//{
		//	return new StandardValuesCollection(new object[] { 0.0, 0.1, 0.5, 1.0, 10.0, 100.0, Double.PositiveInfinity, Double.NegativeInfinity });
		//}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type type)
		{
			if (type == typeof(string))
				return true;

			return base.CanConvertFrom(context, type);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object obj)
		{
			string str = obj as string;
			if (str != null)
				return (str.Length > 0) ? Double.Parse(CorrectDecimalSeparator(str, culture), culture) : 0.0;

			return base.ConvertFrom(context, culture, obj);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type type)
		{
			if (type == typeof(string))
				return true;

			return base.CanConvertTo(context, type);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object obj, Type type)
		{
			if (type == typeof(string))
				return ((Double)obj).ToString(culture);

			return base.ConvertTo(context, culture, obj, type);
		}

		internal static string CorrectDecimalSeparator(string str, CultureInfo culture)
		{
			return SingleConverter.CorrectDecimalSeparator(str, culture);
		}
	}
}
