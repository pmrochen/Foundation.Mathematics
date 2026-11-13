using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Foundation.Mathematics.Tests;

internal static class TestCultures
{
	/// <summary>Invariant culture with ',' as the decimal separator (independent of installed ICU data).</summary>
	public static readonly CultureInfo CommaDecimal = CreateCommaDecimal();

	private static CultureInfo CreateCommaDecimal()
	{
		CultureInfo culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
		culture.NumberFormat.NumberDecimalSeparator = ",";
		culture.NumberFormat.NumberGroupSeparator = " ";
		return CultureInfo.ReadOnly(culture);
	}
}

/// <summary>
/// One representative value of every formattable type in the library.
/// </summary>
internal static class FormattableSamples
{
	public static readonly Dictionary<Type, object> Values = new Dictionary<Type, object>
	{
		[typeof(Vector2)] = new Vector2(1.5f, -2.25f),
		[typeof(Vector3)] = new Vector3(1.5f, -2.25f, 3f),
		[typeof(Vector4)] = new Vector4(1.5f, -2.25f, 3f, -0.125f),
		[typeof(IntVector2)] = new IntVector2(1, -2),
		[typeof(IntVector3)] = new IntVector3(1, -2, 3),
		[typeof(IntVector4)] = new IntVector4(1, -2, 3, -4),
		[typeof(Matrix2)] = new Matrix2(1.5f, -2f, 0.25f, 4f),
		[typeof(Matrix3)] = new Matrix3(1.5f, -2f, 0.25f, 4f, 5f, -6.5f, 7f, 8f, 9f),
		[typeof(Matrix4)] = new Matrix4(1.5f, -2f, 0.25f, 4f, 5f, -6.5f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 15f, 16.75f),
		[typeof(Quaternion)] = new Quaternion(0.5f, -0.5f, 0.25f, 0.75f),
		[typeof(Euler)] = new Euler(0.5f, -1.25f, 2f, EulerOrder.YZX),
		[typeof(YawPitchRoll)] = new YawPitchRoll(0.5f, -1.25f, 2f),
		[typeof(AffineTransform)] = new AffineTransform(1.5f, -2f, 0.25f, 4f, 5f, -6.5f, 7f, 8f, 9f, 10.5f, -11f, 12f),
		[typeof(PackedTransform)] = new PackedTransform(new Vector3(1.5f, -2f, 3f), new Quaternion(0f, 0.5f, 0f, -0.75f), 2.5f),
		[typeof(Interval)] = new Interval(-1.5f, 2.25f),
		[typeof(AxisAlignedRectangle)] = new AxisAlignedRectangle(new Vector2(-1.5f, 2f), new Vector2(3f, 4.25f)),
		[typeof(AxisAlignedBox)] = new AxisAlignedBox(new Vector3(-1.5f, 2f, 0f), new Vector3(3f, 4.25f, 1f)),
		[typeof(Plane)] = new Plane(0.5f, -1f, 2.25f, -3f),
		[typeof(HalfSpace)] = new HalfSpace(0.5f, -1f, 2.25f, -3f),
		[typeof(Line2)] = new Line2(new Vector2(1.5f, -2f), new Vector2(0.5f, 0.25f)),
		[typeof(Line3)] = new Line3(new Vector3(1.5f, -2f, 3f), new Vector3(0.5f, 0.25f, -1f)),
		[typeof(Ray2)] = new Ray2(new Vector2(1.5f, -2f), new Vector2(0.5f, 0.25f)),
		[typeof(Ray3)] = new Ray3(new Vector3(1.5f, -2f, 3f), new Vector3(0.5f, 0.25f, -1f)),
		[typeof(LineSegment2)] = new LineSegment2(new Vector2(1.5f, -2f), new Vector2(0.5f, 0.25f)),
		[typeof(LineSegment3)] = new LineSegment3(new Vector3(1.5f, -2f, 3f), new Vector3(0.5f, 0.25f, -1f)),
		[typeof(Circle2)] = new Circle2(new Vector2(1.5f, -2f), 0.75f),
		[typeof(Sphere)] = new Sphere(new Vector3(1.5f, -2f, 3f), 0.75f),
		[typeof(Ellipsoid)] = new Ellipsoid(new Vector3(1.5f, -2f, 3f), new Matrix3(0f, 1f, 0f, -1f, 0f, 0f, 0f, 0f, 1f), new Vector3(0.5f, 1.25f, 2f)),
		[typeof(Triangle3)] = new Triangle3(new Vector3(1.5f, -2f, 3f), new Vector3(0.5f, 0.25f, -1f), new Vector3(4f, 5f, 6.5f)),
		[typeof(Quadrilateral3)] = new Quadrilateral3(new Vector3(1.5f, -2f, 3f), new Vector3(0.5f, 0.25f, -1f), new Vector3(4f, 5f, 6.5f), new Vector3(-7f, 8f, 9f)),
		[typeof(Tetrahedron)] = new Tetrahedron(new Vector3(1.5f, -2f, 3f), new Vector3(0.5f, 0.25f, -1f), new Vector3(4f, 5f, 6.5f), new Vector3(-7f, 8f, 9f)),
		[typeof(OrientedBox)] = new OrientedBox(new Vector3(1.5f, -2f, 3f), new Matrix3(0f, 1f, 0f, -1f, 0f, 0f, 0f, 0f, 1f), new Vector3(0.5f, 1.25f, 2f)),
		[typeof(SymmetricFrustum)] = new SymmetricFrustum(new Vector3(1.5f, -2f, 3f), new Matrix3(0f, 1f, 0f, -1f, 0f, 0f, 0f, 0f, 1f), new Vector2(0.5f, 0.25f), 1.5f, 100f),
		[typeof(Cylinder)] = new Cylinder(new Vector3(1.5f, -2f, 3f), Vector3.UnitY, 2.5f, 0.75f),
		[typeof(Cone)] = new Cone(new Vector3(1.5f, -2f, 3f), Vector3.UnitY, 2.5f, 0.75f),
		[typeof(Capsule)] = new Capsule(new Vector3(1.5f, -2f, 3f), Vector3.UnitY, 2.5f, 0.75f),
		[typeof(Torus)] = new Torus(new Vector3(1.5f, -2f, 3f), Vector3.UnitY, 2.5f, 0.75f),
		[typeof(BezierCurve2)] = new BezierCurve2(new Vector2(1.5f, -2f), new Vector2(0.5f, 0.25f), new Vector2(4f, 5f), new Vector2(-7f, 8f)),
		[typeof(BezierCurve3)] = new BezierCurve3(new Vector3(1.5f, -2f, 3f), new Vector3(0.5f, 0.25f, -1f), new Vector3(4f, 5f, 6.5f), new Vector3(-7f, 8f, 9f)),
		[typeof(PackedBezierCurve2)] = new PackedBezierCurve2(new Vector2(0.25f, 0.1f), new Vector2(0.75f, 1.5f)),
	};

