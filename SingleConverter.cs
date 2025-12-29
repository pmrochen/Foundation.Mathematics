/*
 *  Name: SingleConverter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class SingleConverter : TypeConverter
	{
		//public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		//{
		//	return true;
		//}

		//public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		//{
		//	return new StandardValuesCollection(new object[] { 0f, 0.1f, 0.5f, 1f, 10f, 100f, Single.PositiveInfinity, Single.NegativeInfinity });
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
				return (str.Length > 0) ? Single.Parse(CorrectDecimalSeparator(str, culture), culture) : 0f;

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
				return ((Single)obj).ToString(culture);

			return base.ConvertTo(context, culture, obj, type);
		}

		internal static string CorrectDecimalSeparator(string str, CultureInfo culture)
		{
			if (String.IsNullOrEmpty(str))
				return str;
			else if (culture.NumberFormat.NumberDecimalSeparator[0] == '.')
				return str.Replace(',', '.');
			else if (culture.NumberFormat.NumberDecimalSeparator[0] == ',')
				return str.Replace('.', ',');
			else
				return str;
		}
	}
}
