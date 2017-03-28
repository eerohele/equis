namespace Equis

module Validator =
    open System
    open System.Collections.Generic
    open System.IO
    open System.Xml
    open System.Xml.Schema

    open FSharp.Collections.ParallelSeq

    type ValidationResult = Valid | Invalid of Location: Uri * Message: string

    let validate (schema: XmlSchemaSet) (sources: IEnumerable<FileInfo>) =
            let settings =
                XmlReaderSettings(ValidationType = ValidationType.Schema, Schemas = schema)

            sources |> Seq.map (fun source ->
                use reader = XmlReader.Create(source.FullName, settings)
                try
                    while reader.Read() do ()
                    Valid
                with
                    | :? XmlSchemaException as ex -> Invalid (Uri ex.SourceUri, ex.Message)
                    | :? XmlException as ex -> Invalid (Uri ex.SourceUri, ex.Message))

    let private maybeValidate category schema files =
        schema
        |> Option.iter
            (fun schema' ->
                validate schema' (PSeq.ofArray files)
                |> Seq.iter (function
                             | Invalid (uri, message) -> logExceptionWithSource category uri message
                             | Valid -> ()))

    let withValidation (xf: Transformation) action =
        maybeValidate "Validate input" xf.InputSchema xf.Sources
        action()
        maybeValidate "Validate output" xf.OutputSchema xf.Targets
