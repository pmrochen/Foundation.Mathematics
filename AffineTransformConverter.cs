/*
 *  Name: AffineTransformConverter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class AffineTransformConverter : ExpandableObjectConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new object[2] { AffineTransform.Zero, AffineTransform.Identity });
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
				return (str.Length > 0) ? AffineTransform.Parse(SingleConverter.CorrectDecimalSeparator(str, culture), culture) : AffineTransform.Identity;

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
				return ((AffineTransform)obj).ToString(culture);

			return base.ConvertTo(context, culture, obj, type);
		}
	}
}
