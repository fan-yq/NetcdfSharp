// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.IO;

/*
 * Copyright (c) 1998-2025 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using GribNumbers = NetcdfSharp.ucar.nc2.grib.GribNumbers;
	using GribUtils = NetcdfSharp.ucar.nc2.grib.GribUtils;
	using BitReader = NetcdfSharp.ucar.nc2.iosp.BitReader;
	using RandomAccessFile = NetcdfSharp.ucar.unidata.io.RandomAccessFile;

	/// <summary>
	/// Reads the data from one grib2 record. Original code almost for sure came from GEMPAK, but the
	/// lineage is unknown.
	/// 
	/// @author caron </summary>
	/// <seealso cref=""https://raw.githubusercontent.com/Unidata/gempak/master/extlibs/NDFD/mdlg2dec/unpk_cmplx.f""/>
	/// <seealso cref=""http://slosh.nws.noaa.gov/svn/degrib/vendor/grib2_unpacker/current/unpksecdif.f""/>
	/// <seealso cref=""unpk_complex in wgrib2 code""
	/// @since 4/2/11/>
	public class Grib2DataReader
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger logger = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(Grib2DataReader));

	  // look up table: 2**i - 1
	  private static readonly int[] bitsmv1 = new int[31];
	  static Grib2DataReader()
	  {
		for (int i = 0; i < 31; i++)
		{
		  bitsmv1[i] = (int) Math.Pow((double) 2, (double) i) - 1;
		}
	  }

	  ///////////////////////////////////////////////

	  private readonly int dataTemplate;
	  private readonly int totalNPoints; // gds: number of points
	  private readonly int dataNPoints; // drs: number of data points where one or more values are specified in Section 7 when
									 // a bit map is present
	  private readonly int scanMode;
	  private readonly int nx;
	  private readonly long startPos;
	  private readonly int dataLength;

	  private int bitmapIndicator;
	  private byte[] bitmap;

	  internal Grib2DataReader(int dataTemplate, int totalNPoints, int dataNPoints, int scanMode, int nx, long startPos, int dataLength)
	  {
		this.dataTemplate = dataTemplate;
		this.totalNPoints = totalNPoints;
		this.dataNPoints = dataNPoints;
		this.scanMode = scanMode;
		this.nx = nx;
		this.startPos = startPos;
		this.dataLength = dataLength;
	  }

	  /*
	   * Code table 5.0 - Data representation template number (5.0)
	   * 0: Grid point data - simple packing
	   * 1: Matrix value at grid point - simple packing
	   * 2: Grid point data - complex packing
	   * 3: Grid point data - complex packing and spatial differencing
	   * 4: Grid point data - IEEE floating point data
	   * 40: Grid point data - JPEG 2000 code stream format
	   * 41: Grid point data - Portable Network Graphics (PNG)
	   * 42: Grid point data - CCSDS recommended lossless compression
	   * 50: Spectral data - simple packing
	   * 51: Spherical harmonics data - complex packing
	   * 61: Grid point data - simple packing with logarithm pre-processing
	   * 200: Run length packing with level values
	   * 50002: Second order packing
	   * 65535: Missing
	   */

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public float[] getData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Grib2SectionBitMap bitmapSection, Grib2Drs gdrs) throws NetcdfSharp.java.io.IOException
	  public virtual float[] getData(RandomAccessFile raf, Grib2SectionBitMap bitmapSection, Grib2Drs gdrs)
	  {
		this.bitmap = bitmapSection.getBitmap(raf);
		this.bitmapIndicator = bitmapSection.BitMapIndicator;

		if (bitmap != null)
		{ // is bitmap ok ?
		  if (bitmap.Length * 8 < totalNPoints)
		  { // gdsNumberPoints == nx * ny ??
			logger.warn("Bitmap section length = {} != grid length {} ({},{})", bitmap.Length, totalNPoints, nx, totalNPoints / nx);
			throw new System.InvalidOperationException("Bitmap section length!= grid length");
		  }
		}

		raf.seek(startPos + 5); // skip past first 5 bytes in data section, now ready to read

		float[] data;
		switch (dataTemplate)
		{
		  case 0:
			data = getData0(raf, (Grib2Drs.Type0) gdrs);
			break;
		  case 2:
			data = getData2(raf, (Grib2Drs.Type2) gdrs);
			break;
		  case 3:
			data = getData3(raf, (Grib2Drs.Type3) gdrs);
			break;
		  case 40:
			data = getData40(raf, (Grib2Drs.Type40) gdrs);
			break;
		  case 41:
			data = getData41(raf, (Grib2Drs.Type0) gdrs);
			break;
		  case 42:
			data = getData42(raf, (Grib2Drs.Type42) gdrs);
			break;
		  case 50002:
			data = getData50002(raf, (Grib2Drs.Type50002) gdrs);
			break;
		  default:
			throw new System.NotSupportedException("Unsupported DRS type = " + dataTemplate);
		}

		// int scanMode = gds.getGds().getScanMode();
		// int nx = gds.getGds().getNx(); // needs some smarts for different type Grids
		scanningModeCheck(data, scanMode, nx);

		return data;
	  }

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Nullable int[] getRawData(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Grib2SectionBitMap bitmapSection, Grib2Drs gdrs) throws NetcdfSharp.java.io.IOException
	  internal virtual int[] getRawData(RandomAccessFile raf, Grib2SectionBitMap bitmapSection, Grib2Drs gdrs)
	  {
		this.bitmap = bitmapSection.getBitmap(raf);
		this.bitmapIndicator = bitmapSection.BitMapIndicator;

		if (bitmap != null)
		{ // is bitmap ok ?
		  if (bitmap.Length * 8 < totalNPoints)
		  { // gdsNumberPoints == nx * ny ??
			logger.warn("Bitmap section length = {} != grid length {} ({},{})", bitmap.Length, totalNPoints, nx, totalNPoints / nx);
			throw new System.InvalidOperationException("Bitmap section length!= grid length");
		  }
		}

		raf.seek(startPos + 5); // skip past first 5 bytes in data section, now ready to read

		if (dataTemplate != 40)
		{
		  return null;
		}

		// LOOK jpeg2k only
		return getData40raw(raf, (Grib2Drs.Type40) gdrs);
	  }

	  private const bool staticMissingValueInUse = true;
	  private static readonly float staticMissingValue = float.NaN;

	  private float getMissingValue(Grib2Drs.Type2 gdrs)
	  {
		int mvm = gdrs.missingValueManagement;

		float mv;
		if (staticMissingValueInUse || mvm == 0)
		{
		  mv = float.NaN;
		}
		else if (mvm == 1)
		{
		  mv = gdrs.primaryMissingValue;
		}
		else if (mvm == 2)
		{
		  mv = gdrs.secondaryMissingValue;
		}
		return mv;
	  }

	  /*
	   * 92.9.4 Data shall be coded in the form of non-negative scaled differences from a reference value
	   * of the whole field plus, if applicable, a local reference value.
	   * Notes:
	   * (1) A reference value is normally the minimum value of the data set which is represented.
	   * 
	   * (2) For grid-point values, complex packing features are intended to reduce the whole size of the GRIB message (data
	   * compression without loss of information with respect to simple packing). The basic concept is to reduce data size
	   * thanks to local redundancy. This is achieved just before packing, by splitting the whole set of scaled data values
	   * into
	   * groups, on which local references (such as local minima) are removed. It is done with some overhead, because extra
	   * descriptors are needed to manage the groups’ characteristics. An optional pre-processing of the scaled values
	   * (spatial
	   * differencing) may also be applied before splittig into groups, and combined methods, along with use of alternate
	   * row
	   * scanning mode, are very efficient on interpolated data.
	   * 
	   * (3) For spectral data, complex packing is provided for better accuracy of packing. This is because many spectral
	   * coefficients
	   * have small values (regardless of the sign), especially for large wave numbers. The first principle is not to pack a
	   * subset of
	   * coefficients, associated with small wave numbers so that the amplitude of the packed coefficients is reduced. The
	   * second
	   * principle is to apply an operator to the remaining part of the spectrum: with appropriate tuning it leads to a more
	   * homogeneous set of values to pack.
	   * 
	   * (4) The original data value Y (in the units of Code table 4.2, unless Notes in Code table 4.10 apply) can be
	   * recovered with the formula:
	   * Y × 10^D = R + (X1 + X2) × 2^E
	   * For simple packing and all spectral data
	   * E = Binary scale factor
	   * D = Decimal scale factor
	   * R = Reference value of the whole field
	   * X1 = 0
	   * X2 = Scaled (encoded) value.
	   * For complex grid-point packing schemes, E, D and R are as above, but
	   * X1 = Reference value (scaled integer) of the group the data value belongs to
	   * X2 = Scaled (encoded) value with the group reference value (X1) removed.
	   */


	  // Grid point data - simple packing
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private float[] getData0(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Grib2Drs.Type0 gdrs) throws NetcdfSharp.java.io.IOException
	  private float[] getData0(RandomAccessFile raf, Grib2Drs.Type0 gdrs)
	  {
		int nb = gdrs.numberOfBits;
		int D = gdrs.decimalScaleFactor;
		float DD = (float) Math.Pow((double) 10, (double) D);
		float R = gdrs.referenceValue;
		int E = gdrs.binaryScaleFactor;
		float EE = (float) Math.Pow(2.0, (double) E);

		// LOOK: can # datapoints differ from bitmap and data ?
		// dataPoints are number of points encoded, it could be less than the
		// totalNPoints in the grid record if bitMap is used, otherwise equal
		float[] data = new float[totalNPoints];

		// Y * 10**D = R + (X1 + X2) * 2**E
		// E = binary scale factor
		// D = decimal scale factor
		// R = reference value
		// X1 = 0
		// X2 = scaled encoded value
		// data[ i ] = (R + ( X1 + X2) * EE)/DD ;

		BitReader reader = new BitReader(raf, startPos + 5);
		if (bitmap == null)
		{
		  for (int i = 0; i < totalNPoints; i++)
		  {
			// data[ i ] = (R + ( X1 + X2) * EE)/DD ;
			data[i] = (R + reader.bits2UInt(nb) * EE) / DD;
		  }
		}
		else
		{
		  for (int i = 0; i < totalNPoints; i++)
		  {
			if (GribNumbers.testBitIsSet(bitmap[i / 8], i % 8))
			{
			  data[i] = (R + reader.bits2UInt(nb) * EE) / DD;
			}
			else
			{
			  data[i] = staticMissingValue;
			  // data[i] = R / DD;
			}
		  }
		}

		return data;
	  }

	  /*
	   * Data template 7.2 – Grid point data – complex packing
	   * Note: For most templates, details of the packing process are described in Regulation 92.9.4.
	   * Octet No. Contents
	   * 6–xx NG group reference values (X1 in the decoding formula), each of which is encoded using
	   * the number of bits specified in octet 20 of data representation template 5.0. Bits set to zero
	   * shall be appended as necessary to ensure this sequence of numbers ends on an octet boundary
	   * [xx+1]–yy NG group widths, each of which is encoded using the number of bits specified in octet 37
	   * of data representation template 5.2. Bits set to zero shall be appended as necessary to
	   * ensure this sequence of numbers ends on an octet boundary
	   * [yy+1]–zz NG scaled group lengths, each of which is encoded using the number of bits specified in
	   * octet 47 of data representation template 5.2. Bits set to zero shall be appended as necessary
	   * to ensure this sequence of numbers ends on an octet boundary (see Note 14 of data
	   * representation template 5.2)
	   * [zz+1]–nn Packed values (X2 in the decoding formula), where each value is a deviation from its respective
	   * group reference value
	   * 
	   * Notes:
	   * (1) Group descriptors mentioned above may not be physically present; if associated field width is 0.
	   * (2) Group lengths have no meaning for row by row packing; for consistency, associated field width should then be
	   * encoded as 0. So no specific test for row by row case is mandatory at decoding software level to handle
	   * encoding/decoding of group descriptors.
	   * (3) Scaled group lengths, if present, are encoded for each group. But the true last group length (unscaled) should
	   * be taken
	   * from data representation template.
	   * (4) For groups with a constant value, associated field width is 0, and no incremental data are physically present.
	   */
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private float[] getData2(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Grib2Drs.Type2 gdrs) throws NetcdfSharp.java.io.IOException
	  private float[] getData2(RandomAccessFile raf, Grib2Drs.Type2 gdrs)
	  {
		int mvm = gdrs.missingValueManagement;
		float mv = getMissingValue(gdrs);

		float DD = (float) Math.Pow((double) 10, (double) gdrs.decimalScaleFactor);
		float R = gdrs.referenceValue;
		float EE = (float) Math.Pow(2.0, (double) gdrs.binaryScaleFactor);
		float ref_val = R / DD;

		int NG = gdrs.numberOfGroups;
		if (NG == 0)
		{
		  return nGroups0(bitmapIndicator, ref_val, mv);
		}

		BitReader reader = new BitReader(raf, startPos + 5);

		// 6-xx Get reference values for groups (X1's)
		int[] X1 = new int[NG];
		int nb = gdrs.numberOfBits;
		if (nb != 0)
		{
		  for (int i = 0; i < NG; i++)
		  {
			X1[i] = (int) reader.bits2UInt(nb);
		  }
		}

		// [xx +1 ]-yy Get number of bits used to encode each group
		int[] NB = new int[NG];
		nb = gdrs.bitsGroupWidths;
		if (nb != 0)
		{
		  reader.incrByte();
		  for (int i = 0; i < NG; i++)
		  {
			NB[i] = (int) reader.bits2UInt(nb);
		  }
		}

		// [yy +1 ]-zz Get the scaled group lengths using formula
		// Ln = ref + Kn * len_inc, where n = 1-NG,
		// ref = referenceGroupLength, and len_inc = lengthIncrement

		int[] L = new int[NG];
		int @ref = gdrs.referenceGroupLength;
		int len_inc = gdrs.lengthIncrement;
		nb = gdrs.bitsScaledGroupLength;

		reader.incrByte();
		for (int i = 0; i < NG; i++)
		{
		  L[i] = @ref + (int) reader.bits2UInt(nb) * len_inc;
		}
		L[NG - 1] = gdrs.lengthLastGroup; // enter Length of Last Group

		float[] data = new float[totalNPoints];

		// [zz +1 ]-nn get X2 values and calculate the results Y using formula

		// Y = R + [(X1 + X2) * (2 ** E) * (10 ** D)]
		// WHERE:
		// Y = THE VALUE WE ARE UNPACKING
		// R = THE REFERENCE VALUE (FIRST ORDER MINIMA)
		// X1 = THE PACKED VALUE
		// X2 = THE SECOND ORDER MINIMA
		// E = THE BINARY SCALE FACTOR
		// D = THE DECIMAL SCALE FACTOR
		int count = 0;
		reader.incrByte();
		for (int i = 0; i < NG; i++)
		{
		  for (int j = 0; j < L[i]; j++)
		  {
			if (NB[i] == 0)
			{
			  if (mvm == 0)
			  { // X2 = 0
				data[count++] = (R + X1[i] * EE) / DD;
			  }
			  else
			  { // if (mvm == 1) || (mvm == 2 )
				data[count++] = mv;
			  }
			}
			else
			{
			  int X2 = (int) reader.bits2UInt(NB[i]);
			  if (mvm == 0)
			  {
				data[count++] = (R + (X1[i] + X2) * EE) / DD;
			  }
			  else
			  { // if (mvm == 1) || (mvm == 2 )
				// X2 is also set to missing value if all bits set to 1's
				if (X2 == bitsmv1[NB[i]])
				{
				  data[count++] = mv;
				}
				else
				{
				  data[count++] = (R + (X1[i] + X2) * EE) / DD;
				}
			  }
			}
		  } // end for j
		} // end for i

		if (bitmap != null)
		{
		  int idx = 0;
		  float[] tmp = new float[totalNPoints];
		  for (int i = 0; i < totalNPoints; i++)
		  {
			if (GribNumbers.testBitIsSet(bitmap[i / 8], i % 8))
			{
			  tmp[i] = data[idx++];
			}
			else
			{
			  tmp[i] = mv;
			}
		  }
		  data = tmp;
		}

		return data;
	  }


	  /*
	   * from wgrib unpk_complex():
	   * 
	   * p = sec[5]; // drs
	   * ref_val = ieee2flt(p+11); // R
	   * factor_2 = Int_Power(2.0, int2(p+15)); // 2^E
	   * factor_10 = Int_Power(10.0, -int2(p+17)); // 10^-D
	   * ref_val *= factor_10; // R/10^D
	   * factor = factor_2 * factor_10; // 2^E*10^-D
	   * nbits = p[19]; // NB
	   * ngroups = uint4(p+31); // NG
	   * bitmap_flag = code_table_6_0(sec); // bitmap->indicator
	   * ctable_5_6 = code_table_5_6(sec);
	   * 
	   * bitmap_flag = code_table_6_0(sec); // == gr.getBitmapSection().getBitMapIndicator();
	   * if (ngroups == 0) {
	   * if (bitmap_flag == 255) {
	   * for (i = 0; i < ndata; i++) data[i] = ref_val;
	   * return 0;
	   * }
	   * if (bitmap_flag == 0 || bitmap_flag == 254) {
	   * mask_pointer = sec[6] + 6; // probably bitmask is here ??
	   * mask = 0;
	   * for (i = 0; i < ndata; i++) {
	   * if ((i & 7) == 0) mask = *mask_pointer++; // get next byte ??
	   * data[i] = (mask & 128) ? ref_val : UNDEFINED; // bit 7 set
	   * mask <<= 1; // shift
	   * }
	   * return 0;
	   * }
	   * fatal_error("unknown bitmap", "");
	   * }
	   */
	  private float[] nGroups0(int bitmap_flag, float @ref, float mv1)
	  {
		float[] data = new float[totalNPoints];
		if (bitmap_flag == 255)
		{
		  for (int i = 0; i < totalNPoints; i++)
		  {
			data[i] = @ref;
		  }

		}
		else if (bitmap_flag == 0 || bitmap_flag == 254)
		{
		  int mask = 0;
		  int mask_pointer = 0;
		  for (int i = 0; i < totalNPoints; i++)
		  {
			if ((i & 7) == 0)
			{
			  mask = bitmap[mask_pointer];
			  mask_pointer++;
			}
			data[i] = ((mask & 128) == 0) ? @ref : mv1;
			mask <<= 1;
		  }

		}
		else
		{
		  throw new ArgumentException("unknown bitmap type =" + bitmap_flag);
		}
		return data;
	  }

	  /*
	   * Data template 7.3 – Grid point data – complex packing and spatial differencing
	   * Note: For most templates, details of the packing process are described in Regulation 92.9.4.
	   * Octet No. Contents
	   * 6–ww First value(s) of original (undifferenced) scaled data values, followed by the overall minimum
	   * of the differences. The number of values stored is 1 greater than the order of differentiation,
	   * and the field width is described at octet 49 of data representation template 5.3 (see Note 1)
	   * [ww+1]–xx NG group reference values (X1 in the decoding formula), each of which is encoded using the number
	   * of bits specified in octet 20 of data representation template 5.0. Bits set to zero shall be
	   * appended where necessary to ensure this sequence of numbers ends on an octet boundary
	   * [xx+1]–nn Same as for data representation template 7.2
	   * 
	   * Notes:
	   * (1) Referring to the notation in Note 1 of data representation template 5.3, at order 1, the values stored in
	   * octets 6–ww are
	   * g1 and gmin. At order 2, the values stored are h1, h2, and hmin.
	   * (2) Extra descriptors related to spatial differencing are added before the splitting descriptors, to reflect the
	   * separation
	   * between the two approaches. It enables to share software parts between cases with and without spatial differencing.
	   * (3) The position of overall minimum after initial data values is a choice that enables less software management.
	   * (4) Overall minimum will be negative in most cases. First bit should indicate the sign: 0 if positive, 1 if
	   * negative.
	   */
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private float[] getData3(NetcdfSharp.ucar.unidata.io.RandomAccessFile raf, Grib2Drs.Type3 gdrs) throws NetcdfSharp.java.io.IOException
	  private float[] getData3(RandomAccessFile raf, Grib2Drs.Type3 gdrs)
	  {
		int mvm = gdrs.missingValueManagement;
		float mv = getMissingValue(gdrs);

		float DD = (float) Math.Pow((double) 10, (double) gdrs.decimalScaleFactor);
		float R = gdrs.referenceValue;
		float EE = (float) Math.Pow(2.0, (double) gdrs.binaryScaleFactor);
		float ref_val = R / DD;

		int NG = gdrs.numberOfGroups;
		if (NG == 0)
		{
		  return nGroups0(bitmapIndicator, ref_val, mv);
		}

		BitReader reader = new BitReader(raf, startPos + 5);

		int ival1;
		int ival2 = 0;
		int minsd;

		// [6-ww] 1st values of undifferenced scaled values and minimums
		int os = gdrs.orderSpatial;
		int nbitsd = gdrs.descriptorSpatial;
		int sign;
		// ds is number of bytes, convert to bits -1 for sign bit
		nbitsd = nbitsd * 8;
		if (nbitsd > 0)
		{ // first order spatial differencing g1 and gMin
		  sign = (int) reader.bits2UInt(1);
		  ival1 = (int) reader.bits2UInt(nbitsd - 1);
		  if (sign == 1)
		  {
			ival1 = -ival1;
		  }
		  if (os == 2)
		  { // second order spatial differencing h1, h2, hMin
			sign = (int) reader.bits2UInt(1);
			ival2 = (int) reader.bits2UInt(nbitsd - 1);
			if (sign == 1)
			{
			  ival2 = -ival2;
			}
		  }
		  sign = (int) reader.bits2UInt(1);
		  minsd = (int) reader.bits2UInt(nbitsd - 1);
		  if (sign == 1)
		  {
			minsd = -minsd;
		  }

		}
		else
		{
		  float[] fillData = new float[totalNPoints];
		  for (int i = 0; i < totalNPoints; i++)
		  {
			fillData[i] = mv;
		  }
		  return fillData;
		}

		// [ww +1]-xx Get reference values for groups (X1's)
		// X1 == gref
		int[] X1 = new int[NG]; // initialized to zero
		int nb = gdrs.numberOfBits;
		if (nb != 0)
		{
		  reader.incrByte();
		  for (int i = 0; i < NG; i++)
		  {
			X1[i] = (int) reader.bits2UInt(nb);
		  }
		}

		// [xx +1 ]-yy Get number of bits used to encode each group
		// NB == gwidth
		int[] NB = new int[NG]; // initialized to zero
		nb = gdrs.bitsGroupWidths;
		if (nb != 0)
		{
		  reader.incrByte();
		  for (int i = 0; i < NG; i++)
		  {
			NB[i] = (int) reader.bits2UInt(nb);
		  }
		}

		int referenceGroupWidths = gdrs.referenceGroupWidths;
		for (int i = 0; i < NG; i++)
		{
		  NB[i] += referenceGroupWidths;
		}

		// [yy +1 ]-zz Get the scaled group lengths using formula
		// Ln = ref + Kn * len_inc, where n = 1-NG,
		// ref = referenceGroupLength, and len_inc = lengthIncrement

		int[] L = new int[NG]; // initialized to zero
		int referenceGroupLength = gdrs.referenceGroupLength;
		nb = gdrs.bitsScaledGroupLength;
		int len_inc = gdrs.lengthIncrement;

		if (nb != 0)
		{
		  reader.incrByte();
		  for (int i = 0; i < NG; i++)
		  {
			L[i] = (int) reader.bits2UInt(nb);
		  }
		}

		int totalL = 0;
		for (int i = 0; i < NG; i++)
		{
		  L[i] = L[i] * len_inc + referenceGroupLength;
		  totalL += L[i];
		}
		totalL -= L[NG - 1];
		totalL += gdrs.lengthLastGroup;

		// enter Length of Last Group
		L[NG - 1] = gdrs.lengthLastGroup;

		// test
		if (mvm != 0)
		{
		  if (totalL != totalNPoints)
		  {
			logger.warn("NPoints != gds.nPts: " + totalL + "!=" + totalNPoints);
			float[] fillData = new float[totalNPoints];
			for (int i = 0; i < totalNPoints; i++)
			{
			  fillData[i] = mv;
			}
			return fillData;
		  }
		}
		else
		{
		  if (totalL != dataNPoints)
		  {
			logger.warn("NPoints != drs.nPts: " + totalL + "!=" + totalNPoints);
			float[] fillData = new float[totalNPoints];
			for (int i = 0; i < totalNPoints; i++)
			{
			  fillData[i] = mv;
			}
			return fillData;
		  }
		}

		float[] data = new float[totalNPoints];

		// [zz +1 ]-nn get X2 values and calculate the results Y using formula
		// formula used to create values, Y * 10**D = R + (X1 + X2) * 2**E

		// Y = (R + (X1 + X2) * (2 ** E) ) / (10 ** D)]
		// WHERE:
		// Y = THE VALUE WE ARE UNPACKING
		// R = THE REFERENCE VALUE (FIRST ORDER MINIMA)
		// X1 = THE PACKED VALUE
		// X2 = THE SECOND ORDER MINIMA
		// E = THE BINARY SCALE FACTOR
		// D = THE DECIMAL SCALE FACTOR
		int count = 0;
		reader.incrByte();
		int dataSize = 0;
		bool[] dataBitMap = null;
		if (mvm == 0)
		{
		  for (int i = 0; i < NG; i++)
		  {
			if (NB[i] != 0)
			{
			  for (int j = 0; j < L[i]; j++)
			  {
				data[count++] = (int) reader.bits2UInt(NB[i]) + X1[i];
			  }
			}
			else
			{
			  for (int j = 0; j < L[i]; j++)
			  {
				data[count++] = X1[i];
			  }
			}
		  } // end for i

		}
		else if (mvm == 1 || mvm == 2)
		{
		  // don't add missing values into data but keep track of them in dataBitMap
		  dataBitMap = new bool[totalNPoints];
		  dataSize = 0;
		  for (int i = 0; i < NG; i++)
		  {
			if (NB[i] != 0)
			{
			  int msng1 = bitsmv1[NB[i]];
			  int msng2 = msng1 - 1;
			  for (int j = 0; j < L[i]; j++)
			  {
				data[count] = (int) reader.bits2UInt(NB[i]);
				if (data[count] == msng1 || mvm == 2 && data[count] == msng2)
				{
				  dataBitMap[count] = false;
				}
				else
				{
				  dataBitMap[count] = true;
				  data[dataSize++] = data[count] + X1[i];
				}
				count++;
			  }
			}
			else
			{ // (NB[i] == 0
			  int msng1 = bitsmv1[gdrs.numberOfBits];
			  int msng2 = msng1 - 1;
			  if (X1[i] == msng1)
			  {
				for (int j = 0; j < L[i]; j++)
				{
				  dataBitMap[count++] = false;
				}
				// data[count++] = X1[i];
			  }
			  else if (mvm == 2 && X1[i] == msng2)
			  {
				for (int j = 0; j < L[i]; j++)
				{
				  dataBitMap[count++] = false;
				}
			  }
			  else
			  {
				for (int j = 0; j < L[i]; j++)
				{
				  dataBitMap[count] = true;
				  data[dataSize++] = X1[i];
				  count++;
				}
			  }
			}
		  } // end for i
		}

		// first order spatial differencing
		if (os == 1)
		{ // g1 and gMin
		  // encoded by G(n) = F(n) - F(n -1 )
		  // decoded by F(n) = G(n) + F(n -1 )
		  // data[] at this point contains G0, G1, G2, ....
		  data[0] = ival1;
		  int itemp;
		  if (mvm == 0)
		  { // no missing values
			itemp = totalNPoints;
		  }
		  else
		  {
			itemp = dataSize;
		  }
		  for (int i = 1; i < itemp; i++)
		  {
			data[i] += minsd;
			data[i] = data[i] + data[i - 1];
		  }
		}
		else if (os == 2)
		{ // 2nd order
		  data[0] = ival1;
		  data[1] = ival2;
		  int itemp;
		  if (mvm == 0)
		  { // no missing values
			itemp = totalNPoints;
		  }
		  else
		  {
			itemp = dataSize;
		  }
		  for (int i = 2; i < itemp; i++)
		  {
			data[i] += minsd;
			data[i] = data[i] + (2 * data[i - 1]) - data[i - 2];
		  }
		}

		if (mvm == 0)
		{ // no missing values
		  for (int i = 0; i < data.Length; i++)
		  {
			data[i] = (R + (data[i] * EE)) / DD;
		  }
		}
		else if (mvm == 1 || mvm == 2)
		{ // missing value == 1 || missing value == 2
		  int count2 = 0;
		  float[] tmp = new float[totalNPoints];
		  for (int i = 0; i < data.Length; i++)
		  {
			if (dataBitMap[i])
			{
			  tmp[i] = (R + (data[count2++] * EE)) / DD;
			}
			else
			{ // mvm = 1 or 2
			  tmp[i] = mv;
			}
		  }
		  data = tmp;
		}

		// bit map is used
		if (bitmap != null)
		{
		  int idx = 0;
		  float[] tmp = new float[totalNPoints];
		  for (int i = 0; i < totalNPoints; i++)
		  {
			if (GribNumbers.testBitIsSet(bitmap[i / 8], i % 8))
			{
			  tmp[i] = data[idx++];
			}
			else
			{
			  tmp[i] = mv;
			}
		  }
		  data = tmp;
		}

		return data;
	  }

	  // Grid point data - JPEG 2000 code stream format
	  private float[] getData40(RandomAccessFile raf, Grib2Drs.Type40 gdrs)
	  {
		// 6-xx jpeg2000 data block to decode

		// dataPoints are number of points encoded, it could be less than the
		// totalNPoints in the grid record if bitMap is used, otherwise equal
		// int dataPoints = drs.getDataPoints();

		int nb = gdrs.numberOfBits;
		int D = gdrs.decimalScaleFactor;
		float DD = (float) Math.Pow((double) 10, (double) D);
		float R = gdrs.referenceValue;
		int E = gdrs.binaryScaleFactor;
		float EE = (float) Math.Pow(2.0, (double) E);
		float ref_val = R / DD;

		Grib2JpegDecoder g2j = null;
		// try {
		if (nb != 0)
		{ // there's data to decode
		  g2j = new Grib2JpegDecoder(nb, false);
		  byte[] buf = new byte[dataLength - 5];
		  raf.readFully(buf);
		  g2j.decode(buf);
		  gdrs.hasSignedProblem_Conflict = g2j.hasSignedProblem();
		}

		float[] result = new float[totalNPoints];

		// no data to decode, set to reference value
		if (nb == 0)
		{
		  for (int i = 0; i < dataNPoints; i++)
		  {
			result[i] = ref_val;
		  }
		  return result;
		}

		int[] idata = g2j.Gdata;
		if (bitmap == null)
		{ // must be one decoded value in idata for every expected data point
		  if (idata.Length != dataNPoints)
		  {
			logger.debug("Number of points in the data record {} != {} expected from GDS", idata.Length, dataNPoints);
			throw new System.InvalidOperationException("Number of points in the data record {} != expected from GDS");
		  }

		  for (int i = 0; i < dataNPoints; i++)
		  {
			// Y * 10^D = R + (X1 + X2) * 2^E ; // regulation 92.9.4
			// Y = (R + ( 0 + X2) * EE)/DD ;
			result[i] = (R + idata[i] * EE) / DD;
		  }
		  return result;

		}
		else
		{ // use bitmap to skip missing values
		  for (int i = 0, j = 0; i < totalNPoints; i++)
		  {
			if (GribNumbers.testBitIsSet(bitmap[i / 8], i % 8))
			{
			  if (j >= idata.Length)
			  {
				logger.warn("jj2000 data count {} < bitmask count {}, i={}, totalNPoints={}", idata.Length, j, i, totalNPoints);
				break;
			  }
			  int indata = idata[j];
			  result[i] = (R + indata * EE) / DD;
			  j++;
			}
			else
			{
			  result[i] = staticMissingValue;
			}
		  }
		}
		return result;
	  }

	  // Grid point data - JPEG 2000 code stream format
	  private int[] getData40raw(RandomAccessFile raf, Grib2Drs.Type40 gdrs)
	  {
		int nb = gdrs.numberOfBits;
		if (nb == 0)
		{
		  return null;
		}
		int missing_value = (2 << nb - 1) - 1; // all ones - reserved for missing value

		Grib2JpegDecoder g2j;
		g2j = new Grib2JpegDecoder(nb, false);
		byte[] buf = new byte[dataLength - 5];
		raf.readFully(buf);
		g2j.decode(buf);
		gdrs.hasSignedProblem_Conflict = g2j.hasSignedProblem();

		int[] idata = g2j.Gdata;

		if (bitmap == null)
		{ // must be one decoded value in idata for every expected data point
		  if (idata.Length != totalNPoints)
		  {
			logger.debug("Number of points in the data record {} != {} expected from GDS", idata.Length, totalNPoints);
			return null;
		  }
		  return idata;

		}
		else
		{ // use bitmap to skip missing values
		  int[] result = new int[totalNPoints];

		  for (int i = 0, j = 0; i < totalNPoints; i++)
		  {
			if (GribNumbers.testBitIsSet(bitmap[i / 8], i % 8))
			{
			  if (j >= idata.Length)
			  {
				logger.warn("jj2000 data count {} < bitmask count {}, i={}, totalNPoints={}", idata.Length, j, i, totalNPoints);
				break;
			  }
			  result[i] = idata[j];
			  j++;
			}
			else
			{
			  result[i] = missing_value;
			}
		  }
		  return result;
		}

	  }

	  // Grid point data - PNG
	  // Requires java.awt image decoding (ImageIO/BufferedImage), which is not ported.
	  private float[] getData41(RandomAccessFile raf, Grib2Drs.Type0 gdrs)
	  {
		throw new System.NotSupportedException("PNG (data template 41) decoding is not ported");
	  }

	  // by jkaehler@meteomatics.com
	  // ported from
	  // https://github.com/erdc-cm/grib_api/blob/master/src/grib_accessor_class_data_g1second_order_general_extended_packing.c
	  private float[] getData50002(RandomAccessFile raf, Grib2Drs.Type50002 gdrs)
	  {

		BitReader reader;

		reader = new BitReader(raf, startPos + 5);
		int[] groupWidth = new int[gdrs.p1];
		for (int i = 0; i < gdrs.p1; i++)
		{
		  groupWidth[i] = (int) reader.bits2UInt(gdrs.widthOfWidth);
		}

		reader = new BitReader(raf, raf.FilePointer);
		int[] groupLength = new int[gdrs.p1];
		for (int i = 0; i < gdrs.p1; i++)
		{
		  groupLength[i] = (int) reader.bits2UInt(gdrs.widthOfLength);
		}

		reader = new BitReader(raf, raf.FilePointer);
		int[] firstOrderValues = new int[gdrs.p1];
		for (int i = 0; i < gdrs.p1; i++)
		{
		  firstOrderValues[i] = (int) reader.bits2UInt(gdrs.widthOfFirstOrderValues);
		}

		int bias = 0;
		if (gdrs.orderOfSPD > 0)
		{
		  bias = gdrs.spd[gdrs.orderOfSPD];
		}

		reader = new BitReader(raf, raf.FilePointer);
		int cnt = gdrs.orderOfSPD;
		int[] data = new int[totalNPoints];
		for (int i = 0; i < gdrs.p1; i++)
		{
		  if (groupWidth[i] > 0)
		  {

			for (int j = 0; j < groupLength[i]; j++)
			{
			  data[cnt] = (int) reader.bits2UInt(groupWidth[i]);
			  data[cnt] += firstOrderValues[i];
			  cnt++;
			}

		  }
		  else
		  {

			for (int j = 0; j < groupLength[i]; j++)
			{
			  data[cnt] = firstOrderValues[i];
			  cnt++;
			}

		  }

		}

		if (gdrs.orderOfSPD >= 0)
		{
		  System.Array.Copy(gdrs.spd, 0, data, 0, gdrs.orderOfSPD);
		}

		int y, z, w;
		switch (gdrs.orderOfSPD)
		{
		  case 1:
			y = data[0];
			for (int i = 1; i < totalNPoints; i++)
			{
			  y += data[i] + bias;
			  data[i] = y;
			}

			break;
		  case 2:
			y = data[1] - data[0];
			z = data[1];
			for (int i = 2; i < totalNPoints; i++)
			{
			  y += data[i] + bias;
			  z += y;
			  data[i] = z;
			}

			break;
		  case 3:
			y = data[2] - data[1];
			z = y - (data[1] - data[0]);
			w = data[2];
			for (int i = 3; i < totalNPoints; i++)
			{
			  z += data[i] + bias;
			  y += z;
			  w += y;
			  data[i] = w;
			}

			break;
		}

		// LOOK see https://github.com/Unidata/netcdf-java/issues/881
		int D = gdrs.decimalScaleFactor;
		float DD = (float) Math.Pow((double) 10, (double) -D);
		float R = gdrs.referenceValue;
		int E = gdrs.binaryScaleFactor;
		float EE = (float) Math.Pow(2.0, (double) E);

		float[] ret = new float[totalNPoints];
		for (int i = 0; i < totalNPoints; i++)
		{
		  ret[i] = (((data[i] * EE) + R) * DD);
		}

		return ret;

	  }

	  private float[] getData42(RandomAccessFile raf, Grib2Drs.Type42 gdrs)
	  {
		// CCSDS (template 42) decoding requires the libaec native library (AecStream/LibAec), which is not ported.
		throw new System.NotSupportedException("CCSDS (data template 42) decoding is not ported (requires native libaec)");
	  }

	  // Rearrange the data array using the scanning mode.
	  // LOOK: not handling scanMode generally
	  // LOOK might be wrong for a quasi regular (thin) grid ??
	  private void scanningModeCheck(float[] data, int scanMode, int Xlength)
	  {
		// Mode 0 +x, -y, adjacent x, adjacent rows same dir
		// Mode 64 +x, +y, adjacent x, adjacent rows same dir
		if ((scanMode == 0) || (scanMode == 64)) // dont flip Y - handle it in the HorizCoordSys
		{
		  return;
		}

		// change -x to +x ie east to west -> west to east
		if (!GribUtils.scanModeXisPositive(scanMode))
		{
		  float tmp;
		  int mid = Xlength / 2;
		  for (int index = 0; index < data.Length; index += Xlength)
		  {
			for (int idx = 0; idx < mid; idx++)
			{
			  tmp = data[index + idx];
			  data[index + idx] = data[index + Xlength - idx - 1];
			  data[index + Xlength - idx - 1] = tmp;
			}
		  }
		  return;
		}

		if (!GribUtils.scanModeSameDirection(scanMode))
		{
		  float tmp;
		  int mid = Xlength / 2;
		  for (int index = 0; index < data.Length; index += Xlength)
		  {
			int row = index / Xlength;
			if (row % 2 != 0)
			{ // odd numbered row, calculate reverse index
			  for (int idx = 0; idx < mid; idx++)
			  {
				tmp = data[index + idx];
				data[index + idx] = data[index + Xlength - idx - 1];
				data[index + Xlength - idx - 1] = tmp;
			  }
			}
		  }
		}
	  }

	}

}
