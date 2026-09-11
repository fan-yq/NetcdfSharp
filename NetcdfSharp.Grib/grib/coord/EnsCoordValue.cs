// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

namespace NetcdfSharp.ucar.nc2.grib.coord
{
	using Misc = NetcdfSharp.ucar.nc2.util.Misc;

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class EnsCoordValue implements Comparable<EnsCoordValue>
	public class EnsCoordValue : IComparable<EnsCoordValue>, IComparable
	{
	  private readonly int code;
	  private readonly int ensMember;
	  private readonly int ensNumber;

	  public EnsCoordValue(int code, int ensMember) : this(code, ensMember, 0)
	  {
	  }

	  public EnsCoordValue(int code, int ensMember, int ensNumber)
	  {
		this.code = code;
		this.ensMember = ensMember;
		this.ensNumber = ensNumber;
	  }

	  public virtual int Code
	  {
		  get
		  {
			return code;
		  }
	  }

	  public virtual int EnsMember
	  {
		  get
		  {
			return ensMember;
		  }
	  }

	  public virtual int EnsNumber
	  {
		  get
		  {
			return ensNumber;
		  }
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override public int compareTo(@Nonnull EnsCoordValue o)
	  public virtual int CompareTo(EnsCoordValue o)
	  {
		int r = JavaLangCompat.IntCompare(code, o.code);
		if (r != 0)
		{
		  return r;
		}
		r = JavaLangCompat.IntCompare(ensNumber, o.ensNumber);
		if (r != 0)
		{
		  return r;
		}
		return JavaLangCompat.IntCompare(ensMember, o.ensMember);
	  }

	  public virtual int CompareTo(object o)
	  {
		return CompareTo((EnsCoordValue) o);
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
		EnsCoordValue that = (EnsCoordValue) o;
		return code == that.code && ensMember == that.ensMember && ensNumber == that.ensNumber;
	  }

	  public override int GetHashCode()
	  {
		int result = 17;
		result += 31 * ensMember;
		result += 31 * code;
		result += 31 * ensNumber;
		return result;
	  }

	  public override string ToString()
	  {
		using (Formatter @out = new Formatter())
		{
		  @out.format("(%d %d %d)", code, ensMember, ensNumber);
		  return @out.ToString();
		}
	  }
	}

}
