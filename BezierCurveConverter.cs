/*
 *  Name: BezierCurveConverter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class BezierCurveConverter : ExpandableObjectConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object val, Attribute[] attributes)
		{
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(val, attributes);
			return pdc.Sort(new string[4] { "ControlPoint0", "ControlPoint1", "ControlPoint2", "ControlPoint3" });
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new object[] { new BezierCurve(0f, 0f, 0f, 0f), new BezierCurve(1f, 1f, 1f, 1f),
				new BezierCurve(0f, 1f/3f, 2f/3f, 1f), new BezierCurve(1f, 2f/3f, 1f/3f, 0f),
				new BezierCurve(0f, 0f, 1f, 1f), new BezierCurve(1f, 1f, 0f, 0f),
				new BezierCurve(0f, 0f, 0f, 1f), new BezierCurve(1f, 0f, 0f, 0f),
				new BezierCurve(0f, 1f, 1f, 1f), new BezierCurve(1f, 1f, 1f, 0f),
				new BezierCurve(0f, 4f/3f, 4f/3f, 0f), new BezierCurve(1f, -1f/3f, -1f/3f, 1f) });
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
				return (str.Length > 0) ? BezierCurve.Parse(SingleConverter.CorrectDecimalSeparator(str, culture), culture) : BezierCurve.Zero;

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
				return ((BezierCurve)obj).ToString(culture);

			return base.ConvertTo(context, culture, obj, type);
		}
	}
}
