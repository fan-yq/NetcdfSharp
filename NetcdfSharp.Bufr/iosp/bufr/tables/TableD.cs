// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr.tables
{

	/// <summary>
	/// BUFR Table D - Data sequences
	/// 
	/// @author caron
	/// @since Sep 25, 2008
	/// </summary>
	public class TableD
	{
	  private string name;
	  private string location;
	  private IDictionary<short, Descriptor> map;

	  public TableD(string name, string location)
	  {
		this.name = name;
		this.location = location;
		map = new Dictionary<short, Descriptor>();
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string Location
	  {
		  get
		  {
			return location;
		  }
	  }

	  public virtual Descriptor addDescriptor(short x, short y, string name, IList<short> seq)
	  {
		short id = (short)((3 << 14) + (x << 8) + y);
		Descriptor d = new Descriptor(x, y, name, seq);
		map[id] = d;
		return d;
	  }

	  public virtual Descriptor getDescriptor(short id)
	  {
		return map[id];
	  }

	  public virtual ICollection<Descriptor> Descriptors
	  {
		  get
		  {
			return map.Values;
		  }
	  }

	  public virtual void show(Formatter @out)
	  {
		IEnumerable<short> keys = map.Keys;
		IList<short> sortKeys = new List<short>(keys);
		sortKeys.Sort();

		@out.format("Table D %s %n", name);
		foreach (short key in sortKeys)
		{
		  Descriptor dd = map[key];
		  dd.show(@out, true);
		}
	  }

	  public class Descriptor : IComparable<Descriptor>
	  {
		internal short x, y;
		internal string name;
		internal IList<short> seq;
		internal bool localOverride;

		internal Descriptor(short x, short y, string name, IList<short> seq)
		{
		  this.x = x;
		  this.y = y;
		  this.name = name;
		  this.seq = seq;
		}

		public virtual IList<short> Sequence
		{
			get
			{
			  return seq;
			}
		}

		public virtual void addFeature(short f)
		{
		  seq.Add(f);
		}

		public virtual string Name
		{
			get
			{
			  return name;
			}
		}

		/// <summary>
		/// Get fxy as a short
		/// </summary>
		/// <returns> fxy encoded as a short </returns>
		public virtual short Id
		{
			get
			{
			  return (short)((3 << 14) + (x << 8) + y);
			}
		}

		/// <summary>
		/// Get fxy as a String, eg 3-4-22
		/// </summary>
		/// <returns> fxy encoded as a String </returns>
		public virtual string Fxy
		{
			get
			{
			  return "3-" + x + "-" + y;
			}
		}

		public override string ToString()
		{
		  return Fxy + " " + Name;
		}

		public virtual void show(Formatter @out, bool oneline)
		{
		  @out.format(" %8s: name=(%s) seq=", Fxy, name);
		  if (oneline)
		  {
			foreach (short s in seq)
			{
			  @out.format(" %s,", NetcdfSharp.ucar.nc2.iosp.bufr.Descriptor.makeString(s));
			}
			@out.format("%n");
		  }
		  else
		  {
			foreach (short s in seq)
			{
			  @out.format("    %s%n", NetcdfSharp.ucar.nc2.iosp.bufr.Descriptor.makeString(s));
			}
		  }
		}

		public virtual int CompareTo(Descriptor o)
		{
		  return Id - o.Id;
		}


		public virtual bool Local
		{
			get
			{
			  return ((x >= 48) || (y >= 192));
			}
		}

		public virtual bool LocalOverride
		{
			set
			{
			  this.localOverride = value;
			}
			get
			{
			  return localOverride;
			}
		}


	  }
	}

}
