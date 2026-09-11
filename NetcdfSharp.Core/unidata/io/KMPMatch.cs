// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

/*
 * Copyright (c) 1998-2018 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.unidata.io
{

	/// <summary>
	/// Knuth-Morris-Pratt Algorithm for Pattern Matching.
	/// </summary>
	/// <seealso cref="<a href="
	///      "http://www.fmi.uni-sofia.bg/fmi/logic/vboutchkova/sources/KMPMatch_java.html">http://www.fmi.uni-sofia.bg/fmi/logic/vboutchkova/sources/KMPMatch_java.html</a>/>
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Immutable public class KMPMatch
	public class KMPMatch
	{

	  private readonly byte[] match;
	  private readonly int[] failure;

	  /// <summary>
	  /// Constructor
	  /// </summary>
	  /// <param name="match"> search for this byte pattern </param>
	  public KMPMatch(byte[] match)
	  {
		this.match = match;
		failure = computeFailure(match);
	  }

	  public virtual int MatchLength
	  {
		  get
		  {
			return match.Length;
		  }
	  }

	  /// <summary>
	  /// Finds the first occurrence of match in data.
	  /// </summary>
	  /// <param name="data"> search in this byte block </param>
	  /// <param name="start"> start at data[start] </param>
	  /// <param name="max"> end at data[start+max] </param>
	  /// <returns> index into data[] of first match, else -1 if not found. </returns>
	  public virtual int indexOf(byte[] data, int start, int max)
	  {
		int j = 0;
		if (data.Length == 0)
		{
		  return -1;
		}

		for (int i = start; i < start + max; i++)
		{
		  while (j > 0 && match[j] != data[i])
		  {
			j = failure[j - 1];
		  }

		  if (match[j] == data[i])
		  {
			j++;
		  }

		  if (j == match.Length)
		  {
			return i - match.Length + 1;
		  }

		}
		return -1;
	  }

	  /*
	   * Finds the first occurrence of match in data.
	   * 
	   * @param data search in this byte block
	   * 
	   * @param start start at data[start]
	   * 
	   * @param max end at data[start+max]
	   * 
	   * @return index into block of first match, else -1 if not found.
	   *
	   * public int scan(InputStream is, int start, int max) {
	   * int j = 0;
	   * if (data.length == 0) return -1;
	   * 
	   * for (int i = start; i < start + max; i++) {
	   * while (j > 0 && match[j] != data[i])
	   * j = failure[j - 1];
	   * 
	   * if (match[j] == data[i])
	   * j++;
	   * 
	   * if (j == match.length)
	   * return i - match.length + 1;
	   * 
	   * }
	   * return -1;
	   * } //
	   */


	  private int[] computeFailure(byte[] match)
	  {
		int[] result = new int[match.Length];

		int j = 0;
		for (int i = 1; i < match.Length; i++)
		{
		  while (j > 0 && match[j] != match[i])
		  {
			j = result[j - 1];
		  }

		  if (match[j] == match[i])
		  {
			j++;
		  }

		  result[i] = j;
		}

		return result;
	  }
	}

}
