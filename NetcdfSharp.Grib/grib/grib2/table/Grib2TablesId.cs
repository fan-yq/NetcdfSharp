// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class Grib2TablesId
	public class Grib2TablesId
	{
	  public enum Type
	  {
		wmo,
		cfsr,
		gempak,
		gsd,
		kma,
		ncep,
		ndfd,
		mrms,
		nwsDev,
		eccodes
	  }

	  public readonly int center, subCenter, masterVersion, localVersion, genProcessId;

	  public Grib2TablesId(int center, int subCenter, int masterVersion, int localVersion, int genProcessId)
	  {
		this.center = center;
		this.subCenter = subCenter;
		this.masterVersion = masterVersion;
		this.localVersion = localVersion;
		this.genProcessId = genProcessId;
	  }

	  internal virtual bool match(Grib2TablesId id)
	  {
		if (id.center != center)
		{
		  return false; // must match center
		}
		if (subCenter != -1 && id.subCenter != subCenter)
		{
		  return false;
		}
		if (masterVersion != -1 && id.masterVersion != masterVersion)
		{
		  return false;
		}
		if (localVersion != -1 && id.localVersion != localVersion)
		{
		  return false;
		}
		return genProcessId == -1 || id.genProcessId == genProcessId;
	  }

	  public override bool Equals(object o)
	  {
		if (this == o)
		{
		  return true;
		}
		if (o == null || this.GetType() != o.GetType())
		{
		  return false;
		}

		Grib2TablesId that = (Grib2TablesId) o;

		if (center != that.center)
		{
		  return false;
		}
		if (genProcessId != that.genProcessId)
		{
		  return false;
		}
		if (localVersion != that.localVersion)
		{
		  return false;
		}
		if (masterVersion != that.masterVersion)
		{
		  return false;
		}
		return subCenter == that.subCenter;

	  }

	  public override int GetHashCode()
	  {
		int result = center;
		result = 31 * result + subCenter;
		result = 31 * result + masterVersion;
		result = 31 * result + localVersion;
		result = 31 * result + genProcessId;
		return result;
	  }

	  public override string ToString()
	  {
		return MoreObjects.toStringHelper(this).add("center", center).add("subCenter", subCenter).add("masterVersion", masterVersion).add("localVersion", localVersion).add("genProcessId", genProcessId).ToString();
	  }
	}

}