	public static IEnumerable<object[]> Types => Values.Keys.OrderBy(t => t.Name).Select(t => new object[] { t });
}

[TestClass]
public sealed class FormattingRoundTripTests
{
	public static IEnumerable<object[]> FormattableTypes => FormattableSamples.Types;

	public static string DisplayName(MethodInfo method, object[] data) => $"{method.Name}({((Type)data[0]).Name})";

	private static object Parse(Type type, string text, IFormatProvider provider)
	{
		MethodInfo parse = (provider != null) ?
			type.GetMethod("Parse", new[] { typeof(string), typeof(IFormatProvider) }) :
			type.GetMethod("Parse", new[] { typeof(string) });
		Assert.IsNotNull(parse, $"{type.Name}.Parse not found.");
		try
		{
			return parse.Invoke(null, (provider != null) ? new object[] { text, provider } : new object[] { text });
		}
		catch (TargetInvocationException e)
		{
			throw e.InnerException;
		}
	}

	private static string Format(object value, IFormatProvider provider)
	{
		MethodInfo toString = value.GetType().GetMethod("ToString", new[] { typeof(IFormatProvider) });
		Assert.IsNotNull(toString, $"{value.GetType().Name}.ToString(IFormatProvider) not found.");
		return (string)toString.Invoke(value, new object[] { provider });
	}

	[TestMethod]
	public void EveryFormattableLibraryTypeHasASample()
	{
		var formattable = typeof(Vector3).Assembly.GetExportedTypes()
			.Where(t => t.IsValueType && !t.IsEnum && typeof(IFormattable).IsAssignableFrom(t));
		foreach (Type type in formattable)
			Assert.IsTrue(FormattableSamples.Values.ContainsKey(type), $"No formatting sample for {type.Name}.");
	}

