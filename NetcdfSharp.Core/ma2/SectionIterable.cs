// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// A Section composed of List<RangeIterator> instead of List<Range>.
	/// SectionIterable knows the fullShape of which it is a section.
	/// The iteration is over the elements in the section, returning 1D index into the full shape.
	/// 
	/// @author John
	/// @since 8/23/2015
	/// </summary>
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @Immutable public class SectionIterable implements Iterable<int>
	public class SectionIterable : IEnumerable<int>
	{

		private readonly IList<RangeIterator> ranges;
		private readonly int[] fullShape;

		public SectionIterable(IList<RangeIterator> ranges, int[] fullShape)
		{
			Debug.Assert(ranges.Count == fullShape.Length, ranges.Count + " != " + fullShape.Length);
			int count = 0;
			foreach (RangeIterator ri in ranges)
			{
				Debug.Assert((ri.length() <= fullShape[count]));
				count++;
			}

			this.ranges = ranges;
			this.fullShape = fullShape;
		}

		public SectionIterable(IList<RangeIterator> ranges, IList<int> fullShapeList)
		{
			Debug.Assert(ranges.Count == fullShapeList.Count, ranges.Count + " != " + fullShapeList.Count);
			int count = 0;
			this.fullShape = new int[fullShapeList.Count];
			foreach (RangeIterator ri in ranges)
			{
				Debug.Assert((ri.length() <= fullShapeList[count]));
				this.fullShape[count] = fullShapeList[count];
				count++;
			}

			this.ranges = ranges;
		}

		public SectionIterable(Section section, int[] fullShape)
		{
			this.ranges = new List<RangeIterator>();
			((List<RangeIterator>)this.ranges).AddRange(section.Ranges);
			this.fullShape = fullShape;
		}

		public virtual int Rank
		{
			get
			{
				return ranges.Count;
			}
		}

		public virtual SectionIterable subSection(int start, int endExclusive)
		{
			int n = endExclusive - start;
			int[] subFullRange = new int[n];
			System.Array.Copy(fullShape, start, subFullRange, 0, n);

			return new SectionIterable(ranges.Skip(start).Take(n).ToList(), subFullRange);
		}

		public virtual RangeIterator getRange(int i)
		{
			return ranges[i];
		}

		public virtual int[] Shape
		{
			get
			{
				int[] result = new int[Rank];
				for (int i = 0; i < Rank; i++)
				{
					result[i] = getRange(i).length();
				}
				return result;
			}
		}

		public virtual long computeSize()
		{
			long product = 1;
			foreach (RangeIterator r in ranges)
			{
				product *= r.length();
			}
			return product;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		// use this if you just need the 1D source index, not the equivilent nD index
		public virtual IEnumerator<int> GetEnumerator()
		{
			return new SectionIterator(this);
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Iterate over a section, returning the index in an equivalent 1D array of shape[]
		/// So this is a section in a (possibly) larger array described by shape[].
		/// The index is in the "source" array.
		/// </summary>
		/// <returns> iterator over this section </returns>
		public virtual SectionIterator Iterator
		{
			get
			{
				return new SectionIterator(this);
			}
		}

		public class SectionIterator : IEnumerator<int>
		{
			internal virtual void InitializeInstanceFields()
			{
				odo = new int[outerInstance.Rank];
				stride = new int[outerInstance.Rank];
			}

			private readonly SectionIterable outerInstance;

			internal int[] odo; // odometer - the current element LOOK could use Index, but must upgrade to
								// using Range
			internal IList<IEnumerator<int>> rangeIterList = new List<IEnumerator<int>>();
			internal int[] stride;
			internal long done, total;

			internal SectionIterator(SectionIterable outerInstance)
			{
				this.outerInstance = outerInstance;

				InitializeInstanceFields();
				int ss = 1;
				for (int i = outerInstance.Rank - 1; i >= 0; i--)
				{ // fastest varying last
					stride[i] = ss;
					ss *= outerInstance.fullShape[i];
				}

				for (int i = 0; i < outerInstance.Rank; i++)
				{
					IEnumerator<int> iter = outerInstance.getRange(i).GetEnumerator();
					iter.MoveNext();
					odo[i] = iter.Current;
					rangeIterList.Add(iter);
				}

				done = 0;
				total = NcIndex.computeSize(outerInstance.Shape); // total in the section
			}

			public virtual bool hasNext()
			{
				return done < total;
			}

			public virtual int? next()
			{
				int next = currentElement();
				done++;
				if (done < total)
				{
					incr(); // increment for next call
				}
				return next;
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
					IEnumerator<int> iterReset = outerInstance.getRange(digit).GetEnumerator();
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

			public virtual int Current => next() ?? 0;
			object System.Collections.IEnumerator.Current => Current;
			public virtual bool MoveNext() => hasNext();
			public virtual void Reset() { }
			public virtual void Dispose() { }
		} // SectionIterator

	}

}
