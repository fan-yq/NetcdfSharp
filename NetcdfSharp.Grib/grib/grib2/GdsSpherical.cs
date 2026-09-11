// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using GdsHorizCoordSys = NetcdfSharp.ucar.nc2.grib.GdsHorizCoordSys;

	/// <summary>
	/// Fake implementation of Grib2Gds template 50, to prevent exception.
	/// 
	/// @author caron
	/// @since 9/15/2014
	/// </summary>
	public class GdsSpherical : Grib2Gds
	{

	  private int j, k, m, type, mode;

	  protected internal GdsSpherical(byte[] data, int template) : base(data)
	  {
		this.template = template;

		j = getOctet4(15);
		k = getOctet4(19);
		m = getOctet4(23);
		type = getOctet(27);
		mode = getOctet(28);
	  }

	  public override GdsHorizCoordSys makeHorizCoordSys()
	  {
		return null;
	  }

	  public override void testHorizCoordSys(Formatter f)
	  {

	  }
	}

}
