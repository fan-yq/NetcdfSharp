// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// An Index into an Array that always returns 0. It can have any shape, so it allows you to create a
	/// constant Array of any shape.
	/// 
	/// @author caron
	/// </summary>
	// LOOK : need to override section, etc !!
	public class IndexConstant : NcIndex
	{

		protected internal IndexConstant(int rank) : base(rank)
		{
		}

		protected internal IndexConstant(int[] shape) : base(shape)
		{
		}

		protected internal IndexConstant(int[] shape, int[] stride) : base(shape, stride)
		{
		}

		/// <summary>
		/// always return 0 </summary>
		public override int currentElement()
		{
			return 0;
		}

		internal override NcIndex flip(int index)
		{
			return this;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override Index sectionNoReduce(NetcdfSharp.java.util.List<Range> ranges) throws InvalidRangeException
		internal override NcIndex sectionNoReduce(IList<NcRange> ranges)
		{
			Section curr = new Section(shape);
			Section want = curr.compose(new Section(ranges));
			return new IndexConstant(want.Shape);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Override Index section(NetcdfSharp.java.util.List<Range> ranges) throws InvalidRangeException
		internal override NcIndex section(IList<NcRange> ranges)
		{
			Section curr = new Section(shape);
			Section want = curr.compose(new Section(ranges)).reduce();
			return new IndexConstant(want.Shape);
		}

		internal override NcIndex reduce()
		{
			Section curr = new Section(shape);
			Section want = curr.reduce();
			return new IndexConstant(want.Shape);
		}

		internal override NcIndex reduce(int dim)
		{
			if ((dim < 0) || (dim >= rank))
			{
				throw new ArgumentException("illegal reduce dim " + dim);
			}
			if (shape[dim] != 1)
			{
				throw new ArgumentException("illegal reduce dim " + dim + " : length != 1");
			}

			Section curr = new Section(shape);
			Section want = curr.toBuilder().removeRange(dim).build();
			return new IndexConstant(want.Shape);
		}

		internal override NcIndex transpose(int index1, int index2)
		{
			return this;
		}

		internal override NcIndex permute(int[] dims)
		{
			return this;
		}

		// This method is only used in Array.get1DJavaArray(Class), where it's used to decide whether a copy is necessary.
		// In the case of a constant array, we DO need a copy because the size of our backing storage (length=1) is likely
		// not the size of the Array.
		// Fixes bug in https://github.com/Unidata/thredds/issues/581
		internal override bool FastIterator
		{
			get
			{
				return false;
			}
		}

		///////////////////////

		internal override IndexIterator getIndexIterator(NcArray maa)
		{
			return new IteratorConstant(size, maa);
		}

		private class IteratorConstant : IndexIterator
		{

			internal int currElement = -1;
			internal readonly NcArray maa;
			internal long size;

			internal IteratorConstant(long size, NcArray maa)
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

			internal NcIndex counter; // usually not used

			public override string ToString()
			{
				if (counter == null)
				{
					counter = new NcIndex(maa.Shape);
				}
				counter.setCurrentCounter(currElement);
				return counter.ToString();
			}

			public virtual int[] CurrentCounter
			{
				get
				{
					if (counter == null)
					{
						counter = new NcIndex(maa.Shape);
					}
					counter.setCurrentCounter(currElement);
					return counter.current;
				}
			}

			public virtual double DoubleCurrent
			{
				get
				{
					return maa.getDouble(0);
				}
				set
				{
					maa.setDouble(0, value);
				}
			}

			public virtual double DoubleNext
			{
				get
				{
					currElement++;
					return maa.getDouble(0);
				}
				set
				{
					currElement++;
					maa.setDouble(0, value);
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
					currElement++;
					return maa.getFloat(0);
				}
				set
				{
					currElement++;
					maa.setFloat(0, value);
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
					currElement++;
					return maa.getLong(0);
				}
				set
				{
					currElement++;
					maa.setLong(0, value);
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
					currElement++;
					return maa.getInt(0);
				}
				set
				{
					currElement++;
					maa.setInt(0, value);
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
					currElement++;
					return maa.getShort(0);
				}
				set
				{
					currElement++;
					maa.setShort(0, value);
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
					currElement++;
					return maa.getByte(0);
				}
				set
				{
					currElement++;
					maa.setByte(0, value);
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
					currElement++;
					return maa.getChar(0);
				}
				set
				{
					currElement++;
					maa.setChar(0, value);
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
					currElement++;
					return maa.getBoolean(0);
				}
				set
				{
					currElement++;
					maa.setBoolean(0, value);
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
					currElement++;
					return maa.getObject(0);
				}
				set
				{
					currElement++;
					maa.setObject(0, value);
				}
			}



			public virtual object next()
			{
				currElement++;
				return maa.getObject(0);
			}
		}
	}

}
