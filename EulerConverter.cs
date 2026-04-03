/*
 *  Name: EulerConverter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class EulerConverter : ExpandableObjectConverter
	{
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object val, Attribute[] attributes)
		{
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(val, attributes);
			return pdc.Sort(new string[4] { "X", "Y", "Z", "Order" });
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new object[6] { Euler.ZeroXYZ, Euler.ZeroXZY, Euler.ZeroYZX, Euler.ZeroYXZ,
				Euler.ZeroZXY, Euler.ZeroZYX });
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
				return (str.Length > 0) ? Euler.Parse(SingleConverter.CorrectDecimalSeparator(str, culture), culture, true) : Euler.ZeroZXY;

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
				Euler e = (Euler)obj;
				if ((context != null) && (context.GetType().Name == "PropertyDescriptorGridEntry"))
					return String.Format(culture, "{0:0.##} {1:0.##} {2:0.##} {3}", Scalar.Degrees((double)e.x_), Scalar.Degrees((double)e.y_), Scalar.Degrees((double)e.z_), e.order_.ToString());
				else
					return String.Format(culture, "{0} {1} {2} {3}", Scalar.Degrees((double)e.x_), Scalar.Degrees((double)e.y_), Scalar.Degrees((double)e.z_), e.order_.ToString());
			}

			return base.ConvertTo(context, culture, obj, type);
		}
	}
}
