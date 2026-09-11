// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Diagnostics;
using System.Linq;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

	/// <summary>
	/// Element by element algebra on Arrays
	/// 
	/// @author caron </summary>
	/// <seealso cref="NcIndex"/>
	public class MAMath
	{

		/// <summary>
		/// Add elements of two arrays together, allocating the result array.
		/// The result type and the operation type are taken from the type of a.
		/// </summary>
		/// <param name="a"> add values from here </param>
		/// <param name="b"> add values from here </param>
		/// <returns> result = a + b </returns>
		/// <exception cref="IllegalArgumentException"> a and b are not conformable </exception>
		/// <exception cref="UnsupportedOperationException"> dont support this data type yet </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static Array add(Array a, Array b) throws IllegalArgumentException
		public static NcArray add(NcArray a, NcArray b)
		{

			NcArray result = NcArray.factory(a.DataType, a.Shape);

			if (a.ElementType == typeof(double))
			{
				addDouble(result, a, b);
			}
			else
			{
				throw new System.NotSupportedException();
			}

			return result;
		}

		/// <summary>
		/// Add elements of two arrays together as doubles, place sum in the result array.
		/// The values from the arrays a and b are converted to double (if needed),
		/// and the sum is converted to the type of result (if needed).
		/// </summary>
		/// <param name="result"> result array </param>
		/// <param name="a"> operand </param>
		/// <param name="b"> operand </param>
		/// <exception cref="IllegalArgumentException"> a,b,and result are not conformable </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void addDouble(Array result, Array a, Array b) throws IllegalArgumentException
		public static void addDouble(NcArray result, NcArray a, NcArray b)
		{

			if (!conformable(result, a) || !conformable(a, b))
			{
				throw new ArgumentException();
			}

			IndexIterator iterR = result.IndexIterator;
			IndexIterator iterA = a.IndexIterator;
			IndexIterator iterB = b.IndexIterator;

			while (iterA.hasNext())
			{
				iterR.DoubleNext = iterA.DoubleNext + iterB.DoubleNext;
			}
		}

		/// <summary>
		/// Check that two arrays are conformable.
		/// </summary>
		/// <param name="a"> operand </param>
		/// <param name="b"> operand </param>
		/// <returns> true if conformable </returns>
		public static bool conformable(NcArray a, NcArray b)
		{
			return conformable(a.Shape, b.Shape);
		}

		/// <summary>
		/// Check that two array shapes are conformable.
		/// The shapes must match exactly, except that dimensions of length 1 are ignored.
		/// </summary>
		/// <param name="shapeA"> shape of array 1 </param>
		/// <param name="shapeB"> shape of array 2 </param>
		/// <returns> true if conformable </returns>
		public static bool conformable(int[] shapeA, int[] shapeB)
		{
			if (reducedRank(shapeA) != reducedRank(shapeB))
			{
				return false;
			}

			int rankB = shapeB.Length;

			int dimB = 0;
			foreach (int aShapeA in shapeA)
			{
				// skip length 1 dimensions
				if (aShapeA == 1)
				{
					continue;
				}
				while (dimB < rankB)
				{
					if (shapeB[dimB] == 1)
					{
						dimB++;
					}
					else
					{
						break;
					}
				}

				// test same shape (NB dimB cant be > rankB due to first test)
				if (aShapeA != shapeB[dimB])
				{
					return false;
				}
				dimB++;
			}

			return true;
		}

		/// <summary>
		/// Convert original array to desired type
		/// </summary>
		/// <param name="org"> original array </param>
		/// <param name="wantType"> desired type </param>
		/// <returns> converted data of desired type, or original array if it is already </returns>
		public static NcArray convert(NcArray org, DataType wantType)
		{
			if (org == null)
			{
				return null;
			}
			Type wantClass = wantType.getPrimitiveClassType();
			if (org.ElementType.Equals(wantClass))
			{
				return org;
			}

			NcArray result = NcArray.factory(wantType, org.Shape);
			copy(wantType, org.IndexIterator, result.IndexIterator);
			return result;
		}

		/// <summary>
		/// Copy using iterators. Will copy until !from.hasNext().
		/// </summary>
		/// <param name="dataType"> use this operation type (eg DataType.DOUBLE uses getDoubleNext()) </param>
		/// <param name="from"> copy from here </param>
		/// <param name="to"> copy to here </param>
		/// <exception cref="IllegalArgumentException"> a and b are not conformable </exception>
		/// <exception cref="UnsupportedOperationException"> dont support this data type </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void copy(DataType dataType, IndexIterator from, IndexIterator to) throws IllegalArgumentException
		public static void copy(DataType dataType, IndexIterator from, IndexIterator to)
		{
			if (dataType == DataType.DOUBLE)
			{
				while (from.hasNext())
				{
					to.DoubleNext = from.DoubleNext;
				}
			}
			else if (dataType == DataType.FLOAT)
			{
				while (from.hasNext())
				{
					to.FloatNext = from.FloatNext;
				}
			}
			else if (dataType.getPrimitiveClassType() == typeof(long))
			{
				while (from.hasNext())
				{
					to.LongNext = from.LongNext;
				}
			}
			else if (dataType.getPrimitiveClassType() == typeof(int))
			{
				while (from.hasNext())
				{
					to.IntNext = from.IntNext;
				}
			}
			else if (dataType.getPrimitiveClassType() == typeof(short))
			{
				while (from.hasNext())
				{
					to.ShortNext = from.ShortNext;
				}
			}
			else if (dataType == DataType.CHAR)
			{
				while (from.hasNext())
				{
					to.CharNext = from.CharNext;
				}
			}
			else if (dataType.getPrimitiveClassType() == typeof(byte))
			{
				while (from.hasNext())
				{
					to.ByteNext = from.ByteNext;
				}
			}
			else if (dataType == DataType.BOOLEAN)
			{
				while (from.hasNext())
				{
					to.BooleanNext = from.BooleanNext;
				}
			}
			else
			{
				while (from.hasNext())
				{
					to.ObjectNext = from.ObjectNext;
				}
			}
		}

		/// <summary>
		/// Copy array a to array result, the result array will be in canonical order
		/// The operation type is taken from the type of a.
		/// </summary>
		/// <param name="result"> copy to here </param>
		/// <param name="a"> copy from here
		/// </param>
		/// <exception cref="IllegalArgumentException"> a and b are not conformable </exception>
		/// <exception cref="UnsupportedOperationException"> dont support this data type yet </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void copy(Array result, Array a) throws IllegalArgumentException
		public static void copy(NcArray result, NcArray a)
		{
			Type classType = a.ElementType;
			if (classType == typeof(double))
			{
				copyDouble(result, a);
			}
			else if (classType == typeof(float))
			{
				copyFloat(result, a);
			}
			else if (classType == typeof(long))
			{
				copyLong(result, a);
			}
			else if (classType == typeof(int))
			{
				copyInt(result, a);
			}
			else if (classType == typeof(short))
			{
				copyShort(result, a);
			}
			else if (classType == typeof(char))
			{
				copyChar(result, a);
			}
			else if (classType == typeof(byte))
			{
				copyByte(result, a);
			}
			else if (classType == typeof(bool))
			{
				copyBoolean(result, a);
			}
			else
			{
				copyObject(result, a);
			}
		}

		/// <summary>
		/// copy array a to array result as doubles
		/// The values from the arrays a are converted to double (if needed),
		/// and then converted to the type of result (if needed).
		/// </summary>
		/// <param name="result"> copy to here </param>
		/// <param name="a"> copy from here
		/// </param>
		/// <exception cref="IllegalArgumentException"> a and result are not conformable </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void copyDouble(Array result, Array a) throws IllegalArgumentException
		public static void copyDouble(NcArray result, NcArray a)
		{
			if (!conformable(a, result))
			{
				throw new ArgumentException("copy arrays are not conformable");
			}

			IndexIterator iterA = a.IndexIterator;
			IndexIterator iterR = result.IndexIterator;
			while (iterA.hasNext())
			{
				iterR.DoubleNext = iterA.DoubleNext;
			}
		}

		/// <summary>
		/// copy array a to array result as floats
		/// The values from the arrays a are converted to float (if needed),
		/// and then converted to the type of result (if needed).
		/// </summary>
		/// <param name="result"> copy to here </param>
		/// <param name="a"> copy from here
		/// </param>
		/// <exception cref="IllegalArgumentException"> a and result are not conformable </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void copyFloat(Array result, Array a) throws IllegalArgumentException
		public static void copyFloat(NcArray result, NcArray a)
		{
			if (!conformable(a, result))
			{
				throw new ArgumentException("copy arrays are not conformable");
			}

			IndexIterator iterA = a.IndexIterator;
			IndexIterator iterR = result.IndexIterator;
			while (iterA.hasNext())
			{
				iterR.FloatNext = iterA.FloatNext;
			}
		}

		/// <summary>
		/// copy array a to array result as longs
		/// The values from the array a are converted to long (if needed),
		/// and then converted to the type of result (if needed).
		/// </summary>
		/// <param name="result"> copy to here </param>
		/// <param name="a"> copy from here
		/// 
		/// </param>
		/// <exception cref="IllegalArgumentException"> a and result are not conformable </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void copyLong(Array result, Array a) throws IllegalArgumentException
		public static void copyLong(NcArray result, NcArray a)
		{
			if (!conformable(a, result))
			{
				throw new ArgumentException("copy arrays are not conformable");
			}

			IndexIterator iterA = a.IndexIterator;
			IndexIterator iterR = result.IndexIterator;
			while (iterA.hasNext())
			{
				iterR.LongNext = iterA.LongNext;
			}
		}

		/// <summary>
		/// copy array a to array result as integers
		/// The values from the arrays a are converted to integer (if needed),
		/// and then converted to the type of result (if needed).
		/// </summary>
		/// <param name="result"> copy to here </param>
		/// <param name="a"> copy from here
		/// </param>
		/// <exception cref="IllegalArgumentException"> a and result are not conformable </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void copyInt(Array result, Array a) throws IllegalArgumentException
		public static void copyInt(NcArray result, NcArray a)
		{
			if (!conformable(a, result))
			{
				throw new ArgumentException("copy arrays are not conformable");
			}

			IndexIterator iterA = a.IndexIterator;
			IndexIterator iterR = result.IndexIterator;
			while (iterA.hasNext())
			{
				iterR.IntNext = iterA.IntNext;
			}
		}

		/// <summary>
		/// copy array a to array result as shorts
		/// The values from the array a are converted to short (if needed),
		/// and then converted to the type of result (if needed).
		/// </summary>
		/// <param name="result"> copy to here </param>
		/// <param name="a"> copy from here
		/// </param>
		/// <exception cref="IllegalArgumentException"> a and result are not conformable </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void copyShort(Array result, Array a) throws IllegalArgumentException
		public static void copyShort(NcArray result, NcArray a)
		{
			if (!conformable(a, result))
			{
				throw new ArgumentException("copy arrays are not conformable");
			}

			IndexIterator iterA = a.IndexIterator;
			IndexIterator iterR = result.IndexIterator;
			while (iterA.hasNext())
			{
				iterR.ShortNext = iterA.ShortNext;
			}
		}

		/// <summary>
		/// copy array a to array result as char
		/// The values from the array a are converted to char (if needed),
		/// and then converted to the type of result (if needed).
		/// </summary>
		/// <param name="result"> copy to here </param>
		/// <param name="a"> copy from here
		/// </param>
		/// <exception cref="IllegalArgumentException"> a and result are not conformable </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void copyChar(Array result, Array a) throws IllegalArgumentException
		public static void copyChar(NcArray result, NcArray a)
		{
			if (!conformable(a, result))
			{
				throw new ArgumentException("copy arrays are not conformable");
			}

			IndexIterator iterA = a.IndexIterator;
			IndexIterator iterR = result.IndexIterator;
			while (iterA.hasNext())
			{
				iterR.CharNext = iterA.CharNext;
			}
		}


		/// <summary>
		/// copy array a to array result as bytes
		/// The values from the array a are converted to byte (if needed),
		/// and then converted to the type of result (if needed).
		/// </summary>
		/// <param name="result"> copy to here </param>
		/// <param name="a"> copy from here
		/// </param>
		/// <exception cref="IllegalArgumentException"> a and result are not conformable </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void copyByte(Array result, Array a) throws IllegalArgumentException
		public static void copyByte(NcArray result, NcArray a)
		{
			if (!conformable(a, result))
			{
				throw new ArgumentException("copy arrays are not conformable");
			}

			IndexIterator iterA = a.IndexIterator;
			IndexIterator iterR = result.IndexIterator;
			while (iterA.hasNext())
			{
				iterR.ByteNext = iterA.ByteNext;
			}
		}

		/// <summary>
		/// copy array a to array result as bytes
		/// The array a and result must be type boolean
		/// </summary>
		/// <param name="result"> copy to here </param>
		/// <param name="a"> copy from here
		/// </param>
		/// <exception cref="IllegalArgumentException"> a and result are not conformable </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void copyBoolean(Array result, Array a) throws IllegalArgumentException
		public static void copyBoolean(NcArray result, NcArray a)
		{
			if (!conformable(a, result))
			{
				throw new ArgumentException("copy arrays are not conformable");
			}

			IndexIterator iterA = a.IndexIterator;
			IndexIterator iterR = result.IndexIterator;
			while (iterA.hasNext())
			{
				iterR.BooleanNext = iterA.BooleanNext;
			}
		}

		/// <summary>
		/// copy array a to array result as an Object
		/// The array a and result must be type object
		/// </summary>
		/// <param name="result"> copy to here </param>
		/// <param name="a"> copy from here
		/// </param>
		/// <exception cref="IllegalArgumentException"> a and result are not conformable </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static void copyObject(Array result, Array a) throws IllegalArgumentException
		public static void copyObject(NcArray result, NcArray a)
		{
			if (!conformable(a, result))
			{
				throw new ArgumentException("copy arrays are not conformable");
			}

			IndexIterator iterA = a.IndexIterator;
			IndexIterator iterR = result.IndexIterator;
			while (iterA.hasNext())
			{
				iterR.ObjectNext = iterA.ObjectNext;
			}
		}

		/// <summary>
		/// Calculate the reduced rank of this shape, by subtracting dimensions with length 1
		/// </summary>
		/// <param name="shape"> shape of the array </param>
		/// <returns> rank without dimensions of length 1 </returns>
		public static int reducedRank(int[] shape)
		{
			int rank = 0;
			foreach (int aShape in shape)
			{
				if (aShape > 1)
				{
					rank++;
				}
			}
			return rank;
		}

		public static double getMinimum(NcArray a)
		{
			IndexIterator iter = a.IndexIterator;
			double min = double.MaxValue;
			while (iter.hasNext())
			{
				double val = iter.DoubleNext;
				if (double.IsNaN(val))
				{
					continue;
				}
				if (val < min)
				{
					min = val;
				}
			}
			return min;
		}

		public static double getMaximum(NcArray a)
		{
			IndexIterator iter = a.IndexIterator;
			double max = -double.MaxValue;
			while (iter.hasNext())
			{
				double val = iter.DoubleNext;
				if (double.IsNaN(val))
				{
					continue;
				}
				if (val > max)
				{
					max = val;
				}
			}
			return max;
		}

		/// <summary>
		/// Find min and max value in this array, getting values as doubles. Skip double.NaN.
		/// </summary>
		/// <param name="a"> the array. </param>
		/// <returns> MinMax </returns>
		public static MAMath.MinMax getMinMax(NcArray a)
		{
			IndexIterator iter = a.IndexIterator;
			double max = -double.MaxValue;
			double min = double.MaxValue;
			while (iter.hasNext())
			{
				double val = iter.DoubleNext;
				if (double.IsNaN(val))
				{
					continue;
				}
				if (val > max)
				{
					max = val;
				}
				if (val < min)
				{
					min = val;
				}
			}
			return new MinMax(min, max);
		}

		public static MAMath.MinMax getMinMaxSkipMissingData(NcArray a, IsMissingEvaluator eval)
		{
			if (eval == null || !eval.hasMissing())
			{
				return MAMath.getMinMax(a);
			}

			IndexIterator iter = a.IndexIterator;
			double max = -double.MaxValue;
			double min = double.MaxValue;
			while (iter.hasNext())
			{
				double val = iter.DoubleNext;
				if (eval.isMissing(val))
				{
					continue;
				}
				if (val > max)
				{
					max = val;
				}
				if (val < min)
				{
					min = val;
				}
			}
			return new MAMath.MinMax(min, max);
		}


		public static double getMinimumSkipMissingData(NcArray a, double missingValue)
		{
			IndexIterator iter = a.IndexIterator;
			double min = double.MaxValue;
			while (iter.hasNext())
			{
				double val = iter.DoubleNext;
				if ((val != missingValue) && (val < min))
				{
					min = val;
				}
			}
			return min;
		}

		public static double getMaximumSkipMissingData(NcArray a, double missingValue)
		{
			IndexIterator iter = a.IndexIterator;
			double max = -double.MaxValue;
			while (iter.hasNext())
			{
				double val = iter.DoubleNext;
				if ((val != missingValue) && (val > max))
				{
					max = val;
				}
			}
			return max;
		}

		public static MAMath.MinMax getMinMaxSkipMissingData(NcArray a, double missingValue)
		{
			IndexIterator iter = a.IndexIterator;
			double max = -double.MaxValue;
			double min = double.MaxValue;
			while (iter.hasNext())
			{
				double val = iter.DoubleNext;
				if (val == missingValue)
				{
					continue;
				}
				if (val > max)
				{
					max = val;
				}
				if (val < min)
				{
					min = val;
				}
			}
			return new MinMax(min, max);
		}


		/// <summary>
		/// Set all the elements of this array to the given double value.
		/// The value is converted to the element type of the array, if needed.
		/// </summary>
		/// <param name="result"> change this Array </param>
		/// <param name="val"> set all elements to this value </param>
		public static void setDouble(NcArray result, double val)
		{
			IndexIterator iter = result.IndexIterator;
			while (iter.hasNext())
			{
				iter.DoubleNext = val;
			}
		}

		/// <summary>
		/// sum all of the elements of array a as doubles.
		/// The values from the array a are converted to double (if needed).
		/// </summary>
		/// <param name="a"> read values from this Array </param>
		/// <returns> sum of elements </returns>
		public static double sumDouble(NcArray a)
		{
			double sum = 0;
			IndexIterator iterA = a.IndexIterator;
			while (iterA.hasNext())
			{
				sum += iterA.DoubleNext;
			}
			return sum;
		}

		/// <summary>
		/// sum all of the elements of array a as doubles.
		/// The values from the array a are converted to double (if needed).
		/// </summary>
		/// <param name="a"> read values from this Array </param>
		/// <param name="missingValue"> skip values equal to this, or which are NaNs </param>
		/// <returns> sum of elements </returns>
		public static double sumDoubleSkipMissingData(NcArray a, double missingValue)
		{
			double sum = 0;
			IndexIterator iterA = a.IndexIterator;
			while (iterA.hasNext())
			{
				double val = iterA.DoubleNext;
				if ((val == missingValue) || double.IsNaN(val))
				{
					continue;
				}
				sum += val;
			}
			return sum;
		}

		/// <summary>
		/// Holds a minimum and maximum value.
		/// </summary>
		public class MinMax
		{
			public double min, max;

			public MinMax(double min, double max)
			{
				this.min = min;
				this.max = max;
			}

			public override string ToString()
			{
				return "MinMax{" + "min=" + min + ", max=" + max + '}';
			}
		}

		/// <summary>
		/// Calculate the scale/offset for an array of numbers.
		/// 
		/// <para>
		/// If signed:
		///   then
		///     max value unpacked = 2^(n-1) - 1 packed
		///     min value unpacked = -(2^(n-1) - 1) packed
		///   note that -2^(n-1) is unused, and a good place to map missing values
		///   by solving 2 eq in 2 unknowns, we get:
		///     scale = (max - min) / (2^n - 2)
		///     offset = (max + min) / 2
		/// If unsigned then
		///     max value unpacked = 2^n - 1 packed
		///     min value unpacked = 0 packed
		///   and:
		///     scale = (max - min) / (2^n - 1)
		///     offset = min
		///   One could modify this to allow a holder for missing values.
		/// </para>
		/// </summary>
		/// <param name="a"> array to convert (not changed) </param>
		/// <param name="missingValue"> skip these </param>
		/// <param name="nbits"> map into this many bits </param>
		/// <returns> ScaleOffset, calculated as above. </returns>
		public static MAMath.ScaleOffset calcScaleOffsetSkipMissingData(NcArray a, double missingValue, int nbits)
		{
			MAMath.MinMax minmax = getMinMaxSkipMissingData(a, missingValue);

			if (a.isUnsigned())
			{
				long size = (1L << nbits) - 1;
				double offset = minmax.min;
				double scale = (minmax.max - minmax.min) / size;
				return new ScaleOffset(scale, offset);
			}
			else
			{
				long size = (1L << nbits) - 2;
				double offset = (minmax.max + minmax.min) / 2;
				double scale = (minmax.max - minmax.min) / size;
				return new ScaleOffset(scale, offset);
			}
		}

		public static NcArray convert2packed(NcArray unpacked, double missingValue, int nbits, DataType packedType)
		{
			MAMath.ScaleOffset scaleOffset = calcScaleOffsetSkipMissingData(unpacked, missingValue, nbits);
			NcArray result = NcArray.factory(packedType, unpacked.Shape);
			IndexIterator riter = result.IndexIterator;
			while (unpacked.hasNext())
			{
				double uv = unpacked.nextDouble();
				double pv = (uv - scaleOffset.offset) / scaleOffset.scale;
				riter.DoubleNext = pv;
			}
			return result;
		}

		public static NcArray convert2Unpacked(NcArray packed, ScaleOffset scaleOffset)
		{
			NcArray result = NcArray.factory(DataType.DOUBLE, packed.Shape);
			IndexIterator riter = result.IndexIterator;
			while (packed.hasNext())
			{
				riter.DoubleNext = packed.nextDouble() * scaleOffset.scale + scaleOffset.offset;
			}
			return result;
		}

		/// <summary>
		/// Holds a scale and offset.
		/// </summary>
		public class ScaleOffset
		{
			public double scale, offset;

			public ScaleOffset(double scale, double offset)
			{
				this.scale = scale;
				this.offset = offset;
			}
		}

		/// <summary>
		/// Returns true if the specified arrays have the same size, signedness, and <b>approximately</b> equal corresponding
		/// elements. float elements must be within Misc.defaultMaxRelativeDiffFloat of each other, as
		/// determined by Misc.nearlyEquals(double, double, double). Similarly, double elements must be within
		/// Misc.defaultMaxRelativeDiffDouble of each other.
		/// <para>
		/// equals(Array, Array) is an alternative to this method that requires that corresponding elements be
		/// exactly equal. It is suitable for use in Object.Equals implementations, whereas this method isnt.
		/// </para>
		/// </summary>
		/// <param name="data1"> one array to be tested for equality. </param>
		/// <param name="data2"> the other array to be tested for equality. </param>
		/// <returns> true if the specified arrays have the same size, signedness, and approximately equal corresponding elems. </returns>
		public static bool nearlyEquals(NcArray data1, NcArray data2)
		{
			if (data1 == data2) // Covers case when both are null.
			{
				return true;
			}
			else if (data1 == null || data2 == null)
			{
				return false;
			}

			if (data1.Size != data2.Size)
			{
				return false;
			}
			if (data1.isUnsigned() != data2.isUnsigned())
			{
				return false;
			}
			DataType dt = DataTypes.getType(data1);

			IndexIterator iter1 = data1.IndexIterator;
			IndexIterator iter2 = data2.IndexIterator;

			if (dt == DataType.DOUBLE)
			{
				while (iter1.hasNext() && iter2.hasNext())
				{
					double v1 = iter1.DoubleNext;
					double v2 = iter2.DoubleNext;
					if (!Misc.nearlyEquals(v1, v2))
					{
						return false;
					}
				}
			}
			else if (dt == DataType.FLOAT)
			{
				while (iter1.hasNext() && iter2.hasNext())
				{
					float v1 = iter1.FloatNext;
					float v2 = iter2.FloatNext;
					if (!Misc.nearlyEquals(v1, v2))
					{
						return false;
					}
				}
			}
			else if (dt.getPrimitiveClassType() == typeof(int))
			{
				while (iter1.hasNext() && iter2.hasNext())
				{
					int v1 = iter1.IntNext;
					int v2 = iter2.IntNext;
					if (v1 != v2)
					{
						return false;
					}
				}
			}
			else if (dt.getPrimitiveClassType() == typeof(byte))
			{
				while (iter1.hasNext() && iter2.hasNext())
				{
					short v1 = iter1.ShortNext;
					short v2 = iter2.ShortNext;
					if (v1 != v2)
					{
						return false;
					}
				}
			}
			else if (dt.getPrimitiveClassType() == typeof(short))
			{
				while (iter1.hasNext() && iter2.hasNext())
				{
					byte v1 = iter1.ByteNext;
					byte v2 = iter2.ByteNext;
					if (v1 != v2)
					{
						return false;
					}
				}
			}
			else if (dt.getPrimitiveClassType() == typeof(long))
			{
				while (iter1.hasNext() && iter2.hasNext())
				{
					long v1 = iter1.LongNext;
					long v2 = iter2.LongNext;
					if (v1 != v2)
					{
						return false;
					}
				}
			}
			else
			{
				while (iter1.hasNext() && iter2.hasNext())
				{
					if (!object.Equals(iter1.next(), iter2.next()))
					{
						return false;
					}
				}
			}

			return true;
		}


		/// <summary>
		/// Returns true if the specified arrays have the same data type, shape, and equal corresponding elements. This method
		/// is suitable for use in Object.Equals implementations.
		/// <para>
		/// Note that floating-point elements must be exactly equal, not merely within some epsilon of each other. This is
		/// because its impossible to write a strictly-conforming Object.Equals implementation when an
		/// epsilon is incorporated, due to the transitivity requirement.
		/// <para>
		/// nearlyEquals is an alternative to this method that returns true if the corresponding elements are
		/// "approximately" equal to each other.
		/// </para>
		/// </summary>
		/// <param name="array1"> one array to be tested for equality. </param>
		/// <param name="array2"> the other array to be tested for equality. </param>
		/// <returns> true if the specified arrays have the same data type, shape, and equal corresponding elements. </returns>
		public static bool equals(NcArray array1, NcArray array2)
		{
			if (array1 == array2) // Covers case when both are null.
			{
				return true;
			}
			else if (array1 == null || array2 == null)
			{
				return false;
			}

			if (array1.DataType != array2.DataType)
			{
				return false;
			}

			if (!array1.Shape.SequenceEqual(array2.Shape))
			{
				return false;
			}

			IndexIterator iter1 = array1.IndexIterator;
			IndexIterator iter2 = array2.IndexIterator;

			while (iter1.hasNext() && iter2.hasNext())
			{
				if (!object.Equals(iter1.next(), iter2.next()))
				{
					return false;
				}
				if (!iter1.CurrentCounter.SequenceEqual(iter2.CurrentCounter))
				{
					return false;
				}
			}

			Debug.Assert(!iter1.hasNext() && !iter2.hasNext()); // Iterators ought to be the same length.
			return true;
		}

		/// <summary>
		/// An implementation of hash code that is consistent with equals(Array, Array).
		/// </summary>
		/// <param name="array"> an array to hash. </param>
		/// <returns> a hash code value for the array. </returns>
		public static int hashCode(NcArray array)
		{
			if (array == null)
			{
				return 0;
			}

			int hash = 3;
			hash = 29 * hash + array.DataType.GetHashCode();
			hash = 29 * hash + hashCode(array.Shape);

			for (IndexIterator iter = array.IndexIterator; iter.hasNext();)
			{
				hash = 29 * hash + iter.next().GetHashCode();
				hash = 29 * hash + hashCode(iter.CurrentCounter);
			}

			return hash;
		}

		private static int hashCode(int[] a)
		{
			if (a == null)
			{
				return 0;
			}
			int result = 1;
			foreach (int element in a)
			{
				result = 31 * result + element;
			}
			return result;
		}

	}
}
