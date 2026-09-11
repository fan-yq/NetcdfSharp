// JDOM (NetcdfSharp.org.jdom2.output) compatible shim - minimal subset used by NetcdfSharp.
using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using NetcdfSharp.org.jdom2;

namespace NetcdfSharp.org.jdom2.@output
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.org.jdom2.output.Format.
    /// </summary>
    public class Format
    {
        private bool _pretty = false;
        private bool _expandEmpty = true;
        private string _lineSeparator = Environment.NewLine;
        private string _indent;

        private Format()
        {
        }

        public static Format getPrettyFormat()
        {
            var f = new Format { _pretty = true, _indent = "  " };
            return f;
        }

        public static Format getRawFormat()
        {
            var f = new Format { _pretty = false };
            return f;
        }

        public static Format getCompactFormat()
        {
            var f = new Format { _pretty = true, _indent = "" };
            return f;
        }

        public Format setPrettyPrinting()
        {
            _pretty = true;
            _indent = _indent ?? "  ";
            return this;
        }

        public Format setRawMode()
        {
            _pretty = false;
            return this;
        }

        public Format setIndent(string indent)
        {
            _indent = indent;
            return this;
        }

        public Format setExpandEmptyElements(bool expandEmpty)
        {
            _expandEmpty = expandEmpty;
            return this;
        }

        public Format setLineSeparator(string separator)
        {
            _lineSeparator = separator;
            return this;
        }

        public Format setEncoding(string encoding)
        {
            return this;
        }

        internal SaveOptions SaveOptions
        {
            get { return _pretty ? SaveOptions.None : SaveOptions.DisableFormatting; }
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.org.jdom2.output.XMLOutputter.
    /// </summary>
    public class XMLOutputter
    {
        private readonly Format _format;

        public XMLOutputter()
        {
            _format = Format.getRawFormat();
        }

        public XMLOutputter(Format format)
        {
            _format = format ?? Format.getRawFormat();
        }

        public string outputString(Document doc)
        {
            if (doc == null)
            {
                return "";
            }
            Element root = doc.getRootElement();
            return root == null ? "" : root.XNode.ToString(_format.SaveOptions);
        }

        public string outputString(Element element)
        {
            if (element == null)
            {
                return "";
            }
            return element.XNode.ToString(_format.SaveOptions);
        }

        public string outputElementContentString(Element element)
        {
            if (element == null)
            {
                return "";
            }
            var sb = new StringBuilder();
            foreach (XNode node in element.XNode.Nodes())
            {
                sb.Append(node.ToString(_format.SaveOptions));
            }
            return sb.ToString();
        }

        public void output(Document doc, TextWriter writer)
        {
            writer.Write(outputString(doc));
        }

        public void output(Document doc, Stream stream)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(outputString(doc));
            stream.Write(bytes, 0, bytes.Length);
        }

        public void output(Element element, TextWriter writer)
        {
            writer.Write(outputString(element));
        }
    }
}
