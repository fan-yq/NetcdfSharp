// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.netcdf3
{
	using static NetcdfSharp.ucar.nc2.NetcdfFile.IOSP_MESSAGE_GET_NETCDF_FILE_FORMAT;

	using Matcher = NetcdfSharp.com.google.re2j.Matcher;
	using Pattern = NetcdfSharp.com.google.re2j.Pattern;
	using NetcdfSharp.ucar.ma2;
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;
	using DataFormatType = NetcdfSharp.ucar.nc2.constants.DataFormatType;
	using NetcdfSharp.ucar.nc2.iosp;
	using NetcdfFileFormat = NetcdfSharp.ucar.nc2.write.NetcdfFileFormat;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// IOServiceProvider implementation abstract base class to read/write "version 3" netcdf files.
	/// AKA "file format version 1" files.
	/// 
	/// @author caron </summary>
	/// <seealso cref="N3raf concrete class"/>
	/// @deprecated do not use 
	[Obsolete("do not use")]
	public abstract class N3iosp : AbstractIOServiceProvider, IOServiceProviderWriter
	{
	  private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(N3iosp));

	  // Default fill values, used unless _FillValue variable attribute is set.
	  public const byte NC_FILL_BYTE = -127;
	  public static readonly char NC_FILL_CHAR = (char) 0;
	  public static readonly short NC_FILL_SHORT = (short) -32767;
	  public const int NC_FILL_INT = -2147483647;
	  public const float NC_FILL_FLOAT = 9.9692099683868690e+36f; // near 15 * 2^119
	  public const double NC_FILL_DOUBLE = 9.9692099683868690e+36;

	  public static readonly byte NC_FILL_UBYTE = unchecked((byte) 255);
	  public static readonly short NC_FILL_USHORT = unchecked((short) 65535);
	  public static readonly int NC_FILL_UINT = unchecked((int) 4294967295L);
	  public const long NC_FILL_INT64 = -9223372036854775806L; // 0x8000000000000002. Only bits 63 and 1 set.

	  // We want to use 18446744073709551614ULL here (see https://goo.gl/buBal9), but that's too big to fit into a
	  // signed long (Java doesn't have unsigned types). So, assign the hex string that WOULD correspond to that value
	  // *if it were treated as unsigned*. Java will treat it as signed and see "-2", but we don't much care.
	  public const long NC_FILL_UINT64 = unchecked((long)0xfffffffffffffffeL);

	  public const string NC_FILL_STRING = "";

	  public static object getFillValueDefault(DataType dtype)
	  {
		if ((dtype == DataType.BYTE) || (dtype == DataType.ENUM1))
		{
		  return N3iosp.NC_FILL_BYTE;
		}
		if (dtype == DataType.UBYTE)
		{
		  return N3iosp.NC_FILL_UBYTE;
		}
		if (dtype == DataType.CHAR)
		{
		  return (byte) 0;
		}
		if ((dtype == DataType.SHORT) || (dtype == DataType.ENUM2))
		{
		  return N3iosp.NC_FILL_SHORT;
		}
		if (dtype == DataType.USHORT)
		{
		  return N3iosp.NC_FILL_USHORT;
		}
		if ((dtype == DataType.INT) || (dtype == DataType.ENUM4))
		{
		  return N3iosp.NC_FILL_INT;
		}
		if (dtype == DataType.UINT)
		{
		  return N3iosp.NC_FILL_UINT;
		}
		if (dtype == DataType.LONG)
		{
		  return N3iosp.NC_FILL_INT64;
		}
		if (dtype == DataType.ULONG)
		{
		  return N3iosp.NC_FILL_UINT64;
		}
		if (dtype == DataType.FLOAT)
		{
		  return N3iosp.NC_FILL_FLOAT;
		}
		if (dtype == DataType.DOUBLE)
		{
		  return N3iosp.NC_FILL_DOUBLE;
		}
		return null;
	  }

	  /*
	   * CLASSIC
	   * The maximum size of a record in the classic format in versions 3.5.1 and earlier is 2^32 - 4 bytes.
	   * In versions 3.6.0 and later, there is no such restriction on total record size for the classic format
	   * or 64-bit offset format.
	   * 
	   * If you don't use the unlimited dimension, only one variable can exceed 2 GiB in size, but it can be as
	   * large as the underlying file system permits. It must be the last variable in the dataset, and the offset
	   * to the beginning of this variable must be less than about 2 GiB.
	   * 
	   * The limit is really 2^31 - 4. If you were to specify a variable size of 2^31 -3, for example, it would be
	   * rounded up to the nearest multiple of 4 bytes, which would be 2^31, which is larger than the largest
	   * signed integer, 2^31 - 1.
	   * 
	   * If you use the unlimited dimension, record variables may exceed 2 GiB in size, as long as the offset of the
	   * start of each record variable within a record is less than 2 GiB - 4.
	   */

	  /*
	   * LARGE FILE
	   * Assuming an operating system with Large File Support, the following restrictions apply to the netCDF 64-bit offset
	   * format.
	   * 
	   * No fixed-size variable can require more than 2^32 - 4 bytes of storage for its data, unless it is the last
	   * fixed-size variable and there are no record variables. When there are no record variables, the last
	   * fixed-size variable can be any size supported by the file system, e.g. terabytes.
	   * 
	   * A 64-bit offset format netCDF file can have up to 2^32 - 1 fixed sized variables, each under 4GiB in size.
	   * If there are no record variables in the file the last fixed variable can be any size.
	   * 
	   * No record variable can require more than 2^32 - 4 bytes of storage for each record's worth of data,
	   * unless it is the last record variable. A 64-bit offset format netCDF file can have up to 2^32 - 1 records,
	   * of up to 2^32 - 1 variables, as long as the size of one record's data for each record variable except the
	   * last is less than 4 GiB - 4.
	   * 
	   * Note also that all netCDF variables and records are padded to 4 byte boundaries.
	   */

	  /// <summary>
	  /// Each fixed-size variable and the data for one record's worth of a single record variable are limited
	  /// to a little less than 4 GiB.
	  /// </summary>
	  public static readonly long MAX_VARSIZE = (long) 2 * int.MaxValue - 2; // 4,294,967,292

	  /// <summary>
	  /// The maximum number of records is 2^32-1.
	  /// </summary>
	  public static readonly int MAX_NUMRECS = int.MaxValue;

	  // NetCDF File Format Type (defined in netcdf.h from the C library)
	  private static readonly string NC_FORMATX_NC3 = NetcdfFileFormat.NETCDF3.version().ToString();

	  private static bool syncExtendOnly;

	  /// <summary>
	  /// Set a static property.
	  /// Supported static properties:
	  /// <ul>
	  /// <li>syncExtendOnly = "true" : assume all file changes are syncExtend only.
	  /// </ul>
	  /// </summary>
	  /// <param name="name"> property name </param>
	  /// <param name="value"> property value </param>
	  public static void setProperty(string name, string value)
	  {
		if (name.Equals("syncExtendOnly", StringComparison.OrdinalIgnoreCase))
		{
		  syncExtendOnly = value.Equals("true", StringComparison.OrdinalIgnoreCase);
		}
	  }

	  /// <summary>
	  /// Determine if the given name can be used for a NetCDF object, i.e. a Dimension, Attribute, or Variable.
	  /// The allowed name syntax (in RE form) is:
	  /// 
	  /// <pre>
	  /// ([a-zA-Z0-9_]|{UTF8})([^\x00-\x1F\x7F/]|{UTF8})*
	  /// </pre>
	  /// 
	  /// where UTF8 represents a multi-byte UTF-8 encoding. Also, no trailing spaces are permitted in names. We do not
	  /// allow '/' because HDF5 does not permit slashes in names as slash is used as a group separator. If UTF-8 is
	  /// supported, then a multi-byte UTF-8 character can occur anywhere within an identifier.
	  /// </summary>
	  /// <param name="name"> the name to validate. </param>
	  /// <returns> {@code true} if the name is valid. </returns>
	  // Implements "int NC_check_name(const char *name)" from NetCDF-C:
	  // https://github.com/Unidata/netcdf-c/blob/v4.3.3.1/libdispatch/dstring.c#L169
	  // Should match makeValidNetcdfObjectName()
	  public static bool isValidNetcdfObjectName(string name)
	  {
		if (string.ReferenceEquals(name, null) || name.Length == 0)
		{ // Null and empty names disallowed
		  return false;
		}

		int cp = char.ConvertToUtf32(name, 0);

		// First char must be [a-z][A-Z][0-9]_ | UTF8
		if (cp <= 0x7f)
		{
		  if (!('A' <= cp && cp <= 'Z') && !('a' <= cp && cp <= 'z') && !('0' <= cp && cp <= '9') && cp != '_')
		  {
			return false;
		  }
		}

		for (int i = 1; i < name.Length; ++i)
		{
		  cp = char.ConvertToUtf32(name, i);

		  // handle simple 0x00-0x7f characters here
		  if (cp <= 0x7f)
		  {
			if (cp < ' ' || cp > 0x7E || cp == '/')
			{ // control char, DEL, or forward-slash
			  return false;
			}
		  }
		}

		// trailing spaces disallowed
		return cp > 0x7f || !char.IsWhiteSpace(cp);

	  }

	  /// <summary>
	  /// Convert a name to a legal netcdf-3 name.
	  /// </summary>
	  /// <param name="name"> the name to convert. </param>
	  /// <returns> the converted name. </returns>
	  /// <seealso cref=".isValidNetcdfObjectName(String)"/>
	  public static string makeValidNetcdfObjectName(string name)
	  {
		StringBuilder sb = new StringBuilder(name);

		while (sb.Length > 0)
		{
		  int cp = sb.codePointAt(0);

		  // First char must be [a-z][A-Z][0-9]_ | UTF8
		  if (cp <= 0x7f)
		  {
			if (!('A' <= cp && cp <= 'Z') && !('a' <= cp && cp <= 'z') && !('0' <= cp && cp <= '9') && cp != '_')
			{
			  sb.Remove(0, 1);
			  continue;
			}
		  }
		  break;
		}

		for (int pos = 1; pos < sb.Length; ++pos)
		{
		  int cp = sb.codePointAt(pos);

		  // handle simple 0x00-0x7F characters here
		  if (cp <= 0x7F)
		  {
			if (cp < ' ' || cp > 0x7E || cp == '/')
			{ // control char, DEL, or forward-slash
			  sb.Remove(pos, 1);
			  --pos;
			}
		  }
		}

		while (sb.Length > 0)
		{
		  int cp = sb.codePointAt(sb.Length - 1);

		  if (cp <= 0x7f && char.IsWhiteSpace(cp))
		  {
			sb.Remove(sb.Length - 1, 1);
		  }
		  else
		  {
			break;
		  }
		}

		if (sb.Length == 0)
		{
		  throw new ArgumentException(string.Format("Illegal NetCDF object name: '{0}'", name));
		}

		return sb.ToString();
	  }

	  /////////////////////////////////////////////////////////////////
	  // I think these go together - consider deprecated

	  /// @deprecated use makeValidNetcdfObjectName 
	  public static string makeValidNetcdf3ObjectName(string name)
	  {
		StringBuilder sb = new StringBuilder(name);

		while (sb.Length > 0)
		{
		  char c = sb[0];
		  if (char.IsLetter(c) || (c == '_'))
		  {
			break;
		  }
		  if (char.IsDigit(c))
		  {
			sb.Insert(0, 'N');
			break;
		  }
		  sb.Remove(0, 1);
		}

		int i = 1;
		while (i < sb.Length)
		{
		  char c = sb[i];
		  if (c == ' ')
		  {
			sb[i] = '_';
		  }
		  else
		  {
			bool ok = char.IsLetterOrDigit(c) || (c == '-') || (c == '_');
			// || (c == '@') || (c == ':') || (c == '(') || (c == ')') || (c == '+') || (c == '.');
			if (!ok)
			{
			  sb.Remove(i, i + 1 - i);
			  i--;
			  // sb.setCharAt(i, '-');
			}
		  }
		  i++;
		}

		return sb.ToString();
	  }

	  // static private final String special1 = "_\\.@\\+\\-";
	  // static private final String special2 = " ";
	  // static private final Pattern objectNamePattern = Pattern.compile("[a-zA-Z0-9_][a-zA-Z0-9_@\\.\\-\\+]*");
	  private static readonly Pattern objectNamePatternOld = Pattern.compile("[a-zA-Z0-9_][a-zA-Z0-9_@\\:\\(\\)\\.\\-\\+]*");

	  /// <summary>
	  /// Determine if the given name can be used for a Dimension, Attribute, or Variable name.
	  /// Should match makeValidNetcdf3ObjectName.
	  /// </summary>
	  /// <param name="name"> test this. </param>
	  /// <returns> true if valid name. </returns>
	  /// @deprecated use isValidNetcdfObjectName 
	  public static bool isValidNetcdf3ObjectName(string name)
	  {
		Matcher m = objectNamePatternOld.matcher(name);
		return m.matches();
	  }

	  private static readonly Pattern objectNamePattern = Pattern.compile("[a-zA-Z0-9_][^\\x00-\\x1F\\x2F\\x7F]*");

	  /// <summary>
	  /// Valid Netcdf Object name as a regular expression.
	  /// </summary>
	  /// <returns> regular expression pattern describing valid Netcdf Object names. </returns>
	  public static Pattern ValidNetcdf3ObjectNamePattern
	  {
		  get
		  {
			return objectNamePattern;
		  }
	  }

	  ///////////////////////////////////////////////////////////////////////////////////////////

	  /// <summary>
	  /// Convert a name to a legal netcdf name.
	  /// From the user manual:
	  /// "The names of dimensions, variables and attributes consist of arbitrary sequences of
	  /// alphanumeric characters (as well as underscore '_' and hyphen '-'), beginning with a letter
	  /// or underscore. (However names commencing with underscore are reserved for system use.)
	  /// Case is significant in netCDF names."
	  /// <p/>
	  /// Algorithm:
	  /// <ol>
	  /// <li>leading character: if alpha or underscore, ok; if digit, prepend "N"; otherwise discard
	  /// <li>other characters: if space, change to underscore; other delete.
	  /// </ol>
	  /// </summary>
	  /// <param name="name"> convert this name </param>
	  /// <returns> converted name </returns>
	  /// @deprecated use makeValidNetcdfObjectName 
	  public static string createValidNetcdf3ObjectName(string name)
	  {
		StringBuilder sb = new StringBuilder(name);

		// LOOK: could escape characters, as in DODS (%xx) ??

		while (sb.Length > 0)
		{
		  char c = sb[0];
		  if (char.IsLetter(c) || (c == '_'))
		  {
			break;
		  }
		  if (char.IsDigit(c))
		  {
			sb.Insert(0, 'N');
			break;
		  }
		  sb.Remove(0, 1);
		}

		int i = 1;
		while (i < sb.Length)
		{
		  char c = sb[i];
		  if ((c == ' ') || (c == '/'))
		  {
			sb[i] = '_';
		  }
		  else
		  {
			bool ok = char.IsLetterOrDigit(c) || (c == '-') || (c == '_') || (c == '@') || (c == ':') || (c == '(') || (c == ')') || (c == '+') || (c == '.');
			if (!ok)
			{
			  sb.Remove(i, i + 1 - i);
			  i--;
			  // sb.setCharAt(i, '-');
			}
		  }
		  i++;
		}

		return sb.ToString();
	  }


	  /////////////////////////////////////////////////////////////////////////////////////////////////

	  protected internal bool @readonly;
	  protected internal N3header header;
	  // protected int numrecs;
	  // protected long recsize;
	  protected internal long lastModified; // used by sync

	  // used for writing only
	  // protected long fileUsed = 0; // how much of the file is written to ?
	  // protected long recStart = 0; // where the record data starts

	  protected internal bool debug, debugSize, debugSPIO, debugRecord, debugRead;
	  protected internal bool showHeaderBytes;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public override bool isValidFile(RandomAccessFile raf)
	  {
		return N3header.isValidFile(raf);
	  }

	  public override string DetailInfo
	  {
		  get
		  {
			Formatter f = new Formatter();
			f.format("%s", base.DetailInfo);

			try
			{
			  header.showDetail(f);
			}
			catch (IOException e)
			{
			  return e.Message;
			}

			return f.ToString();

		  }
	  }

	  // properties
	  protected internal bool useRecordStructure;

	  //////////////////////////////////////////////////////////////////////////////////////
	  // read existing file

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void openForWriting(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public virtual void openForWriting(RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask)
	  {
		open(raf, ncfile, cancelTask);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void open(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public override void open(RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask)
	  {
		base.open(raf, ncfile, cancelTask);

		string location = raf.Location;
		if (!location.StartsWith("http:", StringComparison.Ordinal))
		{
		  File file = new File(location);
		  if (file.exists())
		  {
			lastModified = file.lastModified();
		  }
		}

		// its a netcdf-3 file
		raf.order(RandomAccessFile.BIG_ENDIAN);
		header = new N3header();

		header.read(raf, ncfile, null); // read header here
		// numrecs = header.numrecs;
		// recsize = header.recsize;
		// recStart = header.recStart;

		_open(raf);

		ncfile.finish();
	  }


	  public virtual bool Fill
	  {
		  set
		  {
			this.fill = value;
		  }
	  }

	  /////////////////////////////////////////////////////////////////////////////
	  // data reading

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public Array readData(NetcdfSharp.ucar.nc2.Variable v2, Section section) throws IOException, InvalidRangeException
	  public override NcArray readData(NetcdfSharp.ucar.nc2.Variable v2, Section section)
	  {
		if (v2 is Structure)
		{
		  return readRecordData((Structure) v2, section);
		}

		N3header.Vinfo vinfo = (N3header.Vinfo) v2.SPobject;
		DataType dataType = v2.DataType;

		Layout layout = (!v2.Unlimited) ? new LayoutRegular(vinfo.begin, v2.ElementSize, v2.Shape, section) : new LayoutRegularSegmented(vinfo.begin, v2.ElementSize, header.recsize, v2.Shape, section);

		if (layout.TotalNelems == 0)
		{
		  return NcArray.factory(dataType, section.Shape);
		}

		object data = readData(layout, dataType);
		return NcArray.factory(dataType, section.Shape, data);
	  }

	  /// <summary>
	  /// Read data from record structure. For N3, this is the only possible structure, and there can be no nesting.
	  /// Read all variables for each record, put in ByteBuffer.
	  /// </summary>
	  /// <param name="s"> the record structure </param>
	  /// <param name="section"> the record range to read </param>
	  /// <returns> an ArrayStructure, with all the data read in. </returns>
	  /// <exception cref="IOException"> on error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.ucar.ma2.Array readRecordData(NetcdfSharp.ucar.nc2.Structure s, Section section) throws NetcdfSharp.java.io.IOException
	  private NetcdfSharp.ucar.ma2.NcArray readRecordData(NetcdfSharp.ucar.nc2.Structure s, Section section)
	  {
		// if (s.isSubset())
		// return readRecordDataSubset(s, section);

		// has to be 1D
		Range recordRange = section.getRange(0);

		// create the ArrayStructure
		StructureMembers members = s.makeStructureMembers();
		foreach (StructureMembers.Member m in members.Members)
		{
		  Variable v2 = s.findVariable(m.Name);
		  N3header.Vinfo vinfo = (N3header.Vinfo) v2.SPobject;
		  m.DataParam = (int)(vinfo.begin - header.recStart);
		}

		// protect agains too large of reads
		if (header.recsize > int.MaxValue)
		{
		  throw new ArgumentException("Cant read records when recsize > " + int.MaxValue);
		}
		long nrecs = section.computeSize();
		if (nrecs * header.recsize > int.MaxValue)
		{
		  throw new ArgumentException("Too large read: nrecs * recsize= " + (nrecs * header.recsize) + "bytes exceeds " + int.MaxValue);
		}

		members.StructureSize = (int) header.recsize;
		ArrayStructureBB structureArray = new ArrayStructureBB(members, new int[] {recordRange.length()});

		// note dependency on raf; should probably defer to subclass
		// loop over records
		byte[] result = structureArray.ByteBuffer.array();
		int count = 0;
		foreach (int recnum in recordRange)
		{
		  if (debugRecord)
		  {
			Console.WriteLine(" read record " + recnum);
		  }
		  raf.seek(header.recStart + recnum * header.recsize); // where the record starts

		  if (recnum != header.numrecs - 1)
		  {
			raf.readFully(result, (int)(count * header.recsize), (int) header.recsize);
		  }
		  else
		  {
			raf.read(result, (int)(count * header.recsize), (int) header.recsize); // "wart" allows file to be one byte
		  }
																					// short. since its always padding, we
																					// allow
		  count++;
		}

		return structureArray;
	  }

	  /// <summary>
	  /// Read data from record structure, that has been subsetted.
	  /// Read one record at at time, put requested variable into ArrayStructureMA.
	  /// </summary>
	  /// <param name="s"> the record structure </param>
	  /// <param name="section"> the record range to read </param>
	  /// <returns> an ArrayStructure, with all the data read in. </returns>
	  private NetcdfSharp.ucar.ma2.NcArray readRecordDataSubset(NetcdfSharp.ucar.nc2.Structure s, Section section)
	  {
		Range recordRange = section.getRange(0);
		int nrecords = recordRange.length();

		// create the ArrayStructureMA
		StructureMembers members = s.makeStructureMembers();
		foreach (StructureMembers.Member m in members.Members)
		{
		  Variable v2 = s.findVariable(m.Name);
		  N3header.Vinfo vinfo = (N3header.Vinfo) v2.SPobject;
		  m.DataParam = (int)(vinfo.begin - header.recStart); // offset from start of record

		  // construct the full shape
		  int rank = m.Shape.Length;
		  int[] fullShape = new int[rank + 1];
		  fullShape[0] = nrecords; // the first dimension
		  System.Array.Copy(m.Shape, 0, fullShape, 1, rank); // the remaining dimensions

		  NcArray data = NcArray.factory(m.DataType, fullShape);
		  m.DataArray = data;
		  m.DataObject = data.IndexIterator;
		}

		// LOOK this is all wrong - why using recsize ???
		return null;
		/*
		 * members.setStructureSize(recsize);
		 * ArrayStructureMA structureArray = new ArrayStructureMA(members, new int[]{nrecords});
		 * 
		 * // note dependency on raf; should probably defer to subclass
		 * // loop over records
		 * byte[] record = new byte[ recsize];
		 * ByteBuffer bb = ByteBuffer.wrap(record);
		 * for (int recnum = recordRange.first(); recnum <= recordRange.last(); recnum += recordRange.stride()) {
		 * if (debugRecord) System.out.println(" readRecordDataSubset recno= " + recnum);
		 * 
		 * // read one record
		 * raf.seek(recStart + recnum * recsize); // where the record starts
		 * if (recnum != numrecs - 1)
		 * raf.readFully(record, 0, recsize);
		 * else
		 * raf.read(record, 0, recsize); // "wart" allows file to be one byte short. since its always padding, we allow
		 * 
		 * // transfer desired variable(s) to result array(s)
		 * for (StructureMembers.Member m : members.getMembers()) {
		 * IndexIterator dataIter = (IndexIterator) m.getDataObject();
		 * IospHelper.copyFromByteBuffer(bb, m, dataIter);
		 * }
		 * }
		 * 
		 * return structureArray;
		 */
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.ucar.ma2.Array readNestedData(NetcdfSharp.ucar.nc2.Variable v2, Section section) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  private NetcdfSharp.ucar.ma2.NcArray readNestedData(NetcdfSharp.ucar.nc2.Variable v2, Section section)
	  {
		N3header.Vinfo vinfo = (N3header.Vinfo) v2.SPobject;
		DataType dataType = v2.DataType;

		// construct the full shape for use by RegularIndexer
		int[] fullShape = new int[v2.Rank + 1];
		fullShape[0] = header.numrecs; // the first dimension
		System.Array.Copy(v2.Shape, 0, fullShape, 1, v2.Rank); // the remaining dimensions

		Layout layout = new LayoutRegularSegmented(vinfo.begin, v2.ElementSize, header.recsize, fullShape, section);
		object dataObject = readData(layout, dataType);
		object dataObject = readData(layout, dataType);
		return NcArray.factory(dataType, section.Shape, dataObject);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public long readToByteChannel(NetcdfSharp.ucar.nc2.Variable v2, Section section, WritableByteChannel channel) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  public override long readToByteChannel(NetcdfSharp.ucar.nc2.Variable v2, Section section, WritableByteChannel channel)
	  {

		if (v2 is Structure)
		{
		  return readRecordData((Structure) v2, section, channel);
		}

		N3header.Vinfo vinfo = (N3header.Vinfo) v2.SPobject;
		DataType dataType = v2.DataType;

		Layout layout = (!v2.Unlimited) ? new LayoutRegular(vinfo.begin, v2.ElementSize, v2.Shape, section) : new LayoutRegularSegmented(vinfo.begin, v2.ElementSize, header.recsize, v2.Shape, section);

		return readData(layout, dataType, channel);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private long readRecordData(NetcdfSharp.ucar.nc2.Structure s, Section section, WritableByteChannel out) throws NetcdfSharp.java.io.IOException
	  private long readRecordData(NetcdfSharp.ucar.nc2.Structure s, Section section, WritableByteChannel @out)
	  {
		long count = 0;

		// LOOK this is the OTW layout based on netcdf-3
		// not sure this works but should give an idea of timing
		Range recordRange = section.getRange(0);

		foreach (int recnum in recordRange)
		{
		  if (debugRecord)
		  {
			Console.WriteLine(" read record " + recnum);
		  }
		  raf.seek(header.recStart + recnum * header.recsize); // where the record starts
		  count += raf.readToByteChannel(@out, header.recStart + recnum * header.recsize, header.recsize);
		}

		return count;
	  }

	  //////////////////////////////////////////////////////////////////////////////////////
	  // create new file

	  protected internal bool fill = true;
	  // protected HashMap dimHash = new HashMap(50);

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void create(String filename, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, int extra, long preallocateSize, boolean largeFile) throws IOException
	  public virtual void create(string filename, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, int extra, long preallocateSize, bool largeFile)
	  {
		this.ncfile = ncfile;
		this.@readonly = false;

		// finish any structures
		ncfile.finish();

		raf = new RandomAccessFile(filename, "rw");
		raf.order(RandomAccessFile.BIG_ENDIAN);

		if (preallocateSize > 0)
		{
		  NetcdfSharp.java.io.RandomAccessFile myRaf = raf.RandomAccessFile;
		  myRaf.setLength(preallocateSize);
		}

		header = new N3header();
		header.create(raf, ncfile, extra, largeFile, null);

		// recsize = header.recsize; // record size
		// recStart = header.recStart; // record variables start here
		// fileUsed = headerParser.getMinLength(); // track what is actually used

		_create(raf);

		if (fill)
		{
		  fillNonRecordVariables();
		}
		// else
		// raf.setMinLength(recStart); // make sure file length is long enough, even if not written to.
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public boolean rewriteHeader(boolean largeFile) throws IOException
	  public virtual bool rewriteHeader(bool largeFile)
	  {
		return header.rewriteHeader(largeFile, null);
	  }

	  //////////////////////////////////////////////////////////////////////////////////////
	  // write

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void writeData(Variable v2, Section section, Array values) throws NetcdfSharp.java.io.IOException, InvalidRangeException
	  public virtual void writeData(NetcdfSharp.ucar.nc2.Variable v2, Section section, NcArray values)
	  {
		N3header.Vinfo vinfo = (N3header.Vinfo) v2.SPobject;
		DataType dataType = v2.DataType;

		if (v2.Unlimited)
		{
		  Range firstRange = section.getRange(0);
		  setNumrecs(firstRange.last() + 1);
		}

		if (v2 is Structure)
		{
		  if (!(values is ArrayStructure))
		  {
			throw new ArgumentException("writeData for Structure: data must be ArrayStructure");
		  }

		  if (v2.Rank == 0)
		  {
			throw new ArgumentException("writeData for Structure: must have rank > 0");
		  }

		  NetcdfSharp.ucar.nc2.Dimension d = v2.getDimension(0);
		  if (!d.Unlimited)
		  {
			throw new ArgumentException("writeData for Structure: must have unlimited dimension");
		  }

		  writeRecordData((Structure) v2, section, (ArrayStructure) values);

		}
		else
		{
		  Layout layout = (!v2.Unlimited) ? new LayoutRegular(vinfo.begin, v2.ElementSize, v2.Shape, section) : new LayoutRegularSegmented(vinfo.begin, v2.ElementSize, header.recsize, v2.Shape, section);
		  writeData(values, layout, dataType);
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public int appendStructureData(Structure s, StructureData sdata) throws IOException, InvalidRangeException
	  public virtual int appendStructureData(NetcdfSharp.ucar.nc2.Structure s, StructureData sdata)
	  {
		int recnum = header.numrecs;
		setNumrecs(recnum + 1);
		writeRecordData(s, recnum, sdata);
		return recnum;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void writeRecordData(NetcdfSharp.ucar.nc2.Structure s, Section section, ArrayStructure structureArray) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  private void writeRecordData(NetcdfSharp.ucar.nc2.Structure s, Section section, ArrayStructure structureArray)
	  {
		int countSrcRecnum = 0;
		Range recordRange = section.getRange(0);
		foreach (int recnum in recordRange)
		{
		  StructureData sdata = structureArray.getStructureData(countSrcRecnum);
		  writeRecordData(s, recnum, sdata);
		  countSrcRecnum++;
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void writeRecordData(NetcdfSharp.ucar.nc2.Structure s, int recnum, StructureData sdata) throws NetcdfSharp.java.io.IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  private void writeRecordData(NetcdfSharp.ucar.nc2.Structure s, int recnum, StructureData sdata)
	  {

		StructureMembers members = sdata.StructureMembers;

		// loop over members
		foreach (NetcdfSharp.ucar.nc2.Variable vm in s.Variables)
		{
		  StructureMembers.Member m = members.findMember(vm.ShortName);
		  if (null == m)
		  {
			continue; // this means that the data is missing from the ArrayStructure
		  }

		  // convert String member data into CHAR data
		  NcArray data = sdata.getArray(m);
		  if (data is ArrayObject && vm.DataType == DataType.CHAR && vm.Rank > 0)
		  {
			int strlen = vm.getShape(vm.Rank - 1);
			data = ArrayChar.makeFromStringArray((ArrayObject) data, strlen); // turn it into an ArrayChar
		  }

		  // layout of the destination
		  N3header.Vinfo vinfo = (N3header.Vinfo) vm.SPobject;
		  long begin = vinfo.begin + recnum * header.recsize; // this assumes unlimited dimension
		  Section memberSection = vm.ShapeAsSection;
		  Layout layout = new LayoutRegular(begin, vm.ElementSize, vm.Shape, memberSection);

		  try
		  {
			writeData(data, layout, vm.DataType);
		  }
		  catch (Exception e)
		  {
			log.error("Error writing member=" + vm.ShortName + " in struct=" + s.FullName, e);
			throw new IOException(e.Message, e);
		  }
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected void setNumrecs(int n) throws IOException, InvalidRangeException
	  protected internal virtual void setNumrecs(int n)
	  {
		if (n <= header.numrecs)
		{
		  return;
		}
		int startRec = header.numrecs;

		if (debugSize)
		{
		  Console.WriteLine("extend records to = " + n);
		}
		// fileUsed = recStart + recsize * n;
		header.setNumrecs(n);
		// this.numrecs = n;

		// need to let unlimited dimension know of new shape
		foreach (NetcdfSharp.ucar.nc2.Dimension dim in ncfile.RootGroup.Dimensions)
		{
		  if (dim.Unlimited)
		  {
			dim.Length = n;
		  }
		}

		// need to let all unlimited variables know of new shape
		foreach (NetcdfSharp.ucar.nc2.Variable v in ncfile.Variables)
		{
		  if (v.Unlimited)
		  {
			v.resetShape();
			v.setCachedData(null, false);
		  }
		}

		// extend file, handle filling
		if (fill)
		{
		  fillRecordVariables(startRec, n);
		}
		else
		{
		  raf.MinLength = header.calcFileSize();
		}
	  }

	  /// <summary>
	  /// Update the value of an existing attribute. Attribute is found by name, which must match exactly.
	  /// You cannot make an attribute longer, or change the number of values.
	  /// For strings: truncate if longer, zero fill if shorter. Strings are padded to 4 byte boundaries, ok to use padding
	  /// if it exists.
	  /// For numerics: must have same number of values.
	  /// </summary>
	  /// <param name="v2"> variable, or null for fglobal attribute </param>
	  /// <param name="att"> replace with this value </param>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void updateAttribute(NetcdfSharp.ucar.nc2.Variable v2, Attribute att) throws IOException
	  public virtual void updateAttribute(NetcdfSharp.ucar.nc2.Variable v2, NetcdfSharp.ucar.nc2.Attribute att)
	  {
		header.updateAttribute(v2, att);
	  }

	  /////////////////////////////////////////////////////////////

	  // fill buffer with fill value

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected void fillNonRecordVariables() throws IOException
	  protected internal virtual void fillNonRecordVariables()
	  {
		// run through each variable
		foreach (NetcdfSharp.ucar.nc2.Variable v in ncfile.Variables)
		{
		  if (v.Unlimited)
		  {
			continue;
		  }
		  try
		  {
			writeData(v, v.ShapeAsSection, makeConstantArray(v));
		  }
		  catch (InvalidRangeException e)
		  {
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace); // shouldnt happen
		  }
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: protected void fillRecordVariables(int recStart, int recEnd) throws IOException, InvalidRangeException
	  protected internal virtual void fillRecordVariables(int recStart, int recEnd)
	  {
		// do each record completely, should be a bit more efficient

		for (int i = recStart; i < recEnd; i++)
		{ // do one record at a time
		  Range r = new Range(i, i);

		  // run through each variable
		  foreach (NetcdfSharp.ucar.nc2.Variable v in ncfile.Variables)
		  {
			if (!v.Unlimited || (v is Structure))
			{
			  continue;
			}
			Section.Builder recordSection = Section.builder().appendRanges(v.Ranges);
			recordSection.setRange(0, r);
			writeData(v, recordSection.build(), makeConstantArray(v));
		  }
		}
	  }

	  private NcArray makeConstantArray(NetcdfSharp.ucar.nc2.Variable v)
	  {
		Type classType = v.DataType.getPrimitiveClassType();
		// int [] shape = v.getShape();
		NetcdfSharp.ucar.nc2.Attribute att = v.findAttribute(CDM.FILL_VALUE);

		object storage = null;
		if (classType == typeof(double))
		{
		  double[] storageP = new double[1];
		  storageP[0] = (att == null) ? NC_FILL_DOUBLE : Convert.ToDouble(att.NumericValue);
		  storage = storageP;

		}
		else if (classType == typeof(float))
		{
		  float[] storageP = new float[1];
		  storageP[0] = (att == null) ? NC_FILL_FLOAT : Convert.ToSingle(att.NumericValue);
		  storage = storageP;

		}
		else if (classType == typeof(int))
		{
		  int[] storageP = new int[1];
		  storageP[0] = (att == null) ? NC_FILL_INT : Convert.ToInt32(att.NumericValue);
		  storage = storageP;

		}
		else if (classType == typeof(short))
		{
		  short[] storageP = new short[1];
		  storageP[0] = (att == null) ? NC_FILL_SHORT : Convert.ToInt16(att.NumericValue);
		  storage = storageP;

		}
		else if (classType == typeof(byte))
		{
		  byte[] storageP = new byte[1];
		  storageP[0] = (att == null) ? NC_FILL_BYTE : Convert.ToByte(att.NumericValue);
		  storage = storageP;

		}
		else if (classType == typeof(char))
		{
		  char[] storageP = new char[1];
		  storageP[0] = (att != null) && (att.StringValue.Length > 0) ? att.StringValue[0] : NC_FILL_CHAR;
		  storage = storageP;
		}

		return NcArray.factoryConstant(v.DataType, v.Shape, storage);
	  }

	  //////////////////////////////////////////////////////////////////////////////////////////////
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public boolean syncExtend() throws IOException
	  public override bool syncExtend()
	  {
		bool result = header.synchNumrecs();
		if (result && log.isDebugEnabled())
		{
		  log.debug(" N3iosp syncExtend " + raf.Location + " numrecs =" + header.numrecs);
		}
		return result;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void flush() throws NetcdfSharp.java.io.IOException
	  public virtual void flush()
	  {
		if (raf != null)
		{
		  raf.flush();
		  header.writeNumrecs();
		  raf.flush();
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void close() throws NetcdfSharp.java.io.IOException
	  public override void close()
	  {
		if (raf != null)
		{
		  long size = header.calcFileSize();
		  raf.MinLength = size;
		  raf.close();
		}
		raf = null;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void reacquire() throws IOException
	  public override void reacquire()
	  {
		base.reacquire();
		header.raf = this.raf;
	  }

	  /// <summary>
	  /// Debug info for this object.
	  /// </summary>
	  public override string toStringDebug(object o)
	  {
		return null;
	  }

	  public override object sendIospMessage(object message)
	  {
		if (null == header)
		{
		  return null;
		}

		if (message.Equals(IOSP_MESSAGE_GET_NETCDF_FILE_FORMAT))
		{
		  return header.useLongOffset ? NetcdfFileFormat.NETCDF3_64BIT_OFFSET : NetcdfFileFormat.NETCDF3;
		}

		return base.sendIospMessage(message);
	  }

	  public override string FileTypeId
	  {
		  get
		  {
			return DataFormatType.NETCDF.Description;
		  }
	  }

	  public override string FileTypeDescription
	  {
		  get
		  {
			return "NetCDF-3/CDM";
		  }
	  }

	  public override string FileTypeVersion
	  {
		  get
		  {
			return NC_FORMATX_NC3;
		  }
	  }

	  ////////////////////////////////////////////////////////////////////////////////////////////////////
	  // stuff we need the subclass to implement

	  /// <summary>
	  /// Read data subset from file for a variable, create primitive array.
	  /// </summary>
	  /// <param name="index"> handles skipping around in the file. </param>
	  /// <param name="dataType"> dataType of the variable </param>
	  /// <returns> primitive array with data read in </returns>
	  protected abstract object readData(Layout index, DataType dataType);

	  protected abstract long readData(Layout index, DataType dataType, WritableByteChannel @out);


	  /// <summary>
	  /// Write data subset to file for a variable, create primitive array.
	  /// </summary>
	  /// <param name="aa"> write data from this Array. </param>
	  /// <param name="index"> handles skipping around in the file. </param>
	  /// <param name="dataType"> dataType of the variable </param>
	  protected abstract void writeData(NcArray aa, Layout index, DataType dataType);

	  protected abstract void _open(RandomAccessFile raf);

	  protected abstract void _create(RandomAccessFile raf);
	}

}
