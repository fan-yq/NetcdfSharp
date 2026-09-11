// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*
 * Copyright (c) 1998-2020 University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */

namespace NetcdfSharp.ucar.nc2.iosp.bufr.tables
{
	using Element = NetcdfSharp.org.jdom2.Element;
	using JDOMException = NetcdfSharp.org.jdom2.JDOMException;
	using SAXBuilder = NetcdfSharp.org.jdom2.input.SAXBuilder;
	using Util = NetcdfSharp.ucar.nc2.wmo.Util;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Read WMO BUFR XML formats
	/// 
	/// @author John
	/// @since 8/10/11
	/// </summary>
	public class WmoXmlReader
	{
	  private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(WmoXmlReader));

	  public sealed class Version
	  {
		public static readonly Version BUFR_14_1_0 = new Version("BUFR_14_1_0", InnerEnum.BUFR_14_1_0);
		public static readonly Version BUFR_14_2_0 = new Version("BUFR_14_2_0", InnerEnum.BUFR_14_2_0);
		public static readonly Version BUFR_15_1_1 = new Version("BUFR_15_1_1", InnerEnum.BUFR_15_1_1);
		public static readonly Version BUFR_16_0_0 = new Version("BUFR_16_0_0", InnerEnum.BUFR_16_0_0);
		public static readonly Version BUFR_WMO = new Version("BUFR_WMO", InnerEnum.BUFR_WMO);

		private static readonly List<Version> valueList = new List<Version>();

		static Version()
		{
			valueList.Add(BUFR_14_1_0);
			valueList.Add(BUFR_14_2_0);
			valueList.Add(BUFR_15_1_1);
			valueList.Add(BUFR_16_0_0);
			valueList.Add(BUFR_WMO);
		}

		public enum InnerEnum
		{
			BUFR_14_1_0,
			BUFR_14_2_0,
			BUFR_15_1_1,
			BUFR_16_0_0,
			BUFR_WMO
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private Version(string name, InnerEnum innerEnum)
		{
			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		internal string[] ElemNamesB
		{
			get
			{
			  if (this == BUFR_14_1_0)
			  {
				return new string[] {"BC_TableB_BUFR14_1_0_CREX_6_1_0", "ElementName_E"};

			  }
			  else if (this == BUFR_14_2_0)
			  {
				return new string[] {"Exporting_BCTableB_E", "ElementName"};

			  }
			  else if (this == BUFR_15_1_1)
			  {
				return new string[] {"Exp_JointTableB_E", "ElementName_E"};

			  }
			  else if (this == BUFR_16_0_0)
			  {
				return new string[] {"Exp_BUFRCREXTableB_E", "ElementName_E"};

			  }
			  else if (this == BUFR_WMO)
			  { // from now on this is the element name
				return new string[] {null, "ElementName_en"};

			  }
			  return null;
			}
		}

		internal string[] ElemNamesD
		{
			get
			{
			  if (this == BUFR_14_1_0)
			  {
				return new string[] {"B_TableD_BUFR14_1_0_CREX_6_1_0", "ElementName1_E"};

			  }
			  else if (this == BUFR_14_2_0)
			  {
				return new string[] {"Exporting_BUFRTableD_E", "ElementName1"};

			  }
			  else if (this == BUFR_15_1_1)
			  {
				return new string[] {"Exp_BUFRTableD_E", "ElementName_E", "ExistingElementName_E"};

			  }
			  else if (this == BUFR_16_0_0)
			  {
				return new string[] {"Exp_BUFRTableD_E", "ElementName_E", "ExistingElementName_E"};

			  }
			  else if (this == BUFR_WMO)
			  {
				return new string[] {null, "ElementName_en"};

			  }
			  return null;
			}
		}

		  public static Version[] values()
		  {
			  return valueList.ToArray();
		  }

		  public int ordinal()
		  {
			  return ordinalValue;
		  }

		  public override string ToString()
		  {
			  return nameValue;
		  }

		  public static Version valueOf(string name)
		  {
			  foreach (Version enumInstance in Version.valueList)
			  {
				  if (enumInstance.nameValue == name)
				  {
					  return enumInstance;
				  }
			  }
			  throw new ArgumentException(name);
		  }
	  }

