// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestSectionBuilder.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestSectionBuilder - Section.Builder range manipulation.
	/// </summary>
	public class Ma2SectionBuilderTests
	{
		[Fact]
		public void SetupSectionBuilder()
		{
			Section.Builder sb = Section.builder();
			// add three ranges
			sb.appendRange(NcRange.make(0, 2));
			sb.appendRange(NcRange.make(1, 6));
			sb.appendRange(NcRange.make(5, 7));
		}

		[Fact]
		public void TestBuilderRemoveN()
		{
			Section.Builder sb = Section.builder();
			// add three ranges
			sb.appendRange(NcRange.make(0, 2));
			sb.appendRange(NcRange.make(1, 6));
			sb.appendRange(NcRange.make(5, 7));

			int numberOfRanges = sb.ranges.Count;
			NcRange last = sb.ranges[numberOfRanges - 1];

			// remove the first n ranges
			int n = 2;
			sb.removeFirst(n);
			Assert.Equal(numberOfRanges - n, sb.ranges.Count);

			// the only thing left should be Range third.
			Assert.Equal(last, sb.ranges[0]);
		}

		[Fact]
		public void TestRemoveLast()
		{
			Section.Builder sb = Section.builder();
			// add three ranges
			sb.appendRange(NcRange.make(0, 2));
			sb.appendRange(NcRange.make(1, 6));
			sb.appendRange(NcRange.make(5, 7));

			int numberOfRanges = sb.ranges.Count;
			NcRange rangeToRemove = sb.ranges[numberOfRanges - 1];

			// make a copy of the original ranges
			System.Collections.Generic.List<NcRange> copy = new System.Collections.Generic.List<NcRange>(sb.ranges);

			// remove the last one
			sb.removeLast();
			Assert.Equal(numberOfRanges - 1, sb.ranges.Count);
			int updatedNumberOfRanges = sb.ranges.Count;

			// Check that only the last one was removed
			for (int index = 0; index < updatedNumberOfRanges - 2; index++)
			{
				Assert.Equal(copy[index], sb.ranges[index]);
			}

			Assert.NotEqual(rangeToRemove, sb.ranges[updatedNumberOfRanges - 1]);
		}
	}
}
