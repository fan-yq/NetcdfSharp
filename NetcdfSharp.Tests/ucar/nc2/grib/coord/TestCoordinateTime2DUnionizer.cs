// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/coord/TestCoordinateTime2DUnionizer.java


using System.Collections.Generic;
using NetcdfSharp.ucar.nc2.grib.coord;
using NetcdfSharp.ucar.nc2.grib.grib1;
using NetcdfSharp.ucar.nc2.time;
using NetcdfSharp.ucar.nc2.util;
using Formatter = NetcdfSharp.java.util.Formatter;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.coord
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.coord.TestCoordinateTime2DUnionizer - 2D time coordinate unionization.
	/// </summary>
	public class TestCoordinateTime2DUnionizer
	{
		private static readonly int code = 0;
		private static readonly CalendarPeriod timeUnit = CalendarPeriod.of("1 hour");
		private static readonly CalendarDate startDate = CalendarDate.parseISOformat(null, "1970-01-01T00:00:00");

		[Fact]
		public void testOrthogonalization()
		{
			IList<CoordinateTimeAbstract> coords = new List<CoordinateTimeAbstract>();
			coords.Add(makeTimeCoordinate(startDate, 12, 6));
			coords.Add(makeTimeCoordinate(startDate, 4, 24));

			CoordinateTimeAbstract result = CoordinateTime2DUnionizer<Grib1Record>.testOrthogonal(coords);

			Formatter f = new Formatter();
			f.format("Original%n");
			foreach (CoordinateTimeAbstract coord in coords)
			{
				coord.showInfo(f, new Indent(2));
			}

			f.format("%nResult%n");
			if (result == null)
			{
				f.format("not orthogonal");
			}
			else
			{
				result.showInfo(f, new Indent(2));
			}

			Assert.Null(result);
		}

		private CoordinateTime makeTimeCoordinate(CalendarDate refDate, int size, int spacing)
		{
			IList<int> offsetSorted = new List<int>();
			for (int i = 0; i < size; i++)
			{
				offsetSorted.Add(i * spacing);
			}
			return new CoordinateTime(code, timeUnit, refDate, offsetSorted, null);
		}

		[Fact]
		public void testMakeCoordinate2D()
		{
			IList<CoordinateTime2D> coords = new List<CoordinateTime2D>();
			coords.Add(makeTimeCoordinate2D(12, 6));
			coords.Add(makeTimeCoordinate2D(4, 24));

			int count = 0;
			Formatter f = new Formatter();
			f.format("Original%n");
			foreach (CoordinateTime2D coord in coords)
			{
				f.format("CoordinateTime2D %d%n", count++);
				coord.showInfo(f, new Indent(2));
				f.format("%n%n");
			}

			f.format("Unionize%n");
			foreach (CoordinateTime2D coord in coords)
			{
				f.format("CoordinateTime2D %d%n", count++);
				CoordinateTime2D result = testUnionizer(coord);
				Assert.NotNull(result);
				result.showInfo(f, new Indent(2));
				f.format("%n%n");
				Assert.True(result.Orthogonal);
			}
		}

		[Fact]
		public void testCoordinateUnionizer()
		{
			Formatter f = new Formatter();
			f.format("Original CoordinateTime2D:%n");

			IList<CoordinateTime2D> coord2Ds = new List<CoordinateTime2D>();
			for (int i = 5; i < 15; i += 2)
			{
				CoordinateTime2D coord2D = makeTimeCoordinate2D(i, 3);
				coord2Ds.Add(coord2D);
				coord2D.showInfo(f, new Indent(2));
			}

			CoordinateTime2DUnionizer<Grib1Record> unionizer = new CoordinateTime2DUnionizer<Grib1Record>(false, timeUnit, code, false, null);
			foreach (CoordinateTime2D coord2D in coord2Ds)
			{
				unionizer.addAll(coord2D);
			}
			unionizer.finish();
			CoordinateTime2D result = (CoordinateTime2D)unionizer.Coordinate;

			f.format("%nUnionized Result:%n");
			result.showInfo(f, new Indent(2));
			Assert.True(result.Orthogonal);
		}

		private CoordinateTime2D testUnionizer(CoordinateTime2D coord2D)
		{
			CoordinateTime2DUnionizer<Grib1Record> unionizer = new CoordinateTime2DUnionizer<Grib1Record>(false, timeUnit, code, false, null);
			unionizer.addAll(coord2D);
			unionizer.finish();
			return (CoordinateTime2D)unionizer.Coordinate;
		}

		private CoordinateTime2D makeTimeCoordinate2D(int nruns, int ntimes)
		{
			CoordinateRuntime.Builder1 runBuilder = new CoordinateRuntime.Builder1(timeUnit);
			IDictionary<object, CoordinateBuilderImpl<Grib1Record>> timeBuilders = new Dictionary<object, CoordinateBuilderImpl<Grib1Record>>();

			IList<CoordinateTime2D.Time2D> vals = new List<CoordinateTime2D.Time2D>(nruns * ntimes);
			for (int j = 0; j < nruns; j++)
			{
				CalendarDate runDate = startDate.add(j, CalendarPeriod.Field.Hour);
				for (int i = 0; i < ntimes; i++)
				{
					CoordinateTime2D.Time2D time2D = new CoordinateTime2D.Time2D(runDate, i, null);
					vals.Add(time2D);

					runBuilder.add(time2D.refDate);
					CoordinateBuilderImpl<Grib1Record> timeBuilder;
					if (!timeBuilders.TryGetValue(time2D.refDate, out timeBuilder))
					{
						timeBuilder = new CoordinateTime.Builder1(null, code, timeUnit, time2D.getRefDate());
						timeBuilders[time2D.refDate] = timeBuilder;
					}
					timeBuilder.add(time2D.time);
				}
			}

			CoordinateRuntime runCoord = (CoordinateRuntime)runBuilder.finish();

			IList<Coordinate> times = new List<Coordinate>(runCoord.Size);
			for (int idx = 0; idx < runCoord.Size; idx++)
			{
				long runtime = runCoord.getRuntime(idx);
				CoordinateBuilderImpl<Grib1Record> timeBuilder = timeBuilders[runtime];
				times.Add(timeBuilder.finish());
			}

			((List<CoordinateTime2D.Time2D>)vals).Sort();

			return new CoordinateTime2D(code, timeUnit, vals, runCoord, times, null);
		}
	}
}
