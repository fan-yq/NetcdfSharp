// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2
{
	using NetcdfSharp.ucar.ma2;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;

	/// <summary>
	/// A ProxyReader for slices.
	/// 
	/// @author caron </summary>
	/// <seealso cref="Variable.slice(int, int)"/>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable class SliceReader implements ProxyReader
	internal class SliceReader : ProxyReader
	{
	  private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(SliceReader));

	  private readonly Variable orgClient;
	  private readonly int sliceDim; // dimension index into original
	  private readonly Section slice; // section of the original

	  internal SliceReader(Variable orgClient, int dim, Section slice)
	  {
		// LOOK could do check that slice is compatible with client

		this.orgClient = orgClient;
		this.sliceDim = dim;
		this.slice = slice;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public Array reallyRead(Variable client, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public virtual NcArray reallyRead(Variable client, CancelTask cancelTask)
	  {
		NcArray data;
		try
		{
		  data = orgClient._read(slice);
		}
		catch (InvalidRangeException e)
		{
		  log.error("InvalidRangeException in slice, var=" + client);
		  throw new System.InvalidOperationException(e.Message);
		}
		data = data.reduce(sliceDim);
		return data;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public Array reallyRead(Variable client, Section section, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws IOException, InvalidRangeException
	  public virtual NcArray reallyRead(Variable client, Section section, CancelTask cancelTask)
	  {
		Section.Builder orgSection = Section.builder().appendRanges(section.Ranges);
		orgSection.insertRange(sliceDim, slice.getRange(sliceDim));
		NcArray data = orgClient._read(orgSection.build());
		data = data.reduce(sliceDim);
		return data;
	  }

	}

}