	  /*
	   * 14.1
	   * <BC_TableB_BUFR14_1_0_CREX_6_1_0>
	   * <SNo>1</SNo>
	   * <Class>00</Class>
	   * <FXY>000001</FXY>
	   * <ElementName_E>Table A: entry</ElementName_E>
	   * <ElementName_F>Table A : entr?e</ElementName_F>
	   * <ElementName_R>??????? ?: ???????</ElementName_R>
	   * <ElementName_S>Tabla A: elemento</ElementName_S>
	   * <BUFR_Unit>CCITT IA5</BUFR_Unit>
	   * <BUFR_Scale>0</BUFR_Scale>
	   * <BUFR_ReferenceValue>0</BUFR_ReferenceValue>
	   * <BUFR_DataWidth_Bits>24</BUFR_DataWidth_Bits>
	   * <CREX_Unit>Character</CREX_Unit>
	   * <CREX_Scale>0</CREX_Scale>
	   * <CREX_DataWidth>3</CREX_DataWidth>
	   * <Status>Operational</Status>
	   * <NotesToTable_E>Notes: (see)#BUFR14_1_0_CREX6_1_0_Notes.doc#BC_Cl000</NotesToTable_E>
	   * </BC_TableB_BUFR14_1_0_CREX_6_1_0>
	   * 
	   * 14.2
	   * <Exporting_BCTableB_E>
	   * <No>2</No>
	   * <ClassNo>00</ClassNo>
	   * <ClassName>BUFR/CREX table entries</ClassName>
	   * <FXY>000002</FXY>
	   * <ElementName>Table A: data category description, line 1 </ElementName>
	   * <BUFR_Unit>CCITT IA5 </BUFR_Unit>
	   * <BUFR_Scale>0</BUFR_Scale>
	   * <BUFR_ReferenceValue>0</BUFR_ReferenceValue>
	   * <BUFR_DataWidth_Bits>256</BUFR_DataWidth_Bits>
	   * <CREX_Unit>Character</CREX_Unit>
	   * <CREX_Scale>0</CREX_Scale>
	   * <CREX_DataWidth>32</CREX_DataWidth>
	   * <Status>Operational</Status>
	   * </Exporting_BCTableB_E>
	   * 
	   * 15.1
	   * <Exp_JointTableB_E>
	   * <No>1</No>
	   * <ClassNo>00</ClassNo>
	   * <ClassName_E>BUFR/CREX table entries</ClassName_E>
	   * <FXY>000001</FXY>
	   * <ElementName_E>Table A: entry</ElementName_E>
	   * <BUFR_Unit>CCITT IA5</BUFR_Unit>
	   * <BUFR_Scale>0</BUFR_Scale>
	   * <BUFR_ReferenceValue>0</BUFR_ReferenceValue>
	   * <BUFR_DataWidth_Bits>24</BUFR_DataWidth_Bits>
	   * <CREX_Unit>Character</CREX_Unit>
	   * <CREX_Scale>0</CREX_Scale>
	   * <CREX_DataWidth_Char>3</CREX_DataWidth_Char>
	   * <Status>Operational</Status>
	   * </Exp_JointTableB_E>
	   * 
	   * 16.0
	   * <Exp_BUFRCREXTableB_E>
	   * <No>681</No>
	   * <ClassNo>13</ClassNo>
	   * <ClassName_E>Hydrographic and hydrological elements</ClassName_E>
	   * <FXY>013060</FXY>
	   * <ElementName_E>Total accumulated precipitation</ElementName_E>
	   * <BUFR_Unit>kg m-2</BUFR_Unit>
	   * <BUFR_Scale>1</BUFR_Scale>
	   * <BUFR_ReferenceValue>-1</BUFR_ReferenceValue>
	   * <BUFR_DataWidth_Bits>17</BUFR_DataWidth_Bits>
	   * <CREX_Unit>kg m-2</CREX_Unit>
	   * <CREX_Scale>1</CREX_Scale>
	   * <CREX_DataWidth_Char>5</CREX_DataWidth_Char>
	   * <Status>Operational</Status>
	   * </Exp_BUFRCREXTableB_E>
	   * 
	   * <BUFRCREX_17_0_0_TableB_en>
	   * <No>8</No>
	   * <ClassNo>00</ClassNo>
	   * <ClassName_en>BUFR/CREX table entries</ClassName_en>
	   * <FXY>000008</FXY>
	   * <ElementName_en>BUFR Local table version number</ElementName_en>
	   * <Note_en>(see Note 4)</Note_en>
	   * <BUFR_Unit>CCITT IA5</BUFR_Unit>
	   * <BUFR_Scale>0</BUFR_Scale>
	   * <BUFR_ReferenceValue>0</BUFR_ReferenceValue>
	   * <BUFR_DataWidth_Bits>16</BUFR_DataWidth_Bits>
	   * <CREX_Unit>Character</CREX_Unit>
	   * <CREX_Scale>0</CREX_Scale>
	   * <CREX_DataWidth_Char>2</CREX_DataWidth_Char>
	   * <Status>Operational</Status>
	   * </BUFRCREX_17_0_0_TableB_en>
	   * 
	   * <BUFRCREX_22_0_1_TableB_en>
	   * <No>1018</No>
	   * <ClassNo>21</ClassNo>
	   * <ClassName_en>BUFR/CREX Radar data</ClassName_en>
	   * <FXY>021073</FXY>
	   * <ElementName_en>Satellite altimeter instrument mode</ElementName_en>
	   * <BUFR_Unit>Flag table</BUFR_Unit>
	   * <BUFR_Scale>0</BUFR_Scale>
	   * <BUFR_ReferenceValue>0</BUFR_ReferenceValue>
	   * <BUFR_DataWidth_Bits>9</BUFR_DataWidth_Bits>
	   * <CREX_Unit>Flag table</CREX_Unit>
	   * <CREX_Scale>0</CREX_Scale>
	   * <CREX_DataWidth_Char>3</CREX_DataWidth_Char>
	   * <Status>Operational</Status>
	   * </BUFRCREX_22_0_1_TableB_en>
	   */

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static void readWmoXmlTableB(NetcdfSharp.java.io.InputStream ios, TableB b) throws NetcdfSharp.java.io.IOException
	  internal static void readWmoXmlTableB(Stream ios, TableB b)
	  {
		NetcdfSharp.org.jdom2.Document doc;
		try
		{
		  SAXBuilder builder = new SAXBuilder();
		  builder.setExpandEntities(false);
		  doc = builder.build(ios);
		}
		catch (JDOMException e)
		{
		  throw new IOException(e.Message);
		}

		Element root = doc.getRootElement();

		// what elements do we need to parse tableB?
		string[] elems = elementsUsedFromTableB(root);

		IList<Element> unrecognizedSequenceTermElements = new List<Element>();
		IList<Element> featList = root.getChildren();
		foreach (Element elem in featList)
		{
		  Element ce = null;
		  for (int nameTest = 1; nameTest < elems.Length; nameTest++)
		  {
			ce = elem.getChild(elems[nameTest]);
			if (ce != null)
			{
			  break;
			}
		  }
		  if (ce == null)
		  {
			unrecognizedSequenceTermElements.Add(elem);
			continue;
		  }

		  string name = Util.cleanName(ce.getTextNormalize());
		  string units = cleanUnit(elem.getChildTextNormalize("BUFR_Unit"));
		  int x = 0, y = 0, scale = 0, reference = 0, width = 0;

		  string fxy = null;
		  string s = null;
		  try
		  {
			fxy = elem.getChildTextNormalize("FXY");
			int xy = int.Parse(cleanNumber(fxy));
			x = xy / 1000;
			y = xy % 1000;

		  }
		  catch (System.FormatException)
		  {
			log.warn(" key {} name '{}' fails parsing", fxy, name);
		  }

		  try
		  {
			s = elem.getChildTextNormalize("BUFR_Scale");
			scale = int.Parse(cleanNumber(s));
		  }
		  catch (System.FormatException)
		  {
			log.warn(" key {} name '{}' has bad scale='{}'", fxy, name, s);
		  }

		  try
		  {
			s = elem.getChildTextNormalize("BUFR_ReferenceValue");
			reference = int.Parse(cleanNumber(s));
		  }
		  catch (System.FormatException)
		  {
			log.warn(" key {} name '{}' has bad reference='{}'", fxy, name, s);
		  }

		  try
		  {
			s = elem.getChildTextNormalize("BUFR_DataWidth_Bits");
			width = int.Parse(cleanNumber(s));
		  }
		  catch (System.FormatException)
		  {
			log.warn(" key {} name '{}' has bad width='{}'", fxy, name, s);
		  }

		  b.addDescriptor((short) x, (short) y, scale, reference, width, name, units, null);
		}

		if (log.isDebugEnabled())
		{
		  logUnrecognizedElements(unrecognizedSequenceTermElements, "B", b.Location);
		}

		ios.Close();
	  }

