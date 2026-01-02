/*
 *  Name: DegreeConverter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Globalization;

namespace Foundation.Mathematics
{
	public class DegreeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type type)
		{
			if ((type == typeof(string)) && (context != null) && (context.PropertyDescriptor != null))
			{
				Type objType = context.PropertyDescriptor.PropertyType;
				if ((objType == typeof(float)) || (objType == typeof(double)))
					return true;
			}

			return base.CanConvertFrom(context, type);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object obj)
		{
			if ((obj is string) && (context != null) && (context.PropertyDescriptor != null))
			{
				string str = (string)obj;
				Type objType = context.PropertyDescriptor.PropertyType;
				double val = (str.Length > 0) ? 
					Functions.Radians(Functions.ParseDouble(SingleConverter.CorrectDecimalSeparator(str, culture), culture)) :
					0.0;
				if (objType == typeof(float))
					return (float)val;
				else if (objType == typeof(double))
					return val;
			}

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
				if (obj is float)
				{
					if ((context != null) && (context.GetType().Name == "PropertyDescriptorGridEntry"))
						return Functions.Degrees((double)(float)obj).ToString("0.##", culture);
					else
						return Functions.Degrees((double)(float)obj).ToString(culture);
				}
				else if (obj is double)
				{
					if ((context != null) && (context.GetType().Name == "PropertyDescriptorGridEntry"))
						return Functions.Degrees((double)obj).ToString("0.##", culture);
					else
						return Functions.Degrees((double)obj).ToString(culture);
				}
				else
				{
					return null;
				}
			}

			return base.ConvertTo(context, culture, obj, type);
		}
	}
}
