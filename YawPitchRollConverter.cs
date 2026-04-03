/*
 *  Name: YawPitchRollConverter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class YawPitchRollConverter : ExpandableObjectConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object val, Attribute[] attributes)
		{
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(val, attributes);
			return pdc.Sort(new string[3] { "Yaw", "Pitch", "Roll" });
		}
		
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new object[1] { YawPitchRoll.Zero });
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
				return (str.Length > 0) ? YawPitchRoll.Parse(SingleConverter.CorrectDecimalSeparator(str, culture), culture, true) : YawPitchRoll.Zero;

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
			{
				YawPitchRoll ypr = (YawPitchRoll)obj;
				if ((context != null) && (context.GetType().Name == "PropertyDescriptorGridEntry"))
					return String.Format(culture, "{0:0.##} {1:0.##} {2:0.##}", Scalar.Degrees((double)ypr.yaw_), Scalar.Degrees((double)ypr.pitch_), Scalar.Degrees((double)ypr.roll_));
				else
					return String.Format(culture, "{0} {1} {2}", Scalar.Degrees((double)ypr.yaw_), Scalar.Degrees((double)ypr.pitch_), Scalar.Degrees((double)ypr.roll_));
			}

			return base.ConvertTo(context, culture, obj, type);
		}
	}
}
