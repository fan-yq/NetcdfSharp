// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/ma2/TestString.java


using System;
using NetcdfSharp.ucar.ma2;
using Xunit;

namespace NetcdfSharp.ucar.ma2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.ma2.TestString - char array get/set and string access.
	/// </summary>
	public class Ma2StringTests
	{
		private const int m = 4;
		private const int n = 3;
		private static readonly int[] sA = { m, n };

		private ArrayChar CharArray()
		{
			return new ArrayChar(sA);
		}

		[Fact]
		public void TestRegular()
		{
			ArrayChar A = CharArray();
			NcIndex ima = A.Index;

			// write
			for (int i = 0; i < m; i++)
			{
				ima.set0(i);
				for (int j = 0; j < n; j++)
				{
					ima.set1(j);
					A.setChar(ima, 'a');
				}
			}

			// read
			for (int i = 0; i < m; i++)
			{
				ima.set0(i);
				for (int j = 0; j < n; j++)
				{
					ima.set1(j);
					Assert.Equal('a', A.getChar(ima));
				}
			}
		}

		[Fact]
		public void TestString()
		{
			ArrayChar A = CharArray();
			NcIndex ima = A.Index;

			// write
			for (int i = 0; i < m; i++)
			{
				ima.set0(i);
				for (int j = 0; j < n; j++)
				{
					ima.set1(j);
					A.setChar(ima, 'a');
				}
			}

			// read as string
			for (int i = 0; i < m; i++)
			{
				ima.set0(i);
				Assert.Equal("aaa", A.getString(ima));
			}
		}

		[Fact]
		public void TestStringPutGet()
		{
			ArrayChar A = CharArray();
			NcIndex ima = A.Index;

			// write
			ima.set0(0);
			A.setString(ima, "hiy");
			ima.set0(1);
			A.setString(ima, "there");
			ima.set0(2);
			A.setString(ima, "yo");
			ima.set0(3);
			A.setString(ima, "I");

			// read
			ima.set0(0);
			Assert.Equal("hiy", A.getString(ima));
			ima.set0(1);
			Assert.Equal("the", A.getString(ima));
			ima.set0(2);
			Assert.Equal("yo", A.getString(ima));
			ima.set0(3);
			Assert.Equal("I", A.getString(ima));
		}
	}
}