	[TestMethod]
	[DynamicData(nameof(FormattableTypes), DynamicDataDisplayName = nameof(DisplayName))]
	public void ToString_Parse_RoundTrip_WithProvider(Type type)
	{
		object value = FormattableSamples.Values[type];
		foreach (CultureInfo culture in new[] { CultureInfo.InvariantCulture, TestCultures.CommaDecimal })
		{
			string text = Format(value, culture);
			Assert.AreEqual(value, Parse(type, text, culture), $"{type.Name} via '{text}' ({culture.NumberFormat.NumberDecimalSeparator})");
			string viaFormattable = ((IFormattable)value).ToString(null, culture);
			Assert.AreEqual(value, Parse(type, viaFormattable, culture), $"{type.Name} via IFormattable '{viaFormattable}'");
		}
		Assert.IsFalse(Format(value, TestCultures.CommaDecimal).Contains('.'), "Decimal separator of the provider was not used.");
	}

	[TestMethod]
	[DoNotParallelize]
	[DynamicData(nameof(FormattableTypes), DynamicDataDisplayName = nameof(DisplayName))]
	public void ToString_Parse_RoundTrip_WithCurrentCulture(Type type)
	{
		object value = FormattableSamples.Values[type];
		foreach (CultureInfo culture in new[] { CultureInfo.InvariantCulture, TestCultures.CommaDecimal })
		{
			using (new CultureScope(culture))
			{
				string text = value.ToString();
				Assert.AreEqual(value, Parse(type, text, null), $"{type.Name} via '{text}'");
				Assert.AreEqual(Format(value, culture), text);
			}
		}
	}

	[TestMethod]
	[DynamicData(nameof(FormattableTypes), DynamicDataDisplayName = nameof(DisplayName))]
	public void Parse_RejectsNullAndWrongTokenCount(Type type)
	{
		Assert.ThrowsExactly<ArgumentNullException>(() => Parse(type, null, CultureInfo.InvariantCulture));
		Assert.ThrowsExactly<ArgumentNullException>(() => Parse(type, null, null));
		string text = Format(FormattableSamples.Values[type], CultureInfo.InvariantCulture);
		Assert.ThrowsExactly<FormatException>(() => Parse(type, text + " 1", CultureInfo.InvariantCulture));
		Assert.ThrowsExactly<FormatException>(() => Parse(type, "", CultureInfo.InvariantCulture));
	}
}

[TestClass]
public sealed class TypeConverterTests
{
	private static readonly Dictionary<Type, Type> ConverterTypes = new Dictionary<Type, Type>
	{
		[typeof(AffineTransform)] = typeof(AffineTransformConverter),
		[typeof(AxisAlignedBox)] = typeof(AxisAlignedBoxConverter),
		[typeof(AxisAlignedRectangle)] = typeof(AxisAlignedRectangleConverter),
		[typeof(Euler)] = typeof(EulerConverter),
		[typeof(HalfSpace)] = typeof(HalfSpaceConverter),
		[typeof(IntVector2)] = typeof(IntVector2Converter),
		[typeof(IntVector3)] = typeof(IntVector3Converter),
		[typeof(IntVector4)] = typeof(IntVector4Converter),
		[typeof(Interval)] = typeof(IntervalConverter),
		[typeof(Matrix2)] = typeof(Matrix2Converter),
		[typeof(Matrix3)] = typeof(Matrix3Converter),
		[typeof(Matrix4)] = typeof(Matrix4Converter),
		[typeof(PackedBezierCurve2)] = typeof(PackedBezierCurve2Converter),
		[typeof(PackedTransform)] = typeof(PackedTransformConverter),
		[typeof(Plane)] = typeof(PlaneConverter),
		[typeof(Quaternion)] = typeof(QuaternionConverter),
		[typeof(Vector2)] = typeof(Vector2Converter),
		[typeof(Vector3)] = typeof(Vector3Converter),
		[typeof(Vector4)] = typeof(Vector4Converter),
		[typeof(YawPitchRoll)] = typeof(YawPitchRollConverter),
	};

	public static IEnumerable<object[]> ConvertibleTypes => ConverterTypes.Keys.OrderBy(t => t.Name).Select(t => new object[] { t });

	public static string DisplayName(MethodInfo method, object[] data) => $"{method.Name}({((Type)data[0]).Name})";

	private static bool AreEquivalent(object expected, object actual)
	{
		switch (expected)
		{
			case Euler e: return (actual is Euler a) && e.ApproxEquals(a, 1e-5f);
			case YawPitchRoll y: return (actual is YawPitchRoll b) && y.ApproxEquals(b, 1e-5f);
			default: return expected.Equals(actual);
		}
	}

