// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

namespace NetcdfSharp.ucar.nc2.filter
{
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using IndexIterator = NetcdfSharp.ucar.ma2.IndexIterator;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using NcAttribute = NetcdfSharp.ucar.nc2.NcAttribute;
	using Signedness = NetcdfSharp.ucar.ma2.Signedness;

	// JAVA TO C# CONVERTER TASK: This Java 'import static' statement cannot be converted to C#:
	//	import static NetcdfSharp.ucar.ma2.DataType.*;

	public class FilterHelpers
	{

		public static NcArray bytesToArray(byte[] @in, DataType type, ByteOrder order)
		{
			return NcArray.factory(type, new int[] { @in.Length / type.getSize() }, convertToType(@in, type, order));
		}

		public static byte[] arrayToBytes(NcArray arr, DataType type, ByteOrder order)
		{
			ByteBuffer bb = ByteBuffer.allocate((int)arr.Size * type.getSize());
			bb.order(order);

			IndexIterator ii = arr.IndexIterator;
			while (ii.hasNext())
			{
				switch (type)
				{
					case DataType.BYTE:
					case DataType.UBYTE:
						bb.put(ii.ByteNext);
						break;
					case DataType.SHORT:
					case DataType.USHORT:
						bb.putShort(ii.ShortNext);
						break;
					case DataType.INT:
					case DataType.UINT:
						bb.putInt(ii.IntNext);
						break;
					case DataType.LONG:
					case DataType.ULONG:
						bb.putLong(ii.LongNext);
						break;
					case DataType.FLOAT:
						bb.putFloat(ii.FloatNext);
						break;
					case DataType.DOUBLE:
						bb.putDouble(ii.DoubleNext);
						break;
				}
			}
			return bb.array();
		}

		private static object convertToType(byte[] dataIn, DataType wantType, ByteOrder bo)
		{
			if (wantType.getSize() == 1)
			{
				return dataIn;
			} // no need for conversion

			ByteBuffer bb = ByteBuffer.wrap(dataIn);
			bb.order(bo);
			switch (wantType)
			{
				case DataType.SHORT:
				case DataType.USHORT:
					ShortBuffer sb = bb.asShortBuffer();
					short[] shortArray = new short[sb.limit()];
					sb.get(shortArray);
					return shortArray;
				case DataType.INT:
				case DataType.UINT:
					IntBuffer ib = bb.asIntBuffer();
					int[] intArray = new int[ib.limit()];
					ib.get(intArray);
					return intArray;
				case DataType.LONG:
				case DataType.ULONG:
					LongBuffer lb = bb.asLongBuffer();
					long[] longArray = new long[lb.limit()];
					lb.get(longArray);
					return longArray;
				case DataType.FLOAT:
					FloatBuffer fb = bb.asFloatBuffer();
					float[] floatArray = new float[fb.limit()];
					fb.get(floatArray);
					return floatArray;
				case DataType.DOUBLE:
					DoubleBuffer db = bb.asDoubleBuffer();
					double[] doubleArray = new double[db.limit()];
					db.get(doubleArray);
					return doubleArray;
				default:
					return bb.array();
			}
		}

		/// <summary>
		/// Returns the smallest numeric data type that:
		/// <ol>
		/// <li>can hold a larger integer than {@code dataType} can</li>
		/// <li>if integral, has the same signedness as {@code dataType}</li>
		/// </ol>
		/// <p/>
		/// <table border="1">
		/// <tr>
		/// <th>Argument</th>
		/// <th>Result</th>
		/// </tr>
		/// <tr>
		/// <td>DataType.BYTE</td>
		/// <td>DataType.SHORT</td>
		/// </tr>
		/// <tr>
		/// <td>DataType.UBYTE</td>
		/// <td>DataType.USHORT</td>
		/// </tr>
		/// <tr>
		/// <td>DataType.SHORT</td>
		/// <td>DataType.INT</td>
		/// </tr>
		/// <tr>
		/// <td>DataType.USHORT</td>
		/// <td>DataType.UINT</td>
		/// </tr>
		/// <tr>
		/// <td>DataType.INT</td>
		/// <td>DataType.LONG</td>
		/// </tr>
		/// <tr>
		/// <td>DataType.UINT</td>
		/// <td>DataType.ULONG</td>
		/// </tr>
		/// <tr>
		/// <td>DataType.LONG</td>
		/// <td>DataType.DOUBLE</td>
		/// </tr>
		/// <tr>
		/// <td>DataType.ULONG</td>
		/// <td>DataType.DOUBLE</td>
		/// </tr>
		/// <tr>
		/// <td>Any other data type</td>
		/// <td>Just return argument</td>
		/// </tr>
		/// </table>
		/// <p/>
		/// The returned type is intended to be just big enough to hold the result of performing an unsigned conversion of a
		/// value of the smaller type. For example, the {@code byte} value {@code -106} equals {@code 150} when interpreted
		/// as unsigned. That won't fit in a (signed) {@code byte}, but it will fit in a {@code short}.
		/// </summary>
		/// <param name="dataType"> an integral data type. </param>
		/// <returns> the next larger type. </returns>
		public static DataType nextLarger(DataType dataType)
		{
			switch (dataType)
			{
				case DataType.BYTE:
					return DataType.SHORT;
				case DataType.UBYTE:
					return DataType.USHORT;
				case DataType.SHORT:
					return DataType.INT;
				case DataType.USHORT:
					return DataType.UINT;
				case DataType.INT:
					return DataType.LONG;
				case DataType.UINT:
					return DataType.ULONG;
				case DataType.LONG:
				case DataType.ULONG:
					return DataType.DOUBLE;
				default:
					return dataType;
			}
		}

		// Get the data type of an attribute. Make it unsigned if the variable is unsigned.
		public static DataType getAttributeDataType(NcAttribute attribute, Signedness signedness)
		{
			DataType dataType = attribute.DataType;
			if (signedness == Signedness.UNSIGNED)
			{
				// If variable is unsigned, make its integral attributes unsigned too.
				dataType = dataType.withSignedness(signedness);
			}
			return dataType;
		}


		public static int rank(DataType? dataType)
		{
			if (dataType == null)
			{
				return -1;
			}

			switch (dataType)
			{
				case DataType.BYTE:
					return 0;
				case DataType.UBYTE:
					return 1;
				case DataType.SHORT:
					return 2;
				case DataType.USHORT:
					return 3;
				case DataType.INT:
					return 4;
				case DataType.UINT:
					return 5;
				case DataType.LONG:
					return 6;
				case DataType.ULONG:
					return 7;
				case DataType.FLOAT:
					return 8;
				case DataType.DOUBLE:
					return 9;
				default:
					return -1;
			}
		}

		public static DataType largestOf(params DataType?[] dataTypes)
		{
			DataType? widest = null;
			foreach (DataType? dataType in dataTypes)
			{
				if (dataType == null)
				{
					continue;
				}
				if (widest == null)
				{
					widest = dataType;
				}
				else if (rank(dataType.Value) > rank(widest.Value))
				{
					widest = dataType;
				}
			}
			return widest == null ? default(DataType) : widest.Value;
		}
	}

}
