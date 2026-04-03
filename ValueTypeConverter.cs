/*
 *  Name: ValueTypeConverter
 *  Author: Pawel Mrochen
 */

using System;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Globalization;
using System.Threading;

namespace Foundation.Mathematics
{
	public class ValueTypeConverter : ExpandableObjectConverter
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			if ((context != null) && (context.PropertyDescriptor != null))
			{
				Type objType = context.PropertyDescriptor.PropertyType;
				object obj = Activator.CreateInstance(objType);

				foreach (DictionaryEntry entry in propertyValues)
				{
					PropertyInfo info = objType.GetProperty((string)entry.Key);
					if ((info != null) && info.CanWrite)
					{
						//MethodInfo setMethod = info.GetSetMethod(true);
						//if ((setMethod != null) && !setMethod.IsStatic && (setMethod.GetParameters().Length == 1))
							info.SetValue(obj, entry.Value, null);
					}
				}

				return obj;
			}

			return null;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type type)
		{
			if ((type == typeof(string)) && (context != null) && (context.PropertyDescriptor != null))
			{
				Type objType = context.PropertyDescriptor.PropertyType;
				MethodInfo parseMethod = objType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(IFormatProvider) }, null);
				if (parseMethod != null)
					return true;

				parseMethod = objType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
				if (parseMethod != null)
					return true;
			}

			return base.CanConvertFrom(context, type);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object obj)
		{
			if ((obj is string) && (context != null) && (context.PropertyDescriptor != null))
			{
				Type objType = context.PropertyDescriptor.PropertyType;
				MethodInfo parseMethod = objType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(IFormatProvider) }, null);
				if (parseMethod != null)
					return parseMethod.Invoke(null, new object[] { obj, (IFormatProvider)culture });

				parseMethod = objType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
				if (parseMethod != null)
				{
					CultureInfo prevCulture = Thread.CurrentThread.CurrentCulture;
					Thread.CurrentThread.CurrentCulture = culture;
					try
					{
						return parseMethod.Invoke(null, new object[] { obj });
					}
					finally
					{
						Thread.CurrentThread.CurrentCulture = prevCulture;
					}
				}
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
				return obj.ToString();

			return base.ConvertTo(context, culture, obj, type);
		}
	}

	public class ValueTypeConverter<T> : ExpandableObjectConverter where T : struct
	{
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			Type objType = typeof(T);
			object obj = default(T);

			foreach (DictionaryEntry entry in propertyValues)
			{
				PropertyInfo info = objType.GetProperty((string)entry.Key);
				if ((info != null) && info.CanWrite)
				{
					//MethodInfo setMethod = info.GetSetMethod(true);
					//if ((setMethod != null) && !setMethod.IsStatic && (setMethod.GetParameters().Length == 1))
						info.SetValue(obj, entry.Value/*Convert.ChangeType(entry.Value, info.PropertyType)*/, null);
				}
			}

			return obj;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type type)
		{
			if (type == typeof(string))
			{
				Type objType = typeof(T);
				MethodInfo parseMethod = objType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(IFormatProvider) }, null);
				if (parseMethod != null)
					return true;

				parseMethod = objType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
				if (parseMethod != null)
					return true;
			}

			return base.CanConvertFrom(context, type);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object obj)
		{
			if (obj is string)
			{
				Type objType = typeof(T);
				MethodInfo parseMethod = objType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string), typeof(IFormatProvider) }, null);
				if (parseMethod != null)
					return parseMethod.Invoke(null, new object[] { obj, (IFormatProvider)culture });

				parseMethod = objType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(string) }, null);
				if (parseMethod != null)
				{
					CultureInfo prevCulture = Thread.CurrentThread.CurrentCulture;
					Thread.CurrentThread.CurrentCulture = culture;
					try
					{
						return parseMethod.Invoke(null, new object[] { obj });
					}
					finally
					{
						Thread.CurrentThread.CurrentCulture = prevCulture;
					}
				}
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
				return obj.ToString();

			return base.ConvertTo(context, culture, obj, type);
		}
	}
}
