// ====================================================================================================
// NetcdfSharp.Core - NetcdfSharp.ucar.ma2.DataType
// Rewritten from Java rich-enum to C# enum + extension metadata (Plan 11.3).
// Method names follow the Java originals (getSize, isUnsigned, withSignedness, ...) so that
// translated call sites compile unchanged.
// ====================================================================================================

using System;
using System.Numerics;

namespace NetcdfSharp.ucar.ma2
{
	/// <summary>
	/// The signedness of an integral data type.
	/// </summary>
	public enum Signedness
	{
		/// <summary>
		/// The data type can represent both positive and negative numbers. </summary>
		SIGNED,
		/// <summary>
		/// The data type can represent only non-negative numbers. </summary>
		UNSIGNED
	}

	/// <summary>
	/// Type-safe enumeration of data types.
	/// Do not change the ordering of these enums, as they are used in protobuf messages, only add new ones onto the end.
	/// </summary>
	public enum DataType
	{
		BOOLEAN, BYTE, CHAR, SHORT, INT, LONG, FLOAT, DOUBLE,

		// object types
		SEQUENCE, STRING, STRUCTURE,

		ENUM1, ENUM2, ENUM4,

		OPAQUE,

		// TODO deprecate and remove in v6?
		OBJECT,

		UBYTE, USHORT, UINT, ULONG
	}

	/// <summary>
	/// Metadata and helpers for <see cref="DataType"/>.
	/// </summary>
	public static class DataTypes
	{
		private static readonly string[] NICE_NAMES = {
			"boolean", "byte", "char", "short", "int", "long", "float", "double",
			"Sequence", "String", "Structure",
			"enum1", "enum2", "enum4",
			"opaque",
			"object",
			"ubyte", "ushort", "uint", "ulong"
		};

		private static readonly int[] SIZES = {
			1, 1, 1, 2, 4, 8, 4, 8,
			4, 4, 0,
			1, 2, 4,
			1,
			1,
			1, 2, 4, 8
		};

		private static readonly Type[] PRIMITIVE_CLASSES = {
			typeof(bool), typeof(byte), typeof(char), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double),
			typeof(StructureDataIterator), typeof(string), typeof(StructureData),
			typeof(byte), typeof(short), typeof(int),
			typeof(ByteBuffer),
			typeof(object),
			typeof(byte), typeof(short), typeof(int), typeof(long)
		};

		private static readonly Signedness[] SIGNEDNESS = {
			Signedness.SIGNED, Signedness.SIGNED, Signedness.SIGNED, Signedness.SIGNED, Signedness.SIGNED, Signedness.SIGNED, Signedness.SIGNED, Signedness.SIGNED,
			Signedness.SIGNED, Signedness.SIGNED, Signedness.SIGNED,
			Signedness.SIGNED, Signedness.SIGNED, Signedness.SIGNED,
			Signedness.SIGNED,
			Signedness.SIGNED,
			Signedness.UNSIGNED, Signedness.UNSIGNED, Signedness.UNSIGNED, Signedness.UNSIGNED
		};

		/// <summary>
		/// The DataType name, eg "byte", "float", "String".
		/// </summary>
		public static string ToString(this DataType dt)
		{
			return NICE_NAMES[(int)dt];
		}

		/// <summary>
		/// Size in bytes of one element of this data type.
		/// Strings dont know, so return 0. Structures return 1.
		/// </summary>
		public static int getSize(this DataType dt)
		{
			return SIZES[(int)dt];
		}

		/// <summary>
		/// The primitive class type: char, byte, float, double, short, int, long, boolean, String, StructureData,
		/// StructureDataIterator, ByteBuffer.
		/// </summary>
		public static Type getPrimitiveClassType(this DataType dt)
		{
			return PRIMITIVE_CLASSES[(int)dt];
		}

