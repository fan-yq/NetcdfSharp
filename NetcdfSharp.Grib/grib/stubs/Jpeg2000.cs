// Minimal stubs for the jj2000 JPEG2000 decoding chain (ucar.jpeg.*) referenced by
// Grib2JpegDecoder. JPEG2000 decoding is out of scope ("Direction A"): these types only
// need to compile, they do not perform any real decoding.

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.codestream
{
	/// <summary>Stub for jj2000.j2k.ParameterList.</summary>
	public class ParameterList
	{
		private readonly System.Collections.Generic.Dictionary<string, string> @params;

		public ParameterList()
		{
			@params = new System.Collections.Generic.Dictionary<string, string>();
		}

		public ParameterList(ParameterList defpl)
		{
			@params = new System.Collections.Generic.Dictionary<string, string>();
			if (defpl != null)
			{
				foreach (System.Collections.Generic.KeyValuePair<string, string> kv in defpl.@params)
				{
					@params[kv.Key] = kv.Value;
				}
			}
		}

		public virtual void put(string name, string value)
		{
			@params[name] = value;
		}

		public virtual void parseArgs(string[] argv)
		{
		}

		public virtual string getParameter(string name)
		{
			return @params.TryGetValue(name, out string value) ? value : null;
		}

		public virtual int getIntParameter(string name)
		{
			return @params.TryGetValue(name, out string value) && int.TryParse(value, out int result) ? result : -1;
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.util
{
	public class StringFormatException : System.Exception
	{
		public StringFormatException()
		{
		}

		public StringFormatException(string message) : base(message)
		{
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.io
{
	public class RandomAccessIO
	{
		public virtual void seek(int pos)
		{
		}
	}

	public class ISRandomAccessIO : RandomAccessIO
	{
		internal ISRandomAccessIO(System.IO.Stream inStream, int size, int offset, int dataSize)
		{
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.fileformat.reader
{
	using RandomAccessIO = NetcdfSharp.ucar.jpeg.jj2000.j2k.io.RandomAccessIO;

	public class FileFormatReader
	{
		internal FileFormatReader(RandomAccessIO @in)
		{
		}

		public virtual void readFileFormat()
		{
		}

		public virtual bool JP2FFUsed
		{
			get
			{
				return false;
			}
		}

		public virtual int getFirstCodeStreamPos()
		{
			return 0;
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.image
{
	/// <summary>Stub for jj2000.j2k.image.BlkImgDataSrc.</summary>
	public class BlkImgDataSrc
	{
		public virtual int getNumComps()
		{
			return 1;
		}

		public virtual int getImgWidth()
		{
			return 0;
		}

		public virtual int getImgHeight()
		{
			return 0;
		}

		public virtual int getNomRangeBits(int c)
		{
			return 8;
		}

		public virtual DataBlkInt getInternCompData(DataBlkInt db, int c)
		{
			return db;
		}

		public virtual Coord getNumTiles(object dummy)
		{
			return new Coord();
		}

		public virtual void setTile(int x, int y)
		{
		}

		public virtual int getTileIdx()
		{
			return 0;
		}

		public virtual int getTileCompWidth(int tIdx, int c)
		{
			return 0;
		}

		public virtual int getTileCompHeight(int tIdx, int c)
		{
			return 0;
		}
	}

	public class DataBlkInt
	{
		public int ulx;
		public int uly;
		public int w;
		public int h;
		public int[] data;
		public bool progressive;
	}

	public class Coord
	{
		public int x;
		public int y;
	}

	public class ImgDataConverter : BlkImgDataSrc
	{
		internal ImgDataConverter(BlkImgDataSrc src, int c)
		{
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.codestream.reader
{
	using RandomAccessIO = NetcdfSharp.ucar.jpeg.jj2000.j2k.io.RandomAccessIO;
	using ParameterList = NetcdfSharp.ucar.jpeg.jj2000.j2k.codestream.ParameterList;
	using BlkImgDataSrc = NetcdfSharp.ucar.jpeg.jj2000.j2k.image.BlkImgDataSrc;
	using EntropyDecoder = NetcdfSharp.ucar.jpeg.jj2000.j2k.entropy.decoder.EntropyDecoder;
	using ROIDeScaler = NetcdfSharp.ucar.jpeg.jj2000.j2k.roi.ROIDeScaler;
	using Dequantizer = NetcdfSharp.ucar.jpeg.jj2000.j2k.quantization.dequantizer.Dequantizer;
	using ColorSpace = NetcdfSharp.ucar.jpeg.colorspace.ColorSpace;

	public class HeaderInfo
	{
	}

	public class DecoderSpecs
	{
	}

	public class HeaderDecoder
	{
		internal HeaderDecoder(RandomAccessIO @in, ParameterList pl, HeaderInfo hi)
		{
		}

		public virtual int getNumComps()
		{
			return 1;
		}

		public virtual DecoderSpecs getDecoderSpecs()
		{
			return new DecoderSpecs();
		}

		public virtual int getOriginalBitDepth(int i)
		{
			return 8;
		}

		public virtual EntropyDecoder createEntropyDecoder(BitstreamReaderAgent breader, ParameterList pl)
		{
			return null;
		}

		public virtual ROIDeScaler createROIDeScaler(EntropyDecoder entdec, ParameterList pl, DecoderSpecs decSpec)
		{
			return null;
		}

		public virtual Dequantizer createDequantizer(ROIDeScaler roids, int[] depth, DecoderSpecs decSpec)
		{
			return null;
		}

		public virtual BlkImgDataSrc createChannelDefinitionMapper(BlkImgDataSrc ictransf, ColorSpace csMap)
		{
			return null;
		}

		public virtual BlkImgDataSrc createResampler(BlkImgDataSrc channels, ColorSpace csMap)
		{
			return null;
		}

		public virtual BlkImgDataSrc createPalettizedColorSpaceMapper(BlkImgDataSrc resampled, ColorSpace csMap)
		{
			return null;
		}

		public virtual BlkImgDataSrc createColorSpaceMapper(BlkImgDataSrc palettized, ColorSpace csMap)
		{
			return null;
		}

		public virtual bool isOriginalSigned(int i)
		{
			return false;
		}

		public static string[][] getParameterInfo()
		{
			return null;
		}
	}

	public class BitstreamReaderAgent
	{
		public static BitstreamReaderAgent createInstance(RandomAccessIO @in, HeaderDecoder hd, ParameterList pl, DecoderSpecs decSpec, bool on, HeaderInfo hi)
		{
			return null;
		}

		public virtual int getImgRes()
		{
			return 0;
		}

		public virtual float getActualRate()
		{
			return 0;
		}

		public virtual int getActualNbytes()
		{
			return 0;
		}

		public static string[][] getParameterInfo()
		{
			return null;
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.quantization.dequantizer
{
	public class Dequantizer
	{
		public static string[][] getParameterInfo()
		{
			return null;
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.roi
{
	public class ROIDeScaler
	{
		public static string[][] getParameterInfo()
		{
			return null;
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.entropy.decoder
{
	public class EntropyDecoder
	{
		public static string[][] getParameterInfo()
		{
			return null;
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.wavelet.synthesis
{
	using BlkImgDataSrc = NetcdfSharp.ucar.jpeg.jj2000.j2k.image.BlkImgDataSrc;
	using Dequantizer = NetcdfSharp.ucar.jpeg.jj2000.j2k.quantization.dequantizer.Dequantizer;
	using DecoderSpecs = NetcdfSharp.ucar.jpeg.jj2000.j2k.codestream.reader.DecoderSpecs;

	public class InverseWT : BlkImgDataSrc
	{
		public static InverseWT createInstance(Dequantizer deq, DecoderSpecs decSpec)
		{
			return null;
		}

		public virtual void setImgResLevel(int level)
		{
		}

		public static string[][] getParameterInfo()
		{
			return null;
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.image.invcomptransf
{
	using BlkImgDataSrc = NetcdfSharp.ucar.jpeg.jj2000.j2k.image.BlkImgDataSrc;
	using ImgDataConverter = NetcdfSharp.ucar.jpeg.jj2000.j2k.image.ImgDataConverter;
	using DecoderSpecs = NetcdfSharp.ucar.jpeg.jj2000.j2k.codestream.reader.DecoderSpecs;
	using ParameterList = NetcdfSharp.ucar.jpeg.jj2000.j2k.codestream.ParameterList;

	public class InvCompTransf : BlkImgDataSrc
	{
		internal InvCompTransf(ImgDataConverter converter, DecoderSpecs decSpec, int[] depth, ParameterList pl)
		{
		}

		public static string[][] getParameterInfo()
		{
			return null;
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.decoder
{
	public class Decoder
	{
		public static string[][] getParameterInfo()
		{
			return null;
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.jj2000.j2k.image.output
{
	using BlkImgDataSrc = NetcdfSharp.ucar.jpeg.jj2000.j2k.image.BlkImgDataSrc;

	/// <summary>Stub for jj2000.j2k.writer.ImgWriter.</summary>
	public class ImgWriter
	{
		public const int DEF_STRIP_HEIGHT = 1 << 12;

		protected internal BlkImgDataSrc src;
		protected internal int w;
		protected internal int h;

		public virtual void close()
		{
		}

		public virtual void write(int ulx, int uly, int w, int h)
		{
		}

		public virtual void write()
		{
		}

		public virtual void writeAll()
		{
		}

		public virtual void flush()
		{
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.colorspace
{
	using RandomAccessIO = NetcdfSharp.ucar.jpeg.jj2000.j2k.io.RandomAccessIO;
	using HeaderDecoder = NetcdfSharp.ucar.jpeg.jj2000.j2k.codestream.reader.HeaderDecoder;
	using ParameterList = NetcdfSharp.ucar.jpeg.jj2000.j2k.codestream.ParameterList;

	public class ColorSpace
	{
		internal ColorSpace(RandomAccessIO @in, HeaderDecoder hd, ParameterList pl)
		{
		}

		public virtual bool isOutputSigned(int i)
		{
			return false;
		}
	}

	public class ColorSpaceException : System.Exception
	{
		public ColorSpaceException()
		{
		}

		public ColorSpaceException(string message) : base(message)
		{
		}
	}
}

namespace NetcdfSharp.ucar.jpeg.icc
{
	public class ICCProfiler
	{
		public static string[][] getParameterInfo()
		{
			return null;
		}
	}
}
