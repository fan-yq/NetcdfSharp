// Ported from Java: netcdf-java grib/src/test/java/ucar/nc2/grib/collection/TestGcMFile.java


using System;
using System.IO;
using NetcdfSharp.ucar.nc2.grib.collection;
using NetcdfSharp.thredds.inventory;
using Xunit;

namespace NetcdfSharp.ucar.nc2.grib.collection
{

	/// <summary>
	/// Port of NetcdfSharp.ucar.nc2.grib.collection.TestGcMFile - GcMFile disk-backed MFile behavior.
	/// </summary>
	public class TestGcMFile
	{
		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(60000)]
		[InlineData(100000)]
		public void ShouldWriteFileToStream(int expectedSize)
		{
			using (TempDir dir = new TempDir())
			{
				FileInfo file = dir.CreateFile(expectedSize);
				GcMFile mFile = new GcMFile(MFiles.create(dir.Path), file.Name, file.LastWriteTimeUtc.Ticks, file.Length, 0);

				Assert.Equal((long)expectedSize, mFile.Length);

				using (MemoryStream outputStream = new MemoryStream())
				{
					mFile.writeToStream(outputStream);
					Assert.Equal(expectedSize, (int)outputStream.Length);
					Assert.Equal(File.ReadAllBytes(file.FullName), outputStream.ToArray());
				}
			}
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(60000)]
		[InlineData(100000)]
		public void ShouldWritePartialFileToStream(int expectedSize)
		{
			using (TempDir dir = new TempDir())
			{
				FileInfo file = dir.CreateFile(expectedSize);
				GcMFile mFile = new GcMFile(MFiles.create(dir.Path), file.Name, file.LastWriteTimeUtc.Ticks, file.Length, 0);

				Assert.Equal((long)expectedSize, mFile.Length);

				int[][] testCases = new int[][]
				{
					new int[] { 0, 0 },
					new int[] { 10, 10 },
					new int[] { 0, (int)mFile.Length },
					new int[] { 0, 100 },
					new int[] { 42, 100 }
				};

				foreach (int[] testCase in testCases)
				{
					int offset = testCase[0];
					int maxBytes = testCase[1];

					using (MemoryStream outputStream = new MemoryStream())
					{
						mFile.writeToStream(outputStream, offset, maxBytes);

						int startPosition = Math.Min(offset, expectedSize);
						int endPosition = Math.Min(offset + maxBytes, expectedSize);

						Assert.Equal(Math.Max(0, endPosition - startPosition), (int)outputStream.Length);

						byte[] all = File.ReadAllBytes(file.FullName);
						byte[] partialFile = new byte[endPosition - startPosition];
						Array.Copy(all, startPosition, partialFile, 0, endPosition - startPosition);
						Assert.Equal(partialFile, outputStream.ToArray());
					}
				}
			}
		}

		[Fact]
		public void ShouldReturnTrueForExistingFile()
		{
			using (TempDir dir = new TempDir())
			{
				FileInfo file = dir.CreateFile(0);
				GcMFile mFile = new GcMFile(MFiles.create(dir.Path), file.Name, file.LastWriteTimeUtc.Ticks, file.Length, 0);
				Assert.True(mFile.exists());
			}
		}

		[Fact]
		public void ShouldReturnFalseForNonExistingFile()
		{
			using (TempDir dir = new TempDir())
			{
				GcMFile mFile = new GcMFile(MFiles.create(dir.Path), "notARealFile", 0, 0, 0);
				Assert.False(mFile.exists());
			}
		}

		[Fact]
		public void ShouldGetInputStream()
		{
			using (TempDir dir = new TempDir())
			{
				FileInfo file = dir.CreateFile(1);
				GcMFile mFile = new GcMFile(MFiles.create(dir.Path), file.Name, file.LastWriteTimeUtc.Ticks, file.Length, 0);
				using (Stream inputStream = mFile.InputStream)
				{
					Assert.NotEqual(-1, inputStream.ReadByte());
				}
			}
		}

		private sealed class TempDir : IDisposable
		{
			public readonly string Path;

			public TempDir()
			{
				Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "netcdfsharp-gcmfile-" + Guid.NewGuid().ToString("N"));
				Directory.CreateDirectory(Path);
			}

			public FileInfo CreateFile(int size)
			{
				string path = System.IO.Path.Combine(Path, Guid.NewGuid().ToString("N"));
				byte[] bytes = new byte[size];
				new Random().NextBytes(bytes);
				File.WriteAllBytes(path, bytes);
				return new FileInfo(path);
			}

			public void Dispose()
			{
				try
				{
					Directory.Delete(Path, true);
				}
				catch (IOException)
				{
				}
			}
		}
	}
}
