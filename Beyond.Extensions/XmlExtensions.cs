// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

using Beyond.Extensions.ObjectExtended;

namespace Beyond.Extensions.XmlExtended;

public static class XmlExtensions
{
    public static XmlCDataSection? CreateCDataSection(this XmlNode parentNode)
    {
        return parentNode.CreateCDataSection(string.Empty);
    }

    public static XmlCDataSection? CreateCDataSection(this XmlNode parentNode, string data)
    {
        var document = parentNode as XmlDocument ?? parentNode.OwnerDocument;
        var node = document?.CreateCDataSection(data);
        if (node == null) return null;
        parentNode.AppendChild(node);
        return node;
    }

    public static XmlNode? CreateChildNode(this XmlNode parentNode, string name)
    {
        var document = parentNode as XmlDocument ?? parentNode.OwnerDocument;
        XmlNode? node = document?.CreateElement(name);
        if (node == null) return null;

        parentNode.AppendChild(node);
        return node;
    }

    public static XmlNode? CreateChildNode(this XmlNode parentNode, string name, string namespaceUri)
    {
        var document = parentNode as XmlDocument ?? parentNode.OwnerDocument;
        XmlNode? node = document?.CreateElement(name, namespaceUri);
        if (node == null) return null;
        parentNode.AppendChild(node);
        return node;
    }

    public static T? DeserializeXml<T>(this string xml) where T : class, new()
    {
        if (xml == null) throw new ArgumentNullException(nameof(xml));

        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(xml);
        try
        {
            return (T)serializer.Deserialize(reader)!;
        }
        catch
        {
            return null;
        }
    }

    public static string FormatXmlText(this string xmlText)
    {
        var stringBuilder = new StringBuilder();
        var element = XElement.Parse(xmlText);
        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true,
            NewLineOnAttributes = true
        };
        using var xmlWriter = XmlWriter.Create(stringBuilder, settings);
        element.Save(xmlWriter);

