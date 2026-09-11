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
	/// DO NOT USE
	/// 
	/// @author caron
	/// @since Nov 15, 2008
	/// </summary>
	public class ArrayRagged : NcArray
	{

		protected internal ArrayRagged(int[] shape) : base(DataType.OBJECT, shape)
		{
		}


		public override Type ElementType
		{
			get
			{
				return null; // To change body of implemented methods use File | Settings | File Templates.
			}
		}

		/// <summary>
		/// DO NOT USE, throws UnsupportedOperationException
		/// </summary>
		protected internal override NcArray createView(NcIndex index)
		{
			if (index.Size == Size)
			{
				return this;
			}
			throw new System.NotSupportedException();
		}

		public override object Storage
		{
			get
			{
				return null; // To change body of implemented methods use File | Settings | File Templates.
			}
		} // used to create NcArray from java array

		protected internal override void copyFrom1DJavaArray(IndexIterator iter, object javaArray)
		{
			// To change body of implemented methods use File | Settings | File Templates.
		}

		protected internal override void copyTo1DJavaArray(IndexIterator iter, object javaArray)
		{
			// To change body of implemented methods use File | Settings | File Templates.
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// DO NOT USE, throws UnsupportedOperationException
		/// </summary>
		public override NcArray copy()
		{
			throw new System.NotSupportedException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override double getDouble(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setDouble(NcIndex i, double value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override float getFloat(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setFloat(NcIndex i, float value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override long getLong(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setLong(NcIndex i, long value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override int getInt(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setInt(NcIndex i, int value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override short getShort(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setShort(NcIndex i, short value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override byte getByte(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setByte(NcIndex i, byte value)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override bool getBoolean(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setBoolean(NcIndex i, bool value)
		{
			throw new ForbiddenConversionException();
		}

		public override object getObject(NcIndex ima)
		{
			return null; // To change body of implemented methods use File | Settings | File Templates.
		}

		public override void setObject(NcIndex ima, object value)
		{
			// To change body of implemented methods use File | Settings | File Templates.
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override char getChar(NcIndex i)
		{
			throw new ForbiddenConversionException();
		}

		/// <summary>
		/// DO NOT USE, throw ForbiddenConversionException
		/// </summary>
		public override void setChar(NcIndex i, char value)
		{
			throw new ForbiddenConversionException();
		}

		// trusted, assumes that individual dimension lengths have been checked
		// package private : mostly for iterators
		public override double getDouble(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setDouble(int index, double value)
		{
			throw new ForbiddenConversionException();
		}

		public override float getFloat(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setFloat(int index, float value)
		{
			throw new ForbiddenConversionException();
		}

		public override long getLong(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setLong(int index, long value)
		{
			throw new ForbiddenConversionException();
		}

		public override int getInt(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setInt(int index, int value)
		{
			throw new ForbiddenConversionException();
		}

		public override short getShort(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setShort(int index, short value)
		{
			throw new ForbiddenConversionException();
		}

		public override byte getByte(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setByte(int index, byte value)
		{
			throw new ForbiddenConversionException();
		}

		public override char getChar(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setChar(int index, char value)
		{
			throw new ForbiddenConversionException();
		}

		public override bool getBoolean(int index)
		{
			throw new ForbiddenConversionException();
		}

		public override void setBoolean(int index, bool value)
		{
			throw new ForbiddenConversionException();
		}

		public override object getObject(int elem)
		{
			return null; // To change body of implemented methods use File | Settings | File Templates.
		}

		public override void setObject(int elem, object value)
		{
			// To change body of implemented methods use File | Settings | File Templates.
		}

	}

}
