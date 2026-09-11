// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Helper class for StructureDataAscii
	/// </summary>
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @Immutable public class ArrayScalar extends Array
	public class ArrayScalar : NcArray
	{
		private readonly object value;

		public ArrayScalar(object value, bool isUnsigned) : base(DataTypes.getType(value.GetType(), isUnsigned), new int[] { })
		{
			this.value = value;
		}

		public override Type ElementType
		{
			get
			{
				return value.GetType();
			}
		}

		protected internal override NcArray createView(NcIndex index)
		{
			return this;
		}

		public override object Storage
		{
			get
			{
				return null; // To change body of implemented methods use File | Settings | File Templates.
			}
		}

		protected internal override void copyFrom1DJavaArray(IndexIterator iter, object javaArray)
		{
			// To change body of implemented methods use File | Settings | File Templates.
		}

		protected internal override void copyTo1DJavaArray(IndexIterator iter, object javaArray)
		{
			// To change body of implemented methods use File | Settings | File Templates.
		}

		public override double getDouble(NcIndex ima)
		{
			return getDouble(0);
		}

		public override void setDouble(NcIndex ima, double value)
		{
		}
		public override float getFloat(NcIndex ima)
		{
			return getFloat(0);
		}

		public override void setFloat(NcIndex ima, float value)
		{
		}
		public override long getLong(NcIndex ima)
		{
			return getLong(0);
		}

		public override void setLong(NcIndex ima, long value)
		{
		}
		public override int getInt(NcIndex ima)
		{
			return getInt(0);
		}

		public override void setInt(NcIndex ima, int value)
		{
		}
		public override short getShort(NcIndex ima)
		{
			return getShort(0);
		}

		public override void setShort(NcIndex ima, short value)
		{
		}
		public override byte getByte(NcIndex ima)
		{
			return getByte(0);
		}

		public override void setByte(NcIndex ima, byte value)
		{
		}
		public override char getChar(NcIndex ima)
		{
			return getChar(0);
		}

		public override void setChar(NcIndex ima, char value)
		{
		}
		public override bool getBoolean(NcIndex ima)
		{
			return getBoolean(0);
		}

		public override void setBoolean(NcIndex ima, bool value)
		{
		}
		public override object getObject(NcIndex ima)
		{
			return value;
		}

		public override void setObject(NcIndex ima, object value)
		{
		}
		public override double getDouble(int elem)
		{
			return Convert.ToDouble(value);
		}

		public override void setDouble(int elem, double val)
		{
		}
		public override float getFloat(int elem)
		{
			return Convert.ToSingle(value);
		}

		public override void setFloat(int elem, float val)
		{
		}
		public override long getLong(int elem)
		{
			return Convert.ToInt64(value);
		}

		public override void setLong(int elem, long value)
		{
		}
		public override int getInt(int elem)
		{
			return Convert.ToInt32(value);
		}

		public override void setInt(int elem, int value)
		{
		}
		public override short getShort(int elem)
		{
			return Convert.ToInt16(value);
		}

		public override void setShort(int elem, short value)
		{
		}
		public override byte getByte(int elem)
		{
			return Convert.ToByte(value);
		}

		public override void setByte(int elem, byte value)
		{
		}
		public override char getChar(int elem)
		{
			return ((char?)value).Value;
		}

		public override void setChar(int elem, char value)
		{
		}
		public override bool getBoolean(int elem)
		{
			return ((bool?)value).Value;
		}

		public override void setBoolean(int elem, bool value)
		{
		}
		public override object getObject(int elem)
		{
			return value;
		}

		public override void setObject(int elem, object value)
		{
		}
	}

}
