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
	using NetcdfSharp.ucar.ma2;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask;

	/// <summary>
	/// A ProxyReader for logical sections of a Variable.
	/// 
	/// @author caron </summary>
	/// <seealso cref="Variable.section(Section subsection)"/>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable class SectionReader implements ProxyReader
	internal class SectionReader : ProxyReader
	{
	  private readonly Section orgSection; // section of the original
	  private readonly Variable orgClient;

	  // section must be filled
	  internal SectionReader(Variable orgClient, Section section)
	  {
		this.orgClient = orgClient;
		this.orgSection = section.Immutable ? section : new Section(section.Ranges);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public Array reallyRead(Variable client, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException
	  public virtual NcArray reallyRead(Variable client, CancelTask cancelTask)
	  {
		try
		{
		  return orgClient._read(orgSection);
		}
		catch (InvalidRangeException e)
		{
		  try
		  {
			orgClient._read(orgSection); // debug
		  }
		  catch (InvalidRangeException e1)
		  {
			Console.WriteLine(e1.ToString());
			Console.Write(e1.StackTrace);
		  }
		  throw new Exception(e.Message);
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override public Array reallyRead(Variable client, Section section, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws IOException, InvalidRangeException
	  public virtual NcArray reallyRead(Variable client, Section section, CancelTask cancelTask)
	  {
		Section want = orgSection.compose(section);
		return orgClient._read(want);
	  }

	}

}
