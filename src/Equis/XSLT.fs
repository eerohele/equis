namespace Equis

module XSLT =
    open System
    open System.IO
    open System.Xml
    open System.Xml.Xsl

    let manager nametable =
        let m = XmlNamespaceManager(nametable)
        m.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform") |> ignore
        m

    let select expression (node: XmlNode) =
        let nametable =
            match node with
            | :? XmlDocument as doc -> doc.NameTable
            | _ as n -> n.OwnerDocument.NameTable

        node.SelectNodes(expression, manager nametable)

    let private resolve baseUri (href: XmlAttribute) =
        let dir = Uri(baseUri).LocalPath |> DirectoryInfo
        Path.Combine(dir.Parent.FullName, href.Value) |> FileInfo

    let rec getStylesheets stylesheet =
        stylesheet :: aux (XmlDocument.Load stylesheet)

    and aux doc =
        doc
        |> select "xsl:stylesheet/xsl:include/@href | xsl:stylesheet/xsl:import/@href"
        |> Seq.cast<XmlAttribute>
        |> Seq.collect (fun href -> resolve doc.BaseURI href |> getStylesheets)
        |> List.ofSeq
