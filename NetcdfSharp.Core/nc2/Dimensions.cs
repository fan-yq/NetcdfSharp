// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/* Copyright Unidata */
namespace NetcdfSharp.ucar.nc2
{
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using NcRange = NetcdfSharp.ucar.ma2.NcRange;
	using Section = NetcdfSharp.ucar.ma2.Section;

	/// <summary>
	/// Static helper methods for Dimension.
	/// 
	/// @author caron
	/// @since 10/3/2019.
	/// </summary>
	public class Dimensions
	{

		private Dimensions()
		{
		}

		/// <summary>
		/// A Function that finds a Dimension by name. </summary>
		public interface FindDimensionByName
		{
			// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
			// ORIGINAL LINE: @Nullable Dimension findByName(String dimName);
			Dimension? findByName(string dimName);
		}

		/// <summary>
		/// Make a NetcdfSharp.ucar.ma2.Section.Builder from an ordered set of Dimension objects. </summary>
		public static Section.Builder makeSectionFromDimensions(IEnumerable<Dimension> dimensions)
		{
			try
			{
				Section.Builder builder = Section.builder();
				foreach (Dimension d in dimensions)
				{
					int len = d.Length;
					if (len > 0)
					{
						builder.appendRange(new NcRange(d.ShortName, 0, len - 1));
					}
					else if (len == 0)
					{
						builder.appendRange(NcRange.EMPTY); // LOOK empty not named
					}
					else
					{
						Debug.Assert(d.VariableLength);
						builder.appendRange(NcRange.VLEN); // LOOK vlen not named
					}
				}
				return builder;

			}
			catch (InvalidRangeException e)
			{
				throw new System.InvalidOperationException(e.Message);
			}
		}

		/// <summary>
		/// Get the total number of elements the dimensions represent. </summary>
		public static long getSize(IEnumerable<Dimension> dimensions)
		{
			long size = 1;
			foreach (Dimension dim in dimensions)
			{
				if (dim.Length >= 0)
				{
					size *= dim.Length;
				}
			}
			return size;
		}

		/// <summary>
		/// Make an array of Dimension lengths. </summary>
		public static int[] makeShape(IEnumerable<Dimension> dimensions)
		{
			return makeSectionFromDimensions(dimensions).build().Shape;
		}

		/// <summary>
		/// Make a space-delineated String from a list of Dimension names, inverse of makeDimensionsList(). </summary>
		public static string makeDimensionsString(IList<Dimension> dimensions)
		{
			if (dimensions == null)
			{
				return "";
			}

			Formatter buf = new Formatter();
			for (int i = 0; i < dimensions.Count; i++)
			{
				Dimension myd = dimensions[i];
				string dimName = myd.ShortName;

				if (i != 0)
				{
					buf.format(" ");
				}

				if (myd.VariableLength)
				{
					buf.format("*");
				}
				else if (myd.Shared)
				{
					buf.format("%s", dimName);
				}
				else
				{
					// if (dimName != null) // LOOK losing anon dim name
					// buf.format("%s=", dimName);
					buf.format("%d", myd.Length);
				}
			}
			return buf.ToString();
		}

		/// <summary>
		/// Make a list of Dimensions from a list of names.
		/// </summary>
		/// <param name="finder"> interface to find a Dimension by name. </param>
		/// <param name="dimString"> space separated list of dimension names. </param>
		/// <returns> equivalent list of Dimension objects. </returns>
		/// <exception cref="IllegalArgumentException"> if cant find or parse the name. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static NetcdfSharp.com.google.common.collect.ImmutableList<Dimension> makeDimensionsList(FindDimensionByName finder, String dimString) throws IllegalArgumentException
		public static ImmutableList<Dimension> makeDimensionsList(Func<string, Dimension> finder, string dimString)
		{
			if (string.ReferenceEquals(dimString, null)) // scalar
			{
				return ImmutableList<Dimension>.of(); // empty list
			}
			dimString = dimString.Trim();
			if (dimString.Length == 0) // scalar
			{
				return ImmutableList<Dimension>.of(); // empty list
			}

			ImmutableList<Dimension>.Builder newDimensions = ImmutableList<Dimension>.builder();
			StringTokenizer stoke = new StringTokenizer(dimString);
			while (stoke.hasMoreTokens())
			{
				string dimName = stoke.nextToken();
				Dimension d;
				if (dimName.Equals("*"))
				{
					d = Dimension.VLEN;
				}
				else
				{
					d = finder(dimName);
				}

				if (d == null)
				{
					// if numeric - then its anonymous dimension
					try
					{
						int len = int.Parse(dimName);
						d = Dimension.builder().setLength(len).setIsShared(false).build();
					}
					catch (Exception)
					{
						throw new ArgumentException("Dimension " + dimName + " does not exist");
					}
				}
				newDimensions.add(d);
			}

			return newDimensions.build();
		}

		/// <summary>
		/// Make a list of private dimensions from an array of lengths </summary>
		public static ImmutableList<Dimension> makeDimensionsAnon(int[] shape)
		{
			if ((shape == null) || (shape.Length == 0))
			{ // scalar
				return ImmutableList<Dimension>.of();
			}
			ImmutableList<Dimension>.Builder newDimensions = ImmutableList<Dimension>.builder();
			foreach (int len in shape)
			{
				newDimensions.add(Dimension.builder().setIsVariableLength(len == -1).setLength(len).setIsShared(false).build());
			}
			return newDimensions.build();
		}

		/// <summary>
		/// Get list of Dimensions, including parent Structure(s), if any.
		/// </summary>
		/// <returns> array of Dimension, rank of v plus all parent Structures. </returns>
		public static ImmutableList<Dimension> makeDimensionsAll(Variable v)
		{
			ImmutableList<Dimension>.Builder dimsAll = ImmutableList<Dimension>.builder();
			addDimensionsAll(dimsAll, v);
			return dimsAll.build();
		}

		private static void addDimensionsAll(ImmutableList<Dimension>.Builder result, Variable v)
		{
			if (v.MemberOfStructure)
			{
				addDimensionsAll(result, v.ParentStructure);
			}

			for (int i = 0; i < v.Rank; i++)
			{
				result.add(v.getDimension(i));
			}
		}
	}

}
