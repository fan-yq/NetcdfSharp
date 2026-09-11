// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.ma2
{
	/// <summary>
	/// A "fast" iterator that can be used when the data is in canonical order.
	/// 
	/// @author caron
	/// </summary>
	/* the idea is IteratorFast can do the iteration without an Index */
	public class IteratorFast : IndexIterator
	{

		private int currElement = -1;
		private readonly NcArray maa;
		private long size;

		internal IteratorFast(long size, NcArray maa)
		{
			this.size = size;
			this.maa = maa;
		}

		public virtual bool hasNext()
		{
			return currElement < size - 1;
		}

		public virtual bool hasMore(int howMany)
		{
			return currElement < size - howMany;
		}

		private NcIndex counter; // usually not used

		public override string ToString()
		{
			if (counter == null || counter.ToString().Equals("")) // not sure about the second condition
			{
				counter = NcIndex.factory(maa.Shape);
			}
			counter.setCurrentCounter(currElement);
			return counter.ToString();
		}

		public virtual int[] CurrentCounter
		{
			get
			{
				if (counter == null) // or counter == "" ?
				{
					counter = NcIndex.factory(maa.Shape);
				}
				counter.setCurrentCounter(currElement);
				return counter.current;
			}
		}

		public virtual double DoubleCurrent
		{
			get
			{
				return maa.getDouble(currElement);
			}
			set
			{
				maa.setDouble(currElement, value);
			}
		}

		public virtual double DoubleNext
		{
			get
			{
				return maa.getDouble(++currElement);
			}
			set
			{
				maa.setDouble(++currElement, value);
			}
		}



		public virtual float FloatCurrent
		{
			get
			{
				return maa.getFloat(currElement);
			}
			set
			{
				maa.setFloat(currElement, value);
			}
		}

		public virtual float FloatNext
		{
			get
			{
				return maa.getFloat(++currElement);
			}
			set
			{
				maa.setFloat(++currElement, value);
			}
		}



		public virtual long LongCurrent
		{
			get
			{
				return maa.getLong(currElement);
			}
			set
			{
				maa.setLong(currElement, value);
			}
		}

		public virtual long LongNext
		{
			get
			{
				return maa.getLong(++currElement);
			}
			set
			{
				maa.setLong(++currElement, value);
			}
		}



		public virtual int IntCurrent
		{
			get
			{
				return maa.getInt(currElement);
			}
			set
			{
				maa.setInt(currElement, value);
			}
		}

		public virtual int IntNext
		{
			get
			{
				return maa.getInt(++currElement);
			}
			set
			{
				maa.setInt(++currElement, value);
			}
		}



		public virtual short ShortCurrent
		{
			get
			{
				return maa.getShort(currElement);
			}
			set
			{
				maa.setShort(currElement, value);
			}
		}

		public virtual short ShortNext
		{
			get
			{
				return maa.getShort(++currElement);
			}
			set
			{
				maa.setShort(++currElement, value);
			}
		}



		public virtual byte ByteCurrent
		{
			get
			{
				return maa.getByte(currElement);
			}
			set
			{
				maa.setByte(currElement, value);
			}
		}

		public virtual byte ByteNext
		{
			get
			{
				return maa.getByte(++currElement);
			}
			set
			{
				maa.setByte(++currElement, value);
			}
		}



		public virtual char CharCurrent
		{
			get
			{
				return maa.getChar(currElement);
			}
			set
			{
				maa.setChar(currElement, value);
			}
		}

		public virtual char CharNext
		{
			get
			{
				return maa.getChar(++currElement);
			}
			set
			{
				maa.setChar(++currElement, value);
			}
		}



		public virtual bool BooleanCurrent
		{
			get
			{
				return maa.getBoolean(currElement);
			}
			set
			{
				maa.setBoolean(currElement, value);
			}
		}

		public virtual bool BooleanNext
		{
			get
			{
				return maa.getBoolean(++currElement);
			}
			set
			{
				maa.setBoolean(++currElement, value);
			}
		}



		public virtual object ObjectCurrent
		{
			get
			{
				return maa.getObject(currElement);
			}
			set
			{
				maa.setObject(currElement, value);
			}
		}

		public virtual object ObjectNext
		{
			get
			{
				return maa.getObject(++currElement);
			}
			set
			{
				maa.setObject(++currElement, value);
			}
		}



		public virtual object next()
		{
			return maa.getObject(++currElement);
		}
	}

}
