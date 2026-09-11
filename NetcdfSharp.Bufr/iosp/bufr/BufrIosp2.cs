// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2020 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{

	using Element = NetcdfSharp.org.jdom2.Element;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using ArraySequence = NetcdfSharp.ucar.ma2.ArraySequence;
	using ArrayStructure = NetcdfSharp.ucar.ma2.ArrayStructure;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using StructureData = NetcdfSharp.ucar.ma2.StructureData;
	using StructureDataIterator = NetcdfSharp.ucar.ma2.StructureDataIterator;
	using Group = NetcdfSharp.ucar.nc2.Group;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using Sequence = NetcdfSharp.ucar.nc2.Sequence;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using Variable = NetcdfSharp.ucar.nc2.Variable;
	using DataFormatType = NetcdfSharp.ucar.nc2.constants.DataFormatType;
	using AbstractIOServiceProvider = NetcdfSharp.ucar.nc2.iosp.AbstractIOServiceProvider;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// IOSP for BUFR data - version 2, using the preprocessor.
	/// 
	/// @author caron
	/// @since 8/8/13
	/// </summary>
	public class BufrIosp2 : AbstractIOServiceProvider
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(BufrIosp2));

	  public const string obsRecordName = "obs";
	  public const string fxyAttName = "BUFR:TableB_descriptor";
	  public const string centerId = "BUFR:centerId";

	  // debugging
	  private static bool debugIter;

	  public static NetcdfSharp.ucar.nc2.util.DebugFlags DebugFlags
	  {
		  set
		  {
			debugIter = value.isSet("Bufr/iter");
		  }
	  }

	  // private Structure obsStructure;
	  // private Message protoMessage; // prototypical message: all messages in the file must be the same.
	  private MessageScanner scanner;
	  private IList<Message> protoMessages; // prototypical messages: the messages with different category.
	  private IList<RootVariable> rootVariables;
	  private HashSet<int> messHash;
	  private bool isSingle;
	  private BufrConfig config;
	  private Element iospParam;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public override bool isValidFile(RandomAccessFile raf)
	  {
		return MessageScanner.isValidFile(raf);
	  }

	  public override bool Builder
	  {
		  get
		  {
			return true;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void build(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.Group.Builder rootGroup, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public override void build(RandomAccessFile raf, Group.Builder rootGroup, CancelTask cancelTask)
	  {
		base.open(raf, rootGroup.Ncfile, cancelTask);

		scanner = new MessageScanner(raf);
		Message protoMessage = scanner.FirstDataMessage;
		if (protoMessage == null)
		{
		  throw new IOException("No data messages in the file= " + raf.Location);
		}
		if (!protoMessage.TablesComplete)
		{
		  throw new System.InvalidOperationException("BUFR file has incomplete tables");
		}

		// get all prototype messages - contains different message category in a Bufr data file
		protoMessages = new List<Message>();
		protoMessages.Add(protoMessage);
		int category = protoMessage.ids.Category;
		IList<int> categories = new List<int>();
		categories.Add(category);
		while (scanner.hasNext())
		{
		  Message message = scanner.next();
		  if (!categories.Contains(message.ids.Category))
		  {
			protoMessages.Add(message);
			categories.Add(message.ids.Category);
		  }
		}

		// just get the fields
		config = BufrConfig.openFromMessage(raf, protoMessage, iospParam);

		// this fills the netcdf object
		if (this.protoMessages.Count == 1)
		{
		  new BufrIospBuilder(protoMessage, config, rootGroup, raf.Location);
		}
		else
		{
		  IList<BufrConfig> configs = new List<BufrConfig>();
		  foreach (Message message in protoMessages)
		  {
			configs.Add(BufrConfig.openFromMessage(raf, message, iospParam));
		  }
		  new BufrIospBuilder(protoMessage, configs, rootGroup, raf.Location);
		}
		isSingle = false;
	  }

	  public override void buildFinish(NetcdfFile ncfile)
	  {
		// support multiple root variables in one Bufr data file
		this.rootVariables = new List<RootVariable>();
		if (this.protoMessages.Count == 1)
		{
		  Structure obsStructure = (Structure) ncfile.findVariable(obsRecordName);
		  // The proto DataDescriptor must have a link to the Sequence object to read nested Sequences.
		  connectSequences(obsStructure.Variables, protoMessages[0].RootDataDescriptor.SubKeys);
		  this.rootVariables.Add(new RootVariable(this, protoMessages[0], obsStructure));
		}
		else
		{
		  for (int i = 0; i < this.protoMessages.Count; i++)
		  {
			Structure variable = (Structure) ncfile.Variables[i];
			Message message = protoMessages[i];
			connectSequences(variable.Variables, message.RootDataDescriptor.SubKeys);
			this.rootVariables.Add(new RootVariable(this, message, variable));
		  }
		}
	  }

	  private void connectSequences(IList<Variable> variables, IList<DataDescriptor> dataDescriptors)
	  {
		foreach (Variable v in variables)
		{
		  if (v is Sequence)
		  {
			findDataDescriptor(dataDescriptors, v.ShortName).ifPresent(dds => dds.refersTo = (Sequence) v);
		  }
		  if (v is Structure)
		  { // recurse
			findDataDescriptor(dataDescriptors, v.ShortName).ifPresent(dds => connectSequences(((Structure) v).Variables, dds.subKeys));
		  }
		}
	  }

	  private Optional<DataDescriptor> findDataDescriptor(IList<DataDescriptor> dataDescriptors, string name)
	  {
		foreach (DataDescriptor d in dataDescriptors)
		{
		  if (name.Equals(d.name))
		  {
			return Optional<DataDescriptor>.of(d);
		  }
		}
		throw new System.InvalidOperationException("DataDescriptor does not contain " + name);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void open(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public override void open(RandomAccessFile raf, NetcdfFile ncfile, CancelTask cancelTask)
	  {
		base.open(raf, ncfile, cancelTask);

		scanner = new MessageScanner(raf);
		Message protoMessage = scanner.FirstDataMessage;
		if (protoMessage == null)
		{
		  throw new IOException("No data messages in the file= " + ncfile.Location);
		}
		if (!protoMessage.TablesComplete)
		{
		  throw new System.InvalidOperationException("BUFR file has incomplete tables");
		}

		// get all prototype messages - contains different message category in a Bufr data file
		protoMessages = new List<Message>();
		protoMessages.Add(protoMessage);
		int category = protoMessage.ids.Category;
		IList<int> categories = new List<int>();
		categories.Add(category);
		while (scanner.hasNext())
		{
		  Message message = scanner.next();
		  if (!categories.Contains(message.ids.Category))
		  {
			protoMessages.Add(message);
			categories.Add(message.ids.Category);
		  }
		}

		// just get the fields
		config = BufrConfig.openFromMessage(raf, protoMessage, iospParam);

		// this fills the netcdf object
		if (this.protoMessages.Count == 1)
		{
		  Construct2 construct = new Construct2(protoMessage, config, ncfile);
		}
		else
		{
		  IList<BufrConfig> configs = new List<BufrConfig>();
		  foreach (Message message in protoMessages)
		  {
			configs.Add(BufrConfig.openFromMessage(raf, message, iospParam));
		  }
		  Construct2 construct = new Construct2(protoMessage, configs, ncfile);
		}

		ncfile.finish();
		buildFinish(ncfile);
		isSingle = false;
	  }

	  // for BufrMessageViewer
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void open(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, Message single) throws NetcdfSharp.java.io.IOException
	  public virtual void open(RandomAccessFile raf, NetcdfFile ncfile, Message single)
	  {
		this.raf = raf;

		Message protoMessage = single;
		_ = protoMessage.RootDataDescriptor; // construct the data descriptors, check for complete tables
		if (!protoMessage.TablesComplete)
		{
		  throw new System.InvalidOperationException("BUFR file has incomplete tables");
		}

		BufrConfig config = BufrConfig.openFromMessage(raf, protoMessage, null);

		// this fills the netcdf object
		Construct2 construct = new Construct2(protoMessage, config, ncfile);
		Structure obsStructure = construct.ObsStructure;
		isSingle = true;

		ncfile.finish();
		this.ncfile = ncfile;
	  }

	  public override object sendIospMessage(object message)
	  {
		if (message is Element)
		{
		  iospParam = (Element) message;
		  iospParam.detach();
		  return true;
		}

		return base.sendIospMessage(message);
	  }

	  public virtual BufrConfig Config
	  {
		  get
		  {
			return config;
		  }
	  }

	  public virtual Element Elem
	  {
		  get
		  {
			return iospParam;
		  }
	  }

	  private int nelems = -1;

	  public override NcArray readData(Variable v2, Section section)
	  {
		RootVariable rootVariable = findRootSequence(v2);
		Structure obsStructure = rootVariable.Variable;
		return new ArraySequence(obsStructure.makeStructureMembers(), new SeqIter(this, rootVariable), nelems);
	  }

	  public override StructureDataIterator getStructureIterator(Structure s, int bufferSize)
	  {
		RootVariable rootVariable = findRootSequence(s);
		return isSingle ? new SeqIterSingle(this, rootVariable) : new SeqIter(this, rootVariable);
	  }

	  private Structure findRootSequence()
	  {
		return (Structure) this.ncfile.findVariable(BufrIosp2.obsRecordName);
	  }

	  // find root sequence from root variable list
	  private RootVariable findRootSequence(Variable var)
	  {
		foreach (RootVariable rootVariable in this.rootVariables)
		{
		  if (rootVariable.Variable.ShortName.Equals(var.ShortName))
		  {
			return rootVariable;
		  }
		}
		return null;
	  }

	  // root variable contains prototype message and corresponding variable
	  private class RootVariable
	  {
		  private readonly BufrIosp2 outerInstance;

		internal Message protoMessage;
		internal Structure variable;

		public RootVariable(BufrIosp2 outerInstance, Message message, Structure variable)
		{
			this.outerInstance = outerInstance;
		  this.protoMessage = message;
		  this.variable = variable;
		}

		public virtual Message ProtoMessage
		{
			get
			{
			  return this.protoMessage;
			}
		}

		public virtual Structure Variable
		{
			get
			{
			  return this.variable;
			}
		}
	  }

	  private class SeqIter : StructureDataIterator
	  {
		  private readonly BufrIosp2 outerInstance;

		internal StructureDataIterator currIter;
		internal int recnum;
		// add its own prototype message and observation structure
		internal Message protoMessage;
		internal Structure obsStructure;

		internal SeqIter(BufrIosp2 outerInstance, Message message, Structure structure)
		{
			this.outerInstance = outerInstance;
		  this.protoMessage = message;
		  this.obsStructure = structure;
		  reset();
		}

		internal SeqIter(BufrIosp2 outerInstance, RootVariable rootVariable) : this(outerInstance, rootVariable.protoMessage, rootVariable.variable)
		{
			this.outerInstance = outerInstance;
		}

		public virtual StructureDataIterator reset()
		{
		  recnum = 0;
		  currIter = null;
		  outerInstance.scanner.reset();
		  return this;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public boolean hasNext() throws NetcdfSharp.java.io.IOException
		public virtual bool hasNext()
		{
		  if (currIter == null)
		  {
			currIter = readNextMessage();
			if (currIter == null)
			{
			  outerInstance.nelems = recnum;
			  return false;
			}
		  }

		  if (!currIter.hasNext())
		  {
			currIter = readNextMessage();
			return hasNext();
		  }

		  return true;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.ucar.ma2.StructureData next() throws NetcdfSharp.java.io.IOException
		public virtual StructureData next()
		{
		  recnum++;
		  return currIter.next();
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.ucar.ma2.StructureDataIterator readNextMessage() throws NetcdfSharp.java.io.IOException
		internal virtual StructureDataIterator readNextMessage()
		{
		  if (!outerInstance.scanner.hasNext())
		  {
			return null;
		  }
		  Message m = outerInstance.scanner.next();
		  if (m == null)
		  {
			log.warn("BUFR scanner hasNext() true but next() null!");
			return null;
		  }
		  if (m.containsBufrTable()) // data messages only
		  {
			return readNextMessage();
		  }

		  // mixed messages
		  if (!protoMessage.Equals(m))
		  {
			if (outerInstance.messHash == null)
			{
			  outerInstance.messHash = new HashSet<int>(20);
			}
			if (!outerInstance.messHash.Contains(m.GetHashCode()))
			{
			  log.warn("File " + outerInstance.raf.Location + " has different BUFR message types hash=" + protoMessage.GetHashCode() + "; skipping");
			  outerInstance.messHash.Add(m.GetHashCode());
			}
			return readNextMessage();
		  }

		  ArrayStructure @as = readMessage(m);
		  return @as.StructureDataIterator;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.ucar.ma2.ArrayStructure readMessage(Message m) throws NetcdfSharp.java.io.IOException
		internal virtual ArrayStructure readMessage(Message m)
		{
		  ArrayStructure @as;
		  if (m.dds.Compressed)
		  {
			MessageCompressedDataReader reader = new MessageCompressedDataReader();
			@as = reader.readEntireMessage(obsStructure, protoMessage, m, outerInstance.raf, null);
		  }
		  else
		  {
			MessageUncompressedDataReader reader = new MessageUncompressedDataReader();
			@as = reader.readEntireMessage(obsStructure, protoMessage, m, outerInstance.raf, null);
		  }
		  return @as;
		}

		public virtual int CurrentRecno
		{
			get
			{
			  return recnum - 1;
			}
		}

		public virtual void Dispose()
		{
		  if (currIter != null)
		  {
			currIter.Dispose();
		  }
		  currIter = null;
		  if (debugIter)
		  {
// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			Console.Write("BUFR read recnum {0:D}%n", recnum);
		  }
		}
	  }

	  private class SeqIterSingle : StructureDataIterator
	  {
		  private readonly BufrIosp2 outerInstance;


		internal StructureDataIterator currIter;
		internal int recnum;
		// add its own prototype message and observation structure
		internal Message protoMessage;
		internal Structure obsStructure;

		internal SeqIterSingle(BufrIosp2 outerInstance, Message message, Structure structure)
		{
			this.outerInstance = outerInstance;
		  protoMessage = message;
		  obsStructure = structure;
		  reset();
		}

		internal SeqIterSingle(BufrIosp2 outerInstance, RootVariable rootVariable) : this(outerInstance, rootVariable.protoMessage, rootVariable.variable)
		{
			this.outerInstance = outerInstance;
		}

		public virtual StructureDataIterator reset()
		{
		  recnum = 0;
		  currIter = null;
		  return this;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public boolean hasNext() throws NetcdfSharp.java.io.IOException
		public virtual bool hasNext()
		{
		  if (currIter == null)
		  {
			currIter = readProtoMessage();
			if (currIter == null)
			{
			  outerInstance.nelems = recnum;
			  return false;
			}
		  }

		  return currIter.hasNext();
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public NetcdfSharp.ucar.ma2.StructureData next() throws NetcdfSharp.java.io.IOException
		public virtual StructureData next()
		{
		  recnum++;
		  return currIter.next();
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.ucar.ma2.StructureDataIterator readProtoMessage() throws NetcdfSharp.java.io.IOException
		internal virtual StructureDataIterator readProtoMessage()
		{
		  Message m = protoMessage;
		  ArrayStructure @as;
		  if (m.dds.Compressed)
		  {
			MessageCompressedDataReader reader = new MessageCompressedDataReader();
			@as = reader.readEntireMessage(obsStructure, protoMessage, m, outerInstance.raf, null);
		  }
		  else
		  {
			MessageUncompressedDataReader reader = new MessageUncompressedDataReader();
			@as = reader.readEntireMessage(obsStructure, protoMessage, m, outerInstance.raf, null);
		  }

		  return @as.StructureDataIterator;
		}

		public virtual int CurrentRecno
		{
			get
			{
			  return recnum - 1;
			}
		}

		public virtual void Dispose()
		{
		  if (currIter != null)
		  {
			currIter.Dispose();
		  }
		  currIter = null;
		}
	  }

	  /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	  public override string DetailInfo
	  {
		  get
		  {
			NetcdfSharp.java.util.Formatter ff = new NetcdfSharp.java.util.Formatter();
			ff.format("{0}", base.DetailInfo);
			protoMessages[0].dump(ff);
			ff.format(Environment.NewLine);
			config.show(ff);
			return ff.ToString();
		  }
	  }

	  public override string FileTypeId
	  {
		  get
		  {
			return DataFormatType.BUFR.Description;
		  }
	  }

	  public override string FileTypeDescription
	  {
		  get
		  {
			return "WMO Binary Universal Form";
		  }
	  }

	}

}
