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
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;

	/// <summary>
	/// Reuse immutable calendar date objects.
	/// LOOK: This optimization could go away.
	/// 
	/// @author caron
	/// @since 4/3/2015
	/// </summary>
	public class CalendarDateFactory
	{
	  private IDictionary<long, CalendarDate> map;

	  public CalendarDateFactory(CoordinateRuntime master)
	  {
		map = new Dictionary<long, CalendarDate>(master.Size * 2);
		foreach (object valo in master.Values)
		{
		  CalendarDate cd = CalendarDate.of(((long?) valo).Value);
		  map[cd.Millis] = cd;
		}
	  }

	  public virtual CalendarDate get(CalendarDate cd)
	  {
		CalendarDate cdc;
		map.TryGetValue(cd.Millis, out cdc);
		if (cdc != null)
		{
		  return cdc;
		}
		map[cd.Millis] = cd;
		return cd;
	  }
	}

}
