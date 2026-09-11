// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;

/*
 * Copyright (c) 1998-2020 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.unidata.io
{

	/// <summary>
	/// A remote file that can be read from. </summary>
	public interface ReadableRemoteFile : IDisposable
	{

	  /// <summary>
	  /// Read directly from the remote service. For HTTP based access, this is where "Accept-Ranges" HTTP requests are
	  /// called to do random access
	  /// </summary>
	  /// <param name="pos"> start here in the file </param>
	  /// <param name="buff"> put data into this buffer </param>
	  /// <param name="offset"> buffer offset </param>
	  /// <param name="len"> this number of bytes </param>
	  /// <returns> actual number of bytes read </returns>
	  /// <exception cref="IOException"> on io error </exception>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: int readRemote(long pos, byte[] buff, int offset, int len) throws NetcdfSharp.java.io.IOException;
	  int readRemote(long pos, byte[] buff, int offset, int len);

	  /// <summary>
	  /// Close any resources used to enable remote reading.
	  /// 
	  /// For example, HTTPSession from httpservices, or S3Client from the AWS SDK
	  /// </summary>
	  void closeRemote();
	}

}
