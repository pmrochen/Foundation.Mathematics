/*
 *  Name: IntVector4Converter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class IntVector4Converter : ExpandableObjectConverter
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
			return new StandardValuesCollection(new object[1] { IntVector4.Zero });
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
				return (str.Length > 0) ? IntVector4.Parse(str, culture) : IntVector4.Zero;

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
				return ((IntVector4)obj).ToString(culture);

			return base.ConvertTo(context, culture, obj, type);
		}
	}
}
