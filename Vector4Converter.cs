/*
 *  Name: Vector4Converter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class Vector4Converter : ExpandableObjectConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object val, Attribute[] attributes)
		{
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(val, attributes);
			return pdc.Sort(new string[4] { "X", "Y", "Z", "W" });
		}
		
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new object[6] { Vector4.Zero, Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ, Vector4.UnitW, new Vector4(1f, 1f, 1f, 1f) });
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
				return (str.Length > 0) ? Vector4.Parse(SingleConverter.CorrectDecimalSeparator(str, culture), culture) : Vector4.Zero;

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
				return ((Vector4)obj).ToString(culture);

			return base.ConvertTo(context, culture, obj, type);
		}
	}
}
