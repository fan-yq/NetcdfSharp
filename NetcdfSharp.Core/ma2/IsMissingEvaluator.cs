// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

namespace NetcdfSharp.ucar.ma2
{
	/// <summary>
	/// A mix-in interface for evaluating if a value is missing.
	/// 
	/// @author John
	/// @since 12/27/12
	/// </summary>
	public interface IsMissingEvaluator
	{

	  /// <summary>
	  /// true if there may be missing data
	  /// </summary>
	  /// <returns> true if there may be missing data </returns>
	  bool hasMissing();

	  /// <summary>
	  /// if val is a missing data value
	  /// </summary>
	  /// <param name="val"> test this value </param>
	  /// <returns> true if val is missing data </returns>
	  bool isMissing(double val);

	}

}
