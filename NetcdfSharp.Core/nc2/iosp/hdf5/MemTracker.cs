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

namespace NetcdfSharp.ucar.nc2.iosp.hdf5
{

	/// <summary>
	/// Track use of space in an HDF5 file
	/// 
	/// @author caron
	/// @since 6/27/12
	/// </summary>
	public class MemTracker
	{
	  private IList<Mem> memList = new List<Mem>();

	  private long fileSize;

	  public MemTracker(long fileSize)
	  {
		this.fileSize = fileSize;
	  }

	  public virtual void add(string name, long start, long end)
	  {
		memList.Add(new Mem(name, start, end));
	  }

	  public virtual void addByLen(string name, long start, long size)
	  {
		memList.Add(new Mem(name, start, start + size));
	  }

	  public virtual void report(Formatter f)
	  {
		f.format("Memory used file size= %d%n", fileSize);
		f.format("  start    end   size   name");
		memList.Sort();
		Mem prev = null;
		foreach (Mem m in memList)
		{
		  if ((prev != null) && (m.start > prev.end))
		  {
			f.format(" + %6d %6d %6d %6s%n", prev.end, m.start, m.start - prev.end, "HOLE");
		  }
		  char c = ((prev != null) && (prev.end != m.start)) ? '*' : ' ';
		  f.format(" %s %6d %6d %6d %6s%n", c, m.start, m.end, m.end - m.start, m.name);
		  prev = m;
		}
		f.format("%n");
	  }

	  internal class Mem : IComparable<Mem>, IComparable
	  {
		public string name;
		public long start, end;

		internal Mem(string name, long start, long end)
		{
		  this.name = name;
		  this.start = start;
		  this.end = end;
		}

		public virtual int CompareTo(Mem m)
		{
		  return JavaLangCompat.LongCompare(start, m.start);
		}

		public virtual int CompareTo(object m)
		{
		  return CompareTo((Mem)m);
		}

	  }
	}

}