		/// <summary>
		/// The signedness of this data type.
		/// For non-integral data types, guaranteed <see cref="Signedness.SIGNED"/>.
		/// </summary>
		public static Signedness getSignedness(this DataType dt)
		{
			return SIGNEDNESS[(int)dt];
		}

		/// <summary>
		/// true if this data type is unsigned.
		/// For non-integral data types, guaranteed false.
		/// </summary>
		public static bool isUnsigned(this DataType dt)
		{
			return dt.getSignedness() == Signedness.UNSIGNED;
		}

		/// <summary>
		/// true if String or Char.
		/// </summary>
		public static bool isString(this DataType dt)
		{
			return (dt == DataType.STRING) || (dt == DataType.CHAR);
		}

		/// <summary>
		/// true if numeric (Byte, Float, Double, Int, Short, Long).
		/// </summary>
		public static bool isNumeric(this DataType dt)
		{
			return (dt == DataType.FLOAT) || (dt == DataType.DOUBLE) || dt.isIntegral();
		}

		/// <summary>
		/// true if integral (Byte, Int, Short, Long, UByte, UInt, UShort, ULong).
		/// </summary>
		public static bool isIntegral(this DataType dt)
		{
			return (dt == DataType.BYTE) || (dt == DataType.INT) || (dt == DataType.SHORT) || (dt == DataType.LONG)
				|| (dt == DataType.UBYTE) || (dt == DataType.UINT) || (dt == DataType.USHORT) || (dt == DataType.ULONG);
		}

		/// <summary>
		/// true if floating point type.
		/// </summary>
		public static bool isFloatingPoint(this DataType dt)
		{
			return (dt == DataType.FLOAT) || (dt == DataType.DOUBLE);
		}

		/// <summary>
		/// true if an enumeration type (ENUM1, 2, or 4).
		/// </summary>
		public static bool isEnum(this DataType dt)
		{
			return (dt == DataType.ENUM1) || (dt == DataType.ENUM2) || (dt == DataType.ENUM4);
		}

		/// <summary>
		/// Returns a DataType related to this one but with the specified signedness.
		/// Only meaningful for integral data types; non-integral types are returned unchanged.
		/// </summary>
		public static DataType withSignedness(this DataType dt, Signedness signedness)
		{
			switch (dt)
			{
				case DataType.BYTE:
				case DataType.UBYTE:
					return signedness == Signedness.UNSIGNED ? DataType.UBYTE : DataType.BYTE;
				case DataType.SHORT:
				case DataType.USHORT:
					return signedness == Signedness.UNSIGNED ? DataType.USHORT : DataType.SHORT;
				case DataType.INT:
				case DataType.UINT:
					return signedness == Signedness.UNSIGNED ? DataType.UINT : DataType.INT;
				case DataType.LONG:
				case DataType.ULONG:
					return signedness == Signedness.UNSIGNED ? DataType.ULONG : DataType.LONG;
			}
			return dt;
		}

		public static bool isEnumCompatible(this DataType dt, DataType inferred)
		{
			if (dt == inferred) return true;
			switch (dt)
			{
				case DataType.ENUM1:
					return inferred == DataType.BYTE || inferred == DataType.STRING;
				case DataType.ENUM2:
					return inferred == DataType.SHORT || inferred == DataType.STRING;
				case DataType.ENUM4:
					return inferred == DataType.INT || inferred == DataType.STRING;
				default:
					return false;
			}
		}

		/// <summary>
		/// Returns the matching ENUM type for an integral type; otherwise the argument unchanged.
		/// </summary>
		public static DataType enumTypeize(DataType dt)
		{
			switch (dt)
			{
				case DataType.BYTE:
				case DataType.UBYTE:
					return DataType.ENUM1;
				case DataType.SHORT:
				case DataType.USHORT:
					return DataType.ENUM2;
				case DataType.INT:
				case DataType.UINT:
					return DataType.ENUM4;
				default:
					return dt;
			}
		}

