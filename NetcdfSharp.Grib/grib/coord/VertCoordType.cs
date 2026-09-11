// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

namespace NetcdfSharp.ucar.nc2.grib.coord
{

	/// <summary>
	/// Encapsulate the semantics in GRIB level types (Grib1 table 3, Grib2 code table 4.5).
	/// </summary>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class VertCoordType
	public class VertCoordType
	{
	  private readonly int code;
	  private readonly string desc;
	  private readonly string abbrev;
	  private readonly string units;
	  private readonly string datum;
	  private readonly bool isPositiveUp;
	  private readonly bool isLayer;

	  // LOOK for Grib2Utils - CHANGE THIS
	  public VertCoordType(int code, string units, string datum, bool isPositiveUp)
	  {
		this.code = code;
		this.desc = null;
		this.abbrev = null;
		this.units = units;
		this.datum = datum;
		this.isPositiveUp = isPositiveUp;
		this.isLayer = false;
	  }

	  public VertCoordType(int code, string desc, string abbrev, string units, string datum, bool isPositiveUp, bool isLayer)
	  {
		this.code = code;
		this.desc = desc;
		this.abbrev = abbrev;
		this.units = units;
		this.datum = datum;
		this.isPositiveUp = isPositiveUp;
		this.isLayer = isLayer;
	  }

	  public virtual int Code
	  {
		  get
		  {
			return code;
		  }
	  }

	  public virtual string Desc
	  {
		  get
		  {
			return desc;
		  }
	  }

	  public virtual string Abbrev
	  {
		  get
		  {
			return abbrev;
		  }
	  }

	  public virtual string Units
	  {
		  get
		  {
			return units;
		  }
	  }

	  public virtual string Datum
	  {
		  get
		  {
			return datum;
		  }
	  }

	  public virtual bool PositiveUp
	  {
		  get
		  {
			return isPositiveUp;
		  }
	  }

	  public virtual bool VerticalCoordinate
	  {
		  get
		  {
			return !string.ReferenceEquals(Units, null);
		  }
	  }

	  public virtual bool Layer
	  {
		  get
		  {
			return isLayer;
		  }
	  }
	}


}
