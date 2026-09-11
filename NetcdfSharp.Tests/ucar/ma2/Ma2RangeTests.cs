// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestRange.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestRange - tests the NcRange class and Section.Iterator.
	/// </summary>
	public class Ma2RangeTests
	{
		[Fact]
		public void TestStride()
		{
			int nx = 1237;
			NcRange r = new NcRange(0, nx - 1, 9);

			Assert.Equal(0, r.first());
			Assert.Equal(9, r.stride());
			Assert.Equal(138, r.length());
			Assert.Equal(1233, r.last());

			Section s = new Section(r, r);
			Section.Iterator iter = s.getIterator(new int[] { nx, nx });
			int[] iterResult = new int[2];
			int count = 0;
			for (int y = r.first(); y <= r.last(); y += r.stride())
			{
				for (int x = r.first(); x <= r.last(); x += r.stride())
				{
					Assert.True(iter.hasNext());
					int iterN = iter.next(iterResult);
					Assert.Equal(y, iterResult[0]);
					Assert.Equal(x, iterResult[1]);
					Assert.Equal(y * nx + x, iterN);
					count++;
				}
			}
			Assert.Equal(138 * 138, count);
		}
	}
}