        return stringBuilder.ToString();
    }

    public static string? GetAttribute(this XmlNode node, string attributeName)
    {
        return GetAttribute(node, attributeName, null);
    }

    public static string? GetAttribute(this XmlNode node, string attributeName, string? defaultValue)
    {
        var attribute = node.Attributes?[attributeName];
        return attribute != null ? attribute.InnerText : defaultValue;
    }

    public static T? GetAttribute<T>(this XmlNode node, string attributeName)
    {
        return GetAttribute(node, attributeName, default(T));
    }

    public static T? GetAttribute<T>(this XmlNode node, string attributeName, T? defaultValue)
    {
        var value = GetAttribute(node, attributeName);
        if (!string.IsNullOrEmpty(value)) return defaultValue;

        if (typeof(T) == typeof(Type) && value != null)
            return (T)((object)Type.GetType(value, true)!);

        return value.ConvertTo(defaultValue);
    }

    public static string? GetCDataSection(this XmlNode parentNode)
    {
        foreach (var node in parentNode.ChildNodes)
            if (node is XmlCDataSection section)
                return section.Value;
        return null;
    }

    public static XElement RemoveAllNamespaces(this XElement @this)
    {
        return new XElement(@this.Name.LocalName,
            @this.Nodes().Select(n => n is XElement element ? RemoveAllNamespaces(element) : n),
            @this.HasAttributes ? from a in @this.Attributes() select a : null);
    }

    public static IEnumerable<XmlNode>? SelectNodesEnumerable(this XmlDocument @this, string xpath)
    {
        if (@this == null) throw new ArgumentNullException(nameof(@this), $"{nameof(@this)} is null");
        if (xpath == null) throw new ArgumentNullException(nameof(xpath), $"{nameof(xpath)} is null");
        return @this.SelectNodes(xpath)?.Cast<XmlNode>();
    }

    public static string SerializeXml<T>(this T obj) where T : class, new()
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var serializer = new XmlSerializer(typeof(T));
        using var writer = new StringWriter();
        serializer.Serialize(writer, obj);
        return writer.ToString();
    }

    public static void SetAttribute(this XmlNode? node, string name, object? value)
    {
        SetAttribute(node, name, value?.ToString());
    }

    public static void SetAttribute(this XmlNode? node, string name, string? value)
    {
        if (node == null) return;
        var attribute = node.Attributes?[name, node.NamespaceURI];
        if (attribute == null)
        {
            attribute = node.OwnerDocument?.CreateAttribute(name, node.OwnerDocument.NamespaceURI);
            if (attribute != null) node.Attributes?.Append(attribute);
        }

        if (value == null) return;
        if (attribute != null)
            attribute.InnerText = value;
    }

    public static void Sort(this XElement source, bool bSortAttributes = true)
    {
        //Make sure there is a valid source
        if (source == null) throw new ArgumentNullException(nameof(source));

        //Sort attributes if needed
        if (bSortAttributes)
        {
            var sortedAttributes = source.Attributes().OrderBy(a => a.ToString()).ToList();
            sortedAttributes.ForEach(a => a.Remove());
            sortedAttributes.ForEach(source.Add);
        }

        //Sort the children if any exist
        var sortedChildren = source.Elements().OrderBy(e => e.Name.ToString()).ToList();
        if (!source.HasElements) return;
        source.RemoveNodes();
        sortedChildren.ForEach(c => c.Sort(bSortAttributes));
        sortedChildren.ForEach(source.Add);
    }

    public static string SortXmlText(this string xmlText, bool sortAttributes = true, Action<XElement>? postSort = null,
        params string[]? customAttributes)
    {
        var xmlTree = XElement.Parse(xmlText);
        var sortedXmlTree = SortXmlElement(xmlTree, sortAttributes, postSort, customAttributes);
        return sortedXmlTree.ToString();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static Stream ToMemoryStream(this XmlDocument xmlDocument)
    {
        var xmlStream = new MemoryStream();
        xmlDocument.Save(xmlStream);
        xmlStream.Flush(); //Adjust this if you want read your data
        xmlStream.Position = 0;
        return xmlStream;
    }

    public static Stream ToMemoryStream(this XElement xElement)
    {
        return xElement.ToXmlDocument().ToMemoryStream();
    }

    public static Stream ToMemoryStream(this XDocument xDocument)
    {
        return xDocument.ToXmlDocument().ToMemoryStream();
    }

    public static XDocument ToXDocument(this XmlDocument xmlDocument)
    {
        using var nodeReader = new XmlNodeReader(xmlDocument);
        nodeReader.MoveToContent();
        return XDocument.Load(nodeReader);
    }

    public static XmlDocument ToXmlDocument(this XDocument xDocument)
    {
        var xmlDocument = new XmlDocument();
        using var xmlReader = xDocument.CreateReader();
        xmlDocument.Load(xmlReader);

        return xmlDocument;
    }

    public static XmlDocument ToXmlDocument(this XElement xElement)
    {
        var sb = new StringBuilder();
        var xws = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = false };
        using (var xw = XmlWriter.Create(sb, xws))
        {
            xElement.WriteTo(xw);
        }

        var doc = new XmlDocument();
        doc.LoadXml(sb.ToString());
        return doc;
    }

    // ReSharper disable once CognitiveComplexity
    private static XElement SortXmlElement(this XElement xe, bool sortAttributes = true,
        Action<XElement>? postSort = null, params string[]? customAttributes)
    {
        var nodesToBePreserved = xe.Nodes().Where(p => p.GetType() != typeof(XElement)).ToList();
        if (sortAttributes) xe.ReplaceAttributes(xe.Attributes().OrderBy(x => x.Name.LocalName));
        if (customAttributes == null || customAttributes.Length == 0)
            xe.ReplaceNodes(xe.Elements().OrderBy(x => x.Name.LocalName)
                .Union(nodesToBePreserved.OrderBy(p => p.ToString())).OrderBy(n => n.NodeType.ToString()));
        else
            foreach (var att in customAttributes.Reverse())
                xe.ReplaceNodes(xe.Elements().OrderBy(x => x.Name.LocalName).ThenBy(x =>
                    {
                        var atr = x.Attribute(att);
                        if (atr != null)
                            return (string)atr;
                        return null;
                    })
                    .Union(nodesToBePreserved.OrderBy(p => p.ToString())).OrderBy(n => n.NodeType.ToString()));
        foreach (var xc in xe.Elements())
        {
            postSort?.Invoke(xc);
            if (customAttributes != null) SortXmlElement(xc, sortAttributes, postSort, customAttributes);
        }

        return xe;
    }
}