	/// <summary>Minimal descriptor context exposing a property descriptor, as a property grid would.</summary>
	private sealed class PropertyContext : ITypeDescriptorContext
	{
		public PropertyContext(PropertyDescriptor descriptor) { PropertyDescriptor = descriptor; }
		public IContainer Container => null;
		public object Instance => null;
		public PropertyDescriptor PropertyDescriptor { get; }
		public object GetService(Type serviceType) => null;
		public void OnComponentChanged() { }
		public bool OnComponentChanging() => true;
	}

	[TestMethod]
	[DynamicData(nameof(ConvertibleTypes), DynamicDataDisplayName = nameof(DisplayName))]
	public void ConverterIsRegisteredAndRoundTripsStrings(Type type)
	{
		TypeConverter converter = TypeDescriptor.GetConverter(type);
		Assert.IsInstanceOfType(converter, ConverterTypes[type]);
		Assert.IsTrue(converter.CanConvertFrom(typeof(string)));
		Assert.IsTrue(converter.CanConvertTo(typeof(string)));
		object value = FormattableSamples.Values[type];
		foreach (CultureInfo culture in new[] { CultureInfo.InvariantCulture, TestCultures.CommaDecimal })
		{
			string text = converter.ConvertToString(null, culture, value);
			object back = converter.ConvertFromString(null, culture, text);
			Assert.IsTrue(AreEquivalent(value, back), $"{type.Name}: '{text}' -> {back}");
		}
	}

	[TestMethod]
	[DynamicData(nameof(ConvertibleTypes), DynamicDataDisplayName = nameof(DisplayName))]
	public void ConverterAcceptsEitherDecimalSeparatorAndEmptyStrings(Type type)
	{
		TypeConverter converter = TypeDescriptor.GetConverter(type);
		object value = FormattableSamples.Values[type];
		string invariantText = converter.ConvertToString(null, CultureInfo.InvariantCulture, value);
		object back = converter.ConvertFromString(null, TestCultures.CommaDecimal, invariantText);
		Assert.IsTrue(AreEquivalent(value, back), $"{type.Name}: '{invariantText}' parsed with ',' culture -> {back}");
		object empty = converter.ConvertFromString(null, CultureInfo.InvariantCulture, "");
		Assert.IsInstanceOfType(empty, type);
	}

	[TestMethod]
	[DynamicData(nameof(ConvertibleTypes), DynamicDataDisplayName = nameof(DisplayName))]
	public void StandardValuesHaveTheConvertedType(Type type)
	{
		TypeConverter converter = TypeDescriptor.GetConverter(type);
		Assert.IsTrue(converter.GetStandardValuesSupported());
		ICollection values = converter.GetStandardValues();
		Assert.IsTrue(values.Count > 0);
		foreach (object v in values)
			Assert.IsInstanceOfType(v, type, $"{converter.GetType().Name} offers a standard value of type {v.GetType().Name}.");
	}

	[TestMethod]
	public void EulerAndYawPitchRollConverters_UseDegrees()
	{
		TypeConverter euler = TypeDescriptor.GetConverter(typeof(Euler));
		Euler e = (Euler)euler.ConvertFromString(null, CultureInfo.InvariantCulture, "90 0 -45 XYZ");
		Assert.IsTrue(new Euler(SingleConstants.HalfPi, 0f, -SingleConstants.Pi/4f, EulerOrder.XYZ).ApproxEquals(e));
		Assert.AreEqual(Euler.ZeroZXY, euler.ConvertFromString(null, CultureInfo.InvariantCulture, ""));
		Assert.AreEqual("0 0 0 YXZ", euler.ConvertToString(null, CultureInfo.InvariantCulture, Euler.ZeroYXZ));

		TypeConverter ypr = TypeDescriptor.GetConverter(typeof(YawPitchRoll));
		YawPitchRoll y = (YawPitchRoll)ypr.ConvertFromString(null, CultureInfo.InvariantCulture, "180 -90 0");
		Assert.IsTrue(new YawPitchRoll(SingleConstants.Pi, -SingleConstants.HalfPi, 0f).ApproxEquals(y));
		Assert.AreEqual("0 0 0", ypr.ConvertToString(null, CultureInfo.InvariantCulture, YawPitchRoll.Zero));
	}

