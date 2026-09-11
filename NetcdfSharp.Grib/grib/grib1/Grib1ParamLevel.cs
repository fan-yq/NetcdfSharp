// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib1
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;

	/// <summary>
	/// Level information contained in a particular PDS.
	/// WMO Table 3
	/// 
	/// @author caron
	/// @since 1/13/12
	/// </summary>

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib1ParamLevel
	public class Grib1ParamLevel
	{
	  private readonly Grib1Customizer cust;
	  private readonly int levelType; // code Table 3 (octet 10)
	  private readonly float value1;
	  private readonly float value2;

	  /// <summary>
	  /// Allows center specific parsing
	  /// </summary>
	  /// <param name="cust"> customized for this center/subcenter </param>
	  /// <param name="levelType"> the level type </param>
	  /// <param name="value1"> first level value </param>
	  /// <param name="value2"> second level value </param>
	  public Grib1ParamLevel(Grib1Customizer cust, int levelType, float value1, float value2)
	  {
		this.cust = cust;
		this.levelType = levelType;
		this.value1 = value1;
		this.value2 = value2;
	  }

	  /// <summary>
	  /// Implements tables 3 and 3a.
	  /// </summary>
	  /// <param name="cust"> customized for this center/subcenter </param>
	  /// <param name="pds"> the Grib1SectionProductDefinition </param>
	  public Grib1ParamLevel(Grib1Customizer cust, Grib1SectionProductDefinition pds)
	  {
		this.cust = cust;

		// default surface values
		levelType = pds.LevelType;
		int pds11 = pds.LevelValue1;
		int pds12 = pds.LevelValue2;
		int pds1112 = pds11 << 8 | pds12;

		switch (levelType)
		{
		  default:
			value1 = pds11;
			value2 = pds12;
			break;

		  case 20:
			value1 = (float)(pds1112 * 0.01);
			value2 = GribNumbers.MISSING;
			break;

		  case 100:
		  case 103:
		  case 105:
		  case 160:
		  case 126:
		  case 125:
		  case 117:
		  case 115:
		  case 113:
		  case 111:
		  case 109:
			value1 = pds1112;
			value2 = GribNumbers.MISSING;
			break;

		  case 101:
			value1 = pds11 * 10; // convert from kPa to hPa - who uses kPa???
			value2 = pds12 * 10;
			break;

		  case 104:
		  case 106:
			value1 = (pds11 * 100); // convert hm to m
			value2 = (pds12 * 100);
			break;

		  case 107:
		  case 119:
			value1 = (float)(pds1112 * 0.0001);
			value2 = GribNumbers.MISSING;
			break;

		  case 108:
		  case 120:
			value1 = (float)(pds11 * 0.01);
			value2 = (float)(pds12 * 0.01);
			break;

		  case 110:
			value1 = pds11;
			value2 = pds12;
			break;

		  case 112:
			value1 = pds11;
			value2 = pds12;
			break;

		  case 114:
			value1 = 475 - pds11;
			value2 = 475 - pds12;
			break;

		  case 116:
			value1 = pds11;
			value2 = pds12;
			break;

		  case 121:
			value1 = 1100 - pds11;
			value2 = 1100 - pds12;
			break;

		  case 128:
			value1 = (float)(1.1 - (pds11 * 0.001));
			value2 = (float)(1.1 - (pds12 * 0.001));
			break;

		  case 141:
			// value1 = pds11*10; // convert from kPa to hPa - who uses kPa???
			value1 = pds11; // 388 nows says it is hPA
			value2 = 1100 - pds12;
			break;
		}
	  }

	  /// <summary>
	  /// Index number from table 3 (pds octet 10)
	  /// </summary>
	  /// <returns> index </returns>
	  public virtual int LevelType
	  {
		  get
		  {
			return levelType;
		  }
	  }

	  /// <summary>
	  /// gets the 1st value for the level.
	  /// </summary>
	  /// <returns> level value 1 </returns>
	  public virtual float Value1
	  {
		  get
		  {
			return value1;
		  }
	  }

	  /// <summary>
	  /// gets the 2nd value for the level.
	  /// </summary>
	  /// <returns> level value 2 </returns>
	  public virtual float Value2
	  {
		  get
		  {
			return value2;
		  }
	  }

	  public virtual string NameShort
	  {
		  get
		  {
			return cust.getLevelNameShort(levelType);
		  }
	  }

	  public virtual string Description
	  {
		  get
		  {
			return cust.getLevelDescription(levelType);
		  }
	  }
	}


}
