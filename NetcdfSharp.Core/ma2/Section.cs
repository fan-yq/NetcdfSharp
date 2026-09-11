// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// A section of multidimensional array indices.
	/// Represented as List<Range>.
	/// Immutable if makeImmutable() was called.
	/// TODO Will be immutable in ver6.
	/// TODO evaluate use of null in ver7
	/// 
	/// @author caron
	/// </summary>
	public class Section
	{

		/// <summary>
		/// Return a Section guaranteed to be non null, with no null Ranges, and within the bounds set by shape.
		/// A section with no nulls is called "filled".
		/// If its is already filled, return it, otherwise return a new Section, filled from the shape.
		/// </summary>
		/// <param name="s"> the original Section, may be null or not filled </param>
		/// <param name="shape"> use this as default shape if any of the ranges are null. </param>
		/// <returns> a filled Section </returns>
		/// <exception cref="InvalidRangeException"> if shape and s and shape rank dont match, or if s has invalid range compared to shape </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static Section fill(Section s, int[] shape) throws InvalidRangeException
		public static Section fill(Section s, int[] shape)
		{
			// want all
			if (s == null)
			{
				return new Section(shape);
			}

			string errs = s.checkInRange(shape);
			if (!string.ReferenceEquals(errs, null))
			{
				throw new InvalidRangeException(errs);
			}

			// if s is already filled, use it
			bool ok = true;
			for (int i = 0; i < shape.Length; i++)
			{
				ok &= (s.getRange(i) != null);
			}
			if (ok)
			{
				return s;
			}

			// fill in any nulls
			return new Section(s.Ranges, shape);
		}

		public static Section make(IList<RangeIterator> rangeIter)
		{
			IList<NcRange> ranges = new List<NcRange>();
			foreach (RangeIterator iter in rangeIter)
			{
				if (iter is NcRange)
				{
					ranges.Add((NcRange)iter);
				}
				else
				{
					throw new ArgumentException("Can deal with non Range yet");
				}
			}
			return new Section(ranges);
		}

		//////////////////////////////////////////////////////////////////////////////
		private IList<NcRange> list; // TODO make Immutable List in ver6
		private bool immutable;

		/// <summary>
		/// Create Section from a shape array, assumes 0 origin.
		/// </summary>
		/// <param name="shape"> array of lengths for each Range. 0 = EMPTY, < 0 = VLEN </param>
		public Section(int[] shape)
		{
			list = new List<NcRange>();
			foreach (int aShape in shape)
			{
				if (aShape > 0)
				{
					list.Add(new NcRange(aShape));
				}
				else if (aShape == 0)
				{
					list.Add(NcRange.EMPTY);
				}
				else
				{
					list.Add(NcRange.VLEN);
				}
			}
		}

		/// <summary>
		/// Create Section from a shape and origin arrays.
		/// </summary>
		/// <param name="origin"> array of start for each Range </param>
		/// <param name="shape"> array of lengths for each Range </param>
		/// <exception cref="InvalidRangeException"> if origin < 0, or shape < 1. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Section(int[] origin, int[] shape) throws InvalidRangeException
		public Section(int[] origin, int[] shape)
		{
			list = new List<NcRange>();
			for (int i = 0; i < shape.Length; i++)
			{
				if (shape[i] > 0)
				{
					list.Add(new NcRange(origin[i], origin[i] + shape[i] - 1));
				}
				else if (shape[i] == 0)
				{
					list.Add(NcRange.EMPTY);
				}
				else
				{
					list.Add(NcRange.VLEN);
				}
			}
		}

		/// <summary>
		/// Create Section from a shape, origin, and stride arrays.
		/// </summary>
		/// <param name="origin"> array of start for each Range </param>
		/// <param name="size"> array of lengths for each Range (last = origin + size -1) </param>
		/// <param name="stride"> stride between consecutive elements, must be > 0 </param>
		/// <exception cref="InvalidRangeException"> if origin < 0, or shape < 1. </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Section(int[] origin, int[] size, int[] stride) throws InvalidRangeException
		public Section(int[] origin, int[] size, int[] stride)
		{
			list = new List<NcRange>();
			for (int i = 0; i < size.Length; i++)
			{
				if (size[i] > 0)
				{
					list.Add(new NcRange(origin[i], origin[i] + size[i] - 1, stride[i]));
				}
				else if (size[i] == 0)
				{
					list.Add(NcRange.EMPTY);
				}
				else
				{
					list.Add(NcRange.VLEN);
				}
			}
		}

		/// <summary>
		/// Create Section from a List<Range>.
		/// </summary>
		/// <param name="from"> the list of Range </param>
		public Section(IList<NcRange> from)
		{
			list = new List<NcRange>(from);
		}

		/// <summary>
		/// Create Section from a variable length list of Ranges
		/// </summary>
		/// <param name="ranges"> the list </param>
		public Section(params NcRange[] ranges)
		{
			list = new List<NcRange>();
			Collections.addAll(list, ranges);
		}

		/// <summary>
		/// Copy Constructor.
		/// Returned copy is mutable
		/// </summary>
		/// <param name="from"> the Section to copy </param>
		public Section(Section from)
		{
			list = new List<NcRange>(from.Ranges);
		}

		/// <summary>
		/// Create Section from a List<Range>.
		/// </summary>
		/// <param name="from"> the list of Range </param>
		/// <param name="shape"> use this as default shape if any of the ranges are null. </param>
		/// <exception cref="InvalidRangeException"> if shape and range list dont match </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Section(List<Range> from, int[] shape) throws InvalidRangeException
		public Section(IList<NcRange> from, int[] shape)
		{
			list = new List<NcRange>(from);
			Defaults = shape;
		}

		/// <summary>
		/// Parse an index section String specification, return equivilent Section.
		/// A null Range means "all" (i.e.":") indices in that dimension.
		/// <p/>
		/// The sectionSpec string uses fortran90 array section syntax, namely:
		/// 
		/// <pre>
		///   sectionSpec := dims
		///   dims := dim | dim, dims
		///   dim := ':' | slice | start ':' end | start ':' end ':' stride
		///   slice := INTEGER
		///   start := INTEGER
		///   stride := INTEGER
		///   end := INTEGER
		/// 
		/// where nonterminals are in lower case, terminals are in upper case, literals are in single quotes.
		/// 
		/// Meaning of index selector :
		///  ':' = all
		///  slice = hold index to that value
		///  start:end = all indices from start to end inclusive
		///  start:end:stride = all indices from start to end inclusive with given stride
		/// 
		/// </pre>
		/// </summary>
		/// <param name="sectionSpec"> the token to parse, eg "(1:20,:,3,10:20:2)", parenthesis optional </param>
		/// <exception cref="InvalidRangeException"> when the Range is illegal </exception>
		/// <exception cref="IllegalArgumentException"> when sectionSpec is misformed </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Section(String sectionSpec) throws InvalidRangeException
		public Section(string sectionSpec)
		{

			list = new List<NcRange>();
			NcRange range;

			StringTokenizer stoke = new StringTokenizer(sectionSpec, "(),"); // LOOK deal with scatterRange {1,2,3}
			while (stoke.hasMoreTokens())
			{
				string s = stoke.nextToken().Trim();
				if (s.Equals(":"))
				{
					range = null; // all
				}

				else if (s.IndexOf(':') < 0)
				{ // just a number : slice
					try
					{
						int index = int.Parse(s);
						range = new NcRange(index, index);
					}
					catch (System.FormatException)
					{
						throw new ArgumentException(" illegal selector: " + s + " part of <" + sectionSpec + ">");
					}

				}
				else
				{ // gotta be "start : end" or "start : end : stride"
					StringTokenizer stoke2 = new StringTokenizer(s, ":");
					string s1 = stoke2.nextToken();
					string s2 = stoke2.nextToken();
					string s3 = stoke2.hasMoreTokens() ? stoke2.nextToken() : null;
					try
					{
						int index1 = int.Parse(s1);
						int index2 = int.Parse(s2);
						int stride = (!string.ReferenceEquals(s3, null)) ? int.Parse(s3) : 1;
						range = new NcRange(index1, index2, stride);
					}
					catch (System.FormatException)
					{
						throw new ArgumentException(" illegal selector: " + s + " part of <" + sectionSpec + ">");
					}
				}
				list.Add(range);
			}

		}

		/// <summary>
		/// Create a new Section by compacting each Range.
		/// first = first/stride, last=last/stride, stride=1.
		/// </summary>
		/// <returns> compacted Section </returns>
		/// <exception cref="InvalidRangeException"> elements must be nonnegative, 0 <= first <= last </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Section compact() throws InvalidRangeException
		public virtual Section compact()
		{
			IList<NcRange> results = new List<NcRange>(Rank);
			foreach (NcRange r in list)
			{
				results.Add(r.compact());
			}
			return new Section(results);
		}


		/*
		 * Create a new Section by compacting each Range.
		 * first = first/stride, last=last/stride, stride=1.
		 *
		 * @return compacted Section
		 * 
		 * @throws InvalidRangeException elements must be nonnegative, 0 <= first <= last
		 *
		 * public Section removeVlen() throws InvalidRangeException {
		 * boolean need = false;
		 * for (Range r : list) {
		 * if (r == Range.VLEN) need = true;
		 * }
		 * if (!need) return this;
		 * 
		 * List<Range> results = new ArrayList<>(getRank());
		 * for (Range r : list) {
		 * if (r != Range.VLEN) results.add(r);
		 * }
		 * return new Section(results);
		 * }
		 */


		/// <summary>
		/// Create a new Section by composing with a Section that is relative to this Section.
		/// </summary>
		/// <param name="want"> Section relative to this one. If null, return this. If individual ranges are null, use corresponding
		///        Range in this. </param>
		/// <returns> new Section, composed </returns>
		/// <exception cref="InvalidRangeException"> if want.getRank() not equal to this.getRank(), or invalid component Range </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Section compose(Section want) throws InvalidRangeException
		public virtual Section compose(Section want)
		{
			// all nulls
			if (want == null)
			{
				return this; // LOOK maybe a copy ??
			}

			if (want.Rank != Rank)
			{
				throw new InvalidRangeException("Invalid Section rank");
			}

			// check individual nulls
			IList<NcRange> results = new List<NcRange>(Rank);
			for (int j = 0; j < list.Count; j++)
			{
				NcRange @base = list[j];
				NcRange r = want.getRange(j);

				if (r == null)
				{
					results.Add(@base);
				}
				else
				{
					results.Add(@base.compose(r));
				}
			}

			return new Section(results);
		}

		/// <summary>
		/// Create a new Section by intersection with another Section
		/// </summary>
		/// <param name="other"> Section other section </param>
		/// <returns> new Section, composed </returns>
		/// <exception cref="InvalidRangeException"> if want.getRank() not equal to this.getRank(), or invalid component Range </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Section intersect(Section other) throws InvalidRangeException
		public virtual Section intersect(Section other)
		{
			if (!compatibleRank(other))
			{
				throw new InvalidRangeException("Invalid Section rank");
			}

			// check individual nulls
			IList<NcRange> results = new List<NcRange>(Rank);
			for (int j = 0; j < list.Count; j++)
			{
				NcRange @base = list[j];
				NcRange r = other.getRange(j);
				results.Add(@base.intersect(r));
			}

			return new Section(results);
		}

		/// <summary>
		/// Compute the element offset of an intersecting subrange of this.
		/// </summary>
		/// <param name="intersect"> the subrange </param>
		/// <returns> element offset </returns>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public int offset(Section intersect) throws InvalidRangeException
		public virtual int offset(Section intersect)
		{
			if (!compatibleRank(intersect))
			{
				throw new InvalidRangeException("Incompatible Section rank");
			}

			int result = 0;
			int stride = 1;
			for (int j = list.Count - 1; j >= 0; j--)
			{
				NcRange @base = list[j];
				NcRange r = intersect.getRange(j);
				int offset = @base.index(r.first());
				result += offset * stride;
				stride *= @base.length();
			}

			return result;
		}

		/// <summary>
		/// Create a new Section by union with another Section
		/// </summary>
		/// <param name="other"> Section other section </param>
		/// <returns> new Section, union of the two </returns>
		/// <exception cref="InvalidRangeException"> if want.getRank() not equal to this.getRank(), or invalid component Range </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Section union(Section other) throws InvalidRangeException
		public virtual Section union(Section other)
		{
			if (other.Rank != Rank)
			{
				throw new InvalidRangeException("Invalid Section rank");
			}

			IList<NcRange> results = new List<NcRange>(Rank);
			for (int j = 0; j < list.Count; j++)
			{
				NcRange @base = list[j];
				NcRange r = other.getRange(j);
				results.Add(@base.union(r));
			}

			return new Section(results);
		}

		/// <summary>
		/// Create a new Section by shifting each range by newOrigin.first()
		/// The result is then a relative offset from the newOrigin.
		/// </summary>
		/// <param name="newOrigin"> this becomes the origin of the result </param>
		/// <returns> new Section, shifted </returns>
		/// <exception cref="InvalidRangeException"> if want.getRank() not equal to this.getRank() </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Section shiftOrigin(Section newOrigin) throws InvalidRangeException
		public virtual Section shiftOrigin(Section newOrigin)
		{
			if (newOrigin.Rank != Rank)
			{
				throw new InvalidRangeException("Invalid Section rank");
			}

			// check individual nulls
			IList<NcRange> results = new List<NcRange>(Rank);
			for (int j = 0; j < list.Count; j++)
			{
				NcRange @base = list[j];
				NcRange r = newOrigin.getRange(j);
				results.Add(@base.shiftOrigin(r.first()));
			}

			return new Section(results);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Section shiftOrigin(int[] newOrigin) throws InvalidRangeException
		public virtual Section shiftOrigin(int[] newOrigin)
		{
			if (newOrigin.Length != Rank)
			{
				throw new InvalidRangeException("Invalid Section rank");
			}

			IList<NcRange> results = new List<NcRange>(Rank);
			for (int j = 0; j < list.Count; j++)
			{
				NcRange @base = list[j];
				results.Add(@base.shiftOrigin(-newOrigin[j]));
			}

			return new Section(results);
		}

		/// <summary>
		/// See if this Section intersects with another Section. ignores strides, vlen
		/// </summary>
		/// <param name="other"> another section </param>
		/// <returns> true if intersection is non-empty </returns>
		/// <exception cref="InvalidRangeException"> if want.getRank() not equal to this.getRank(),
		///  </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public boolean intersects(Section other) throws InvalidRangeException
		public virtual bool intersects(Section other)
		{
			if (!compatibleRank(other))
			{
				throw new InvalidRangeException("Invalid Section rank");
			}

			for (int j = 0; j < list.Count; j++)
			{
				NcRange @base = list[j];
				NcRange r = other.getRange(j);
				if (@base == NcRange.VLEN || r == NcRange.VLEN)
				{
					continue;
				}
				if (!@base.intersects(r))
				{
					return false;
				}
			}

			return true;
		}


		/// <summary>
		/// See if this Section contains another Section. ignores strides
		/// Must have same rank and last >= other.last.
		/// </summary>
		/// <param name="other"> another section </param>
		/// <returns> true if its a subset </returns>
		public virtual bool contains(Section other)
		{
			if (other.Rank != Rank)
			{
				return false;
			}

			for (int j = 0; j < list.Count; j++)
			{
				NcRange @base = list[j];
				NcRange r = other.getRange(j);
				if (@base.first() > r.first())
				{
					return false;
				}
				if (@base.last() < r.last())
				{
					return false;
				}
			}

			return true;
		}


		/// <summary>
		/// Convert List of Ranges to String Spec.
		/// Inverse of new Section(String sectionSpec)
		/// </summary>
		/// <returns> index section String specification </returns>
		public override string ToString()
		{
			Formatter sbuff = new Formatter();
			for (int i = 0; i < list.Count; i++)
			{
				NcRange r = list[i];
				if (i > 0)
				{
					sbuff.format(",");
				}
				if (r == null)
				{
					sbuff.format(":");
				}
				else
				{
					sbuff.format("%s", r.ToString());
				}
			}
			return sbuff.ToString();
		}

		public virtual string show()
		{
			Formatter sbuff = new Formatter();
			for (int i = 0; i < list.Count; i++)
			{
				NcRange r = list[i];
				if (i > 0)
				{
					sbuff.format("%n");
				}
				if (r == null)
				{
					sbuff.format(":");
				}
				else
				{
					if (!string.ReferenceEquals(r.Name, null))
					{
						sbuff.format("%s=", r.Name);
					}
					sbuff.format("%s", r.ToString());
				}
			}
			return sbuff.ToString();
		}

		/// <summary>
		/// No-arg Constructor
		/// </summary>
		public Section()
		{
			list = new List<NcRange>();
		}

		///////////////////////////////////////////////////////
		// these make it mutable

		/// <summary>
		/// Append a null Range to the Section - meaning "all"
		/// </summary>
		/// <returns> this </returns>
		/// @deprecated use builder().appendNullRange() 
		[Obsolete("use builder().appendNullRange()")]
		public virtual Section appendRange()
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			list.Add(null);
			return this;
		}

		/// <summary>
		/// Append a Range to the Section
		/// </summary>
		/// <returns> this </returns>
		/// @deprecated use builder().appendRange(r) 
		[Obsolete("use builder().appendRange(r)")]
		public virtual Section appendRange(NcRange r)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			list.Add(r);
			return this;
		}

		/// <summary>
		/// Append a new Range(0,size-1) to the Section
		/// </summary>
		/// <param name="size"> add this Range </param>
		/// <returns> this </returns>
		/// @deprecated use builder().appendRange(size) 
		[Obsolete("use builder().appendRange(size)")]
		public virtual Section appendRange(int size)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			if (size > 0)
			{
				list.Add(new NcRange(size));
			}
			else if (size == 0)
			{
				list.Add(NcRange.EMPTY);
			}
			else
			{
				list.Add(NcRange.VLEN);
			}
			return this;
		}

		/// <summary>
		/// Append a new Range(first, last) to the Section
		/// </summary>
		/// <param name="first"> starting index </param>
		/// <param name="last"> last index, inclusive </param>
		/// <returns> this </returns>
		/// <exception cref="InvalidRangeException"> if last < first </exception>
		/// @deprecated use builder().appendRange 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use builder().appendRange") public Section appendRange(int first, int last) throws InvalidRangeException
		[Obsolete("use builder().appendRange")]
		public virtual Section appendRange(int first, int last)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			if (last < 0)
			{
				list.Add(NcRange.VLEN);
			}
			else
			{
				list.Add(new NcRange(first, last));
			}
			return this;
		}

		/// <summary>
		/// Append a new Range(first,last,stride) to the Section
		/// </summary>
		/// <param name="first"> starting index </param>
		/// <param name="last"> last index, inclusive </param>
		/// <param name="stride"> stride </param>
		/// <returns> this </returns>
		/// <exception cref="InvalidRangeException"> if last < first </exception>
		/// @deprecated use builder().appendRange 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use builder().appendRange") public Section appendRange(int first, int last, int stride) throws InvalidRangeException
		[Obsolete("use builder().appendRange")]
		public virtual Section appendRange(int first, int last, int stride)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			list.Add(new NcRange(first, last, stride));
			return this;
		}

		/// <summary>
		/// Append a new Range(name,first,last,stride) to the Section
		/// </summary>
		/// <param name="name"> name of Range </param>
		/// <param name="first"> starting index </param>
		/// <param name="last"> last index, inclusive </param>
		/// <param name="stride"> stride </param>
		/// <returns> this </returns>
		/// <exception cref="InvalidRangeException"> if last < first </exception>
		/// @deprecated use builder().appendRange 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use builder().appendRange") public Section appendRange(String name, int first, int last, int stride) throws InvalidRangeException
		[Obsolete("use builder().appendRange")]
		public virtual Section appendRange(string name, int first, int last, int stride)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			list.Add(new NcRange(name, first, last, stride));
			return this;
		}

		/// <summary>
		/// Insert a range at the specified index in the list.
		/// </summary>
		/// <param name="index"> insert here in the list, existing ranges at or after this index get shifted by one </param>
		/// <param name="r"> insert this Range </param>
		/// <returns> this </returns>
		/// <exception cref="IndexOutOfRangeException"> if bad index </exception>
		/// @deprecated use builder().insertRange 
		[Obsolete("use builder().insertRange")]
		public virtual Section insertRange(int index, NcRange r)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			list.Insert(index, r);
			return this;
		}

		/// <summary>
		/// Remove a range at the specified index in the list.
		/// </summary>
		/// <param name="index"> remove here in the list, existing ranges after this index get shifted by one </param>
		/// <returns> this </returns>
		/// <exception cref="IndexOutOfRangeException"> if bad index </exception>
		/// @deprecated use builder().removeRange 
		[Obsolete("use builder().removeRange")]
		public virtual Section removeRange(int index)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			list.RemoveAt(index);
			return this;
		}

		/// <summary>
		/// Set the range at the specified index in the list, previous Range is discarded
		/// </summary>
		/// <param name="index"> list index, must be in interval [0,size). </param>
		/// <param name="r"> insert this Range </param>
		/// <returns> this </returns>
		/// <exception cref="IndexOutOfRangeException"> if bad index </exception>
		/// @deprecated use builder().setRange 
		[Obsolete("use builder().setRange")]
		public virtual Section setRange(int index, NcRange r)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			list[index] = r;
			return this;
		}

		/// <summary>
		/// Replace a range at the specified index in the list.
		/// </summary>
		/// <param name="index"> replace here in the list. </param>
		/// <param name="r"> use this Range </param>
		/// <returns> this </returns>
		/// <exception cref="IndexOutOfRangeException"> if bad index </exception>
		/// @deprecated use builder().replaceRange 
		[Obsolete("use builder().replaceRange")]
		public virtual Section replaceRange(int index, NcRange r)
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			list[index] = r;
			return this;
		}

		/// <summary>
		/// Remove any ranges of length 1
		/// </summary>
		/// <returns> new Section if any Ranges needed replacement, else this </returns>
		public virtual Section reduce()
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
			bool needed = false;
			foreach (NcRange r in list)
			{
				if (r.length() == 1)
				{
					needed = true;
				}
			}
			if (!needed)
			{
				return this;
			}

			IList<NcRange> newList = new List<NcRange>();
			foreach (NcRange r in list)
			{
				if (r.length() > 1)
				{
					newList.Add(r);
				}
			}
			return new Section(newList);
		}

		/// <summary>
		/// If any of the ranges are null, which means "all", set the Range from the
		/// corresponding length in shape[].
		/// </summary>
		/// <param name="shape"> default length for each Range; must have matching rank. </param>
		public virtual int[] Defaults
		{
			set
			{
				if (immutable)
				{
					throw new System.InvalidOperationException("Cant modify");
				}
				if (value.Length != list.Count)
				{
					throw new InvalidRangeException(" shape[] must have same rank as Section");
				}

				// check that any individual Range is null
				for (int i = 0; i < value.Length; i++)
				{
					NcRange r = list[i];
					if (r == null)
					{
						if (value[i] > 0)
						{
							list[i] = new NcRange(value[i]);
						}
						else if (value[i] == 0)
						{
							list[i] = NcRange.EMPTY;
						}
						else
						{
							list[i] = NcRange.VLEN;
						}
					}
				}
			}
		}

		/// <summary>
		/// Makes the object immutable, so can be safely shared
		/// </summary>
		/// <returns> this Section </returns>
		/// @deprecated use builder() 
		[Obsolete("use builder()")]
		public virtual Section makeImmutable()
		{
			if (immutable)
			{
				return this;
			}
			immutable = true;
			list = new System.Collections.ObjectModel.ReadOnlyCollection<NcRange>(list);
			return this;
		}

		/////////////////////////////////// end mutable methods
		// replace with these:

		public virtual Section subSection(int fromIndex, int endExclusive)
		{
			IList<NcRange> sub = new List<NcRange>();
			for (int i = fromIndex; i < endExclusive; i++)
			{
				sub.Add(list[i]);
			}
			return new Section(sub);
		}

		/// @deprecated use builder().removeLast() 
		[Obsolete("use builder().removeLast()")]
		public virtual Section removeLast()
		{
			int size = list.Count;
			return subSection(size - 2, size - 1);
		}

		/// @deprecated use builder().removeVlen() 
		[Obsolete("use builder().removeVlen()")]
		public virtual Section removeVlen()
		{
			int size = list.Count;
			if (list[size - 1] == NcRange.VLEN)
			{
				return removeLast();
			}
			else
			{
				return this;
			}
		}

		public virtual Section removeFirst(Section parentSection)
		{
			int parentSize = parentSection.Rank;
			Debug.Assert(parentSize <= list.Count);
			if (parentSize == list.Count)
			{
				return new Section(); // scalar
			}
			return subSection(parentSize, list.Count);
		}

		public virtual Section prepend(Section parentSection)
		{
			if (parentSection == null)
			{
				return this;
			}
			IList<NcRange> ranges = new List<NcRange>(parentSection.Ranges);
			foreach (NcRange r in Ranges)
			{
				ranges.Add(r);
			}
			return new Section(ranges);
		}

		public virtual bool Immutable
		{
			get
			{
				return immutable;
			}
		}

		public virtual bool isVariableLength()
		{
			foreach (NcRange aFrom in list)
			{
				if (aFrom == NcRange.VLEN)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Get shape array using the Range.length() values.
		/// </summary>
		/// <returns> int[] shape </returns>
		public virtual int[] Shape
		{
			get
			{
				int[] result = new int[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					result[i] = list[i].length();
				}
				return result;
			}
		}

		/// <summary>
		/// Get origin array using the Range.first() values.
		/// </summary>
		/// <returns> int[] origin </returns>
		public virtual int[] Origin
		{
			get
			{
				int[] result = new int[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					result[i] = list[i].first();
				}
				return result;
			}
		}

		/// <summary>
		/// Get origin of the ith Range
		/// </summary>
		/// <param name="i"> index of Range </param>
		/// <returns> origin of ith Range </returns>
		public virtual int getOrigin(int i)
		{
			return list[i].first();
		}

		/// <summary>
		/// Get length of the ith Range
		/// </summary>
		/// <param name="i"> index of Range </param>
		/// <returns> length of ith Range </returns>
		public virtual int getShape(int i)
		{
			return list[i].length();
		}

		/// <summary>
		/// Get origin of the entire Section
		/// </summary>
		public virtual long Size
		{
			get
			{
				return NcIndex.computeSize(Shape);
			}
		}

		/// <summary>
		/// Get rank - number of Ranges.
		/// </summary>
		/// <returns> rank </returns>
		public virtual int Rank
		{
			get
			{
				return list.Count;
			}
		}

		/// <summary>
		/// Compare this section with another upto the vlen in either
		/// </summary>
		public virtual bool compatibleRank(Section other)
		{
			return (Rank == other.Rank);
			/*
			 * if((isVariableLength() && other.isVariableLength())
			 * || (!isVariableLength() && !other.isVariableLength()))
			 * return getRank() == other.getRank();
			 * else if(isVariableLength() && !other.isVariableLength())
			 * return getVlenRank() == other.getRank();
			 * else // !isVariableLength() && other.isVariableLength()
			 * return getRank() == other.getVlenRank();
			 */
		}

		/// <summary>
		/// Compute total number of elements represented by the section.
		/// Any VLEN or EMPTY Ranges are skipped.
		/// </summary>
		/// <returns> total number of elements </returns>
		public virtual long computeSize()
		{
			long product = 1;
			foreach (NcRange r in list)
			{
				if (r == NcRange.VLEN)
				{
					continue;
				}
				product *= r.length();
			}
			return product;
		}

		public static long computeSize(int[] shape)
		{
			long product = 1;
			foreach (int len in shape)
			{
				product *= len;
			}
			return product;
		}

		/// <summary>
		/// Get the list of Ranges. </summary>
		public virtual IList<NcRange> Ranges
		{
			get
			{
				return new List<NcRange>(list); // defensive copy. Cant use ImmutableList because of nulls.
			}
		}

		/// <summary>
		/// Get the ith Range
		/// </summary>
		/// <param name="i"> index into the list of Ranges </param>
		/// <returns> ith Range </returns>
		public virtual NcRange getRange(int i)
		{
			return list[i];
		}

		/// <summary>
		/// Find a Range by its name.
		/// </summary>
		/// <param name="rangeName"> find this Range </param>
		/// <returns> named Range or null </returns>
		public virtual NcRange find(string rangeName)
		{
			foreach (NcRange r in list)
			{
				if (rangeName.Equals(r.Name))
				{
					return r;
				}
			}
			return null;
		}

		/// <summary>
		/// Check if this Section is legal for the given shape.
		/// [Note: modified by dmh to address the case of unlimited
		/// where the size is zero]
		/// </summary>
		/// <param name="shape"> range must fit within this shape, rank must match. </param>
		/// <returns> error message if illegal, null if all ok </returns>
		public virtual string checkInRange(int[] shape)
		{
			if (list.Count != shape.Length)
			{
				return "object of ranges in section (" + list.Count + ") must be = " + shape.Length;
			}

			for (int i = 0; i < list.Count; i++)
			{
				NcRange r = list[i];
				if (r == null)
				{
					continue;
				}
				if (r == NcRange.VLEN)
				{
					continue;
				}
				if (r == NcRange.EMPTY)
				{
					if (shape[i] != 0)
					{
						return "Illegal Range for dimension " + i + ": empty range only for unlimited dimension len = 0";
					}
					else
					{
						continue;
					}
				}
				if (r.last() >= shape[i])
				{
					return "Illegal Range for dimension " + i + ": last requested " + r.last() + " > max " + (shape[i] - 1);
				}
			}

			return null;
		}

		/// <summary>
		/// Is this section equivilent to the given shape.
		/// All non-null ranges must have origin 0 and length = shape[i]
		/// </summary>
		/// <param name="shape"> the given shape. </param>
		/// <returns> true if equivilent </returns>
		public virtual bool equivalent(int[] shape)
		{
			if (Rank != shape.Length)
			{
				throw new InvalidRangeException("Invalid Section rank");
			}

			for (int i = 0; i < list.Count; i++)
			{
				NcRange r = list[i];
				if (r == null)
				{
					continue;
				}
				if (r.first() != 0)
				{
					return false;
				}
				if (r.length() != shape[i])
				{
					return false;
				}
			}
			return true;
		}

		public virtual bool conformal(Section other)
		{
			if (computeSize() != other.computeSize())
			{
				return false;
			}
			Section reduceThis = this.reduce();
			Section reduceOther = other.reduce();
			return reduceThis.equalShape(reduceOther);
		}

		public virtual bool equalShape(Section other)
		{
			if (computeSize() != other.computeSize())
			{
				return false;
			}
			if (Rank != other.Rank)
			{
				return false;
			}
			for (int i = 0; i < Rank; i++)
			{
				NcRange r = getRange(i);
				NcRange or = other.getRange(i);
				if (r.length() != or.length())
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Sections with equals Ranges are equal.
		/// </summary>
		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (!(o is Section))
			{
				return false;
			}
			Section os = (Section)o;

			if (Rank != os.Rank)
			{
				return false;
			}
			for (int i = 0; i < Rank; i++)
			{
				NcRange r = getRange(i);
				NcRange or = os.getRange(i);
				if ((r == null) && (or != null))
				{
					return false;
				}
				if ((or == null) && (r != null))
				{
					return false;
				}
				if (r == null)
				{
					continue; // then or is also null
				}
				if (!r.Equals(or))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Override Object.hashCode() to agree with equals.
		/// </summary>
		public override int GetHashCode()
		{
			int result = 17;
			foreach (NcRange r in list)
			{
				if (r != null)
				{
					result += 37 * result + r.GetHashCode();
				}
			}
			return result;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Iterate over a section, returning the index in an equivalent 1D array of shape[], and optionally the corresponding
		/// index[n]
		/// So this is a section in a (possibly) larger array described by shape[].
		/// The index is in the "source" array.
		/// </summary>
		/// <param name="shape"> total array shape </param>
		/// <returns> iterator over this section </returns>
		public virtual Iterator getIterator(int[] shape)
		{
			return new Iterator(this, shape);
		}

		public class Iterator
		{
			private readonly Section outerInstance;

			internal int[] odo; // odometer - the current element LOOK could use Index, but must upgrade to using Range
			internal IList<IEnumerator<int>> rangeIterList = new List<IEnumerator<int>>();
			internal int[] stride;
			internal long done, total;

			internal Iterator(Section outerInstance, int[] shape)
			{
				this.outerInstance = outerInstance;
				odo = new int[outerInstance.Rank];
				stride = new int[outerInstance.Rank];
				int ss = 1;
				for (int i = outerInstance.Rank - 1; i >= 0; i--)
				{ // fastest varying last
					stride[i] = ss;
					ss *= shape[i];
				}

				for (int i = 0; i < outerInstance.Rank; i++)
				{
					IEnumerator<int> iter = outerInstance.getRange(i).iterator();
					iter.MoveNext();
					odo[i] = iter.Current;
					rangeIterList.Add(iter);
				}

				done = 0;
				total = NcIndex.computeSize(outerInstance.Shape); // total in the section
			}

			/// <summary>
			/// Return true if there are more elements
			/// </summary>
			/// <returns> true if there are more elements </returns>
			public virtual bool hasNext()
			{
				return done < total;
			}

			/// <summary>
			/// Get the position in the equivalant 1D array of shape[]
			/// </summary>
			/// <param name="index"> if not null, return the current nD index </param>
			/// <returns> the current position in a 1D array </returns>
			public virtual int next(int[] index)
			{
				int next = currentElement();
				if (index != null)
				{
					System.Array.Copy(odo, 0, index, 0, odo.Length);
				}

				done++;
				if (done < total)
				{
					incr(); // increment for next call
				}
				return next;
			}

			internal virtual void incr()
			{
				int digit = outerInstance.Rank - 1;
				while (digit >= 0)
				{
					IEnumerator<int> iter = rangeIterList[digit];
					if (iter.MoveNext())
					{
						odo[digit] = iter.Current;
						break; // normal exit
					}

					// else, carry to next digit in the odometer
					IEnumerator<int> iterReset = outerInstance.getRange(digit).iterator();
					iterReset.MoveNext();
					odo[digit] = iterReset.Current;
					rangeIterList[digit] = iterReset;
					digit--;
					Debug.Assert(digit >= 0); // catch screw-ups
				}
			}

			internal virtual int currentElement()
			{
				int value = 0;
				for (int ii = 0; ii < outerInstance.Rank; ii++)
				{
					value += odo[ii] * stride[ii];
				}
				return value;
			}
		} // Section.Iterator

		public virtual Builder toBuilder()
		{
			return new Builder().appendRanges(this.Ranges);
		}

		public static Builder builder()
		{
			return new Builder();
		}

		public class Builder
		{
			internal IList<NcRange> ranges = new List<NcRange>();

			/// <summary>
			/// Append a Range to the Section meaning "all" </summary>
			public virtual Builder appendRangeAll()
			{
				ranges.Add(null);
				return this;
			}

			/// <summary>
			/// Append a Range to the Section
			/// </summary>
			/// <param name="range"> not null. </param>
			public virtual Builder appendRange(NcRange range)
			{
				ranges.Add(range);
				return this;
			}

			/// <summary>
			/// Append a new Range(0,size-1) </summary>
			public virtual Builder appendRange(int size)
			{
				if (size > 0)
				{
					ranges.Add(new NcRange(size));
				}
				else if (size == 0)
				{
					ranges.Add(NcRange.EMPTY);
				}
				else
				{
					ranges.Add(NcRange.VLEN);
				}
				return this;
			}

			/// <summary>
			/// Append a new Range(first, last) to the Section
			/// </summary>
			/// <param name="first"> starting index </param>
			/// <param name="last"> last index, inclusive. If last < 0, then append a VLEN Range. </param>
			public virtual Builder appendRange(int first, int last)
			{
				if (last < 0)
				{
					ranges.Add(NcRange.VLEN);
				}
				else
				{
					ranges.Add(new NcRange(first, last));
				}
				return this;
			}

			/// <summary>
			/// Append a new Range(first,last,stride) to the Section.
			/// </summary>
			/// <param name="first"> starting index </param>
			/// <param name="last"> last index, inclusive </param>
			/// <param name="stride"> stride </param>
			public virtual Builder appendRange(int first, int last, int stride)
			{
				if (last < 0)
				{
					ranges.Add(NcRange.VLEN);
				}
				else
				{
					ranges.Add(new NcRange(first, last, stride));
				}
				return this;
			}

			/// <summary>
			/// Append a new Range(name,first,last,stride) to the Section
			/// </summary>
			/// <param name="name"> name of Range </param>
			/// <param name="first"> starting index </param>
			/// <param name="last"> last index, inclusive </param>
			/// <param name="stride"> stride </param>
			public virtual Builder appendRange(string name, int first, int last, int stride)
			{
				if (last < 0)
				{
					ranges.Add(NcRange.VLEN);
				}
				else
				{
					ranges.Add(new NcRange(name, first, last, stride));
				}
				return this;
			}

			/// <summary>
			/// Append Ranges to the Section
			/// </summary>
			/// <param name="ranges"> not null. </param>
			public virtual Builder appendRanges(IList<NcRange> ranges)
			{
				foreach (NcRange r in ranges)
				{
					this.ranges.Add(r);
				}
				return this;
			}

			/// <summary>
			/// Create Section from a shape array, assumes 0 origin.
			/// </summary>
			/// <param name="shape"> array of lengths for each Range. 0 = EMPTY, < 0 = VLEN </param>
			public virtual Builder appendRanges(int[] shape)
			{
				foreach (int aShape in shape)
				{
					if (aShape > 0)
					{
						ranges.Add(new NcRange(aShape));
					}
					else if (aShape == 0)
					{
						ranges.Add(NcRange.EMPTY);
					}
					else
					{
						ranges.Add(NcRange.VLEN);
					}
				}
				return this;
			}

			/// <summary>
			/// Insert a range at the specified index in the list.
			/// </summary>
			/// <param name="index"> insert here in the list, existing ranges at or after this index get shifted by one </param>
			/// <param name="r"> insert this Range </param>
			public virtual Builder insertRange(int index, NcRange r)
			{
				ranges.Insert(index, r);
				return this;
			}

			/// <summary>
			/// Remove a range at the specified index in the list.
			/// </summary>
			/// <param name="index"> remove here in the list, existing ranges after this index get shifted by one </param>
			public virtual Builder removeRange(int index)
			{
				ranges.RemoveAt(index);
				return this;
			}

			/// <summary>
			/// Replace a range at the specified index in the list.
			/// </summary>
			/// <param name="index"> replace here in the list. </param>
			/// <param name="r"> use this Range </param>
			/// <returns> this </returns>
			/// <exception cref="IndexOutOfRangeException"> if bad index </exception>
			public virtual Builder replaceRange(int index, NcRange r)
			{
				ranges[index] = r;
				return this;
			}

			/// <summary>
			/// Set the range at the specified index in the list, previous Range is discarded
			/// </summary>
			/// <param name="index"> list index, must be in interval [0,size). </param>
			/// <param name="r"> insert this Range </param>
			public virtual Builder setRange(int index, NcRange r)
			{
				ranges[index] = r;
				return this;
			}

			/// <summary>
			/// Remove the last range, if it exists. </summary>
			public virtual Builder removeLast()
			{
				int size = ranges.Count;
				if (size > 0)
				{
					ranges.RemoveAt(size - 1);
				}
				return this;
			}

			/// <summary>
			/// Remove the first n Ranges, n <= number of ranges. </summary>
			public virtual Builder removeFirst(int n)
			{
				Debug.Assert(n <= ranges.Count);
				IList<NcRange> sub = new List<NcRange>();
				for (int i = n; i < ranges.Count; i++)
				{
					sub.Add(ranges[i]);
				}
				ranges = sub;
				return this;
			}

			/// <summary>
			/// Remove the last range, if it exists and is a Vlen. </summary>
			public virtual Builder removeVlen()
			{
				int size = ranges.Count;
				if (ranges[size - 1] == NcRange.VLEN)
				{
					ranges.RemoveAt(size - 1);
				}
				return this;
			}

			public virtual Section build()
			{
				return new Section(ranges);
			}
		}

	}

}
