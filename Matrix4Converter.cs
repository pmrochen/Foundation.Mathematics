/*
 *  Name: Matrix4Converter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class Matrix4Converter : ExpandableObjectConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new object[2] { Matrix4.Zero, Matrix4.Identity });
		}

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
				return (str.Length > 0) ? Matrix4.Parse(SingleConverter.CorrectDecimalSeparator(str, culture), culture) : Matrix4.Identity;

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
				return ((Matrix4)obj).ToString(culture);

			return base.ConvertTo(context, culture, obj, type);
		}
	}
}