	[TestMethod]
	public void ExpandableConverters_ExposeComponentProperties()
	{
		PropertyDescriptorCollection eulerProperties = TypeDescriptor.GetConverter(typeof(Euler)).GetProperties(Euler.ZeroXYZ);
		CollectionAssert.AreEqual(new[] { "X", "Y", "Z", "Order" }, eulerProperties.Cast<PropertyDescriptor>().Select(p => p.Name).Take(4).ToArray());
		PropertyDescriptorCollection curveProperties = TypeDescriptor.GetConverter(typeof(PackedBezierCurve2)).GetProperties(PackedBezierCurve2.Linear);
		CollectionAssert.IsSubsetOf(new[] { "ControlPoint1", "ControlPoint2" }, curveProperties.Cast<PropertyDescriptor>().Select(p => p.Name).ToArray());
		PropertyDescriptorCollection vectorProperties = TypeDescriptor.GetConverter(typeof(Vector3)).GetProperties(Vector3.Zero);
		CollectionAssert.IsSubsetOf(new[] { "X", "Y", "Z" }, vectorProperties.Cast<PropertyDescriptor>().Select(p => p.Name).ToArray());
	}

	[TestMethod]
	public void ValueTypeConverterOfT_CreatesInstanceFromPropertyValues()
	{
		TypeConverter converter = TypeDescriptor.GetConverter(typeof(Interval));
		Assert.IsTrue(converter.GetCreateInstanceSupported());
		object created = converter.CreateInstance(new Hashtable { ["Minimum"] = -1f, ["Maximum"] = 2f, ["Unknown"] = 3f });
		Assert.AreEqual(new Interval(-1f, 2f), created);
		Assert.AreEqual(Interval.Empty, converter.ConvertFromString(null, CultureInfo.InvariantCulture, ""));
		Assert.AreEqual(new Interval(1.5f, 2f), converter.ConvertFromString(null, CultureInfo.InvariantCulture, "1,5 2"));

		TypeConverter boxConverter = TypeDescriptor.GetConverter(typeof(AxisAlignedBox));
		object box = boxConverter.CreateInstance(new Hashtable { ["Minimum"] = Vector3.Zero, ["Maximum"] = new Vector3(1f) });
		Assert.AreEqual(new AxisAlignedBox(Vector3.Zero, new Vector3(1f)), box);
	}

	[TestMethod]
	public void ValueTypeConverter_UsesPropertyTypeFromContext()
	{
		var converter = new ValueTypeConverter();
		var context = new PropertyContext(TypeDescriptor.GetProperties(typeof(AxisAlignedBox))["Minimum"]);
		Assert.IsTrue(converter.GetCreateInstanceSupported(context));
		Assert.IsTrue(converter.CanConvertFrom(context, typeof(string)));
		Assert.IsTrue(converter.CanConvertTo(context, typeof(string)));
		Assert.AreEqual(new Vector3(1f, 2.5f, 3f), converter.ConvertFrom(context, CultureInfo.InvariantCulture, "1 2.5 3"));
		Assert.AreEqual(new Vector3(1f, 2.5f, 3f), converter.ConvertFrom(context, TestCultures.CommaDecimal, "1 2,5 3"));
		Assert.AreEqual("1 2 3", converter.ConvertTo(context, CultureInfo.InvariantCulture, new IntVector3(1, 2, 3), typeof(string)));
		object created = converter.CreateInstance(context, new Hashtable { ["X"] = 1f, ["Y"] = 2f, ["Z"] = 3f });
		Assert.AreEqual(new Vector3(1f, 2f, 3f), created);
		Assert.IsNull(converter.CreateInstance(null, new Hashtable()));
	}

	[TestMethod]
	public void SingleAndDoubleConverters()
	{
		var single = new SingleConverter();
		Assert.IsTrue(single.CanConvertFrom(typeof(string)));
		Assert.IsTrue(single.CanConvertTo(typeof(string)));
		Assert.AreEqual(1.5f, single.ConvertFrom(null, CultureInfo.InvariantCulture, "1,5"));
		Assert.AreEqual(1.5f, single.ConvertFrom(null, TestCultures.CommaDecimal, "1.5"));
		Assert.AreEqual(0f, single.ConvertFrom(null, CultureInfo.InvariantCulture, ""));
		Assert.AreEqual("1,5", single.ConvertTo(null, TestCultures.CommaDecimal, 1.5f, typeof(string)));

		var dbl = new DoubleConverter();
		Assert.IsTrue(dbl.CanConvertFrom(typeof(string)));
		Assert.IsTrue(dbl.CanConvertTo(typeof(string)));
		Assert.AreEqual(2.25, dbl.ConvertFrom(null, CultureInfo.InvariantCulture, "2,25"));
		Assert.AreEqual(2.25, dbl.ConvertFrom(null, TestCultures.CommaDecimal, "2.25"));
		Assert.AreEqual(0.0, dbl.ConvertFrom(null, CultureInfo.InvariantCulture, ""));
		Assert.AreEqual("2,25", dbl.ConvertTo(null, TestCultures.CommaDecimal, 2.25, typeof(string)));
	}

