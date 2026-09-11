// JDOM (org.jdom2) compatible shim - minimal subset used by NetcdfSharp.
// Element/Document wrap System.Xml.Linq.XElement/XDocument.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace NetcdfSharp.org.jdom2
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.org.jdom2.JDOMException.
    /// </summary>
    public class JDOMException : Exception
    {
        public JDOMException(string message)
            : base(message)
        {
        }

        public JDOMException(string message, Exception cause)
            : base(message, cause)
        {
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.org.jdom2.Namespace.
    /// </summary>
    public class Namespace
    {
        public static readonly Namespace NO_NAMESPACE = new Namespace("", "");

        public string Prefix { get; }
        public string URI { get; }

        private Namespace(string prefix, string uri)
        {
            Prefix = prefix;
            URI = uri;
        }

        public static Namespace getNamespace(string uri)
        {
            return new Namespace("", uri);
        }

        public static Namespace getNamespace(string prefix, string uri)
        {
            return new Namespace(prefix, uri);
        }

        public override bool Equals(object obj)
        {
            return obj is Namespace other && other.URI == URI && other.Prefix == Prefix;
        }

        public override int GetHashCode()
        {
            return (URI ?? "").GetHashCode();
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Prefix) ? URI : "xmlns:" + Prefix + "=" + URI;
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.org.jdom2.Attribute.
    /// </summary>
    public class Attribute
    {
        private readonly XElement _parent;
        private readonly XAttribute _attribute;
        private string _detachedName;
        private string _detachedValue;

        internal Attribute(XElement parent, XAttribute attribute)
        {
            _parent = parent;
            _attribute = attribute;
        }

        public Attribute(string name, string value)
        {
            _detachedName = name;
            _detachedValue = value;
        }

        public string getName()
        {
            if (_attribute != null)
            {
                return _attribute.Name.LocalName;
            }
            return _detachedName;
        }

        public string getValue()
        {
            if (_attribute != null)
            {
                return _attribute.Value;
            }
            return _detachedValue;
        }

        public void setValue(string value)
        {
            if (_attribute != null)
            {
                _attribute.Value = value;
            }
            else
            {
                _detachedValue = value;
            }
        }

        public string getNamespacePrefix()
        {
            return _attribute != null ? _attribute.Name.NamespaceName : "";
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.org.jdom2.Element.
    /// </summary>
    public class Element
    {
        private readonly XElement _element;

        public Element(string name)
        {
            _element = new XElement(GetXName(name, null));
        }

        public Element(string name, Namespace ns)
        {
            _element = new XElement(GetXName(name, ns));
        }

        public Element(string name, string uri)
        {
            _element = new XElement(GetXName(name, Namespace.getNamespace("", uri)));
        }

        internal Element(XElement element)
        {
            _element = element;
        }        public string getName()
        {
            return _element.Name.LocalName;
        }

        public string getNamespaceURI()
        {
            return _element.Name.NamespaceName;
        }

        public Namespace getNamespace()
        {
            XNamespace xns = _element.Name.Namespace;
            return Namespace.getNamespace(_element.GetPrefixOfNamespace(xns) ?? "", xns.NamespaceName);
        }

        public Element getParentElement()
        {
            XElement parent = _element.Parent;
            return parent == null ? null : new Element(parent);
        }

        /// <summary>
        /// Detach this element from its parent (Java jdom Element.detach()).
        /// </summary>
        public void detach()
        {
            _element.Remove();
        }

        public Element addContent(Element content)
        {
            if (content != null)
            {
                _element.Add(content._element);
            }
            return this;
        }

        public Element addContent(string content)
        {
            if (content != null)
            {
                _element.Add(new XText(content));
            }
            return this;
        }

        public Element setText(string text)
        {
            _element.Value = text ?? "";
            return this;
        }

        public Element setContent(string content)
        {
            return setText(content);
        }

        public Element setAttribute(string name, string value)
        {
            XAttribute attr = _element.Attribute(name);
            if (attr == null)
            {
                _element.SetAttributeValue(name, value);
            }
            else
            {
                attr.Value = value ?? "";
            }
            return this;
        }

        public Element setAttribute(Attribute attribute)
        {
            if (attribute != null)
            {
                setAttribute(attribute.getName(), attribute.getValue());
            }
            return this;
        }

        public Attribute getAttribute(string name)
        {
            XAttribute attr = _element.Attribute(name);
            return attr == null ? null : new Attribute(_element, attr);
        }

        public Attribute getAttribute(string name, Namespace ns)
        {
            XAttribute attr = FindAttribute(name, ns);
            return attr == null ? null : new Attribute(_element, attr);
        }

        public string getAttributeValue(string name)
        {
            XAttribute attr = _element.Attribute(name);
            return attr == null ? null : attr.Value;
        }

        public string getAttributeValue(string name, Namespace ns)
        {
            XAttribute attr = FindAttribute(name, ns);
            return attr == null ? null : attr.Value;
        }

        public IList<Attribute> getAttributes()
        {
            return _element.Attributes().Select(a => new Attribute(_element, a)).ToList();
        }

        public IList<Attribute> getAttributes(string name)
        {
            return _element.Attributes(name).Select(a => new Attribute(_element, a)).ToList();
        }

        public Element getChild(string name)
        {
            XElement child = _element.Elements().FirstOrDefault(e => e.Name.LocalName == name);
            return child == null ? null : new Element(child);
        }

        public Element getChild(string name, Namespace ns)
        {
            XElement child = FindChild(name, ns);
            return child == null ? null : new Element(child);
        }

        public IList<Element> getChildren()
        {
            return _element.Elements().Select(e => new Element(e)).ToList();
        }

        public IList<Element> getChildren(string name)
        {
            return _element.Elements().Where(e => e.Name.LocalName == name).Select(e => new Element(e)).ToList();
        }

        public IList<Element> getChildren(string name, Namespace ns)
        {
            return _element.Elements().Where(e => e.Name.LocalName == name && NamespaceMatches(e.Name.Namespace, ns)).Select(e => new Element(e)).ToList();
        }

        public string getText()
        {
            return _element.Value;
        }

        public string getTextNormalize()
        {
            return Normalize(_element.Value);
        }

        public string getChildText(string name)
        {
            Element child = getChild(name);
            return child == null ? null : child.getText();
        }

        public string getChildText(string name, Namespace ns)
        {
            Element child = getChild(name, ns);
            return child == null ? null : child.getText();
        }

        public string getChildTextNormalize(string name)
        {
            Element child = getChild(name);
            return child == null ? null : child.getTextNormalize();
        }

        public string getChildTextNormalize(string name, Namespace ns)
        {
            Element child = getChild(name, ns);
            return child == null ? null : child.getTextNormalize();
        }

        public bool hasChildren()
        {
            return _element.HasElements;
        }

        public int getContentSize()
        {
            return _element.Nodes().Count();
        }

        public override string ToString()
        {
            return _element.ToString();
        }

        /// <summary>
        /// JDOM text normalization: trim and collapse runs of whitespace into a single space.
        /// </summary>
        private static string Normalize(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            return Regex.Replace(text.Trim(), "\\s+", " ");
        }

        private static XName GetXName(string localName, Namespace ns)
        {
            if (ns == null || string.IsNullOrEmpty(ns.URI))
            {
                return XName.Get(localName);
            }
            return XNamespace.Get(ns.URI) + localName;
        }

        private XAttribute FindAttribute(string name, Namespace ns)
        {
            if (ns == null || string.IsNullOrEmpty(ns.URI))
            {
                return _element.Attribute(name);
            }
            return _element.Attributes().FirstOrDefault(a => a.Name.LocalName == name && a.Name.NamespaceName == ns.URI);
        }

        private XElement FindChild(string name, Namespace ns)
        {
            return _element.Elements().FirstOrDefault(e => e.Name.LocalName == name && NamespaceMatches(e.Name.Namespace, ns));
        }

        private static bool NamespaceMatches(XNamespace xns, Namespace ns)
        {
            if (ns == null)
            {
                return true;
            }
            if (string.IsNullOrEmpty(ns.URI))
            {
                return xns == XNamespace.None;
            }
            return xns.NamespaceName == ns.URI;
        }

        internal XElement XNode
        {
            get { return _element; }
        }
    }

    /// <summary>
    /// Simplified replacement for NetcdfSharp.org.jdom2.Document.
    /// </summary>
    public class Document
    {
        private readonly XDocument _document;
        private readonly Element _root;

        public Document(Element rootElement)
        {
            _root = rootElement;
            _document = new XDocument(rootElement != null ? rootElement.XNode : null);
        }

        public Document()
        {
            _root = null;
            _document = new XDocument();
        }

        public Element getRootElement()
        {
            if (_root != null)
            {
                return _root;
            }
            return _document.Root == null ? null : new Element(_document.Root);
        }

        public void setRootElement(Element rootElement)
        {
            _document.RemoveNodes();
            if (rootElement != null)
            {
                _document.Add(rootElement.XNode);
            }
        }

        public bool hasRootElement()
        {
            return getRootElement() != null;
        }

        public string getBaseURI()
        {
            return _document.BaseUri;
        }
    }
}