	  internal static string cleanNumber(string s)
	  {
		return StringUtil2.remove(s, ' ');
	  }

	  public static string cleanUnit(string unit)
	  {
		string result = StringUtil2.remove(unit, 176);
		return StringUtil2.replace(result, (char) 65533, "2"); // seems to be a superscript 2 in some language
	  }

	  internal static string[] elementsUsedFromTableD(Element root)
	  {
		return elementsUsedFromTable(root, "D");
	  }

	  internal static string[] elementsUsedFromTableB(Element root)
	  {
		return elementsUsedFromTable(root, "B");
	  }

	  internal static string[] elementsUsedFromTable(Element root, string tableType)
	  {
		string[] elems = null;
		// does the table have its own enum value? If so, use it.
		foreach (Version v in Version.values())
		{
		  bool match = root.getAttributes().Any(attr => attr.getValue().Contains(v.ToString()));
		  if (match)
		  {
			elems = tableType.Equals("B") ? v.ElemNamesB : v.ElemNamesD;
			break;
		  }
		}

		// exact table match not found. Try seeing if the table uses
		// the sequence element from a version defined in the Version enum.
		// Note: will stop on the first version that works, as defined by
		// the order of the Version enum. might not be correct.
		if (elems == null)
		{
		  foreach (Version v in Version.values())
		  {
			elems = tableType.Equals("B") ? v.ElemNamesB : v.ElemNamesD;
			IList<Element> featList = null;
			if ((elems != null) && (elems.Length > 0))
			{
			  featList = root.getChildren(elems[0]);
			}
			if (featList != null && featList.Count > 0)
			{
			  break;
			}
		  }
		}

		return elems;
	  }

