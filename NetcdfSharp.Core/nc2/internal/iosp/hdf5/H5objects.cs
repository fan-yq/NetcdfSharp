// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace NetcdfSharp.ucar.nc2.@internal.iosp.hdf5
{
  using BTree2 = NetcdfSharp.ucar.nc2.iosp.hdf5.BTree2;
  using DataType = NetcdfSharp.ucar.ma2.DataType;
  using Dimension = NetcdfSharp.ucar.nc2.Dimension;
  using Filters = NetcdfSharp.ucar.nc2.filter.Filters;
  using FractalHeap = NetcdfSharp.ucar.nc2.iosp.hdf5.FractalHeap;
  using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
  using MemTracker = NetcdfSharp.ucar.nc2.iosp.hdf5.MemTracker;
  using NcArray = NetcdfSharp.ucar.ma2.NcArray;
  using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

  /// <summary>
  /// The low-level HDF5 data objects. </summary>
  public class H5objects
  {
    private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(H5objects));

    // debugging
    // LOOK why are there so many debug settings here?
    private static bool debugEnum;
    private static bool debug1, debugDetail, debugPos, debugHeap;
    private static bool debugGroupBtree, debugDataBtree, debugBtree2;
    private static bool debugContinueMessage, debugTracker, debugSoftLink, debugHardLink, debugSymbolTable;
    private static bool warnings = true, debugReference, debugRegionReference, debugCreationOrder;

    private readonly H5headerNew header;

    private readonly PrintWriter debugOut;
    private readonly MemTracker memTracker;
    private readonly IDictionary<long, GlobalHeap> heapMap = new Dictionary<long, GlobalHeap>();

    internal H5objects(H5headerNew header, PrintWriter debugOut, MemTracker memTracker)
    {
      this.header = header;
      this.debugOut = debugOut;
      this.memTracker = memTracker;
    }

    internal virtual RandomAccessFile RandomAccessFile
    {
      get
      {
        return header.RandomAccessFile;
      }
    }

    // JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
    // ORIGINAL LINE: H5Group readRootSymbolTable(long pos) throws NetcdfSharp.java.io.IOException
    internal virtual H5Group readRootSymbolTable(long pos)
    {
      // The root object's SymbolTableEntry
      SymbolTableEntry rootEntry = new SymbolTableEntry(this, pos);

      // extract the root group object, recursively read all objects
      long rootObjectAddress = rootEntry.getObjectAddress();
      DataObjectFacade f = new DataObjectFacade(this, null, "", rootObjectAddress);
      return new H5Group(this, f);
    }

    // JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
    // ORIGINAL LINE: H5Group readRootObject(long rootObjectAddress) throws NetcdfSharp.java.io.IOException
    internal virtual H5Group readRootObject(long rootObjectAddress)
    {
      DataObjectFacade f = new DataObjectFacade(this, null, "", rootObjectAddress);
      return new H5Group(this, f);
    }

    /// <summary>
    /// A DataObjectFacade can be:
    /// 1) a DataObject with a specific group/name.
    /// 2) a SymbolicLink to a DataObject.
    /// DataObjects can be pointed to from multiple places.
    /// A DataObjectFacade is in a specific group and has a name specific to that group.
    /// A DataObject's name is one of its names.
    /// </summary>
    internal class DataObjectFacade
    {
      private readonly H5objects outerInstance;

      internal H5Group parent;
      internal string name, displayName;
      internal DataObject dobj;

      internal bool isGroup;
      internal bool isVariable;
      internal bool isTypedef;
      internal bool is2DCoordinate;
      internal bool hasNetcdfDimensions;

      // is a group
      internal H5Group group;

      // or a variable
      internal string dimList; // list of dimension names for this variable

      // or a link
      internal string linkName;

      internal DataObjectFacade(H5objects outerInstance, H5Group parent, string name, string linkName)
      {
        this.outerInstance = outerInstance;
        this.parent = parent;
        this.name = name;
        this.linkName = linkName;
      }

      // JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
      // ORIGINAL LINE: DataObjectFacade(H5Group parent, String name, long address) throws NetcdfSharp.java.io.IOException
      internal DataObjectFacade(H5objects outerInstance, H5Group parent, string name, long address)
      {
        this.outerInstance = outerInstance;
        this.parent = parent;
        this.name = name;
        displayName = (name.Length == 0) ? "root" : name;
        dobj = outerInstance.header.getDataObject(address, displayName);

        // hash for soft link lookup
        outerInstance.header.addSymlinkMap(Name, this); // LOOK does getName() match whats stored in soft link ??

        // if has a "group message", then its a group
        if ((dobj.groupMessage != null) || (dobj.groupNewMessage != null))
        { // if has a "groupNewMessage", then its a
          // groupNew
          isGroup = true;

          // if it has a Datatype and a StorageLayout, then its a Variable
        }
        else if ((dobj.mdt != null) && (dobj.msl != null))
        {
          isVariable = true;

          // if it has only a Datatype, its a Typedef
        }
        else if (dobj.mdt != null)
        {
          isTypedef = true;

        }
        else if (warnings)
        { // we dont know what it is
          log.warn("WARNING Unknown DataObjectFacade = {}", this);
          // return;
        }

      }

      internal virtual string Name
      {
        get
        {
          return (parent == null) ? name : parent.Name + "/" + name;
        }
      }

      public override string ToString()
      {
        StringBuilder sbuff = new StringBuilder();
        sbuff.Append(Name);
        if (dobj == null)
        {
          sbuff.Append(" dobj is NULL! ");
        }
        else
        {
          sbuff.Append(" id= ").Append(dobj.address);
          sbuff.Append(" messages= ");
          foreach (HeaderMessage message in dobj.messages)
          {
            sbuff.Append("\n  ").Append(message);
          }
        }

        return sbuff.ToString();
      }

    }

    // JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
    // ORIGINAL LINE: H5Group readH5Group(DataObjectFacade facade) throws NetcdfSharp.java.io.IOException
    internal virtual H5Group readH5Group(DataObjectFacade facade)
    {
      return new H5Group(this, facade);
    }

    internal class H5Group
    {
      private readonly H5objects outerInstance;

      internal H5Group parent;
      internal string name, displayName;
      internal DataObjectFacade facade;
      internal IList<DataObjectFacade> nestedObjects = new List<DataObjectFacade>(); // nested data objects
      internal IDictionary<string, Dimension> dimMap = new Dictionary<string, Dimension>();
      internal IList<Dimension> dimList = new List<Dimension>(); // need to track dimension order

      // "Data Object Header" Level 2A
      // read a Data Object Header
      // no side effects, can be called multiple time for debugging
      // JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
      // ORIGINAL LINE: private H5Group(DataObjectFacade facade) throws NetcdfSharp.java.io.IOException
      internal H5Group(H5objects outerInstance, DataObjectFacade facade)
      {
        this.outerInstance = outerInstance;
        this.facade = facade;
        this.parent = facade.parent;
        this.name = facade.name;
        displayName = (name.Length == 0) ? "root" : name;

        // if has a "group message", then its an "old group"
        if (facade.dobj.groupMessage != null)
        {
          // check for hard links
          if (debugHardLink)
          {
            log.debug("HO look for group address = {}", facade.dobj.groupMessage.btreeAddress);
          }
          if (outerInstance.hashGroups.TryGetValue(facade.dobj.groupMessage.btreeAddress, out facade.group))
          {
            if (debugHardLink)
            {
              log.debug("WARNING hard link to group = {}", facade.group.Name);
            }
            if (parent.isChildOf(facade.group))
            {
              if (debugHardLink)
              {
                log.debug("ERROR hard link to group create a loop = {}", facade.group.Name);
              }
              log.debug("Remove hard link to group that creates a loop = {}", facade.group.Name);
              facade.group = null;
              return;
            }
          }

          // read the group, and its contained data objects.
          outerInstance.readGroupOld(this, facade.dobj.groupMessage.btreeAddress, facade.dobj.groupMessage.nameHeapAddress);

        }
        else if (facade.dobj.groupNewMessage != null)
        { // if has a "groupNewMessage", then its a groupNew
          // read the group, and its contained data objects.
          outerInstance.readGroupNew(this, facade.dobj.groupNewMessage, facade.dobj);

        }
        else
        { // we dont know what it is
          throw new System.InvalidOperationException("H5Group needs group messages " + facade.Name);
        }

        facade.group = this;
      }

      internal virtual string Name
      {
        get
        {
          return (parent == null) ? name : parent.Name + "/" + name;
        }
      }

      // is this a child of that ?
      internal virtual bool isChildOf(H5Group that)
      {
        if (parent == null)
        {
          return false;
        }
        if (parent == that)
        {
          return true;
        }
        return parent.isChildOf(that);
      }

      public override string ToString()
      {
        return displayName;
      }
    }

    //////////////////////////////////////////////////////////////
    // HDF5 primitive objects

    //////////////////////////////////////////////////////////////
    // Level 2A "data object header"

    // JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
    // ORIGINAL LINE: DataObject readDataObject(long address, String who) throws NetcdfSharp.java.io.IOException
    internal virtual DataObject readDataObject(long address, string who)
    {
      return new DataObject(this, address, who);
    }

    public class DataObject : Named
    {
      private readonly H5objects outerInstance;

      // debugging
      public virtual long Address
      {
        get
        {
          return address;
        }
      }

      public virtual string Name
      {
        get
        {
          return who;
        }
      }

      public virtual IList<HeaderMessage> Messages
      {
        get
        {
          IList<HeaderMessage> result = new List<HeaderMessage>(100);
          foreach (HeaderMessage m in messages)
          {
            if (!(m.messData is MessageAttribute))
            {
              result.Add(m);
            }
          }
          return result;
        }
      }

      public virtual IList<MessageAttribute> Attributes
      {
        get
        {
          /*
           * List<MessageAttribute> result = new ArrayList<MessageAttribute>(100);
           * for (HeaderMessage m : messages)
           * if (m.messData instanceof MessageAttribute)
           * result.add((MessageAttribute)m.messData);
           * result.addAll(attributes);
           */
          return attributes;
        }
      }

      internal long address; // aka object id : obviously unique
      internal string who; // may be null, may not be unique
      internal IList<HeaderMessage> messages = new List<HeaderMessage>();
      internal IList<MessageAttribute> attributes = new List<MessageAttribute>();

      // need to look for these
      internal MessageGroup groupMessage;
      internal MessageGroupNew groupNewMessage;
      internal MessageDatatype mdt;
      internal MessageDataspace mds;
      internal MessageLayout msl;
      internal MessageFilter mfp;

      internal byte version; // 1 or 2

      public virtual void show(Formatter f)
      {
        if (mdt != null)
        {
          f.format("%s ", mdt.Type);
        }
        f.format("%s", Name);
        if (mds != null)
        {
          f.format("(");
          foreach (int len in mds.dimLength)
          {
            f.format("%d,", len);
          }
          f.format(");%n");
        }
        /*
         * for (MessageAttribute mess : getAttributes()) {
         * Attribute att = mess.getNcAttribute();
         * f.format("  :%s%n", att);
         * }
         */
        f.format("%n");
      }

      // "Data Object Header" Level 2A
      // read a Data Object Header
      // no side effects, can be called multiple time for debugging
      // JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
      // ORIGINAL LINE: private DataObject(long address, String who) throws NetcdfSharp.java.io.IOException
      internal DataObject(H5objects outerInstance, long address, string who)
      {
        this.outerInstance = outerInstance;
        this.address = address;
        this.who = who;

        if (debug1)
        {
          log.debug("\n--> DataObject.read parsing <" + who + "> object ID/address=" + address);
        }
        if (debugPos)
        {
          log.debug("      DataObject.read now at position=" + outerInstance.RandomAccessFile.FilePointer + " for <" + who + "> reposition to " + outerInstance.header.getFileOffset(address));
        }
        // if (offset < 0) return null;
        outerInstance.RandomAccessFile.seek(outerInstance.header.getFileOffset(address));

        version = outerInstance.RandomAccessFile.readByte();
        if (version == 1)
        { // Level 2A1 (first part, before the messages)
          outerInstance.RandomAccessFile.readByte(); // skip byte
          short nmess = outerInstance.RandomAccessFile.readShort();
          if (debugDetail)
          {
            log.debug(" version=" + version + " nmess=" + nmess);
          }

          int referenceCount = outerInstance.RandomAccessFile.readInt();
          int headerSize = outerInstance.RandomAccessFile.readInt();
          if (debugDetail)
          {
            log.debug(" referenceCount=" + referenceCount + " headerSize=" + headerSize);
          }

          // if (referenceCount > 1)
          // log.debug("WARNING referenceCount="+referenceCount);
          outerInstance.RandomAccessFile.skipBytes(4); // header messages multiples of 8

          long posMess = outerInstance.RandomAccessFile.FilePointer;
          int count = readMessagesVersion1(posMess, nmess, int.MaxValue, this.who);
          if (debugContinueMessage)
          {
            log.debug(" nmessages read = {}", count);
          }
          if (debugPos)
          {
            log.debug("<--done reading messages for <" + who + ">; position=" + outerInstance.RandomAccessFile.FilePointer);
          }
          if (debugTracker)
          {
            outerInstance.memTracker.addByLen("Object " + who, outerInstance.header.getFileOffset(address), headerSize + 16);
          }

        }
        else
        { // level 2A2 (first part, before the messages)
          // first byte was already read
          string magic = outerInstance.RandomAccessFile.readString(3);
          if (!magic.Equals("HDR"))
          {
            throw new System.InvalidOperationException("DataObject doesnt start with OHDR");
          }

          version = outerInstance.RandomAccessFile.readByte();
          byte flags = outerInstance.RandomAccessFile.readByte(); // data object header flags (version 2)
          if (debugDetail)
          {
            log.debug(" version=" + version + " flags=" + Convert.ToString(flags, 2));
          }

          // raf.skipBytes(2);
          if (((flags >> 5) & 1) == 1)
          {
            int accessTime = outerInstance.RandomAccessFile.readInt();
            int modTime = outerInstance.RandomAccessFile.readInt();
            int changeTime = outerInstance.RandomAccessFile.readInt();
            int birthTime = outerInstance.RandomAccessFile.readInt();
          }
          if (((flags >> 4) & 1) == 1)
          {
            short maxCompactAttributes = outerInstance.RandomAccessFile.readShort();
            short minDenseAttributes = outerInstance.RandomAccessFile.readShort();
          }

          long sizeOfChunk = outerInstance.readVariableSizeFactor(flags & 3);
          if (debugDetail)
          {
            log.debug(" sizeOfChunk=" + sizeOfChunk);
          }

          long posMess = outerInstance.RandomAccessFile.FilePointer;
          int count = readMessagesVersion2(posMess, sizeOfChunk, (flags & 4) != 0, this.who);
          if (debugContinueMessage)
          {
            log.debug(" nmessages read = {}", count);
          }
          if (debugPos)
          {
            log.debug("<--done reading messages for <" + who + ">; position=" + outerInstance.RandomAccessFile.FilePointer);
          }
        }

        // look for group or a datatype/dataspace/layout message
        foreach (HeaderMessage mess in messages)
        {
          if (debugTracker)
          {
            outerInstance.memTracker.addByLen("Message (" + who + ") " + mess.mtype, mess.start, mess.size + 8);
          }

          if (mess.mtype == MessageType.Group)
          {
            groupMessage = (MessageGroup)mess.messData;
          }
          else if (mess.mtype == MessageType.GroupNew)
          {
            groupNewMessage = (MessageGroupNew)mess.messData;
          }
          else if (mess.mtype == MessageType.SimpleDataspace)
          {
            mds = (MessageDataspace)mess.messData;
          }
          else if (mess.mtype == MessageType.Datatype)
          {
            mdt = (MessageDatatype)mess.messData;
          }
          else if (mess.mtype == MessageType.Layout)
          {
            msl = (MessageLayout)mess.messData;
          }
          else if (mess.mtype == MessageType.FilterPipeline)
          {
            mfp = (MessageFilter)mess.messData;
          }
          else if (mess.mtype == MessageType.Attribute)
          {
            attributes.Add((MessageAttribute)mess.messData);
          }
          else if (mess.mtype == MessageType.AttributeInfo)
          {
            processAttributeInfoMessage((MessageAttributeInfo)mess.messData, attributes);
          }
        }

        if (debug1)
        {
          log.debug("<-- end DataObject {}", who);
        }
      }

      private void processAttributeInfoMessage(MessageAttributeInfo attInfo, IList<MessageAttribute> list)
      {
        long btreeAddress = (attInfo.v2BtreeAddressCreationOrder > 0) ? attInfo.v2BtreeAddressCreationOrder : attInfo.v2BtreeAddress;
        if ((btreeAddress < 0) || (attInfo.fractalHeapAddress < 0))
        {
          return;
        }

        BTree2 btree = new BTree2(outerInstance.header, who, btreeAddress);
        FractalHeap fractalHeap = new FractalHeap(outerInstance.header, who, attInfo.fractalHeapAddress, outerInstance.memTracker);

        foreach (BTree2.Entry2 e in btree.entryList)
        {
          byte[] heapId;
          switch (btree.btreeType)
          {
            case 8:
              heapId = ((BTree2.Record8)e.record).HeapId;
              break;
            case 9:
              heapId = ((BTree2.Record9)e.record).HeapId;
              break;
            default:
              continue;
          }

          // the heapId points to an Attribute Message in the fractal Heap
          FractalHeap.DHeapId fractalHeapId = fractalHeap.getFractalHeapId(heapId);
          long pos = fractalHeapId.Pos;
          if (pos > 0)
          {
            MessageAttribute attMessage = new MessageAttribute(outerInstance);
            if (attMessage.read(pos))
            {
              list.Add(attMessage);
            }
            if (debugBtree2)
            {
              log.debug("    attMessage={}", attMessage);
            }
          }
        }
      }

      // read messages, starting at pos, until you hit maxMess read, or maxBytes read
      // if you hit a continuation message, call recursively
      // return number of messaages read
      private int readMessagesVersion1(long pos, int maxMess, int maxBytes, string objectName)
      {
        if (debugContinueMessage)
        {
          log.debug(" readMessages start at =" + pos + " maxMess= " + maxMess + " maxBytes= " + maxBytes);
        }

        int count = 0;
        int bytesRead = 0;
        while ((count < maxMess) && (bytesRead < maxBytes))
        {
          /*
           * LOOK: MessageContinue not correct ??
           * if (posMess >= actualSize)
           * break;
           */

          HeaderMessage mess = new HeaderMessage(outerInstance);
          // messages.add( mess);
          int n = mess.read(pos, 1, false, objectName);
          pos += n;
          bytesRead += n;
          count++;
          if (debugContinueMessage)
          {
            log.debug("   count=" + count + " bytesRead=" + bytesRead);
          }

          // if we hit a continuation, then we go into nested reading
          if (mess.mtype == MessageType.ObjectHeaderContinuation)
          {
            MessageContinue c = (MessageContinue)mess.messData;
            if (debugContinueMessage)
            {
              log.debug(" ---ObjectHeaderContinuation--- ");
            }
            count += readMessagesVersion1(outerInstance.header.getFileOffset(c.offset), maxMess - count, (int)c.length, objectName);
            if (debugContinueMessage)
            {
              log.debug(" ---ObjectHeaderContinuation return --- ");
            }
          }
          else if (mess.mtype != MessageType.NIL)
          {
            messages.Add(mess);
          }
        }
        return count;
      }

      private int readMessagesVersion2(long filePos, long maxBytes, bool creationOrderPresent, string objectName)
      {
        if (debugContinueMessage)
        {
          outerInstance.debugOut.println(" readMessages2 starts at =" + filePos + " maxBytes= " + maxBytes);
        }

        // maxBytes is number of bytes of messages to be read. however, a message is at least 4 bytes long, so
        // we are done if we have read > maxBytes - 4. There appears to be an "off by one" possibility
        maxBytes -= 3;

        int count = 0;
        int bytesRead = 0;
        while (bytesRead < maxBytes)
        {

          HeaderMessage mess = new HeaderMessage(outerInstance);
          // messages.add( mess);
          int n = mess.read(filePos, 2, creationOrderPresent, objectName);
          filePos += n;
          bytesRead += n;
          count++;
          if (debugContinueMessage)
          {
            outerInstance.debugOut.println("   mess size=" + n + " bytesRead=" + bytesRead + " maxBytes=" + maxBytes);
          }

          // if we hit a continuation, then we go into nested reading
          if (mess.mtype == MessageType.ObjectHeaderContinuation)
          {
            MessageContinue c = (MessageContinue)mess.messData;
            long continuationBlockFilePos = outerInstance.header.getFileOffset(c.offset);
            if (debugContinueMessage)
            {
              outerInstance.debugOut.println(" ---ObjectHeaderContinuation filePos= " + continuationBlockFilePos);
            }

            outerInstance.RandomAccessFile.seek(continuationBlockFilePos);
            string sig = outerInstance.readStringFixedLength(4);
            if (!sig.Equals("OCHK"))
            {
              throw new System.InvalidOperationException(" ObjectHeaderContinuation Missing signature");
            }

            count += readMessagesVersion2(continuationBlockFilePos + 4, (int)c.length - 8, creationOrderPresent, objectName);
            if (debugContinueMessage)
            {
              outerInstance.debugOut.println(" ---ObjectHeaderContinuation return --- ");
            }
            if (debugContinueMessage)
            {
              outerInstance.debugOut.println("   continuationMessages =" + count + " bytesRead=" + bytesRead + " maxBytes=" + maxBytes);
            }

          }
          else if (mess.mtype != MessageType.NIL)
          {
            messages.Add(mess);
          }
        }
        return count;
      }
    } // DataObject

    // type safe enum
    public class MessageType
    {
      private const int MAX_MESSAGE = 23;
      private static readonly IDictionary<string, MessageType> hash = new Dictionary<string, MessageType>(10);
      private static readonly MessageType[] mess = new MessageType[MAX_MESSAGE];

      public static readonly MessageType NIL = new MessageType("NIL", 0);
      public static readonly MessageType SimpleDataspace = new MessageType("SimpleDataspace", 1);
      public static readonly MessageType GroupNew = new MessageType("GroupNew", 2);
      public static readonly MessageType Datatype = new MessageType("Datatype", 3);
      public static readonly MessageType FillValueOld = new MessageType("FillValueOld", 4);
      public static readonly MessageType FillValue = new MessageType("FillValue", 5);
      public static readonly MessageType Link = new MessageType("Link", 6);
      public static readonly MessageType ExternalDataFiles = new MessageType("ExternalDataFiles", 7);
      public static readonly MessageType Layout = new MessageType("Layout", 8);
      public static readonly MessageType GroupInfo = new MessageType("GroupInfo", 10);
      public static readonly MessageType FilterPipeline = new MessageType("FilterPipeline", 11);
      public static readonly MessageType Attribute = new MessageType("Attribute", 12);
      public static readonly MessageType Comment = new MessageType("Comment", 13);
      public static readonly MessageType LastModifiedOld = new MessageType("LastModifiedOld", 14);
      public static readonly MessageType SharedObject = new MessageType("SharedObject", 15);
      public static readonly MessageType ObjectHeaderContinuation = new MessageType("ObjectHeaderContinuation", 16);
      public static readonly MessageType Group = new MessageType("Group", 17);
      public static readonly MessageType LastModified = new MessageType("LastModified", 18);
      public static readonly MessageType AttributeInfo = new MessageType("AttributeInfo", 21);
      public static readonly MessageType ObjectReferenceCount = new MessageType("ObjectReferenceCount", 22);

      private string name;
      private int num;

      private MessageType(string name, int num)
      {
        this.name = name;
        this.num = num;
        hash[name] = this;
        mess[num] = this;
      }

      /// <summary>
      /// Find the MessageType that matches this name.
      /// </summary>
      /// <param name="name"> find DataTYpe with this name. </param>
      /// <returns> DataType or null if no match. </returns>
      public static MessageType getType(string name)
      {
        if (string.ReferenceEquals(name, null))
        {
          return null;
        }
        return hash.TryGetValue(name, out var result) ? result : null;
      }

      /// <summary>
      /// Get the MessageType by number.
      /// </summary>
      /// <param name="num"> message number. </param>
      /// <returns> the MessageType </returns>
      public static MessageType getType(int num)
      {
        if ((num < 0) || (num >= MAX_MESSAGE))
        {
          return null;
        }
        return mess[num];
      }

      /// <summary>
      /// Message name.
      /// </summary>
      public override string ToString()
      {
        return name + "(" + num + ")";
      }

      /// <summary>
      /// @return Message number.
      /// </summary>
      public virtual int getNum()
      {
        return num;
      }

    }

    // Header Message: Level 2A1 and 2A2 (part of Data Object)
    public class HeaderMessage : IComparable<HeaderMessage>
    {
      internal H5objects outerInstance;

      internal HeaderMessage()
      {
      }

      internal HeaderMessage(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal long start;
      internal byte headerMessageFlags;
      internal int size;
      internal short type, header_length;
      internal Named messData; // header message data

      public virtual MessageType Mtype
      {
        get
        {
          return mtype;
        }
      }

      public virtual string Name
      {
        get
        {
          return messData.Name;
        }
      }

      public virtual int Size
      {
        get
        {
          return size;
        }
      }

      public virtual short Type
      {
        get
        {
          return type;
        }
      }

      public virtual byte Flags
      {
        get
        {
          return headerMessageFlags;
        }
      }

      public virtual long Start
      {
        get
        {
          return start;
        }
      }

      internal MessageType mtype;

      internal short creationOrder = -1;

      /// <summary>
      /// Read a message
      /// </summary>
      /// <param name="filePos"> at this filePos </param>
      /// <param name="version"> header version </param>
      /// <param name="creationOrderPresent"> true if bit2 of data object header flags is set </param>
      /// <returns> number of bytes read </returns>
      internal int read(long filePos, int version, bool creationOrderPresent, string objectName)
      {
        this.start = filePos;
        outerInstance.RandomAccessFile.seek(filePos);
        if (debugPos)
        {
          log.debug("  --> Message Header starts at =" + outerInstance.RandomAccessFile.FilePointer);
        }

        if (version == 1)
        {
          type = outerInstance.RandomAccessFile.readShort();
          size = DataTypes.unsignedShortToInt(outerInstance.RandomAccessFile.readShort());
          headerMessageFlags = outerInstance.RandomAccessFile.readByte();
          outerInstance.RandomAccessFile.skipBytes(3);
          header_length = 8;

        }
        else
        {
          type = outerInstance.RandomAccessFile.readByte();
          size = DataTypes.unsignedShortToInt(outerInstance.RandomAccessFile.readShort());
          // if (size > Short.MAX_VALUE)
          // log.debug("HEY");

          headerMessageFlags = outerInstance.RandomAccessFile.readByte();
          header_length = 4;
          if (creationOrderPresent)
          {
            creationOrder = outerInstance.RandomAccessFile.readShort();
            header_length += 2;
          }
        }
        mtype = MessageType.getType(type);
        if (debug1)
        {
          log.debug("  -->" + mtype + " messageSize=" + size + " flags = " + Convert.ToString(headerMessageFlags, 2));
          if (creationOrderPresent && debugCreationOrder)
          {
            log.debug("     creationOrder = " + creationOrder);
          }
        }
        if (debugPos)
        {
          log.debug("  --> Message Data starts at=" + outerInstance.RandomAccessFile.FilePointer);
        }

        if ((headerMessageFlags & 2) != 0)
        { // shared
          messData = outerInstance.getSharedDataObject(mtype).mdt; // eg a shared datatype, eg enums
          return header_length + size;
        }

        if (mtype == MessageType.NIL)
        { // 0
          // dont do nuttin

        }
        else if (mtype == MessageType.SimpleDataspace)
        { // 1
          MessageDataspace data = new MessageDataspace(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.GroupNew)
        { // 2
          MessageGroupNew data = new MessageGroupNew(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.Datatype)
        { // 3
          MessageDatatype data = new MessageDatatype(outerInstance);
          data.read(objectName);
          messData = data;

        }
        else if (mtype == MessageType.FillValueOld)
        { // 4
          MessageFillValueOld data = new MessageFillValueOld(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.FillValue)
        { // 5
          MessageFillValue data = new MessageFillValue(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.Link)
        { // 6
          MessageLink data = new MessageLink(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.Layout)
        { // 8
          MessageLayout data = new MessageLayout(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.GroupInfo)
        { // 10
          MessageGroupInfo data = new MessageGroupInfo(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.FilterPipeline)
        { // 11
          MessageFilter data = new MessageFilter(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.Attribute)
        { // 12
          MessageAttribute data = new MessageAttribute(outerInstance);
          data.read(outerInstance.RandomAccessFile.FilePointer);
          messData = data;

        }
        else if (mtype == MessageType.Comment)
        { // 13
          MessageComment data = new MessageComment(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.LastModifiedOld)
        { // 14
          MessageLastModifiedOld data = new MessageLastModifiedOld(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.ObjectHeaderContinuation)
        { // 16
          MessageContinue data = new MessageContinue(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.Group)
        { // 17
          MessageGroup data = new MessageGroup(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.LastModified)
        { // 18
          MessageLastModified data = new MessageLastModified(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.AttributeInfo)
        { // 21
          MessageAttributeInfo data = new MessageAttributeInfo(outerInstance);
          data.read();
          messData = data;

        }
        else if (mtype == MessageType.ObjectReferenceCount)
        { // 21
          MessageObjectReferenceCount data = new MessageObjectReferenceCount(outerInstance);
          data.read();
          messData = data;

        }
        else
        {
          log.debug("****UNPROCESSED MESSAGE type = " + mtype + " raw = " + type);
          log.warn("SKIP UNPROCESSED MESSAGE type = " + mtype + " raw = " + type);
          // throw new UnsupportedOperationException("****UNPROCESSED MESSAGE type = " + mtype + " raw = " + type);
        }

        return header_length + size;
      }

      public virtual int CompareTo(HeaderMessage o)
      {
        return type.CompareTo(o.type);
      }

      public override string ToString()
      {
        return "message type = " + mtype + "; " + messData;
      }

      // debugging
      public virtual void showFractalHeap(Formatter f)
      {
        if (mtype != MessageType.AttributeInfo)
        {
          f.format("No fractal heap");
          return;
        }

        MessageAttributeInfo info = (MessageAttributeInfo)messData;
        info.showFractalHeap(f);
      }

      // debugging
      public virtual void showCompression(Formatter f)
      {
        if (mtype != MessageType.AttributeInfo)
        {
          f.format("No fractal heap");
          return;
        }

        MessageAttributeInfo info = (MessageAttributeInfo)messData;
        info.showFractalHeap(f);
      }

    }

    private DataObject getSharedDataObject(MessageType mtype)
    {
      byte sharedVersion = RandomAccessFile.readByte();
      byte sharedType = RandomAccessFile.readByte();
      if (sharedVersion == 1)
      {
        RandomAccessFile.skipBytes(6);
      }
      if ((sharedVersion == 3) && (sharedType == 1))
      {
        long heapId = RandomAccessFile.readLong();
        if (debug1)
        {
          log.debug("     Shared Message " + sharedVersion + " type=" + sharedType + " heapId = " + heapId);
        }
        if (debugPos)
        {
          log.debug("  --> Shared Message reposition to =" + RandomAccessFile.FilePointer);
        }
        // dunno where is the file's shared object header heap ??
        throw new System.NotSupportedException("****SHARED MESSAGE type = " + mtype + " heapId = " + heapId);

      }
      else
      {
        long address = header.readOffset();
        if (debug1)
        {
          log.debug("     Shared Message " + sharedVersion + " type=" + sharedType + " address = " + address);
        }
        DataObject dobj = header.getDataObject(address, null);
        if (null == dobj)
        {
          throw new System.InvalidOperationException("cant find data object at" + address);
        }
        if (mtype == MessageType.Datatype)
        {
          return dobj;
        }
        throw new System.NotSupportedException("****SHARED MESSAGE type = " + mtype);
      }
    }

    internal interface Named
    {
      string Name { get; }
    }

    // Message Type 1 : "Simple Dataspace" = dimension list / shape
    public class MessageDataspace : Named
    {
      internal H5objects outerInstance;

      internal MessageDataspace()
      {
      }

      internal MessageDataspace(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal byte ndims, flags;
      internal byte type; // 0 A scalar dataspace, i.e. a dataspace with a single, dimensionless element.
                          // 1 A simple dataspace, i.e. a dataspace with a a rank > 0 and an appropriate # of dimensions.
                          // 2 A null dataspace, i.e. a dataspace with no elements.
      internal int[] dimLength, maxLength; // , permute;
                                           // boolean isPermuted;

      public virtual string Name
      {
        get
        {
          StringBuilder sbuff = new StringBuilder();
          sbuff.Append("(");
          foreach (int size in dimLength)
          {
            sbuff.Append(size).Append(",");
          }
          sbuff.Append(")");
          return sbuff.ToString();
        }
      }

      public override string ToString()
      {
        Formatter sbuff = new Formatter();
        sbuff.format(" ndims=%d flags=%x type=%d ", ndims, flags, type);
        if (dimLength != null)
        {
          sbuff.format(" length=(");
          foreach (int size in dimLength)
          {
            sbuff.format("%d,", size);
          }
          sbuff.format(") ");
        }
        if (maxLength != null)
        {
          sbuff.format("max=(");
          foreach (int aMaxLength in maxLength)
          {
            sbuff.format("%d,", aMaxLength);
          }
          sbuff.format(")");
        }
        return sbuff.ToString();
      }

      internal void read()
      {
        if (debugPos)
        {
          log.debug("   *MessageSimpleDataspace start pos= " + outerInstance.RandomAccessFile.FilePointer);
        }

        byte version = outerInstance.RandomAccessFile.readByte();
        if (version == 1)
        {
          ndims = outerInstance.RandomAccessFile.readByte();
          flags = outerInstance.RandomAccessFile.readByte();
          type = (byte)((ndims == 0) ? 0 : 1);
          outerInstance.RandomAccessFile.skipBytes(5); // skip 5 bytes

        }
        else if (version == 2)
        {
          ndims = outerInstance.RandomAccessFile.readByte();
          flags = outerInstance.RandomAccessFile.readByte();
          type = outerInstance.RandomAccessFile.readByte();

        }
        else
        {
          throw new System.InvalidOperationException("MessageDataspace: unknown version= " + version);
        }

        if (debug1)
        {
          log.debug("   SimpleDataspace version= " + version + " flags=" + Convert.ToString(flags, 2) + " ndims=" + ndims + " type=" + type);
        }

        /*
         * if (ndims == 0 && !alreadyWarnNdimZero) {
         * log.warn("ndims == 0 in HDF5 file= " + raf.getLocation());
         * alreadyWarnNdimZero = true;
         * }
         */

        dimLength = new int[ndims];
        for (int i = 0; i < ndims; i++)
        {
          dimLength[i] = (int)outerInstance.header.readLength();
        }

        bool hasMax = (flags & 0x01) != 0;
        maxLength = new int[ndims];
        if (hasMax)
        {
          for (int i = 0; i < ndims; i++)
          {
            maxLength[i] = (int)outerInstance.header.readLength();
          }
        }
        else
        {
          System.Array.Copy(dimLength, 0, maxLength, 0, ndims);
        }

        if (debug1)
        {
          for (int i = 0; i < ndims; i++)
          {
            log.debug("    dim length = " + dimLength[i] + " max = " + maxLength[i]);
          }
        }
      }
    }

    // Message Type 17/0x11 "Old Group" or "Symbol Table"
    internal class MessageGroup : Named
    {
      internal H5objects outerInstance;

      internal MessageGroup()
      {
      }

      internal MessageGroup(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal long btreeAddress, nameHeapAddress;

      internal void read()
      {
        btreeAddress = outerInstance.header.readOffset();
        nameHeapAddress = outerInstance.header.readOffset();
        if (debug1)
        {
          log.debug("   Group btreeAddress=" + btreeAddress + " nameHeapAddress=" + nameHeapAddress);
        }
      }

      public override string ToString()
      {
        return " btreeAddress=" + btreeAddress + " nameHeapAddress=" + nameHeapAddress;
      }

      public virtual string Name
      {
        get
        {
          return btreeAddress.ToString();
        }
      }

    }

    // Message Type 2 "New Group" or "Link Info" (version 2)
    internal class MessageGroupNew : Named
    {
      internal H5objects outerInstance;

      internal MessageGroupNew()
      {
      }

      internal MessageGroupNew(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal long maxCreationIndex = -2, fractalHeapAddress, v2BtreeAddress, v2BtreeAddressCreationOrder = -2;

      public override string ToString()
      {
        Formatter f = new Formatter();
        f.format("   GroupNew fractalHeapAddress=%d v2BtreeAddress=%d ", fractalHeapAddress, v2BtreeAddress);
        if (v2BtreeAddressCreationOrder > -2)
        {
          f.format(" v2BtreeAddressCreationOrder=%d ", v2BtreeAddressCreationOrder);
        }
        if (maxCreationIndex > -2)
        {
          f.format(" maxCreationIndex=%d", maxCreationIndex);
        }
        f.format(" %n%n");

        if (fractalHeapAddress > 0)
        {
          try
          {
            f.format("%n%n");
            FractalHeap fractalHeap = new FractalHeap(outerInstance.header, "", fractalHeapAddress, outerInstance.memTracker);
            fractalHeap.showDetails(f);
          }
          catch (System.IO.IOException e)
          {
            Console.WriteLine(e.ToString());
          }
        }

        return f.ToString();
      }

      internal void read()
      {
        if (debugPos)
        {
          log.debug("   *MessageGroupNew start pos= " + outerInstance.RandomAccessFile.FilePointer);
        }
        byte version = outerInstance.RandomAccessFile.readByte();
        byte flags = outerInstance.RandomAccessFile.readByte();
        if ((flags & 1) != 0)
        {
          maxCreationIndex = outerInstance.RandomAccessFile.readLong();
        }

        fractalHeapAddress = outerInstance.header.readOffset();
        v2BtreeAddress = outerInstance.header.readOffset(); // aka name index

        if ((flags & 2) != 0)
        {
          v2BtreeAddressCreationOrder = outerInstance.header.readOffset();
        }

        if (debug1)
        {
          log.debug("   MessageGroupNew version= " + version + " flags = " + flags + this);
        }
      }

      public virtual string Name
      {
        get
        {
          return fractalHeapAddress.ToString();
        }
      }

    }

    // Message Type 10/0xA "Group Info" (version 2)
    internal class MessageGroupInfo : Named
    {
      internal H5objects outerInstance;

      internal MessageGroupInfo()
      {
      }

      internal MessageGroupInfo(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal byte flags;
      internal short maxCompactValue = -1, minDenseValue = -1, estNumEntries = -1, estLengthEntryName = -1;

      public override string ToString()
      {
        StringBuilder sbuff = new StringBuilder();
        sbuff.Append("   MessageGroupInfo ");
        if ((flags & 1) != 0)
        {
          sbuff.Append(" maxCompactValue=").Append(maxCompactValue).Append(" minDenseValue=").Append(minDenseValue);
        }
        if ((flags & 2) != 0)
        {
          sbuff.Append(" estNumEntries=").Append(estNumEntries).Append(" estLengthEntryName=").Append(estLengthEntryName);
        }
        return sbuff.ToString();
      }

      internal void read()
      {
        if (debugPos)
        {
          log.debug("   *MessageGroupInfo start pos= " + outerInstance.RandomAccessFile.FilePointer);
        }
        byte version = outerInstance.RandomAccessFile.readByte();
        flags = outerInstance.RandomAccessFile.readByte();

        if ((flags & 1) != 0)
        {
          maxCompactValue = outerInstance.RandomAccessFile.readShort();
          minDenseValue = outerInstance.RandomAccessFile.readShort();
        }

        if ((flags & 2) != 0)
        {
          estNumEntries = outerInstance.RandomAccessFile.readShort();
          estLengthEntryName = outerInstance.RandomAccessFile.readShort();
        }

        if (debug1)
        {
          log.debug("   MessageGroupInfo version= " + version + " flags = " + flags + this);
        }
      }

      public virtual string Name
      {
        get
        {
          return "";
        }
      }
    }

    // Message Type 6 "Link" (version 2)
    internal class MessageLink : Named
    {
      internal H5objects outerInstance;

      internal MessageLink()
      {
      }

      internal MessageLink(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal byte version, flags, encoding;
      internal byte linkType; // 0=hard, 1=soft, 64 = external
      internal long creationOrder;
      internal string linkName, link;
      internal long linkAddress;

      public override string ToString()
      {
        StringBuilder sbuff = new StringBuilder();
        sbuff.Append("   MessageLink ");
        sbuff.Append(" name=").Append(linkName).Append(" type=").Append(linkType);
        if (linkType == 0)
        {
          sbuff.Append(" linkAddress=" + linkAddress);
        }
        else
        {
          sbuff.Append(" link=").Append(link);
        }

        if ((flags & 4) != 0)
        {
          sbuff.Append(" creationOrder=" + creationOrder);
        }
        if ((flags & 0x10) != 0)
        {
          sbuff.Append(" encoding=" + encoding);
        }
        return sbuff.ToString();
      }

      internal void read()
      {
        if (debugPos)
        {
          log.debug("   *MessageLink start pos= {}", outerInstance.RandomAccessFile.FilePointer);
        }
        version = outerInstance.RandomAccessFile.readByte();
        flags = outerInstance.RandomAccessFile.readByte();

        if ((flags & 8) != 0)
        {
          linkType = outerInstance.RandomAccessFile.readByte();
        }

        if ((flags & 4) != 0)
        {
          creationOrder = outerInstance.RandomAccessFile.readLong();
        }

        if ((flags & 0x10) != 0)
        {
          encoding = outerInstance.RandomAccessFile.readByte();
        }

        int linkNameLength = (int)outerInstance.readVariableSizeFactor(flags & 3);
        linkName = outerInstance.readStringFixedLength(linkNameLength);

        if (linkType == 0)
        {
          linkAddress = outerInstance.header.readOffset();

        }
        else if (linkType == 1)
        {
          short len = outerInstance.RandomAccessFile.readShort();
          link = outerInstance.readStringFixedLength(len);

        }
        else if (linkType == 64)
        {
          short len = outerInstance.RandomAccessFile.readShort();
          link = outerInstance.readStringFixedLength(len); // actually 2 strings - see docs
        }

        if (debug1)
        {
          log.debug("   MessageLink version= " + version + " flags = " + Convert.ToString(flags, 2) + this);
        }
      }

      public virtual string Name
      {
        get
        {
          return linkName;
        }
      }
    }

    // Message Type 3 : "Datatype"
    public class MessageDatatype : Named
    {
      internal H5objects outerInstance;

      internal MessageDatatype()
      {
      }

      internal MessageDatatype(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal int type, version;
      internal byte[] flags = new byte[3];
      internal int byteSize;
      internal int endian; // 0 (LE) or 1 (BE) == RandomAccessFile.XXXXXX_ENDIAN
      internal bool isOK = true;
      internal bool unsigned;

      // time (2)
      internal DataType timeType;

      // opaque (5)
      internal string opaque_desc;

      // compound type (6)
      internal IList<StructureMember> members;

      // reference (7)
      internal int referenceType; // 0 = object, 1 = region

      // enums (8)
      internal IDictionary<int, string> map;
      internal string enumTypeName;

      // enum, variable-length, array types have "base" DataType
      internal MessageDatatype @base;
      internal bool isVString; // variable length (not a string)
      internal bool isVlen; // vlen but not string

      // array (10)
      internal int[] dim;

      public override string ToString()
      {
        Formatter f = new Formatter();
        f.format(" datatype= %d", type);
        f.format(" byteSize= %d", byteSize);
        DataType? dtype = outerInstance.header.getNCtype(type, byteSize, unsigned);
        f.format(" NCtype= %s %s", dtype, unsigned ? "(unsigned)" : "");
        f.format(" flags= ");
        for (int i = 0; i < 3; i++)
        {
          f.format(" %d", flags[i]);
        }
        f.format(" endian= %s", endian == RandomAccessFile.BIG_ENDIAN ? "BIG" : "LITTLE");

        if (type == 2)
        {
          f.format(" timeType= %s", timeType);
        }
        else if (type == 6)
        {
          f.format("%n  members%n");
          foreach (StructureMember mm in members)
          {
            f.format("   %s%n", mm);
          }
        }
        else if (type == 7)
        {
          f.format(" referenceType= %s", referenceType);
        }
        else if (type == 8)
        {
          f.format(" enumTypeName= %s", enumTypeName);
        }
        else if (type == 9)
        {
          f.format(" isVString= %s", isVString);
          f.format(" isVlen= %s", isVlen);
        }
        if ((type == 8) || (type == 9) || (type == 10))
        {
          f.format(" parent base= {%s}", @base);
        }
        return f.ToString();
      }

      public virtual string Name
      {
        get
        {
          DataType? dtype = outerInstance.header.getNCtype(type, byteSize, unsigned);
          if (dtype != null)
          {
            return dtype + " size= " + byteSize;
          }
          else
          {
            return "type=" + type + " size= " + byteSize;
          }
        }
      }

      public virtual string Type
      {
        get
        {
          DataType? dtype = outerInstance.header.getNCtype(type, byteSize, unsigned);
          if (dtype != null)
          {
            return dtype.ToString();
          }
          else
          {
            return "type=" + type + " size= " + byteSize;
          }
        }
      }

      internal void read(string objectName)
      {
        if (debugPos)
        {
          log.debug("   *MessageDatatype start pos= {}", outerInstance.RandomAccessFile.FilePointer);
        }

        byte tandv = outerInstance.RandomAccessFile.readByte();
        type = (tandv & 0xf);
        version = ((tandv & 0xf0) >> 4);

        outerInstance.RandomAccessFile.readFully(flags);
        byteSize = outerInstance.RandomAccessFile.readInt();
        endian = ((flags[0] & 1) == 0) ? RandomAccessFile.LITTLE_ENDIAN : RandomAccessFile.BIG_ENDIAN;

        if (debug1)
        {
          log.debug("   Datatype type=" + type + " version= " + version + " flags = " + flags[0] + " " + flags[1] + " " + flags[2] + " byteSize=" + byteSize + " byteOrder=" + (endian == RandomAccessFile.BIG_ENDIAN ? "BIG" : "LITTLE"));
        }

        if (type == 0)
        { // fixed point
          unsigned = ((flags[0] & 8) == 0);
          short bitOffset = outerInstance.RandomAccessFile.readShort();
          short bitPrecision = outerInstance.RandomAccessFile.readShort();
          if (debug1)
          {
            log.debug("   type 0 (fixed point): bitOffset= " + bitOffset + " bitPrecision= " + bitPrecision + " unsigned= " + unsigned);
          }
          isOK = (bitOffset == 0) && (bitPrecision % 8 == 0);

        }
        else if (type == 1)
        { // floating point
          short bitOffset = outerInstance.RandomAccessFile.readShort();
          short bitPrecision = outerInstance.RandomAccessFile.readShort();
          byte expLocation = outerInstance.RandomAccessFile.readByte();
          byte expSize = outerInstance.RandomAccessFile.readByte();
          byte manLocation = outerInstance.RandomAccessFile.readByte();
          byte manSize = outerInstance.RandomAccessFile.readByte();
          int expBias = outerInstance.RandomAccessFile.readInt();
          if (debug1)
          {
            log.debug("   type 1 (floating point): bitOffset= " + bitOffset + " bitPrecision= " + bitPrecision + " expLocation= " + expLocation + " expSize= " + expSize + " manLocation= " + manLocation + " manSize= " + manSize + " expBias= " + expBias);
          }
        }
        else if (type == 2)
        { // time
          short bitPrecision = outerInstance.RandomAccessFile.readShort();
          if (bitPrecision == 16)
          {
            timeType = DataType.SHORT;
          }
          else if (bitPrecision == 32)
          {
            timeType = DataType.INT;
          }
          else if (bitPrecision == 64)
          {
            timeType = DataType.LONG;
          }

          if (debug1)
          {
            log.debug("   type 2 (time): bitPrecision= " + bitPrecision + " timeType = " + timeType);
          }

        }
        else if (type == 3)
        { // string (I think a fixed length seq of chars)
          int ptype = flags[0] & 0xf;
          if (debug1)
          {
            log.debug("   type 3 (String): pad type= " + ptype);
          }

        }
        else if (type == 4)
        { // bit field
          short bitOffset = outerInstance.RandomAccessFile.readShort();
          short bitPrecision = outerInstance.RandomAccessFile.readShort();
          if (debug1)
          {
            log.debug("   type 4 (bit field): bitOffset= " + bitOffset + " bitPrecision= " + bitPrecision);
          }
          // isOK = (bitOffset == 0) && (bitPrecision % 8 == 0); LOOK

        }
        else if (type == 5)
        { // opaque
          byte len = flags[0];
          opaque_desc = (len > 0) ? outerInstance.readString(outerInstance.RandomAccessFile).Trim() : null;
          if (debug1)
          {
            log.debug("   type 5 (opaque): len= " + len + " desc= " + opaque_desc);
          }

        }
        else if (type == 6)
        { // compound
          int nmembers = H5objects.makeUnsignedIntFromBytes(flags[1], flags[0]);
          if (debug1)
          {
            log.debug("   --type 6(compound): nmembers={}", nmembers);
          }
          members = new List<StructureMember>();
          for (int i = 0; i < nmembers; i++)
          {
            members.Add(new StructureMember(outerInstance, version, byteSize));
          }
          if (debugDetail)
          {
            log.debug("   --done with compound type");
          }

        }
        else if (type == 7)
        { // reference
          referenceType = flags[0] & 0xf;
          if (debug1 || debugReference)
          {
            log.debug("   --type 7(reference): type= {}", referenceType);
          }

        }
        else if (type == 8)
        { // enums
          int nmembers = H5objects.makeUnsignedIntFromBytes(flags[1], flags[0]);
          bool saveDebugDetail = debugDetail;
          if (debug1 || debugEnum)
          {
            log.debug("   --type 8(enums): nmembers={}", nmembers);
            debugDetail = true;
          }
          @base = new MessageDatatype(outerInstance); // base type
          @base.read(objectName);
          debugDetail = saveDebugDetail;

          // read the enum names
          string[] enumName = new string[nmembers];
          for (int i = 0; i < nmembers; i++)
          {
            if (version < 3)
            {
              enumName[i] = outerInstance.readString8(outerInstance.RandomAccessFile); // padding
            }
            else
            {
              enumName[i] = outerInstance.readString(outerInstance.RandomAccessFile); // no padding
            }
          }

          // read the enum values; must switch to base byte order (!)
          if (@base.endian >= 0)
          {
            outerInstance.RandomAccessFile.order(@base.endian);
          }
          int[] enumValue = new int[nmembers];
          for (int i = 0; i < nmembers; i++)
          {
            enumValue[i] = (int)outerInstance.header.readVariableSizeUnsigned(@base.byteSize); // assume size is 1, 2, or 4
          }
          outerInstance.RandomAccessFile.order(RandomAccessFile.LITTLE_ENDIAN);

          enumTypeName = objectName;
          map = new SortedDictionary<int, string>();
          for (int i = 0; i < nmembers; i++)
          {
            map[enumValue[i]] = enumName[i];
          }
          if (debugEnum)
          {
            for (int i = 0; i < nmembers; i++)
            {
              log.debug("   " + enumValue[i] + "=" + enumName[i]);
            }
          }

        }
        else if (type == 9)
        { // String (A variable-length sequence of characters) or Sequence (A variable-length sequence of any datatype)
          isVString = (flags[0] & 0xf) == 1;
          if (!isVString)
          {
            isVlen = true;
          }
          if (debug1)
          {
            log.debug("   type 9(variable length): type= {}", ((isVString ? "string" : "sequence of type:")));
          }
          @base = new MessageDatatype(outerInstance); // base type
          @base.read(objectName);

        }
        else if (type == 10)
        { // array
          if (debug1)
          {
            outerInstance.debugOut.Write("   type 10(array) lengths= ");
          }
          int ndims = outerInstance.RandomAccessFile.readByte();
          if (version < 3)
          {
            outerInstance.RandomAccessFile.skipBytes(3);
          }

          dim = new int[ndims];
          for (int i = 0; i < ndims; i++)
          {
            dim[i] = outerInstance.RandomAccessFile.readInt();
            if (debug1)
            {
              outerInstance.debugOut.Write(" " + dim[i]);
            }
          }

          if (version < 3)
          { // not present in version 3, never used anyway
            int[] pdim = new int[ndims];
            for (int i = 0; i < ndims; i++)
            {
              pdim[i] = outerInstance.RandomAccessFile.readInt();
            }
          }
          if (debug1)
          {
            log.debug("");
          }

          @base = new MessageDatatype(outerInstance); // base type
          @base.read(objectName);

        }
        else if (warnings)
        {
          log.warn(" WARNING not dealing with type= {}", type);
        }
      }

      internal int getBaseType()
      {
        return (@base != null) ? @base.getBaseType() : type;
      }

      internal int getBaseSize()
      {
        return (@base != null) ? @base.getBaseSize() : byteSize;
      }

      internal byte[] getFlags()
      {
        return (@base != null) ? @base.getFlags() : flags;
      }

      internal bool isVlenType()
      {
        return (type == 10 ? @base.isVlenType() : isVlen);
      }

      internal bool isVStringType()
      {
        return (type == 10 ? @base.isVStringType() : isVString);
      }
    }

    internal class StructureMember
    {
      internal H5objects outerInstance;

      internal StructureMember(H5objects outerInstance, int version, int byteSize)
      {
        this.outerInstance = outerInstance;
        if (debugPos)
        {
          log.debug("   *StructureMember now at position={}", outerInstance.RandomAccessFile.FilePointer);
        }

        name = outerInstance.readString(outerInstance.RandomAccessFile);
        if (version < 3)
        {
          outerInstance.RandomAccessFile.skipBytes(padding(name.Length + 1, 8));
          offset = outerInstance.RandomAccessFile.readInt();
        }
        else
        {
          offset = (int)outerInstance.readVariableSizeMax(byteSize);
        }

        if (debug1)
        {
          log.debug("   Member name=" + name + " offset= " + offset);
        }

        if (version == 1)
        {
          dims = outerInstance.RandomAccessFile.readByte();
          outerInstance.RandomAccessFile.skipBytes(3);
          outerInstance.RandomAccessFile.skipBytes(24); // ignore dimension info for now
        }

        // HDFdumpWithCount(buffer, raf.getFilePointer(), 16);
        mdt = new MessageDatatype(outerInstance);
        mdt.read(name);
        if (debugDetail)
        {
          log.debug("   ***End Member name={}", name);
        }

        // ??
        // HDFdump(ncfile.out, "Member end", buffer, 16);
        // if (HDFdebug) ncfile.log.debug(" Member pos="+raf.getFilePointer());
        // HDFpadToMultiple( buffer, 8);
        // if (HDFdebug) ncfile.log.debug(" Member padToMultiple="+raf.getFilePointer());
        // raf.skipBytes( 4); // huh ??
      }

      internal string name;
      internal int offset;
      internal byte dims;
      internal MessageDatatype mdt;

      public override string ToString()
      {
        return "StructureMember" + "{name='" + name + '\'' + ", offset=" + offset + ", dims=" + dims + ", mdt=" + mdt + '}';
      }
    }

    // Message Type 4 "Fill Value Old" : fill value is stored in the message
    internal class MessageFillValueOld : Named
    {
      internal H5objects outerInstance;

      internal MessageFillValueOld()
      {
      }

      internal MessageFillValueOld(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal byte[] value;
      internal int size;

      internal void read()
      {
        size = outerInstance.RandomAccessFile.readInt();
        value = new byte[size];
        outerInstance.RandomAccessFile.readFully(value);

        if (debug1)
        {
          log.debug("{}", this);
        }
      }

      public override string ToString()
      {
        StringBuilder sbuff = new StringBuilder();
        sbuff.Append("   FillValueOld size= ").Append(size).Append(" value=");
        for (int i = 0; i < size; i++)
        {
          sbuff.Append(" ").Append(value[i]);
        }
        return sbuff.ToString();
      }

      public virtual string Name
      {
        get
        {
          StringBuilder sbuff = new StringBuilder();
          for (int i = 0; i < size; i++)
          {
            sbuff.Append(" ").Append(value[i]);
          }
          return sbuff.ToString();
        }
      }
    }

    // Message Type 5 "Fill Value New" : fill value is stored in the message, with extra metadata
    internal class MessageFillValue : Named
    {
      internal H5objects outerInstance;

      internal MessageFillValue()
      {
      }

      internal MessageFillValue(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal byte version; // 1,2,3
      internal byte spaceAllocateTime; // 1= early, 2=late, 3=incremental
      internal byte fillWriteTime;
      internal int size;
      internal byte[] value;
      internal bool hasFillValue;

      internal byte flags;

      internal void read()
      {
        version = outerInstance.RandomAccessFile.readByte();

        if (version < 3)
        {
          spaceAllocateTime = outerInstance.RandomAccessFile.readByte();
          fillWriteTime = outerInstance.RandomAccessFile.readByte();
          hasFillValue = outerInstance.RandomAccessFile.readByte() != 0;

        }
        else
        {
          flags = outerInstance.RandomAccessFile.readByte();
          spaceAllocateTime = (byte)(flags & 3);
          fillWriteTime = (byte)((flags >> 2) & 3);
          hasFillValue = (flags & 32) != 0;
        }

        if (hasFillValue)
        {
          size = outerInstance.RandomAccessFile.readInt();
          if (size > 0)
          {
            value = new byte[size];
            outerInstance.RandomAccessFile.readFully(value);
            hasFillValue = true;
          }
          else
          {
            hasFillValue = false;
          }
        }

        if (debug1)
        {
          log.debug("{}", this);
        }
      }

      public override string ToString()
      {
        StringBuilder sbuff = new StringBuilder();
        sbuff.Append("   FillValue version= ").Append(version).Append(" spaceAllocateTime = ").Append(spaceAllocateTime).Append(" fillWriteTime=").Append(fillWriteTime).Append(" hasFillValue= ").Append(hasFillValue);
        sbuff.Append("\n size = ").Append(size).Append(" value=");
        for (int i = 0; i < size; i++)
        {
          sbuff.Append(" ").Append(value[i]);
        }
        return sbuff.ToString();
      }

      public virtual string Name
      {
        get
        {
          StringBuilder sbuff = new StringBuilder();
          for (int i = 0; i < size; i++)
          {
            sbuff.Append(" ").Append(value[i]);
          }
          return sbuff.ToString();
        }
      }

    }

    // Message Type 8 "Data Storage Layout" : regular (contiguous), chunked, or compact (stored with the message)
    internal class MessageLayout : Named
    {
      internal H5objects outerInstance;

      internal MessageLayout()
      {
      }

      internal MessageLayout(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal byte type; // 0 = Compact, 1 = Contiguous, 2 = Chunked
      internal long dataAddress = -1; // -1 means "not allocated"
      internal long contiguousSize; // size of data allocated contiguous
      internal int[] chunkSize; // only for chunked, otherwise must use Dataspace
      internal int dataSize;

      public override string ToString()
      {
        StringBuilder sbuff = new StringBuilder();
        sbuff.Append(" type= ").Append((int)type).Append(" (");
        switch (type)
        {
          case 0:
            sbuff.Append("compact");
            break;
          case 1:
            sbuff.Append("contiguous");
            break;
          case 2:
            sbuff.Append("chunked");
            break;
          default:
            sbuff.Append("unknown type= ").Append(type);
        break;
        }
        sbuff.Append(")");

        if (chunkSize != null)
        {
          sbuff.Append(" storageSize = (");
          for (int i = 0; i < chunkSize.Length; i++)
          {
            if (i > 0)
            {
              sbuff.Append(",");
            }
            sbuff.Append(chunkSize[i]);
          }
          sbuff.Append(")");
        }

        sbuff.Append(" dataSize=").Append(dataSize);
        sbuff.Append(" dataAddress=").Append(dataAddress);
        return sbuff.ToString();
      }

      public virtual string Name
      {
        get
        {
          StringBuilder sbuff = new StringBuilder();
          switch (type)
          {
            case 0:
              sbuff.Append("compact");
              break;
            case 1:
              sbuff.Append("contiguous");
              break;
            case 2:
              sbuff.Append("chunked");
              break;
            default:
              sbuff.Append("unknown type= ").Append(type);
          break;
          }

          if (chunkSize != null)
          {
            sbuff.Append(" chunk = (");
            for (int i = 0; i < chunkSize.Length; i++)
            {
              if (i > 0)
              {
                sbuff.Append(",");
              }
              sbuff.Append(chunkSize[i]);
            }
            sbuff.Append(")");
          }

          return sbuff.ToString();
        }
      }

      internal void read()
      {
        int ndims;

        byte version = outerInstance.RandomAccessFile.readByte();
        if (version < 3)
        {
          ndims = outerInstance.RandomAccessFile.readByte();
          type = outerInstance.RandomAccessFile.readByte();
          outerInstance.RandomAccessFile.skipBytes(5); // skip 5 bytes

          bool isCompact = (type == 0);
          if (!isCompact)
          {
            dataAddress = outerInstance.header.readOffset();
          }
          chunkSize = new int[ndims];
          for (int i = 0; i < ndims; i++)
          {
            chunkSize[i] = outerInstance.RandomAccessFile.readInt();
          }

          if (isCompact)
          {
            dataSize = outerInstance.RandomAccessFile.readInt();
            dataAddress = outerInstance.RandomAccessFile.FilePointer;
          }

        }
        else
        {
          type = outerInstance.RandomAccessFile.readByte();

          if (type == 0)
          {
            dataSize = outerInstance.RandomAccessFile.readShort();
            dataAddress = outerInstance.RandomAccessFile.FilePointer;

          }
          else if (type == 1)
          {
            dataAddress = outerInstance.header.readOffset();
            contiguousSize = outerInstance.header.readLength();

          }
          else if (type == 2)
          {
            ndims = outerInstance.RandomAccessFile.readByte();
            dataAddress = outerInstance.header.readOffset();
            chunkSize = new int[ndims];
            for (int i = 0; i < ndims; i++)
            {
              chunkSize[i] = outerInstance.RandomAccessFile.readInt();
            }
          }
        }

        if (debug1)
        {
          log.debug("   StorageLayout version= " + version + this);
        }
      }
    }

    // Message Type 11/0xB "Filter Pipeline" : apply a filter to the "data stream"
    internal class MessageFilter : Named
    {
      internal H5objects outerInstance;

      internal MessageFilter()
      {
      }

      internal MessageFilter(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal Filter[] filters;

      internal void read()
      {

        byte version = outerInstance.RandomAccessFile.readByte();
        byte nfilters = outerInstance.RandomAccessFile.readByte();
        // version 1 has 6 bytes reserved
        if (version == 1)
        {
          outerInstance.RandomAccessFile.skipBytes(6);
        }

        // create filters list
        filters = new Filter[nfilters];
        for (int i = 0; i < nfilters; i++)
        {
          // create properties map
          IDictionary<string, object> props = new Dictionary<string, object>();
          // read filter description
          short id = outerInstance.RandomAccessFile.readShort();
          props[NetcdfSharp.ucar.nc2.filter.Filters.Keys.ID] = id;
          // if the filter id < 256 then this field is not stored
          short nameSize = ((version > 1) && (id < 256)) ? (short)0 : outerInstance.RandomAccessFile.readShort();
          short flags = outerInstance.RandomAccessFile.readShort();
          props[NetcdfSharp.ucar.nc2.filter.Filters.Keys.OPTIONAL] = (flags != 0); // save as boolean
          short nValues = outerInstance.RandomAccessFile.readShort();

          // get filter name, if present
          string name = null;
          if (nameSize > 0)
          {
            // version 1 name is padded to multiple of 8
            name = version == 1 ? outerInstance.readString8(outerInstance.RandomAccessFile) : outerInstance.readStringFixedLength(nameSize);
          }
          props[NetcdfSharp.ucar.nc2.filter.Filters.Keys.NAME] = name;

          // read data
          int[] data = new int[nValues];
          for (int n = 0; n < nValues; n++)
          {
            data[n] = outerInstance.RandomAccessFile.readInt();
          }
          props[NetcdfSharp.ucar.nc2.filter.Filters.Keys.DATA] = data;

          // add padding if nValues is odd
          if ((version == 1) && (nValues & 1) != 0)
          {
            outerInstance.RandomAccessFile.skipBytes(4);
          }

          // add to filters list
          filters[i] = new Filter(props);
        }

        if (debug1)
        {
          log.debug("   MessageFilter version=" + version + this);
        }
      }

      public virtual Filter[] Filters
      {
        get
        {
          return filters;
        }
      }

      public override string ToString()
      {
        StringBuilder sbuff = new StringBuilder();
        sbuff.Append("   MessageFilter filters=\n");
        foreach (Filter f in filters)
        {
          sbuff.Append(" ").Append(f).Append("\n");
        }
        return sbuff.ToString();
      }

      public virtual string Name
      {
        get
        {
          StringBuilder sbuff = new StringBuilder();
          foreach (Filter f in filters)
          {
            sbuff.Append(f.getName()).Append(", ");
          }
          return sbuff.ToString();
        }
      }
    }

    public class Filter
    {
      internal IDictionary<string, object> properties;

      internal Filter(IDictionary<string, object> props)
      {
        this.properties = props;
      }

      internal string getName()
      {
        return this.properties[NetcdfSharp.ucar.nc2.filter.Filters.Keys.NAME].ToString();
      }

      internal int getId()
      {
        return ((short)this.properties[NetcdfSharp.ucar.nc2.filter.Filters.Keys.ID]);
      }

      internal virtual IDictionary<string, object> Properties
      {
        get
        {
          return this.properties;
        }
      }
    }

    // Message Type 12/0xC "Attribute" : define an Attribute
    public class MessageAttribute : Named
    {
      internal H5objects outerInstance;

      internal MessageAttribute()
      {
        mdt = new MessageDatatype(outerInstance);
        mds = new MessageDataspace(outerInstance);
      }

      internal MessageAttribute(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
        mdt = new MessageDatatype(outerInstance);
        mds = new MessageDataspace(outerInstance);
      }

      internal byte version;
      // short typeSize, spaceSize;
      internal string name;
      internal MessageDatatype mdt;
      internal MessageDataspace mds;

      public virtual byte Version
      {
        get
        {
          return version;
        }
      }

      public virtual MessageDatatype Mdt
      {
        get
        {
          return mdt;
        }
      }

      public virtual MessageDataspace Mds
      {
        get
        {
          return mds;
        }
      }

      public virtual long DataPosAbsolute
      {
        get
        {
          return dataPos;
        }
      }

      internal long dataPos; // pointer to the attribute data section, must be absolute file position

      public override string ToString()
      {
        StringBuilder sbuff = new StringBuilder();
        sbuff.Append("   Name= ").Append(name);
        sbuff.Append(" dataPos = ").Append(dataPos);
        if (mdt != null)
        {
          sbuff.Append("\n mdt=");
          sbuff.Append(mdt);
        }
        if (mds != null)
        {
          sbuff.Append("\n mds=");
          sbuff.Append(mds);
        }
        return sbuff.ToString();
      }

      public virtual string Name
      {
        get
        {
          return name;
        }
      }

      internal bool read(long pos)
      {
        outerInstance.RandomAccessFile.seek(pos);
        if (debugPos)
        {
          log.debug("   *MessageAttribute start pos= {}", outerInstance.RandomAccessFile.FilePointer);
        }
        short nameSize, typeSize, spaceSize;
        byte flags = 0;
        byte encoding = 0; // 0 = ascii, 1 = UTF-8

        version = outerInstance.RandomAccessFile.readByte();
        if (version == 1)
        {
          outerInstance.RandomAccessFile.read(); // skip byte
          nameSize = outerInstance.RandomAccessFile.readShort();
          typeSize = outerInstance.RandomAccessFile.readShort();
          spaceSize = outerInstance.RandomAccessFile.readShort();

        }
        else if ((version == 2) || (version == 3))
        {
          flags = outerInstance.RandomAccessFile.readByte();
          nameSize = outerInstance.RandomAccessFile.readShort();
          typeSize = outerInstance.RandomAccessFile.readShort();
          spaceSize = outerInstance.RandomAccessFile.readShort();
          if (version == 3)
          {
            encoding = outerInstance.RandomAccessFile.readByte();
          }

        }
        else if (version == 72)
        {
          flags = outerInstance.RandomAccessFile.readByte();
          nameSize = outerInstance.RandomAccessFile.readShort();
          typeSize = outerInstance.RandomAccessFile.readShort();
          spaceSize = outerInstance.RandomAccessFile.readShort();
          log.error("HDF5 MessageAttribute found bad version " + version + " at filePos " + outerInstance.RandomAccessFile.FilePointer);
          // G:/work/galibert/IMOS_ANMN-NSW_AETVZ_20131127T230000Z_PH100_FV01_PH100-1311-Workhorse-ADCP-109.5_END-20140306T010000Z_C-20140521T053527Z.nc
          // E:/work/antonio/2014_ch.nc
          // return false;
        }
        else
        {
          // buggery, may be HDF5 "more than 8 attributes" error
          log.error("bad version " + version + " at filePos " + outerInstance.RandomAccessFile.FilePointer);
          return false;
          // throw new IllegalStateException("MessageAttribute unknown version " + version);
        }

        // read the attribute name
        long filePos = outerInstance.RandomAccessFile.FilePointer;
        name = outerInstance.readString(outerInstance.RandomAccessFile); // read at current pos
        if (version == 1)
        {
          nameSize += (short)padding(nameSize, 8);
        }
        outerInstance.RandomAccessFile.seek(filePos + nameSize); // make it more robust for errors

        if (debug1)
        {
          log.debug("   MessageAttribute version= " + version + " flags = " + Convert.ToString(flags, 2) + " nameSize = " + nameSize + " typeSize=" + typeSize + " spaceSize= " + spaceSize + " name= " + name);
        }

        // read the datatype
        filePos = outerInstance.RandomAccessFile.FilePointer;
        if (debugPos)
        {
          log.debug("   *MessageAttribute before mdt pos= {}", filePos);
        }
        bool isShared = (flags & 1) != 0;
        if (isShared)
        {
          mdt = outerInstance.getSharedDataObject(MessageType.Datatype).mdt;
          if (debug1)
          {
            log.debug("    MessageDatatype: {}", mdt);
          }
        }
        else
        {
          mdt.read(name);
          if (version == 1)
          {
            typeSize += (short)padding(typeSize, 8);
          }
        }
        outerInstance.RandomAccessFile.seek(filePos + typeSize); // make it more robust for errors

        // read the dataspace
        filePos = outerInstance.RandomAccessFile.FilePointer;
        if (debugPos)
        {
          log.debug("   *MessageAttribute before mds = {}", filePos);
        }
        mds.read();
        if (version == 1)
        {
          spaceSize += (short)padding(spaceSize, 8);
        }
        outerInstance.RandomAccessFile.seek(filePos + spaceSize); // make it more robust for errors

        // the data starts immediately afterward - ie in the message
        dataPos = outerInstance.RandomAccessFile.FilePointer; // note this is absolute position (no offset needed)
        if (debug1)
        {
          log.debug("   *MessageAttribute dataPos= {}", dataPos);
        }
        return true;
      }
    } // MessageAttribute

    // Message Type 21/0x15 "Attribute Info" (version 2)
    internal class MessageAttributeInfo : Named
    {
      internal H5objects outerInstance;

      internal MessageAttributeInfo()
      {
      }

      internal MessageAttributeInfo(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal byte flags;
      internal short maxCreationIndex = -1;
      internal long fractalHeapAddress = -2, v2BtreeAddress = -2, v2BtreeAddressCreationOrder = -2;

      public virtual string Name
      {
        get
        {
          long btreeAddress = (v2BtreeAddressCreationOrder > 0) ? v2BtreeAddressCreationOrder : v2BtreeAddress;
          return btreeAddress.ToString();
        }
      }

      public override string ToString()
      {
        Formatter f = new Formatter();
        f.format("   MessageAttributeInfo ");
        if ((flags & 1) != 0)
        {
          f.format(" maxCreationIndex=" + maxCreationIndex);
        }
        f.format(" fractalHeapAddress=%d v2BtreeAddress=%d", fractalHeapAddress, v2BtreeAddress);
        if ((flags & 2) != 0)
        {
          f.format(" v2BtreeAddressCreationOrder=%d", v2BtreeAddressCreationOrder);
        }

        showFractalHeap(f);

        return f.ToString();
      }

      internal void showFractalHeap(Formatter f)
      {
        long btreeAddress = (v2BtreeAddressCreationOrder > 0) ? v2BtreeAddressCreationOrder : v2BtreeAddress;
        if ((fractalHeapAddress > 0) && (btreeAddress > 0))
        {
          try
          {
            FractalHeap fractalHeap = new FractalHeap(outerInstance.header, "", fractalHeapAddress, outerInstance.memTracker);
            fractalHeap.showDetails(f);

            f.format(" Btree:%n");
            f.format("  type n m  offset size pos       attName%n");

            BTree2 btree = new BTree2(outerInstance.header, "", btreeAddress);
            foreach (BTree2.Entry2 e in btree.entryList)
            {
              byte[] heapId;
              switch (btree.btreeType)
              {
                case 8:
                  heapId = ((BTree2.Record8)e.record).HeapId;
                  break;
                case 9:
                  heapId = ((BTree2.Record9)e.record).HeapId;
                  break;
                default:
                  f.format(" unknown btreetype %d%n", btree.btreeType);
                  continue;
              }

              // the heapId points to an Attribute Message in the fractal Heap
              FractalHeap.DHeapId dh = fractalHeap.getFractalHeapId(heapId);
              dh.show(f);
              if (dh.Pos > 0)
              {
                MessageAttribute attMessage = new MessageAttribute(outerInstance);
                attMessage.read(dh.Pos);
                f.format(" %-30s", trunc(attMessage.Name, 30));
              }
              f.format(" heapId=:%s%n", "[" + string.Join(", ", heapId) + "]");
            }

          }
          catch (System.IO.IOException e)
          {
            Console.WriteLine(e.ToString());
          }
        }
      }

      internal string trunc(string s, int max)
      {
        if (string.ReferenceEquals(s, null))
        {
          return null;
        }
        if (s.Length < max)
        {
          return s;
        }
        return s.Substring(0, max);
      }

      internal void read()
      {
        if (debugPos)
        {
          log.debug("   *MessageAttributeInfo start pos= {}", outerInstance.RandomAccessFile.FilePointer);
        }
        byte version = outerInstance.RandomAccessFile.readByte();
        byte flags = outerInstance.RandomAccessFile.readByte();
        if ((flags & 1) != 0)
        {
          maxCreationIndex = outerInstance.RandomAccessFile.readShort();
        }

        fractalHeapAddress = outerInstance.header.readOffset();
        v2BtreeAddress = outerInstance.header.readOffset();

        if ((flags & 2) != 0)
        {
          v2BtreeAddressCreationOrder = outerInstance.header.readOffset();
        }

        if (debug1)
        {
          log.debug("   MessageAttributeInfo version= " + version + " flags = " + flags + this);
        }
      }
    }

    // Message Type 13/0xD ("Object Comment" : "short description of an Object"
    internal class MessageComment : Named
    {
      internal H5objects outerInstance;

      internal MessageComment()
      {
      }

      internal MessageComment(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal string comment;

      internal void read()
      {
        comment = outerInstance.readString(outerInstance.RandomAccessFile);
      }

      public override string ToString()
      {
        return comment;
      }

      public virtual string Name
      {
        get
        {
          return comment;
        }
      }

    }

    // Message Type 18/0x12 "Last Modified" : last modified date represented as secs since 1970
    internal class MessageLastModified : Named
    {
      internal H5objects outerInstance;

      internal MessageLastModified()
      {
      }

      internal MessageLastModified(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal byte version;
      internal int secs;

      internal void read()
      {
        version = outerInstance.RandomAccessFile.readByte();
        outerInstance.RandomAccessFile.skipBytes(3); // skip byte
        secs = outerInstance.RandomAccessFile.readInt();
      }

      public override string ToString()
      {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(secs).ToString();
      }

      public virtual string Name
      {
        get
        {
          return ToString();
        }
      }
    }

    // Message Type 14/0xE ("Last Modified (old)" : last modified date represented as a String YYMM etc. use message type
    // 18 instead
    internal class MessageLastModifiedOld : Named
    {
      internal H5objects outerInstance;

      internal MessageLastModifiedOld()
      {
      }

      internal MessageLastModifiedOld(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal string datemod;

      internal void read()
      {
        datemod = outerInstance.RandomAccessFile.readString(14);
        if (debug1)
        {
          log.debug("   MessageLastModifiedOld={}", datemod);
        }
      }

      public override string ToString()
      {
        return datemod;
      }

      public virtual string Name
      {
        get
        {
          return ToString();
        }
      }
    }

    // Message Type 16/0x10 "Continue" : point to more messages
    internal class MessageContinue : Named
    {
      internal H5objects outerInstance;

      internal MessageContinue()
      {
      }

      internal MessageContinue(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal long offset, length;

      internal void read()
      {
        offset = outerInstance.header.readOffset();
        length = outerInstance.header.readLength();
        if (debug1)
        {
          log.debug("   Continue offset=" + offset + " length=" + length);
        }
      }

      public virtual string Name
      {
        get
        {
          return "";
        }
      }
    }

    // Message Type 22/0x11 Object Reference COunt
    internal class MessageObjectReferenceCount : Named
    {
      internal H5objects outerInstance;

      internal MessageObjectReferenceCount()
      {
      }

      internal MessageObjectReferenceCount(H5objects outerInstance)
      {
        this.outerInstance = outerInstance;
      }

      internal int refCount;

      internal void read()
      {
        int version = outerInstance.RandomAccessFile.readByte();
        refCount = outerInstance.RandomAccessFile.readInt();
        if (debug1)
        {
          log.debug("   ObjectReferenceCount={}", refCount);
        }
      }

      public virtual string Name
      {
        get
        {
          return refCount.ToString();
        }
      }
    }

    internal virtual void readGroupNew(H5Group group, MessageGroupNew groupNewMessage, DataObject dobj)
    {
      if (debug1)
      {
        log.debug("\n--> GroupNew read <{}>", group.displayName);
      }

      if (groupNewMessage.fractalHeapAddress >= 0)
      {
        FractalHeap fractalHeap = new FractalHeap(header, group.displayName, groupNewMessage.fractalHeapAddress, memTracker);

        long btreeAddress = (groupNewMessage.v2BtreeAddressCreationOrder >= 0) ? groupNewMessage.v2BtreeAddressCreationOrder : groupNewMessage.v2BtreeAddress;
        if (btreeAddress < 0)
        {
          throw new System.InvalidOperationException("no valid btree for GroupNew with Fractal Heap");
        }

        // read in btree and all entries
        BTree2 btree = new BTree2(header, group.displayName, btreeAddress);
        foreach (BTree2.Entry2 e in btree.entryList)
        {
          byte[] heapId;
          switch (btree.btreeType)
          {
            case 5:
              heapId = ((BTree2.Record5)e.record).HeapId;
              break;
            case 6:
              heapId = ((BTree2.Record6)e.record).HeapId;
              break;
            default:
              continue;
          }

          // the heapId points to a Link message in the Fractal Heap
          FractalHeap.DHeapId fractalHeapId = fractalHeap.getFractalHeapId(heapId);
          long pos = fractalHeapId.Pos;
          if (pos < 0)
          {
            continue;
          }
          RandomAccessFile.seek(pos);
          MessageLink linkMessage = new MessageLink(this);
          linkMessage.read();
          if (debugBtree2)
          {
            log.debug("    linkMessage={}", linkMessage);
          }

          group.nestedObjects.Add(new DataObjectFacade(this, group, linkMessage.linkName, linkMessage.linkAddress));
        }

      }
      else
      {
        // look for link messages
        foreach (HeaderMessage mess in dobj.messages)
        {
          if (mess.mtype == MessageType.Link)
          {
            MessageLink linkMessage = (MessageLink)mess.messData;
            if (linkMessage.linkType == 0)
            { // hard link
              group.nestedObjects.Add(new DataObjectFacade(this, group, linkMessage.linkName, linkMessage.linkAddress));
            }
          }
        }
      }

      if (debug1)
      {
        log.debug("<-- end GroupNew read <" + group.displayName + ">");
      }
    }

    internal IDictionary<long, H5Group> hashGroups = new Dictionary<long, H5Group>();

    internal virtual void readGroupOld(H5Group group, long btreeAddress, long nameHeapAddress)
    {
      // track by address for hard links
      hashGroups[btreeAddress] = group;

      if (debug1)
      {
        log.debug("\n--> GroupOld read <" + group.displayName + ">");
      }
      LocalHeap nameHeap = new LocalHeap(this, group, nameHeapAddress);
      GroupBTree btree = new GroupBTree(this, group.displayName, btreeAddress);

      // now read all the entries in the btree : Level 1C
      foreach (SymbolTableEntry s in btree.getSymbolTableEntries())
      {
        string sname = nameHeap.getString((int)s.getNameOffset());
        if (debugSoftLink)
        {
          log.debug("\n   Symbol name={}", sname);
        }
        if (s.cacheType == 2)
        {
          string linkName = nameHeap.getString(s.linkOffset);
          if (debugSoftLink)
          {
            log.debug("   Symbolic link name=" + linkName + " symbolName=" + sname);
          }
          group.nestedObjects.Add(new DataObjectFacade(this, group, sname, linkName));
        }
        else
        {
          group.nestedObjects.Add(new DataObjectFacade(this, group, sname, s.getObjectAddress()));
        }
      }
      if (debug1)
      {
        log.debug("<-- end GroupOld read <" + group.displayName + ">");
      }
    }

    // Level 1A
    // this just reads in all the entries into a list
    internal class GroupBTree
    {
      internal H5objects h5;
      protected internal string owner;
      protected internal int wantType;
      private IList<SymbolTableEntry> sentries = new List<SymbolTableEntry>(); // list of type SymbolTableEntry

      internal GroupBTree(H5objects h5, string owner, long address)
      {
        this.h5 = h5;
        this.owner = owner;

        IList<GroupBTree.Entry> entryList = new List<GroupBTree.Entry>();
        readAllEntries(address, entryList);

        // now convert the entries to SymbolTableEntry
        foreach (GroupBTree.Entry e in entryList)
        {
          GroupBTree.GroupNode node = new GroupBTree.GroupNode(h5, owner, e.address);
          sentries.AddRange(node.Symbols);
        }
      }

      internal virtual IList<SymbolTableEntry> getSymbolTableEntries()
      {
        return sentries;
      }

      // recursively read all entries, place them in order in list
      internal virtual void readAllEntries(long address, IList<GroupBTree.Entry> entryList)
      {
        h5.RandomAccessFile.seek(h5.header.getFileOffset(address));
        if (debugGroupBtree)
        {
          log.debug("\n--> GroupBTree read tree at position={}", h5.RandomAccessFile.FilePointer);
        }

        string magic = h5.RandomAccessFile.readString(4);
        if (!magic.Equals("TREE"))
        {
          throw new System.InvalidOperationException("BtreeGroup doesnt start with TREE");
        }

        int type = h5.RandomAccessFile.readByte();
        int level = h5.RandomAccessFile.readByte();
        int nentries = h5.RandomAccessFile.readShort();
        if (debugGroupBtree)
        {
          log.debug("    type=" + type + " level=" + level + " nentries=" + nentries);
        }
        if (type != wantType)
        {
          throw new System.InvalidOperationException("BtreeGroup must be type " + wantType);
        }

        long size = 8 + 2 * h5.header.sizeOffsets + nentries * (h5.header.sizeOffsets + h5.header.sizeLengths);
        if (debugTracker)
        {
          h5.memTracker.addByLen("Group BTree (" + owner + ")", address, size);
        }

        long leftAddress = h5.header.readOffset();
        long rightAddress = h5.header.readOffset();
        if (debugGroupBtree)
        {
          log.debug("    leftAddress=" + leftAddress + " " + leftAddress.ToString("x") + " rightAddress=" + rightAddress + " " + rightAddress.ToString("x"));
        }

        // read all entries in this Btree "Node"
        IList<GroupBTree.Entry> myEntries = new List<GroupBTree.Entry>();
        for (int i = 0; i < nentries; i++)
        {
          myEntries.Add(new GroupBTree.Entry(h5));
        }

        if (level == 0)
        {
          foreach (GroupBTree.Entry entry in myEntries)
          {
            entryList.Add(entry);
          }
        }
        else
        {
          foreach (GroupBTree.Entry entry in myEntries)
          {
            if (debugDataBtree)
            {
              log.debug("  nonzero node entry at =" + entry.address);
            }
            readAllEntries(entry.address, entryList);
          }
        }

      }

      // these are part of the level 1A data structure, type = 0
      internal class Entry
      {
        internal H5objects h5;

        internal long key, address;

        internal Entry(H5objects h5)
        {
          this.h5 = h5;
          this.key = h5.header.readLength();
          this.address = h5.header.readOffset();
          if (debugGroupBtree)
          {
            log.debug("     GroupEntry key={} address={}", key, address);
          }
        }
      }

      // level 1B
      internal class GroupNode
      {
        internal H5objects h5;
        internal string owner;

        internal long address;
        internal byte version;
        internal short nentries;
        internal IList<SymbolTableEntry> symbols = new List<SymbolTableEntry>(); // SymbolTableEntry

        internal GroupNode(H5objects h5, string owner, long address)
        {
          this.h5 = h5;
          this.owner = owner;
          this.address = address;

          h5.RandomAccessFile.seek(h5.header.getFileOffset(address));
          if (debugDetail)
          {
            log.debug("--Group Node position={}", h5.RandomAccessFile.FilePointer);
          }

          // header
          string magic = h5.RandomAccessFile.readString(4);
          if (!magic.Equals("SNOD"))
          {
            throw new System.InvalidOperationException(magic + " should equal SNOD");
          }

          version = h5.RandomAccessFile.readByte();
          h5.RandomAccessFile.readByte(); // skip byte
          nentries = h5.RandomAccessFile.readShort();
          if (debugDetail)
          {
            log.debug("   version={} nentries={}", version, nentries);
          }

          long posEntry = h5.RandomAccessFile.FilePointer;
          for (int i = 0; i < nentries; i++)
          {
            SymbolTableEntry entry = new SymbolTableEntry(h5, posEntry);
            posEntry += entry.getSize();
            if (entry.objectHeaderAddress != 0)
            { // LOOK: Probably a bug in HDF5 file format ?? jc July 16 2010
              if (debug1)
              {
                log.debug("   add {}", entry);
              }
              symbols.Add(entry);
            }
            else
            {
              if (debug1)
              {
                log.debug("   BAD objectHeaderAddress==0 !! {}", entry);
              }
            }
          }
          if (debugDetail)
          {
            log.debug("-- Group Node end position={}", h5.RandomAccessFile.FilePointer);
          }
          long size = 8 + nentries * 40;
          if (debugTracker)
          {
            h5.memTracker.addByLen("Group BtreeNode (" + owner + ")", address, size);
          }
        }

        internal virtual IList<SymbolTableEntry> Symbols
        {
          get
          {
            return symbols;
          }
        }
      }


    } // GroupBTree

    // aka Group Entry "level 1C"
    internal class SymbolTableEntry
    {
      internal H5objects outerInstance;

      internal long nameOffset, objectHeaderAddress;
      internal long btreeAddress, nameHeapAddress;
      internal int cacheType, linkOffset;
      internal long posData;

      internal bool isSymbolicLink;

      internal SymbolTableEntry(H5objects outerInstance, long filePos)
      {
        this.outerInstance = outerInstance;
        outerInstance.RandomAccessFile.seek(filePos);
        if (debugSymbolTable)
        {
          log.debug("--> readSymbolTableEntry position={}", outerInstance.RandomAccessFile.FilePointer);
        }

        nameOffset = outerInstance.header.readOffset();
        objectHeaderAddress = outerInstance.header.readOffset();
        cacheType = outerInstance.RandomAccessFile.readInt();
        outerInstance.RandomAccessFile.skipBytes(4);

        if (debugSymbolTable)
        {
          log.debug(" nameOffset={} objectHeaderAddress={} cacheType={}", nameOffset, objectHeaderAddress, cacheType);
        }

        // "scratch pad"
        posData = outerInstance.RandomAccessFile.FilePointer;
        if (debugSymbolTable)
        {
          outerInstance.dump("Group Entry scratch pad", posData, 16, false);
        }

        if (cacheType == 1)
        {
          btreeAddress = outerInstance.header.readOffset();
          nameHeapAddress = outerInstance.header.readOffset();
          if (debugSymbolTable)
          {
            log.debug("btreeAddress={} nameHeadAddress={}", btreeAddress, nameHeapAddress);
          }
        }

        // check for symbolic link
        if (cacheType == 2)
        {
          linkOffset = outerInstance.RandomAccessFile.readInt(); // offset in local heap
          if (debugSymbolTable)
          {
            log.debug("WARNING Symbolic Link linkOffset={}", linkOffset);
          }
          isSymbolicLink = true;
        }

        if (debugSymbolTable)
        {
          log.debug("<-- end readSymbolTableEntry position={}", outerInstance.RandomAccessFile.FilePointer);
        }

        if (debugTracker)
        {
          outerInstance.memTracker.add("SymbolTableEntry", filePos, posData + 16);
        }
      }

      public virtual int getSize()
      {
        return outerInstance.header.isOffsetLong ? 40 : 32;
      }

      internal long getObjectAddress()
      {
        return objectHeaderAddress;
      }

      internal long getNameOffset()
      {
        return nameOffset;
      }

      public override string ToString()
      {
        return "SymbolTableEntry{" + "nameOffset=" + nameOffset + ", objectHeaderAddress=" + objectHeaderAddress + ", btreeAddress=" + btreeAddress + ", nameHeapAddress=" + nameHeapAddress + ", cacheType=" + cacheType + ", linkOffset=" + linkOffset + ", posData=" + posData + ", isSymbolicLink=" + isSymbolicLink + '}';
      }
    } // SymbolTableEntry

    /// <summary>
    /// Fetch a Vlen data array.
    /// </summary>
    /// <param name="globalHeapIdAddress"> address of the heapId, used to get the String out of the heap </param>
    /// <param name="dataType"> type of data </param>
    /// <param name="endian"> byteOrder of the data (0 = BE, 1 = LE) </param>
    /// <returns> the Array read from the heap </returns>
    internal virtual NcArray getHeapDataArray(long globalHeapIdAddress, DataType dataType, int endian)
    {
      HeapIdentifier heapId = new HeapIdentifier(this, globalHeapIdAddress);
      if (debugHeap)
      {
        log.debug(" heapId= {}", heapId);
      }
      return getHeapDataArray(heapId, dataType, endian);
      // Object pa = getHeapDataArray(heapId, dataType, endian);
      // return Array.factory(dataType.getPrimitiveClassType(), new int[]{heapId.nelems}, pa);
    }

    internal virtual NcArray getHeapDataArray(HeapIdentifier heapId, DataType dataType, int endian)
    {
      GlobalHeap.HeapObject ho = heapId.getHeapObject();
      if (ho == null)
      {
        throw new InvalidRangeException("Illegal Heap address, HeapObject = " + heapId);
      }
      if (debugHeap)
      {
        log.debug(" HeapObject= {}", ho);
      }
      if (endian >= 0)
      {
        RandomAccessFile.order(endian);
      }

      if (DataType.FLOAT == dataType)
      {
        float[] pa = new float[heapId.nelems];
        RandomAccessFile.seek(ho.dataPos);
        RandomAccessFile.readFloat(pa, 0, pa.Length);
        return NcArray.factory(dataType, new int[] { pa.Length }, pa);

      }
      else if (DataType.DOUBLE == dataType)
      {
        double[] pa = new double[heapId.nelems];
        RandomAccessFile.seek(ho.dataPos);
        RandomAccessFile.readDouble(pa, 0, pa.Length);
        return NcArray.factory(dataType, new int[] { pa.Length }, pa);

      }
      else if (dataType.getPrimitiveClassType() == typeof(byte))
      {
        byte[] pa = new byte[heapId.nelems];
        RandomAccessFile.seek(ho.dataPos);
        RandomAccessFile.readFully(pa, 0, pa.Length);
        return NcArray.factory(dataType, new int[] { pa.Length }, pa);

      }
      else if (dataType.getPrimitiveClassType() == typeof(short))
      {
        short[] pa = new short[heapId.nelems];
        RandomAccessFile.seek(ho.dataPos);
        RandomAccessFile.readShort(pa, 0, pa.Length);
        return NcArray.factory(dataType, new int[] { pa.Length }, pa);

      }
      else if (dataType.getPrimitiveClassType() == typeof(int))
      {
        int[] pa = new int[heapId.nelems];
        RandomAccessFile.seek(ho.dataPos);
        RandomAccessFile.readInt(pa, 0, pa.Length);
        return NcArray.factory(dataType, new int[] { pa.Length }, pa);

      }
      else if (dataType.getPrimitiveClassType() == typeof(long))
      {
        long[] pa = new long[heapId.nelems];
        RandomAccessFile.seek(ho.dataPos);
        RandomAccessFile.readLong(pa, 0, pa.Length);
        return NcArray.factory(dataType, new int[] { pa.Length }, pa);
      }

      throw new System.NotSupportedException("getHeapDataAsArray dataType=" + dataType);
    }

    // see "Global Heap Id" in http://www.hdfgroup.org/HDF5/doc/H5.format.html
    internal virtual HeapIdentifier readHeapIdentifier(long globalHeapIdAddress)
    {
      return new HeapIdentifier(this, globalHeapIdAddress);
    }

    // the heap id is has already been read into a byte array at given pos
    internal virtual HeapIdentifier readHeapIdentifier(NetcdfSharp.java.nio.ByteBuffer bb, int pos)
    {
      return new HeapIdentifier(this, bb, pos);
    }

    // see "Global Heap Id" in http://www.hdfgroup.org/HDF5/doc/H5.format.html
    internal class HeapIdentifier
    {
      internal H5objects outerInstance;

      internal readonly int nelems; // "number of 'base type' elements in the sequence in the heap"
      internal readonly long heapAddress;
      internal readonly int index;

      // address must be absolute, getFileOffset already added
      internal HeapIdentifier(H5objects outerInstance, long address)
      {
        this.outerInstance = outerInstance;
        // header information is in le byte order
        outerInstance.RandomAccessFile.order(RandomAccessFile.LITTLE_ENDIAN);
        outerInstance.RandomAccessFile.seek(address);
        nelems = outerInstance.RandomAccessFile.readInt();
        heapAddress = outerInstance.header.readOffset();
        index = outerInstance.RandomAccessFile.readInt();
        if (debugDetail)
        {
          log.debug("   read HeapIdentifier address=" + address + this);
        }
        if (debugHeap)
        {
          outerInstance.dump("heapIdentifier", outerInstance.header.getFileOffset(address), 16, true);
        }
      }

      // the heap id is has already been read into a byte array at given pos
      internal HeapIdentifier(H5objects outerInstance, NetcdfSharp.java.nio.ByteBuffer bb, int pos)
      {
        this.outerInstance = outerInstance;
        bb.order(NetcdfSharp.java.nio.ByteOrder.LITTLE_ENDIAN); // header information is in le byte order
        bb.position(pos); // relative reading
        nelems = bb.getInt();
        heapAddress = outerInstance.header.isOffsetLong ? bb.getLong() : (long)bb.getInt();
        index = bb.getInt();
        if (debugDetail)
        {
          log.debug("   read HeapIdentifier from ByteBuffer={}", this);
        }
      }

      public override string ToString()
      {
        return " nelems=" + nelems + " heapAddress=" + heapAddress + " index=" + index;
      }

      public virtual bool isEmpty()
      {
        return (heapAddress == 0);
      }

      internal virtual GlobalHeap.HeapObject getHeapObject()
      {
        if (isEmpty())
        {
          return null;
        }
        GlobalHeap gheap;
        if (!outerInstance.heapMap.TryGetValue(heapAddress, out gheap))
        {
          gheap = new GlobalHeap(outerInstance, heapAddress);
          outerInstance.heapMap[heapAddress] = gheap;
        }

        GlobalHeap.HeapObject ho = gheap.getHeapObject((short)index);
        if (ho == null)
        {
          throw new System.InvalidOperationException("cant find HeapObject");
        }
        return ho;
      }

    } // HeapIdentifier

    internal class RegionReference
    {
      internal H5objects outerInstance;

      private long heapAddress;
      private int index;

      internal RegionReference(H5objects outerInstance, long filePos)
      {
        this.outerInstance = outerInstance;
        // header information is in le byte order
        outerInstance.RandomAccessFile.order(RandomAccessFile.LITTLE_ENDIAN);
        outerInstance.RandomAccessFile.seek(filePos);
        heapAddress = outerInstance.header.readOffset();
        index = outerInstance.RandomAccessFile.readInt();

        GlobalHeap gheap;
        if (!outerInstance.heapMap.TryGetValue(heapAddress, out gheap) || gheap == null)
        {
          gheap = new GlobalHeap(outerInstance, heapAddress);
          outerInstance.heapMap[heapAddress] = gheap;
        }

        GlobalHeap.HeapObject want = gheap.getHeapObject((short)index);
        if (debugRegionReference)
        {
          log.debug(" found ho={}", want);
        }
        /*
         * - The offset of the object header of the object (ie. dataset) pointed to (yes, an object ID)
         * - A serialized form of a dataspace _selection_ of elements (in the dataset pointed to).
         * I don't have a formal description of this information now, but it's encoded in the H5S_<foo>_serialize()
         * routines in
         * src/H5S<foo>.c, where foo = {all, hyper, point, none}.
         * There is _no_ datatype information stored for these kind of selections currently.
         */
        outerInstance.RandomAccessFile.seek(want.dataPos);
        long objId = outerInstance.RandomAccessFile.readLong();
        DataObject ndo = outerInstance.header.getDataObject(objId, null);
        // String what = (ndo == null) ? "none" : ndo.getName();
        if (debugRegionReference)
        {
          log.debug(" objId=" + objId + " DataObject= " + ndo);
        }
        if (null == ndo)
        {
          throw new System.InvalidOperationException("cant find data object at" + objId);
        }
      }

    } // RegionReference

    // level 1E
    internal class GlobalHeap
    {
      internal H5objects outerInstance;

      private byte version;
      private int sizeBytes;
      private IDictionary<short, GlobalHeap.HeapObject> hos = new Dictionary<short, GlobalHeap.HeapObject>();

      internal GlobalHeap(H5objects outerInstance, long address)
      {
        this.outerInstance = outerInstance;
        // header information is in le byte order
        outerInstance.RandomAccessFile.order(RandomAccessFile.LITTLE_ENDIAN);
        outerInstance.RandomAccessFile.seek(outerInstance.header.getFileOffset(address));

        // header
        string magic = outerInstance.RandomAccessFile.readString(4);
        if (!magic.Equals("GCOL"))
        {
          throw new System.InvalidOperationException(magic + " should equal GCOL");
        }

        version = outerInstance.RandomAccessFile.readByte();
        outerInstance.RandomAccessFile.skipBytes(3);
        sizeBytes = outerInstance.RandomAccessFile.readInt();
        if (debugDetail)
        {
          log.debug("-- readGlobalHeap address=" + address + " version= " + version + " size = " + sizeBytes);
          // log.debug("-- readGlobalHeap address=" + address + " version= " + version + " size = " + sizeBytes);
        }
        outerInstance.RandomAccessFile.skipBytes(4); // pad to 8

        int count = 0;
        int countBytes = 0;
        while (true)
        {
          long startPos = outerInstance.RandomAccessFile.FilePointer;
          GlobalHeap.HeapObject o = new GlobalHeap.HeapObject();
          o.id = outerInstance.RandomAccessFile.readShort();
          if (o.id == 0)
          {
            break; // ?? look
          }
          o.refCount = outerInstance.RandomAccessFile.readShort();
          outerInstance.RandomAccessFile.skipBytes(4);
          o.dataSize = outerInstance.header.readLength();
          o.dataPos = outerInstance.RandomAccessFile.FilePointer;

          int dsize = ((int)o.dataSize) + padding((int)o.dataSize, 8);
          countBytes += dsize + 16;

          if (o.dataSize < 0)
          {
            break; // ran off the end, must be done
          }
          if (countBytes < 0)
          {
            break; // ran off the end, must be done
          }
          if (countBytes > sizeBytes)
          {
            break; // ran off the end
          }

          if (debugDetail)
          {
            log.debug("   HeapObject  position=" + startPos + " id=" + o.id + " refCount= " + o.refCount + " dataSize = " + o.dataSize + " dataPos = " + o.dataPos + " count= " + count + " countBytes= " + countBytes);
          }

          outerInstance.RandomAccessFile.skipBytes(dsize);
          hos[o.id] = o;
          count++;

          if (countBytes + 16 >= sizeBytes)
          {
            break; // ran off the end, must be done
          }
        }

        if (debugDetail)
        {
          log.debug("-- endGlobalHeap position=" + outerInstance.RandomAccessFile.FilePointer);
        }
        if (debugTracker)
        {
          outerInstance.memTracker.addByLen("GlobalHeap", address, sizeBytes);
        }
      }

      internal virtual GlobalHeap.HeapObject getHeapObject(short id)
      {
        return hos.TryGetValue(id, out var result) ? result : null;
      }

      internal class HeapObject
      {
        internal short id, refCount;
        internal long dataSize;
        internal long dataPos;

        public override string ToString()
        {
          return "id=" + id + ", refCount=" + refCount + ", dataSize=" + dataSize + ", dataPos=" + dataPos;
        }
      }

    } // GlobalHeap

    // level 1D
    internal class LocalHeap
    {
      internal H5objects outerInstance;

      internal H5Group group;
      internal int size;
      internal long freelistOffset, dataAddress;
      internal byte[] heap;
      internal byte version;

      internal LocalHeap(H5objects outerInstance, H5Group group, long address)
      {
        this.outerInstance = outerInstance;
        this.group = group;

        // header information is in le byte order
        outerInstance.RandomAccessFile.order(RandomAccessFile.LITTLE_ENDIAN);
        outerInstance.RandomAccessFile.seek(outerInstance.header.getFileOffset(address));

        if (debugDetail)
        {
          log.debug("-- readLocalHeap position={}", outerInstance.RandomAccessFile.FilePointer);
        }

        // header
        string magic = outerInstance.RandomAccessFile.readString(4);
        if (!magic.Equals("HEAP"))
        {
          throw new System.InvalidOperationException(magic + " should equal HEAP");
        }

        version = outerInstance.RandomAccessFile.readByte();
        outerInstance.RandomAccessFile.skipBytes(3);
        size = (int)outerInstance.header.readLength();
        freelistOffset = outerInstance.header.readLength();
        dataAddress = outerInstance.header.readOffset();
        if (debugDetail)
        {
          log.debug(" version=" + version + " size=" + size + " freelistOffset=" + freelistOffset + " heap starts at dataAddress=" + dataAddress);
        }
        if (debugPos)
        {
          log.debug("    *now at position={}", outerInstance.RandomAccessFile.FilePointer);
        }

        // data
        outerInstance.RandomAccessFile.seek(outerInstance.header.getFileOffset(dataAddress));
        heap = new byte[size];
        outerInstance.RandomAccessFile.readFully(heap);
        // if (debugHeap) printBytes( out, "heap", heap, size, true);

        if (debugDetail)
        {
          log.debug("-- endLocalHeap position={}", outerInstance.RandomAccessFile.FilePointer);
        }
        int hsize = 8 + 2 * outerInstance.header.sizeLengths + outerInstance.header.sizeOffsets;
        if (debugTracker)
        {
          outerInstance.memTracker.addByLen("Group LocalHeap (" + group.displayName + ")", address, hsize);
        }
        if (debugTracker)
        {
          outerInstance.memTracker.addByLen("Group LocalHeapData (" + group.displayName + ")", dataAddress, size);
        }
      }

      public virtual string getString(int offset)
      {
        int count = 0;
        while (heap[offset + count] != 0)
        {
          count++;
        }
        return StringHelper.NewString(heap, offset, count, Encoding.UTF8);
      }

    } // LocalHeap

    // Utilitie routines

    /// <summary>
    /// Read a zero terminated String. Leave file positioned after zero terminator byte.
    /// </summary>
    /// <param name="raf"> from this file </param>
    /// <returns> String (dont include zero terminator) </returns>
    private string readString(RandomAccessFile raf)
    {
      long filePos = raf.FilePointer;

      int count = 0;
      while (raf.readByte() != 0)
      {
        count++;
      }

      raf.seek(filePos);
      string result = raf.readString(count);
      raf.readByte(); // skip the zero byte! nn
      return result;
    }

    /// <summary>
    /// Read a zero terminated String at current position; advance file to a multiple of 8.
    /// </summary>
    /// <param name="raf"> from this file </param>
    /// <returns> String (dont include zero terminator) </returns>
    private string readString8(RandomAccessFile raf)
    {
      long filePos = raf.FilePointer;

      int count = 0;
      while (raf.readByte() != 0)
      {
        count++;
      }

      raf.seek(filePos);
      byte[] s = new byte[count];
      raf.readFully(s);

      // skip to 8 byte boundary, note zero byte is skipped
      count++;
      count += padding(count, 8);
      raf.seek(filePos + count);

      return StringHelper.NewString(s, Encoding.UTF8); // all Strings are UTF-8 unicode
    }

    /// <summary>
    /// Read a String of known length.
    /// </summary>
    /// <param name="size"> number of bytes </param>
    /// <returns> String result </returns>
    private string readStringFixedLength(int size)
    {
      return RandomAccessFile.readString(size);
    }

    // size of data depends on "maximum possible number"
    private long readVariableSizeMax(int maxNumber)
    {
      int size = header.getNumBytesFromMax(maxNumber);
      return header.readVariableSizeUnsigned(size);
    }

    private long readVariableSizeFactor(int sizeFactor)
    {
      int size = (int)Math.Pow(2, sizeFactor);
      return header.readVariableSizeUnsigned(size);
    }

    // find number of bytes needed to pad to multipleOf byte boundary
    private static int padding(int nbytes, int multipleOf)
    {
      int pad = nbytes % multipleOf;
      if (pad != 0)
      {
        pad = multipleOf - pad;
      }
      return pad;
    }

    private static int makeUnsignedIntFromBytes(byte upper, byte lower)
    {
      return DataTypes.unsignedByteToShort(upper) * 256 + DataTypes.unsignedByteToShort(lower);
    }

    private void dump(string head, long filePos, int nbytes, bool count)
    {
      if (debugOut == null)
      {
        return;
      }
      long savePos = RandomAccessFile.FilePointer;
      if (filePos >= 0)
      {
        RandomAccessFile.seek(filePos);
      }
      byte[] mess = new byte[nbytes];
      RandomAccessFile.readFully(mess);
      printBytes(head, mess, nbytes, false, debugOut);
      RandomAccessFile.seek(savePos);
    }

    private static void printBytes(string head, byte[] buff, int n, bool count, PrintWriter ps)
    {
      ps.Write(head + " == ");
      for (int i = 0; i < n; i++)
      {
        byte b = buff[i];
        int ub = (b < 0) ? b + 256 : b;
        if (count)
        {
          ps.Write(i + ":");
        }
        ps.Write(ub);
        if (!count)
        {
          ps.Write("(");
          ps.Write(b);
          ps.Write(")");
        }
        ps.Write(" ");
      }
      ps.WriteLine();
    }

  }

}
