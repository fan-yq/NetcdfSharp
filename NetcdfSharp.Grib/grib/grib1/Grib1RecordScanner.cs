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

namespace NetcdfSharp.ucar.nc2.grib.grib1
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using KMPMatch = NetcdfSharp.ucar.unidata.io.KMPMatch;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Scan files and extract Grib1Records. usage:
	/// 
	/// <pre>
	/// Grib1RecordScanner reader = new Grib1RecordScanner(raf);
	/// while (reader.hasNext()) {
	/// NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record gr = reader.next();
	/// Grib1SectionProductDefinition pds = gr.getPDSsection();
	/// Grib1SectionGridDefinition gds = gr.getGDSsection();
	/// ...
	/// }
	/// 
	/// </pre>
	/// 
	/// @author John
	/// @since 9/3/11
	/// </summary>
	public class Grib1RecordScanner
	{

	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib1RecordScanner));
	  private static readonly KMPMatch matcher = new KMPMatch(new byte[] {(byte)'G', (byte)'R', (byte)'I', (byte)'B'});
	  private const bool debug = false;
	  private const bool debugGds = false;
	  private const int maxScan = 16000;

	  internal static bool allowBadIsLength;
	  internal static bool allowBadDsLength; // ECMWF workaround

	  public static bool AllowBadIsLength
	  {
		  set
		  {
			Grib1RecordScanner.allowBadIsLength = value;
		  }
	  }

	  public static bool AllowBadDsLength
	  {
		  set
		  {
			Grib1RecordScanner.allowBadDsLength = value;
		  }
	  }

	  public static bool isValidFile(RandomAccessFile raf)
	  {
		try
		{
		  raf.seek(0);
		  bool found = raf.searchForward(matcher, maxScan); // look in first 16K
		  if (!found)
		  {
			return false;
		  }
		  raf.skipBytes(4); // will be positioned on byte 0 of indicator section
		  int len = GribNumbers.uint3(raf);
		  int edition = raf.read(); // read at byte 8
		  if (edition != 1)
		  {
			return false;
		  }

		  /*
		   * Due to a trick done by ECMWF's GRIBEX to support large GRIBs, we need a special treatment
		   * to fix the length of the GRIB message. See:
		   * https://software.ecmwf.int/wiki/display/EMOS/Changes+in+cycle+000281
		   * https://github.com/Unidata/thredds/issues/445
		   */
		  len = getFixedTotalLengthEcmwfLargeGrib(raf, len);

		  // check ending = 7777
		  if (len > raf.length())
		  {
			return false;
		  }
		  if (allowBadIsLength)
		  {
			return true;
		  }

		  raf.skipBytes(len - 12);
		  for (int i = 0; i < 4; i++)
		  {
			if (raf.read() != 55)
			{
			  return false;
			}
		  }
		  return true;

		}
		catch (IOException)
		{
		  return false;
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static int getFixedTotalLengthEcmwfLargeGrib(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, int len) throws NetcdfSharp.java.io.IOException
	  internal static int getFixedTotalLengthEcmwfLargeGrib(RandomAccessFile raf, int len)
	  {
		int lenActual = len;
		if ((len & 0x800000) == 0x800000)
		{
		  long pos0 = raf.FilePointer; // remember the actual pos
		  int lenS1 = GribNumbers.uint3(raf); // section1Length
		  raf.skipBytes(1); // table2Version
		  if (GribNumbers.@uint(raf) == 98)
		  { // center (if ECMWF make the black magic)
			raf.skipBytes(2); // generatingProcessIdentifier, gridDefinition
			int s1f = GribNumbers.@uint(raf); // section1Flags
			raf.skipBytes(lenS1 - (3 + 5)); // skips to next section
			int lenS2;
			int lenS3;
			if ((s1f & 128) == 128)
			{ // section2 GDS exists
			  lenS2 = GribNumbers.uint3(raf); // section2Length
			  raf.skipBytes(lenS2 - 3); // skips to next section
			}
			if ((s1f & 64) == 64)
			{ // section3 BMS exists
			  lenS3 = GribNumbers.uint3(raf); // section3Length
			  raf.skipBytes(lenS3 - 3); // skips to next section
			}
			int lenS4 = GribNumbers.uint3(raf); // section4Length
			if (lenS4 < 120)
			{ // here we are!!!!
			  lenActual = (len & 0x7FFFFF) * 120 - lenS4 + 4; // the actual totalLength
			}
		  }
		  raf.seek(pos0); // recall the pos
		}
		return lenActual;
	  }

	  ////////////////////////////////////////////////////////////

	  private readonly IDictionary<long, Grib1SectionGridDefinition> gdsMap = new Dictionary<long, Grib1SectionGridDefinition>();
	  private readonly RandomAccessFile raf;

	  private byte[] header;
	  private long lastPos;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib1RecordScanner(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib1RecordScanner(RandomAccessFile raf)
	  {
		this.raf = raf;
		raf.seek(0);
		raf.order(RandomAccessFile.BIG_ENDIAN);
		lastPos = 0;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public boolean hasNext() throws NetcdfSharp.java.io.IOException
	  public virtual bool hasNext()
	  {
		if (lastPos >= raf.length())
		{
		  return false;
		}
		bool more;
		long foundAt = 0;

		while (true)
		{ // scan until we get a GRIB-1 or more is false
		  raf.seek(lastPos);
		  more = raf.searchForward(matcher, -1); // will scan to end for a 'GRIB' string
		  if (!more)
		  {
			break;
		  }

		  foundAt = raf.FilePointer;
		  // see if its GRIB-1
		  raf.skipBytes(7);
		  int edition = raf.read();
		  if (edition == 1)
		  {
			break;
		  }
		  lastPos = raf.FilePointer; // not edition 1 ! could terminate ??
		}

		if (more)
		{
		  // read the header - stuff between the records
		  int sizeHeader = (int)(foundAt - lastPos);
		  if (sizeHeader > 100)
		  {
			sizeHeader = 100; // maximum 100 bytes, more likely to be garbage
		  }
		  long startPos = foundAt - sizeHeader;
		  header = new byte[sizeHeader];
		  raf.seek(startPos);
		  raf.readFully(header);
		  raf.seek(foundAt);
		  this.lastPos = foundAt; // ok start from here next time
		}

		return more;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Nullable public Grib1Record next() throws NetcdfSharp.java.io.IOException
	  public virtual Grib1Record next()
	  {

		Grib1SectionIndicator @is = null;
		try
		{
		  @is = new Grib1SectionIndicator(raf);
		  Grib1SectionProductDefinition pds = new Grib1SectionProductDefinition(raf);
		  Grib1SectionGridDefinition gds = pds.gdsExists() ? new Grib1SectionGridDefinition(raf) : new Grib1SectionGridDefinition(pds);
		  if (!pds.gdsExists() && debugGds)
		  {
			log.warn(" NO GDS: center = {}, GridDefinition={} file={}", pds.Center, pds.GridDefinition, raf.Location);
		  }

		  Grib1SectionBitMap bitmap = pds.bmsExists() ? new Grib1SectionBitMap(raf) : null;
		  Grib1SectionBinaryData dataSection = new Grib1SectionBinaryData(raf);

		  long ending = @is.EndPos;
		  long dataEnding = dataSection.StartingPosition + dataSection.Length;

		  if (dataEnding > @is.EndPos)
		  { // presumably corrupt
			// raf.seek(dataSection.getStartingPosition()); // go back to start of the dataSection, in hopes of salvaging
			log.warn("BAD GRIB-1 data message at " + dataSection.StartingPosition + " header= " + StringUtil2.cleanup(header) + " for=" + raf.Location);
			throw new System.InvalidOperationException("Illegal Grib1SectionBinaryData Message Length");
		  }

		  /*
		   * ecmwf offset by 1 bug - LOOK not sure if this is still needed
		   * // obtain BMS or BDS offset in the file for this product
		   * if (pds.getPdsVars().getCenter() == 98) { // check for ecmwf offset by 1 bug
		   * int length = GribNumbers.uint3(raf); // should be length of BMS
		   * if ((length + raf.getFilePointer()) < EOR) {
		   * dataOffset = raf.getFilePointer() - 3; // ok
		   * } else {
		   * // ECMWF off by 1 bug
		   * dataOffset = raf.getFilePointer() - 2;
		   * }
		   * } else {
		   * dataOffset = raf.getFilePointer();
		   * }
		   */

		  // look for duplicate gds
		  long crc = gds.calcCRC();
		  Grib1SectionGridDefinition gdsCached;
		  if (gdsMap.TryGetValue(crc, out gdsCached))
		  {
			gds = gdsCached;
		  }
		  else
		  {
			gdsMap[crc] = gds;
		  }

		  // check that end section is correct
		  bool foundEnding = checkEnding(ending);
		  log.debug(" read until {} grib ending at {} header ='{}' foundEnding={}", raf.FilePointer, ending, StringUtil2.cleanup(header), foundEnding);

		  if (!foundEnding && (allowBadIsLength || @is.isMessageLengthFixed))
		  {
			foundEnding = checkEnding(dataSection.StartingPosition + dataSection.Length);
		  }

		  if (!foundEnding && (allowBadDsLength || @is.isMessageLengthFixed))
		  {
			foundEnding = true;
		  }

		  if (foundEnding)
		  {
			lastPos = raf.FilePointer;
			return new Grib1Record(header, @is, gds, pds, bitmap, dataSection);
		  }

		  // skip this record
		  // lastPos = is.getEndPos() + 20; cant use is.getEndPos(), may be bad
		  lastPos += 20; // skip over the "GRIB" of this message
		  if (hasNext())
		  { // search forward for another one
			return next();
		  }

		}
		catch (Exception)
		{
		  long pos = (@is == null) ? -1 : @is.StartPos;
		  log.warn("Bad Grib1 record in file {}, skipping pos={}", raf.Location, pos);
		  // t.printStackTrace();
		  lastPos += 20; // skip over the "GRIB"
		  if (hasNext())
		  { // search forward for another one
			return next();
		  }
		}

		// EOF that hasNext() couldnt detect
		return null;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private boolean checkEnding(long ending) throws NetcdfSharp.java.io.IOException
	  private bool checkEnding(long ending)
	  {
		// check that end section = "7777" is correct
		raf.seek(ending - 4);
		for (int i = 0; i < 4; i++)
		{
		  if (raf.read() != 55)
		  {
			string clean = StringUtil2.cleanup(header);
			if (clean.Length > 40)
			{
			  clean = clean.Substring(0, 40) + "...";
			}
			log.debug("Missing End of GRIB message at pos=" + ending + " header= " + clean + " for=" + raf.Location);
			return false;
		  }
		}
		return true;
	  }
	}

}
