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
	/// Specialization of Index for rank 4 arrays.
	/// </summary>
	/// <seealso cref="NcIndex"
	/// @author caron/>
	public class Index4D : NcIndex
	{

		/// <summary>
		/// current element's indices </summary>
		private int curr0, curr1, curr2, curr3;
		/// <summary>
		/// array strides </summary>
		private int stride0, stride1, stride2, stride3;
		/// <summary>
		/// array shapes </summary>
		private int shape0, shape1, shape2, shape3;

		internal Index4D() : base(4)
		{
		}

		public Index4D(int[] shape) : base(shape)
		{
			precalc();
		}

		protected internal override void precalc()
		{
			shape0 = shape[0];
			shape1 = shape[1];
			shape2 = shape[2];
			shape3 = shape[3];

			stride0 = stride[0];
			stride1 = stride[1];
			stride2 = stride[2];
			stride3 = stride[3];

			curr0 = current[0];
			curr1 = current[1];
			curr2 = current[2];
			curr3 = current[3];
		}

		public override string ToString()
		{
			return curr0 + "," + curr1 + "," + curr2 + "," + curr3;
		}

		public override int[] CurrentCounter
		{
			get
			{
				current[0] = curr0;
				current[1] = curr1;
				current[2] = curr2;
				current[3] = curr3;
				return (int[])current.Clone();
			}
		}

		public override int currentElement()
		{
			return offset + curr0 * stride0 + curr1 * stride1 + curr2 * stride2 + +curr3 * stride3;
		}

		public override int incr()
		{
			if (++curr3 >= shape3)
			{
				curr3 = 0;
				if (++curr2 >= shape2)
				{
					curr2 = 0;
					if (++curr1 >= shape1)
					{
						curr1 = 0;
						if (++curr0 >= shape0)
						{
							curr0 = 0; // rollover !
						}
					}
				}
			}

			return offset + curr0 * stride0 + curr1 * stride1 + curr2 * stride2 + curr3 * stride3;
		}

		public override void setDim(int dim, int value)
		{
			if (value < 0 || value >= shape[dim]) // check index here
			{
				throw new System.IndexOutOfRangeException();
			}

			if (dim == 3)
			{
				curr3 = value;
			}
			else if (dim == 2)
			{
				curr2 = value;
			}
			else if (dim == 1)
			{
				curr1 = value;
			}
			else
			{
				curr0 = value;
			}
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
				throw new System.IndexOutOfRangeException();
			}
			curr1 = v;
			return this;
		}

		public override NcIndex set2(int v)
		{
			if (v < 0 || v >= shape2) // check index here
			{
				throw new System.IndexOutOfRangeException();
			}
			curr2 = v;
			return this;
		}

		public override NcIndex set3(int v)
		{
			if (v < 0 || v >= shape3) // check index here
			{
				throw new System.IndexOutOfRangeException();
			}
			curr3 = v;
			return this;
		}

		public override NcIndex set(int v0, int v1, int v2, int v3)
		{
			set0(v0);
			set1(v1);
			set2(v2);
			set3(v3);
			return this;
		}

		public override NcIndex set(int[] index)
		{
			if (index.Length != rank)
			{
				throw new System.IndexOutOfRangeException();
			}
			set0(index[0]);
			set1(index[1]);
			set2(index[2]);
			set3(index[3]);
			return this;
		}

		public override object clone()
		{
			return base.clone();
		}

		// experimental : should be package private
		internal virtual int setDirect(int v0, int v1, int v2, int v3)
		{
			return offset + v0 * stride0 + v1 * stride1 + v2 * stride2 + v3 * stride3;
		}


	}

}
