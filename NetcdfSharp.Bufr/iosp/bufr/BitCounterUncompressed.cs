// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.bufr
{
	using Indent = NetcdfSharp.ucar.nc2.util.Indent;

	/// <summary>
	/// Counts the size of nested tables, for uncompressed messages.
	/// 
	/// A top-level BitCounterUncompressed counts bits for one row = obs = dataset.
	/// obs = new BitCounterUncompressed(root, 1, 0);
	/// 
	/// @author caron
	/// @since May 10, 2008
	/// </summary>
	public class BitCounterUncompressed : BitCounter
	{
	  private readonly DataDescriptor parent; // represents the table - fields/cols are the subKeys of dkey
	  private readonly int nrows; // number of rows in this table
	  private readonly int replicationCountSize; // number of bits taken up by the count variable (non-zero only for sequences)

	  private IDictionary<DataDescriptor, int> bitPosition;
	  private IDictionary<DataDescriptor, BitCounterUncompressed[]> subCounters; // nested tables; null for regular fields
	  private int[] startBit; // from start of data section, for each row
// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private int countBits_Conflict; // total nbits in this table
	  private int bitOffset; // count bits

	  private static bool debug;

	  /// <summary>
	  /// This counts the size of an array of Structures or Sequences, ie Structure(n)
	  /// </summary>
	  /// <param name="parent"> is a structure or a sequence - so has subKeys </param>
	  /// <param name="nrows"> numbers of rows in the table, equals 1 for top level </param>
	  /// <param name="replicationCountSize"> number of bits taken up by the count variable (non-zero only for sequences) </param>
	  internal BitCounterUncompressed(DataDescriptor parent, int nrows, int replicationCountSize)
	  {
		this.parent = parent;
		this.nrows = nrows;
		this.replicationCountSize = replicationCountSize;
	  }

	  // not used yet
	  public virtual DataDescriptor BitOffset
	  {
		  set
		  {
			if (bitPosition == null)
			{
			  bitPosition = new Dictionary<DataDescriptor, int>(2 * parent.SubKeys.Count);
			}
			bitPosition[value] = bitOffset;
			bitOffset += value.BitWidth;
		  }
	  }

	  public virtual int getOffset(DataDescriptor dkey)
	  {
		return bitPosition[dkey];
	  }


	  /// <summary>
	  /// Track nested Tables.
	  /// </summary>
	  /// <param name="subKey"> subKey is a structure or a sequence - so itself has subKeys </param>
	  /// <param name="n"> numbers of rows in the nested table </param>
	  /// <param name="row"> which row in the parent Table this belongs to </param>
	  /// <param name="replicationCountSize"> number of bits taken up by the count (non-zero for sequences) </param>
	  /// <returns> nested ReplicationCounter </returns>
	  internal virtual BitCounterUncompressed makeNested(DataDescriptor subKey, int n, int row, int replicationCountSize)
	  {
		if (subCounters == null)
		{
		  subCounters = new Dictionary<DataDescriptor, BitCounterUncompressed[]>(5); // assumes DataDescriptor.equals is ==
		}

		// one for each row in this table
		if (!subCounters.TryGetValue(subKey, out BitCounterUncompressed[] subCounter))
		{
			subCounter = new BitCounterUncompressed[nrows];
			subCounters[subKey] = subCounter;
		}

		BitCounterUncompressed rc = new BitCounterUncompressed(subKey, n, replicationCountSize);
		subCounter[row] = rc;

		return rc;
	  }

	  public virtual BitCounterUncompressed[] getNested(DataDescriptor subKey)
	  {
		return (subCounters == null) ? null : subCounters[subKey];
	  }

	  // total bits of this table and all subtables
	  internal virtual int countBits(int startBit)
	  {
		countBits_Conflict = replicationCountSize;
		this.startBit = new int[nrows];

		for (int i = 0; i < nrows; i++)
		{
		  this.startBit[i] = startBit + countBits_Conflict;
		  if (debug)
		  {
			Console.WriteLine(" BitCounterUncompressed row " + i + " startBit=" + this.startBit[i]);
		  }

		  foreach (DataDescriptor nd in parent.subKeys)
		  {
			BitCounterUncompressed[] bitCounter = (subCounters == null) ? null : subCounters[nd];
			if (bitCounter == null) // a regular field
			{
			  countBits_Conflict += nd.BitWidth;
			}
			else
			{
			  if (debug)
			  {
				Console.WriteLine(" ---------> nested " + nd.FxyName + " starts at =" + (startBit + countBits_Conflict));
			  }
			  countBits_Conflict += bitCounter[i].countBits(startBit + countBits_Conflict);
			  if (debug)
			  {
				Console.WriteLine(" <--------- nested " + nd.FxyName + " ends at =" + (startBit + countBits_Conflict));
			  }
			}
		  }
		}
		return countBits_Conflict;
	  }

	  public virtual int CountBits
	  {
		  get
		  {
			return countBits_Conflict;
		  }
	  }

	  public virtual int NumberRows
	  {
		  get
		  {
			return nrows;
		  }
	  }

	  public virtual int getStartBit(int row)
	  {
		if (row >= startBit.Length)
		{
		  throw new System.InvalidOperationException();
		}
		return startBit[row];
	  }

	  public virtual void toString(Formatter f, Indent indent)
	  {
		f.format("%s dds=%s, ", indent, parent.FxyName);
		f.format("nrows=%d%n", nrows);
		if (subCounters == null)
		{
		  return;
		}

		indent.incr();
		int count = 0;

		// Map<DataDescriptor, BitCounterUncompressed[]> subCounters; // nested tables; null for regular fields
		foreach (BitCounterUncompressed[] bcus in subCounters.Values)
		{
		  if (bcus == null)
		  {
			f.format("%s%d: null", indent, count);
		  }
		  else
		  {
			foreach (BitCounterUncompressed bcu in bcus)
			{
			  bcu.toString(f, indent);
			}
		  }
		  count++;
		}
		indent.decr();
	  }
	}

}
