// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.collection
{
	using NetcdfSharp.ucar.ma2;
	using CoordinateTime2D = NetcdfSharp.ucar.nc2.grib.coord.CoordinateTime2D;
	using CoordsSet = NetcdfSharp.ucar.nc2.ft2.coverage.CoordsSet;
	using GdsHorizCoordSys = NetcdfSharp.ucar.nc2.grib.GdsHorizCoordSys;
	using Grib1Customizer = NetcdfSharp.ucar.nc2.grib.grib1.tables.Grib1Customizer;
	using Grib1Parameter = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Parameter;
	using Grib1ParamTime = NetcdfSharp.ucar.nc2.grib.grib1.Grib1ParamTime;
	using Grib1Record = NetcdfSharp.ucar.nc2.grib.grib1.Grib1Record;
	using Grib1SectionProductDefinition = NetcdfSharp.ucar.nc2.grib.grib1.Grib1SectionProductDefinition;
	using Grib2Record = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Record;
	using Grib2RecordScanner = NetcdfSharp.ucar.nc2.grib.grib2.Grib2RecordScanner;
	using Grib2Tables = NetcdfSharp.ucar.nc2.grib.grib2.table.Grib2Tables;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;
	using SubsetParams = NetcdfSharp.ucar.nc2.ft2.coverage.SubsetParams;
	using TimeCoordIntvDateValue = NetcdfSharp.ucar.nc2.grib.coord.TimeCoordIntvDateValue;

	/// <summary>
	/// Grib Data Reader.
	/// Split from GribIosp, so can be used by GribCoverage.
	/// 
	/// @author caron
	/// @since 4/6/11
	/// </summary>
	// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
	// ORIGINAL LINE: @Immutable public abstract class GribDataReader
	public abstract class GribDataReader
	{
		private static readonly Logger logger = LoggerFactory.getLogger(typeof(GribDataReader));


		public static GribDataReader factory(GribCollectionImmutable gribCollection, GribCollectionImmutable.VariableIndex vindex)
		{
			if (gribCollection.isGrib1)
			{
				return new Grib1DataReader(gribCollection, vindex);
			}
			else
			{
				return new Grib2DataReader(gribCollection, vindex);
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: protected abstract float[] readData(NetcdfSharp.ucar.unidata.io.RandomAccessFile rafData, DataRecord dr) throws NetcdfSharp.java.io.IOException;
		protected internal abstract float[] readData(RandomAccessFile rafData, DataRecord dr);

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: protected abstract void show(NetcdfSharp.ucar.unidata.io.RandomAccessFile rafData, long dataPos) throws NetcdfSharp.java.io.IOException;
		protected internal abstract void show(RandomAccessFile rafData, long dataPos);

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public static GribCollectionImmutable.Record currentDataRecord;
		public static GribDataValidator validator;
		public static string currentDataRafFilename;
		// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
		private const bool show_Conflict = false; // debug

#pragma warning disable CS0162 // const debug flags: Java-compatible compile-time folding of debug blocks

		protected internal readonly GribCollectionImmutable gribCollection;
		private readonly GribCollectionImmutable.VariableIndex vindex;
		private readonly IList<DataRecord> records = new List<DataRecord>();

		protected internal GribDataReader(GribCollectionImmutable gribCollection, GribCollectionImmutable.VariableIndex vindex)
		{
			this.gribCollection = gribCollection;
			this.vindex = vindex;
		}

		/// <summary>
		/// Read the section of data described by want
		/// </summary>
		/// <param name="want"> which data do you want? </param>
		/// <returns> data as an Array </returns>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Array readData(SectionIterable want) throws IOException, InvalidRangeException
		public virtual NcArray readData(SectionIterable want)
		{
			if (vindex is PartitionCollectionImmutable.VariableIndexPartitioned)
			{
				return readDataFromPartition((PartitionCollectionImmutable.VariableIndexPartitioned)vindex, want);
			}
			else
			{
				return readDataFromCollection(vindex, want);
			}
		}

		/*
		 * SectionIterable iterates over the source indexes, corresponding to vindex's SparseArray.
		 * IOSP: works because variable coordinate corresponds 1-1 to Grib Coordinate.
		 * GribCoverage: must translate coordinates to Grib Coordinate index.
		 * want.getShape() indicates the result Array shape.
		 * SectionIterable.next(int[] index) is not used here.
		 */
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private Array readDataFromCollection(GribCollectionImmutable.VariableIndex vindex, SectionIterable want) throws NetcdfSharp.java.io.IOException
		private NcArray readDataFromCollection(GribCollectionImmutable.VariableIndex vindex, SectionIterable want)
		{
			// first time, read records and keep in memory
			vindex.readRecords();

			int rank = want.Rank;
			int sectionLen = rank - 2; // all but x, y
			SectionIterable sectionWanted = want.subSection(0, sectionLen);
			// assert sectionLen == vindex.getRank(); LOOK true or false ??

			// collect all the records that need to be read
			int resultIndex = 0;
			foreach (int sourceIndex in sectionWanted)
			{
				// addRecord(sourceIndex, count++);
				GribCollectionImmutable.Record record = vindex.getRecordAt(sourceIndex);
				if (Grib.debugRead)
				{
					logger.debug("GribIosp debugRead sourceIndex={} resultIndex={} record is null={}", sourceIndex, resultIndex, record == null);
				}
				if (record != null)
				{
					records.Add(new DataRecord(resultIndex, record, vindex.group.getGdsHorizCoordSys()));
				}
				resultIndex++;
			}

			// sort by file and position, then read
			DataReceiverIF dataReceiver = new DataReceiver(want.Shape, want.getRange(rank - 2), want.getRange(rank - 1));
			read(dataReceiver);
			return dataReceiver.Array;
		}

		/*
		 * Iterates using SectionIterable.next(int[] index).
		 * The work of translating that down into partition heirarchy and finally to a GC is all in
		 * VariableIndexPartitioned.getDataRecord(int[] index)
		 * want.getShape() indicates the result Array shape.
		 */
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private Array readDataFromPartition(PartitionCollectionImmutable.VariableIndexPartitioned vindexP, SectionIterable section) throws NetcdfSharp.java.io.IOException
		private NcArray readDataFromPartition(PartitionCollectionImmutable.VariableIndexPartitioned vindexP, SectionIterable section)
		{

			int rank = section.Rank;
			SectionIterable sectionWanted = section.subSection(0, rank - 2); // all but x, y
			SectionIterable.SectionIterator iterWanted = sectionWanted.Iterator; // iterator over wanted indices in vindexP
			int[] indexWanted = new int[rank - 2]; // place to put the iterator result
			int[] useIndex = indexWanted;

			// collect all the records that need to be read
			int resultPos = 0;
			while (iterWanted.MoveNext())
			{
				iterWanted.next(indexWanted); // returns the vindexP index in indexWanted array

				// for MRUTP, must munge the index here (not in vindexP.getDataRecord, because its recursive
				if (vindexP.Type == GribCollectionImmutable.GribType.MRUTP)
				{
					// find the partition from getRuntimeIdxFromMrutpTimeIndex
					CoordinateTime2D time2D = (CoordinateTime2D)vindexP.CoordinateTime;
					Debug.Assert(time2D != null);
					int[] timeIndices = time2D.getTimeIndicesFromMrutp(indexWanted[0]);

					int[] indexReallyWanted = new int[indexWanted.Length + 1];
					indexReallyWanted[0] = timeIndices[0];
					indexReallyWanted[1] = timeIndices[1];
					System.Array.Copy(indexWanted, 1, indexReallyWanted, 2, indexWanted.Length - 1);
					useIndex = indexReallyWanted;
				}

				PartitionCollectionImmutable.DataRecord record = vindexP.getDataRecord(useIndex);
				if (record == null)
				{
					if (Grib.debugRead)
					{
						logger.debug("readDataFromPartition missing data%n");
					}
					resultPos++; // can just skip, since result is prefilled with NaNs
					continue;
				}
				record.resultIndex = resultPos;
				records.Add(record);
				resultPos++;
			}

			// sort by file and position, then read
			DataReceiverIF dataReceiver = new DataReceiver(section.Shape, section.getRange(rank - 2), section.getRange(rank - 1));
			readPartitioned(dataReceiver);

			return dataReceiver.Array;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// Coordinate based subsetting for Coverage

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: public Array readData2(NetcdfSharp.ucar.nc2.ft2.coverage.CoordsSet want, RangeIterator yRange, RangeIterator xRange) throws NetcdfSharp.java.io.IOException
		public virtual NcArray readData2(CoordsSet want, RangeIterator yRange, RangeIterator xRange)
		{
			if (vindex is PartitionCollectionImmutable.VariableIndexPartitioned)
			{
				return readDataFromPartition2((PartitionCollectionImmutable.VariableIndexPartitioned)vindex, want, yRange, xRange);
			}
			else
			{
				return readDataFromCollection2(vindex, want, yRange, xRange);
			}
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private Array readDataFromCollection2(GribCollectionImmutable.VariableIndex vindex, NetcdfSharp.ucar.nc2.ft2.coverage.CoordsSet want, RangeIterator yRange, RangeIterator xRange) throws NetcdfSharp.java.io.IOException
		private NcArray readDataFromCollection2(GribCollectionImmutable.VariableIndex vindex, CoordsSet want, RangeIterator yRange, RangeIterator xRange)
		{
			// first time, read records and keep in memory
			vindex.readRecords();

			// collect all the records that need to be read
			int resultIndex = 0;
			foreach (SubsetParams coords in want)
			{
				GribCollectionImmutable.Record record = vindex.getRecordAt(coords);
				if (record != null)
				{
					DataRecord dr = new DataRecord(resultIndex, record, vindex.group.getGdsHorizCoordSys());
					if (GribDataReader.validator != null)
					{
						dr.validation = coords;
					}
					records.Add(dr);
				}
				resultIndex++;
			}

			DataReceiverIF dataReceiver = new DataReceiver(want.getShape(yRange, xRange), yRange, xRange);
			read(dataReceiver);
			return dataReceiver.Array;
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private Array readDataFromPartition2(PartitionCollectionImmutable.VariableIndexPartitioned vindexP, NetcdfSharp.ucar.nc2.ft2.coverage.CoordsSet want, RangeIterator yRange, RangeIterator xRange) throws NetcdfSharp.java.io.IOException
		private NcArray readDataFromPartition2(PartitionCollectionImmutable.VariableIndexPartitioned vindexP, CoordsSet want, RangeIterator yRange, RangeIterator xRange)
		{

			// collect all the records that need to be read
			int resultPos = 0;
			foreach (SubsetParams coords in want)
			{
				PartitionCollectionImmutable.DataRecord record = vindexP.getDataRecord(coords);
				if (record != null)
				{
					record.resultIndex = resultPos;
					records.Add(record);
				}
				resultPos++;
			}

			// sort by file and position, then read
			DataReceiverIF dataReceiver = new DataReceiver(want.getShape(yRange, xRange), yRange, xRange);
			readPartitioned(dataReceiver);

			return dataReceiver.Array;
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Read all of the data records that have been added.
		/// The full (x,y) record is read, the reciever will subset the (x, y) as needed.
		/// </summary>
		/// <param name="dataReceiver"> send data here. </param>
		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void read(DataReceiverIF dataReceiver) throws NetcdfSharp.java.io.IOException
		private void read(DataReceiverIF dataReceiver)
		{
			records.Sort();

			int currFile = -1;
			RandomAccessFile rafData = null;
			try
			{
				foreach (DataRecord dr in records)
				{
					if (Grib.debugIndexOnly || Grib.debugGbxIndexOnly)
					{
						GribIosp.debugIndexOnlyCount++;
						currentDataRecord = dr.record;
						currentDataRafFilename = gribCollection.getDataRafFilename(dr.record.fileno);
						if (Grib.debugIndexOnlyShow)
						{
							dr.show(gribCollection);
						}
						dataReceiver.setDataToZero();
						continue;
					}

					if (dr.record.fileno != currFile)
					{
						if (rafData != null)
						{
							rafData.close();
						}
						rafData = gribCollection.getDataRaf(dr.record.fileno);
						currFile = dr.record.fileno;
					}

					if (dr.record.pos == GribCollectionMutable.MISSING_RECORD)
					{
						continue;
					}

					if (GribDataReader.validator != null && dr.validation != null && rafData != null)
					{
						GribDataReader.validator.validate(gribCollection.cust, rafData, dr.record.pos + dr.record.drsOffset, dr.validation);

					}
					else if (show_Conflict && rafData != null)
					{ // for validation
						show(dr.validation);
						show(rafData, dr.record.pos + dr.record.drsOffset);
					}

					float[] data = readData(rafData, dr);
					GdsHorizCoordSys hcs = vindex.group.getGdsHorizCoordSys();
					dataReceiver.addData(data, dr.resultIndex, hcs.nx);
				}

			}
			finally
			{
				if (rafData != null)
				{
					rafData.close(); // make sure its closed even on exception
				}
			}
		}

		private void show(SubsetParams validation)
		{
			if (validation == null)
			{
				return;
			}
			// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
			Console.Write("Coords wanted%n %s", validation);
		}

		// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		// ORIGINAL LINE: private void readPartitioned(DataReceiverIF dataReceiver) throws NetcdfSharp.java.io.IOException
		private void readPartitioned(DataReceiverIF dataReceiver)
		{
			records.Sort();

			PartitionCollectionImmutable.DataRecord lastRecord = null;
			RandomAccessFile rafData = null;
			try
			{

				foreach (DataRecord dr in records)
				{
					PartitionCollectionImmutable.DataRecord drp = (PartitionCollectionImmutable.DataRecord)dr;
					if (Grib.debugIndexOnly || Grib.debugGbxIndexOnly)
					{
						GribIosp.debugIndexOnlyCount++;
						if (Grib.debugIndexOnlyShow)
						{
							drp.show();
						}
						dataReceiver.setDataToZero();
						continue;
					}

					if ((rafData == null) || !drp.usesSameFile(lastRecord))
					{
						if (rafData != null)
						{
							rafData.close();
						}
						rafData = drp.usePartition.getRaf(drp.partno, dr.record.fileno);
					}
					lastRecord = drp;

					if (dr.record.pos == GribCollectionMutable.MISSING_RECORD)
					{
						continue;
					}

					if (GribDataReader.validator != null && dr.validation != null)
					{
						GribDataReader.validator.validate(gribCollection.cust, rafData, dr.record.pos + dr.record.drsOffset, dr.validation);
					}
					else if (show_Conflict)
					{ // for validation
						show(dr.validation);
						show(rafData, dr.record.pos + dr.record.drsOffset);
					}

					float[] data = readData(rafData, dr);
					GdsHorizCoordSys hcs = dr.hcs;
					dataReceiver.addData(data, dr.resultIndex, hcs.nx);
				}

			}
			finally
			{
				if (rafData != null)
				{
					rafData.close(); // make sure its closed even on exception
				}
			}
		}

		public class DataRecord : IComparable<DataRecord>, IComparable
		{
			internal int resultIndex; // index into the result array
			internal readonly GribCollectionImmutable.Record record;
			internal readonly GdsHorizCoordSys hcs;
			internal SubsetParams validation;

			internal DataRecord(int resultIndex, GribCollectionImmutable.Record record, GdsHorizCoordSys hcs)
			{
				this.resultIndex = resultIndex;
				this.record = record;
				this.hcs = hcs;
			}

			// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
			// ORIGINAL LINE: @Override public int compareTo(@Nonnull DataRecord o)
			public virtual int CompareTo(DataRecord o)
			{
				int r = JavaLangCompat.IntCompare(record.fileno, o.record.fileno);
				if (r != 0)
				{
					return r;
				}
				return JavaLangCompat.LongCompare(record.pos, o.record.pos);
			}

			public virtual int CompareTo(object o)
			{
				return CompareTo((DataRecord)o);
			}

			// debugging
			public virtual void show(GribCollectionImmutable gribCollection)
			{
				string dataFilename = gribCollection.getFilename(record.fileno);
				// JAVA TO C# CONVERTER TASK: The following line has a Java format specifier which cannot be directly translated to .NET:
				Console.Write(" fileno={0:D} filename={1} startPos={2:D}%n", record.fileno, dataFilename, record.pos);
			}
		}

		public interface DataReceiverIF
		{
			void addData(float[] data, int resultIndex, int nx);

			void setDataToZero(); // only used when debugging with gbx/ncx only, to fake the data

			NcArray Array { get; }
		}

		public class DataReceiver : DataReceiverIF
		{
			internal NcArray dataArray;
			internal readonly RangeIterator yRange;
			internal readonly RangeIterator xRange;
			internal readonly int horizSize;

			internal DataReceiver(int[] shape, RangeIterator yRange, RangeIterator xRange)
			{
				this.yRange = yRange;
				this.xRange = xRange;
				this.horizSize = yRange.length() * xRange.length();

				long len = Section.computeSize(shape);
				if (len > 100 * 1000 * 1000 * 4)
				{ // LOOK make configurable
					logger.debug("Len greater that 100MB shape={}%n{}", "[" + string.Join(", ", shape) + "]", new Exception().StackTrace);
					throw new ArgumentException("RequestTooLarge: Len greater that 100M ");
				}
				float[] data = new float[(int)len];
				System.Array.Fill(data, float.NaN); // prefill primitive array
				dataArray = NetcdfSharp.ucar.ma2.NcArray.factory(DataType.FLOAT, shape, data);
			}

			public virtual void addData(float[] data, int resultIndex, int nx)
			{
				int start = resultIndex * horizSize;
				int count = 0;
				foreach (int y in yRange)
				{
					foreach (int x in xRange)
					{
						int dataIdx = y * nx + x;
						dataArray.setFloat(start + count, data[dataIdx]);
						count++;
					}
				}
			}

			// optimization
			public virtual void setDataToZero()
			{
				float[] data = (float[])dataArray.get1DJavaArray(dataArray.DataType);
				System.Array.Fill(data, 0.0f);
			}

			public virtual NcArray Array
			{
				get
				{
					return dataArray;
				}
			}
		}

		/*
		 * public static class ChannelReceiver implements DataReceiverIF {
		 * private WritableByteChannel channel;
		 * private DataOutputStream outStream;
		 * private Range yRange, xRange;
		 * 
		 * ChannelReceiver(WritableByteChannel channel, Range yRange, Range xRange) {
		 * this.channel = channel;
		 * this.outStream = new DataOutputStream(Channels.newOutputStream(channel));
		 * this.yRange = yRange;
		 * this.xRange = xRange;
		 * }
		 * 
		 * @Override
		 * public void addData(float[] data, int resultIndex, int nx) throws IOException {
		 * // LOOK: write some ncstream header
		 * // outStream.write(header);
		 * 
		 * // now write the data
		 * for (int y = yRange.first(); y <= yRange.last(); y += yRange.stride()) {
		 * for (int x = xRange.first(); x <= xRange.last(); x += xRange.stride()) {
		 * int dataIdx = y * nx + x;
		 * outStream.writeFloat(data[dataIdx]);
		 * }
		 * }
		 * }
		 * 
		 * // optimization
		 * 
		 * @Override
		 * public void setDataToZero() {
		 * }
		 * 
		 * @Override
		 * public Array getArray() {
		 * return null;
		 * }
		 * }
		 */

		/////////////////////////////////////////////////////////

		private class Grib2DataReader : GribDataReader
		{
			internal readonly Grib2Tables cust;

			internal Grib2DataReader(GribCollectionImmutable gribCollection, GribCollectionImmutable.VariableIndex vindex) : base(gribCollection, vindex)
			{
				this.cust = (Grib2Tables)gribCollection.cust;
			}

			// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
			// ORIGINAL LINE: @Override protected float[] readData(NetcdfSharp.ucar.unidata.io.RandomAccessFile rafData, GribDataReader.DataRecord dr) throws NetcdfSharp.java.io.IOException
			protected internal override float[] readData(RandomAccessFile rafData, GribDataReader.DataRecord dr)
			{
				GdsHorizCoordSys hcs = dr.hcs;
				long dataPos = dr.record.pos + dr.record.drsOffset;
				long bmsPos = (dr.record.bmsOffset > 0) ? dr.record.pos + dr.record.bmsOffset : 0;
				return Grib2Record.readData(rafData, dataPos, bmsPos, hcs.gdsNumberPoints, hcs.ScanMode, hcs.nxRaw, hcs.nyRaw, hcs.nptsInLine);
			}

			// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
			// ORIGINAL LINE: @Override protected void show(NetcdfSharp.ucar.unidata.io.RandomAccessFile rafData, long pos) throws NetcdfSharp.java.io.IOException
			protected internal override void show(RandomAccessFile rafData, long pos)
			{
				Grib2Record gr = Grib2RecordScanner.findRecordByDrspos(rafData, pos);
				if (gr != null)
				{
					using (Formatter f = new Formatter())
					{
						f.format("File=%s%n", rafData.Location);
						f.format("  Parameter=%s%n", cust.getVariableName(gr));
						f.format("  ReferenceDate=%s%n", gr.ReferenceDate);
						f.format("  ForecastDate=%s%n", cust.getForecastDate(gr));
						TimeCoordIntvDateValue tinv = cust.getForecastTimeInterval(gr);
						if (tinv != null)
						{
							f.format("  TimeInterval=%s%n", tinv);
						}
						f.format("  ");
						gr.PDS.show(f);
						Console.Write("%nGrib2Record.readData at drsPos {0:D} = {1}%n", pos, f.ToString());
					}
				}
			}
		}

		private class Grib1DataReader : GribDataReader
		{
			internal readonly Grib1Customizer cust;

			internal Grib1DataReader(GribCollectionImmutable gribCollection, GribCollectionImmutable.VariableIndex vindex) : base(gribCollection, vindex)
			{
				this.cust = (Grib1Customizer)gribCollection.cust;
			}

			// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
			// ORIGINAL LINE: @Override protected float[] readData(NetcdfSharp.ucar.unidata.io.RandomAccessFile rafData, GribDataReader.DataRecord dr) throws NetcdfSharp.java.io.IOException
			protected internal override float[] readData(RandomAccessFile rafData, GribDataReader.DataRecord dr)
			{
				return Grib1Record.readData(rafData, dr.record.pos);
			}

			// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
			// ORIGINAL LINE: @Override protected void show(NetcdfSharp.ucar.unidata.io.RandomAccessFile rafData, long dataPos) throws NetcdfSharp.java.io.IOException
			protected internal override void show(RandomAccessFile rafData, long dataPos)
			{
				rafData.seek(dataPos);
				Grib1Record gr = new Grib1Record(rafData);
				using (Formatter f = new Formatter())
				{
					f.format("File=%s%n", rafData.Location);
					Grib1SectionProductDefinition pds = gr.PDSsection;
					Grib1Parameter param = cust.getParameter(pds.Center, pds.SubCenter, pds.TableVersion, pds.ParameterNumber);
					f.format("  Parameter=%s%n", param);
					f.format("  ReferenceDate=%s%n", gr.ReferenceDate);
					Grib1ParamTime ptime = gr.getParamTime(cust);
					f.format("  ForecastTime=%d%n", ptime.ForecastTime);
					if (ptime.IsInterval())
					{
						int[] tinv = ptime.Interval;
						f.format("  TimeInterval=(%d,%d)%n", tinv[0], tinv[1]);
					}
					f.format("%n");
					gr.PDSsection.showPds(cust, f);
					Console.Write("%nGrib1Record.readData at drsPos {0:D} = {1}%n", dataPos, f.ToString());
				}
			}
		}

	}

#pragma warning restore CS0162

}
