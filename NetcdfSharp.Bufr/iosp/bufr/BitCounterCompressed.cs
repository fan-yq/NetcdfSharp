// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{

	/// <summary>
	/// Count size of compressed fields
	/// 
	/// @author caron
	/// @since Jul 4, 2008
	/// </summary>
	public class BitCounterCompressed : BitCounter
	{

	  private readonly DataDescriptor dkey; // the field to count
	  private readonly int nrows; // number of (obs) in the compression
	  private readonly int bitOffset; // starting position of the compressed data, relative to start of data section
	  private int dataWidth; // bitWidth of incremental values
	  private BitCounterCompressed[][] nested; // used if the dkey is a structure = nested[innerRows][dkey.subkeys.size]

	  /// <summary>
	  /// This counts the size of an array of Structures or Sequences, ie Structure(n)
	  /// </summary>
	  /// <param name="dkey"> is a structure or a sequence - so has subKeys </param>
	  /// <param name="n"> numbers of rows in the table </param>
	  /// <param name="bitOffset"> number of bits taken up by the count variable (non-zero only for sequences) </param>
	  public BitCounterCompressed(DataDescriptor dkey, int n, int bitOffset)
	  {
		this.dkey = dkey;
		this.nrows = n;
		this.bitOffset = bitOffset;
	  }

	  internal virtual int DataWidth
	  {
		  set
		  {
			this.dataWidth = value;
		  }
	  }

	  public virtual int StartingBitPos
	  {
		  get
		  {
			return bitOffset;
		  }
	  }

	  public virtual int getBitPos(int msgOffset)
	  {
		return bitOffset + dkey.bitWidth + 6 + dataWidth * msgOffset;
	  }

	  public virtual int TotalBits
	  {
		  get
		  {
			if (nested == null)
			{
			  return dkey.bitWidth + 6 + dataWidth * nrows;
			}
			else
			{
			  int totalBits = 0;
			  foreach (BitCounterCompressed[] counters in nested)
			  {
				if (counters == null)
				{
				  continue;
				}
				foreach (BitCounterCompressed counter in counters)
				{
				  if (counter != null)
				  {
					totalBits += counter.TotalBits;
				  }
				}
			  }
			  if (dkey.replicationCountSize > 0)
			  {
				totalBits += dkey.replicationCountSize + 6; // 6 boit count, 6 bit extra
			  }
			  return totalBits;
			}
		  }
	  }

	  public virtual BitCounterCompressed[] getNestedCounters(int innerIndex)
	  {
		return nested[innerIndex];
	  }

	  public virtual void addNestedCounters(int innerDimensionSize)
	  {
		nested = new BitCounterCompressed[innerDimensionSize][];
		for (int i = 0; i < innerDimensionSize; i++)
		{
			nested[i] = new BitCounterCompressed[dkey.SubKeys.Count];
		}
	  }

	  /// <summary>
	  /// Number of nested fields
	  /// </summary>
	  /// <returns> 1 if no nested fields, otherwise count of nested fields </returns>
	  public virtual int ncounters()
	  {
		if (nested == null)
		{
		  return 1;
		}
		else
		{
		  int ncounters = 0;
		  foreach (BitCounterCompressed[] counters in nested)
		  {
			if (counters == null)
			{
			  continue;
			}
			foreach (BitCounterCompressed counter in counters)
			{
			  if (counter != null)
			  {
				ncounters += counter.ncounters();
			  }
			}
		  }
		  return ncounters;
		}
	  }

	  public virtual void show(Formatter @out, int indent)
	  {
		for (int i = 0; i < indent; i++)
		{
		  @out.format(" ");
		}
		@out.format("%8d %8d %4d %s %n", TotalBits, bitOffset, dataWidth, dkey.name);
		if (nested != null)
		{
		  foreach (BitCounterCompressed[] counters in nested)
		  {
			if (counters == null)
			{
			  continue;
			}
			foreach (BitCounterCompressed counter in counters)
			{
			  if (counter != null)
			  {
				counter.show(@out, indent + 2);
			  }
			}
		  }
		}
	  }

	  public virtual int NumberRows
	  {
		  get
		  {
			return nrows;
		  }
	  }
	}


}
