// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.iosp.hdf4
{
	using Document = NetcdfSharp.org.jdom2.Document;
	using Element = NetcdfSharp.org.jdom2.Element;
	using XMLOutputter = NetcdfSharp.org.jdom2.output.XMLOutputter;
	using Format = NetcdfSharp.org.jdom2.output.Format;
	using IO = NetcdfSharp.ucar.nc2.util.IO;

	/// <summary>
	/// Turn ODL into XML
	/// 
	/// @author caron
	/// @since Aug 7, 2007
	/// </summary>

	/*
	 * http://newsroom.gsfc.nasa.gov/sdptoolkit/hdfeosfaq.html
	 * 
	 * 3.2 What types of metadata are embedded in an HDF-EOS file and what are the added storage requirements?
	 * An HDF-EOS file must contain ECS "core" metadata which is essential for ECS search services. Core metadata are
	 * populated
	 * using the SDP Toolkit, rather than through HDF-EOS calls. "Archive" metadata (supplementary information included by
	 * the
	 * data provider) may also be present. If grid, point, or swath data types have been used, there also will be structural
	 * metadata describing how these data types have been translated into standard HDF data types. Metadata resides in
	 * human-readable form in the Object Descriptor Language (ODL). Structural metadata uses 32K of storage, regardless of
	 * the amount actually required. The sizes of the core and archive metadata vary depending on what has been entered by
	 * the user.
	 * 
	 * 3.3 What are the options for adding ECS metadata to standard HDF files?
	 * For data products that will be accessed by ECS but which remain in native HDF, there is a choice of
	 * 1) adding no ECS metadata in the HDF file,
	 * 2) inserting ECS metadata into the HDF file, or
	 * 3) "appending" ECS metadata to the HDF file. "Append" means updating the HDF location table so that the appended
	 * metadata
	 * becomes known to the HDF libraries/tools.
	 * 
	 * 3.4 Some DAACs currently provide descriptor files that give background information about the data. Will this
	 * information be included in an HDF-EOS file?
	 * Yes. The descriptor file will be retained. It can be viewed by EOSView if it stored either as a global attribute or a
	 * file annotation.
	 * 
	 */
	public class ODLparser
	{
	  private static NetcdfSharp.org.slf4j.Logger log = NetcdfSharp.org.slf4j.LoggerFactory.getLogger(typeof(ODLparser));
	  private static bool debug, showRaw, show;

	  private Document doc;

	  public virtual void showDoc(PrintWriter @out)
	  {
		XMLOutputter fmt = new XMLOutputter(Format.getPrettyFormat());
		try
		{
		  @out.write(fmt.outputString(doc));
		}
		catch (IOException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
	  }

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void parseFile(String filename) throws NetcdfSharp.java.io.IOException
	  public virtual void parseFile(string filename)
	  {
		string text = StringHelper.NewString(IO.readFileToByteArray(filename), Encoding.UTF8);
		parseFromString(text);
	  }

	  public virtual Element parseFromString(string text)
	  {
		if (showRaw)
		{
		  Console.WriteLine("Raw ODL=\n" + text);
		}

		Element rootElem = new Element("odl");
		doc = new Document(rootElem);

		Element current = rootElem;
		StringTokenizer lineFinder = new StringTokenizer(text, "\t\n\r\f");
		while (lineFinder.hasMoreTokens())
		{
		  string line = lineFinder.nextToken();
		  if (string.ReferenceEquals(line, null))
		  {
			continue;
		  }

		  if (line.StartsWith("GROUP", StringComparison.Ordinal))
		  {
			current = startGroup(current, line);

		  }
		  else if (line.StartsWith("OBJECT", StringComparison.Ordinal))
		  {
			current = startObject(current, line);

		  }
		  else if (line.StartsWith("END_OBJECT", StringComparison.Ordinal))
		  {
			endObject(current, line);
			current = current.getParentElement();
			if (current == null)
			{
			  throw new System.InvalidOperationException();
			}

		  }
		  else if (line.StartsWith("END_GROUP", StringComparison.Ordinal))
		  {
			endGroup(current, line);
			current = current.getParentElement();
			if (current == null)
			{
			  throw new System.InvalidOperationException();
			}

		  }
		  else
		  {
			addField(current, line);
		  }
		}

		if (show)
		{
		showDoc(new PrintWriter(JSystem.@out));
		}
		return rootElem;
	  }

	  internal virtual Element startGroup(Element parent, string line)
	  {
		StringTokenizer stoke = new StringTokenizer(line, "=");
		string toke = stoke.nextToken();
		Debug.Assert(toke.Equals("GROUP"));
		string name = stoke.nextToken();
		Element group = new Element(name);
		parent.addContent(group);
		return group;
	  }

	  internal virtual void endGroup(Element current, string line)
	  {
		StringTokenizer stoke = new StringTokenizer(line, "=");
		string toke = stoke.nextToken();
		Debug.Assert(toke.Equals("END_GROUP"));
		string name = stoke.nextToken();
		if (debug)
		{
		  Console.WriteLine(line + " -> " + current);
		}
		Debug.Assert(name.Equals(current.getName()));
	  }

	  internal virtual Element startObject(Element parent, string line)
	  {
		StringTokenizer stoke = new StringTokenizer(line, "=");
		string toke = stoke.nextToken();
		Debug.Assert(toke.Equals("OBJECT"));
		string name = stoke.nextToken();
		Element obj = new Element(name);
		parent.addContent(obj);
		return obj;
	  }

	  internal virtual void endObject(Element current, string line)
	  {
		StringTokenizer stoke = new StringTokenizer(line, "=");
		string toke = stoke.nextToken();
		Debug.Assert(toke.Equals("END_OBJECT"));
		string name = stoke.nextToken();
		if (debug)
		{
		  Console.WriteLine(line + " -> " + current);
		}
		Debug.Assert(name.Equals(current.getName()), name + " !+ " + current.getName());
	  }

	  internal virtual void addField(Element parent, string line)
	  {
		StringTokenizer stoke = new StringTokenizer(line, "=");
		string name = stoke.nextToken();
		if (stoke.hasMoreTokens())
		{
		  Element field = new Element(name);
		  parent.addContent(field);
		  string value = stoke.nextToken();

		  if (value.StartsWith("(", StringComparison.Ordinal))
		  {
			parseValueCollection(field, value);
			return;
		  }

		  value = stripQuotes(value);
		  field.addContent(value);
		}
	  }

	  internal virtual void parseValueCollection(Element field, string value)
	  {
		if (value.StartsWith("(", StringComparison.Ordinal))
		{
		  value = value.Substring(1);
		}
		if (value.EndsWith(")", StringComparison.Ordinal))
		{
		  value = value.Substring(0, value.Length - 1);
		}
		StringTokenizer stoke = new StringTokenizer(value, "\",");
		while (stoke.hasMoreTokens())
		{
		  field.addContent((new Element("value")).addContent(stripQuotes(stoke.nextToken())));
		}
	  }

	  internal virtual string stripQuotes(string name)
	  {
		if (name.StartsWith("\"", StringComparison.Ordinal))
		{
		  name = name.Substring(1);
		}
		if (name.EndsWith("\"", StringComparison.Ordinal))
		{
		  name = name.Substring(0, name.Length - 1);
		}
		return name;
	  }
	}

}
