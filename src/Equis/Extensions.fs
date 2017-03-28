namespace Equis

[<AutoOpen>]
module Extensions =
    open System
    open System.IO
    open System.Xml
    open System.Xml.Schema

    type XmlDocument with
        static member Load(file: FileInfo) =
            let doc = XmlDocument()
            doc.Load(file.FullName)
            doc

    type XmlSchemaSet with
        static member Load(path: string): Result<XmlSchemaSet, (Uri * string)> =
            let schemaSet = XmlSchemaSet()

            try
                use reader = XmlReader.Create(path)
                XmlSchema.Read(reader, null) |> schemaSet.Add |> ignore
                schemaSet.Compile()
                Ok schemaSet
            with
                | :? FileNotFoundException as ex -> Error (Uri path, ex.Message)
                | :? XmlException as ex -> Error (Uri ex.SourceUri, ex.Message)
