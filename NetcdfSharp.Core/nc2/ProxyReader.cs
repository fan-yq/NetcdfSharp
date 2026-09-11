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
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using InvalidRangeException = NetcdfSharp.ucar.ma2.InvalidRangeException;
	using Section = NetcdfSharp.ucar.ma2.Section;
	using CancelTask = NetcdfSharp.ucar.nc2.util.CancelTask; // ??

	/// <summary>
	/// Something that knows how to read the data for a Variable.
	/// 
	/// @author caron
	/// </summary>
	public interface ProxyReader
	{

	  /// <summary>
	  /// Read all the data for a Variable.
	  /// </summary>
	  /// <param name="client"> the client Variable </param>
	  /// <param name="cancelTask"> user may cancel </param>
	  /// <returns> memory resident Array containing the data. Will have same shape as the Variable. </returns>
	  /// <exception cref="IOException"> on error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: NetcdfSharp.ucar.ma2.Array reallyRead(Variable client, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws NetcdfSharp.java.io.IOException;
	  NcArray reallyRead(Variable client, CancelTask cancelTask);

	  /// <summary>
	  /// Read a section of the data for a Variable.
	  /// </summary>
	  /// <param name="client"> the client Variable </param>
	  /// <param name="section"> the section of data to read. </param>
	  /// <param name="cancelTask"> user may cancel
	  /// </param>
	  /// <returns> memory resident Array containing the data. Will have same shape as the Section. </returns>
	  /// <exception cref="IOException"> on error </exception>
	  /// <exception cref="NetcdfSharp.ucar.ma2.InvalidRangeException"> if section has incorrect rank or illegal shape. </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: NetcdfSharp.ucar.ma2.Array reallyRead(Variable client, NetcdfSharp.ucar.ma2.Section section, NetcdfSharp.ucar.nc2.util.CancelTask cancelTask) throws IOException, NetcdfSharp.ucar.ma2.InvalidRangeException;
	  NcArray reallyRead(Variable client, Section section, CancelTask cancelTask);
	}

}
