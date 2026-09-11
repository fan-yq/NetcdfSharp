// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/iosp/TestIndexChunker.java
// The tiled-chunker cases in the Java original only print chunks and assert nothing; they are
// omitted here.


using System;
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2.iosp;
using Xunit;

namespace NetcdfSharp.ucar.nc2.iosp
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.iosp.TestIndexChunker - chunking of full and partial sections.
	/// </summary>
	public class IospIndexChunkerTests
	{
		[Fact]
		public void TestFull()
		{
			int[] shape = new int[] { 123, 22, 92, 12 };
			Section section = new Section(shape);
			IndexChunker index = new IndexChunker(shape, section);
			Assert.Equal(section.computeSize(), index.TotalNelems);
			IndexChunker.Chunk chunk = index.next();
			Assert.Equal(section.computeSize(), chunk.Nelems);
			Assert.False(index.hasNext());
		}

		[Fact]
		public void TestPart()
		{
			int[] full = new int[] { 2, 10, 20 };
			int[] part = new int[] { 2, 5, 20 };
			Section section = new Section(part);
			IndexChunker index = new IndexChunker(full, section);
			Assert.Equal(section.computeSize(), index.TotalNelems);
			IndexChunker.Chunk chunk = index.next();
			Assert.Equal(section.computeSize() / 2, chunk.Nelems);
		}
	}
}
