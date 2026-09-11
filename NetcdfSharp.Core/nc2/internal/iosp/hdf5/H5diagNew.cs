// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/* Copyright Unidata */
namespace NetcdfSharp.ucar.nc2.@internal.iosp.hdf5
{
	using NetcdfFile = NetcdfSharp.ucar.nc2.NetcdfFile;
	using Variable = NetcdfSharp.ucar.nc2.Variable;
	using DataBTree = NetcdfSharp.ucar.nc2.iosp.hdf5.DataBTree;

	/// <summary>
	/// HDF5 diagnostic helper
	/// 
	/// @author caron
	/// @since 6/25/12
	/// </summary>
	public class H5diagNew
	{
	  private readonly NetcdfFile ncfile;
	  private readonly H5iospNew iosp;

	  public H5diagNew(NetcdfFile ncfile, H5iospNew iosp)
	  {
		this.ncfile = ncfile;
		this.iosp = iosp;
	  }

	  public class Size
	  {
		internal long storage;
		internal long nominal;
		internal int count;

		internal Size(long storage, int count)
		{
		  this.storage = storage;
		  this.count = count;
		}

		internal virtual float Ratio
		{
			get
			{
			  if (storage == 0)
			  {
				return 0;
			  }
			  return ((float) nominal / storage);
			}
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void showCompress(NetcdfSharp.java.util.Formatter f) throws NetcdfSharp.java.io.IOException
	  public virtual void showCompress(Formatter f)
	  {
		Size totalSize = new Size(0, 0);
		foreach (Variable v in ncfile.Variables)
		{
		  H5headerNew.Vinfo vinfo = (H5headerNew.Vinfo) v.SPobject;
		  showCompress(v, vinfo, totalSize, f);
		}
		f.format("%n");
		f.format(" total bytes   = %d%n", totalSize.nominal);
		f.format(" total storage = %d%n", totalSize.storage);
		f.format(" compression = %f%n", totalSize.Ratio);
		f.format(" nchunks     = %d%n", totalSize.count);

		File raf = new File(ncfile.Location);
		f.format(" file size    = %d%n", raf.length());
		float overhead = totalSize.storage == 0 ? 0 : ((float) raf.length() / totalSize.storage);
		f.format(" overhead     = %f%n", overhead);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void showCompress(NetcdfSharp.ucar.nc2.Variable v, H5headerNew.Vinfo vinfo, Size total, NetcdfSharp.java.util.Formatter f) throws NetcdfSharp.java.io.IOException
	  public virtual void showCompress(Variable v, H5headerNew.Vinfo vinfo, Size total, Formatter f)
	  {
		H5objects.MessageDataspace mdt = vinfo.mds;

		long total_elems = 1;
		f.format("%8s %-40s(", v.DataType, v.ShortName);
		foreach (int len in mdt.dimLength)
		{
		  f.format("%d ", len);
		  total_elems *= len;
		}
		bool sizeOk = total_elems == v.Size;
		total_elems = v.Size;

		long nominalSize = total_elems * v.ElementSize;

		Size size = new Size(nominalSize, 1);
		countStorageSize(vinfo, size);
		total.storage += size.storage;
		total.nominal += nominalSize;
		total.count += size.count;
		float ratio = (size.storage == 0) ? 0 : (float) nominalSize / size.storage;

		f.format(") == %d nelems %s == %d bytes storage = %d (%f) nchunks = %d%n", total_elems, sizeOk ? "" : "*", nominalSize, size.storage, ratio, size.count);
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void countStorageSize(H5headerNew.Vinfo vinfo, Size size) throws NetcdfSharp.java.io.IOException
	  private void countStorageSize(H5headerNew.Vinfo vinfo, Size size)
	  {
		DataBTree btree = vinfo.btree;
		if (btree == null || vinfo.useFillValue_Conflict)
		{
		  size.storage = 0;
		  size.count = 0;
		  return; // 0 storage
		}

		int count = 0;
		long total = 0;
		DataBTree.DataChunkIterator iter = btree.getDataChunkIteratorFilter(null);
		while (iter.hasNext())
		{
		  DataBTree.DataChunk dc = iter.next();
		  total += dc.size;
		  count++;
		}

		size.storage = total;
		size.count = count;
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public long[] countStorageSize(H5headerNew.Vinfo vinfo, Size size, NetcdfSharp.java.util.Formatter f) throws NetcdfSharp.java.io.IOException
	  public virtual long[] countStorageSize(H5headerNew.Vinfo vinfo, Size size, Formatter f)
	  {
		long[] result = new long[2];
		DataBTree btree = vinfo.btree;
		if (btree == null)
		{
		  if (f != null)
		  {
			f.format("btree is null%n");
		  }
		  return result;
		}
		if (vinfo.useFillValue_Conflict)
		{
		  if (f != null)
		  {
			f.format("useFillValue - no data is stored%n");
		  }
		  return result;
		}

		int count = 0;
		long total = 0;
		DataBTree.DataChunkIterator iter = btree.getDataChunkIteratorFilter(null);
		while (iter.hasNext())
		{
		  DataBTree.DataChunk dc = iter.next();
		  if (f != null)
		  {
			f.format(" %s%n", dc);
		  }
		  total += dc.size;
		  count++;
		}

		result[0] = total;
		result[1] = count;
		return result;
	  }


	  //////////////////////////////////////////////////////////////////////////////////////////////

	  public virtual void deflate(Formatter f, Variable v)
	  {
		H5headerNew.Vinfo vinfo = (H5headerNew.Vinfo) v.SPobject;
		DataBTree btree = vinfo.btree;
		if (btree == null || vinfo.useFillValue_Conflict)
		{
		  f.format("%s not chunked%n", v.ShortName);
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void deflate(NetcdfSharp.java.util.Formatter f, NetcdfSharp.ucar.nc2.iosp.hdf5.DataBTree btree) throws NetcdfSharp.java.io.IOException
	  public virtual void deflate(Formatter f, DataBTree btree)
	  {
		int count = 0;
		long total = 0;
		DataBTree.DataChunkIterator iter = btree.getDataChunkIteratorFilter(null);
		while (iter.hasNext())
		{
		  DataBTree.DataChunk dc = iter.next();
		  total += dc.size;
		  count++;
		}
	  }

	}

}
