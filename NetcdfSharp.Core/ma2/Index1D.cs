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
	/// Specialization of Index for rank 1 arrays.
	/// </summary>
	/// <seealso cref="NcIndex"
	/// @author caron/>
	public class Index1D : NcIndex
	{

		/// <summary>
		/// current element's indices </summary>
		private int curr0;
		/// <summary>
		/// array strides </summary>
		private int stride0;
		/// <summary>
		/// array shapes </summary>
		private int shape0;

		internal Index1D() : base(1)
		{
		}

		public Index1D(int[] shape) : base(shape)
		{
			precalc();
		}

		public override string ToString()
		{
			return Convert.ToString(curr0);
		}

		protected internal override void precalc()
		{
			shape0 = shape[0];
			stride0 = stride[0];
			curr0 = current[0];
		}

		public override int[] CurrentCounter
		{
			get
			{
				current[0] = curr0;
				return (int[])current.Clone();
			}
		}

		public override int currentElement()
		{
			return offset + curr0 * stride0;
		}

		public override int incr()
		{
			if (++curr0 >= shape0)
			{
				curr0 = 0; // rollover !
			}
			return offset + curr0 * stride0;
		}


		public override void setDim(int dim, int value)
		{
			if (value < 0 || value >= shape[dim]) // check index here
			{
				throw new System.IndexOutOfRangeException();
			}
			curr0 = value;
		}

		public override NcIndex set0(int v)
		{
			if (v < 0 || v >= shape0) // check index here
			{
				throw new System.IndexOutOfRangeException();
			}
			curr0 = v;
			return this;
		}

		public override NcIndex set(int v0)
		{
			set0(v0);
			return this;
		}

		public override NcIndex set(int[] index)
		{
			if (index.Length != rank)
			{
				throw new System.IndexOutOfRangeException();
			}
			set0(index[0]);
			return this;
		}

		public override object clone()
		{
			return base.clone();
		}

		internal virtual int setDirect(int v0)
		{
			if (v0 < 0 || v0 >= shape0)
			{
				throw new System.IndexOutOfRangeException();
			}
			return offset + v0 * stride0;
		}

	}

}
