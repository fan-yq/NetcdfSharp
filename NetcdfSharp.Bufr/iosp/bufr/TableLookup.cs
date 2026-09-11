// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using BufrTables = NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables;
	using TableA = NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableA;
	using TableB = NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableB;
	using TableD = NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableD;

	/// <summary>
	/// Encapsolates lookup into the BUFR Tables.
	/// 
	/// @author caron
	/// @since Jul 14, 2008
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class TableLookup
	public class TableLookup
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(TableLookup));
	  private const bool showErrors = false;

	  /////////////////////////////////////////
	  private TableA localTableA = null;
	  private readonly TableB localTableB;
	  private readonly TableD localTableD;

	  private readonly TableB wmoTableB;
	  private readonly TableD wmoTableD;
	  private readonly BufrTables.Mode mode;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public TableLookup(int center, int subcenter, int masterTableVersion, int local, int cat) throws NetcdfSharp.java.io.IOException
	  public TableLookup(int center, int subcenter, int masterTableVersion, int local, int cat)
	  {
		this.wmoTableB = BufrTables.getWmoTableB(masterTableVersion);
		this.wmoTableD = BufrTables.getWmoTableD(masterTableVersion);

		BufrTables.Tables tables = BufrTables.getLocalTables(center, subcenter, masterTableVersion, local, cat);
		if (tables != null)
		{
		  this.localTableB = tables.b;
		  this.localTableD = tables.d;
		  this.mode = tables.mode;
		}
		else
		{
		  this.localTableB = null;
		  this.localTableD = null;
		  this.mode = BufrTables.Mode.localOverride;
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public TableLookup(BufrIdentificationSection ids, NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableB b, NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableD d) throws NetcdfSharp.java.io.IOException
	  public TableLookup(BufrIdentificationSection ids, TableB b, TableD d)
	  {
		this.wmoTableB = BufrTables.getWmoTableB(ids.MasterTableVersion);
		this.wmoTableD = BufrTables.getWmoTableD(ids.MasterTableVersion);
		this.localTableB = b;
		this.localTableD = d;
		this.mode = BufrTables.Mode.localOverride;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public TableLookup(BufrIdentificationSection ids, NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableA a, NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableB b, NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableD d) throws NetcdfSharp.java.io.IOException
	  public TableLookup(BufrIdentificationSection ids, TableA a, TableB b, TableD d)
	  {
		this.wmoTableB = BufrTables.getWmoTableB(ids.MasterTableVersion);
		this.wmoTableD = BufrTables.getWmoTableD(ids.MasterTableVersion);
		this.localTableA = a;
		this.localTableB = b;
		this.localTableD = d;
		this.mode = BufrTables.Mode.localOverride;
	  }

	  public virtual string WmoTableBName
	  {
		  get
		  {
			return wmoTableB.Name;
		  }
	  }

	  public virtual string LocalTableAName
	  {
		  get
		  {
			return localTableA == null ? "none" : localTableA.Name;
		  }
	  }

	  public virtual string LocalTableBName
	  {
		  get
		  {
			return localTableB == null ? "none" : localTableB.Name;
		  }
	  }

	  public virtual string LocalTableDName
	  {
		  get
		  {
			return localTableD == null ? "none" : localTableD.Name;
		  }
	  }

	  public virtual string WmoTableDName
	  {
		  get
		  {
			return wmoTableD.Name;
		  }
	  }

	  public virtual BufrTables.Mode Mode
	  {
		  get
		  {
			return mode;
		  }
	  }

	  public virtual TableA LocalTableA
	  {
		  get
		  {
			return localTableA;
		  }
	  }

	  public virtual TableB LocalTableB
	  {
		  get
		  {
			return localTableB;
		  }
	  }

	  public virtual TableD LocalTableD
	  {
		  get
		  {
			return localTableD;
		  }
	  }

	  public virtual TableA.Descriptor getDescriptorTableA(int code)
	  {
		if (localTableA != null)
		{
		  return localTableA.getDescriptor(code);
		}
		else
		{
		  return null;
		}
	  }

	  public virtual TableB.Descriptor getDescriptorTableB(short fxy)
	  {
		TableB.Descriptor b = null;
		bool isWmoRange = Descriptor.isWmoRange(fxy);

		if (isWmoRange && (mode == BufrTables.Mode.wmoOnly))
		{
		  b = wmoTableB.getDescriptor(fxy);

		}
		else if (isWmoRange && (mode == BufrTables.Mode.wmoLocal))
		{
		  b = wmoTableB.getDescriptor(fxy);
		  if ((b == null) && (localTableB != null))
		  {
			b = localTableB.getDescriptor(fxy);
		  }

		}
		else if (isWmoRange && (mode == BufrTables.Mode.localOverride))
		{
		  if (localTableB != null)
		  {
			b = localTableB.getDescriptor(fxy);
		  }
		  if (b == null)
		  {
			b = wmoTableB.getDescriptor(fxy);
		  }
		  else
		  {
			b.LocalOverride = true;
		  }

		}
		else if (!isWmoRange)
		{
		  if (localTableB != null)
		  {
			b = localTableB.getDescriptor(fxy);
		  }
		}

		if (b == null)
		{ // look forward in standard WMO table; often the version number of the message is wrong
		  b = BufrTables.WmoTableBlatest.getDescriptor(fxy);
		}

		if (b == null && showErrors)
		{
		  log.warn(" TableLookup cant find Table B descriptor = {} in tables {}, {} mode={}", Descriptor.makeString(fxy), LocalTableBName, WmoTableBName, mode);
		}
		return b;
	  }

	  public virtual TableD.Descriptor getDescriptorTableD(short fxy)
	  {
		TableD.Descriptor d = null;
		bool isWmoRange = Descriptor.isWmoRange(fxy);

		if (isWmoRange && (mode == BufrTables.Mode.wmoOnly))
		{
		  d = wmoTableD.getDescriptor(fxy);

		}
		else if (isWmoRange && (mode == BufrTables.Mode.wmoLocal))
		{
		  d = wmoTableD.getDescriptor(fxy);
		  if ((d == null) && (localTableD != null))
		  {
			d = localTableD.getDescriptor(fxy);
		  }

		}
		else if (isWmoRange && (mode == BufrTables.Mode.localOverride))
		{
		  if (localTableD != null)
		  {
			d = localTableD.getDescriptor(fxy);
		  }
		  if (d == null)
		  {
			d = wmoTableD.getDescriptor(fxy);
		  }
		  else
		  {
			d.LocalOverride = true;
		  }

		}
		else
		{
		  if (localTableD != null)
		  {
			d = localTableD.getDescriptor(fxy);
		  }
		}

		if (d == null)
		{ // look forward in standard WMO table; often the version number of the message is wrong
		  d = BufrTables.WmoTableDlatest.getDescriptor(fxy);
		}

		if (d == null && showErrors)
		{
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
		  log.warn(string.Format(" TableLookup cant find Table D descriptor {0} in tables {1},{2} mode={3}%n", Descriptor.makeString(fxy), LocalTableDName, WmoTableDName, mode));
		}
		return d;
	  }

	}

}
