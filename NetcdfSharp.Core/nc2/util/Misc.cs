// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.util
{

	using DataType = NetcdfSharp.ucar.ma2.DataType;

	/// <summary>
	/// Miscellaneous static routines. </summary>
	public class Misc
	{
	  /// <summary>
	  /// Estimates pointer size in bytes </summary>
	  public const int referenceSize = 4;
	  /// <summary>
	  /// Estimates object size in bytes </summary>
	  public const int objectSize = 16;

	  /// <summary>
	  /// The default maximum <seealso cref="relativeDifference(float, float) relative difference"/> that two floats can have in
	  /// order to be deemed <seealso cref="nearlyEquals(float, float) nearly equal"/>.
	  /// </summary>
	  public const float defaultMaxRelativeDiffFloat = 1.0e-5f;

	  /// <summary>
	  /// The default maximum <seealso cref="relativeDifference(double, double) relative difference"/> that two doubles can have in
	  /// order to be deemed <seealso cref="nearlyEquals(double, double) nearly equal"/>.
	  /// </summary>
	  public const double defaultMaxRelativeDiffDouble = 1.0e-8;

	  /// <summary>
	  /// The absolute difference between two floats, i.e. {@code |a - b|}. </summary>
	  public static float absoluteDifference(float a, float b)
	  {
		if (a.CompareTo(b) == 0)
		{ // Shortcut: handles infinities and NaNs.
		  return 0;
		}
		else
		{
		  return Math.Abs(a - b);
		}
	  }

	  /// <summary>
	  /// The absolute difference between two doubles, i.e. {@code |a - b|}. </summary>
	  public static double absoluteDifference(double a, double b)
	  {
		if (a.CompareTo(b) == 0)
		{ // Shortcut: handles infinities and NaNs.
		  return 0;
		}
		else
		{
		  return Math.Abs(a - b);
		}
	  }

	  /// <summary>
	  /// Returns the relative difference between two numbers, i.e. {@code |a - b| / max(|a|, |b|)}.
	  /// <para>
	  /// For cases where {@code a == 0}, {@code b == 0}, or {@code a} and {@code b} are extremely close, traditional
	  /// relative difference calculation breaks down. So, in those instances, we compute the difference relative to
	  /// <seealso cref="JavaLangCompat.FloatMinNormal"/>, i.e. {@code |a - b| / JavaLangCompat.FloatMinNormal}.
	  /// 
	  /// </para>
	  /// </summary>
	  /// <param name="a"> first number. </param>
	  /// <param name="b"> second number. </param>
	  /// <returns> the relative difference. </returns>
	  /// <seealso cref="<a href="http://floating-point-gui.de/errors/comparison/">The Floating-Point Guide</a>"/>
	  /// <seealso cref="<a href="https://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/">"
	  ///      Comparing Floating Point Numbers, 2012 Edition</a>/>
	  public static float relativeDifference(float a, float b)
	  {
		float absDiff = absoluteDifference(a, b);

		if (a.CompareTo(b) == 0)
		{ // Shortcut: handles infinities and NaNs.
		  return 0;
		}
		else if (a == 0 || b == 0 || absDiff < JavaLangCompat.FloatMinNormal)
		{
		  return absDiff / JavaLangCompat.FloatMinNormal;
		}
		else
		{
		  float maxAbsValue = Math.Max(Math.Abs(a), Math.Abs(b));
		  return absDiff / maxAbsValue;
		}
	  }

	  /// <summary>
	  /// Same as <seealso cref="relativeDifference(float, float)"/>, but for doubles. </summary>
	  public static double relativeDifference(double a, double b)
	  {
		double absDiff = absoluteDifference(a, b);

		if (a.CompareTo(b) == 0)
		{ // Shortcut: handles infinities and NaNs.
		  return 0;
		}
		else if (a == 0 || b == 0 || absDiff < JavaLangCompat.DoubleMinNormal)
		{
		  return absDiff / JavaLangCompat.DoubleMinNormal;
		}
		else
		{
		  double maxAbsValue = Math.Max(Math.Abs(a), Math.Abs(b));
		  return absDiff / maxAbsValue;
		}
	  }

	  /// <summary>
	  /// AbsoluteDifference is less than <seealso cref="defaultMaxRelativeDiffFloat"/>. </summary>
	  public static bool nearlyEquals(float a, float b)
	  {
		return nearlyEquals(a, b, defaultMaxRelativeDiffFloat);
	  }

	  /// <summary>
	  /// RelativeDifference is less than maxRelDiff. </summary>
	  public static bool nearlyEquals(float a, float b, float maxRelDiff)
	  {
		return relativeDifference(a, b) < maxRelDiff;
	  }

	  /// <summary>
	  /// AbsoluteDifference is less than <seealso cref="defaultMaxRelativeDiffDouble"/>. </summary>
	  public static bool nearlyEquals(double a, double b)
	  {
		return nearlyEquals(a, b, defaultMaxRelativeDiffDouble);
	  }

	  /// <summary>
	  /// RelativeDifference is less than maxRelDiff. </summary>
	  public static bool nearlyEquals(double a, double b, double maxRelDiff)
	  {
		return relativeDifference(a, b) < maxRelDiff;
	  }

	  /// <summary>
	  /// AbsoluteDifference is less than maxAbsDiff. </summary>
	  public static bool nearlyEqualsAbs(float a, float b, float maxAbsDiff)
	  {
		return absoluteDifference(a, b) <= Math.Abs(maxAbsDiff);
	  }

	  /// <summary>
	  /// AbsoluteDifference is less than maxAbsDiff. </summary>
	  public static bool nearlyEqualsAbs(double a, double b, double maxAbsDiff)
	  {
		return absoluteDifference(a, b) <= Math.Abs(maxAbsDiff);
	  }

	  //////////////////////////////////////////////////////////////////////

	  /// @deprecated use Arrays.toString(arr) 
	  [Obsolete("use Arrays.toString(arr)")]
	  public static string showInts(int[] inta)
	  {
		Formatter f = new Formatter();
		showInts(inta, f);
		return f.ToString();
	  }

	  /// @deprecated use List.toString() 
	  public static string showInts(IList<int> intList)
	  {
		if (intList == null)
		{
		  return "null";
		}
		Formatter f = new Formatter();
		foreach (int i in intList)
		{
		  f.format("%d,", i);
		}
		return f.ToString();
	  }

	  /// @deprecated use Arrays.toString(arr) 
	  public static void showInts(int[] inta, Formatter f)
	  {
		if (inta == null)
		{
		  f.format("null");
		  return;
		}
		foreach (int i in inta)
		{
		  f.format("%d, ", i);
		}
	  }

	  /// @deprecated use Arrays.toString(arr) 
	  public static string showBytes(byte[] buff)
	  {
		StringBuilder sbuff = new StringBuilder();
		for (int i = 0; i < buff.Length; i++)
		{
		  byte b = buff[i];
		  int ub = (b < 0) ? b + 256 : b;
		  if (i > 0)
		  {
			sbuff.Append(" ");
		  }
		  sbuff.Append(ub);
		}
		return sbuff.ToString();
	  }

	  /// @deprecated use Arrays.toString(arr) 
	  public static void showBytes(byte[] buff, Formatter f)
	  {
		foreach (byte b in buff)
		{
		  int ub = (b < 0) ? b + 256 : b;
		  f.format("%3d ", ub);
		}
	  }

	  public static string showBits(byte[] bytes)
	  {
		using (Formatter f = new Formatter())
		{
		  int count = 0;
		  foreach (byte b in bytes)
		  {
			short s = DataTypes.unsignedByteToShort(b);
			f.format("%8s", Convert.ToString(s, 2));
			if (++count % 10 == 0)
			{
			  f.format("%n");
			}
		  }
		  return f.ToString();
		}
	  }

	  //////////////////////////////////////////////////////////////////////

	  public static bool compare(byte[] raw1, byte[] raw2, Formatter f)
	  {
		if (raw1 == null || raw2 == null)
		{
		  return false;
		}

		if (raw1.Length != raw2.Length)
		{
		  f.format("length 1= %3d != length 2=%3d%n", raw1.Length, raw2.Length);
		}
		int len = Math.Min(raw1.Length, raw2.Length);

		int ndiff = 0;
		for (int i = 0; i < len; i++)
		{
		  if (raw1[i] != raw2[i])
		  {
			f.format(" %3d : %3d != %3d%n", i + 1, raw1[i], raw2[i]);
			ndiff++;
		  }
		}
		if (ndiff > 0)
		{
		  f.format("Misc.compare %d bytes, %d are different %n", len, ndiff);
		}
		return ndiff == 0 && (raw1.Length == raw2.Length);
	  }

	  public static bool compare(float[] raw1, float[] raw2, Formatter f)
	  {
		bool ok = true;
		if (raw1.Length != raw2.Length)
		{
		  f.format("compareFloat: length 1= %3d != length 2=%3d%n", raw1.Length, raw2.Length);
		  ok = false;
		}
		int len = Math.Min(raw1.Length, raw2.Length);

		int ndiff = 0;
		for (int i = 0; i < len; i++)
		{
		  if (!Misc.nearlyEquals(raw1[i], raw2[i]) && !double.IsNaN(raw1[i]) && !double.IsNaN(raw2[i]))
		  {
			f.format(" %5d : %3f != %3f%n", i, raw1[i], raw2[i]);
			ndiff++;
			ok = false;
		  }
		}
		if (ndiff > 0)
		{
		  f.format("Misc.compare %d floats, %d are different %n", len, ndiff);
		}
		return ok;
	  }

	  public static bool compare(int[] raw1, int[] raw2, Formatter f)
	  {
		bool ok = true;
		if (raw1.Length != raw2.Length)
		{
		  f.format("compareFloat: length 1= %3d != length 2=%3d%n", raw1.Length, raw2.Length);
		  ok = false;
		}
		int len = Math.Min(raw1.Length, raw2.Length);

		int ndiff = 0;
		for (int i = 0; i < len; i++)
		{
		  if (raw1[i] != raw2[i])
		  {
			f.format(" %5d : %3d != %3d%n", i, raw1[i], raw2[i]);
			ndiff++;
			ok = false;
		  }
		}
		if (ndiff > 0)
		{
		  f.format("Misc.compare %d ints, %d are different %n", len, ndiff);
		}
		return ok;
	  }

	  /// @deprecated use JavaLangCompat.IntCompare(x, y) 
	  [Obsolete("use JavaLangCompat.IntCompare(x, y)")]
	  public static int compare(int x, int y)
	  {
		return JavaLangCompat.IntCompare(x, y);
	  }

	  /// @deprecated use JavaLangCompat.LongCompare(x, y) 
	  [Obsolete("use JavaLangCompat.LongCompare(x, y)")]
	  public static int compare(long x, long y)
	  {
		return JavaLangCompat.LongCompare(x, y);
	  }

	  /// @deprecated use Iterables.size(Iterable it) 
	  [Obsolete("use Iterables.size(Iterable it)")]
	  public static int getSize(System.Collections.IEnumerable ii)
	  {
		if (ii is System.Collections.ICollection)
		{
		  return ((System.Collections.ICollection) ii).Count;
		}
		int count = 0;
		foreach (object i in ii)
		{
		  count++;
		}
		return count;
	  }

	  /// @deprecated use ImmutableList.copyOf(iterator) or Lists.newArrayList(iterator) 
	  [Obsolete("use ImmutableList.copyOf(iterator) or Lists.newArrayList(iterator)")]
	  public static System.Collections.IList getList(System.Collections.IEnumerable ii)
	  {
		if (ii is System.Collections.IList)
		{
		  return (System.Collections.IList) ii;
		}
		System.Collections.IList result = new List<object>();
		foreach (object i in ii)
		{
		  result.Add(i);
		}
		return result;
	  }



	}

}