	  internal static void logUnrecognizedElements(IList<Element> unrecognizedSequenceTermElements, string tableType, string location)
	  {
		// not every sequence entry in the WMO xml table D files is processed. This has caused trouble before.
		// this is a pretty specific, low level debug message to hopefully give a clue to us in the future
		// that if we are having trouble decoding BUFR messages, maybe we're not fully parsing the WMO xml TableD
		// entries, and so the sequence being used might not be the full sequence necessary to decode.
		if (log.isDebugEnabled())
		{
		  if (unrecognizedSequenceTermElements.Count > 0)
		  {
			StringBuilder msgBuilder = new StringBuilder();
			msgBuilder.Append(string.Format("{0:D} Unprocessed sequences in WMO table {1} {2}", unrecognizedSequenceTermElements.Count, tableType, location));
			if (tableType.Equals("D"))
			{
			  string tableDChecker = "bufr/src/test/java/ucar/nc2/iosp/bufr/tables/WmoTableDVariations.java";
			  msgBuilder.Append(string.Format("This might be ok, but to know for sure, consider running {0}", tableDChecker));
			}
			log.debug(msgBuilder.ToString());
		  }
		}
	  }

	  /*
	   * <B_TableD_BUFR14_1_0_CREX_6_1_0>
	   * <SNo>2647</SNo>
	   * <Category>10</Category>
	   * <FXY1>310013</FXY1>
	   * <ElementName1_E>(AVHRR (GAC) report)</ElementName1_E>
	   * <FXY2>004005</FXY2>
	   * <ElementName2_E>Minute</ElementName2_E>
	   * <Remarks_E>Minute</Remarks_E>
	   * <Status>Operational</Status>
	   * </B_TableD_BUFR14_1_0_CREX_6_1_0>
	   * 
	   * 14.2.0
	   * <Exporting_BUFRTableD_E>
	   * <No>2901</No>
	   * <Category>10</Category>
	   * <CategoryOfSequences>Vertical sounding sequences (satellite data)</CategoryOfSequences>
	   * <FXY1>310025</FXY1>
	   * <ElementName1>(SSMIS Temperature data record)</ElementName1>
	   * <FXY2>004006</FXY2>
	   * <Status>Operational</Status>
	   * </Exporting_BUFRTableD_E>
	   * 
	   * 15.1.1
	   * <Exp_BUFRTableD_E>
	   * <No>102</No>
	   * <Category>01</Category>
	   * <CategoryOfSequences_E>Location and identification sequences</CategoryOfSequences_E>
	   * <FXY1>301034</FXY1>
	   * <Title_E>(Buoy/platform - fixed)</Title_E>
	   * <FXY2>001005</FXY2>
	   * <ElementName_E>Buoy/platform identifier</ElementName_E>
	   * <ExistingElementName_E>Buoy/platform identifier</ExistingElementName_E>
	   * <Status>Operational</Status>
	   * </Exp_BUFRTableD_E>
	   * 
	   * 16.0.0
	   * <Exp_BUFRTableD_E>
	   * <No>402</No>
	   * <Category>02</Category>
	   * <CategoryOfSequences_E>Meteorological sequences common to surface data</CategoryOfSequences_E>
	   * <FXY1>302001</FXY1>
	   * <FXY2>010051</FXY2>
	   * <ElementName_E>Pressure reduced to mean sea level</ElementName_E>
	   * <ExistingElementName_E>Pressure reduced to mean sea level</ExistingElementName_E>
	   * <Status>Operational</Status>
	   * </Exp_BUFRTableD_E>
	   * 
	   * <BUFR_19_1_1_TableD_en>
	   * <No>4</No>
	   * <Category>00</Category>
	   * <CategoryOfSequences_en>BUFR table entries sequences</CategoryOfSequences_en>
	   * <FXY1>300003</FXY1>
	   * <Title_en>(F, X, Y of descriptor to be added or defined)</Title_en>
	   * <FXY2>000011</FXY2>
	   * <ElementName_en>X descriptor to be added or defined</ElementName_en>
	   * <Status>Operational</Status>
	   * </BUFR_19_1_1_TableD_en>
	   * 
	   * <BUFR_22_0_1_TableD_en>
	   * <No>5874</No>
	   * <Category>15</Category>
	   * <CategoryOfSequences_en>Oceanographic report sequences</CategoryOfSequences_en>
	   * <FXY1>315004</FXY1>
	   * <Title_en>(XBT temperature profile data sequence)</Title_en>
	   * <FXY2>025061</FXY2>
	   * <ElementName_en>Software identification and version number</ElementName_en>
	   * <Status>Operational</Status>
	   * </BUFR_22_0_1_TableD_en>
	   * 
	   */
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: static void readWmoXmlTableD(NetcdfSharp.java.io.InputStream ios, TableD tableD) throws NetcdfSharp.java.io.IOException
	  internal static void readWmoXmlTableD(Stream ios, TableD tableD)
	  {
		NetcdfSharp.org.jdom2.Document doc;
		try
		{
		  SAXBuilder builder = new SAXBuilder();
		  builder.setExpandEntities(false);
		  doc = builder.build(ios);
		}
		catch (JDOMException e)
		{
		  throw new IOException(e.Message);
		}

		int currSeqno = -1;
		TableD.Descriptor currDesc = null;

		Element root = doc.getRootElement();

		// what elements do we need to parse tableD?
		string[] elems = elementsUsedFromTableD(root);
		IList<Element> unrecognizedSequenceTermElements = new List<Element>();
		IList<Element> featList = root.getChildren();
		foreach (Element elem in featList)
		{
		  // see if element in table is recognized
		  Element ce = null;
		  for (int nameTest = 1; nameTest < elems.Length; nameTest++)
		  {
			ce = elem.getChild(elems[nameTest]);
			if (ce != null)
			{
			  break;
			}
		  }
		  if (ce == null)
		  {
			unrecognizedSequenceTermElements.Add(elem);
			continue;
		  }

		  string seqs = elem.getChildTextNormalize("FXY1");
		  int seq = int.Parse(seqs);

		  if (currSeqno != seq)
		  {
			int yseq = seq % 1000;
			int wseq = seq / 1000;
			int xseq = wseq % 100;
			string seqName = Util.cleanName(ce.getTextNormalize());
			currDesc = tableD.addDescriptor((short) xseq, (short) yseq, seqName, new List<short>());
			currSeqno = seq;
		  }

		  string fnos = elem.getChildTextNormalize("FXY2");
		  int fno = int.Parse(fnos);
		  int y = fno % 1000;
		  int w = fno / 1000;
		  int x = w % 100;
		  int f = w / 100;
		  int fxy = (f << 14) + (x << 8) + y;
		  currDesc.addFeature((short) fxy);
		}

		if (log.isDebugEnabled())
		{
		  logUnrecognizedElements(unrecognizedSequenceTermElements, "D", tableD.Location);
		}

		ios.Close();
	  }

	}


}
