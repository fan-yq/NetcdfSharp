// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.filter
{
	using CDM = NetcdfSharp.ucar.nc2.constants.CDM;

	using SummaryStatistics = NetcdfSharp.org.apache.commons.math3.stat.descriptive.SummaryStatistics;
	using NcArray = NetcdfSharp.ucar.ma2.NcArray;
	using DataType = NetcdfSharp.ucar.ma2.DataType;
	using IndexIterator = NetcdfSharp.ucar.ma2.IndexIterator;
	using AttributeContainer = NetcdfSharp.ucar.nc2.AttributeContainer;
	using Enhance = NetcdfSharp.ucar.nc2.dataset.NetcdfDataset.Enhance;
	using VariableDS = NetcdfSharp.ucar.nc2.dataset.VariableDS;

	public class Standardizer : Enhancement
	{

	  private readonly ScaleOffset scaleOffset;
	  private readonly double mean;
	  private readonly double stdDev;

	  public static Standardizer createFromVariable(VariableDS var)
	  {
		try
		{
		  NcArray arr = var.read();
		  DataType type = var.DataType;
		  return createFromArray(arr, type);
		}
		catch (IOException)
		{
		  return new Standardizer(0.0, 1.0, var.DataType);
		}
	  }

	  public static Standardizer createFromArray(NcArray arr, DataType type)
	  {
		SummaryStatistics statistics = calculationHelper(arr);
		if (statistics.getStandardDeviation() == 0)
		{
		  return new Standardizer(0.0, 1.0, type);
		}
		return new Standardizer(statistics.getMean(), statistics.getStandardDeviation(), type);
	  }

	  private Standardizer(double mean, double stdDev, DataType type)
	  {
		this.mean = mean;
		this.stdDev = stdDev;
		IDictionary<string, object> props = new Dictionary<string, object>();
		props["offset"] = mean;
		props["scale"] = 1 / stdDev;
		props["dtype"] = type;
		scaleOffset = new ScaleOffset(props);
	  }

	  private static SummaryStatistics calculationHelper(NcArray arr)
	  {
		SummaryStatistics sumStat = new SummaryStatistics();
		IndexIterator iterArr = arr.IndexIterator;
		while (iterArr.hasNext())
		{
		  object value = iterArr.ObjectNext;
		  if (!double.IsNaN(Convert.ToDouble(value)))
		  {
			sumStat.addValue(Convert.ToDouble(value));
		  }
		}
		return sumStat;
	  }

	  public virtual NcArray convert(NcArray arr)
	  {
		return scaleOffset.applyScaleOffset(arr);
	  }

	  public virtual double convert(double val)
	  {
		return scaleOffset.applyScaleOffset(val);
	  }

	  public virtual double Mean
	  {
		  get
		  {
			return mean;
		  }
	  }

	  public virtual double StdDev
	  {
		  get
		  {
			return stdDev;
		  }
	  }



	  public class Provider : EnhancementProvider
	  {

		internal const string ATTRIBUTE_NAME = "standardize";

		public virtual string AttributeName
		{
			get
			{
			  return ATTRIBUTE_NAME;
			}
		}

		public virtual bool appliesTo(ISet<Enhance> enhance, DataType dt)
		{
		  return dt.isFloatingPoint();
		}

		public virtual Enhancement create(VariableDS var)
		{
		  return createFromVariable(var);
		}
	  }
	}


}
