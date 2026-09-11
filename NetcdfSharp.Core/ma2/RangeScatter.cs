// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NetcdfSharp.ucar.ma2
{
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

	/// <summary>
	/// A Range of indices describes by a list, rather than start:stop:stride.
	/// Issues:
	/// NetcdfSharp.ucar.ma2.Index
	/// 
	/// @author John
	/// @since 8/12/2015
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class RangeScatter implements RangeIterator
	public class RangeScatter : RangeIterator
	{
	  private readonly int[] vals;
	  private readonly string name;

	  /// <summary>
	  /// Ctor
	  /// </summary>
	  /// <param name="name"> optional name </param>
	  /// <param name="val"> should be sorted </param>
	  public RangeScatter(string name, params int[] val)
	  {
		// super(name, val[0], val[val.length-1], 1, val.length);
		this.name = name;
		this.vals = val;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  /// @deprecated use copyWithName() 
	  [Obsolete("use copyWithName()")]
	  public virtual RangeIterator setName(string name)
	  {
		if (name.Equals(this.Name))
		{
		  return this;
		}
		return new RangeScatter(name, vals);
	  }

	  public virtual RangeIterator copyWithName(string name)
	  {
		if (name.Equals(this.Name))
		{
		  return this;
		}
		return new RangeScatter(name, vals);
	  }

	  public virtual int length()
	  {
		return vals.Length;
	  }

	  /*
	   * @Override
	   * public Range copy(String name) {
	   * try {
	   * return new RangeScatter(name, vals);
	   * } catch (InvalidRangeException e) {
	   * throw new RuntimeException(e); // cant happen
	   * }
	   * }
	   * 
	   * @Override
	   * public RangeScatter compose(Range r) throws InvalidRangeException {
	   * int[] svals = new int[r.length()];
	   * 
	   * int count = 0;
	   * for (int idx : r) {
	   * svals[count++] = element(idx);
	   * }
	   * return new RangeScatter(name, svals);
	   * }
	   * 
	   * @Override
	   * public RangeScatter shiftOrigin(int origin) throws InvalidRangeException {
	   * int[] svals = new int[length];
	   * for (int i=0; i< length; i++) svals[i] = vals[i] + origin;
	   * return new RangeScatter(name, svals);
	   * }
	   * 
	   * @Override
	   * public Range intersect(Range r) throws InvalidRangeException {
	   * Set<Integer> sect = new HashSet<>();
	   * for (int idx : r) {
	   * sect.add(idx);
	   * }
	   * 
	   * List<Integer> result = new ArrayList<>();
	   * for (int idx : this) {
	   * if (sect.contains(idx))
	   * result.add(idx);
	   * }
	   * 
	   * if (result.size() == 0) return EMPTY;
	   * 
	   * int[] svals = new int[result.size()];
	   * for (int i=0; i<result.size(); i++) svals[i] = result.get(i);
	   * return new RangeScatter(name, svals);
	   * }
	   * 
	   * @Override
	   * public boolean intersects(Range r) {
	   * try {
	   * return intersect(r) != EMPTY;
	   * } catch (InvalidRangeException e) {
	   * e.printStackTrace();
	   * return false;
	   * }
	   * }
	   * 
	   * @Override
	   * public RangeScatter union(Range r) throws InvalidRangeException {
	   * Set<Integer> union = new HashSet<>();
	   * for (int idx : r)
	   * union.add(idx);
	   * for (int idx : this)
	   * union.add(idx);
	   * 
	   * List<Integer> result = union.stream().collect(Collectors.toList());
	   * Collections.sort(result);
	   * 
	   * int[] svals = new int[result.size()];
	   * for (int i=0; i<result.size(); i++) svals[i] = result.get(i);
	   * return new RangeScatter(name, svals); }
	   * 
	   * @Override
	   * public int element(int i) throws InvalidRangeException {
	   * if (i < 0)
	   * throw new InvalidRangeException("i must be >= 0");
	   * if (i >= vals.length)
	   * throw new InvalidRangeException("i must be < length");
	   * 
	   * return vals[i];
	   * }
	   * 
	   * @Override
	   * public int index(int want) throws InvalidRangeException {
	   * int res = Arrays.binarySearch(vals, want);
	   * if (res >= 0) return res;
	   * throw new InvalidRangeException("elem not found in RangeScatter");
	   * }
	   * 
	   * @Override
	   * public boolean contains(int want) {
	   * return Arrays.binarySearch(vals, want) >= 0;
	   * }
	   * 
	   * /**
	   * Find the first element in a strided array after some index start.
	   * Return the smallest element k in the Range, such that <ul>
	   * <li>k >= first
	   * <li>k >= start
	   * <li>k <= last
	   * <li>k = element of this Range
	   * </ul>
	   *
	   * @param start starting index
	   * 
	   * @return first in interval, else -1 if there is no such element.
	   *
	   * @Override
	   * public int getFirstInInterval(int start) {
	   * if (start > last()) return -1;
	   * if (start <= first()) return first();
	   * for (int i=1; i< length; i++)
	   * if (start > vals[i])
	   * return i-1;
	   * 
	   * return length -1;
	   * }
	   */

	  public override string ToString()
	  {
		return "{" + "[" + string.Join(", ", vals) + "]" + "}";
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}
		if (!base.Equals(o))
		{
		  return false;
		}

		RangeScatter that = (RangeScatter) o;
		return vals.SequenceEqual(that.vals);
	  }

	  public override int GetHashCode()
	  {
		return Arrays.hashCode(vals);
	  }

	  public virtual IEnumerator<int> GetEnumerator()
	  {
		return new ScatterIterator(this);
	  }

	  IEnumerator System.Collections.IEnumerable.GetEnumerator()
	  {
		return GetEnumerator();
	  }

	  public virtual IEnumerator<int> iterator()
	  {
		return GetEnumerator();
	  }

	  private class ScatterIterator : IEnumerator<int>
	  {
		  private readonly RangeScatter outerInstance;

		  public ScatterIterator(RangeScatter outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		internal int current;

		public bool hasNext()
		{
		  return current < outerInstance.vals.Length;
		}

		public int? next()
		{
		  return outerInstance.vals[current++];
		}

		public virtual int Current => next() ?? 0;
		object System.Collections.IEnumerator.Current => Current;
		public virtual bool MoveNext() => hasNext();
		public virtual void Reset() { }
		public virtual void Dispose() { }
	  }
	}

}
