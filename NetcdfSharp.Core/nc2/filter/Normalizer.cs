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
	using Variable = NetcdfSharp.ucar.nc2.Variable;
	using Enhance = NetcdfSharp.ucar.nc2.dataset.NetcdfDataset.Enhance;
	using VariableDS = NetcdfSharp.ucar.nc2.dataset.VariableDS;

	public class Normalizer : Enhancement
	{

	  private readonly ScaleOffset scaleOffset;
	  private readonly double minimum;
	  private readonly double range; // maximum - minimum

	  public static Normalizer createFromVariable(VariableDS var)
	  {
		try
		{
		  NcArray arr = var.read();
		  DataType type = var.DataType;
		  return createFromArray(arr, type);
		}
		catch (IOException)
		{
		  return new Normalizer(0.0, 1.0, var.DataType);
		}
	  }

	  public static Normalizer createFromArray(NcArray arr, DataType type)
	  {
		SummaryStatistics statistics = calculationHelper(arr);
		if ((statistics.getMax() - statistics.getMin()) == 0)
		{
		  return new Normalizer(0.0, 1.0, type);
		}
		return new Normalizer(statistics.getMin(), (statistics.getMax() - statistics.getMin()), type);
	  }

	  private Normalizer(double minimum, double range, DataType type)
	  {
		this.minimum = minimum;
		this.range = range;
		IDictionary<string, object> props = new Dictionary<string, object>();
		props["offset"] = minimum;
		props["scale"] = 1 / range;
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

	  public virtual double Minimum
	  {
		  get
		  {
			return minimum;
		  }
	  }

	  public virtual double Range
	  {
		  get
		  {
			return range;
		  }
	  }

	  public class Provider : EnhancementProvider
	  {

		internal const string ATTRIBUTE_NAME = "normalize";

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
		  return Normalizer.createFromVariable(var);
		}

	  }

	}

}
