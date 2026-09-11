// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Indexes for Multidimensional arrays. An Index refers to a particular element of an array.
	/// <para>
	/// This is a generalization of index as int[]. Its main function is
	/// to do the index arithmetic to translate an n-dim index into a 1-dim index.
	/// The user obtains this by calling getIndex() on an Array.
	/// The set() and seti() routines are convenience routines for 1-7 dim arrays.
	/// 
	/// @author caron
	/// </para>
	/// </summary>
	/// <seealso cref="Array"/>

	public class NcIndex : ICloneable
	{
		public static readonly Index0D scalarIndexImmutable = new Index0D(); // immutable, so can be shared

		/// <summary>
		/// Generate a subclass of Index optimized for this array's rank
		/// </summary>
		/// <param name="shape"> use this shape </param>
		/// <returns> a subclass of Index optimized for this array's rank </returns>
		public static NcIndex factory(int[] shape)
		{
			int rank = shape.Length;
			switch (rank)
			{
				case 0:
					return new Index0D();
				case 1:
					return new Index1D(shape);
				case 2:
					return new Index2D(shape);
				case 3:
					return new Index3D(shape);
				case 4:
					return new Index4D(shape);
				case 5:
					return new Index5D(shape);
				case 6:
					return new Index6D(shape);
				case 7:
					return new Index7D(shape);
				default:
					return new NcIndex(shape);
			}
		}

		private static NcIndex factory(int rank)
		{
			switch (rank)
			{
				case 0:
					return new Index0D();
				case 1:
					return new Index1D();
				case 2:
					return new Index2D();
				case 3:
					return new Index3D();
				case 4:
					return new Index4D();
				case 5:
					return new Index5D();
				case 6:
					return new Index6D();
				case 7:
					return new Index7D();
				default:
					return new NcIndex(rank);
			}
		}

		/// <summary>
		/// Compute total number of elements in the array.
		/// Stop at vlen
		/// </summary>
		/// <param name="shape"> length of array in each dimension. </param>
		/// <returns> total number of elements in the array. </returns>
		public static long computeSize(int[] shape)
		{
			long product = 1;
			foreach (int aShape in shape)
			{
				if (aShape < 0)
				{
					break; // stop at vlen
				}
				product *= aShape;
			}
			return product;
		}

		/// <summary>
		/// Compute standard strides based on array's shape.
		/// Ignore vlen
		/// </summary>
		/// <param name="shape"> length of array in each dimension. </param>
		/// <param name="stride"> put result here </param>
		/// <returns> standard strides based on array's shape. </returns>
		private static long computeStrides(int[] shape, int[] stride)
		{
			long product = 1;
			for (int ii = shape.Length - 1; ii >= 0; ii--)
			{
				int thisDim = shape[ii];
				if (thisDim < 0)
				{
					continue; // ignore vlen
				}
				stride[ii] = (int)product;
				product *= thisDim;
			}
			return product;
		}

		////////////////////////////////////////////////////////////////////////////////////////////
		protected internal int[] shape;
		protected internal int[] stride;
		protected internal int rank;

		protected internal long size; // total number of elements
		protected internal int offset; // element = offset + stride[0]*current[0] + ...
		private bool fastIterator = true; // use fast iterator if in canonical order

		protected internal int[] current; // current element's index, used only for the general case

		protected internal bool hasvlen;

		/// <summary>
		/// General case Index - use when you want to manipulate current elements yourself
		/// </summary>
		/// <param name="rank"> rank of the Index </param>
		protected internal NcIndex(int rank)
		{
			this.rank = rank;
			shape = new int[rank];
			current = new int[rank];
			stride = new int[rank];
			hasvlen = false;
		}

		/// <summary>
		/// Constructor for subclasses only.
		/// </summary>
		/// <param name="_shape"> describes an index section: slowest varying comes first (row major) </param>
		protected internal NcIndex(int[] _shape)
		{
			this.shape = new int[_shape.Length]; // optimization over clone
			System.Array.Copy(_shape, 0, this.shape, 0, _shape.Length);

			rank = shape.Length;
			current = new int[rank];
			stride = new int[rank];
			size = computeStrides(shape, stride);
			offset = 0;
			hasvlen = (shape.Length > 0 && shape[shape.Length - 1] < 0);
		}

		/// <summary>
		/// Constructor that lets you set the strides yourself.
		/// This is used as a counter, not a description of an index section.
		/// </summary>
		/// <param name="_shape"> Index shape </param>
		/// <param name="_stride"> Index stride </param>
		public NcIndex(int[] _shape, int[] _stride)
		{
			this.shape = new int[_shape.Length]; // optimization over clone
			System.Array.Copy(_shape, 0, this.shape, 0, _shape.Length);

			this.stride = new int[_stride.Length]; // optimization over clone
			System.Array.Copy(_stride, 0, this.stride, 0, _stride.Length);

			rank = shape.Length;
			current = new int[rank];
			size = computeSize(shape);
			offset = 0;
			hasvlen = (shape.Length > 0 && shape[shape.Length - 1] < 0);
		}

		/// <summary>
		/// subclass specialization/optimization calculations
		/// </summary>
		protected internal virtual void precalc()
		{
		}

		/// <summary>
		/// Create a new Index based on current one, except
		/// flip the index so that it runs from shape[index]-1 to 0.
		/// Leave rightmost vlen alone.
		/// </summary>
		/// <param name="index"> dimension to flip </param>
		/// <returns> new index with flipped dimension </returns>
		internal virtual NcIndex flip(int index)
		{
			if ((index < 0) || (index >= rank))
			{
				throw new ArgumentException();
			}

			NcIndex i = (NcIndex)this.clone();
			if (shape[index] >= 0)
			{ // !vlen case
				i.offset += stride[index] * (shape[index] - 1);
				i.stride[index] = -stride[index];
			}
			i.fastIterator = false;
			i.precalc(); // any subclass-specific optimizations
			return i;
		}

		/// <summary>
		/// create a new Index based on a subsection of this one, with rank reduction if
		/// dimension length == 1.
		/// </summary>
		/// <param name="ranges"> array of Ranges that specify the array subset.
		///        Must be same rank as original Array.
		///        A particular Range: 1) may be a subset; 2) may be null, meaning use entire Range. </param>
		/// <returns> new Index, with same or smaller rank as original. </returns>
		/// <exception cref="InvalidRangeException"> if ranges dont match current shape </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Index section(NetcdfSharp.java.util.List<Range> ranges) throws InvalidRangeException
		internal virtual NcIndex section(IList<NcRange> ranges)
		{

			// check ranges are valid
			if (ranges.Count != rank)
			{
				throw new InvalidRangeException("Bad ranges [] length");
			}
			for (int ii = 0; ii < rank; ii++)
			{
				NcRange r = ranges[ii];
				if (r == null)
				{
					continue;
				}
				if (r == NcRange.VLEN)
				{
					continue;
				}
				if ((r.first() < 0) || (r.first() >= shape[ii]))
				{
					throw new InvalidRangeException("Bad range starting value at index " + ii + " == " + r.first());
				}
				if ((r.last() < 0) || (r.last() >= shape[ii]))
				{
					throw new InvalidRangeException("Bad range ending value at index " + ii + " == " + r.last());
				}
			}

			int reducedRank = rank;
			foreach (NcRange r in ranges)
			{
				if ((r != null) && (r.length() == 1))
				{
					reducedRank--;
				}
			}
			NcIndex newindex = NcIndex.factory(reducedRank);
			newindex.offset = offset;

			// calc shape, size, and index transformations
			// calc strides into original (backing) store
			int newDim = 0;
			for (int ii = 0; ii < rank; ii++)
			{
				NcRange r = ranges[ii];
				if (r == null)
				{ // null range means use the whole original dimension
					newindex.shape[newDim] = shape[ii];
					newindex.stride[newDim] = stride[ii];
					// if (name != null) newindex.name[newDim] = name[ii];
					newDim++;
				}
				else if (r.length() != 1)
				{
					newindex.shape[newDim] = r.length();
					newindex.stride[newDim] = stride[ii] * r.stride();
					newindex.offset += stride[ii] * r.first();
					// if (name != null) newindex.name[newDim] = name[ii];
					newDim++;
				}
				else
				{
					newindex.offset += stride[ii] * r.first(); // constant due to rank reduction
				}
			}
			newindex.size = computeSize(newindex.shape);
			newindex.fastIterator = fastIterator && (newindex.size == size); // if equal, then its not a real subset, so can
																			 // still use fastIterator
			newindex.precalc(); // any subclass-specific optimizations
			return newindex;
		}

		/// <summary>
		/// create a new Index based on a subsection of this one, without rank reduction.
		/// </summary>
		/// <param name="ranges"> list of Ranges that specify the array subset.
		///        Must be same rank as original Array.
		///        A particular Range: 1) may be a subset; 2) may be null, meaning use entire Range. </param>
		/// <returns> new Index, with same rank as original. </returns>
		/// <exception cref="InvalidRangeException"> if ranges dont match current shape </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: Index sectionNoReduce(NetcdfSharp.java.util.List<Range> ranges) throws InvalidRangeException
		internal virtual NcIndex sectionNoReduce(IList<NcRange> ranges)
		{

			// check ranges are valid
			if (ranges.Count != rank)
			{
				throw new InvalidRangeException("Bad ranges [] length");
			}
			for (int ii = 0; ii < rank; ii++)
			{
				NcRange r = ranges[ii];
				if (r == null)
				{
					continue;
				}
				if (r == NcRange.VLEN)
				{
					continue;
				}
				if ((r.first() < 0) || (r.first() >= shape[ii]))
				{
					throw new InvalidRangeException("Bad range starting value at index " + ii + " == " + r.first());
				}
				if ((r.last() < 0) || (r.last() >= shape[ii]))
				{
					throw new InvalidRangeException("Bad range ending value at index " + ii + " == " + r.last());
				}
			}

			// allocate
			NcIndex newindex = NcIndex.factory(rank);
			newindex.offset = offset;

			// calc shape, size, and index transformations
			// calc strides into original (backing) store
			for (int ii = 0; ii < rank; ii++)
			{
				NcRange r = ranges[ii];
				if (r == null)
				{ // null range means use the whole original dimension
					newindex.shape[ii] = shape[ii];
					newindex.stride[ii] = stride[ii];
				}
				else
				{
					newindex.shape[ii] = r.length();
					newindex.stride[ii] = stride[ii] * r.stride();
					newindex.offset += stride[ii] * r.first();
				}
				// if (name != null) newindex.name[ii] = name[ii];
			}
			newindex.size = computeSize(newindex.shape);
			newindex.fastIterator = fastIterator && (newindex.size == size); // if equal, then its not a real subset, so can
																			 // still use fastIterator
			newindex.precalc(); // any subclass-specific optimizations
			return newindex;
		}

		/// <summary>
		/// Create a new Index based on current one by
		/// eliminating any dimensions with length one.
		/// </summary>
		/// <returns> the new Index </returns>
		internal virtual NcIndex reduce()
		{
			NcIndex c = this;
			for (int ii = 0; ii < rank; ii++)
			{
				if (shape[ii] == 1)
				{ // do this on the first one you find
					NcIndex newc = c.reduce(ii);
					return newc.reduce(); // any more to do?
				}
			}
			return c;
		}

		/// <summary>
		/// Create a new Index based on current one by
		/// eliminating the specified dimension;
		/// </summary>
		/// <param name="dim">: dimension to eliminate: must be of length one, else IllegalArgumentException </param>
		/// <returns> the new Index </returns>
		internal virtual NcIndex reduce(int dim)
		{
			if ((dim < 0) || (dim >= rank))
			{
				throw new ArgumentException("illegal reduce dim " + dim);
			}
			if (shape[dim] != 1)
			{
				throw new ArgumentException("illegal reduce dim " + dim + " : length != 1");
			}

			NcIndex newindex = NcIndex.factory(rank - 1);
			newindex.offset = offset;
			int count = 0;
			for (int ii = 0; ii < rank; ii++)
			{
				if (ii != dim)
				{
					newindex.shape[count] = shape[ii];
					newindex.stride[count] = stride[ii];
					// if (name != null) newindex.name[count] = name[ii];

					count++;
				}
			}
			newindex.size = computeSize(newindex.shape);
			newindex.fastIterator = fastIterator;
			newindex.precalc(); // any subclass-specific optimizations
			return newindex;
		}

		/// <summary>
		/// create a new Index based on current one, except
		/// transpose two of the indices.
		/// </summary>
		/// <param name="index1"> transpose these two indices </param>
		/// <param name="index2"> transpose these two indices </param>
		/// <returns> new Index with transposed indices </returns>
		internal virtual NcIndex transpose(int index1, int index2)
		{
			if ((index1 < 0) || (index1 >= rank))
			{
				throw new ArgumentException();
			}
			if ((index2 < 0) || (index2 >= rank))
			{
				throw new ArgumentException();
			}

			NcIndex newIndex = (NcIndex)this.clone();
			newIndex.stride[index1] = stride[index2];
			newIndex.stride[index2] = stride[index1];
			newIndex.shape[index1] = shape[index2];
			newIndex.shape[index2] = shape[index1];
			/*
			 * if (name != null) {
			 * newIndex.name[index1] = name[index2];
			 * newIndex.name[index2] = name[index1];
			 * }
			 */

			newIndex.fastIterator = false;
			newIndex.precalc(); // any subclass-specific optimizations
			return newIndex;
		}

		/// <summary>
		/// create a new Index based on a permutation of the current indices; vlen fails.
		/// </summary>
		/// <param name="dims">: the old index dim[k] becomes the new kth index. </param>
		/// <returns> new Index with permuted indices </returns>
		internal virtual NcIndex permute(int[] dims)
		{
			if (dims.Length != shape.Length)
			{
				throw new ArgumentException();
			}
			foreach (int dim in dims)
			{
				if ((dim < 0) || (dim >= rank))
				{
					throw new ArgumentException();
				}
			}

			bool isPermuted = false;
			NcIndex newIndex = (NcIndex)this.clone();
			for (int i = 0; i < dims.Length; i++)
			{
				newIndex.stride[i] = stride[dims[i]];
				newIndex.shape[i] = shape[dims[i]];
				// if (name != null) newIndex.name[i] = name[dims[i]];
				if (i != dims[i])
				{
					isPermuted = true;
				}
			}

			newIndex.fastIterator = fastIterator && !isPermuted; // useful optimization
			newIndex.precalc(); // any subclass-specific optimizations
			return newIndex;
		}


		/// <summary>
		/// Get the number of dimensions in the array.
		/// </summary>
		/// <returns> the number of dimensions in the array. </returns>
		public virtual int Rank
		{
			get
			{
				return rank;
			}
		}

		/// <summary>
		/// Get the shape: length of array in each dimension.
		/// </summary>
		/// <returns> the shape </returns>
		public virtual int[] Shape
		{
			get
			{
				int[] result = new int[shape.Length]; // optimization over clone
				System.Array.Copy(shape, 0, result, 0, shape.Length);
				return result;
			}
		}

		/// <summary>
		/// Get the length of the ith dimension.
		/// </summary>
		/// <param name="index"> which dimension. must be in [0, getRank()) </param>
		/// <returns> the ith dimension length </returns>
		public virtual int getShape(int index)
		{
			return shape[index];
		}

		/// <summary>
		/// Get an index iterator for traversing the array in canonical order.
		/// </summary>
		/// <param name="maa"> the array to iterate through </param>
		/// <returns> an index iterator for traversing the array in canonical order. </returns>
		/// <seealso cref="IndexIterator"/>
		internal virtual IndexIterator getIndexIterator(NcArray maa)
		{
			if (fastIterator)
			{
				return new IteratorFast(size, maa);
			}
			else
			{
				return new IteratorImpl(this, maa);
			}
		}

		// debugging
		internal virtual IndexIterator getSlowIndexIterator(NcArray maa)
		{
			return new IteratorImpl(this, maa);
		}

		internal virtual IteratorFast getIndexIteratorFast(NcArray maa)
		{
			return new IteratorFast(size, maa);
		}

		internal virtual bool FastIterator
		{
			get
			{
				return fastIterator;
			}
		}


		/// <summary>
		/// Get the total number of elements in the array.
		/// </summary>
		/// <returns> the total number of elements in the array. </returns>
		public virtual long Size
		{
			get
			{
				return size;
			}
		}

		/// <summary>
		/// Get the current element's index into the 1D backing array.
		/// VLEN stops processing.
		/// </summary>
		/// <returns> the current element's index into the 1D backing array. </returns>
		public virtual int currentElement()
		{
			int value = offset; // NB: dont have to check each index again
			for (int ii = 0; ii < rank; ii++)
			{ // general rank
				if (shape[ii] < 0)
				{
					break; // vlen
				}
				value += current[ii] * stride[ii];
			}
			return value;
		}

		/// <summary>
		/// Get the current counter.
		/// </summary>
		/// <returns> copy of the current counter. </returns>
		public virtual int[] CurrentCounter
		{
			get
			{
				return (int[])current.Clone();
			}
		}

		// only use from FastIterator, where the indices are not permuted

		/// <summary>
		/// Set the current counter from the 1D "current element"
		/// currElement = offset + stride[0]*current[0] + ...
		/// </summary>
		/// <param name="currElement"> set to this value </param>
		public virtual void setCurrentCounter(int currElement)
		{
			currElement -= offset;
			for (int ii = 0; ii < rank; ii++)
			{ // general rank
				if (shape[ii] < 0)
				{
					current[ii] = -1;
					break;
				}
				current[ii] = currElement / stride[ii];
				currElement -= current[ii] * stride[ii];
			}
			set(current); // transfer to subclass fields
		}

		/// <summary>
		/// Increment the current element by 1. Used by IndexIterator.
		/// General rank, with subclass specialization. Vlen skipped.
		/// </summary>
		/// <returns> currentElement() </returns>
		public virtual int incr()
		{
			int digit = rank - 1;
			while (digit >= 0)
			{
				if (shape[digit] < 0)
				{
					current[digit] = -1;
					continue;
				} // do not increment vlen
				current[digit]++;
				if (current[digit] < shape[digit])
				{
					break; // normal exit
				}
				current[digit] = 0; // else, carry
				digit--;
			}
			return currentElement();
		}

		/// <summary>
		/// Set the current element's index. General-rank case.
		/// </summary>
		/// <param name="index"> set current value to these values </param>
		/// <returns> this, so you can use A.get(i.set(i)) </returns>
		/// <exception cref="IndexOutOfRangeException"> if index.length != rank. </exception>
		public virtual NcIndex set(int[] index)
		{
			if (index.Length != rank)
			{
				throw new System.IndexOutOfRangeException();
			}
			if (rank == 0)
			{
				return this;
			}
			int prefixrank = (hasvlen ? rank - 1 : rank);
			System.Array.Copy(index, 0, current, 0, prefixrank);
			if (hasvlen)
			{
				current[rank] = -1; // ??
			}
			return this;
		}

		/// <summary>
		/// set current element at dimension dim to v
		/// </summary>
		/// <param name="dim"> set this dimension </param>
		/// <param name="value"> to this value </param>
		public virtual void setDim(int dim, int value)
		{
			if (value < 0 || value >= shape[dim]) // check index here
			{
				throw new System.IndexOutOfRangeException();
			}
			if (shape[dim] >= 0) // !vlen
			{
				current[dim] = value;
			}
		}

		/// <summary>
		/// set current element at dimension 0 to v
		/// </summary>
		/// <param name="v"> set 0th dimension index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i)) </returns>
		public virtual NcIndex set0(int v)
		{
			setDim(0, v);
			return this;
		}

		/// <summary>
		/// set current element at dimension 1 to v
		/// </summary>
		/// <param name="v"> set dimension 1 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i)) </returns>
		public virtual NcIndex set1(int v)
		{
			setDim(1, v);
			return this;
		}

		/// <summary>
		/// set current element at dimension 2 to v
		/// </summary>
		/// <param name="v"> set dimension 2 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i)) </returns>
		public virtual NcIndex set2(int v)
		{
			setDim(2, v);
			return this;
		}

		/// <summary>
		/// set current element at dimension 3 to v
		/// </summary>
		/// <param name="v"> set dimension 3 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i)) </returns>
		public virtual NcIndex set3(int v)
		{
			setDim(3, v);
			return this;
		}

		/// <summary>
		/// set current element at dimension 4 to v
		/// </summary>
		/// <param name="v"> set dimension 4 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i)) </returns>
		public virtual NcIndex set4(int v)
		{
			setDim(4, v);
			return this;
		}

		/// <summary>
		/// set current element at dimension 5 to v
		/// </summary>
		/// <param name="v"> set dimension 5 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i)) </returns>
		public virtual NcIndex set5(int v)
		{
			setDim(5, v);
			return this;
		}

		/// <summary>
		/// set current element at dimension 6 to v
		/// </summary>
		/// <param name="v"> set dimension 6 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i)) </returns>
		public virtual NcIndex set6(int v)
		{
			setDim(6, v);
			return this;
		}

		/// <summary>
		/// set current element at dimension 0 to v0
		/// </summary>
		/// <param name="v0"> set dimension 0 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i)) </returns>
		public virtual NcIndex set(int v0)
		{
			setDim(0, v0);
			return this;
		}

		/// <summary>
		/// set current element at dimension 0,1 to v0,v1
		/// </summary>
		/// <param name="v0"> set dimension 0 index to this value </param>
		/// <param name="v1"> set dimension 1 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i,j)) </returns>
		public virtual NcIndex set(int v0, int v1)
		{
			setDim(0, v0);
			setDim(1, v1);
			return this;
		}

		/// <summary>
		/// set current element at dimension 0,1,2 to v0,v1,v2
		/// </summary>
		/// <param name="v0"> set dimension 0 index to this value </param>
		/// <param name="v1"> set dimension 1 index to this value </param>
		/// <param name="v2"> set dimension 2 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i,j,k)) </returns>
		public virtual NcIndex set(int v0, int v1, int v2)
		{
			setDim(0, v0);
			setDim(1, v1);
			setDim(2, v2);
			return this;
		}

		/// <summary>
		/// set current element at dimension 0,1,2,3 to v0,v1,v2,v3
		/// </summary>
		/// <param name="v0"> set dimension 0 index to this value </param>
		/// <param name="v1"> set dimension 1 index to this value </param>
		/// <param name="v2"> set dimension 2 index to this value </param>
		/// <param name="v3"> set dimension 3 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i,j,k,l)) </returns>
		public virtual NcIndex set(int v0, int v1, int v2, int v3)
		{
			setDim(0, v0);
			setDim(1, v1);
			setDim(2, v2);
			setDim(3, v3);
			return this;
		}

		/// <summary>
		/// set current element at dimension 0,1,2,3,4 to v0,v1,v2,v3,v4
		/// </summary>
		/// <param name="v0"> set dimension 0 index to this value </param>
		/// <param name="v1"> set dimension 1 index to this value </param>
		/// <param name="v2"> set dimension 2 index to this value </param>
		/// <param name="v3"> set dimension 3 index to this value </param>
		/// <param name="v4"> set dimension 4 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i,j,k,l,m)) </returns>
		public virtual NcIndex set(int v0, int v1, int v2, int v3, int v4)
		{
			setDim(0, v0);
			setDim(1, v1);
			setDim(2, v2);
			setDim(3, v3);
			setDim(4, v4);
			return this;
		}

		/// <summary>
		/// set current element at dimension 0,1,2,3,4,5 to v0,v1,v2,v3,v4,v5
		/// </summary>
		/// <param name="v0"> set dimension 0 index to this value </param>
		/// <param name="v1"> set dimension 1 index to this value </param>
		/// <param name="v2"> set dimension 2 index to this value </param>
		/// <param name="v3"> set dimension 3 index to this value </param>
		/// <param name="v4"> set dimension 4 index to this value </param>
		/// <param name="v5"> set dimension 5 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i,j,k,l,m,n)) </returns>
		public virtual NcIndex set(int v0, int v1, int v2, int v3, int v4, int v5)
		{
			setDim(0, v0);
			setDim(1, v1);
			setDim(2, v2);
			setDim(3, v3);
			setDim(4, v4);
			setDim(5, v5);
			return this;
		}

		/// <summary>
		/// set current element at dimension 0,1,2,3,4,5,6 to v0,v1,v2,v3,v4,v5,v6
		/// </summary>
		/// <param name="v0"> set dimension 0 index to this value </param>
		/// <param name="v1"> set dimension 1 index to this value </param>
		/// <param name="v2"> set dimension 2 index to this value </param>
		/// <param name="v3"> set dimension 3 index to this value </param>
		/// <param name="v4"> set dimension 4 index to this value </param>
		/// <param name="v5"> set dimension 5 index to this value </param>
		/// <param name="v6"> set dimension 6 index to this value </param>
		/// <returns> this, so you can use A.get(i.set(i,j,k,l,m,n,p)) </returns>
		public virtual NcIndex set(int v0, int v1, int v2, int v3, int v4, int v5, int v6)
		{
			setDim(0, v0);
			setDim(1, v1);
			setDim(2, v2);
			setDim(3, v3);
			setDim(4, v4);
			setDim(5, v5);
			setDim(6, v6);
			return this;
		}

		/// <summary>
		/// String representation
		/// </summary>
		/// <returns> String representation </returns>
		public virtual string toStringDebug()
		{
			StringBuilder sbuff = new StringBuilder(100);
			sbuff.Length = 0;

			sbuff.Append(" shape= ");
			for (int ii = 0; ii < rank; ii++)
			{
				sbuff.Append(shape[ii]);
				sbuff.Append(" ");
			}

			sbuff.Append(" stride= ");
			for (int ii = 0; ii < rank; ii++)
			{
				sbuff.Append(stride[ii]);
				sbuff.Append(" ");
			}

			sbuff.Append(" offset= ").Append(offset);
			sbuff.Append(" rank= ").Append(rank);
			sbuff.Append(" size= ").Append(size);

			sbuff.Append(" current= ");
			for (int ii = 0; ii < rank; ii++)
			{
				sbuff.Append(current[ii]);
				sbuff.Append(" ");
			}

			return sbuff.ToString();
		}

		public override string ToString()
		{
			StringBuilder sbuff = new StringBuilder(100);
			sbuff.Length = 0;
			for (int ii = 0; ii < rank; ii++)
			{
				if (ii > 0)
				{
					sbuff.Append(",");
				}
				sbuff.Append(current[ii]);
			}
			return sbuff.ToString();
		}

		public virtual object clone()
		{
			NcIndex i = (NcIndex)base.MemberwiseClone();
			i.stride = (int[])stride.Clone();
			i.shape = (int[])shape.Clone();
			i.current = new int[rank]; // want zeros

			return i;
		}

		public virtual object Clone()
		{
			return clone();
		}

		private class IteratorImpl : IndexIterator
		{
			private readonly NcIndex outerInstance;
			private int count;
			private int currElement;
			private NcIndex counter;
			private NcArray maa;

			internal IteratorImpl(NcIndex outerInstance, NcArray maa)
			{
				this.outerInstance = outerInstance;
				this.maa = maa;
				counter = (NcIndex)outerInstance.clone(); // could be subtype of Index
				if (outerInstance.rank > 0)
				{
					counter.current[outerInstance.rank - 1] = -1; // avoid "if first" on every incr.
				}
				counter.precalc();
			}

			public virtual bool hasNext()
			{
				return count < outerInstance.size;
			}

			public override string ToString()
			{
				return counter.ToString();
			}

			public virtual int[] CurrentCounter
			{
				get
				{
					return counter.CurrentCounter;
				}
			}

			public virtual object next()
			{
				count++;
				currElement = counter.incr();
				return maa.getObject(currElement);
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
					count++;
					currElement = counter.incr();
					return maa.getDouble(currElement);
				}
				set
				{
					count++;
					currElement = counter.incr();
					maa.setDouble(currElement, value);
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
					count++;
					currElement = counter.incr();
					return maa.getFloat(currElement);
				}
				set
				{
					count++;
					currElement = counter.incr();
					maa.setFloat(currElement, value);
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
					count++;
					currElement = counter.incr();
					return maa.getLong(currElement);
				}
				set
				{
					count++;
					currElement = counter.incr();
					maa.setLong(currElement, value);
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
					count++;
					currElement = counter.incr();
					return maa.getInt(currElement);
				}
				set
				{
					count++;
					currElement = counter.incr();
					maa.setInt(currElement, value);
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
					count++;
					currElement = counter.incr();
					return maa.getShort(currElement);
				}
				set
				{
					count++;
					currElement = counter.incr();
					maa.setShort(currElement, value);
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
					count++;
					currElement = counter.incr();
					return maa.getByte(currElement);
				}
				set
				{
					count++;
					currElement = counter.incr();
					maa.setByte(currElement, value);
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
					count++;
					currElement = counter.incr();
					return maa.getChar(currElement);
				}
				set
				{
					count++;
					currElement = counter.incr();
					maa.setChar(currElement, value);
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
					count++;
					currElement = counter.incr();
					return maa.getBoolean(currElement);
				}
				set
				{
					count++;
					currElement = counter.incr();
					maa.setBoolean(currElement, value);
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
					count++;
					currElement = counter.incr();
					return maa.getObject(currElement);
				}
				set
				{
					count++;
					currElement = counter.incr();
					maa.setObject(currElement, value);
				}
			}
		}

	}

}
