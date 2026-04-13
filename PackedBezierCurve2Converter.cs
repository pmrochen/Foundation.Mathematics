/*
 *  Name: PackedBezierCurve2Converter
 *  Description:
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class PackedBezierCurve2Converter : ExpandableObjectConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object val, Attribute[] attributes)
		{
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(val, attributes);
			return pdc.Sort(new string[2] { "ControlPoint1", "ControlPoint2" });
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new object[]
			{
				new PackedBezierCurve2(new Vector2(0.000f, 0.000f), new Vector2(1.000f, 1.000f)),
				new PackedBezierCurve2(new Vector2(0.470f, 0.000f), new Vector2(0.745f, 0.715f)),
				new PackedBezierCurve2(new Vector2(0.550f, 0.085f), new Vector2( 0.680f, 0.530f)),
				new PackedBezierCurve2(new Vector2(0.550f, 0.055f), new Vector2( 0.675f, 0.190f)),
				new PackedBezierCurve2(new Vector2(0.895f, 0.030f), new Vector2(0.685f, 0.220f)),
				new PackedBezierCurve2(new Vector2(0.755f, 0.050f), new Vector2(0.855f, 0.060f)),
				new PackedBezierCurve2(new Vector2(0.950f, 0.050f), new Vector2(0.795f, 0.035f)),
				new PackedBezierCurve2(new Vector2(0.600f, 0.040f), new Vector2(0.980f, 0.335f)),
				new PackedBezierCurve2(new Vector2(0.600f, -0.28f), new Vector2(0.735f, 0.045f)),
				new PackedBezierCurve2(new Vector2(0.390f, 0.575f), new Vector2(0.565f, 1.000f)),
				new PackedBezierCurve2(new Vector2(0.250f, 0.460f), new Vector2(0.450f, 0.940f)),
				new PackedBezierCurve2(new Vector2(0.215f, 0.610f), new Vector2(0.355f, 1.000f)),
				new PackedBezierCurve2(new Vector2(0.165f, 0.840f), new Vector2(0.440f, 1.000f)),
				new PackedBezierCurve2(new Vector2(0.230f, 1.000f), new Vector2(0.320f, 1.000f)),
				new PackedBezierCurve2(new Vector2(0.190f, 1.000f), new Vector2(0.220f, 1.000f)),
				new PackedBezierCurve2(new Vector2(0.075f, 0.820f), new Vector2(0.165f, 1.000f)),
				new PackedBezierCurve2(new Vector2(0.175f, 0.885f), new Vector2(0.320f, 1.275f)),
				new PackedBezierCurve2(new Vector2(0.445f, 0.050f), new Vector2(0.550f, 0.950f)),
				new PackedBezierCurve2(new Vector2(0.455f, 0.030f), new Vector2(0.515f, 0.955f)),
				new PackedBezierCurve2(new Vector2(0.645f, 0.045f), new Vector2(0.355f, 1.000f)),
				new PackedBezierCurve2(new Vector2(0.770f, 0.000f), new Vector2(0.175f, 1.000f)),
				new PackedBezierCurve2(new Vector2(0.860f, 0.000f), new Vector2(0.070f, 1.000f)),
				new PackedBezierCurve2(new Vector2(1.000f, 0.000f), new Vector2(0.000f, 1.000f)),
				new PackedBezierCurve2(new Vector2(0.785f, 0.135f), new Vector2(0.150f, 0.860f)),
				new PackedBezierCurve2(new Vector2(0.680f, -0.55f), new Vector2(0.265f, 1.550f))
			});
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
				return (str.Length > 0) ? PackedBezierCurve2.Parse(SingleConverter.CorrectDecimalSeparator(str, culture), culture) : PackedBezierCurve2.Zero;

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
				return ((PackedBezierCurve2)obj).ToString(culture);

			return base.ConvertTo(context, culture, obj, type);
		}
	}
}
