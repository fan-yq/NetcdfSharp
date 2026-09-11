// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.coord
{

	/// <summary>
	/// Builds Coordinates by finding distinct values.
	/// 
	/// @author caron
	/// @since 11/27/13
	/// </summary>
	public interface CoordinateBuilder<T>
	{

	  void addRecord(T r);

	  object extract(T gr);

	  // must sort values; Coordinate must be immutable
	  Coordinate makeCoordinate(IList<object> values);

	  Coordinate finish();

	  int getIndex(T gr);

	  Coordinate Coordinate {get;}

	  void addAll(Coordinate coord);

	  void addAll(IList<object> coords);

	  internal interface TwoD<T1>
	  {
		int[] getCoordIndices(T1 gr);
	  }
	}

}
