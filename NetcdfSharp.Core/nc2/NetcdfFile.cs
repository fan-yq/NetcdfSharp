// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

/*
 * Copyright (c) 1998-2020 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2
{
	using AbstractIOServiceProvider = NetcdfSharp.ucar.nc2.iosp.AbstractIOServiceProvider;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using CBZip2InputStream = NetcdfSharp.ucar.unidata.io.bzip2.CBZip2InputStream;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using DebugFlags = NetcdfSharp.ucar.nc2.util.DebugFlags;
	using Element = NetcdfSharp.org.jdom2.Element;
	using EscapeStrings = NetcdfSharp.ucar.nc2.util.EscapeStrings;
	using H5headerNew = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5headerNew;
	using H5iospNew = NetcdfSharp.ucar.nc2.@internal.iosp.hdf5.H5iospNew;
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;
	using InMemoryRandomAccessFile = NetcdfSharp.ucar.unidata.io.InMemoryRandomAccessFile;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using IO = NetcdfSharp.ucar.nc2.util.IO;
	using IOServiceProvider = NetcdfSharp.ucar.nc2.iosp.IOServiceProvider;
	using IospHelper = NetcdfSharp.ucar.nc2.iosp.IospHelper;
	using N3headerNew = NetcdfSharp.ucar.nc2.@internal.iosp.netcdf3.N3headerNew;
	using N3iospNew = NetcdfSharp.ucar.nc2.@internal.iosp.netcdf3.N3iospNew;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;
	using StructureDataIterator = NetcdfSharp.ucar.ma2.StructureDataIterator;
	using UncompressInputStream = NetcdfSharp.ucar.unidata.io.UncompressInputStream;

	/// <summary>
	/// <para>
	/// Read-only scientific datasets that are accessible through the netCDF API.
	/// Immutable after {@code setImmutable()} is called. Reading data is not
	/// thread-safe because of the use of {@code RandomAccessFile}.
	/// </para>
	/// <para>
	/// Using this class's {@code Builder} scheme to create a {@code NetcdfFile} object could, for
	/// example, be accomplished as follows, using a try/finally block to ensure that the
	/// {@code NetcdfFile} is closed when done.
	/// 
	/// <pre>
	/// NetcdfFile ncfile = null;
	/// try {
	///   ncfile = NetcdfFile.builder().setLocation(fileName).build();
	///   // do stuff
	/// } finally {
	///   if (ncfile != null) {
	///     ncfile.close();
	///   }
	/// }
	/// </pre>
	/// 
	/// More conveniently, a {@code NetcdfFile} object may be created using one of the static methods
	/// in {@code NetcdfFiles}:
	/// 
	/// <pre>
	/// NetcdfFile ncfile = null;
	/// try {
	///   ncfile = NetcdfFiles.open(fileName);
	///   // do stuff
	/// } finally {
	///   if (ncfile != null) {
	///     ncfile.close();
	///   }
	/// }
	/// </pre>
	/// 
	/// Or better yet, use try-with-resources:
	/// 
	/// <pre>
	/// try (NetcdfFile ncfile = NetcdfFiles.open(fileName)) {
	///   // do stuff
	/// }
	/// </pre>
	/// 
	/// <h3>Naming</h3>
	/// Each object has a name (aka "full name") that is unique within the entire netcdf file, and
	/// a "short name" that is unique within the parent group.
	/// These coincide for objects in the root group, and so are backwards compatible with version
	/// 3 files.
	/// <ol>
	/// <li>Variable: group1/group2/varname
	/// <li>Structure member Variable: group1/group2/varname.s1.s2
	/// <li>Group Attribute: group1/group2@attName
	/// <li>Variable Attribute: group1/group2/varName@attName
	/// </ol>
	/// </para>
	/// </summary>
	public class NetcdfFile : IDisposable
	{
		private void InitializeInstanceFields()
		{
			rootGroup = makeRootGroup();
		}

		private Group makeRootGroup()
		{
			return Group.builder().setNcfile(this).setName("").build();
		}

		public virtual Group RootGroup
		{
			get
			{
				return rootGroup;
			}
		}

		public virtual void setRootGroup(Group rootGroup)
		{
			this.rootGroup = rootGroup;
		}

		// forward "add" operations to the root group (used by the grib/collection builders)
		public virtual NcAttribute addAttribute(NcAttribute att)
		{
			return rootGroup.addAttribute(att);
		}

		/// <summary>
		/// Add attribute to a group. If group is null, use root group.
		/// </summary>
		[Obsolete("Use NetcdfFile.builder()")]
		public virtual NcAttribute addAttribute(Group parent, NcAttribute att)
		{
			Group g = (parent == null) ? rootGroup : parent;
			g.addAttribute(att);
			return att;
		}

		/// <summary>
		/// Add optional String attribute to a group. If group is null, use root group.
		/// If value is null, do not add.
		/// </summary>
		[Obsolete("Use NetcdfFile.builder()")]
		public virtual NcAttribute addAttribute(Group parent, string name, string value)
		{
			if (string.ReferenceEquals(value, null))
			{
				return null;
			}
			Group g = (parent == null) ? rootGroup : parent;
			NcAttribute att = new NcAttribute(name, value);
			g.addAttribute(att);
			return att;
		}

		public virtual void addDimension(Dimension dim)
		{
			rootGroup.addDimension(dim);
		}

		public virtual void addGroup(Group g)
		{
			rootGroup.addGroup(g);
		}

		public virtual void addVariable(Variable v)
		{
			rootGroup.addVariable(v);
		}

		/// <summary>
		/// Add variable to a group. If group is null, use root group.
		/// </summary>
		[Obsolete("Use NetcdfFile.builder()")]
		public virtual Variable addVariable(Group g, Variable v)
		{
			Group grp = (g == null) ? rootGroup : g;
			grp.addVariable(v);
			return v;
		}

		public virtual string Location
		{
			get
			{
				return location;
			}
		}

		/// <summary>
		/// Get the unique id, if it exists.
		/// </summary>
		/// <returns> unique id, may be null </returns>
		public virtual string Id
		{
			get
			{
				return id;
			}
		}

		/// <summary>
		/// Get the human-readable title, if it exists.
		/// </summary>
		/// <returns> title, may be null </returns>
		public virtual string Title
		{
			get
			{
				return title;
			}
		}

		/// <summary>
		/// Look up an Attribute by (short) name in the root Group, exact match.
		/// </summary>
		/// <param name="attName"> the name of the attribute </param>
		/// <returns> the root group attribute with given name, or null if not found </returns>
		public virtual NcAttribute? findGlobalAttribute(string attName)
		{
			return rootGroup.findAttribute(attName);
		}

		public virtual IList<Variable> Variables
		{
			get
			{
				List<Variable> result = new List<Variable>();
				addVariables(rootGroup, result);
				return result;
			}
		}

		private void addVariables(Group g, IList<Variable> result)
		{
			if (g == null)
			{
				return;
			}
			foreach (Variable v in g.Variables)
			{
				result.Add(v);
			}
			foreach (Group nested in g.Groups)
			{
				addVariables(nested, result);
			}
		}

		public virtual Variable? findVariable(string fullNameEscaped)
		{
			if (string.ReferenceEquals(fullNameEscaped, null) || fullNameEscaped.Length == 0)
			{
				return null;
			}

			Group g = rootGroup;
			string vars = fullNameEscaped;

			int pos = fullNameEscaped.LastIndexOf('/');
			if (pos >= 0)
			{
				string groups = fullNameEscaped.Substring(0, pos);
				vars = fullNameEscaped.Substring(pos + 1);
				foreach (string name in groups.Split('/'))
				{
					Group ng = g.findGroupLocal(name);
					if (ng == null)
					{
						return null;
					}
					g = ng;
				}
			}

			// heres var.var - tokenize respecting the possible escaped '.'
			System.Collections.Generic.IList<string> snames = NetcdfSharp.ucar.nc2.util.EscapeStrings.tokenizeEscapedName(vars);
			if (snames.Count == 0)
			{
				return null;
			}

			string varShortName = NetcdfFiles.makeNameUnescaped(snames[0]);
			Variable v = g.findVariableLocal(varShortName);
			if (v == null)
			{
				return null;
			}

			int memberCount = 1;
			while (memberCount < snames.Count)
			{
				if (!(v is Structure))
				{
					return null;
				}
				string name = NetcdfFiles.makeNameUnescaped(snames[memberCount++]);
				v = ((Structure)v).findVariable(name);
				if (v == null)
				{
					return null;
				}
			}
			return v;
		}

		public virtual Variable? findVariable(Group g, string shortName)
		{
			if (g == null)
			{
				g = rootGroup;
			}
			return g.findVariable(shortName);
		}

		public virtual IList<NcAttribute> GlobalAttributes
		{
			get
			{
				List<NcAttribute> result = new List<NcAttribute>();
				addGlobalAttributes(rootGroup, result);
				return result;
			}
		}

		private void addGlobalAttributes(Group g, IList<NcAttribute> result)
		{
			if (g == null)
			{
				return;
			}
			foreach (NcAttribute oldAtt in g.attributes())
			{
				if (g == rootGroup)
				{
					result.Add(oldAtt);
				}
				else
				{
					string newName = makeFullNameWithString(g, oldAtt.ShortName);
					result.Add(oldAtt.toBuilder().setName(newName).build());
				}
			}
			foreach (Group nested in g.Groups)
			{
				addGlobalAttributes(nested, result);
			}
		}

		public virtual string FileTypeId
		{
			get
			{
				return iosp != null ? iosp.FileTypeId : null;
			}
		}

		public virtual NcArray readData(Variable v2, Section section)
		{
			if (iosp == null)
			{
				throw new IOException("NetcdfFile is closed");
			}
			return iosp.readData(v2, section);
		}

		public virtual long readToByteChannel(Variable v2, Section section, WritableByteChannel channel)
		{
			if (iosp == null)
			{
				throw new IOException("NetcdfFile is closed");
			}
			return iosp.readToByteChannel(v2, section, channel);
		}

		public virtual long readToOutputStream(Variable v2, Section section, Stream @out)
		{
			if (iosp == null)
			{
				throw new IOException("NetcdfFile is closed");
			}
			return iosp.readToOutputStream(v2, section, @out);
		}

		public virtual StructureDataIterator getStructureIterator(Structure s, int bufferSize)
		{
			if (iosp == null)
			{
				throw new IOException("NetcdfFile is closed");
			}
			return iosp.getStructureIterator(s, bufferSize);
		}

		public virtual string toStringDebug(object o)
		{
			return iosp != null ? iosp.toStringDebug(o) : null;
		}

		public static string makeValidCDLName(string vname)
		{
			return EscapeStrings.backslashEscape(vname, "[ !\"#$%&'()*,:;<=>?[]^`{|}~\\");
		}

		public static string makeNameUnescaped(string vname)
		{
			return EscapeStrings.backslashUnescape(vname);
		}

		public static string makeValidCdmObjectName(string shortName)
		{
			return StringUtil2.makeValidCdmObjectName(shortName);
		}

		internal static string makeFullName(CDMNode v)
		{
			if (v is Group g)
			{
				return NetcdfFiles.makeFullName(g);
			}
			if (v is Variable var)
			{
				return NetcdfFiles.makeFullName(var);
			}
			return EscapeStrings.backslashEscape(v.ShortName, ".\\");
		}

		private static readonly Logger log = LoggerFactory.getLogger(typeof(NetcdfFile));

		public const string IOSP_MESSAGE_RANDOM_ACCESS_FILE = "RandomAccessFile";
		public const string IOSP_MESSAGE_GET_NETCDF_FILE_FORMAT = "NetcdfFileFormat";
		public const string IOSP_MESSAGE_ADD_RECORD_STRUCTURE = "AddRecordStructure";
		private const int default_buffersize = 8092;
		private static readonly IList<IOServiceProvider> registeredProviders = new List<IOServiceProvider>();
		private static readonly StringLocker stringLocker = new StringLocker();

		internal static bool debugSPI, debugCompress;
		internal static bool debugStructureIterator;
		private static bool loadWarnings, showRequest;
		private static bool userLoads;

		// IOSPs are loaded by reflection.
		// TODO: Replace these using the ServiceLoader mechanism. One problem with this is that we no longer
		// control the order which IOSPs try to open. So its harder to avoid mis-behaving and slow IOSPs from
		// making open() slow.
		static NetcdfFile()
		{

			try
			{
				registerIOProvider("NetcdfSharp.ucar.nc2.stream.NcStreamIosp");
			}
			catch (Exception e)
			{
				if (loadWarnings)
				{
					log.info("Cant load class NcStreamIosp", e);
				}
			}
			try
			{
				registerIOProvider("NetcdfSharp.ucar.nc2.iosp.hdf4.H4iosp");
			}
			catch (Exception e)
			{
				if (loadWarnings)
				{
					log.info("Cant load class H4iosp", e);
				}
			}

			userLoads = true;
		}

		/// @deprecated do not use 
		[Obsolete("do not use")]
		public static DebugFlags DebugFlags
		{
			set
			{
				debugSPI = value.isSet("NetcdfFile/debugSPI");
				debugCompress = value.isSet("NetcdfFile/debugCompress");
				debugStructureIterator = value.isSet("NetcdfFile/structureIterator");
				N3headerNew.disallowFileTruncation = value.isSet("NetcdfFile/disallowFileTruncation");
				N3headerNew.disallowFileTruncation = value.isSet("NetcdfFile/disallowFileTruncation");
				N3headerNew.debugHeaderSize = value.isSet("NetcdfFile/debugHeaderSize");
				N3headerNew.debugHeaderSize = value.isSet("NetcdfFile/debugHeaderSize");
				showRequest = value.isSet("NetcdfFile/showRequest");
			}
		}

		//////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Register an IOServiceProvider, using its class string name.
		/// </summary>
		/// <param name="className"> Class that implements IOServiceProvider. </param>
		/// <exception cref="IllegalAccessException"> if class is not accessible. </exception>
		/// <exception cref="InstantiationException"> if class doesnt have a no-arg constructor. </exception>
		/// <exception cref="ClassNotFoundException"> if class not found. </exception>
		/// @deprecated use NetcdfFiles.registerIOProvider 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use NetcdfFiles.registerIOProvider") public static void registerIOProvider(String className) throws IllegalAccessException, InstantiationException, ClassNotFoundException
		[Obsolete("use NetcdfFiles.registerIOProvider")]
		public static void registerIOProvider(string className)
		{
			Type ioClass = getClass(className);
			registerIOProvider(ioClass);
		}

		private static Type getClass(string className)
		{
			Type t = Type.GetType(className);
			if (t == null)
			{
				foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
				{
					t = a.GetType(className);
					if (t != null)
					{
						break;
					}
				}
			}
			return t;
		}

		/// <summary>
		/// Register an IOServiceProvider. A new instance will be created when one of its files is opened.
		/// </summary>
		/// <param name="iospClass"> Class that implements IOServiceProvider. </param>
		/// <exception cref="IllegalAccessException"> if class is not accessible. </exception>
		/// <exception cref="InstantiationException"> if class doesnt have a no-arg constructor. </exception>
		/// <exception cref="ClassCastException"> if class doesnt implement IOServiceProvider interface. </exception>
		/// @deprecated use NetcdfFiles.registerIOProvider 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use NetcdfFiles.registerIOProvider") public static void registerIOProvider(Class iospClass) throws IllegalAccessException, InstantiationException
		[Obsolete("use NetcdfFiles.registerIOProvider")]
		public static void registerIOProvider(Type iospClass)
		{
			registerIOProvider(iospClass, false);
		}

		/// <summary>
		/// Register an IOServiceProvider. A new instance will be created when one of its files is opened.
		/// </summary>
		/// <param name="iospClass"> Class that implements IOServiceProvider. </param>
		/// <param name="last"> true=>insert at the end of the list; otherwise front </param>
		/// <exception cref="IllegalAccessException"> if class is not accessible. </exception>
		/// <exception cref="InstantiationException"> if class doesnt have a no-arg constructor. </exception>
		/// <exception cref="ClassCastException"> if class doesnt implement IOServiceProvider interface. </exception>
		/// @deprecated use NetcdfFiles.registerIOProvider 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use NetcdfFiles.registerIOProvider") public static void registerIOProvider(Class iospClass, boolean last) throws IllegalAccessException, InstantiationException
		[Obsolete("use NetcdfFiles.registerIOProvider")]
		public static void registerIOProvider(Type iospClass, bool last)
		{
			IOServiceProvider spi;
			spi = (IOServiceProvider)System.Activator.CreateInstance(iospClass); // fail fast
			if (userLoads && !last)
			{
				registeredProviders.Insert(0, spi); // put user stuff first
			}
			else
			{
				registeredProviders.Add(spi);
			}
		}

		/// <summary>
		/// Set properties. Currently recognized:
		/// "syncExtendOnly", "true" or "false" (default). if true, can only extend file on a sync.
		/// </summary>
		/// <param name="name"> name of property </param>
		/// <param name="value"> value of property </param>
		/// @deprecated do not use 
		[Obsolete("do not use")]
		public static void setProperty(string name, string value)
		{
			// legacy N3iosp.setProperty is not part of the new iosp; no-op in NetcdfSharp.
		}

		///////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Open an existing netcdf file (read only).
		/// </summary>
		/// <param name="location"> location of file. </param>
		/// <returns> the NetcdfFile. </returns>
		/// <exception cref="IOException"> if error </exception>
		/// @deprecated use NetcdfFiles.open 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use NetcdfFiles.open") public static NetcdfFile open(String location) throws NetcdfSharp.java.io.IOException
		[Obsolete("use NetcdfFiles.open")]
		public static NetcdfFile open(string location)
		{
			return open(location, null);
		}

		/// <summary>
		/// Open an existing file (read only), with option of cancelling.
		/// </summary>
		/// <param name="location"> location of the file. </param>
		/// <param name="cancelTask"> allow task to be cancelled; may be null. </param>
		/// <returns> NetcdfFile object, or null if cant find IOServiceProver </returns>
		/// <exception cref="IOException"> if error </exception>
		/// @deprecated use NetcdfFiles.open 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use NetcdfFiles.open") public static NetcdfFile open(String location, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
		[Obsolete("use NetcdfFiles.open")]
		public static NetcdfFile open(string location, CancelTask cancelTask)
		{
			return open(location, -1, cancelTask);
		}

		/// <summary>
		/// Open an existing file (read only), with option of cancelling, setting the RandomAccessFile buffer size for
		/// efficiency.
		/// </summary>
		/// <param name="location"> location of file. </param>
		/// <param name="buffer_size"> RandomAccessFile buffer size, if <= 0, use default size </param>
		/// <param name="cancelTask"> allow task to be cancelled; may be null. </param>
		/// <returns> NetcdfFile object, or null if cant find IOServiceProver </returns>
		/// <exception cref="IOException"> if error </exception>
		/// @deprecated use NetcdfFiles.open 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use NetcdfFiles.open") public static NetcdfFile open(String location, int buffer_size, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
		[Obsolete("use NetcdfFiles.open")]
		public static NetcdfFile open(string location, int buffer_size, CancelTask cancelTask)
		{
			return open(location, buffer_size, cancelTask, null);
		}

		/// <summary>
		/// Open an existing file (read only), with option of cancelling, setting the RandomAccessFile buffer size for
		/// efficiency,
		/// with an optional special object for the iosp.
		/// </summary>
		/// <param name="location"> location of file. This may be a
		///        <ol>
		///        <li>local netcdf-3 filename (with a file: prefix or no prefix)
		///        <li>remote netcdf-3 filename (with an http: prefix)
		///        <li>local netcdf-4 filename (with a file: prefix or no prefix)
		///        <li>local hdf-5 filename (with a file: prefix or no prefix)
		///        <li>local iosp filename (with a file: prefix or no prefix)
		///        </ol>
		///        http://thredds.ucar.edu/thredds/fileServer/grib/NCEP/GFS/Alaska_191km/files/GFS_Alaska_191km_20130416_0600.grib1
		///        If file ends with ".Z", ".zip", ".gzip", ".gz", or ".bz2", it will uncompress/unzip and write to new file
		///        without the suffix,
		///        then use the uncompressed file. It will look for the uncompressed file before it does any of that. Generally
		///        it prefers to
		///        place the uncompressed file in the same directory as the original file. If it does not have write permission
		///        on that directory,
		///        it will use the directory defined by NetcdfSharp.ucar.nc2.util.DiskCache class. </param>
		/// <param name="buffer_size"> RandomAccessFile buffer size, if <= 0, use default size </param>
		/// <param name="cancelTask"> allow task to be cancelled; may be null. </param>
		/// <param name="iospMessage"> special iosp tweaking (sent before open is called), may be null </param>
		/// <returns> NetcdfFile object, or null if cant find IOServiceProver </returns>
		/// <exception cref="IOException"> if error </exception>
		/// @deprecated use NetcdfFiles.open 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use NetcdfFiles.open") public static NetcdfFile open(String location, int buffer_size, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask, Object iospMessage) throws NetcdfSharp.java.io.IOException
		[Obsolete("use NetcdfFiles.open")]
		public static NetcdfFile open(string location, int buffer_size, CancelTask cancelTask, object iospMessage)
		{

			RandomAccessFile raf = getRaf(location, buffer_size);

			try
			{
				return open(raf, location, cancelTask, iospMessage);
			}
			catch (Exception t)
			{
				raf.close();
				throw new IOException(t.Message, t);
			}
		}

		/// <summary>
		/// Find out if the file can be opened, but dont actually open it.
		/// Experimental.
		/// </summary>
		/// <param name="location"> same as open </param>
		/// <returns> true if can be opened </returns>
		/// <exception cref="IOException"> on read error </exception>
		/// @deprecated use NetcdfFiles.canOpen 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use NetcdfFiles.canOpen") public static boolean canOpen(String location) throws NetcdfSharp.java.io.IOException
		[Obsolete("use NetcdfFiles.canOpen")]
		public static bool canOpen(string location)
		{
			using (RandomAccessFile raf = getRaf(location, -1))
			{
				return (raf != null) && canOpen(raf);
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private static boolean canOpen(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
		private static bool canOpen(RandomAccessFile raf)
		{
			if (N3headerNew.isValidFile(raf))
			{
				return true;
			}
			else
			{
				foreach (IOServiceProvider iosp in ServiceLoader.load(typeof(IOServiceProvider)))
				{
					// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.info("ServiceLoader IOServiceProvider {}", iosp.GetType().FullName);
					// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
					Console.Write("ServiceLoader IOServiceProvider found {0}%n", iosp.GetType().FullName);
					if (iosp.isValidFile(raf))
					{
						return true;
					}
				}
				foreach (IOServiceProvider registeredSpi in registeredProviders)
				{
					if (registeredSpi.isValidFile(raf))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Open an existing file (read only), specifying which IOSP is to be used.
		/// </summary>
		/// <param name="location"> location of file </param>
		/// <param name="iospClassName"> fully qualified class name of the IOSP class to handle this file </param>
		/// <param name="bufferSize"> RandomAccessFile buffer size, if <= 0, use default size </param>
		/// <param name="cancelTask"> allow task to be cancelled; may be null. </param>
		/// <param name="iospMessage"> special iosp tweaking (sent before open is called), may be null </param>
		/// <returns> NetcdfFile object, or null if cant find IOServiceProver </returns>
		/// <exception cref="IOException"> if read error </exception>
		/// <exception cref="ClassNotFoundException"> cannat find iospClassName in thye class path </exception>
		/// <exception cref="InstantiationException"> if class cannot be instantiated </exception>
		/// <exception cref="IllegalAccessException"> if class is not accessible </exception>
		/// @deprecated use NetcdfFiles.open 
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: @Deprecated("use NetcdfFiles.open") public static NetcdfFile open(String location, String iospClassName, int bufferSize, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask, Object iospMessage) throws ClassNotFoundException, IllegalAccessException, InstantiationException, NetcdfSharp.java.io.IOException
		[Obsolete("use NetcdfFiles.open")]
		public static NetcdfFile open(string location, string iospClassName, int bufferSize, CancelTask cancelTask, object iospMessage)
		{

			Type iospClass = getClass(iospClassName);
			IOServiceProvider spi = (IOServiceProvider)System.Activator.CreateInstance(iospClass); // fail fast

			// send iospMessage before iosp is opened
			if (iospMessage != null)
			{
				spi.sendIospMessage(iospMessage);
			}

			if (bufferSize <= 0)
			{
				bufferSize = default_buffersize;
			}

			RandomAccessFile raf = RandomAccessFile.acquire(canonicalizeUriString(location), bufferSize);

			NetcdfFile result = new NetcdfFile(spi, raf, location, cancelTask);

			// send after iosp is opened
			if (iospMessage != null)
			{
				spi.sendIospMessage(iospMessage);
			}

			return result;
		}

		/// <summary>
		/// Removes the {@code "file:"} or {@code "file://"} prefix from the location, if necessary. Also replaces
		/// back slashes with forward slashes.
		/// </summary>
		/// <param name="location"> a URI string. </param>
		/// <returns> a canonical URI string. </returns>
		/// @deprecated use NetcdfFiles.canonicalizeUriString 
		[Obsolete("use NetcdfFiles.canonicalizeUriString")]
		public static string canonicalizeUriString(string location)
		{
			// get rid of file prefix, if any
			string uriString = location.Trim();
			if (uriString.StartsWith("file://", StringComparison.Ordinal))
			{
				uriString = uriString.Substring(7);
			}
			else if (uriString.StartsWith("file:", StringComparison.Ordinal))
			{
				uriString = uriString.Substring(5);
			}

			// get rid of crappy microsnot \ replace with happy /
			return StringUtil2.replace(uriString, '\\', "/");
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private static NetcdfSharp.ucar.unidata.io.RandomAccessFile getRaf(String location, int buffer_size) throws NetcdfSharp.java.io.IOException
		private static RandomAccessFile getRaf(string location, int buffer_size)
		{

			string uriString = location.Trim();

			if (buffer_size <= 0)
			{
				buffer_size = default_buffersize;
			}

			RandomAccessFile raf;
			if (uriString.StartsWith("http:", StringComparison.Ordinal) || uriString.StartsWith("https:", StringComparison.Ordinal))
			{ // open through URL
				throw new NotSupportedException("HTTP access is not supported in NetcdfSharp.");
			}
			else if (uriString.StartsWith("nodods:", StringComparison.Ordinal))
			{ // deprecated use httpserver
				throw new NotSupportedException("HTTP access is not supported in NetcdfSharp.");
			}
			else if (uriString.StartsWith("httpserver:", StringComparison.Ordinal))
			{ // open through URL
				throw new NotSupportedException("HTTP access is not supported in NetcdfSharp.");
			}
			else if (uriString.StartsWith("slurp:", StringComparison.Ordinal))
			{ // open through URL
				uriString = "http" + uriString.Substring(5);
				byte[] contents = IO.readURLContentsToByteArray(uriString); // read all into memory
				raf = new InMemoryRandomAccessFile(uriString, contents);

			}
			else
			{
				// get rid of crappy microsnot \ replace with happy /
				uriString = StringUtil2.replace(uriString, '\\', "/");

				if (uriString.StartsWith("file:", StringComparison.Ordinal))
				{
					// uriString = uriString.substring(5);
					uriString = StringUtil2.unescape(uriString.Substring(5)); // 11/10/2010 from erussell@ngs.org
				}

				string uncompressedFileName = null;
				try
				{
					stringLocker.control(uriString); // Avoid race condition where the decompressed file is trying to be read by one
													 // thread while another is decompressing it
					uncompressedFileName = makeUncompressed(uriString);
				}
				catch (Exception e)
				{
					log.warn("Failed to uncompress {}, err= {}; try as a regular file.", uriString, e.Message);
					// allow to fall through to open the "compressed" file directly - may be a misnamed suffix
				}
				finally
				{
					stringLocker.release(uriString);
				}

				if (!string.ReferenceEquals(uncompressedFileName, null))
				{
					// open uncompressed file as a RandomAccessFile.
					raf = RandomAccessFile.acquire(uncompressedFileName, buffer_size);
					// raf = new NetcdfSharp.ucar.unidata.io.MMapRandomAccessFile(uncompressedFileName, "r");

				}
				else
				{
					// normal case - not compressed
					raf = RandomAccessFile.acquire(uriString, buffer_size);
					// raf = new NetcdfSharp.ucar.unidata.io.MMapRandomAccessFile(uriString, "r");
				}
			}

			return raf;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private static String makeUncompressed(String filename) throws NetcdfSharp.java.io.IOException
		private static string makeUncompressed(string filename)
		{
			// see if its a compressed file
			int pos = filename.LastIndexOf('.');
			if (pos < 0)
			{
				return null;
			}

			string suffix = filename.Substring(pos + 1);
			string uncompressedFilename = filename.Substring(0, pos);

			if (!suffix.Equals("Z", StringComparison.OrdinalIgnoreCase) && !suffix.Equals("zip", StringComparison.OrdinalIgnoreCase) && !suffix.Equals("gzip", StringComparison.OrdinalIgnoreCase) && !suffix.Equals("gz", StringComparison.OrdinalIgnoreCase) && !suffix.Equals("bz2", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}

			// coverity claims resource leak, but attempts to fix break. so beware
			// see if already decompressed (cache removed; look in the same directory)
			File uncompressedFile = new File(uncompressedFilename);
			if (uncompressedFile.exists() && uncompressedFile.length() > 0)
			{
				if (debugCompress)
				{
					log.info("found uncompressed {} for {}", uncompressedFile, filename);
				}
				return uncompressedFile.getPath();
			}
			// make sure compressed file exists
			File file = new File(filename);
			if (!file.exists())
			{
				return null; // bail out */
			}

			using (FileStream fout = new FileStream(uncompressedFile.GetPath(), FileMode.Create, FileAccess.Write))
			{

				try
				{
					if (suffix.Equals("Z", StringComparison.OrdinalIgnoreCase))
					{
						// Z file can only contain one file - copy the whole thing
						using (Stream @in = new UncompressInputStream(new BufferedInputStream(new FileStream(filename, FileMode.Open, FileAccess.Read))))
						{
							copy(@in, fout, 100000);
						}
						if (debugCompress)
						{
							log.info("uncompressed {} to {}", filename, uncompressedFile);
						}

					}
					else if (suffix.Equals("zip", StringComparison.OrdinalIgnoreCase))
					{
						using (ZipArchive zin = new ZipArchive(new BufferedInputStream(new FileStream(filename, FileMode.Open, FileAccess.Read)), ZipArchiveMode.Read))
						{
							ZipArchiveEntry ze = zin.Entries.Count > 0 ? zin.Entries[0] : null;
							if (ze != null)
							{
								using (Stream @in = ze.Open())
								{
									copy(@in, fout, 100000);
								}
								if (debugCompress)
								{
									log.info("unzipped {} entry {} to {}", filename, ze.FullName, uncompressedFile);
								}
							}
						}

					}
					else if (suffix.Equals("bz2", StringComparison.OrdinalIgnoreCase))
					{
						using (Stream @in = new CBZip2InputStream(new BufferedInputStream(new FileStream(filename, FileMode.Open, FileAccess.Read)), true))
						{
							copy(@in, fout, 100000);
						}
						if (debugCompress)
						{
							log.info("unbzipped {} to {}", filename, uncompressedFile);
						}

					}
					else if (suffix.Equals("gzip", StringComparison.OrdinalIgnoreCase) || suffix.Equals("gz", StringComparison.OrdinalIgnoreCase))
					{
						using (Stream @in = new GZipStream(new BufferedInputStream(new FileStream(filename, FileMode.Open, FileAccess.Read)), CompressionMode.Decompress))
						{
							copy(@in, fout, 100000);
						}

						if (debugCompress)
						{
							log.info("ungzipped {} to {}", filename, uncompressedFile);
						}
					}
				}
				catch (Exception e)
				{
					log.warn("Failed to uncompress file {}", filename, e);
					// dont leave bad files around
					if (uncompressedFile.exists())
					{
						if (!uncompressedFile.delete())
						{
							log.warn("failed to delete uncompressed file (IOException) {}", uncompressedFile);
						}
					}
					throw;

				}
				finally
				{
				}
			}

			return uncompressedFile.getPath();
		}

		private static void copy(Stream @in, Stream @out, int bufferSize)
		{
			byte[] buffer = new byte[bufferSize];
			while (true)
			{
				int bytesRead = @in.Read(buffer, 0, buffer.Length);
				if (bytesRead == -1)
				{
					break;
				}
				@out.Write(buffer, 0, bytesRead);
			}
		}

		/// <summary>
		/// Open an in-memory netcdf file, with a specific iosp.
		/// </summary>
		/// <param name="name"> name of the dataset. Typically use the filename or URI. </param>
		/// <param name="data"> in-memory netcdf file </param>
		/// <param name="iospClassName"> fully qualified class name of the IOSP class to handle this file </param>
		/// <returns> NetcdfFile object, or null if cant find IOServiceProver </returns>
		/// @deprecated use NetcdfFiles.openInMemory 
		[Obsolete("use NetcdfFiles.openInMemory")]
		public static NetcdfFile openInMemory(string name, byte[] data, string iospClassName)
		{
			InMemoryRandomAccessFile raf = new InMemoryRandomAccessFile(name, data);
			Type iospClass = getClass(iospClassName);
			IOServiceProvider spi = (IOServiceProvider)System.Activator.CreateInstance(iospClass);

			return new NetcdfFile(spi, raf, name, null);
		}

		/// <summary>
		/// Open an in-memory netcdf file.
		/// </summary>
		/// <param name="name"> name of the dataset. Typically use the filename or URI. </param>
		/// <param name="data"> in-memory netcdf file </param>
		/// <returns> memory-resident NetcdfFile </returns>
		/// @deprecated use NetcdfFiles.openInMemory 
		[Obsolete("use NetcdfFiles.openInMemory")]
		public static NetcdfFile openInMemory(string name, byte[] data)
		{
			InMemoryRandomAccessFile raf = new InMemoryRandomAccessFile(name, data);
			return open(raf, name, null, null);
		}

		/// <summary>
		/// Read a local CDM file into memory. All reads are then done from memory.
		/// </summary>
		/// <param name="filename"> location of CDM file, must be a local file. </param>
		/// <returns> a NetcdfFile, which is completely in memory </returns>
		/// @deprecated use NetcdfFiles.openInMemory 
		[Obsolete("use NetcdfFiles.openInMemory")]
		public static NetcdfFile openInMemory(string filename)
		{
			File file = new File(filename);
			MemoryStream bos = new MemoryStream((int)file.length());
			using (Stream @in = new BufferedInputStream(new FileStream(filename, FileMode.Open, FileAccess.Read)))
			{
				IO.copy(@in, bos);
			}
			return openInMemory(filename, bos.ToArray());
		}

		/// <summary>
		/// Read a remote CDM file into memory. All reads are then done from memory.
		/// </summary>
		/// <param name="uri"> location of CDM file, must be accessible through url.toURL().openStream(). </param>
		/// <returns> a NetcdfFile, which is completely in memory </returns>
		/// @deprecated use NetcdfFiles.openInMemory 
		[Obsolete("use NetcdfFiles.openInMemory")]
		public static NetcdfFile openInMemory(URI uri)
		{
			URL url = uri.toURL();
			byte[] contents;
			using (Stream @in = url.openStream())
			{
				contents = IO.readContentsToByteArray(@in);
			}
			return openInMemory(uri.ToString(), contents);
		}

		/// <summary>
		/// Open a RandomAccessFile as a NetcdfFile, if possible.
		/// </summary>
		/// <param name="raf"> The open raf, is not cloised by this method. </param>
		/// <param name="location"> human readable locatoin of this dataset. </param>
		/// <param name="cancelTask"> used to monitor user cancellation; may be null. </param>
		/// <param name="iospMessage"> send this message to iosp; may be null. </param>
		/// <returns> NetcdfFile or throw an Exception. </returns>
		/// @deprecated use NetcdfFiles.open 
		[Obsolete("use NetcdfFiles.open")]
		public static NetcdfFile open(RandomAccessFile raf, string location, CancelTask cancelTask, object iospMessage)
		{
			IOServiceProvider spi = getIosp(raf);
			if (spi == null)
			{
				raf.close();
				throw new IOException("Cant read " + location + ": not a valid CDM file.");
			}

			// send iospMessage before the iosp is opened
			if (iospMessage != null)
			{
				spi.sendIospMessage(iospMessage);
			}

			if (log.isDebugEnabled())
			{
				log.debug("Using IOSP {}", spi.GetType().FullName);
			}

			NetcdfFile result = new NetcdfFile(spi, raf, location, cancelTask);

			// send iospMessage after iosp is opened
			if (iospMessage != null)
			{
				spi.sendIospMessage(iospMessage);
			}

			return result;
		}

		private static IOServiceProvider getIosp(RandomAccessFile raf)
		{
			if (debugSPI)
			{
				log.info("NetcdfFile try to open = {}", raf.Location);
			}

			// Registered providers override defaults.
			foreach (IOServiceProvider registeredSpi in registeredProviders)
			{
				if (debugSPI)
				{
					log.info(" try iosp = {}", registeredSpi.GetType().FullName);
				}

				if (registeredSpi.isValidFile(raf))
				{
					// need a new instance for thread safety
					Type c = registeredSpi.GetType();
					try
					{
						return (IOServiceProvider)System.Activator.CreateInstance(c);
					}
					catch (System.MissingMethodException)
					{
						throw new IOException("IOServiceProvider " + c.FullName + "must have no-arg constructor.");
					}
					catch (System.MemberAccessException e)
					{
						throw new IOException("IOServiceProvider " + c.FullName + " IllegalAccessException: " + e.Message);
					}
				}
			}

			if (N3headerNew.isValidFile(raf))
			{
				return new N3iospNew();

			}
			else if (H5headerNew.isValidFile(raf))
			{
				return new H5iospNew();

			}
			else
			{
				// look for dynamically loaded IOSPs
				foreach (IOServiceProvider loadedSpi in ServiceLoader.load(typeof(IOServiceProvider)))
				{
					if (loadedSpi.isValidFile(raf))
					{
						Type c = loadedSpi.GetType();
						try
						{
							return (IOServiceProvider)System.Activator.CreateInstance(c);
						}
						catch (System.MissingMethodException)
						{
							throw new IOException("IOServiceProvider " + c.FullName + "must have no-arg constructor.");
						}
						catch (System.MemberAccessException e)
						{
							throw new IOException("IOServiceProvider " + c.FullName + " IllegalAccessException: " + e.Message);
						}
					}
				}
			}
			return null;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Close all resources (files, sockets, etc) associated with this file.
		/// If the underlying file was acquired, it will be released, otherwise closed.
		/// if isClosed() already, nothing will happen
		/// </summary>
		public virtual void close()
		{
			try
			{
				if (null != iosp)
				{
					// log.warn("NetcdfFile.close called for ncfile="+this.hashCode()+" for iosp="+spi.hashCode());
					iosp.close();
				}
			}
			finally
			{
				iosp = null;
			}
		}

		public virtual void Dispose()
		{
			close();
		}

		/// <summary>
		/// Public by accident.
		/// Release any resources like file handles
		/// </summary>
		/// @deprecated do not use 
		[Obsolete("do not use")]
		public virtual void release()
		{
			if (iosp != null)
			{
				iosp.release();
			}
		}

		/// <summary>
		/// Public by accident.
		/// Reacquire any resources like file handles
		/// </summary>
		/// @deprecated do not use 
		[Obsolete("do not use")]
		public virtual void reacquire()
		{
			if (iosp != null)
			{
				iosp.reacquire();
			}
		}



		protected internal string location, id, title;
		protected internal Group rootGroup;
		protected internal AbstractIOServiceProvider iosp;
		private bool immutable;

		public virtual void finish()
		{
			if (immutable)
			{
				throw new System.InvalidOperationException("Cant modify");
			}
		}

		protected internal string makeFullNameWithString(Group parent, string name)
		{
			return parent.FullName + "/" + name;
		}

		protected internal NetcdfFile(Builder builder)
		{
			this.location = builder.location;
			this.id = builder.id;
			this.title = builder.title;
			this.iosp = builder.iosp;
			if (builder.rootGroup != null)
			{
				builder.rootGroup.setNcfile(this);
				this.rootGroup = builder.rootGroup.build();
			}
			if (builder.iosp != null)
			{
				builder.iosp.NetcdfFile = this;
				this.iosp = builder.iosp;
			}
			finish(); // LOOK
		}

		protected internal NetcdfFile(IOServiceProvider spi, RandomAccessFile raf, string location, CancelTask cancelTask)
		{
			this.iosp = (AbstractIOServiceProvider)spi;
			this.location = location;

			try
			{
				spi.open(raf, this, cancelTask);
			}
			catch (IOException)
			{
				try
				{
					spi.close();
				}
				catch (Exception)
				{
				}
				try
				{
					raf.close();
				}
				catch (Exception)
				{
				}
				this.iosp = null;
				throw;
			}
			finish();
		}

		/// <summary>
		/// Open an existing netcdf file, given the class name of the iosp.
		/// Use NetcdfFileSubclass to access this constructor
		/// </summary>
		/// <param name="iospClassName"> class that implements IOServiceProvider. </param>
		/// <param name="iospParam"> send this message to iosp; may be null. </param>
		/// <param name="location"> location of data. </param>
		/// <param name="buffer_size"> read buffer size. </param>
		/// <param name="cancelTask"> allow user to cancel. </param>
		/// <exception cref="IOException"> on error </exception>
		/// <exception cref="IllegalAccessException"> if class is not accessible. </exception>
		/// <exception cref="InstantiationException"> if class doesnt have a no-arg constructor. </exception>
		/// <exception cref="ClassNotFoundException"> if class not found. </exception>
		/// @deprecated use NetcdfFile.builder()
		[Obsolete("use NetcdfFile.builder()")]
		protected internal NetcdfFile(string iospClassName, object iospParam, string location, int buffer_size, CancelTask cancelTask)
		{
			Type iospClass = getClass(iospClassName);
			AbstractIOServiceProvider iosp = (AbstractIOServiceProvider)System.Activator.CreateInstance(iospClass);
			if (debugSPI)
			{
				log.info("NetcdfFile uses iosp = {}", iosp.GetType().FullName);
			}
			if (iospParam != null)
			{
				iosp.sendIospMessage(iospParam);
			}

			this.location = location;
			RandomAccessFile raf = getRaf(location, buffer_size);

			try
			{
				this.iosp = iosp;
				this.iosp.open(raf, this, cancelTask);
				finish();
			}
			catch (IOException)
			{
				try
				{
					this.iosp.close();
				}
				catch (Exception)
				{
				}
				try
				{
					raf.close();
				}
				catch (Exception)
				{
				}
				this.iosp = null;
				throw;
			}
			catch (Exception t)
			{
				try
				{
					this.iosp.close();
				}
				catch (Exception)
				{
				}
				try
				{
					raf.close();
				}
				catch (Exception)
				{
				}
				this.iosp = null;
				throw new System.InvalidOperationException(t.Message, t);
			}

			if (id == null)
			{
				id = rootGroup.findAttributeString("_Id", null);
			}
			if (title == null)
			{
				title = rootGroup.findAttributeString("_Title", null);
			}
		}

		/// <summary>
		/// Open an existing netcdf file (read only) , but dont do nuttin else
		/// Use NetcdfFileSubclass to access this constructor
		/// </summary>
		/// <param name="spi"> use this IOServiceProvider instance </param>
		/// <param name="location"> location of data </param>
		/// @deprecated use NetcdfFile.builder()
		[Obsolete("use NetcdfFile.builder()")]
		protected internal NetcdfFile(IOServiceProvider spi, string location)
		{
			this.iosp = (AbstractIOServiceProvider)spi;
			this.location = location;
		}

		/// <summary>
		/// For subclass construction.
		/// Use NetcdfFileSubclass to access this constructor
		/// </summary>
		/// @deprecated use NetcdfFile.builder()
		[Obsolete("use NetcdfFile.builder()")]
		protected internal NetcdfFile()
		{
		}

		/// <summary>
		/// Turn into a mutable Builder. Can use toBuilder().build() to copy. </summary>
		public virtual Builder toBuilder()
		{
			return addLocalFieldsToBuilder(builder());
		}

		// Add local fields to the passed - in builder.
		protected internal virtual Builder addLocalFieldsToBuilder(Builder b)
		{
			return b.setLocation(this.location).setId(this.id).setTitle(this.title).setRootGroup(this.rootGroup.toBuilder()).setIosp((AbstractIOServiceProvider)this.iosp);
		}

		/// <summary>
		/// Get Builder for this class.
		/// Allows subclassing.
		/// </summary>
		public static Builder builder()
		{
			return new Builder();
		}

		/// <summary>
		/// A builder of NetcdfFile objects. </summary>
		public class Builder
		{
			public Group.Builder rootGroup = Group.builder().setName("");
			internal string id;
			internal string title;
			public string location;
			protected internal AbstractIOServiceProvider iosp;
			private bool built;

			public virtual Builder setRootGroup(Group.Builder rootGroup)
			{
				this.rootGroup = rootGroup;
				return this;
			}

			public virtual Builder setIosp(AbstractIOServiceProvider iosp)
			{
				this.iosp = iosp;
				return this;
			}

			public virtual Builder setId(string id)
			{
				this.id = id;
				return this;
			}

			/// <summary>
			/// Set the dataset "human readable" title. </summary>
			public virtual Builder setTitle(string title)
			{
				this.title = title;
				return this;
			}

			/// <summary>
			/// Set the location, a URL or local filename. </summary>
			public virtual Builder setLocation(string location)
			{
				this.location = location;
				return this;
			}

			public virtual NetcdfFile build()
			{
				if (built)
				{
					throw new System.InvalidOperationException("already built");
				}
				built = true;
				return new NetcdfFile(this);
			}
		}
	}

}
