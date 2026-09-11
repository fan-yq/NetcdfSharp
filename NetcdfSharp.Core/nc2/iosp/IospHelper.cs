// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp
{
	using NetcdfSharp.ucar.ma2;
	using NcStream = NetcdfSharp.ucar.nc2.stream.NcStream;
	using ParsedSectionSpec = NetcdfSharp.ucar.nc2.ParsedSectionSpec;
	using PositioningDataInputStream = NetcdfSharp.ucar.unidata.io.PositioningDataInputStream;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using Structure = NetcdfSharp.ucar.nc2.Structure;
	using Variable = NetcdfSharp.ucar.nc2.Variable;

	/// <summary>
	/// Helper methods for IOSP's for reading data.
	/// 
	/// @author caron
	/// @since Jan 3, 2008
	/// </summary>
	public class IospHelper
	{
		private static bool showLayoutTypes = false;

		/// <summary>
		/// Read data subset from RandomAccessFile, create primitive array of size Layout.getTotalNelems.
		/// Reading is controlled by the Layout object.
		/// </summary>
		/// <param name="raf"> read from here. </param>
		/// <param name="index"> handles skipping around in the file. </param>
		/// <param name="dataType"> dataType of the variable </param>
		/// <param name="fillValue"> must be Number if dataType.isNumeric(), or String for STRING, byte[] for Structure, or null for
		///        none </param>
		/// <param name="byteOrder"> if equal to RandomAccessFile.ORDER_XXXX, set the byte order just before reading </param>
		/// <returns> primitive array with data read in </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static Object readDataFill(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Layout index, DataType dataType, Object fillValue, int byteOrder) throws NetcdfSharp.java.io.IOException
		public static object readDataFill(RandomAccessFile raf, Layout index, DataType dataType, object fillValue, int byteOrder)
		{
			object arr = (fillValue == null) ? makePrimitiveArray((int)index.TotalNelems, dataType) : makePrimitiveArray((int)index.TotalNelems, dataType, fillValue);
			return readData(raf, index, dataType, arr, byteOrder, true);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static Object readDataFill(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Layout index, DataType dataType, Object fillValue, int byteOrder, boolean convertChar) throws NetcdfSharp.java.io.IOException
		public static object readDataFill(RandomAccessFile raf, Layout index, DataType dataType, object fillValue, int byteOrder, bool convertChar)
		{
			object arr = (fillValue == null) ? makePrimitiveArray((int)index.TotalNelems, dataType) : makePrimitiveArray((int)index.TotalNelems, dataType, fillValue);
			return readData(raf, index, dataType, arr, byteOrder, convertChar);
		}

		/// <summary>
		/// Read data subset from RandomAccessFile, place in given primitive array.
		/// Reading is controlled by the Layout object.
		/// </summary>
		/// <param name="raf"> read from here. </param>
		/// <param name="layout"> handles skipping around in the file. </param>
		/// <param name="dataType"> dataType of the variable </param>
		/// <param name="arr"> primitive array to read data into </param>
		/// <param name="byteOrder"> if equal to RandomAccessFile.ORDER_XXXX, set the byte order just before reading </param>
		/// <param name="convertChar"> true if bytes should be converted to char for dataType CHAR </param>
		/// <returns> primitive array with data read in </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static Object readData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Layout layout, DataType dataType, Object arr, int byteOrder, boolean convertChar) throws NetcdfSharp.java.io.IOException
		public static object readData(RandomAccessFile raf, Layout layout, DataType dataType, object arr, int byteOrder, bool convertChar)
		{
			if (showLayoutTypes)
			{
				// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Console.WriteLine("***RAF LayoutType=" + layout.GetType().FullName);
			}

			if (dataType.getPrimitiveClassType() == typeof(byte) || dataType == DataType.CHAR)
			{
				byte[] pa = (byte[])arr;
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					raf.order(byteOrder);
					raf.seek(chunk.SrcPos);
					raf.readFully(pa, (int)chunk.DestElem, chunk.Nelems);
				}
				if (convertChar && dataType == DataType.CHAR)
				{
					return convertByteToChar(pa);
				}
				else
				{
					return pa; // javac ternary compile error
				}

			}
			else if (dataType.getPrimitiveClassType() == typeof(short))
			{
				short[] pa = (short[])arr;
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					raf.order(byteOrder);
					raf.seek(chunk.SrcPos);
					raf.readShort(pa, (int)chunk.DestElem, chunk.Nelems);
				}
				return pa;

			}
			else if (dataType.getPrimitiveClassType() == typeof(int))
			{
				int[] pa = (int[])arr;
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					raf.order(byteOrder);
					raf.seek(chunk.SrcPos);
					raf.readInt(pa, (int)chunk.DestElem, chunk.Nelems);
				}
				return pa;

			}
			else if (dataType == DataType.FLOAT)
			{
				float[] pa = (float[])arr;
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					raf.order(byteOrder);
					raf.seek(chunk.SrcPos);
					raf.readFloat(pa, (int)chunk.DestElem, chunk.Nelems);
				}
				return pa;

			}
			else if (dataType == DataType.DOUBLE)
			{
				double[] pa = (double[])arr;
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					raf.order(byteOrder);
					raf.seek(chunk.SrcPos);
					raf.readDouble(pa, (int)chunk.DestElem, chunk.Nelems);
				}
				return pa;

			}
			else if (dataType.getPrimitiveClassType() == typeof(long))
			{
				long[] pa = (long[])arr;
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					raf.order(byteOrder);
					raf.seek(chunk.SrcPos);
					raf.readLong(pa, (int)chunk.DestElem, chunk.Nelems);
				}
				return pa;

			}
			else if (dataType == DataType.STRUCTURE)
			{
				byte[] pa = (byte[])arr;
				int recsize = layout.ElemSize;
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					raf.order(byteOrder);
					raf.seek(chunk.SrcPos);
					raf.readFully(pa, (int)chunk.DestElem * recsize, chunk.Nelems * recsize);
				}
				return pa;
			}
			else if (dataType == DataType.STRING)
			{
				int size = (int)layout.TotalNelems;
				int elemSize = layout.ElemSize;
				StringBuilder sb = new StringBuilder(size);
				while (layout.hasNext())
				{
					Layout.Chunk chunk = layout.next();
					if (chunk == null)
					{
						continue;
					}
					for (int i = 0; i < chunk.Nelems; i++)
					{
						sb.Append(raf.readString(elemSize));
					}
				}
				return sb.ToString();
			}

			throw new System.InvalidOperationException("unknown type= " + dataType);
		}

		/// <summary>
		/// Read data subset from PositioningDataInputStream, create primitive array of size Layout.getTotalNelems.
		/// Reading is controlled by the Layout object.
		/// </summary>
		/// <param name="is"> read from here. </param>
		/// <param name="index"> handles skipping around in the file. </param>
		/// <param name="dataType"> dataType of the variable </param>
		/// <param name="fillValue"> must be Number if dataType.isNumeric(), or String for STRING, byte[] for Structure, or null for
		///        none </param>
		/// <returns> primitive array with data read in </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static Object readDataFill(NetcdfSharp.ucar.unidata.io.PositioningDataInputStream is, Layout index, DataType dataType, Object fillValue) throws NetcdfSharp.java.io.IOException
		public static object readDataFill(PositioningDataInputStream @is, Layout index, DataType dataType, object fillValue)
		{
			object arr = (fillValue == null) ? makePrimitiveArray((int)index.TotalNelems, dataType) : makePrimitiveArray((int)index.TotalNelems, dataType, fillValue);
			return readData(@is, index, dataType, arr);
		}


		/// <summary>
		/// Read data subset from PositioningDataInputStream, place in given primitive array.
		/// Reading is controlled by the Layout object.
		/// </summary>
		/// <param name="raf"> read from here. </param>
		/// <param name="index"> handles skipping around in the file. </param>
		/// <param name="dataType"> dataType of the variable </param>
		/// <param name="arr"> primitive array to read data into </param>
		/// <returns> primitive array with data read in </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on read error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static Object readData(NetcdfSharp.ucar.unidata.io.PositioningDataInputStream raf, Layout index, DataType dataType, Object arr) throws NetcdfSharp.java.io.IOException
		public static object readData(PositioningDataInputStream raf, Layout index, DataType dataType, object arr)
		{
			if (showLayoutTypes)
			{
				// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Console.WriteLine("***PositioningDataInputStream LayoutType=" + index.GetType().FullName);
			}

			if (dataType.getPrimitiveClassType() == typeof(byte) || dataType == DataType.CHAR)
			{
				byte[] pa = (byte[])arr;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.read(chunk.SrcPos, pa, (int)chunk.DestElem, chunk.Nelems);
				}
				// return (dataType == DataType.CHAR) ? convertByteToChar(pa) : pa;
				if (dataType == DataType.CHAR)
				{
					return convertByteToChar(pa);
				}
				else
				{
					return pa;
				}

			}
			else if (dataType.getPrimitiveClassType() == typeof(short))
			{
				short[] pa = (short[])arr;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.readShort(chunk.SrcPos, pa, (int)chunk.DestElem, chunk.Nelems);
				}
				return pa;

			}
			else if (dataType.getPrimitiveClassType() == typeof(int))
			{
				int[] pa = (int[])arr;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.readInt(chunk.SrcPos, pa, (int)chunk.DestElem, chunk.Nelems);
				}
				return pa;

			}
			else if (dataType == DataType.FLOAT)
			{
				float[] pa = (float[])arr;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.readFloat(chunk.SrcPos, pa, (int)chunk.DestElem, chunk.Nelems);
				}
				return pa;

			}
			else if (dataType == DataType.DOUBLE)
			{
				double[] pa = (double[])arr;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.readDouble(chunk.SrcPos, pa, (int)chunk.DestElem, chunk.Nelems);
				}
				return pa;

			}
			else if (dataType.getPrimitiveClassType() == typeof(long))
			{
				long[] pa = (long[])arr;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.readLong(chunk.SrcPos, pa, (int)chunk.DestElem, chunk.Nelems);
				}
				return pa;

			}
			else if (dataType == DataType.STRUCTURE)
			{
				int recsize = index.ElemSize;
				byte[] pa = (byte[])arr;
				while (index.hasNext())
				{
					Layout.Chunk chunk = index.next();
					raf.read(chunk.SrcPos, pa, (int)chunk.DestElem * recsize, chunk.Nelems * recsize);
				}
				return pa;
			}

			throw new System.InvalidOperationException();
		}

		/// <summary>
		/// Read data subset from ByteBuffer, create primitive array of size Layout.getTotalNelems.
		/// Reading is controlled by the Layout object.
		/// </summary>
		/// <param name="layout"> handles skipping around in the file, provide ByteBuffer to read from </param>
		/// <param name="dataType"> dataType of the variable </param>
		/// <param name="fillValue"> must be Number if dataType.isNumeric(), or String for STRING, byte[] for Structure, or null for
		///        none </param>
		/// <returns> primitive array with data read in </returns>
		public static object readDataFill(LayoutBB layout, DataType dataType, object fillValue)
		{
			long size = layout.TotalNelems;
			if (dataType == DataType.STRUCTURE)
			{
				size *= layout.ElemSize;
			}
			if (size > int.MaxValue)
			{
				throw new Exception("Read request size of " + size + " is too large.");
			}
			object arr = (fillValue == null) ? makePrimitiveArray((int)size, dataType) : makePrimitiveArray((int)size, dataType, fillValue);
			return readData(layout, dataType, arr);
		}

		/// <summary>
		/// Read data subset from ByteBuffer, place in given primitive array.
		/// Reading is controlled by the LayoutBB object.
		/// </summary>
		/// <param name="layout"> handles skipping around in the file, privide ByteBuffer to read from </param>
		/// <param name="dataType"> dataType of the variable </param>
		/// <param name="arr"> primitive array to read data into </param>
		/// <returns> the primitive array with data read in </returns>
		public static object readData(LayoutBB layout, DataType dataType, object arr)
		{
			if (showLayoutTypes)
			{
				// JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Console.WriteLine("***BB LayoutType=" + layout.GetType().FullName);
			}

			if (dataType.getPrimitiveClassType() == typeof(byte) || (dataType == DataType.CHAR) || dataType == DataType.BOOLEAN)
			{
				byte[] pa = (byte[])arr;
				while (layout.hasNext())
				{
					LayoutBB.ChunkBB chunk = (LayoutBB.ChunkBB)layout.next();
					ByteBuffer bb = chunk.ByteBuffer;
					// if chunk is empty, use fill value
					if (!bb.hasRemaining())
					{
						continue;
					}
					bb.position(chunk.SrcElem);
					int pos = (int)chunk.DestElem;
					for (int i = 0; i < chunk.Nelems; i++)
					{
						pa[pos++] = bb.get();
					}
				}
				// return (dataType == DataType.CHAR) ? convertByteToChar(pa) : pa;
				if (dataType == DataType.CHAR)
				{
					return convertByteToChar(pa);
				}
				else if (dataType == DataType.BOOLEAN)
				{
					return convertByteToBoolean(pa);
				}
				else
				{
					return pa;
				}

			}
			else if (dataType.getPrimitiveClassType() == typeof(short))
			{
				short[] pa = (short[])arr;
				while (layout.hasNext())
				{
					LayoutBB.ChunkBB chunk = (LayoutBB.ChunkBB)layout.next();
					ShortBuffer buff = chunk.ShortBuffer;
					// if chunk is empty, use fill value
					if (!buff.hasRemaining())
					{
						continue;
					}
					buff.position(chunk.SrcElem);
					int pos = (int)chunk.DestElem;
					for (int i = 0; i < chunk.Nelems; i++)
					{
						pa[pos++] = buff.get();
					}
				}
				return pa;

			}
			else if (dataType.getPrimitiveClassType() == typeof(int))
			{
				int[] pa = (int[])arr;
				while (layout.hasNext())
				{
					LayoutBB.ChunkBB chunk = (LayoutBB.ChunkBB)layout.next();
					IntBuffer buff = chunk.IntBuffer;
					// if chunk is empty, use fill value
					if (!buff.hasRemaining())
					{
						continue;
					}
					buff.position(chunk.SrcElem);
					int pos = (int)chunk.DestElem;
					for (int i = 0; i < chunk.Nelems; i++)
					{
						pa[pos++] = buff.get();
					}
				}
				return pa;

			}
			else if (dataType == DataType.FLOAT)
			{
				float[] pa = (float[])arr;
				while (layout.hasNext())
				{
					LayoutBB.ChunkBB chunk = (LayoutBB.ChunkBB)layout.next();
					FloatBuffer buff = chunk.FloatBuffer;
					// if chunk is empty, use fill value
					if (!buff.hasRemaining())
					{
						continue;
					}
					buff.position(chunk.SrcElem);
					int pos = (int)chunk.DestElem;
					for (int i = 0; i < chunk.Nelems; i++)
					{
						pa[pos++] = buff.get();
					}
				}
				return pa;

			}
			else if (dataType == DataType.DOUBLE)
			{
				double[] pa = (double[])arr;
				while (layout.hasNext())
				{
					LayoutBB.ChunkBB chunk = (LayoutBB.ChunkBB)layout.next();
					DoubleBuffer buff = chunk.DoubleBuffer;
					// if chunk is empty, use fill value
					if (!buff.hasRemaining())
					{
						continue;
					}
					buff.position(chunk.SrcElem);
					int pos = (int)chunk.DestElem;
					for (int i = 0; i < chunk.Nelems; i++)
					{
						pa[pos++] = buff.get();
					}
				}
				return pa;

			}
			else if (dataType.getPrimitiveClassType() == typeof(long))
			{
				long[] pa = (long[])arr;
				while (layout.hasNext())
				{
					LayoutBB.ChunkBB chunk = (LayoutBB.ChunkBB)layout.next();
					LongBuffer buff = chunk.LongBuffer;
					// if chunk is empty, use fill value
					if (!buff.hasRemaining())
					{
						continue;
					}
					buff.position(chunk.SrcElem);
					int pos = (int)chunk.DestElem;
					for (int i = 0; i < chunk.Nelems; i++)
					{
						pa[pos++] = buff.get();
					}
				}
				return pa;

			}
			else if (dataType == DataType.STRUCTURE)
			{
				byte[] pa = (byte[])arr;
				int recsize = layout.ElemSize;
				while (layout.hasNext())
				{
					LayoutBB.ChunkBB chunk = (LayoutBB.ChunkBB)layout.next();
					ByteBuffer bb = chunk.ByteBuffer;
					// if chunk is empty, use fill value
					if (!bb.hasRemaining())
					{
						continue;
					}
					bb.position(chunk.SrcElem * recsize);
					int pos = (int)chunk.DestElem * recsize;
					for (int i = 0; i < chunk.Nelems * recsize; i++)
					{
						pa[pos++] = bb.get();
					}
				}
				return pa;
			}
			else if (dataType == DataType.STRING)
			{
				string[] pa = (string[])arr;
				int recsize = layout.ElemSize;
				while (layout.hasNext())
				{
					LayoutBB.ChunkBB chunk = (LayoutBB.ChunkBB)layout.next();
					ByteBuffer bb = chunk.ByteBuffer;
					// if chunk is empty, use fill value
					if (!bb.hasRemaining())
					{
						continue;
					}
					bb.position(chunk.SrcElem * recsize);
					int pos = (int)chunk.DestElem;
					for (int i = 0; i < chunk.Nelems; i++)
					{
						char[] ch = new char[dataType.getSize()];
						for (int j = 0; j < ch.Length; j++)
						{
							ch[j] = (char)bb.get();
						}
						pa[pos++] = new string(ch);
					}
				}
				return pa;
			}

			throw new System.InvalidOperationException();
		}

		/// <summary>
		/// Copy data to a channel. Used by ncstream. Not doing Structures correctly yet.
		/// </summary>
		/// <param name="data"> copy from here </param>
		/// <param name="channel"> copy to here </param>
		/// <returns> number of bytes copied </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on write error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static long copyToByteChannel(Array data, WritableByteChannel channel) throws NetcdfSharp.java.io.IOException
		public static long copyToByteChannel(NcArray data, WritableByteChannel channel)
		{
			Type classType = data.ElementType;

			DataOutputStream outStream = new DataOutputStream(Channels.newOutputStream(channel));
			IndexIterator iterA = data.IndexIterator;

			if (classType == typeof(double))
			{
				while (iterA.hasNext())
				{
					outStream.writeDouble(iterA.DoubleNext);
				}

			}
			else if (classType == typeof(float))
			{
				while (iterA.hasNext())
				{
					outStream.writeFloat(iterA.FloatNext);
				}

			}
			else if (classType == typeof(long))
			{
				while (iterA.hasNext())
				{
					outStream.writeLong(iterA.LongNext);
				}

			}
			else if (classType == typeof(int))
			{
				while (iterA.hasNext())
				{
					outStream.writeInt(iterA.IntNext);
				}

			}
			else if (classType == typeof(short))
			{
				while (iterA.hasNext())
				{
					outStream.writeShort(iterA.ShortNext);
				}

			}
			else if (classType == typeof(char))
			{ // LOOK why are we using chars anyway ?
				byte[] pa = convertCharToByte((char[])data.get1DJavaArray(DataType.CHAR));
				outStream.write(pa, 0, pa.Length);

			}
			else if (classType == typeof(byte))
			{
				while (iterA.hasNext())
				{
					outStream.writeByte(iterA.ByteNext);
				}

			}
			else if (classType == typeof(bool))
			{
				while (iterA.hasNext())
				{
					outStream.writeBoolean(iterA.BooleanNext);
				}

			}
			else if (classType == typeof(string))
			{
				long size = 0;
				while (iterA.hasNext())
				{
					string s = (string)iterA.ObjectNext;
					size += NcStream.writeVInt(outStream, s.Length);
					byte[] b = Encoding.UTF8.GetBytes(s);
					outStream.write(b);
					size += b.Length;
				}
				return size;

			}
			else if (classType == typeof(ByteBuffer))
			{ // OPAQUE
				long size = 0;
				while (iterA.hasNext())
				{
					ByteBuffer bb = (ByteBuffer)iterA.ObjectNext;
					size += NcStream.writeVInt(outStream, bb.limit());
					bb.rewind();
					channel.write(bb);
					size += bb.limit();
				}
				return size;

			}
			else if (data is ArrayObject)
			{ // vlen
				long size = 0;
				while (iterA.hasNext())
				{
					NcArray row = (NcArray)iterA.ObjectNext;
					ByteBuffer bb = row.getDataAsByteBuffer();
					byte[] result = bb.array();
					size += NcStream.writeVInt(outStream, result.Length); // size in bytes
					outStream.write(result); // array
					size += result.Length;
				}
				return size;

			}
			else
			{
				throw new System.NotSupportedException("Class type = " + classType.FullName);
			}

			return data.SizeBytes;
		}

		/// <summary>
		/// Copy data to a OutputStream. Used by ncstream. Not doing Structures correctly yet.
		/// </summary>
		/// <param name="data"> copy from here </param>
		/// <param name="out"> copy to here </param>
		/// <returns> number of bytes copied </returns>
		/// <exception cref="NetcdfSharp.java.io.IOException"> on write error </exception>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static long copyToOutputStream(Array data, OutputStream out) throws NetcdfSharp.java.io.IOException
		public static long copyToOutputStream(NcArray data, Stream @out)
		{
			Type classType = data.ElementType;

			DataOutputStream dataOut = new DataOutputStream(@out);

			IndexIterator iterA = data.IndexIterator;

			if (classType == typeof(double))
			{
				while (iterA.hasNext())
				{
					dataOut.writeDouble(iterA.DoubleNext);
				}

			}
			else if (classType == typeof(float))
			{
				while (iterA.hasNext())
				{
					dataOut.writeFloat(iterA.FloatNext);
				}

			}
			else if (classType == typeof(long))
			{
				while (iterA.hasNext())
				{
					dataOut.writeLong(iterA.LongNext);
				}

			}
			else if (classType == typeof(int))
			{
				while (iterA.hasNext())
				{
					dataOut.writeInt(iterA.IntNext);
				}

			}
			else if (classType == typeof(short))
			{
				while (iterA.hasNext())
				{
					dataOut.writeShort(iterA.ShortNext);
				}

			}
			else if (classType == typeof(char))
			{ // LOOK why are we using chars anyway ?
				byte[] pa = convertCharToByte((char[])data.get1DJavaArray(DataType.CHAR));
				dataOut.write(pa, 0, pa.Length);

			}
			else if (classType == typeof(byte))
			{
				while (iterA.hasNext())
				{
					dataOut.writeByte(iterA.ByteNext);
				}

			}
			else if (classType == typeof(bool))
			{
				while (iterA.hasNext())
				{
					dataOut.writeBoolean(iterA.BooleanNext);
				}

			}
			else if (classType == typeof(string))
			{
				long size = 0;
				while (iterA.hasNext())
				{
					string s = (string)iterA.ObjectNext;
					size += NcStream.writeVInt(dataOut, s.Length);
					byte[] b = Encoding.UTF8.GetBytes(s);
					dataOut.write(b);
					size += b.Length;
				}
				return size;

			}
			else if (classType == typeof(ByteBuffer))
			{ // OPAQUE
				long size = 0;
				while (iterA.hasNext())
				{
					ByteBuffer bb = (ByteBuffer)iterA.ObjectNext;
					size += NcStream.writeVInt(dataOut, bb.limit());
					bb.rewind();
					dataOut.write(bb.array());
					size += bb.limit();
				}
				return size;

			}
			else if (data is ArrayObject)
			{ // vlen
				long size = 0;
				while (iterA.hasNext())
				{
					NcArray row = (NcArray)iterA.ObjectNext;
					ByteBuffer bb = row.getDataAsByteBuffer();
					byte[] result = bb.array();
					size += NcStream.writeVInt(dataOut, result.Length); // size in bytes
					dataOut.write(result); // array
					size += result.Length;
				}
				return size;

			}
			else
			{
				throw new System.NotSupportedException("Class type = " + classType.FullName);
			}

			return data.SizeBytes;
		}

		/// <summary>
		/// @deprecated use StructureDataDeep.copyToArrayBB
		/// </summary>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static ArrayStructureBB makeArrayBB(ArrayStructure as) throws IOException
		public static ArrayStructureBB makeArrayBB(ArrayStructure @as)
		{
			if (@as.GetType().Equals(typeof(ArrayStructureBB))) // no subclasses
			{
				return (ArrayStructureBB)@as;
			}

			StructureMembers sm = @as.StructureMembers.toBuilder(false).build();
			ArrayStructureBB abb = new ArrayStructureBB(sm, @as.Shape);
			ArrayStructureBB.setOffsets(sm);

			StructureDataIterator iter = @as.StructureDataIterator;
			while (iter.hasNext())
			{
				StructureDataDeep.copyToArrayBB(iter.next(), abb);
			}
			return abb;
		}

		/// <summary>
		/// @deprecated use StructureDataDeep.copyToArrayBB
		/// </summary>
		public static ArrayStructureBB copyToArrayBB(StructureData sdata)
		{
			StructureMembers sm = sdata.StructureMembers;
			int size = sm.StructureSize;
			ByteBuffer bb = ByteBuffer.allocate(size); // default is big endian
			ArrayStructureBB abb = new ArrayStructureBB(sm, new int[] { 1 }, bb, 0);
			ArrayStructureBB.setOffsets(sm);
			StructureDataDeep.copyToArrayBB(sdata, abb);
			return abb;
		}

		/// <summary>
		/// Create 1D primitive array of the given size and type
		/// </summary>
		/// <param name="size"> the size of the array to create </param>
		/// <param name="dataType"> dataType of the variable </param>
		/// <returns> primitive array with data read in </returns>
		public static object makePrimitiveArray(int size, DataType dataType)
		{
			object arr = null;

			if ((dataType.getPrimitiveClassType() == typeof(byte)) || (dataType == DataType.CHAR) || (dataType == DataType.OPAQUE) || (dataType == DataType.STRUCTURE))
			{
				arr = new byte[size];

			}
			else if (dataType.getPrimitiveClassType() == typeof(short))
			{
				arr = new short[size];

			}
			else if (dataType.getPrimitiveClassType() == typeof(int))
			{
				arr = new int[size];

			}
			else if (dataType.getPrimitiveClassType() == typeof(long))
			{
				arr = new long[size];

			}
			else if (dataType == DataType.FLOAT)
			{
				arr = new float[size];

			}
			else if (dataType == DataType.DOUBLE)
			{
				arr = new double[size];

			}
			else if (dataType == DataType.STRING)
			{
				arr = new string[size];
			}

			return arr;
		}


		/// <summary>
		/// Create 1D primitive array of the given size and type, fill it with the given value
		/// </summary>
		/// <param name="size"> the size of the array to create </param>
		/// <param name="dataType"> dataType of the variable </param>
		/// <param name="fillValue"> must be Number if dataType.isNumeric(), or String for STRING, byte[] for Structure, or null for
		///        none </param>
		/// <returns> primitive array with data read in </returns>
		public static object makePrimitiveArray(int size, DataType dataType, object fillValue)
		{

			if (dataType.getPrimitiveClassType() == typeof(byte) || (dataType == DataType.CHAR) || dataType == DataType.BOOLEAN)
			{
				byte[] pa = new byte[size];
				byte val;
				if (fillValue is string)
				{
					byte[] bytes = Encoding.UTF8.GetBytes((string)fillValue);
					val = bytes.Length > 0 ? bytes[0] : (byte)0;
				}
				else
				{
					val = Convert.ToByte(fillValue);
				}
				if (val != 0)
				{
					for (int i = 0; i < size; i++)
					{
						pa[i] = val;
					}
				}
				// if (dataType == DataType.CHAR) return convertByteToChar(pa);
				return pa;

			}
			else if (dataType == DataType.OPAQUE)
			{
				return new byte[size];

			}
			else if (dataType.getPrimitiveClassType() == typeof(short))
			{
				short[] pa = new short[size];
				short val = Convert.ToInt16(fillValue);
				if (val != 0)
				{
					for (int i = 0; i < size; i++)
					{
						pa[i] = val;
					}
				}
				return pa;

			}
			else if (dataType.getPrimitiveClassType() == typeof(int))
			{
				int[] pa = new int[size];
				int val = Convert.ToInt32(fillValue);
				if (val != 0)
				{
					for (int i = 0; i < size; i++)
					{
						pa[i] = val;
					}
				}
				return pa;

			}
			else if (dataType.getPrimitiveClassType() == typeof(long))
			{
				long[] pa = new long[size];
				long val = Convert.ToInt64(fillValue);
				if (val != 0)
				{
					for (int i = 0; i < size; i++)
					{
						pa[i] = val;
					}
				}
				return pa;

			}
			else if (dataType == DataType.FLOAT)
			{
				float[] pa = new float[size];
				float val = Convert.ToSingle(fillValue);
				if (val != 0.0)
				{
					for (int i = 0; i < size; i++)
					{
						pa[i] = val;
					}
				}
				return pa;

			}
			else if (dataType == DataType.DOUBLE)
			{
				double[] pa = new double[size];
				double val = Convert.ToDouble(fillValue);
				if (val != 0.0)
				{
					for (int i = 0; i < size; i++)
					{
						pa[i] = val;
					}
				}
				return pa;

			}
			else if (dataType == DataType.STRING)
			{
				string[] pa = new string[size];
				for (int i = 0; i < size; i++)
				{
					pa[i] = (string)fillValue;
				}
				return pa;

			}
			else if (dataType == DataType.STRUCTURE)
			{
				byte[] pa = new byte[size];
				if (fillValue != null)
				{
					byte[] val = (byte[])fillValue;
					int count = 0;
					while (count < size)
					{
						foreach (byte aVal in val)
						{
							pa[count++] = aVal;
						}
					}
				}
				return pa;
			}

			throw new System.InvalidOperationException();
		}

		// convert byte array to char array, assuming UTF-8 encoding

		public static char[] convertByteToCharUTF(byte[] byteArray)
		{
			return Encoding.UTF8.GetChars(byteArray);
		}

		// convert char array to byte array, assuming UTF-8 encoding
		public static byte[] convertCharToByteUTF(char[] from)
		{
			return Encoding.UTF8.GetBytes(from);
		}

		// convert byte array to char array
		public static char[] convertByteToChar(byte[] byteArray)
		{
			int size = byteArray.Length;
			char[] cbuff = new char[size];
			for (int i = 0; i < size; i++)
			{
				cbuff[i] = (char)DataTypes.unsignedByteToShort(byteArray[i]); // NOTE: not Unicode !
			}
			return cbuff;
		}

		// convert char array to byte array
		public static byte[] convertCharToByte(char[] from)
		{
			byte[] to = null;
			if (from != null)
			{
				int size = from.Length;
				to = new byte[size];
				for (int i = 0; i < size; i++)
				{
					to[i] = (byte)from[i]; // LOOK wrong, convert back to unsigned byte ???
				}
			}
			return to;
		}

		// convert byte array to boolean array
		public static bool[] convertByteToBoolean(byte[] from)
		{
			bool[] to = null;
			if (from != null)
			{
				int size = from.Length;
				to = new bool[size];
				for (int i = 0; i < size; i++)
				{
					to[i] = from[i] != 0;
				}
			}
			return to;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static long transferData(Array result, WritableByteChannel channel) throws NetcdfSharp.java.io.IOException
		public static long transferData(NcArray result, WritableByteChannel channel)
		{

			// LOOK should we buffer ??
			DataOutputStream outStream = new DataOutputStream(Channels.newOutputStream(channel));

			IndexIterator iterA = result.IndexIterator;
			Type classType = result.ElementType;

			if (classType == typeof(double))
			{
				while (iterA.hasNext())
				{
					outStream.writeDouble(iterA.DoubleNext);
				}

			}
			else if (classType == typeof(float))
			{
				while (iterA.hasNext())
				{
					outStream.writeFloat(iterA.FloatNext);
				}

			}
			else if (classType == typeof(long))
			{
				while (iterA.hasNext())
				{
					outStream.writeLong(iterA.LongNext);
				}

			}
			else if (classType == typeof(int))
			{
				while (iterA.hasNext())
				{
					outStream.writeInt(iterA.IntNext);
				}

			}
			else if (classType == typeof(short))
			{
				while (iterA.hasNext())
				{
					outStream.writeShort(iterA.ShortNext);
				}

			}
			else if (classType == typeof(char))
			{
				while (iterA.hasNext())
				{
					outStream.writeChar(iterA.CharNext);
				}

			}
			else if (classType == typeof(byte))
			{
				while (iterA.hasNext())
				{
					outStream.writeByte(iterA.ByteNext);
				}

			}
			else if (classType == typeof(bool))
			{
				while (iterA.hasNext())
				{
					outStream.writeBoolean(iterA.BooleanNext);
				}

			}
			else
			{
				throw new System.NotSupportedException("Class type = " + classType.FullName);
			}

			return 0;
		}

		// section reading for member data

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public static NetcdfSharp.ucar.ma2.Array readSection(ParsedSectionSpec cer) throws IOException, InvalidRangeException
		public static NetcdfSharp.ucar.ma2.NcArray readSection(ParsedSectionSpec cer)
		{
			Variable inner = null;
			List<NcRange> totalRanges = new List<NcRange>();
			ParsedSectionSpec current = cer;
			while (current != null)
			{
				totalRanges.AddRange(current.section.Ranges);
				inner = current.v;
				current = current.child;
			}
			// assert inner != null;

			Section total = new Section(totalRanges);
			NcArray result = NcArray.factory(inner.DataType, total.Shape);

			// must be a Structure
			Structure outer = (Structure)cer.v;
			Structure outerSubset = outer.select(cer.child.v.ShortName); // allows IOSPs to optimize for this case
			ArrayStructure outerData = (ArrayStructure)outerSubset.read(cer.section);
			extractSection(cer.child, outerData, result.IndexIterator);
			return result;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private static void extractSection(ParsedSectionSpec child, ArrayStructure outerData, IndexIterator to) throws IOException, InvalidRangeException
		private static void extractSection(ParsedSectionSpec child, ArrayStructure outerData, IndexIterator to)
		{
			long wantNelems = child.section.computeSize();

			StructureMembers.Member m = outerData.findMember(child.v.ShortName);
			for (int recno = 0; recno < outerData.Size; recno++)
			{
				NcArray innerData = outerData.getArray(recno, m);

				if (child.child == null)
				{ // inner variable
					if (wantNelems != innerData.Size)
					{
						innerData = innerData.section(child.section.Ranges);
					}
					MAMath.copy(child.v.DataType, innerData.IndexIterator, to);

				}
				else
				{ // not an inner variable - must be an ArrayStructure

					if (innerData is ArraySequence)
					{
						extractSectionFromSequence(child.child, (ArraySequence)innerData, to);
					}
					else
					{
						if (wantNelems != innerData.Size)
						{
							innerData = sectionArrayStructure(child, (ArrayStructure)innerData, m);
						}
						extractSection(child.child, (ArrayStructure)innerData, to);
					}
				}
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private static void extractSectionFromSequence(ParsedSectionSpec child, ArraySequence outerData, IndexIterator to) throws IOException
		private static void extractSectionFromSequence(ParsedSectionSpec child, ArraySequence outerData, IndexIterator to)
		{
			StructureDataIterator sdataIter = outerData.StructureDataIterator;
			while (sdataIter.hasNext())
			{
				StructureData sdata = sdataIter.next();
				StructureMembers.Member m = outerData.findMember(child.v.ShortName);
				NcArray innerData = sdata.getArray(child.v.ShortName);
				MAMath.copy(m.DataType, innerData.IndexIterator, to);
			}
		}

		// LOOK could be used in createView ??
		private static ArrayStructure sectionArrayStructure(ParsedSectionSpec child, ArrayStructure innerData, StructureMembers.Member m)
		{
			StructureMembers membersw = m.StructureMembers.toBuilder(false).build(); // no data arrays get propagated
			ArrayStructureW result = new ArrayStructureW(membersw, child.section.Shape);

			int count = 0;
			Section.Iterator iter = child.section.getIterator(child.v.Shape);
			while (iter.hasNext())
			{
				int recno = iter.next(null);
				StructureData sd = innerData.getStructureData(recno);
				result.setStructureData(sd, count++);
			}

			return result;
		}

	}

}
