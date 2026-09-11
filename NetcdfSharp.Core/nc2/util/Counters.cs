// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.util
{

	/// <summary>
	/// Count number of times a value appears.
	/// value may be any Comparable;
	/// equals() is used for uniqueness.
	/// 
	/// @author caron
	/// @since 11/15/2014
	/// </summary>
	public class Counters
	{
	  internal IList<Counter> counters = new List<Counter>();
	  internal IDictionary<string, Counter> map = new Dictionary<string, Counter>();

	  public virtual Counter add(string name)
	  {
		Counter c = new Counter(name);
		counters.Add(c);
		map[name] = c;
		return c;
	  }

	  public virtual void reset()
	  {
		foreach (Counter c in counters)
		{
		  c.reset();
		}
	  }

	  public virtual void show(Formatter f)
	  {
		foreach (Counter c in counters)
		{
		  c.show(f);
		}
	  }

	  public override string ToString()
	  {
		Formatter f = new Formatter();
		show(f);
		return f.ToString();
	  }

	  public virtual Counter get(string name)
	  {
		return map[name];
	  }

	  /// <summary>
	  /// Add value to the named counter.
	  /// Add counter if it doesnt already exist.
	  /// </summary>
	  /// <returns> true if its a new value, not seen before. </returns>
	  public virtual bool count(string name, IComparable value)
	  {
		Counter counter = map[name];
		if (counter == null)
		{
		  counter = add(name);
		}
		return counter.count(value);
	  }

	  public virtual void addTo(Counters sub)
	  {
		foreach (Counter subC in sub.counters)
		{
		  Counter all = map[subC.Name];
		  if (all == null)
		  {
			all = add(subC.Name);
		  }
		  all.addTo(subC);
		}
	  }

	  public virtual Counters makeSubCounters()
	  {
		Counters result = new Counters();
		foreach (Counter c in counters)
		{
		  result.add(c.Name);
		}
		return result;
	  }

	  public class Counter
	  {
		internal string name;
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		internal bool showRange_Conflict;
		internal IComparable first, last;
		internal IDictionary<IComparable, int> set = new Dictionary<IComparable, int>();
		internal string range;

		public Counter(string name)
		{
		  this.name = name;
		}

		public virtual string Name
		{
			get
			{
			  return name;
			}
		}

		public virtual Counter setShowRange(bool showRange)
		{
		  this.showRange_Conflict = showRange;
		  return this;
		}

		public virtual void reset()
		{
		  set = new Dictionary<IComparable, int>();
		}

		public virtual bool count(IComparable value)
		{
		  int? count = set[value];
		  if (count == null)
		  {
			set[value] = 1;
			return true;
		  }
		  else
		  {
			set[value] = count.Value + 1;
			return false;
		  }
		}

		public virtual void addTo(Counter sub)
		{
		  foreach (KeyValuePair<IComparable, int> entry in sub.set.SetOfKeyValuePairs())
		  {
			int? count = this.set[entry.Key];
			if (count == null)
			{
			  count = 0;
			}
			set[entry.Key] = count.Value + entry.Value;
		  }
		}

		public virtual int Unique
		{
			get
			{
			  return set.Count;
			}
		}

		public virtual ISet<IComparable> Values
		{
			get
			{
			  return new HashSet<IComparable>(set.Keys);
			}
		}

		public virtual int? getCount(IComparable key)
		{
		  return set[key];
		}

		public virtual IComparable First
		{
			get
			{
			  return first;
			}
		}

		public virtual IComparable Last
		{
			get
			{
			  return last;
			}
		}

		// get the value with greatest number of values.
		// if more than one, but all have same number, return null
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Nullable public Comparable getMode()
		public virtual IComparable Mode
		{
			get
			{
			  if (set.Count == 1)
			  {
				var enumerator = set.Keys.GetEnumerator();
				enumerator.MoveNext();
				return enumerator.Current; // if only one, return it
			  }

			  int max = -1;
			  IComparable mode = null;
			  bool same = true; // are all keys the same ??
			  IComparable testKey = null;
			  foreach (KeyValuePair<IComparable, int> entry in set.SetOfKeyValuePairs())
			  {
				IComparable entryKey = entry.Key;
				if (testKey != null && entryKey.CompareTo(testKey) != 0)
				{
				  same = false;
				}
				testKey = entryKey;

				if (entry.Value > max)
				{
				  max = entry.Value;
				  mode = entry.Key;
				}
			  }
			  return same ? null : mode;
			}
		}

		public virtual int Total
		{
			get
			{
			  int total = 0;
			  foreach (KeyValuePair<IComparable, int> entry in set.SetOfKeyValuePairs())
			  {
				total += entry.Value;
			  }
			  return total;
			}
		}

		public virtual void show(Formatter f)
		{
		  IList<IComparable> list = new List<IComparable>(set.Keys);
		  f.format("%n%s (%d)%n", name, list.Count);
		  list.Sort();

		  if (showRange_Conflict)
		  {
			int n = list.Count;
			if (n == 0)
			{
			  f.format("none%n");
			}
			else
			{
			  f.format("   %10s - %10s: count = %d%n", list[0], list[n - 1], Unique);
			}

		  }
		  else
		  {
			IComparable prev = null;
			foreach (IComparable key in list)
			{
			  int count = set[key];
			  bool isHashDup = (prev != null) && key.GetHashCode() == prev.GetHashCode();
			  bool isNameDup = (prev != null) && key.ToString().Equals(prev.ToString());
			  f.format("  %s %10s: count = %d%n", isHashDup != isNameDup ? "*" : " ", key, count);
			  prev = key;
			}
		  }
		}

		public virtual string showRange()
		{
		  if (string.ReferenceEquals(range, null))
		  {
			IList<IComparable> list = new List<IComparable>(set.Keys);
			list.Sort();
			int n = list.Count;
			if (n == 0)
			{
			  return "none";
			}

			Formatter f = new Formatter();
			this.first = list[0];
			this.last = list[n - 1];
			f.format("%10s - %10s", first, last);
			range = f.ToString();
		  }
		  return range;
		}
	  }

	}

}
