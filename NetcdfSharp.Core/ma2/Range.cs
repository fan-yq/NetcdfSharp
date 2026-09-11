// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Represents a set of integers, used as an index for arrays.
	/// No duplicates are allowed.
	/// It should be considered as a subset of the interval of integers [first(), last()] inclusive.
	/// For example Range(1:11:3) represents the set of integers {1,4,7,10}
	/// Note that Range no longer is always strided or monotonic.
	/// Immutable.
	/// <para>
	/// Elements must be nonnegative and unique.
	/// EMPTY is the empty Range.
	/// VLEN is for variable length dimensions.
	/// </para>
	/// <para>
	/// Standard iteration is
	/// 
	/// <pre>
	///  for (int idx : range) {
	///    ...
	///  }
	/// </pre>
	/// </para>
	/// </summary>

	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @Immutable public class Range implements RangeIterator
	public class NcRange : RangeIterator
	{
		public static readonly NcRange EMPTY = new NcRange(); // used for unlimited dimension = 0
		public static readonly NcRange ONE = new NcRange("ONE", 1);
		public static readonly NcRange VLEN = new NcRange("VLEN", -1);

		public static NcRange make(string name, int len)
		{
			try
			{
				return new NcRange(name, 0, len - 1, 1);
			}
			catch (InvalidRangeException e)
			{
				throw new Exception(e.Message, e); // cant happen if len > 0
			}
		}

		public static NcRange make(int first, int last)
		{
			try
			{
				return new NcRange(first, last);
			}
			catch (InvalidRangeException e)
			{
				throw new Exception(e.Message, e); // cant happen if len > 0
			}
		}

		////////////////////////////////////////////////////////

		// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private readonly int length_Conflict; // number of elements
											  // JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private readonly int first_Conflict; // first value in range
											 // JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private readonly int last_Conflict; // last value in range, inclusive
											// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private readonly int stride_Conflict; // stride, must be >= 1
		private readonly string name; // optional name

		/// <summary>
		/// Used for EMPTY </summary>
		private NcRange()
		{
			this.length_Conflict = 0;
			this.first_Conflict = 0;
			this.last_Conflict = 0;
			this.stride_Conflict = 1;
			this.name = "EMPTY";
		}

		/// <summary>
		/// Used for ONE, VLEN </summary>
		private NcRange(string name, int length)
		{
			Debug.Assert((length != 0));
			this.name = name;
			this.first_Conflict = 0;
			this.last_Conflict = length - 1;
			this.stride_Conflict = 1;
			this.length_Conflict = length;
		}

		/// <summary>
		/// Create a range with unit stride.
		/// </summary>
		/// <param name="first"> first value in range </param>
		/// <param name="last"> last value in range, inclusive </param>
		/// <exception cref="InvalidRangeException"> elements must be nonnegative, 0 <= first <= last </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Range(int first, int last) throws InvalidRangeException
		public NcRange(int first, int last) : this(null, first, last, 1)
		{
		}

		/// <summary>
		/// Create a range starting at zero, with unit stride.
		/// </summary>
		/// <param name="length"> number of elements in the Range </param>
		public NcRange(int length)
		{
			Debug.Assert((length != 0));
			this.name = null;
			this.first_Conflict = 0;
			this.last_Conflict = length - 1;
			this.stride_Conflict = 1;
			this.length_Conflict = length;
		}

		/// <summary>
		/// Create a named range with unit stride.
		/// </summary>
		/// <param name="name"> name of Range </param>
		/// <param name="first"> first value in range </param>
		/// <param name="last"> last value in range, inclusive </param>
		/// <exception cref="InvalidRangeException"> elements must be nonnegative, 0 <= first <= last </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Range(String name, int first, int last) throws InvalidRangeException
		public NcRange(string name, int first, int last) : this(name, first, last, 1)
		{
		}

		/// <summary>
		/// Create a range with a specified values.
		/// </summary>
		/// <param name="first"> first value in range </param>
		/// <param name="last"> last value in range, inclusive </param>
		/// <param name="stride"> stride between consecutive elements, must be > 0 </param>
		/// <exception cref="InvalidRangeException"> elements must be nonnegative: 0 <= first <= last, stride > 0 </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Range(int first, int last, int stride) throws InvalidRangeException
		public NcRange(int first, int last, int stride) : this(null, first, last, stride)
		{
		}

		/// <summary>
		/// Create a named range with a specified name and values.
		/// </summary>
		/// <param name="name"> name of Range </param>
		/// <param name="first"> first value in range </param>
		/// <param name="last"> last value in range, inclusive </param>
		/// <param name="stride"> stride between consecutive elements, must be > 0 </param>
		/// <exception cref="InvalidRangeException"> elements must be nonnegative: 0 <= first <= last, stride > 0 </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Range(String name, int first, int last, int stride) throws InvalidRangeException
		public NcRange(string name, int first, int last, int stride)
		{
			if (first < 0)
			{
				throw new InvalidRangeException("first (" + first + ") must be >= 0");
			}
			if (last < first)
			{
				throw new InvalidRangeException("last (" + last + ") must be >= first (" + first + ")");
			}
			if (stride < 1)
			{
				throw new InvalidRangeException("stride (" + stride + ") must be > 0");
			}

			this.name = name;
			this.first_Conflict = first;
			this.stride_Conflict = stride;
			this.length_Conflict = 1 + (last - first) / stride;
			this.last_Conflict = first + (this.length_Conflict - 1) * stride;
			Debug.Assert(stride != 1 || this.last_Conflict == last);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: protected Range(String name, int first, int last, int stride, int length) throws InvalidRangeException
		protected internal NcRange(string name, int first, int last, int stride, int length)
		{
			if (first < 0)
			{
				throw new InvalidRangeException("first (" + first + ") must be >= 0");
			}
			if (last < first)
			{
				throw new InvalidRangeException("last (" + last + ") must be >= first (" + first + ")");
			}
			if (stride < 1)
			{
				throw new InvalidRangeException("stride (" + stride + ") must be > 0");
			}
			if (length < (1 + last - first) / stride)
			{
				throw new InvalidRangeException("length (" + length + ") must be > (1 + last - first) / stride");
			}

			this.name = name;
			this.first_Conflict = first;
			this.last_Conflict = last;
			this.stride_Conflict = stride;
			this.length_Conflict = length;
		}

		/// <summary>
		/// Make a copy with a different stride. </summary>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Range copyWithStride(int stride) throws InvalidRangeException
		public virtual NcRange copyWithStride(int stride)
		{
			if (stride == this.stride_Conflict)
			{
				return this;
			}
			return new NcRange(this.first(), this.last(), stride);
		}

		/// @deprecated use copyWithName() 
		[Obsolete("use copyWithName()")]
		public virtual NcRange setName(string name)
		{
			if (name.Equals(this.Name))
			{
				return this;
			}
			try
			{
				return new NcRange(name, first_Conflict, last_Conflict, stride_Conflict, length_Conflict);
			}
			catch (InvalidRangeException e)
			{
				throw new Exception(e.Message, e); // cant happen
			}
		}

		RangeIterator RangeIterator.setName(string name)
		{
			return setName(name);
		}

		/// <summary>
		/// Make a copy with a different name. </summary>
		public virtual NcRange copyWithName(string name)
		{
			if (name.Equals(this.Name))
			{
				return this;
			}
			try
			{
				return new NcRange(name, first_Conflict, last_Conflict, stride_Conflict, length_Conflict);
			}
			catch (InvalidRangeException e)
			{
				throw new Exception(e.Message, e); // cant happen
			}
		}

		RangeIterator RangeIterator.copyWithName(string name)
		{
			return copyWithName(name);
		}

		/// <returns> name, or null if none </returns>
		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		/// <returns> first in range </returns>
		public virtual int first()
		{
			return first_Conflict;
		}

		/// <returns> last in range, inclusive </returns>
		public virtual int last()
		{
			return last_Conflict;
		}

		/// <returns> the number of elements in the range. </returns>
		public virtual int length()
		{
			return length_Conflict;
		}

		/// <returns> stride, must be >= 1 when evenly strided, -1 if not </returns>
		///         // * @deprecated use iterator(), dont assume evenly strided 
		public virtual int stride()
		{
			return stride_Conflict;
		}

		/////////////////////////////////////////////

		/// <summary>
		/// Is want contained in this Range?
		/// </summary>
		/// <param name="want"> index in the original Range </param>
		/// <returns> true if the ith element would be returned by the Range iterator </returns>
		public virtual bool contains(int want)
		{
			if (want < first())
			{
				return false;
			}
			if (want > last())
			{
				return false;
			}
			if (stride_Conflict == 1)
			{
				return true;
			}
			return (want - first_Conflict) % stride_Conflict == 0;
		}

		/// <summary>
		/// Create a new Range by composing a Range that is relative to this Range.
		/// Revised 2013/04/19 by Dennis Heimbigner to handle edge cases.
		/// See the commentary associated with the netcdf-c file dceconstraints.h,
		/// function dceslicecompose().
		/// </summary>
		/// <param name="r"> range relative to base </param>
		/// <returns> combined Range, may be EMPTY </returns>
		/// <exception cref="InvalidRangeException"> elements must be nonnegative, 0 <= first <= last </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Range compose(Range r) throws InvalidRangeException
		public virtual NcRange compose(NcRange r)
		{
			if ((length() == 0) || (r.length() == 0))
			{
				return EMPTY;
			}
			if (this == VLEN || r == VLEN)
			{
				return VLEN;
			}
			/*
			 * if(false) {// Original version
			 * // Note that this version assumes that range r is
			 * // correct with respect to this.
			 * int first = element(r.first());
			 * int stride = stride() * r.stride();
			 * int last = element(r.last());
			 * return new Range(name, first, last, stride);
			 * } else {//new version: handles versions all values of r.
			 */
			int sr_stride = this.stride_Conflict * r.stride_Conflict;
			int sr_first = element(r.first()); // MAP(this,i) == element(i)
			int lastx = element(r.last());
			int sr_last = Math.Min(last(), lastx);
			// unused int sr_length = (sr_last + 1) - sr_first;
			return new NcRange(name, sr_first, sr_last, sr_stride);
		}

		/// <summary>
		/// Create a new Range by compacting this Range by removing the stride.
		/// first = first/stride, last=last/stride, stride=1.
		/// </summary>
		/// <returns> compacted Range </returns>
		/// <exception cref="InvalidRangeException"> elements must be nonnegative, 0 <= first <= last </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Range compact() throws InvalidRangeException
		public virtual NcRange compact()
		{
			if (stride_Conflict == 1)
			{
				return this;
			}
			int first = this.first() / stride_Conflict; // LOOK WTF ?
			int last = first + length() - 1;
			return new NcRange(name, first, last, 1);
		}

		/// <summary>
		/// Get ith element
		/// </summary>
		/// <param name="i"> index of the element </param>
		/// <returns> the i-th element of a range. </returns>
		/// <exception cref="InvalidRangeException"> i must be: 0 <= i < length </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public int element(int i) throws InvalidRangeException
		public virtual int element(int i)
		{
			if (i < 0)
			{
				throw new InvalidRangeException("i must be >= 0");
			}
			if (i >= length_Conflict)
			{
				throw new InvalidRangeException("i must be < length");
			}

			return first_Conflict + i * stride_Conflict;
		}

		/// <summary>
		/// Get the index for this element: inverse of element
		/// </summary>
		/// <param name="want"> the element of the range </param>
		/// <returns> index </returns>
		/// <exception cref="InvalidRangeException"> if illegal elem </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public int index(int want) throws InvalidRangeException
		public virtual int index(int want)
		{
			if (want < first_Conflict)
			{
				throw new InvalidRangeException("elem must be >= first");
			}
			int result = (want - first_Conflict) / stride_Conflict;
			if (result > length_Conflict)
			{
				throw new InvalidRangeException("elem must be <= first = n * stride");
			}
			return result;
		}

		/// <summary>
		/// Create a new Range by intersecting with a Range using same interval as this Range.
		/// NOTE: we dont yet support intersection when both Ranges have strides
		/// </summary>
		/// <param name="r"> range to intersect </param>
		/// <returns> intersected Range, may be EMPTY </returns>
		/// <exception cref="InvalidRangeException"> elements must be nonnegative </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Range intersect(Range r) throws InvalidRangeException
		public virtual NcRange intersect(NcRange r)
		{
			if ((length() == 0) || (r.length() == 0))
			{
				return EMPTY;
			}
			if (this == VLEN || r == VLEN)
			{
				return VLEN;
			}

			int last = Math.Min(this.last(), r.last());
			int resultStride = stride_Conflict * r.stride();

			int useFirst;
			if (resultStride == 1)
			{ // both strides are 1
				useFirst = Math.Max(this.first(), r.first());

			}
			else if (stride_Conflict == 1)
			{ // then r has a stride

				if (r.first() >= first())
				{
					useFirst = r.first();
				}
				else
				{
					int incr = (first() - r.first()) / resultStride;
					useFirst = r.first() + incr * resultStride;
					if (useFirst < first())
					{
						useFirst += resultStride;
					}
				}

			}
			else if (r.stride_Conflict == 1)
			{ // then this has a stride

				if (first() >= r.first())
				{
					useFirst = first();
				}
				else
				{
					int incr = (r.first() - first()) / resultStride;
					useFirst = first() + incr * resultStride;
					if (useFirst < r.first())
					{
						useFirst += resultStride;
					}
				}

			}
			else
			{
				throw new System.NotSupportedException("Intersection when both ranges have a stride");
			}

			if (useFirst > last)
			{
				return EMPTY;
			}
			return new NcRange(name, useFirst, last, resultStride);
		}

		/// <summary>
		/// Determine if a given Range intersects this one.
		/// NOTE: we dont yet support intersection when both Ranges have strides
		/// </summary>
		/// <param name="r"> range to intersect </param>
		/// <returns> true if they intersect </returns>
		/// <exception cref="UnsupportedOperationException"> if both Ranges have strides </exception>
		public virtual bool intersects(NcRange r)
		{
			if ((length() == 0) || (r.length() == 0))
			{
				return false;
			}
			if (this == VLEN || r == VLEN)
			{
				return true;
			}

			int last = Math.Min(this.last(), r.last());
			int resultStride = stride_Conflict * r.stride();

			int useFirst;
			if (resultStride == 1)
			{ // both strides are 1
				useFirst = Math.Max(this.first(), r.first());

			}
			else if (stride_Conflict == 1)
			{ // then r has a stride

				if (r.first() >= first())
				{
					useFirst = r.first();
				}
				else
				{
					int incr = (first() - r.first()) / resultStride;
					useFirst = r.first() + incr * resultStride;
					if (useFirst < first())
					{
						useFirst += resultStride;
					}
				}

			}
			else if (r.stride() == 1)
			{ // then this has a stride

				if (first() >= r.first())
				{
					useFirst = first();
				}
				else
				{
					int incr = (r.first() - first()) / resultStride;
					useFirst = first() + incr * resultStride;
					if (useFirst < r.first())
					{
						useFirst += resultStride;
					}
				}

			}
			else
			{
				throw new System.NotSupportedException("Intersection when both ranges have a stride");
			}

			return (useFirst <= last);
		}

		/// <summary>
		/// If this range is completely past the wanted range
		/// </summary>
		/// <param name="want"> desired range </param>
		/// <returns> true if first() > want.last() </returns>
		public virtual bool past(NcRange want)
		{
			return (first() > want.last());
		}

		/// <summary>
		/// Create a new Range shifting this range by a constant factor.
		/// </summary>
		/// <param name="origin"> subtract this from each element </param>
		/// <returns> shifted range </returns>
		/// <exception cref="InvalidRangeException"> elements must be nonnegative, 0 <= first <= last </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Range shiftOrigin(int origin) throws InvalidRangeException
		public virtual NcRange shiftOrigin(int origin)
		{
			if (this == VLEN)
			{
				return VLEN;
			}

			int first = this.first() - origin;
			int last = this.last() - origin;
			return new NcRange(name, first, last, stride_Conflict);
		}

		/// <summary>
		/// Create a new Range by making the union with a Range using same interval as this Range.
		/// NOTE: no strides
		/// </summary>
		/// <param name="r"> range to add </param>
		/// <returns> intersected Range, may be EMPTY </returns>
		/// <exception cref="InvalidRangeException"> elements must be nonnegative </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Range union(Range r) throws InvalidRangeException
		public virtual NcRange union(NcRange r)
		{
			if (length() == 0)
			{
				return r;
			}
			if (this == VLEN || r == VLEN)
			{
				return VLEN;
			}

			if (r.length() == 0)
			{
				return this;
			}

			int first = Math.Min(this.first(), r.first());
			int last = Math.Max(this.last(), r.last());
			return new NcRange(name, first, last);
		}

		/// <summary>
		/// Find the first element in a strided array after some index start.
		/// Return the smallest element k in the Range, such that
		/// <ul>
		/// <li>k >= first
		/// <li>k >= start
		/// <li>k <= last
		/// <li>k = element of this Range
		/// </ul>
		/// </summary>
		/// <param name="start"> starting index </param>
		/// <returns> first in interval, else -1 if there is no such element. </returns>
		public virtual int getFirstInInterval(int start)
		{
			if (start > last())
			{
				return -1;
			}
			if (start <= first_Conflict)
			{
				return first_Conflict;
			}
			if (stride_Conflict == 1)
			{
				return start;
			}
			int offset = start - first_Conflict;
			int i = offset / stride_Conflict;
			i = (offset % stride_Conflict == 0) ? i : i + 1; // round up
			return first_Conflict + i * stride_Conflict;
		}

		public override string ToString()
		{
			if (this.length_Conflict == 0)
			{
				return ":"; // EMPTY
			}
			else if (this.length_Conflict < 0)
			{
				return ":"; // VLEN
			}
			else
			{
				return first_Conflict + ":" + last() + (stride_Conflict > 1 ? ":" + stride_Conflict : "");
			}
		}

		/// <summary>
		/// Range elements with same first, last, stride are equal.
		/// </summary>
		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (!(o is NcRange))
			{
				return false; // this catches nulls
			}
			NcRange or = (NcRange)o;

			if ((length_Conflict == 0) && (or.length_Conflict == 0))
			{ // empty ranges are equal
				return true;
			}

			return (or.first_Conflict == first_Conflict) && (or.length_Conflict == length_Conflict) && (or.stride_Conflict == stride_Conflict) && (or.last_Conflict == last_Conflict);
		}

		/// <summary>
		/// Override Object.hashCode() to implement equals.
		/// </summary>
		public override int GetHashCode()
		{
			int result = first_Conflict;
			result = 37 * result + last_Conflict;
			result = 37 * result + stride_Conflict;
			result = 37 * result + length_Conflict;
			return result;
		}

		/////////////////////////////////////////////////////////

		/// <summary>
		/// Iterate over Range index
		/// </summary>
		/// <returns> Iterator over element indices </returns>
		/// @deprecated use iterator() or foreach 
		[Obsolete("use iterator() or foreach")]
		public virtual IEnumerator<int> getIterator()
		{
			return new MyIterator(this);
		}

		public virtual IEnumerator<int> iterator()
		{
			return new MyIterator(this);
		}

		public virtual IEnumerator<int> GetEnumerator()
		{
			return new MyIterator(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private class MyIterator : IEnumerator<int>
		{
			private readonly NcRange outerInstance;

			internal int current = -1;

			public MyIterator(NcRange outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual bool hasNext()
			{
				return current < outerInstance.length_Conflict - 1;
			}

			public virtual int next()
			{
				current++;
				return outerInstance.elementNC(current);
			}

			public virtual bool MoveNext()
			{
				current++;
				return current < outerInstance.length_Conflict;
			}

			public virtual int Current
			{
				get
				{
					return outerInstance.elementNC(current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public virtual void Reset()
			{
				current = -1;
			}

			public virtual void Dispose()
			{
			}
		}

		/// <summary>
		/// Get ith element; skip checking, for speed.
		/// </summary>
		private int elementNC(int i)
		{
			return first_Conflict + i * stride_Conflict;
		}

	}

}
