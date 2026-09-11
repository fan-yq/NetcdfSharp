// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/coord/TestCoordinate.java


using System.Collections.Generic;
using NetcdfSharp.ucar.nc2.time;
using NetcdfSharp.ucar.nc2.util;
using Formatter = NetcdfSharp.java.util.Formatter;

namespace NetcdfSharp.ucar.nc2.grib.coord
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.coord.TestCoordinate - a simple Coordinate implementation used by the tests.
	/// </summary>
	public class TestCoordinate : Coordinate
	{
		private readonly IList<object> vals;
		private readonly Coordinate.Type type = Coordinate.Type.vert;

		public TestCoordinate(int nvals)
		{
			vals = new List<object>(nvals);
			for (int i = 0; i < nvals; i++)
			{
				vals.Add(i);
			}
		}

		public static Coordinate factory(int nvals, Coordinate.Type type)
		{
			CalendarPeriod period = CalendarPeriod.of("1 hour");
			if (type == Coordinate.Type.runtime)
			{
				IList<long> cd = new List<long>(nvals);
				for (int i = 0; i < nvals; i++)
				{
					cd.Add(CalendarDate.of(null, 1953, 11, i + 1, 9, i + 1, 0).Millis);
				}
				return new CoordinateRuntime(cd, period);
			}
			if (type == Coordinate.Type.time)
			{
				IList<int> vals2 = new List<int>(nvals);
				for (int i = 0; i < nvals; i++)
				{
					vals2.Add(i);
				}
				return new CoordinateTime(0, period, null, vals2, null);
			}
			if (type == Coordinate.Type.vert)
			{
				IList<VertCoordValue> vert = new List<VertCoordValue>(nvals);
				for (int i = 0; i < nvals; i++)
				{
					vert.Add(new VertCoordValue((double)(i + 1), (double)(i + 2)));
				}
				return new CoordinateVert(1, new VertCoordType(11, "m", null, true), vert); // random vert unit
			}
			return null;
		}

		public void showInfo(Formatter info, Indent indent)
		{
		}

		public void showCoords(Formatter info)
		{
		}

		public Counters calcDistributions()
		{
			return null;
		}

		public IList<object> Values
		{
			get { return vals; }
		}

		public object getValue(int idx)
		{
			return vals[idx];
		}

		public int getIndex(object val)
		{
			return vals.IndexOf(val);
		}

		public int Size
		{
			get { return vals.Count; }
		}

		public int NCoords
		{
			get { return Size; }
		}

		public int estMemorySize()
		{
			return 0;
		}

		public int Code
		{
			get { return 0; }
		}

		public Coordinate.Type getType()
		{
			return type;
		}

		public string Name
		{
			get { return "testVals for type " + type.ToString(); }
		}

		public string Unit
		{
			get { return null; }
		}
	}
}
