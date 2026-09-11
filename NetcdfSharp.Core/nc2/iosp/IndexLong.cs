// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp
{
	/// <summary>
	/// Uses longs for indexing, otherwise similar to NetcdfSharp.ucar.ma2.Index
	/// 
	/// @author caron
	/// @since Jul 30, 2010
	/// </summary>
	public class IndexLong
	{
	  private int[] shape;
	  private long[] stride;
	  private int rank;

	  private int offset; // element = offset + stride[0]*current[0] + ...
	  private int[] current; // current element's index

	  // shape = int[] {1}
	  public IndexLong()
	  {
		shape = new int[] {1};
		stride = new long[] {1};

		rank = shape.Length;
		current = new int[rank];

		stride[0] = 1;
		offset = 0;
	  }

	  public IndexLong(int[] _shape, long[] _stride)
	  {
		this.shape = new int[_shape.Length]; // optimization over clone
		System.Array.Copy(_shape, 0, this.shape, 0, _shape.Length);

		this.stride = new long[_stride.Length]; // optimization over clone
		System.Array.Copy(_stride, 0, this.stride, 0, _stride.Length);

		rank = shape.Length;
		current = new int[rank];
		offset = 0;
	  }

	  public static long computeSize(int[] shape)
	  {
		long product = 1;
		for (int ii = shape.Length - 1; ii >= 0; ii--)
		{
		  product *= shape[ii];
		}
		return product;
	  }

	  public virtual long incr()
	  {
		int digit = rank - 1;
		while (digit >= 0)
		{
		  current[digit]++;
		  if (current[digit] < shape[digit])
		  {
			break;
		  }
		  current[digit] = 0;
		  digit--;
		}
		return currentElement();
	  }

	  public virtual long currentElement()
	  {
		long value = offset;
		for (int ii = 0; ii < rank; ii++)
		{
		  value += current[ii] * stride[ii];
		}
		return value;
	  }
	}

}
