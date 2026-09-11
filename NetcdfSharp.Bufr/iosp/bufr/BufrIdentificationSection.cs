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
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;


	/// <summary>
	/// A class representing the IdentificationSection (section 1) of a BUFR record.
	/// Handles editions 2,3,4.
	/// 
	/// @author Robb Kambic
	/// @author caron
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class BufrIdentificationSection
	public class BufrIdentificationSection
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(BufrIdentificationSection));
	  private const bool warnDate = false;

	  /// <summary>
	  /// Master Table number.
	  /// </summary>
	  private readonly int master_table;

	  /// <summary>
	  /// Identification of subcenter .
	  /// </summary>
	  private readonly int subcenter_id;

	  /// <summary>
	  /// Identification of center.
	  /// </summary>
	  private readonly int center_id;

	  /// <summary>
	  /// Update Sequence Number.
	  /// </summary>
	  private readonly int update_sequence;

	  /// <summary>
	  /// Optional section exists.
	  /// </summary>
	  private readonly bool hasOptionalSection;
	  private readonly int optionalSectionLen;
	  private readonly long optionalSectionPos;

	  /// <summary>
	  /// Data category.
	  /// </summary>
	  private readonly int category;

	  /// <summary>
	  /// Data sub category.
	  /// </summary>
	  private readonly int subCategory;

	  private readonly int localSubCategory; // edition >= 4

	  /// <summary>
	  /// Table Version numbers.
	  /// </summary>
	  private readonly int master_table_version;
	  private readonly int local_table_version;

	  /// <summary>
	  /// Time of the obs (nominal)
	  /// </summary>
	  private readonly int year, month, day, hour, minute, second;

	  private readonly byte[] localUse;

	  // *** constructors *******************************************************

	  /// <summary>
	  /// Constructs a <tt>BufrIdentificationSection</tt> object from a raf.
	  /// </summary>
	  /// <param name="raf"> RandomAccessFile with Section 1 content </param>
	  /// <param name="is"> the BufrIndicatorSection, needed for the bufr edition number </param>
	  /// <exception cref="IOException"> if raf contains no valid BUFR file </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public BufrIdentificationSection(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, BufrIndicatorSection is) throws NetcdfSharp.java.io.IOException
	  public BufrIdentificationSection(RandomAccessFile raf, BufrIndicatorSection @is)
	  {

		// section 1 octet 1-3 (length of section)
		int length = BufrNumbers.int3(raf);

		// master table octet 4
		master_table = raf.read();

		if (@is.BufrEdition < 4)
		{
		  if (length < 17)
		  {
			throw new IOException("Invalid BUFR message on " + raf.Location);
		  }

		  if (@is.BufrEdition == 2)
		  {
			subcenter_id = 255;
			// Center octet 5-6
			center_id = BufrNumbers.int2(raf);

		  }
		  else
		  { // edition 3
			// Center octet 5
			subcenter_id = raf.read();
			// Center octet 6
			center_id = raf.read();
		  }

		  // Update sequence number octet 7
		  update_sequence = raf.read();

		  // Optional section octet 8
		  int optional = raf.read();
		  hasOptionalSection = (optional & 0x80) != 0;

		  // Category octet 9
		  category = raf.read();

		  // Category octet 10
		  subCategory = raf.read();
		  localSubCategory = -1; // not used

		  // master table version octet 11
		  master_table_version = raf.read();

		  // local table version octet 12
		  local_table_version = raf.read();

		  // octets 13-17 (reference time of forecast)
		  int lyear = raf.read();
		  if (lyear > 100)
		  {
			lyear -= 100;
		  }
		  year = lyear + 2000;
		  int tempMonth = raf.read();
		  month = (tempMonth == 0) ? 1 : tempMonth; // joda time does not allow 0 month
		  int tempDay = raf.read();
		  day = (tempDay == 0) ? 1 : tempDay; // joda time does not allow 0 day
		  hour = raf.read();
		  minute = raf.read();
		  second = 0;
		  if (warnDate && (tempMonth == 0 || tempDay == 0))
		  {
			// From manual on codes
			// When accuracy of the time does not define a time unit, then the value for this unit shall be set to zero
			// (e.g. for a
			// SYNOP observation at 09 UTC, minute = 0, second = 0.
			// NCEP codes their BUFR table messages with 0/0/0 0:0:0 in edition 3
			log.warn(raf.Location + ": month or day is zero, set to 1. {}/{}/{} {}:{}:{}", year, tempMonth, tempDay, hour, minute, second);
		  }

		  int n = length - 17;
		  localUse = new byte[n];
		  int nRead = raf.read(localUse);
		  if (nRead != localUse.Length)
		  {
			throw new IOException("Error reading BUFR local use field.");
		  }
		}
		else
		{ // BUFR Edition 4 and above are slightly different
		  if (length < 22)
		  {
			throw new IOException("Invalid BUFR message");
		  }

		  // Center octet 5 - 6
		  center_id = BufrNumbers.int2(raf);

		  // Sub Center octet 7-8
		  subcenter_id = BufrNumbers.int2(raf);

		  // Update sequence number octet 9
		  update_sequence = raf.read();

		  // Optional section octet 10
		  int optional = raf.read();
		  // Most Sig. Bit = 1 : has optional section
		  // 0 : does not have an optional section
		  hasOptionalSection = (optional & 0x80) != 0;

		  // Category octet 11
		  category = raf.read();

		  // International Sub Category octet 12
		  subCategory = raf.read();

		  // Local Sub Category Octet 13 - just read this for now
		  localSubCategory = raf.read();

		  // master table version octet 14
		  master_table_version = raf.read();

		  // local table version octet 15
		  local_table_version = raf.read();
		  // octets 16-22 (reference time of forecast)

		  // Octet 16-17 is the 4-digit year
		  year = BufrNumbers.int2(raf);
		  month = raf.read();
		  day = raf.read();
		  hour = raf.read();
		  minute = raf.read();
		  second = raf.read();

		  int n = length - 22;
		  localUse = new byte[n];
		  int nRead = raf.read(localUse);
		  if (nRead != localUse.Length)
		  {
			throw new IOException("Error reading BUFR local use field.");
		  }
		}

		// skip optional section, but store position so can read if caller wants it
		if (hasOptionalSection)
		{
		  int optionalLen = BufrNumbers.int3(raf);
		  if (optionalLen % 2 != 0)
		  {
			optionalLen++;
		  }
		  optionalSectionLen = optionalLen - 4;
		  raf.skipBytes(1);
		  optionalSectionPos = raf.FilePointer;
		  raf.skipBytes(optionalSectionLen);

		}
		else
		{
		  optionalSectionLen = -1;
		  optionalSectionPos = -1;
		}
	  }

	  /// <summary>
	  /// Identification of center.
	  /// </summary>
	  /// <returns> center id as int </returns>
	  public int CenterId
	  {
		  get
		  {
			return center_id;
		  }
	  }

	  /// <summary>
	  /// Identification of subcenter.
	  /// </summary>
	  /// <returns> subcenter as int </returns>
	  public int SubCenterId
	  {
		  get
		  {
			return subcenter_id;
		  }
	  }

	  /// <summary>
	  /// Get update sequence.
	  /// </summary>
	  /// <returns> update_sequence </returns>
	  public int UpdateSequence
	  {
		  get
		  {
			return update_sequence;
		  }
	  }

	  /// <summary>
	  /// return record header time as a CalendarDate
	  /// </summary>
	  /// <returns> referenceTime </returns>
	  public CalendarDate ReferenceTime
	  {
		  get
		  {
			int sec = (second < 0 || second > 59) ? 0 : second;
			return CalendarDate.of(null, year, month, day, hour, minute, sec);
		  }
	  }

	  public int Category
	  {
		  get
		  {
			return category;
		  }
	  }

	  public int SubCategory
	  {
		  get
		  {
			return subCategory;
		  }
	  }

	  public int LocalSubCategory
	  {
		  get
		  {
			return localSubCategory;
		  }
	  }

	  public int MasterTableId
	  {
		  get
		  {
			return master_table;
		  }
	  }

	  public int MasterTableVersion
	  {
		  get
		  {
			return master_table_version;
		  }
	  }

	  public int LocalTableVersion
	  {
		  get
		  {
			return local_table_version;
		  }
	  }

	  /// <summary>
	  /// last bytes of the id section are "reserved for local use by ADP centers.
	  /// </summary>
	  /// <returns> local use bytes, if any. </returns>
	  public byte[] LocalUseBytes
	  {
		  get
		  {
			return localUse;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public final byte[] getOptiondsalSection(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public byte[] getOptiondsalSection(RandomAccessFile raf)
	  {
		if (!hasOptionalSection)
		{
		  return null;
		}

		byte[] optionalSection = new byte[optionalSectionLen - 4];
		raf.seek(optionalSectionPos);
		int nRead = raf.read(optionalSection);
		if (nRead != optionalSection.Length)
		{
		  log.warn("Error reading optional section -- expected " + optionalSection.Length + " but read " + nRead);
		}
		return optionalSection;
	  }

	}

}
