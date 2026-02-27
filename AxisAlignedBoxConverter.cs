/*
 *  Name: AxisAlignedBoxConverter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class AxisAlignedBoxConverter : ValueTypeConverter<AxisAlignedBox> //ExpandableObjectConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new object[] { AxisAlignedBox.Empty/*, new AxisAlignedBox(Vector3.Zero, Vector3.Zero)*/ });
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
				return (str.Length > 0) ? AxisAlignedBox.Parse(SingleConverter.CorrectDecimalSeparator(str, culture), culture) : AxisAlignedBox.Empty;

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
				return ((AxisAlignedBox)obj).ToString(culture);

			return base.ConvertTo(context, culture, obj, type);
		}
	}
}
