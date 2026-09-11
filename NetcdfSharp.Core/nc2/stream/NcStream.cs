// Minimal shim for NetcdfSharp.ucar.nc2.stream.NcStream (out-of-scope streaming module).
// Only the writeVInt helper used by IospHelper is provided.

namespace NetcdfSharp.ucar.nc2.stream
{
	using DataOutputStream = NetcdfSharp.java.io.DataOutputStream;

	/// <summary>
	/// Defines the ncstream format. Only a minimal subset is ported.
	/// </summary>
	public class NcStream
	{
	  // must start with this "CDFS"
	  public static readonly byte[] MAGIC_START = {0x43, 0x44, 0x46, 0x53};

	  /// <summary>
	  /// Write a value as a variable-length int (protobuf-style varint32).
	  /// </summary>
	  public static int writeVInt(DataOutputStream @out, int value)
	  {
		int count = 0;

		while (true)
		{
		  if ((value & ~0x7F) == 0)
		  {
			@out.write((byte) value);
			break;
		  }
		  else
		  {
			@out.write((byte) ((value & 0x7F) | 0x80));
			value = (int)((uint)value >> 7);
		  }
		}

		return count + 1;
	  }
	}
}
