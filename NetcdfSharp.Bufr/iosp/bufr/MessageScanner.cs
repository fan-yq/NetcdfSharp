// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using KMPMatch = NetcdfSharp.ucar.unidata.io.KMPMatch;

	/// <summary>
	/// Sequentially scans a BUFR file, extracts the messages.
	/// 
	/// @author caron
	/// @since May 9, 2008
	/// </summary>
	public class MessageScanner
	{
	  // static public final int MAX_MESSAGE_SIZE = 500 * 1000; // GTS allows up to 500 Kb messages (ref?)
	  private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(MessageScanner));

	  private static readonly KMPMatch matcher = new KMPMatch(Encoding.UTF8.GetBytes("BUFR"));

	  /// <summary>
	  /// is this a valid BUFR file.
	  /// </summary>
	  /// <param name="raf"> check this file </param>
	  /// <returns> true if its a BUFR file </returns>
	  /// <exception cref="IOException"> on read error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws IOException
	  public static bool isValidFile(RandomAccessFile raf)
	  {
		raf.seek(0);
		if (!raf.searchForward(matcher, 40 * 1000))
		{
		  return false; // must find "BUFR" in first 40k
		}
		raf.skipBytes(4);
		BufrIndicatorSection @is = new BufrIndicatorSection(raf);
		if (@is.BufrEdition > 4)
		{
		  return false;
		}
		// if(is.getBufrLength() > MAX_MESSAGE_SIZE) return false;
		return !(@is.BufrLength > raf.length());
	  }

	  /////////////////////////////////

	  private RandomAccessFile raf;
	  private bool useEmbeddedTables;

	  private int countMsgs;
	  private int countObs;
	  private byte[] header;
	  private long startPos;
	  private long lastPos;
	  private bool debug;

	  private EmbeddedTable embedTable;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public MessageScanner(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws IOException
	  public MessageScanner(RandomAccessFile raf) : this(raf, 0, true)
	  {
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public MessageScanner(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, long startPos, boolean useEmbeddedTables) throws IOException
	  public MessageScanner(RandomAccessFile raf, long startPos, bool useEmbeddedTables)
	  {
		startPos = (startPos < 30) ? 0 : startPos - 30; // look for the header
		this.raf = raf;
		lastPos = startPos;
		this.useEmbeddedTables = useEmbeddedTables;
		raf.seek(startPos);
		raf.order(RandomAccessFile.BIG_ENDIAN);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public Message getFirstDataMessage() throws IOException
	  public virtual Message FirstDataMessage
	  {
		  get
		  {
			while (hasNext())
			{
			  Message m = next();
			  if (m == null)
			  {
				continue;
			  }
			  if (m.containsBufrTable())
			  {
				continue; // not data
			  }
			  if (m.NumberDatasets == 0)
			  {
				continue; // empty
			  }
			  return m;
			}
			return null;
		  }
	  }

	  public virtual void reset()
	  {
		lastPos = 0;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public boolean hasNext() throws IOException
	  public virtual bool hasNext()
	  {
		if (lastPos >= raf.length())
		{
		  return false;
		}
		raf.seek(lastPos);
		bool more = raf.searchForward(matcher, -1); // will scan to end for another BUFR header
		if (more)
		{
		  long stop = raf.FilePointer;
		  int sizeHeader = (int)(stop - lastPos);
		  if (sizeHeader > 30)
		  {
			sizeHeader = 30;
		  }
		  header = new byte[sizeHeader];
		  startPos = stop - sizeHeader;
		  raf.seek(startPos);
		  int nRead = raf.read(header);
		  if (nRead != header.Length)
		  {
			log.warn("Unable to read full BUFR header. Got " + nRead + " but expected " + header.Length);
			return false;
		  }
		}
		if (debug && countMsgs % 100 == 0)
		{
		  Console.Write("{0:D} ", countMsgs);
		}
		return more;
	  }

	  public virtual Message next()
	  {

		try
		{
		  long start = raf.FilePointer;
		  raf.seek(start + 4);

		  BufrIndicatorSection @is = new BufrIndicatorSection(raf);
		  BufrIdentificationSection ids = new BufrIdentificationSection(raf, @is);
		  BufrDataDescriptionSection dds = new BufrDataDescriptionSection(raf);

		  long dataPos = raf.FilePointer;
		  int dataLength = BufrNumbers.uint3(raf);
		  BufrDataSection dataSection = new BufrDataSection(dataPos, dataLength);
		  lastPos = dataPos + dataLength + 4; // position to the end message plus 1
		  // nbytes += lastPos - startPos;

		  /*
		   * length consistency checks
		   * if (is.getBufrLength() > MAX_MESSAGE_SIZE) {
		   * log.warn("Illegal length - BUFR message at pos "+start+" header= "+cleanup(header)+" size= "+is.getBufrLength()
		   * );
		   * return null;
		   * }
		   */

		  if (@is.BufrEdition > 4)
		  {
			log.warn("Illegal edition - BUFR message at pos " + start + " header= " + cleanup(header));
			return null;
		  }

		  if (@is.BufrEdition < 2)
		  {
			log.warn("Edition " + @is.BufrEdition + " is not supported - BUFR message at pos " + start + " header= " + cleanup(header));
			return null;
		  }

		  // check that end section is correct
		  long ending = dataPos + dataLength;
		  raf.seek(dataPos + dataLength);
		  for (int i = 0; i < 3; i++)
		  {
			if (raf.read() != 55)
			{
			  log.warn("Missing End of BUFR message at pos= {} header= {} file= {}", ending, cleanup(header), raf.Location);
			  return null;
			}
		  }
		  // allow off by one : may happen when dataLength rounded to even bytes
		  if (raf.read() != 55)
		  {
			raf.seek(dataPos + dataLength - 1); // see if byte before is a '7'
			if (raf.read() != 55)
			{
			  log.warn("Missing End of BUFR message at pos= {} header= {} edition={} file= {}", ending, cleanup(header), @is.BufrEdition, raf.Location);
			  return null;
			}
			else
			{
			  log.info("End of BUFR message off-by-one at pos= {} header= {} edition={} file= {}", ending, cleanup(header), @is.BufrEdition, raf.Location);
			  lastPos--;
			}
		  }

		  Message m = new Message(raf, @is, ids, dds, dataSection);
		  m.Header = cleanup(header);
		  m.StartPos = start;

		  if (useEmbeddedTables && m.containsBufrTable())
		  {
			if (embedTable == null)
			{
			  embedTable = new EmbeddedTable(m, raf);
			}
			embedTable.addTable(m);
		  }
		  else if (embedTable != null)
		  {
			m.TableLookup = embedTable.TableLookup;
		  }

		  countMsgs++;
		  countObs += dds.NumberDatasets;
		  raf.seek(start + @is.BufrLength);
		  return m;

		}
		catch (IOException ioe)
		{
		  log.error("Error reading message at " + lastPos, ioe);
		  lastPos = raf.FilePointer; // dont do an infinite loop
		  return null;
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public TableLookup getTableLookup() throws IOException
	  public virtual TableLookup TableLookup
	  {
		  get
		  {
			while (hasNext())
			{
			  next();
			}
			return (embedTable != null) ? embedTable.TableLookup : null;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public byte[] getMessageBytesFromLast(Message m) throws IOException
	  public virtual byte[] getMessageBytesFromLast(Message m)
	  {
		long startPos = m.StartPos;
		int length = (int)(lastPos - startPos);
		byte[] result = new byte[length];

		raf.seek(startPos);
		raf.readFully(result);
		return result;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public byte[] getMessageBytes(Message m) throws IOException
	  public virtual byte[] getMessageBytes(Message m)
	  {
		long startPos = m.StartPos;
		int length = m.@is.BufrLength;
		byte[] result = new byte[length];

		raf.seek(startPos);
		raf.readFully(result);
		return result;
	  }

	  public virtual int TotalObs
	  {
		  get
		  {
			return countObs;
		  }
	  }

	  public virtual int TotalMessages
	  {
		  get
		  {
			return countMsgs;
		  }
	  }

	  // the WMO header is in here somewhere when the message comes over the IDD
	  private static string cleanup(byte[] h)
	  {
		byte[] bb = new byte[h.Length];
		int count = 0;
		foreach (byte b in h)
		{
		  if (b >= 32 && b < 127)
		  {
			bb[count++] = b;
		  }
		}
		return StringHelper.NewString(bb, 0, count, Encoding.UTF8);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public long writeCurrentMessage(NetcdfSharp.java.nio.channels.WritableByteChannel out) throws IOException
	  public virtual long writeCurrentMessage(WritableByteChannel @out)
	  {
		long nbytes = lastPos - startPos;
		return raf.readToByteChannel(@out, startPos, nbytes);
	  }

	}

}
