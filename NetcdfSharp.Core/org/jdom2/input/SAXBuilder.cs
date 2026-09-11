// NetcdfSharp.org.jdom2.input.SAXBuilder compatible shim - minimal subset used by NetcdfSharp.
// Parses an XML stream into an NetcdfSharp.org.jdom2.Document using System.Xml.Linq.
using System.IO;
using System.Xml.Linq;
using NetcdfSharp.org.jdom2;

namespace NetcdfSharp.org.jdom2.input
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.org.jdom2.input.SAXBuilder.
    /// </summary>
    public class SAXBuilder
    {
        public bool ExpandEntities { get; set; } = true;

        public void setExpandEntities(bool expandEntities)
        {
            ExpandEntities = expandEntities;
        }

        public Document build(Stream stream)
        {
            XDocument xdoc = XDocument.Load(stream);
            Element root = xdoc.Root == null ? null : new Element(xdoc.Root);
            return new Document(root);
        }

        public Document build(NetcdfSharp.java.io.InputStream stream)
        {
            XDocument xdoc = XDocument.Load(new StreamReader(stream));
            Element root = xdoc.Root == null ? null : new Element(xdoc.Root);
            return new Document(root);
        }
    }
}