		/// <summary>
		/// Find the DataType that matches this name (case-insensitive).
		/// </summary>
		public static DataType? getType(string name)
		{
			if (name == null) return null;
			if (Enum.TryParse(name, true, out DataType result)) return result;
			return null;
		}

		/// <summary>
		/// Find the DataType for an Array.
		/// </summary>
		public static DataType getType(NcArray arr)
		{
			return getType(arr.ElementType, arr.isUnsigned());
		}

		/// <summary>
		/// Find the DataType that matches this class.
		/// </summary>
		public static DataType getType(Type c, bool isUnsigned)
		{
			if ((c == typeof(float)))
			{
				return DataType.FLOAT;
			}
			if ((c == typeof(double)))
			{
				return DataType.DOUBLE;
			}
			if ((c == typeof(short)))
			{
				return isUnsigned ? DataType.USHORT : DataType.SHORT;
			}
			if ((c == typeof(int)))
			{
				return isUnsigned ? DataType.UINT : DataType.INT;
			}
			if ((c == typeof(byte)))
			{
				return isUnsigned ? DataType.UBYTE : DataType.BYTE;
			}
			if ((c == typeof(char)))
			{
				return DataType.CHAR;
			}
			if ((c == typeof(bool)))
			{
				return DataType.BOOLEAN;
			}
			if ((c == typeof(long)))
			{
				return isUnsigned ? DataType.ULONG : DataType.LONG;
			}
			if (c == typeof(string))
			{
				return DataType.STRING;
			}
			if (c == typeof(StructureData))
			{
				return DataType.STRUCTURE;
			}
			if (c == typeof(StructureDataIterator))
			{
				return DataType.SEQUENCE;
			}
			if (c == typeof(ByteBuffer))
			{
				return DataType.OPAQUE;
			}
			return DataType.OBJECT;
		}

		/// <summary>
		/// Convert the argument to the next largest integral data type by an unsigned conversion,
		/// so the result is interpreted as non-negative.
		/// </summary>
		public static object widenNumber(object number)
		{
			if (number is byte)
			{
				return unsignedByteToShort((byte)number);
			}
			else if (number is short)
			{
				return unsignedShortToInt((short)number);
			}
			else if (number is int)
			{
				return unsignedIntToLong((int)number);
			}
			else if (number is long)
			{
				return unsignedLongToBigInt((long)number);
			}
			else
			{
				return number;
			}
		}

		/// <summary>
		/// Like <see cref="widenNumber(object)"/>, but only widens values that are negative.
		/// </summary>
		public static object widenNumberIfNegative(object number)
		{
			if (number is byte && (sbyte)(byte)number < 0)
			{
				return unsignedByteToShort((byte)number);
			}
			else if (number is short && (short)number < 0)
			{
				return unsignedShortToInt((short)number);
			}
			else if (number is int && (int)number < 0)
			{
				return unsignedIntToLong((int)number);
			}
			else if (number is long && (long)number < 0)
			{
				return unsignedLongToBigInt((long)number);
			}
			return number;
		}

		/// <summary>
		/// Converts a long to BigInteger by an unsigned conversion.
		/// </summary>
		public static BigInteger unsignedLongToBigInt(long l)
		{
			if (l >= 0L)
			{
				return new BigInteger(l);
			}
			else
			{
				uint upper = (uint)(l >> 32);
				uint lower = (uint)l;
				BigInteger result = new BigInteger(upper) << 32;
				result += new BigInteger(lower);
				return result;
			}
		}

		/// <summary>
		/// Converts an int to long by an unsigned conversion.
		/// </summary>
		public static long unsignedIntToLong(int i)
		{
			return (long)(uint)i;
		}

		/// <summary>
		/// Converts a short to int by an unsigned conversion.
		/// </summary>
		public static int unsignedShortToInt(short s)
		{
			return s & 0xFFFF;
		}

		/// <summary>
		/// Converts a byte to short by an unsigned conversion.
		/// </summary>
		public static short unsignedByteToShort(byte b)
		{
			return (short)(b & 0xFF);
		}
	}
}
