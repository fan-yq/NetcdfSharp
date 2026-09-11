// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.grib.grib2
{
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.quantization.dequantizer;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.image.invcomptransf;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.fileformat.reader;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.codestream.reader;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.wavelet.synthesis;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.entropy.decoder;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.decoder;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.image.output;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.codestream;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.image;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.util;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.roi;
	using NetcdfSharp.ucar.jpeg.jj2000.j2k.io;
	using NetcdfSharp.ucar.jpeg.colorspace;
	using NetcdfSharp.ucar.jpeg.icc;

	/// <summary>
	/// Adaptation of jj2000.j2k.decoder.Decoder, in order to read input from memory.
	/// 
	/// @author robb kambic
	/// @author caron rewritten Aug 2014
	/// </summary>
	public class Grib2JpegDecoder
	{
	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(Grib2JpegDecoder));

	  private bool debug;
	  private readonly int rate;

	  /// <summary>
	  /// Return the packBytes ie number bytes / number 1,2 or 4
	  /// </summary>
	  private int packBytes;

	  /// <summary>
	  /// Parses the inputstream to analyze the box structure of the JP2 file.
	  /// </summary>
	  private ColorSpace csMap;

	  /// <summary>
	  /// The exit code of the run method
	  /// </summary>
	  private int exitCode;

	  /// <summary>
	  /// The parameter list (arguments)
	  /// </summary>
	  private readonly ParameterList pl;

	  /// <summary>
	  /// Instantiates a decoder object, width the 'argv' command
	  /// line arguments. It also initializes the default parameters. If the
	  /// argument list is empty an IllegalArgumentException is thrown. If an
	  /// error occurs while parsing the arguments error messages are written to
	  /// stderr and the run exit code is set to non-zero, see getExitCode()
	  /// </summary>
	  /// <exception cref="IllegalArgumentException"> If 'argv' is empty </exception>
	  /// <seealso cref="Grib2JpegDecoder.getExitCode"/>
	  internal Grib2JpegDecoder(int nbits, bool debug)
	  {
		this.rate = nbits;
		this.debug = debug;

		// not sure if these are needed in the bowels of jj2000
		string[] argv = new string[6];
		argv[0] = "-rate";
		argv[1] = Convert.ToString(nbits);
		argv[2] = "-verbose";
		argv[3] = "off";
		argv[4] = "-debug";
		argv[5] = "on";

		// Initialize default parameters
		// System.err.println("calling Grib2JpegDecoder with argv argument");
		/*
		 * The default parameter list (with modules arguments)
		 */
		ParameterList defpl = new ParameterList();
		string[][] param = Grib2JpegDecoder.AllParameters;

		for (int i = param.Length - 1; i >= 0; i--)
		{
		  if (!string.ReferenceEquals(param[i][3], null))
		  {
			defpl.put(param[i][0], param[i][3]);
		  }
		}

		// Create parameter list using defaults
		pl = new ParameterList(defpl);

		// Parse arguments from argv
		try
		{
		  pl.parseArgs(argv);
		}
		catch (StringFormatException e)
		{
		  Console.Error.WriteLine("An error occurred while parsing the arguments: " + e.Message);
		}
	  } // end Grib2JpegDecoder constructor

	  /// <summary>
	  /// Returns the exit code of the class. This is only initialized after the
	  /// constructor and when the run method returns.
	  /// </summary>
	  /// <returns> The exit code of the constructor and the run() method. </returns>
	  public virtual int ExitCode
	  {
		  get
		  {
			return exitCode;
		  }
	  }

// JAVA TO C# CONVERTER NOTE: Field name conflicts with a method name of the current type:
	  private bool hasSignedProblem_Conflict;

	  public virtual bool hasSignedProblem()
	  {
		return hasSignedProblem_Conflict;
	  }

	  /// <summary>
	  /// Runs the decoder. After completion the exit code is set, a non-zero
	  /// value indicates that an error occurred.
	  /// </summary>
	  /// <seealso cref=".getExitCode"/>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void decode(byte[] buf) throws NetcdfSharp.java.io.IOException
	  public virtual void decode(byte[] buf)
	  {
		// int dataSize = buf.length;
		bool verbose = false;
		int res; // resolution level to reconstruct
		FileFormatReader ff;
		HeaderDecoder hd;
		EntropyDecoder entdec;
		ROIDeScaler roids;
		Dequantizer deq;
		InverseWT invWT;
		InvCompTransf ictransf;
		// ImgWriter imwriter[];
		ImgDataConverter converter;
		DecoderSpecs decSpec;
		BlkImgDataSrc palettized;
		BlkImgDataSrc channels;
		BlkImgDataSrc resampled;
		BlkImgDataSrc color;
		int i;
		int[] depth;

		try
		{

		  // create a ByteArrayInputStream from byte array for ISRandomAccessIO
		  MemoryStream bais = new MemoryStream(buf);
		  RandomAccessIO @in = new ISRandomAccessIO(bais, buf.Length, 1, buf.Length);

		  // **** File Format ****
		  // If the codestream is wrapped in the jp2 fileformat, Read the
		  // file format wrapper
		  ff = new FileFormatReader(@in);
		  ff.readFileFormat();
		  if (ff.JP2FFUsed)
		  {
			@in.seek(ff.getFirstCodeStreamPos());
			logger.warn("ff.JP2FFUsed is used"); // LOOK probably not
		  }

		  // +----------------------------+
		  // | Instantiate decoding chain |
		  // +----------------------------+

		  // **** Header decoder ****
		  // Instantiate header decoder and read main header
		  /*
		   * Information contained in the codestream's headers
		   */
		  HeaderInfo hi = new HeaderInfo();
		  try
		  {
			hd = new HeaderDecoder(@in, pl, hi);
		  }
		  catch (EOFException e)
		  {
			error("Codestream too short or bad header, unable to decode.", 2, e);
			throw;
		  }

		  int nCompCod = hd.getNumComps();
		  decSpec = hd.getDecoderSpecs();

		  // Get demixed bitdepths
		  depth = new int[nCompCod];
		  for (i = 0; i < nCompCod; i++)
		  {
			depth[i] = hd.getOriginalBitDepth(i);
		  }

		  // **** Bit stream reader ****
		  BitstreamReaderAgent breader = BitstreamReaderAgent.createInstance(@in, hd, pl, decSpec, false, hi);

		  // **** Entropy decoder ****
		  try
		  {
			entdec = hd.createEntropyDecoder(breader, pl);
		  }
		  catch (ArgumentException e)
		  {
			error("Cannot instantiate entropy decoder", 2, e);
			return;
		  }

		  // **** ROI de-scaler ****
		  try
		  {
			roids = hd.createROIDeScaler(entdec, pl, decSpec);
		  }
		  catch (ArgumentException e)
		  {
			error("Cannot instantiate roi de-scaler", 2, e);
			return;
		  }

		  // **** Dequantizer ****
		  try
		  {
			deq = hd.createDequantizer(roids, depth, decSpec);
		  }
		  catch (ArgumentException e)
		  {
			error("Cannot instantiate dequantizer", 2, e);
			return;
		  }

		  // **** Inverse wavelet transform ***
		  try
		  {
			// full page inverse wavelet transform
			invWT = InverseWT.createInstance(deq, decSpec);
		  }
		  catch (ArgumentException e)
		  {
			error("Cannot instantiate inverse wavelet transform", 2, e);
			return;
		  }

		  res = breader.getImgRes();
		  invWT.setImgResLevel(res);

		  // **** Data converter **** (after inverse transform module)
		  converter = new ImgDataConverter(invWT, 0);

		  // **** Inverse component transformation ****
		  ictransf = new InvCompTransf(converter, decSpec, depth, pl);

		  // **** Color space mapping ****
		  string p = pl.getParameter("nocolorspace");
		  bool nocolorspace = "off".Equals(p); // LOOK not sure what default is here
		  if (ff.JP2FFUsed && nocolorspace)
		  {
			try
			{
			  csMap = new ColorSpace(@in, hd, pl);
			  channels = hd.createChannelDefinitionMapper(ictransf, csMap);
			  resampled = hd.createResampler(channels, csMap);
			  palettized = hd.createPalettizedColorSpaceMapper(resampled, csMap);
			  color = hd.createColorSpaceMapper(palettized, csMap);

			}
			catch (ArgumentException e)
			{
			  error("Could not instantiate ICC profiler", 1, e);
			  return;
			}
			catch (ColorSpaceException e)
			{
			  error("error processing jp2 colorspace information", 1, e);
			  return;
			}
		  }
		  else
		  { // Skip colorspace mapping
			color = ictransf;
		  }

		  // This is the last image in the decoding chain and should be
		  // assigned by the last transformation:
		  BlkImgDataSrc decodedImage = color;
		  if (color == null)
		  {
			decodedImage = ictransf;
		  }

		  int nCompImg = decodedImage.getNumComps();

		  // code to get data
		  // **** Decode and write/display result ****

		  ImgWriter[] imwriter = new ImgWriter[nCompImg];

		  // Now write the image to the array (decodes as needed)
		  bool isSigned;
		  for (i = 0; i < imwriter.Length; i++)
		  {
			try
			{
			  if (csMap != null)
			  {
				isSigned = csMap.isOutputSigned(i);
				imwriter[i] = new ImgWriterArray(decodedImage, i, csMap.isOutputSigned(i));
			  }
			  else
			  {
				isSigned = hd.isOriginalSigned(i);
				imwriter[i] = new ImgWriterArray(decodedImage, i, hd.isOriginalSigned(i));
			  }
			}
			catch (IOException e)
			{
			  if (debug)
			  {
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			  }
			  return;
			}

			try
			{
			  imwriter[i].writeAll(); // write data to array
			  ImgWriterArray iwa = (ImgWriterArray) imwriter[i];
			  data = iwa.Gdata;
			  // unSigned data processing here
			  if (!isSigned)
			  {
				// float unSignIt = (float) NetcdfSharp.java.lang.Math.pow((double) 2.0, fnb - 1); // LOOK WTF ?
				int nb = depth[i];
				int levShift = 1 << (nb - 1); // check
				if (nb != rate)
				{
				  hasSignedProblem_Conflict = true;
				}

				for (int j = 0; j < data.Length; j++)
				{
				  data[j] += levShift;
				}
			  }
			  packBytes = iwa.PackBytes;
			}
			catch (IOException e)
			{
			  if (debug)
			  {
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			  }
			  return;
			}
		  } // end for(i=0; i<imwriter.length; i++)

		  // **** Print some resulting info ****
		  if (verbose)
		  {
			// Print actually read bitrate
			// if file format used add the read file format bytes
			float bitrate = breader.getActualRate();
			int numBytes = breader.getActualNbytes();
			if (ff.JP2FFUsed)
			{
			  int imageSize = (int)((8.0f * numBytes) / bitrate);
			  numBytes += ff.getFirstCodeStreamPos();
			  bitrate = (numBytes * 8.0f) / imageSize;
			}

			if (pl.getIntParameter("ncb_quit") == -1)
			{
			  logger.info("Actual bit rate = " + bitrate + " bpp (i.e. " + numBytes + " bytes)");
			}
			else
			{
			  logger.info("object of packet body bytes read = " + numBytes);
			}
		  }

		}
		catch (ArgumentException e)
		{
		  error(e.Message, 2);
		  if (debug)
		  {
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		  }

		}
		catch (Exception e)
		{
		  error("An uncaught runtime exception has occurred", 2, e);
		  throw new IOException(e.Message, e);

		}
	  } // end decode

	  private void error(string msg, int code)
	  {
		exitCode = code;
		logger.debug(msg);
	  }

	  private void error(string msg, int code, Exception e)
	  {
		exitCode = code;
		logger.debug(string.Format("{0}={1}", msg, e.Message));
		if (debug)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
	  }

	  /// <summary>
	  /// Return the packBytes ie number bytes / number 1,2 or 4
	  /// </summary>
	  public virtual int PackBytes
	  {
		  get
		  {
			return packBytes;
		  }
	  }

	  /// <summary>
	  /// Return the "raw" decoded data as an int array
	  /// </summary>
	  public virtual int[] Gdata
	  {
		  get
		  {
			return data;
		  }
	  }

	  private int[] data;

	  /// <summary>
	  /// Returns all the parameters used in the decoding chain. It calls
	  /// parameter from each module and store them in one array (one row per
	  /// parameter and 4 columns).
	  /// </summary>
	  /// <returns> All decoding parameters </returns>
	  public static string[][] AllParameters
	  {
		  get
		  {
			IList<string[]> vec = new List<string[]>();
			int i;

			string[][] str = BitstreamReaderAgent.getParameterInfo();
			if (str != null)
			{
			  for (i = str.Length - 1; i >= 0; i--)
			  {
				vec.Add(str[i]);
			  }
			}

			str = EntropyDecoder.getParameterInfo();
			if (str != null)
			{
			  for (i = str.Length - 1; i >= 0; i--)
			  {
				vec.Add(str[i]);
			  }
			}

			str = ROIDeScaler.getParameterInfo();
			if (str != null)
			{
			  for (i = str.Length - 1; i >= 0; i--)
			  {
				vec.Add(str[i]);
			  }
			}

			str = Dequantizer.getParameterInfo();
			if (str != null)
			{
			  for (i = str.Length - 1; i >= 0; i--)
			  {
				vec.Add(str[i]);
			  }
			}

			str = InvCompTransf.getParameterInfo();
			if (str != null)
			{
			  for (i = str.Length - 1; i >= 0; i--)
			  {
				vec.Add(str[i]);
			  }
			}

			str = HeaderDecoder.getParameterInfo();
			if (str != null)
			{
			  for (i = str.Length - 1; i >= 0; i--)
			  {
				vec.Add(str[i]);
			  }
			}

			str = ICCProfiler.getParameterInfo();
			if (str != null)
			{
			  for (i = str.Length - 1; i >= 0; i--)
			  {
				vec.Add(str[i]);
			  }
			}

			str = NetcdfSharp.ucar.jpeg.jj2000.j2k.decoder.Decoder.getParameterInfo();
			if (str != null)
			{


			  for (i = str.Length - 1; i >= 0; i--)
			  {
				vec.Add(str[i]);
			  }
			}

			string[][] result = new string[vec.Count][];
			for (i = vec.Count - 1; i >= 0; i--)
			{
			  result[i] = vec[i];
			}

			return result;
		  }
	  }

	  /// <summary>
	  /// This class extends the ImgWriter abstract class for writing Array .
	  /// <para/>
	  /// <u>Data:</u> The image binary values appear one after the other (in raster
	  /// order) immediately after the last header character ('\n') and are
	  /// byte-aligned (they are packed into 1,2 or 4 bytes per sample, depending
	  /// upon the bit-depth value).
	  /// </para>
	  /// <para/>
	  /// <para>
	  /// If the data is unsigned, level shifting is applied adding 2^(bit depth
	  /// - 1)
	  /// </para>
	  /// <para/>
	  /// <para>
	  /// <u>NOTE</u>: This class is not thread safe, for reasons of internal
	  /// buffering.
	  /// </para>
	  /// </summary>
	  /// <seealso cref="ImgWriter"/>
	  /// <seealso cref="BlkImgDataSrc"/>
	  private class ImgWriterArray : ImgWriter
	  {

		/// <summary>
		/// Whether the data must be signed when writing or not. In the latter
		/// case inverse level shifting must be applied
		/// </summary>
		internal readonly bool isSigned;

		/// <summary>
		/// The bit-depth of the input file (must be between 1 and 31)
		/// </summary>
		private readonly int bitDepth;

		/// <summary>
		/// A DataBlk, just used to avoid allocating a new one each time it is
		/// needed
		/// </summary>
		internal DataBlkInt db = new DataBlkInt();

		// The number of fractional bits in the source data */
		// private int fb;

		/// <summary>
		/// The index of the component from where to get the data
		/// </summary>
		private readonly int c;

		/// <summary>
		/// The pack length of one sample (in bytes, according to the output
		/// bit-depth
		/// </summary>
		private int packBytes;

		/// <summary>
		/// Creates a new writer to the specified Array object, to write data from
		/// the specified component.
		/// <para/>
		/// <para>
		/// The size of the image that is written to the file is the size of the
		/// component from which to get the data, specified by b, not the size of
		/// the source image (they differ if there is some sub-sampling).
		/// </para>
		/// </summary>
		/// <param name="imgSrc"> The source from where to get the image data to write. </param>
		/// <param name="c"> The index of the component from where to get the data. </param>
		/// <param name="isSigned"> Whether the data are signed or not (needed only when
		///        writing header). </param>
		/// <seealso cref="DataBlk"/>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: ImgWriterArray(BlkImgDataSrc imgSrc, int c, boolean isSigned) throws NetcdfSharp.java.io.IOException
		internal ImgWriterArray(BlkImgDataSrc imgSrc, int c, bool isSigned)
		{
		  // Initialize
		  this.c = c;
		  this.isSigned = isSigned;
		  src = imgSrc;
		  w = src.getImgWidth();
		  h = src.getImgHeight();

		  bitDepth = src.getNomRangeBits(this.c);
		  if ((bitDepth <= 0) || (bitDepth > 31))
		  {
			throw new IOException("Array supports only bit-depth between 1 and 31");
		  }
		  if (bitDepth <= 8)
		  {
			packBytes = 1;
		  }
		  else if (bitDepth <= 16)
		  {
			packBytes = 2;
		  }
		  else
		  { // <= 31
			packBytes = 4;
		  }

		} // end ImgWriterArray

		/// <summary>
		/// Closes the underlying file or network connection to where the data is
		/// written. Any call to other methods of the class become illegal after a
		/// call to this one.
		///
		/// </summary>
		public override void close()
		{
		}

		/// <summary>
		/// Writes the data of the specified area to the file, coordinates are
		/// relative to the current tile of the source. Before writing, the
		/// coefficients are limited to the nominal range and packed into 1,2 or 4
		/// bytes (according to the bit-depth).
		/// <para/>
		/// <para>
		/// If the data is unisigned, level shifting is applied adding 2^(bit depth - 1)
		/// </para>
		/// <para/>
		/// <para>
		/// This method may not be called concurrently from different threads.
		/// </para>
		/// <para/>
		/// <para>
		/// If the data returned from the BlkImgDataSrc source is progressive,
		/// then it is requested over and over until it is not progressive
		/// anymore.
		/// </para>
		/// </summary>
		/// <param name="ulx"> The horizontal coordinate of the upper-left corner of the
		///        area to write, relative to the current tile. </param>
		/// <param name="uly"> The vertical coordinate of the upper-left corner of the area
		///        to write, relative to the current tile. </param>
		/// <param name="w"> The width of the area to write. </param>
		/// <param name="h"> The height of the area to write. </param>
		public override void write(int ulx, int uly, int w, int h)
		{
		  // Initialize db
		  db.ulx = ulx;
		  db.uly = uly;
		  db.w = w;
		  db.h = h;
		  if (db.data != null && db.data.Length < w * h)
		  {
			// A new one will be allocated by getInternCompData()
			db.data = null;
		  }
		  // Request the data and make sure it is not
		  // progressive
		  do
		  {
			db = (DataBlkInt) src.getInternCompData(db, c);
		  } while (db.progressive);

		} // end int ulx, int uly, int w, int h

		public override void writeAll()
		{
		  // Find the list of tile to decode.
		  Coord nT = src.getNumTiles(null);

		  // Loop on vertical tiles
		  for (int y = 0; y < nT.y; y++)
		  {
			// Loop on horizontal tiles
			for (int x = 0; x < nT.x; x++)
			{
			  src.setTile(x, y);
			  write(0, 0, src.getImgWidth(), src.getImgHeight());
			} // End loop on horizontal tiles
		  } // End loop on vertical tiles
		}

		/// <summary>
		/// Writes the source's current tile to the output. The requests of data
		/// issued to the source BlkImgDataSrc object are done by strips, in order
		/// to reduce memory usage.
		/// <para/>
		/// <para>
		/// If the data returned from the BlkImgDataSrc source is progressive,
		/// then it is requested over and over until it is not progressive
		/// anymore.
		/// </para>
		/// </summary>
		/// <seealso cref="DataBlk"/>
		public override void write()
		{
		  int i;
		  int tIdx = src.getTileIdx();
		  int tw = src.getTileCompWidth(tIdx, c); // Tile width
		  int th = src.getTileCompHeight(tIdx, c); // Tile height
		  // Write in strips
		  for (i = 0; i < th; i += DEF_STRIP_HEIGHT)
		  {
			write(0, i, tw, (th - i < DEF_STRIP_HEIGHT) ? th - i : DEF_STRIP_HEIGHT);
		  }
		}

		/// <summary>
		/// The pack length of one sample (in bytes, according to the output bit-depth
		/// </summary>
		internal virtual int PackBytes
		{
			get
			{
			  return packBytes;
			}
		}

		/// <summary>
		/// the jpeg data decoded into an int array
		/// </summary>
		/// <returns> a int[] </returns>
		internal virtual int[] Gdata
		{
			get
			{
			  return db.data;
			}
		}

		public override void flush()
		{
		}

		public override string ToString()
		{
		  return MoreObjects.toStringHelper(this).add("isSigned", isSigned).add("bitDepth", bitDepth).add("component", c).add("w", w).add("h", h).ToString();
		}
	  } // end ImgWriterArray

	} // end Grib2JpegDecoder

}
