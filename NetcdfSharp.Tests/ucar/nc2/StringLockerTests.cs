// Ported from Java: netcdf-java cdm/core/src/test/java/ucar/nc2/StringLockerTest.java


using System.Threading;
using NetcdfSharp.ucar.nc2;
using Xunit;

namespace NetcdfSharp.ucar.nc2
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.StringLockerTest - StringLocker thread safety.
	/// </summary>
	public class StringLockerTests
	{
		[Fact]
		public void TestNonconflicting()
		{
			Thread t = new Thread(() =>
			{
				StringLocker locker = new StringLocker();
				locker.control("first");
				locker.control("second");
				locker.release("first");
				locker.release("second");
			});
			t.Start();
			t.Join(500);
			Assert.False(t.IsAlive);
		}

		[Fact]
		public void TestNonconflictingOtherOrder()
		{
			Thread t = new Thread(() =>
			{
				StringLocker locker = new StringLocker();
				locker.control("first");
				locker.control("second");
				locker.release("second");
				locker.release("first");
			});
			t.Start();
			t.Join(500);
			Assert.False(t.IsAlive);
		}
	}
}
