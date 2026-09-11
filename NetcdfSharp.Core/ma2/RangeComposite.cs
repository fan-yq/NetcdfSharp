// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// A Composite of other RangeIterators.
	/// Iterate over them in sequence.
	/// 
	/// @author John
	/// @since 8/19/2015
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class RangeComposite implements RangeIterator
	public class RangeComposite : RangeIterator
	{
	  private readonly IList<RangeIterator> ranges;
	  private readonly string name;

	  public RangeComposite(string name, IList<RangeIterator> ranges)
	  {
		this.name = name;
		this.ranges = ranges;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual IList<RangeIterator> Ranges
	  {
		  get
		  {
			return ranges;
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
		return new RangeComposite(name, ranges);
	  }

	  public virtual RangeIterator copyWithName(string name)
	  {
		if (name.Equals(this.Name))
		{
		  return this;
		}
		return new RangeComposite(name, ranges);
	  }

	  public virtual IEnumerator<int> GetEnumerator()
	  {
		ICollection<IEnumerable<int>> iters = new List<IEnumerable<int>>();
		foreach (RangeIterator r in ranges) iters.Add(r);

		return new CompositeIterator<int>(iters);
	  }

	  IEnumerator System.Collections.IEnumerable.GetEnumerator()
	  {
		return GetEnumerator();
	  }

	  public virtual IEnumerator<int> iterator()
	  {
		return GetEnumerator();
	  }

	  public virtual int length()
	  {
		int result = 0;
		foreach (RangeIterator r in ranges)
		{
		  result += r.length();
		}
		return result;
	  }

	  // generic could be moved to utils
	  private class CompositeIterator<T> : IEnumerator<T>
	  {
		internal IEnumerator<IEnumerable<T>> iters;
		internal IEnumerator<T> current;

		internal CompositeIterator(ICollection<IEnumerable<T>> iters)
		{
		  this.iters = iters.GetEnumerator();
		  MoveNext();
		}

		public bool hasNext()
		{
		  return current != null;
		}

		public T next()
		{
		  if (current == null) throw new System.InvalidOperationException();
		  T result = current.Current;
		  if (!current.MoveNext())
		  {
			current = null;
			if (iters.MoveNext())
			{
			  current = iters.Current.GetEnumerator();
			  if (!current.MoveNext()) current = null;
			}
		  }
		  return result;
		}

		public virtual T Current
		{
		  get
		  {
			if (current == null) throw new System.InvalidOperationException();
			return current.Current;
		  }
		}
		object System.Collections.IEnumerator.Current => Current;
		public virtual bool MoveNext()
		{
		  if (current == null) return false;
		  return true;
		}
		public virtual void Reset() { }
		public virtual void Dispose() { }
	  }

	}

}
