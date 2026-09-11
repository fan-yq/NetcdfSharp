// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

/*
 * Copyright (c) 2021 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.filter
{
	using IO = NetcdfSharp.ucar.nc2.util.IO;

	/// <summary>
	/// Filter implementation of zlib compression.
	/// </summary>
	public class Deflate : Filter
	{
	  private static readonly int MAX_ARRAY_LEN = int.MaxValue - 8;

	  private const string name = "zlib";

	  private const int id = 1;

	  private readonly int clevel; // compression level

	  public Deflate(IDictionary<string, object> properties)
	  {
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final Object levelObj = properties.get("level");
		object levelObj;
		properties.TryGetValue("level", out levelObj);
		if (levelObj == null)
		{
		  this.clevel = 1; // default value
		}
		else if (levelObj is string)
		{
		  this.clevel = int.Parse((string) levelObj);
		}
		else
		{
		  this.clevel = Convert.ToInt32(levelObj);
		}
		validateLevel();
	  }

	  private void validateLevel()
	  {
		if (clevel < 0 || clevel > 9)
		{
		  throw new ArgumentException("Invalid compression level: " + clevel);
		}
	  }

	  public override string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public override int Id
	  {
		  get
		  {
			return id;
		  }
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public byte[] encode(byte[] dataIn) throws NetcdfSharp.java.io.IOException
	  public override byte[] encode(byte[] dataIn)
	  {
		using (MemoryStream os = new MemoryStream(dataIn.Length))
		{
		  using (ZLibStream dos = new ZLibStream(os, clevel == 0 ? CompressionLevel.NoCompression : CompressionLevel.Optimal, true))
		  {
			dos.Write(dataIn, 0, dataIn.Length);
		  }
		  // close everything and return
		  return os.ToArray();
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public byte[] decode(byte[] dataIn) throws NetcdfSharp.java.io.IOException
	  public override byte[] decode(byte[] dataIn)
	  {
		long approxLen = (long) 8 * dataIn.Length;
		int len = approxLen > MAX_ARRAY_LEN ? MAX_ARRAY_LEN : (int) approxLen;
		using (MemoryStream @in = new MemoryStream(dataIn), os = new MemoryStream(len))
		{
		  using (ZLibStream iis = new ZLibStream(@in, CompressionMode.Decompress, true))
		  {
			IO.copyB(iis, os, IO.default_socket_buffersize);
		  }

		  return os.ToArray();
		}
	  }

	  public class Provider : FilterProvider
	  {

		public virtual string Name
		{
			get
			{
			  return name;
			}
		}

		public virtual int Id
		{
			get
			{
			  return id;
			}
		}

		public virtual Filter create(IDictionary<string, object> properties)
		{
		  return new Deflate(properties);
		}
	  }
	}

}
