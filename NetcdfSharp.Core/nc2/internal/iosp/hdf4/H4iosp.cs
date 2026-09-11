// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.@internal.iosp.hdf4

{

	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using ArrayStructure = NetcdfSharp.ucar.ma2.ArrayStructure;
	using ArrayStructureBB = NetcdfSharp.ucar.ma2.ArrayStructureBB;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using StructureMembers = NetcdfSharp.ucar.ma2.StructureMembers;
	using Group = NetcdfSharp.ucar.nc2.Group;
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using Variable = NetcdfSharp.ucar.nc2.Variable;
	using DataFormatType = NetcdfSharp.ucar.nc2.constants.DataFormatType;
	using AbstractIOServiceProvider = NetcdfSharp.ucar.nc2.iosp.AbstractIOServiceProvider;
	using IospHelper = NetcdfSharp.ucar.nc2.iosp.IospHelper;
	using Layout = NetcdfSharp.ucar.nc2.iosp.Layout;
	using LayoutBB = NetcdfSharp.ucar.nc2.iosp.LayoutBB;
	using LayoutBBTiled = NetcdfSharp.ucar.nc2.iosp.LayoutBBTiled;
	using LayoutRegular = NetcdfSharp.ucar.nc2.iosp.LayoutRegular;
	using LayoutSegmented = NetcdfSharp.ucar.nc2.iosp.LayoutSegmented;
	using LayoutTiled = NetcdfSharp.ucar.nc2.iosp.LayoutTiled;
	using TagEnum = NetcdfSharp.ucar.nc2.iosp.hdf4.TagEnum;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using IO = NetcdfSharp.ucar.nc2.util.IO;
	using PositioningDataInputStream = NetcdfSharp.ucar.unidata.io.PositioningDataInputStream;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// HDF4 iosp </summary>
	public class H4iosp : AbstractIOServiceProvider
	{
	  private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(H4iosp));
	  private static bool showLayoutTypes;

	  private H4header header;
	  private Charset valueCharset;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public boolean isValidFile(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf) throws NetcdfSharp.java.io.IOException
	  public override bool isValidFile(RandomAccessFile raf)
	  {
		return H4header.isValidFile(raf);
	  }

	  public override string FileTypeId
	  {
		  get
		  {
			if (header != null && header.Eos)
			{
			  return "HDF4-EOS";
			}
			return DataFormatType.HDF4.Description;
		  }
	  }

	  public override string FileTypeDescription
	  {
		  get
		  {
			return "Hierarchical Data Format, version 4";
		  }
	  }

	  public override string FileTypeVersion
	  {
		  get
		  {
			return header.version;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public void open(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, NetcdfSharp.ucar.nc2.NetcdfFile ncfile, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public override void open(RandomAccessFile raf, NetcdfFile ncfile, CancelTask cancelTask)
	  {
		base.open(raf, ncfile, cancelTask);
		Group.Builder rootGroup = Group.builder().setName("").setNcfile(ncfile);
		Header.read(raf, rootGroup, null);
		ncfile.setRootGroup(rootGroup.build());
		ncfile.finish();
	  }

	  /// <summary>
	  /// Return header for reading netcdf file.
	  /// Create it if it's not already created.
	  /// </summary>
	  /// <returns> header for reading HDF4 file. </returns>
	  private H4header Header
	  {
		  get
		  {
			if (header == null)
			{
			  header = new H4header(this);
			}
			return header;
		  }
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
		base.open(raf, rootGroup.getNcfile(), cancelTask);

		raf.order(RandomAccessFile.BIG_ENDIAN);
		header = new H4header(this);
		header.read(raf, rootGroup, null);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfSharp.ucar.ma2.Array readData(NetcdfSharp.ucar.nc2.Variable v, NetcdfSharp.ucar.ma2.Section section) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  public override NcArray readData(Variable v, Section section)
	  {
		if (v is Structure)
		{
		  return readStructureData((Structure) v, section);
		}

		H4header.Vinfo vinfo = (H4header.Vinfo) v.SPobject;
		DataType dataType = v.DataType;
		vinfo.LayoutInfo = this.ncfile; // make sure needed info is present

		// make sure section is complete
		section = Section.fill(section, v.Shape);

		if (vinfo.hasNoData)
		{
		  object arr = (vinfo.fillValue == null) ? IospHelper.makePrimitiveArray((int) section.computeSize(), dataType) : IospHelper.makePrimitiveArray((int) section.computeSize(), dataType, vinfo.fillValue);
		  if (dataType == DataType.CHAR)
		  {
			arr = IospHelper.convertByteToChar((byte[]) arr);
		  }
		  return NcArray.factory(dataType, section.Shape, arr);
		}

		if (!vinfo.isCompressed)
		{
		  if (!vinfo.isLinked && !vinfo.isChunked)
		  {
			Layout layout = new LayoutRegular(vinfo.start, v.ElementSize, v.Shape, section);
			object data = IospHelper.readDataFill(raf, layout, dataType, vinfo.fillValue, -1);
			return NcArray.factory(dataType, section.Shape, data);

		  }
		  else if (vinfo.isLinked)
		  {
			Layout layout = new LayoutSegmented(vinfo.segPos, vinfo.segSize, v.ElementSize, v.Shape, section);
			object data = IospHelper.readDataFill(raf, layout, dataType, vinfo.fillValue, -1);
			return NcArray.factory(dataType, section.Shape, data);

		  }
		  else if (vinfo.isChunked)
		  {
			H4ChunkIterator chunkIterator = new H4ChunkIterator(vinfo);
			Layout layout = new LayoutTiled(chunkIterator, vinfo.chunkSize, v.ElementSize, section);
			object data = IospHelper.readDataFill(raf, layout, dataType, vinfo.fillValue, -1);
			return NcArray.factory(dataType, section.Shape, data);
		  }

		}
		else
		{
		  if (!vinfo.isLinked && !vinfo.isChunked)
		  {
			if (showLayoutTypes)
			{
			  Console.WriteLine("***notLinked, compressed");
			}
			Layout index = new LayoutRegular(0, v.ElementSize, v.Shape, section);
			Stream @is = getCompressedInputStream(vinfo);
			PositioningDataInputStream dataSource = new PositioningDataInputStream(@is);
			object data = IospHelper.readDataFill(dataSource, index, dataType, vinfo.fillValue);
			return NcArray.factory(dataType, section.Shape, data);

		  }
		  else if (vinfo.isLinked)
		  {
			if (showLayoutTypes)
			{
			  Console.WriteLine("***Linked, compressed");
			}
			Layout index = new LayoutRegular(0, v.ElementSize, v.Shape, section);
			Stream @is = getLinkedCompressedInputStream(vinfo);
			PositioningDataInputStream dataSource = new PositioningDataInputStream(@is);
			object data = IospHelper.readDataFill(dataSource, index, dataType, vinfo.fillValue);
			return NcArray.factory(dataType, section.Shape, data);

		  }
		  else if (vinfo.isChunked)
		  {
			LayoutBBTiled.DataChunkIterator chunkIterator = new H4CompressedChunkIterator(this, vinfo);
			LayoutBB layout = new LayoutBBTiled(chunkIterator, vinfo.chunkSize, v.ElementSize, section);
			object data = IospHelper.readDataFill(layout, dataType, vinfo.fillValue);
			return NcArray.factory(dataType, section.Shape, data);
		  }
		}

		throw new System.InvalidOperationException();
	  }

	  /// <summary>
	  /// Structures must be fixed sized
	  /// </summary>
	  /// <param name="s"> the record structure </param>
	  /// <param name="section"> the record range to read </param>
	  /// <returns> an ArrayStructure, with all the data read in. </returns>
	  /// <exception cref="IOException"> on error </exception>
	  /// <exception cref="InvalidRangeException"> if invalid section </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.ucar.ma2.ArrayStructure readStructureData(NetcdfSharp.ucar.nc2.Structure s, NetcdfSharp.ucar.ma2.Section section) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException
	  private ArrayStructure readStructureData(Structure s, Section section)
	  {
		H4header.Vinfo vinfo = (H4header.Vinfo) s.SPobject;
		vinfo.LayoutInfo = this.ncfile; // make sure needed info is present
		int recsize = vinfo.elemSize;

		// create the ArrayStructure
		StructureMembers members = s.makeStructureMembers();
		foreach (StructureMembers.Member m in members.Members)
		{
		  Variable v2 = s.findVariable(m.Name);
		  H4header.Minfo minfo = (H4header.Minfo) v2.SPobject;
		  m.DataParam = minfo.offset;
		}
		members.StructureSize = recsize;
		ArrayStructureBB structureArray = new ArrayStructureBB(members, section.Shape); // LOOK subset

		// loop over records
		byte[] result = structureArray.ByteBuffer.array();

		/*
		 * if (vinfo.isChunked) {
		 * InputStream is = getChunkedInputStream(vinfo);
		 * PositioningDataInputStream dataSource = new PositioningDataInputStream(is);
		 * Layout layout = new LayoutRegular(vinfo.start, recsize, s.getShape(), section);
		 * IospHelper.readData(dataSource, layout, DataType.STRUCTURE, result);
		 */

		if (!vinfo.isLinked && !vinfo.isCompressed)
		{
		  Layout layout = new LayoutRegular(vinfo.start, recsize, s.Shape, section);
		  IospHelper.readData(raf, layout, DataType.STRUCTURE, result, -1, true);

		  /*
		   * option 1
		   * } else if (vinfo.isLinked && !vinfo.isCompressed) {
		   * Layout layout = new LayoutSegmented(vinfo.segPos, vinfo.segSize, recsize, s.getShape(), section);
		   * IospHelper.readData(raf, layout, DataType.STRUCTURE, result, -1);
		   */

		  // option 2
		}
		else if (vinfo.isLinked && !vinfo.isCompressed)
		{
		  Stream @is = new LinkedInputStream(this, vinfo);
		  PositioningDataInputStream dataSource = new PositioningDataInputStream(@is);
		  Layout layout = new LayoutRegular(0, recsize, s.Shape, section);
		  IospHelper.readData(dataSource, layout, DataType.STRUCTURE, result);

		}
		else if (!vinfo.isLinked && vinfo.isCompressed)
		{
		  Stream @is = getCompressedInputStream(vinfo);
		  PositioningDataInputStream dataSource = new PositioningDataInputStream(@is);
		  Layout layout = new LayoutRegular(0, recsize, s.Shape, section);
		  IospHelper.readData(dataSource, layout, DataType.STRUCTURE, result);

		}
		else if (vinfo.isLinked && vinfo.isCompressed)
		{
		  Stream @is = getLinkedCompressedInputStream(vinfo);
		  PositioningDataInputStream dataSource = new PositioningDataInputStream(@is);
		  Layout layout = new LayoutRegular(0, recsize, s.Shape, section);
		  IospHelper.readData(dataSource, layout, DataType.STRUCTURE, result);

		}
		else
		{
		  throw new System.InvalidOperationException();
		}

		return structureArray;
	  }

	  public override string toStringDebug(object o)
	  {
		if (o is Variable)
		{
		  Variable v = (Variable) o;
		  H4header.Vinfo vinfo = (H4header.Vinfo) v.SPobject;
		  return (vinfo != null) ? vinfo.ToString() : "";
		}
		return null;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private NetcdfSharp.java.io.InputStream getCompressedInputStream(H4header.Vinfo vinfo) throws NetcdfSharp.java.io.IOException
	  private Stream getCompressedInputStream(H4header.Vinfo vinfo)
	  {
		// probably could construct an input stream from a channel from a raf
		// for now, just read it in.
		byte[] buffer = new byte[vinfo.length];
		raf.seek(vinfo.start);
		raf.readFully(buffer);
		MemoryStream @in = new MemoryStream(buffer);
		return new DeflateStream(@in, CompressionMode.Decompress);
	  }

	  private Stream getLinkedCompressedInputStream(H4header.Vinfo vinfo)
	  {
		return new DeflateStream(new LinkedInputStream(this, vinfo), CompressionMode.Decompress);
	  }

	  private class LinkedInputStream : NetcdfSharp.java.io.InputStream
	  {
		  private readonly H4iosp outerInstance;

		internal byte[] buffer;

		internal int nsegs;
		internal long[] segPosA;
		internal int[] segSizeA;

		internal int segno = -1;
		internal int segpos;
		internal int segSize;
		// H4header.Vinfo vinfo;

		internal LinkedInputStream(H4iosp outerInstance, H4header.Vinfo vinfo)
		{
			this.outerInstance = outerInstance;
		  segPosA = vinfo.segPos;
		  segSizeA = vinfo.segSize;
		  nsegs = segSizeA.Length;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: LinkedInputStream(H4header.SpecialLinked linked) throws NetcdfSharp.java.io.IOException
		internal LinkedInputStream(H4iosp outerInstance, H4header.SpecialLinked linked)
		{
			this.outerInstance = outerInstance;
		  IList<H4header.TagLinkedBlock> linkedBlocks = linked.LinkedDataBlocks;
		  nsegs = linkedBlocks.Count;
		  segPosA = new long[nsegs];
		  segSizeA = new int[nsegs];
		  int count = 0;
		  foreach (H4header.TagLinkedBlock tag in linkedBlocks)
		  {
			segPosA[count] = tag.offset;
			segSizeA[count] = tag.length;
			count++;
		  }
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private boolean readSegment() throws NetcdfSharp.java.io.IOException
		internal virtual bool readSegment()
		{
		  segno++;
		  if (segno == nsegs)
		  {
			return false;
		  }

		  segSize = segSizeA[segno];
		  while (segSize == 0)
		  { // for some reason may have a 0 length segment
			segno++;
			if (segno == nsegs)
			{
			  return false;
			}
			segSize = segSizeA[segno];
		  }

		  buffer = new byte[segSize]; // Look: could do this in buffer size 4096 to save memory
		  outerInstance.raf.seek(segPosA[segno]);
		  outerInstance.raf.readFully(buffer);
		  segpos = 0;

		  return true;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public int read() throws NetcdfSharp.java.io.IOException
		public override int read()
		{
		  if (segpos == segSize)
		  {
			bool ok = readSegment();
			if (!ok)
			{
			  return -1;
			}
		  }

		  int b = buffer[segpos] & 0xff;
		  segpos++;
		  return b;
		}
	  }

	  /*
	   * // TODO implement this
	   * private InputStream getChunkedInputStream(H4header.Vinfo vinfo) {
	   * return new ChunkedInputStream(vinfo);
	   * }
	   * 
	   * // look not implemented : skip data
	   * private class ChunkedInputStream extends InputStream {
	   * List<H4header.DataChunk> chunks;
	   * int chunkNo;
	   * 
	   * byte[] buffer;
	   * int segPos, segSize;
	   * 
	   * ChunkedInputStream(H4header.Vinfo vinfo) {
	   * this.chunks = vinfo.chunks;
	   * this.chunkNo = 0;
	   * }
	   * 
	   * private void readChunk() throws IOException {
	   * H4header.DataChunk chunk = chunks.get(chunkNo);
	   * H4header.TagData chunkData = chunk.data;
	   * chunkNo++;
	   * 
	   * if (chunkData.ext_type == TagEnum.SPECIAL_COMP) {
	   * // read compressed data in
	   * H4header.TagData cdata = chunkData.compress.getDataTag();
	   * byte[] cbuffer = new byte[cdata.length];
	   * raf.seek(cdata.offset);
	   * raf.readFully(cbuffer);
	   * 
	   * // uncompress it
	   * if (chunkData.compress.compress_type == TagEnum.COMP_CODE_DEFLATE) {
	   * InputStream in = new NetcdfSharp.java.util.zip.InflaterInputStream(new ByteArrayInputStream(cbuffer));
	   * ByteArrayOutputStream out = new ByteArrayOutputStream(chunkData.compress.uncomp_length);
	   * IO.copy(in, out);
	   * buffer = out.toByteArray();
	   * } else {
	   * throw new IllegalStateException("unknown compression type =" + chunkData.compress.compress_type);
	   * }
	   * 
	   * } else {
	   * buffer = new byte[chunkData.length];
	   * raf.seek(chunkData.offset);
	   * raf.readFully(buffer);
	   * }
	   * 
	   * segPos = 0;
	   * segSize = buffer.length;
	   * }
	   * 
	   * public int read() throws IOException {
	   * if (segPos == segSize)
	   * readChunk();
	   * int b = buffer[segPos] & 0xff;
	   * segPos++;
	   * return b;
	   * }
	   * }
	   */

	  private class H4ChunkIterator : LayoutTiled.DataChunkIterator
	  {
		internal IList<H4header.DataChunk> chunks;
		internal int chunkNo;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: H4ChunkIterator(H4header.Vinfo vinfo) throws NetcdfSharp.java.io.IOException
		internal H4ChunkIterator(H4header.Vinfo vinfo)
		{
		  this.chunks = vinfo.chunks;
		  this.chunkNo = 0;
		}

		public virtual bool hasNext()
		{
		  return chunkNo < chunks.Count;
		}

		public virtual LayoutTiled.DataChunk next()
		{
		  H4header.DataChunk chunk = chunks[chunkNo];
		  H4header.TagData chunkData = chunk.data;
		  chunkNo++;

		  return new LayoutTiled.DataChunk(chunk.origin, chunkData.offset);
		}
	  }

	  private class H4CompressedChunkIterator : LayoutBBTiled.DataChunkIterator
	  {
		  private readonly H4iosp outerInstance;

		internal IList<H4header.DataChunk> chunks;
		internal int chunkNo;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: H4CompressedChunkIterator(H4header.Vinfo vinfo) throws NetcdfSharp.java.io.IOException
		internal H4CompressedChunkIterator(H4iosp outerInstance, H4header.Vinfo vinfo)
		{
			this.outerInstance = outerInstance;
		  this.chunks = vinfo.chunks;
		  this.chunkNo = 0;
		}

		public virtual bool hasNext()
		{
		  return chunkNo < chunks.Count;
		}

		public virtual LayoutBBTiled.DataChunk next()
		{
		  H4header.DataChunk chunk = chunks[chunkNo];
		  H4header.TagData chunkData = chunk.data;
		  Debug.Assert((chunkData.ext_type == TagEnum.SPECIAL_COMP));
		  chunkNo++;

		  return new DataChunk(outerInstance, chunk.origin, chunkData.compress);
		}
	  }

	  private class DataChunk : LayoutBBTiled.DataChunk
	  {
		  private readonly H4iosp outerInstance;

    internal int[] offset; // offset index of this chunk, relative to entire array
    internal H4header.SpecialComp compress;
    private NetcdfSharp.java.nio.ByteBuffer bb; // the data is placed into here

    internal DataChunk(H4iosp outerInstance, int[] offset, H4header.SpecialComp compress)
    {
        this.outerInstance = outerInstance;
      this.offset = offset;
      this.compress = compress;
    }

    public virtual int[] Offset
    {
        get
        {
          return offset;
        }
    }

    public virtual NetcdfSharp.java.nio.ByteBuffer ByteBuffer
    {
        get
        {
          if (bb == null)
          {
            // read compressed data in
            H4header.TagData cdata = compress.getDataTag();
            Stream @in;

            // compressed data stored in one place
            if (cdata.linked == null)
            {
              byte[] cbuffer = new byte[cdata.length];
              outerInstance.raf.seek(cdata.offset);
              outerInstance.raf.readFully(cbuffer);
              @in = new MemoryStream(cbuffer);

            }
            else
            { // or compressed data stored in linked storage
              @in = new LinkedInputStream(outerInstance, cdata.linked);
            }

            // uncompress it
            if (compress.compress_type == TagEnum.COMP_CODE_DEFLATE)
            {
              // read the stream in and uncompress
			  Stream zin = new DeflateStream(@in, CompressionMode.Decompress);
              MemoryStream @out = new MemoryStream(compress.uncomp_length);
              IO.copy(zin, @out);
              byte[] buffer = @out.ToArray();
              bb = NetcdfSharp.java.nio.ByteBuffer.wrap(buffer);

            }
            else if (compress.compress_type == TagEnum.COMP_CODE_NONE)
            {
              // just read the stream in
              MemoryStream @out = new MemoryStream(compress.uncomp_length);
              IO.copy(@in, @out);
              byte[] buffer = @out.ToArray();
              bb = NetcdfSharp.java.nio.ByteBuffer.wrap(buffer);
            }
            else
            {
              throw new System.InvalidOperationException("unknown compression type =" + compress.compress_type);
            }
          }

          return bb;
        }
    }

  }

  public override void reacquire()
  {
    base.reacquire();
    Header.raf = this.raf;
  }

  public override object sendIospMessage(object message)
  {
    if (message is Charset)
    {
      setValueCharset((Charset) message);
    }
    if (message.ToString().Equals("header"))
    {
      return Header;
    }
    return base.sendIospMessage(message);
  }

  /// <summary>
  /// Return <seealso cref="Charset value charset"/> if it was defined. Definition of charset
  /// occurs by sending a charset as a message using the <seealso cref="sendIospMessage"/>
  /// method.
  /// </summary>
  /// <returns> <seealso cref="Charset value charset"/> if it was defined. </returns>
	protected internal Optional<Charset> getValueCharset()
  {
    return Optional<Charset>.ofNullable(valueCharset);
  }

  /// <summary>
  /// Define <seealso cref="Charset value charset"/>.
  /// </summary>
  /// <param name="charset"> may be null. </param>
  protected void setValueCharset(Charset charset)
  {
    this.valueCharset = charset;
  }
}

}
