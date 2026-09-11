// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System.Collections.Generic;

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using TimeCoordIntvValue = NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvValue;

	internal class GribTimeCoordIntervalUtils
	{
	  /// <summary>
	  /// Generate values for 1-D time coordinate variable given an ordered set of time coordinate intervals
	  /// (supports datasets with 2D run/forecast time),
	  /// ensures that resulting values are increasing in a strictly-monotonic manner.
	  /// 
	  /// Assumes that sets of intervals are always sorted by
	  /// end point then starting point. That sorting scheme is implemented in
	  /// NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvValue.compareTo(o) -- 21 Dec 2022.
	  /// </summary>
	  /// <param name="timeIntervals"> given list of time coord intervals from which to calculate time coordinate values </param>
	  /// <param name="timeCoordValues"> array for storing generated time coord values, 1D array that may represent a 2D data array
	  ///        (run/forecast) </param>
	  /// <param name="bounds"> array for storing interval bounds, may be null if bounds are loaded elsewhere. </param>
	  /// <param name="runOffsetIndex"> </param>
	  /// <param name="timeUnitValue"> time unit </param>
	  /// <param name="time2D_Offset"> </param>
	  /// <returns> the index into the timeCoordValues array following the last value written </returns>
	  public static int generateTimeCoordValuesFromTimeCoordIntervals(IList<TimeCoordIntvValue> timeIntervals, double[] timeCoordValues, double[] bounds, int runOffsetIndex, int timeUnitValue, int time2D_Offset)
	  {
		int dataIndex = runOffsetIndex;
		int boundsIndex = runOffsetIndex * 2;
		double[] currentBounds = new double[2];
		double[] prevBounds = new double[2];
		foreach (TimeCoordIntvValue tinv in timeIntervals)
		{
		  currentBounds[0] = timeUnitValue * tinv.Bounds1 + time2D_Offset;
		  currentBounds[1] = timeUnitValue * tinv.Bounds2 + time2D_Offset;
		  // Use end-point of current interval as initial guess for current time coordinate value
		  timeCoordValues[dataIndex] = currentBounds[1];
		  if (dataIndex >= runOffsetIndex + 1)
		  {
			// Check that time coordinate values are increasing in a strictly-monotonic manner
			// (as required by CF conventions). If not strictly-monotonic ...
			if (timeCoordValues[dataIndex] <= timeCoordValues[dataIndex - 1])
			{
			  if (dataIndex >= runOffsetIndex + 2)
			  {
				if (timeCoordValues[dataIndex - 2] <= currentBounds[0])
				{
				  // Change previous time coordinate value to mid-point between
				  // current time interval start and end values.
				  timeCoordValues[dataIndex - 1] = (currentBounds[1] - currentBounds[0]) / 2.0 + currentBounds[0];
				}
				else
				{
				  // Or change previous time coordinate value to mid-point between
				  // current time interval end value and the time coord value from two steps back.
				  timeCoordValues[dataIndex - 1] = (currentBounds[1] - timeCoordValues[dataIndex - 2]) / 2.0 + timeCoordValues[dataIndex - 2];
				}
			  }
			  else
			  {
				timeCoordValues[dataIndex - 1] = (prevBounds[1] - prevBounds[0]) / 2.0 + prevBounds[0];
			  }
			}
		  }
		  if (bounds != null)
		  {
			bounds[boundsIndex++] = currentBounds[0];
			bounds[boundsIndex++] = currentBounds[1];
		  }
		  prevBounds[0] = currentBounds[0];
		  prevBounds[1] = currentBounds[1];
		  dataIndex++;
		}
		return dataIndex;
	  }
	}

}
