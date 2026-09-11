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

namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using KMPMatch = NetcdfSharp.ucar.unidata.io.KMPMatch;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Scan raf for grib-2 messages
	/// 
	/// @author caron
	/// @since 3/28/11
	/// </summary>
	public class Grib2RecordScanner
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2RecordScanner));
	  private static readonly KMPMatch matcher = new KMPMatch(new byte[] {(byte)'G', (byte)'R', (byte)'I', (byte)'B'});
	  private const bool debug = false;
	  private const bool debugRepeat = false;
	  private const bool debugEnding = false;
	  private const int maxScan = 16000;

#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks

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
		  raf.skipBytes(7); // will be positioned on byte 0 of indicator section
		  int edition = raf.read(); // read at byte 8
		  if (edition != 2)
		  {
			return false;
		  }

		  // check ending = 7777
		  long len = GribNumbers.int8(raf);
		  if (len > raf.length())
		  {
			return false;
		  }
		  raf.skipBytes(len - 20);
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

	  /// <summary>
	  /// tricky bit of business. recapture the entire record based on drs position.
	  /// for validation.
	  /// </summary>
	  /// <param name="raf"> from this RandomAccessFile </param>
	  /// <param name="drsPos"> Grib2SectionDataRepresentation starts here </param>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Nullable public static Grib2Record findRecordByDrspos(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long drsPos) throws NetcdfSharp.java.io.IOException
	  public static Grib2Record findRecordByDrspos(RandomAccessFile raf, long drsPos)
	  {
		long pos = Math.Max(0, drsPos - (20 * 1000)); // go back 20K
		Grib2RecordScanner scan = new Grib2RecordScanner(raf, pos);
		while (scan.hasNext())
		{
		  NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record gr = scan.next();
		  Grib2SectionDataRepresentation drs = gr.DataRepresentationSection;
		  if (drsPos == drs.StartingPosition)
		  {
			return gr;
		  }
		  if (raf.FilePointer > drsPos)
		  {
			break; // missed it.
		  }
		}
		return null;
	  }


	  //////////////////////////////////////////////////////////////////////////////

	  private readonly IDictionary<long, Grib2SectionGridDefinition> gdsMap = new Dictionary<long, Grib2SectionGridDefinition>();
	  private readonly RandomAccessFile raf;

	  private byte[] header;
	  private int badEndings;
	  private long lastPos; // start scanning from here

	  // deal with repeating sections - each becomes a Grib2Record
	  private long repeatPos = -1; // if > 0, we are in middle of repeating record
	  private Grib2Record repeatRecord; // current repeating record
	  private Grib2SectionBitMap repeatBms; // current repeating bms

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib2RecordScanner(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public Grib2RecordScanner(RandomAccessFile raf)
	  {
		this.raf = raf;
		raf.seek(0);
		raf.order(RandomAccessFile.BIG_ENDIAN);
		lastPos = 0;

		if (debugRepeat)
		{
		  logger.debug(" Grib2RecordScanner {}", raf.Location);
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private Grib2RecordScanner(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long startFrom) throws NetcdfSharp.java.io.IOException
	  private Grib2RecordScanner(RandomAccessFile raf, long startFrom)
	  {
		this.raf = raf;
		raf.seek(startFrom);
		raf.order(RandomAccessFile.BIG_ENDIAN);
		lastPos = startFrom;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public boolean hasNext() throws NetcdfSharp.java.io.IOException
	  public virtual bool hasNext()
	  {
		if (lastPos >= raf.length())
		{
		  return false;
		}
		if (repeatPos > 0)
		{
		  if (nextRepeating()) // this has created a new repeatRecord
		  {
			return true;
		  }
		}
		else
		{
		  repeatRecord = null;
		  repeatBms = null;
		  // fall through to new record
		}

		bool more;
		long gribStart = 0;

		while (true)
		{ // scan until we get a GRIB-2 or more == false
		  raf.seek(lastPos);
		  more = raf.searchForward(matcher, -1); // will scan to end for a 'GRIB' string
		  if (!more)
		  {
			break;
		  }

		  gribStart = raf.FilePointer; // this is where the next 'GRIB' starts
		  // see if its GRIB-2
		  raf.skipBytes(7);
		  int edition = raf.read();
		  if (edition == 2)
		  {
			break;
		  }
		  lastPos = raf.FilePointer; // not edition 2 ! just skip it !! start scanning from there
		  logger.warn("GRIB message at pos=" + gribStart + " not GRIB2; skip");
		}

		if (more)
		{
		  int sizeHeader = (int)(gribStart - lastPos); // wmo headers are embedded between records in some idd streams
		  if (debugEnding)
		  {
			logger.debug("bytes between last and next={}", sizeHeader);
		  }
		  if (sizeHeader > 100)
		  {
			sizeHeader = 100; // maximum 100 bytes; more is likely to be garbage
		  }
		  long goBack = gribStart - sizeHeader;
		  header = new byte[sizeHeader];
		  raf.seek(goBack);
		  raf.readFully(header);
		  raf.seek(gribStart);
		  this.lastPos = gribStart; // ok start from here next time
		}

		return more;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Grib2Record next() throws NetcdfSharp.java.io.IOException
	  public virtual Grib2Record next()
	  {
		if (repeatRecord != null)
		{ // serve current repeatRecord if it exists
		  return new Grib2Record(repeatRecord);
		}

		Grib2SectionIndicator @is = null;
		try
		{
		  @is = new Grib2SectionIndicator(raf);
		  Grib2SectionIdentification ids = new Grib2SectionIdentification(raf);
		  Grib2SectionLocalUse lus = new Grib2SectionLocalUse(raf);
		  Grib2SectionGridDefinition gds = new Grib2SectionGridDefinition(raf);
		  Grib2SectionProductDefinition pds = new Grib2SectionProductDefinition(raf);
		  Grib2SectionDataRepresentation drs = new Grib2SectionDataRepresentation(raf);
		  Grib2SectionBitMap bms = new Grib2SectionBitMap(raf);
		  Grib2SectionData dataSection = new Grib2SectionData(raf);
		  if (dataSection.MsgLength > @is.MessageLength)
		  { // presumably corrupt
			throw new System.InvalidOperationException("Illegal Grib2SectionData Message Length");
		  }

		  // look for duplicate gds
		  long crc = gds.calcCRC();
		  Grib2SectionGridDefinition gdsCached;
		  if (gdsMap.TryGetValue(crc, out gdsCached))
		  {
			gds = gdsCached; // hmmmm why ??
		  }
		  else
		  {
			gdsMap[crc] = gds;
		  }

		  // check to see if we have a repeating record
		  long pos = raf.FilePointer;
		  long ending = @is.EndPos;
		  if (pos + 34 < ending)
		  { // give it 30 bytes of slop
			if (debugRepeat)
			{
			  logger.debug(" REPEAT AT {} != {}", pos + 4, ending);
			}
			repeatPos = pos;
			repeatRecord = new Grib2Record(header, @is, ids, lus, gds, pds, drs, bms, dataSection, false, Grib2Index.ScanModeMissing); // this
																														   // assumes
																														   // immutable
																														   // sections
			// track bms in case its a repeat
			if (bms.BitMapIndicator == 0)
			{
			  repeatBms = bms;
			}
			return new Grib2Record(repeatRecord); // GribRecord isnt immutable; still, may not be necessary
		  }

		  if (debug)
		  {
			logger.debug(" read until {} grib ending at {} header ='{}'", raf.FilePointer, ending, StringUtil2.cleanup(header));
		  }

		  // check that end section is correct
		  bool foundEnding = true;
		  raf.seek(ending - 4);
		  for (int i = 0; i < 4; i++)
		  {
			if (raf.read() != 55)
			{
			  foundEnding = false;
			  badEndings++;
			  // String clean = StringUtil2.cleanup(header);
			  // if (clean.length() > 40) clean = clean.substring(0,40) + "...";
			  if (debug)
			  {
				logger.debug(" **missing End of GRIB message at pos={} start= {}", ending, @is.StartPos);
			  }
			  logger.warn("Missing End of GRIB message {} starting at pos={} is.ending={} data.ending={} file={}", badEndings, @is.StartPos, ending, dataSection.EndingPosition, raf.Location);
			  break;
			}
		  }
		  if (debug)
		  {
			logger.debug(" read until {} grib ending at {} header ='{}' foundEnding={}", raf.FilePointer, ending, StringUtil2.cleanup(header), foundEnding);
		  }

		  if (foundEnding || debugEnding)
		  {
			lastPos = raf.FilePointer;
			return new Grib2Record(header, @is, ids, lus, gds, pds, drs, bms, dataSection, false, Grib2Index.ScanModeMissing);

		  }
		  else
		  { // skip this record
			// lastPos = is.getEndPos() + 20; dont trust is.getEndPos()
			lastPos += 20; // skip "GRIB"
			if (hasNext()) // search forward for another one
			{
			  return next();
			}
		  }

		}
		catch (Exception t)
		{
		  long pos = (@is == null) ? -1 : @is.StartPos;
		  logger.warn("Bad GRIB2 record in file {}, skipping pos={} cause={}", raf.Location, pos, t.Message);
		  lastPos += 20; // skip "GRIB"
		  if (hasNext()) // search forward for another one
		  {
			return next();
		  }
		}

		throw new IOException("last record was incomplete");
	  }

	  // return true if got another repeat out of this record
	  // side effect is that the new record is in repeatRecord
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private boolean nextRepeating() throws NetcdfSharp.java.io.IOException
	  private bool nextRepeating()
	  {
		raf.seek(repeatPos);

		GribNumbers.int4(raf); // skip octets 1-4
		int section = raf.read(); // find out what section this is
		raf.seek(repeatPos); // back to beginning of section

		if (section == 2)
		{
		  repeatRecord.Lus = new Grib2SectionLocalUse(raf);
		  repeatRecord.Gdss = new Grib2SectionGridDefinition(raf);
		  repeatRecord.Pdss = new Grib2SectionProductDefinition(raf);
		  repeatRecord.Drs = new Grib2SectionDataRepresentation(raf);
		  repeatRecord.setBms(new Grib2SectionBitMap(raf), false);
		  repeatRecord.DataSection = new Grib2SectionData(raf);
		  repeatRecord.repeat = section;

		}
		else if (section == 3)
		{
		  repeatRecord.Gdss = new Grib2SectionGridDefinition(raf);
		  repeatRecord.Pdss = new Grib2SectionProductDefinition(raf);
		  repeatRecord.Drs = new Grib2SectionDataRepresentation(raf);
		  repeatRecord.setBms(new Grib2SectionBitMap(raf), false);
		  repeatRecord.DataSection = new Grib2SectionData(raf);
		  repeatRecord.repeat = section;

		}
		else if (section == 4)
		{
		  repeatRecord.Pdss = new Grib2SectionProductDefinition(raf);
		  repeatRecord.Drs = new Grib2SectionDataRepresentation(raf);
		  repeatRecord.setBms(new Grib2SectionBitMap(raf), false);
		  repeatRecord.DataSection = new Grib2SectionData(raf);
		  repeatRecord.repeat = section;

		}
		else
		{
		  if (debugRepeat)
		  {
			logger.debug(" REPEAT Terminate {}", section);
		  }
		  lastPos = repeatPos; // start next scan from here
		  repeatPos = -1;
		  repeatRecord = null;
		  repeatBms = null;
		  return false;
		}

		// look for repeating bms
		Grib2SectionBitMap bms = repeatRecord.BitmapSection;
		if (bms.BitMapIndicator == 254)
		{
		  // replace BMS with last good one
		  if (repeatBms == null)
		  {
			throw new System.InvalidOperationException("No bms in repeating section");
		  }
		  repeatRecord.setBms(repeatBms, true);
		  // debug
		  if (debugRepeat)
		  {
			logger.debug("replaced bms {}", section);
		  }
		  repeatRecord.repeat += 1000;

		}
		else if (bms.BitMapIndicator == 0)
		{
		  // track last good bms
		  repeatBms = repeatRecord.BitmapSection;
		}

		// keep only unique gds
		if ((section == 2) || (section == 3))
		{
		  // look for duplicate gds
		  Grib2SectionGridDefinition gds = repeatRecord.GDSsection;
		  long crc = gds.calcCRC();
		  Grib2SectionGridDefinition gdsCached;
		  if (gdsMap.TryGetValue(crc, out gdsCached))
		  {
			repeatRecord.Gdss = gdsCached;
		  }
		  else
		  {
			gdsMap[crc] = gds;
		  }
		}

		// check to see if we are at the end
		long pos = raf.FilePointer;
		long ending = repeatRecord.Is.EndPos;
		if (pos + 34 < ending)
		{ // give it 30 bytes of slop
		  if (debugRepeat)
		  {
			logger.debug(" REPEAT AGAIN {} != {}", pos + 4, ending);
		  }
		  repeatPos = pos;
		  return true;
		}

		if (debug)
		{
		  logger.debug(" REPEAT read until {} grib ending at {} header ='{}'", raf.FilePointer, ending, StringUtil2.cleanup(header));
		}

		// check that end section is correct
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
			logger.warn("  REPEAT Missing End of GRIB message at pos= {}, header={} for= {}", ending, clean, raf.Location);
			break;
		  }
		}
		lastPos = raf.FilePointer;

		if (debugRepeat)
		{
		  logger.debug(" REPEAT DONE");
		}
		repeatPos = -1; // no more repeats in this record
		return true;
	  }
	}

#pragma warning restore CS0162

}
