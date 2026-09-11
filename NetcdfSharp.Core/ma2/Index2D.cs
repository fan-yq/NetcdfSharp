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
	/// Specialization of Index for rank 2 arrays.
	/// </summary>
	/// <seealso cref="NcIndex"
	/// @author caron/>
	public class Index2D : NcIndex
	{

		/// <summary>
		/// current element's indices </summary>
		private int curr0, curr1;
		/// <summary>
		/// array strides </summary>
		private int stride0, stride1;
		/// <summary>
		/// array shapes </summary>
		private int shape0, shape1;

		internal Index2D() : base(2)
		{
		}

		public Index2D(int[] shape) : base(shape)
		{
			precalc();
		}

		protected internal override void precalc()
		{
			shape0 = shape[0];
			shape1 = shape[1];

			stride0 = stride[0];
			stride1 = stride[1];

			curr0 = current[0];
			curr1 = current[1];
		}

		public override int[] CurrentCounter
		{
			get
			{
				current[0] = curr0;
				current[1] = curr1;
				return (int[])current.Clone();
			}
		}

		public override string ToString()
		{
			return curr0 + "," + curr1;
		}

		public override int currentElement()
		{
			return offset + curr0 * stride0 + curr1 * stride1;
		}

		public override int incr()
		{
			if (++curr1 >= shape1)
			{
				curr1 = 0;
				if (++curr0 >= shape0)
				{
					curr0 = 0; // rollover !
				}
			}
			return offset + curr0 * stride0 + curr1 * stride1;
		}


		public override void setDim(int dim, int value)
		{
			if (value < 0 || value >= shape[dim]) // check index here
			{
				throw new System.IndexOutOfRangeException();
			}
			if (dim == 1)
			{
				curr1 = value;
			}
			else
			{
				curr0 = value;
			}
		}

		public override NcIndex set(int[] index)
		{
			if (index.Length != rank)
			{
				throw new System.IndexOutOfRangeException();
			}
			set0(index[0]);
			set1(index[1]);
			return this;
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

		public override NcIndex set1(int v)
		{
			if (v < 0 || v >= shape1) // check index here
			{
				throw new System.IndexOutOfRangeException("index=" + v + " shape=" + shape1);
			}
			curr1 = v;
			return this;
		}

		public override NcIndex set(int v0, int v1)
		{
			set0(v0);
			set1(v1);
			return this;
		}

		public override object clone()
		{
			return base.clone();
		}

		internal virtual int setDirect(int v0, int v1)
		{
			if (v0 < 0 || v0 >= shape0)
			{
				throw new System.IndexOutOfRangeException();
			}
			if (v1 < 0 || v1 >= shape1)
			{
				throw new System.IndexOutOfRangeException();
			}
			return offset + v0 * stride0 + v1 * stride1;
		}

	}

}
