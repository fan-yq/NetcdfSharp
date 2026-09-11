// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2
{
	using IOServiceProvider = NetcdfSharp.ucar.nc2.iosp.IOServiceProvider;
	using AbstractIOServiceProvider = NetcdfSharp.ucar.nc2.iosp.AbstractIOServiceProvider;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// To allow subclassing of NetcdfFile.
	/// Expert use only
	/// 
	/// @author caron
	/// @since 10/29/2014 </summary>
	/// @deprecated Use NetcdfFile.builder(). Remove in v6 
	[Obsolete("Use NetcdfFile.builder(). Remove in v6")]
	public class NetcdfFileSubclass : NetcdfFile
	{

	  public NetcdfFileSubclass()
	  {
		// The Java original initializes rootGroup as a field initializer; the C# port moved that
		// into InitializeInstanceFields() which the empty constructor does not run. Build it here so
		// in-memory model construction (used by tests) has a root group, matching Java semantics.
		if (RootGroup == null)
		{
			setRootGroup(Group.builder().setNcfile(this).setName("").build());
		}
	  }

	  public NetcdfFileSubclass(IOServiceProvider iosp, string location)
	  {
		this.iosp = (AbstractIOServiceProvider) iosp;
		this.location = location;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfFileSubclass(NetcdfSharp.ucar.nc2.iosp.IOServiceProvider spi, NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, String location, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public NetcdfFileSubclass(IOServiceProvider spi, RandomAccessFile raf, string location, CancelTask cancelTask) : base(spi, raf, location, cancelTask)
	  {
		if (raf == null)
		{
		  this.location = location;
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public NetcdfFileSubclass(String iospClassName, Object iospParam, String location, int buffer_size, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws IOException, IllegalAccessException, ClassNotFoundException, InstantiationException
	  public NetcdfFileSubclass(string iospClassName, object iospParam, string location, int buffer_size, CancelTask cancelTask) : base(iospClassName, iospParam, location, buffer_size, cancelTask)
	  {

	  }
	}

}
