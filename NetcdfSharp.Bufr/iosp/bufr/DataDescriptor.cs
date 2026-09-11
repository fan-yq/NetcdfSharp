// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using Sequence = NetcdfSharp.ucar.nc2.Sequence;
	using TableC = NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableC;
	using TableB = NetcdfSharp.ucar.nc2.iosp.bufr.tables.TableB;

	/// <summary>
	/// Essentially a TableB entry, modified by any relevent TableC operators.
	/// TableD has been expanded.
	/// Replication gets made into nested DataDescriptors, which we map to Structures (fixed replication) or
	/// Sequences (deferred replication).
	/// Most of the processing is done by DataDescriptorTreeConstructor.convert().
	/// Here we encapsulate the final result, ready to map to the CDM.
	/// 
	/// @author caron
	/// @since Apr 5, 2008
	/// </summary>
	public class DataDescriptor
	{
	  private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(DataDescriptor));

	  ////////////////////////////////

	  // from the TableB.Descriptor
	  internal short fxy;
	  internal int f, x, y;
	  internal string name;
	  private string units, desc, source;
	  private bool localOverride;
	  internal bool bad; // no descriptor found

	  // may get modified by TableC operators
	  internal int scale;
	  internal int refVal;
	  internal int bitWidth;
	  internal int type; // 0 = isNumeric, 1 = isString, 2 = isEnum, 3 = compound;

	  // replication info
	  internal IList<DataDescriptor> subKeys;
	  internal int replication = 1; // number of replications, essentially dk.y when sk.f == 1
	  internal int replicationCountSize; // for delayed replication : size of count in bits
	  internal int repetitionCountSize; // for delayed repetition

	  internal AssociatedField assField; // associated field == 02 04 Y, Y number of extra bits
	  internal Sequence refersTo; // needed for nested sequence objects
	  internal DataDescriptorTreeConstructor.DataPresentIndicator dpi;

	  internal DataDescriptor()
	  {
	  }

	  public DataDescriptor(short fxy, BufrTableLookup lookup)
	  {
		this.fxy = fxy;
		this.f = (fxy & 0xC000) >> 14;
		this.x = (fxy & 0x3F00) >> 8;
		this.y = fxy & 0xFF;

		TableB.Descriptor db;
		if (f == 0)
		{
		  db = lookup.getDescriptorTableB(fxy);
		  if (db != null)
		  {
			Descriptor = db;
		  }
		  else
		  {
			bad = true;
			this.name = "*NOT FOUND";
		  }
		}

		else if (f == 1) // replication
		{
		  this.type = 3; // compound
		}

		else if (f == 2)
		{
		  this.name = TableC.getOperatorName(x);
		}
	  }

	  /// <summary>
	  /// Test if unit string indicates that the data are 7-bit coded characters following
	  /// the International Reference Alphabet (formally known as the International Alphabet
	  /// No.5 (IA5)) Recommendation/International Standard from the International Telegraph
	  /// and Telephone Consultative Committee (CCITT)
	  /// 
	  /// https://www.itu.int/rec/T-REC-T.50/en
	  /// </summary>
	  /// <param name="unitString"> unit </param>
	  /// <returns> If true, treat the data as 7-bit coded International Reference Alphabet Characters </returns>
	  public static bool isInternationalAlphabetUnit(string unitString)
	  {
		string testUnitString = unitString.ToLower();
		return testUnitString.StartsWith("ccitt", StringComparison.Ordinal);
	  }

	  /// <summary>
	  /// Test if the unit string indicates that we are dealing with data associated with a code table
	  /// </summary>
	  /// <param name="unitString"> unit </param>
	  /// <returns> If true, the unit indicates we are working with data associated with a code table </returns>
	  public static bool isCodeTableUnit(string unitString)
	  {
		string testUnitString = unitString.ToLower();
		return testUnitString.Equals("Code Table", StringComparison.OrdinalIgnoreCase) || testUnitString.Equals("Code_Table", StringComparison.OrdinalIgnoreCase) || testUnitString.StartsWith("codetable", StringComparison.Ordinal);
	  }

	  /// <summary>
	  /// Test if the unit string indicates that we are dealing with data associated with a flag table
	  /// </summary>
	  /// <param name="unitString"> unit </param>
	  /// <returns> If true, the unit indicates we are working with data associated with a flag table </returns>
	  public static bool isFlagTableUnit(string unitString)
	  {
		string testUnitString = unitString.ToLower();
		return testUnitString.Equals("Flag Table", StringComparison.OrdinalIgnoreCase) || testUnitString.Equals("Flag_Table", StringComparison.OrdinalIgnoreCase) || testUnitString.StartsWith("flagtable", StringComparison.Ordinal);
	  }

	  private TableB.Descriptor Descriptor
	  {
		  set
		  {
			this.name = value.Name.Trim();
			this.units = value.Units.Trim();
			this.desc = value.Desc;
			this.refVal = value.RefVal;
			this.scale = value.Scale;
			this.bitWidth = value.DataWidth;
			this.localOverride = value.LocalOverride;
			this.source = value.Source;

			if (isInternationalAlphabetUnit(units))
			{
			  this.type = 1; // String
			}

			// LOOK what about flag table ??
			if (isCodeTableUnit(units))
			{
			  this.type = 2; // enum
			}
		  }
	  }

	  /*
	   * for dpi fields
	   * DataDescriptor makeStatField(String statType) {
	   * DataDescriptor statDD = new DataDescriptor();
	   * statDD.name = name + "_" + statType;
	   * statDD.units = units;
	   * statDD.refVal = 0;
	   * 
	   * return statDD;
	   * }
	   */

	  // for associated fields
	  internal virtual DataDescriptor makeAssociatedField(int bitWidth)
	  {
		DataDescriptor assDD = new DataDescriptor();
		assDD.name = name + "_associated_field";
		assDD.units = "";
		assDD.refVal = 0;
		assDD.scale = 0;
		assDD.bitWidth = bitWidth;
		assDD.type = 0;

		assDD.f = 0;
		assDD.x = 31;
		assDD.y = 22;
		assDD.fxy = (short)((f << 14) + (x << 8) + (y));

		return assDD;
	  }

	  internal class AssociatedField
	  {
		internal int nbits;
		internal int nfields;
		internal string dataFldName;

		internal AssociatedField(int nbits)
		{
		  this.nbits = nbits;
		}
	  }

	  public virtual IList<DataDescriptor> SubKeys
	  {
		  get
		  {
			return subKeys;
		  }
	  }

	  internal virtual bool OkForVariable
	  {
		  get
		  {
			return (f == 0) || (f == 1) || ((f == 2) && (x == 5) || ((f == 2) && (x == 24) && (y == 255)));
		  }
	  }

	  public virtual bool Local
	  {
		  get
		  {
			if ((f == 0) || (f == 3))
			{
			  return (x >= 48) || (y >= 192);
			}
			return false;
		  }
	  }

	  public virtual bool LocalOverride
	  {
		  get
		  {
			return localOverride;
		  }
	  }

	  public virtual string FxyName
	  {
		  get
		  {
			return NetcdfSharp.ucar.nc2.iosp.bufr.Descriptor.makeString(f, x, y);
		  }
	  }


	  public virtual short Fxy
	  {
		  get
		  {
			return fxy;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string Source
	  {
		  get
		  {
			return source;
		  }
	  }

	  public virtual int Type
	  {
		  get
		  {
			return type;
		  }
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

	  public virtual string Units
	  {
		  get
		  {
			return units;
		  }
	  }

	  public virtual string Desc
	  {
		  get
		  {
			return desc;
		  }
	  }

	  public virtual float convert(long raw)
	  {
		if (BufrNumbers.isMissing(raw, bitWidth))
		{
		  return float.NaN;
		}

		// bpacked = (value * 10^scale - refVal)
		// value = (bpacked + refVal) / 10^scale
		float fscale = (float) Math.Pow(10.0, -scale); // LOOK precompute ??
		float fval = (raw + refVal);
		return fscale * fval;
	  }

	  public static float convert(long raw, int scale, int refVal, int bitWidth)
	  {
		if (BufrNumbers.isMissing(raw, bitWidth))
		{
		  return float.NaN;
		}

		// bpacked = (value * 10^scale - refVal)
		// value = (bpacked + refVal) / 10^scale
		float fscale = (float) Math.Pow(10.0, -scale); // LOOK precompute ??
		float fval = (raw + refVal);
		return fscale * fval;
	  }

	  /// <summary>
	  /// Transfer info from the "proto message" to another message with the exact same structure.
	  /// </summary>
	  /// <param name="fromList"> transfer from here </param>
	  /// <param name="toList"> to here </param>
	  internal static void transferInfo(IList<DataDescriptor> fromList, IList<DataDescriptor> toList)
	  { // get info from proto
																							 // message
		if (fromList.Count != toList.Count)
		{
		  throw new ArgumentException("list sizes dont match " + fromList.Count + " != " + toList.Count);
		}

		for (int i = 0; i < fromList.Count; i++)
		{
		  DataDescriptor from = fromList[i];
		  DataDescriptor to = toList[i];
		  to.refersTo = from.refersTo;
		  to.name = from.name;

		  if (from.SubKeys != null)
		  {
			transferInfo(from.SubKeys, to.SubKeys);
		  }
		}
	  }

	  ///////////////////////////////////////////////////////////////////////////////////////////////
	  private int total_nbytesCDM;

	  /// <summary>
	  /// count the bits used by the data in this dd and its children
	  /// only accurate for not compressed, and not variable length
	  /// </summary>
	  /// <returns> bits used by the data in the file </returns>
	  internal virtual int countBits()
	  {
		int total_nbits = 0;
		total_nbytesCDM = 0;

		foreach (DataDescriptor dd in subKeys)
		{
		  if (dd.subKeys != null)
		  {
			total_nbits += dd.countBits();
			total_nbytesCDM += dd.total_nbytesCDM;

		  }
		  else if (dd.f == 0)
		  {
			total_nbits += dd.bitWidth;
			total_nbytesCDM += dd.ByteWidthCDM;
		  }
		}

		// replication
		if (replication > 1)
		{
		  total_nbits *= replication;
		  total_nbytesCDM *= replication;
		}

		return total_nbits;
	  }

	  public virtual int BitWidth
	  {
		  get
		  {
			return bitWidth;
		  }
	  }

	  /// <summary>
	  /// Get the number of bytes the CDM datatype will take
	  /// </summary>
	  /// <returns> the number of bytes the CDM datatype will take </returns>
	  internal virtual int ByteWidthCDM
	  {
		  get
		  {
			if (type == 1) // string
			{
			  return bitWidth / 8;
			}

			if (type == 3) // compound
			{
			  return total_nbytesCDM;
			}

			// numeric or enum
			if (bitWidth < 9)
			{
			  return 1;
			}
			if (bitWidth < 17)
			{
			  return 2;
			}
			if (bitWidth < 33)
			{
			  return 4;
			}
			return 8;
		  }
	  }

	  public override string ToString()
	  {
		string id = FxyName;
		StringBuilder sbuff = new StringBuilder();
		if (f == 0)
		{
		  sbuff.Append(FxyName).Append(": ");
		  sbuff.Append(name).Append(" units=").Append(units);
		  if (type == 0)
		  {
			sbuff.Append(" scale=").Append(scale).Append(" refVal=").Append(refVal);
			sbuff.Append(" nbits=").Append(bitWidth);
		  }
		  else if (type == 1)
		  {
			sbuff.Append(" nchars=").Append(bitWidth / 8);
		  }
		  else
		  {
			sbuff.Append(" enum nbits=").Append(bitWidth);
		  }

		}
		else if (f == 1)
		{
		  sbuff.Append(id).Append(": ").Append("Replication");
		  if (replication != 1)
		  {
			sbuff.Append(" count=").Append(replication);
		  }
		  if (replicationCountSize != 0)
		  {
			sbuff.Append(" replicationCountSize=").Append(replicationCountSize);
		  }
		  if (repetitionCountSize != 0)
		  {
			sbuff.Append(" repetitionCountSize=").Append(repetitionCountSize);
		  }
		  if (!string.ReferenceEquals(name, null))
		  {
			sbuff.Append(": " + name);
		  }

		}
		else if (f == 2)
		{
		  string desc = TableC.getOperatorName(x);
		  if (string.ReferenceEquals(desc, null))
		  {
			desc = "Operator";
		  }
		  sbuff.Append(id).Append(": ").Append(desc);

		}
		else
		{
		  sbuff.Append(id).Append(": ").Append(name);
		}

		return sbuff.ToString();
	  }

	  /////////////////////////////////
	  // stuff for the root
	  internal bool isVarLength;
	  internal bool isBad;
	  internal int total_nbits;

	  public virtual int TotalBits
	  {
		  get
		  {
			return total_nbits;
		  }
	  }

	  public virtual bool VarLength
	  {
		  get
		  {
			return isVarLength;
		  }
	  }

	  ////////////////////////////////////////////////////////////////////////////////////////////////
	  // LOOK need different hashCode, reader assumes using object id
	  public virtual bool equals2(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}

		DataDescriptor that = (DataDescriptor) o;

		if (fxy != that.fxy)
		{
		  return false;
		}
		if (replication != that.replication)
		{
		  return false;
		}
		if (type != that.type)
		{
		  return false;
		}
		return object.Equals(subKeys, that.subKeys);
	  }

	  public virtual int hashCode2()
	  {
		int result = (int) fxy;
		result = 31 * result + type;
		result = 31 * result + getListHash();
		result = 31 * result + replication;
		return result;
	  }

	  // has to use hashCode2, so cant use list.hashCode()
	  private int getListHash()
	  {
		if (subKeys == null)
		{
		  return 0;
		}
		int result = 1;
		foreach (DataDescriptor e in subKeys)
		{
		  result = 31 * result + (e == null ? 0 : e.hashCode2());
		}
		return result;
	  }

	}
  }
