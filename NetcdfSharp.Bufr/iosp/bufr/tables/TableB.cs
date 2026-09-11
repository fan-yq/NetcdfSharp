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
	using DataDescriptor = NetcdfSharp.ucar.nc2.iosp.bufr.DataDescriptor;

	/// <summary>
	/// BUFR Table B - Data descriptors
	/// 
	/// @author caron
	/// @since Sep 25, 2008
	/// </summary>
	public class TableB
	{
	  private readonly string name;
	  private readonly string location;
	  private readonly IDictionary<short, Descriptor> map;

	  public TableB(string name, string location)
	  {
		this.name = name;
		this.location = location;
		map = new Dictionary<short, Descriptor>();
	  }

	  public virtual void addDescriptor(short x, short y, int scale, int refVal, int width, string name, string units, string desc)
	  {
		short id = (short)((x << 8) + y);
		map[id] = new Descriptor(this, x, y, scale, refVal, width, name, units, desc);
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

	  public virtual ICollection<short> Keys
	  {
		  get
		  {
			return map.Keys;
		  }
	  }

	  public virtual void show(Formatter @out)
	  {
		IList<short> sortKeys = new List<short>(Keys);
		sortKeys.Sort();

		@out.format("Table B %s %n", name);
		foreach (short? key in sortKeys)
		{
		  Descriptor dd = getDescriptor(key.Value);
		  if (dd != null)
		  {
			dd.show(@out);
		  }
		  @out.format("%n");
		}
	  }

	  /// <summary>
	  /// Composite pattern - collection of TableB
	  /// </summary>
	  public class Composite : TableB
	  {
		internal IList<TableB> list = new List<TableB>(3);

		public Composite(string name, string location) : base(name, location)
		{
		}

		public virtual void addTable(TableB b)
		{
		  list.Add(b);
		}

		public override Descriptor getDescriptor(short id)
		{
		  foreach (TableB b in list)
		  {
			Descriptor d = b.getDescriptor(id);
			if (d != null)
			{
			  return d;
			}
		  }
		  return null;
		}

		public override ICollection<Descriptor> Descriptors
		{
			get
			{
			  List<Descriptor> result = new List<Descriptor>(3000);
			  foreach (TableB b in list)
			  {
				result.AddRange(b.Descriptors);
			  }
			  return result;
			}
		}

		public override ICollection<short> Keys
		{
			get
			{
			  List<short> result = new List<short>(3000);
			  foreach (TableB b in list)
			  {
				result.AddRange(b.Keys);
			  }
			  return result;
			}
		}
	  }

	  // inner class
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Descriptor implements Comparable<Descriptor>
	  public class Descriptor : IComparable<Descriptor>
	  {
		  private readonly TableB outerInstance;


		internal readonly short x, y;
		internal readonly int scale;
		internal readonly int refVal;
		internal readonly int dataWidth;
		internal readonly string units;
		internal readonly string name;
		internal readonly string desc;
		internal readonly bool numeric;
		internal bool localOverride;

		internal Descriptor(TableB outerInstance, short x, short y, int scale, int refVal, int width, string name, string units, string desc)
		{
			this.outerInstance = outerInstance;
		  this.x = x;
		  this.y = y;
		  this.scale = scale;
		  this.refVal = refVal;
		  this.dataWidth = width;
		  this.name = name.Trim();
		  this.units = units.Trim(); // String.intern() has no C# equivalent; skip
		  this.desc = desc;

		  this.numeric = !DataDescriptor.isInternationalAlphabetUnit(units);
		}

		public virtual int Scale
		{
			get
			{
			  return scale;
			}
		}

		public virtual int RefVal
		{
			get
			{
			  return refVal;
			}
		}

		public virtual int DataWidth
		{
			get
			{
			  return dataWidth;
			}
		}

		public virtual string Units
		{
			get
			{
			  return units;
			}
		}

		public virtual string Name
		{
			get
			{
			  return name;
			}
		}

		public virtual string Desc
		{
			get
			{ // optional - use as long name
			  return desc;
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
			  return (short)((x << 8) + y);
			}
		}

		/// <summary>
		/// Get fxy as a String, eg 0-5-22
		/// </summary>
		/// <returns> fxy encoded as a String </returns>
		public virtual string Fxy
		{
			get
			{
			  return "0-" + x + "-" + y;
			}
		}

		/// <summary>
		/// is descriptor numeric or String
		/// </summary>
		/// <returns> true if numeric </returns>
		public virtual bool Numeric
		{
			get
			{
			  return numeric;
			}
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


		public override string ToString()
		{
		  Formatter @out = new Formatter();
		  show(@out);
		  return @out.ToString();
		}

		public virtual string Source
		{
			get
			{
			  return outerInstance.Location;
			}
		}

		internal virtual void show(Formatter @out)
		{
		  @out.format(" %8s scale=%d refVal=%d width=%d  units=(%s) name=(%s)", Fxy, scale, refVal, dataWidth, units, name);
		}

		public virtual int CompareTo(Descriptor o)
		{
		  return Id - o.Id;
		}
	  }


	}

}