	[TestMethod]
	public void SingleAndDoubleConverters_ParseSpecialValues()
	{
		NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
		Assert.AreEqual(Single.PositiveInfinity, SingleConverter.Parse(nfi.PositiveInfinitySymbol, CultureInfo.InvariantCulture));
		Assert.AreEqual(Single.NegativeInfinity, SingleConverter.Parse(" " + nfi.NegativeInfinitySymbol, CultureInfo.InvariantCulture));
		Assert.IsTrue(Single.IsNaN(SingleConverter.Parse(nfi.NaNSymbol, CultureInfo.InvariantCulture)));
		Assert.AreEqual(1.25f, SingleConverter.Parse("1.25", CultureInfo.InvariantCulture));
		Assert.AreEqual(Double.PositiveInfinity, DoubleConverter.Parse(nfi.PositiveInfinitySymbol, CultureInfo.InvariantCulture));
		Assert.AreEqual(Double.NegativeInfinity, DoubleConverter.Parse(nfi.NegativeInfinitySymbol, CultureInfo.InvariantCulture));
		Assert.IsTrue(Double.IsNaN(DoubleConverter.Parse(nfi.NaNSymbol, CultureInfo.InvariantCulture)));
		Assert.AreEqual(1.25, DoubleConverter.Parse("1.25", CultureInfo.InvariantCulture));
		using (CultureScope.Invariant())
		{
			Assert.AreEqual(Single.PositiveInfinity, SingleConverter.Parse(nfi.PositiveInfinitySymbol));
			Assert.AreEqual(Single.NegativeInfinity, SingleConverter.Parse(nfi.NegativeInfinitySymbol));
			Assert.IsTrue(Single.IsNaN(SingleConverter.Parse(nfi.NaNSymbol)));
			Assert.AreEqual(-3.5f, SingleConverter.Parse("-3.5"));
			Assert.AreEqual(Double.PositiveInfinity, DoubleConverter.Parse(nfi.PositiveInfinitySymbol));
			Assert.AreEqual(-3.5, DoubleConverter.Parse("-3.5"));
		}
	}

	[TestMethod]
	public void DegreeConverter_ConvertsBetweenDegreesAndRadians()
	{
		var converter = new DegreeConverter();
		PropertyDescriptor yaw = TypeDescriptor.GetProperties(typeof(YawPitchRoll))["Yaw"];
		Assert.IsInstanceOfType(yaw.Converter, typeof(DegreeConverter));
		var context = new PropertyContext(yaw);
		Assert.IsTrue(converter.CanConvertFrom(context, typeof(string)));
		Assert.IsFalse(converter.CanConvertFrom(null, typeof(string)));
		Assert.IsTrue(converter.CanConvertTo(context, typeof(string)));
		AssertEx.AreEqual(SingleConstants.HalfPi, (float)converter.ConvertFrom(context, CultureInfo.InvariantCulture, "90"));
		AssertEx.AreEqual(SingleConstants.Pi/4f, (float)converter.ConvertFrom(context, CultureInfo.InvariantCulture, "45,0"));
		Assert.AreEqual(0f, converter.ConvertFrom(context, CultureInfo.InvariantCulture, ""));
		Assert.AreEqual(180.0, Double.Parse((string)converter.ConvertTo(null, CultureInfo.InvariantCulture, Math.PI, typeof(string)), CultureInfo.InvariantCulture), 1e-9);
		AssertEx.AreEqual(90f, Single.Parse((string)converter.ConvertTo(null, CultureInfo.InvariantCulture, SingleConstants.HalfPi, typeof(string)), CultureInfo.InvariantCulture), 1e-4f);
		Assert.IsNull(converter.ConvertTo(null, CultureInfo.InvariantCulture, "not a number", typeof(string)));
	}
}
