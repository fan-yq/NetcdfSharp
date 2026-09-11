// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using Matcher = NetcdfSharp.com.google.re2j.Matcher;
	using Pattern = NetcdfSharp.com.google.re2j.Pattern;
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Encapsolates a complete BUFR message.
	/// A message has a DataDescriptor and one or more "datasets" aka "data subsets" aka "observations" aka "obs".
	/// Table lookup is done through getLookup().
	/// </summary>
	public class Message
	{
	  private static readonly Pattern wmoPattern = Pattern.compile(".*([IJ]..... ....) .*");

	  public BufrIndicatorSection @is;
	  public BufrIdentificationSection ids;
	  public BufrDataDescriptionSection dds;
	  public BufrDataSection dataSection;

	  private RandomAccessFile raf;
	  private BufrTableLookup lookup;
	  private DataDescriptor root;

	  private string header; // wmo header
	  private long startPos; // starting pos in raf
	  private byte[] raw; // raw bytes

	  // bit counting
	  internal BitCounterUncompressed[] counterDatasets; // uncompressed: one for each dataset
	  internal int msg_nbits;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Message(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, BufrIndicatorSection is, BufrIdentificationSection ids, BufrDataDescriptionSection dds, BufrDataSection dataSection) throws NetcdfSharp.java.io.IOException
	  public Message(RandomAccessFile raf, BufrIndicatorSection @is, BufrIdentificationSection ids, BufrDataDescriptionSection dds, BufrDataSection dataSection)
	  {
		this.raf = raf;
		this.@is = @is;
		this.ids = ids;
		this.dds = dds;
		this.dataSection = dataSection;
		lookup = BufrTableLookup.factory(this);
	  }

	  internal virtual TableLookup TableLookup
	  {
		  set
		  {
			this.lookup.TableLookup = value;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void close() throws NetcdfSharp.java.io.IOException
	  public virtual void close()
	  {
		if (raf != null)
		{
		  raf.close();
		}
	  }

	  /// <summary>
	  /// Get number of datasets in this message.
	  /// </summary>
	  /// <returns> number of datasets in this message </returns>
	  public virtual int NumberDatasets
	  {
		  get
		  {
			return dds.NumberDatasets;
		  }
	  }

	  public virtual CalendarDate ReferenceTime
	  {
		  get
		  {
			return ids.ReferenceTime;
		  }
	  }

	  ///////////////////////////////////////////////////////////////////////////

	  // the WMO header is in here somewhere when the message comes over the IDD
	  public virtual string Header
	  {
		  set
		  {
			this.header = value;
		  }
		  get
		  {
			return header;
		  }
	  }


	  // where the message starts in the file
	  public virtual long StartPos
	  {
		  set
		  {
			this.startPos = value;
		  }
		  get
		  {
			return startPos;
		  }
	  }


	  public virtual byte[] RawBytes
	  {
		  set
		  {
			this.raw = value;
		  }
		  get
		  {
			return raw;
		  }
	  }


	  public virtual string extractWMO()
	  {
		Matcher matcher = wmoPattern.matcher(header);
		if (!matcher.matches())
		{
		  return "";
		}
		return matcher.group(1);
	  }

	  /// <summary>
	  /// Get the byte length of the entire BUFR record.
	  /// </summary>
	  /// <returns> length in bytes of BUFR record </returns>
	  public virtual long MessageSize
	  {
		  get
		  {
			return @is.BufrLength;
		  }
	  }

	  /// <summary>
	  /// Get the root of the DataDescriptor tree.
	  /// </summary>
	  /// <returns> root DataDescriptor </returns>
	  public virtual DataDescriptor RootDataDescriptor
	  {
		  get
		  {
			if (root == null)
			{
			  root = (new DataDescriptorTreeConstructor()).factory(lookup, dds);
			}
			return root;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public boolean usesLocalTable() throws NetcdfSharp.java.io.IOException
	  public virtual bool usesLocalTable()
	  {
		DataDescriptor root = RootDataDescriptor;
		return usesLocalTable(root);
	  }

	  private bool usesLocalTable(DataDescriptor dds)
	  {
		foreach (DataDescriptor key in dds.SubKeys)
		{
		  if (key.Local)
		  {
			return true;
		  }
		  if ((key.SubKeys != null) && usesLocalTable(key))
		  {
			return true;
		  }
		}
		return false;
	  }

	  /// <summary>
	  /// Check if this message contains a BUFR table
	  /// </summary>
	  /// <returns> true if message contains a BUFR table </returns>
	  public virtual bool containsBufrTable()
	  {
		foreach (short? key in dds.DataDescriptors)
		{
		  if (Descriptor.isBufrTable(key.Value))
		  {
			return true;
		  }
		}
		return false;
	  }

	  /// <summary>
	  /// Check if all descriptors were found in the tables.
	  /// </summary>
	  /// <returns> true if all dds were found. </returns>
	  public virtual bool TablesComplete
	  {
		  get
		  {
			DataDescriptor root = RootDataDescriptor;
			return !root.isBad;
		  }
	  }

	  public virtual BufrTableLookup Lookup
	  {
		  get
		  {
			return lookup;
		  }
	  }

	  ////////////////////////////////////////////////////////////////////////
	  // bit counting

	  public virtual bool BitCountOk
	  {
		  get
		  {
			_ = RootDataDescriptor; // make sure root is calculated
			_ = TotalBits; // make sure bits are counted
			// int nbitsGiven = 8 * (dataSection.getDataLength() - 4);
			int nbytesCounted = CountedDataBytes;
			int nbytesGiven = dataSection.DataLength;
			return Math.Abs(nbytesCounted - nbytesGiven) <= 1; // radiosondes dataLen not even number of bytes
		  }
	  }

	  public virtual int CountedDataBytes
	  {
		  get
		  {
			int msg_nbytes = msg_nbits / 8;
			if (msg_nbits % 8 != 0)
			{
			  msg_nbytes++;
			}
			msg_nbytes += 4;
			if (msg_nbytes % 2 != 0)
			{
			  msg_nbytes++; // LOOK seems to be violated by some messages
			}
			return msg_nbytes;
		  }
	  }

	  public virtual int CountedDataBits
	  {
		  get
		  {
			return msg_nbits;
		  }
	  }


	  /// <summary>
	  /// Get the offset of this obs from the start of the message data.
	  /// Use only for non compressed data
	  /// </summary>
	  /// <param name="obsOffsetInMessage"> index of obs in the message </param>
	  /// <returns> offset in bits
	  ///         <p/>
	  ///         public int getBitOffset(int obsOffsetInMessage) {
	  ///         if (dds.isCompressed())
	  ///         throw new IllegalArgumentException("cant call BufrMessage.getBitOffset() on compressed message");
	  ///         <p/>
	  ///         if (!root.isVarLength)
	  ///         return root.total_nbits * obsOffsetInMessage;
	  ///         <p/>
	  ///         getTotalBits(); // make sure its been set
	  ///         return nestedTableCounter[obsOffsetInMessage].getStartBit();
	  ///         } </returns>

	  public virtual BitCounterUncompressed getBitCounterUncompressed(int obsOffsetInMessage)
	  {
		if (dds.Compressed)
		{
		  throw new ArgumentException("cant call BufrMessage.getBitOffset() on compressed message");
		}

		calcTotalBits(null); // make sure its been set
		return counterDatasets[obsOffsetInMessage];
	  }

	  /// <summary>
	  /// This is the total number of bits taken by the data in the data section of the message.
	  /// This is the counted number.
	  /// </summary>
	  /// <returns> total number of bits </returns>
	  public virtual int TotalBits
	  {
		  get
		  {
			if (msg_nbits == 0)
			{
			  calcTotalBits(null);
			}
			return msg_nbits;
		  }
	  }

	  // sets msg_nbits as side-effect
	  public virtual int calcTotalBits(Formatter @out)
	  {
		try
		{
		  if (!dds.Compressed)
		  {
			MessageUncompressedDataReader reader = new MessageUncompressedDataReader();
			reader.readData(null, this, raf, null, false, @out);
		  }
		  else
		  {
			MessageCompressedDataReader reader = new MessageCompressedDataReader();
			reader.readData(null, this, raf, null, @out);
		  }
		}
		catch (IOException)
		{
		  return 0;
		}
		return msg_nbits;
	  }

	  ///////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Override hashcode to be consistent with equals.
	  /// </summary>
	  /// <returns> the hash code of dds.getDescriptors() </returns>
	  public override int GetHashCode()
	  {
		int result = 17;
		result += 37 * result + DDShashcode;
		// result += 37 * result + ids.getCenterId();
		// result += 37 * result + ids.getSubCenter_id();
		result += 37 * result + ids.Category;
		result += 37 * result + ids.SubCategory;
		return result;
	  }

	  public virtual int DDShashcode
	  {
		  get
		  {
			root = RootDataDescriptor;
			return root.hashCode2();
		  }
	  }

	  /// <summary>
	  /// BufrMessage is equal if they have the same dds.
	  /// </summary>
	  /// <param name="obj"> other BufrMessage </param>
	  /// <returns> true if equals </returns>
	  public override bool Equals(object obj)
	  {
		if (!(obj is Message))
		{
		  return false;
		}
		Message o = (Message) obj;
// JAVA TO C# CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
// ORIGINAL LINE: if (!dds.getDataDescriptors().equals(o.dds.getDataDescriptors()))
		if (!dds.DataDescriptors.SequenceEqual(o.dds.DataDescriptors))
		{
		  return false;
		}
		if (ids.CenterId != o.ids.CenterId)
		{
		  return false;
		}
		// if (ids.getSubCenter_id() != o.ids.getSubCenter_id()) return false;
		if (ids.Category != o.ids.Category)
		{
		  return false;
		}
		return ids.SubCategory == o.ids.SubCategory;
	  }

	  ////////////////////////////////////////////////////////////////////
	  // perhaps move this into a helper class - started from NetcdfSharp.ucar.bufr.Dump


// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void showMissingFields(NetcdfSharp.java.util.Formatter out) throws NetcdfSharp.java.io.IOException
	  public virtual void showMissingFields(Formatter @out)
	  {
		lookup.showMissingFields(dds.DataDescriptors, @out);
	  }

	  public virtual void dump(Formatter @out)
	  { // throws IOException {

		int listHash = dds.DataDescriptors.GetHashCode();
		@out.format(" BUFR edition %d time= %s wmoHeader=%s hash=[0x%x] listHash=[0x%x] (%d) %n", @is.BufrEdition, ReferenceTime, Header, GetHashCode(), listHash, listHash);
		@out.format("   Category= %s %n", lookup.CategoryFullName);
		@out.format("   Center= %s %n", lookup.CenterName);
		@out.format("   Table= %s %n", lookup.TableName);
		@out.format("    Table B= wmoTable= %s localTable= %s mode=%s%n", lookup.WmoTableBName, lookup.LocalTableBName, lookup.Mode);
		@out.format("    Table D= wmoTable= %s localTable= %s%n", lookup.WmoTableDName, lookup.LocalTableDName);

		@out.format("  DDS nsubsets=%d type=0x%x isObs=%b isCompressed=%b%n", dds.NumberDatasets, dds.DataType, dds.Observed, dds.Compressed);

		long startPos = @is.StartPos;
		long startData = dataSection.DataPos;
		@out.format("  startPos=%d len=%d endPos=%d dataStart=%d dataLen=%d dataEnd=%d %n", startPos, @is.BufrLength, (startPos + @is.BufrLength), startData, dataSection.DataLength, startData + dataSection.DataLength);

		dumpDesc(@out, dds.DataDescriptors, lookup, 4);

		@out.format("%n  CDM Nested Table=%n");
		DataDescriptor root = (new DataDescriptorTreeConstructor()).factory(lookup, dds);
		dumpKeys(@out, root, 4);

		/*
		 * int nbits = m.getTotalBits();
		 * int nbytes = (nbits % 8 == 0) ? nbits / 8 : nbits / 8 + 1;
		 * out.format("  totalBits = %d (%d bytes) outputBytes= %d isVarLen=%s isCompressed=%s\n\n",
		 * nbits, nbytes, root.getByteWidthCDM(), root.isVarLength(), m.dds.isCompressed());
		 */
	  }

	  private void dumpDesc(Formatter @out, IList<short> desc, BufrTableLookup table, int indent)
	  {
		if (desc == null)
		{
		  return;
		}

		foreach (short? fxy in desc)
		{
		  for (int i = 0; i < indent; i++)
		  {
			@out.format(" ");
		  }
		  Descriptor.show(@out, fxy.Value, table);
		  @out.format("%n");
		  int f = (fxy.Value & 0xC000) >> 14;
		  if (f == 3)
		  {
			IList<short> sublist = table.getDescriptorListTableD(fxy.Value);
			dumpDesc(@out, sublist, table, indent + 2);
		  }
		}
	  }

	  private void dumpKeys(Formatter @out, DataDescriptor tree, int indent)
	  {
		foreach (DataDescriptor key in tree.subKeys)
		{
		  for (int i = 0; i < indent; i++)
		  {
			@out.format(" ");
		  }
		  @out.format("%s%n", key);
		  if (key.SubKeys != null)
		  {
			dumpKeys(@out, key, indent + 2);
		  }
		}
	  }

	  public virtual void dumpHeader(Formatter @out)
	  {

		@out.format(" BUFR edition %d time= %s wmoHeader=%s %n", @is.BufrEdition, ReferenceTime, Header);
		@out.format("   Category= %d %s %s %n", lookup.Category, lookup.CategoryName, lookup.CategoryNo);
		@out.format("   Center= %s %s %n", lookup.CenterName, lookup.CenterNo);
		@out.format("   Table= %d.%d local= %d wmoTables= %s,%s localTables= %s,%s %n", ids.MasterTableId, ids.MasterTableVersion, ids.LocalTableVersion, lookup.WmoTableBName, lookup.WmoTableDName, lookup.LocalTableBName, lookup.LocalTableDName);

		@out.format("  DDS nsubsets=%d type=0x%x isObs=%b isCompressed=%b%n", dds.NumberDatasets, dds.DataType, dds.Observed, dds.Compressed);
	  }

	  public virtual void dumpHeaderShort(Formatter @out)
	  {
		@out.format(" %s, Cat= %s, Center= %s (%s), Table= %d.%d.%d %n", Header, lookup.CategoryName, lookup.CenterName, lookup.CenterNo, ids.MasterTableId, ids.MasterTableVersion, ids.LocalTableVersion);
	  }

	}

}
