namespace Equis

module Transformers =
    module Default =
        open System
        open System.Collections.Generic
        open System.IO
        open System.Xml
        open System.Xml.Schema
        open System.Xml.Xsl

        type XsltArgumentList with
            static member OfIDictionary(dictionary: IDictionary<XmlQualifiedName, obj>) =
                let args = XsltArgumentList()
                for kv in dictionary do args.AddParam(kv.Key.Name, kv.Key.Namespace, kv.Value)
                args

        let xmlUrlResolver = XmlUrlResolver()

        let defaultXsltSettings =
            let s = XsltSettings()
            s.EnableScript <- true
            s.EnableDocumentFunction <- true
            s

        let parseParameters (strings: string list) =
            let dictionary = new Dictionary<XmlQualifiedName, obj>()

            for key, value in (List.pairwise strings) do
                dictionary.Add(XmlQualifiedName(key), value :> obj)

            dictionary :> IDictionary<XmlQualifiedName, obj>

        let rec compile (transformer: XslCompiledTransform) (stylesheet: FileInfo) xsltSettings =
            let settings = defaultArg xsltSettings defaultXsltSettings

            use xmlReader = XmlReader.Create(stylesheet.FullName)

            try
                transformer.Load(xmlReader, settings, xmlUrlResolver)

                let transform = fun (xf: Transformation) ->
                    xf.Sources
                    |> Seq.map (fun source ->
                        let targetPath = Path.Combine(xf.Target.FullName, source.Name)
                        let parameters = XsltArgumentList.OfIDictionary(xf.Parameters)

                        use xmlWriter = XmlWriter.Create(targetPath, transformer.OutputSettings)

                        try
                            transformer.Transform(source.FullName, parameters, xmlWriter)
                            Ok ()
                        with
                        | :? XsltException as ex ->
                            let num = ex.LineNumber |> string
                            let pos = ex.LinePosition |> string
                            let msg = ex.Message

                            Error (sprintf "%s:%s:%s: %s" stylesheet.Name num pos msg))
                    |> List.ofSeq
                    |> List.head

                Ok {
                    Transform = transform
                    Path = stylesheet
                    Compile = (fun () -> compile transformer stylesheet (Some settings))
                }
            with
                | :? XsltException as ex -> ex |> string |> Error
