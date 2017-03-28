namespace Equis

module Equis =
    open System
    open System.Collections.Generic
    open System.IO
    open System.Xml
    open System.Xml.Schema
    open System.Xml.Xsl

    open Argu

    type CommonArguments =
        | [<Mandatory; ExactlyOnce>] Stylesheet of path: string
        | [<Mandatory; ExactlyOnce>] Source of path: string
        | [<ExactlyOnce>] Filter of pattern: string
        | [<ExactlyOnce>] Target of path: string
        | [<ExactlyOnce>] Parameters of parameters: string list
        | [<ExactlyOnce>] Input_Schema of path: string
        | [<ExactlyOnce>] Output_Schema of path: string
    with
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | Stylesheet _ -> "the XSLT stylesheet that defines the XML transformation."
                | Source _ -> "the directory with XML source files to transform."
                | Filter _ -> "a pattern that selects the XML files to transform."
                | Target _ -> "output directory."
                | Parameters _ -> "parameters for the XSLT transformation"
                | Input_Schema _ -> "the XML schema to use for validating the input"
                | Output_Schema _ -> "the XML schema to use for validating the output"
    and CliArguments =
        | [<CliPrefix(CliPrefix.None)>] Transform of ParseResults<CommonArguments>
        | [<CliPrefix(CliPrefix.None)>] Watch of ParseResults<CommonArguments>
    with
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | Watch _ -> "watch an XSLT stylesheet and a directory of XML source files for changes and transform them"
                | Transform _ -> "transform a directory of XML source files with an XSLT stylesheet"

    let private defaultOutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "out")

    let private loadSchema path =
        match (path |> XmlSchemaSet.Load) with
        | Ok schemaSet -> Some schemaSet
        | Error (uri, message) ->
            logExceptionWithSource "XML schema loading" uri message
            None

    let private getTransformation (results: ParseResults<CommonArguments>) =
        {
            Source = DirectoryInfo(results.GetResult (<@ Source @>))
            Filter = results.GetResult (<@ Filter @>, defaultValue = "*.xml")
            Target = DirectoryInfo(results.GetResult (<@ Target @>, defaultValue = defaultOutputDirectory))
            Parameters = results.GetResult (<@ Parameters @>, defaultValue = []) |> Transformers.Default.parseParameters
            InputSchema = results.TryGetResult ( <@ Input_Schema @> ) |> Option.bind loadSchema
            OutputSchema = results.TryGetResult ( <@ Output_Schema @> ) |> Option.bind loadSchema
        }

    let private getTransformer (results: ParseResults<CommonArguments>) =
        let stylesheet = FileInfo (results.GetResult (<@ Stylesheet @>))
        Transformers.Default.compile (XslCompiledTransform()) stylesheet None

    [<EntryPoint>]
    let main argv =
        let argu = ArgumentParser.Create<CliArguments>(programName = "Equis.exe")

        try
            let cli = argu.Parse argv

            match cli.TryGetSubCommand() with
            | Some (Watch result) ->
                match getTransformer result with
                | Ok transformer ->
                    let xf = getTransformation result
                    xf.Target.Create()
                    Watcher.start transformer xf
                | Error message -> Log.Error message
            | Some (Transform result) ->
                // TODO: Refactor to use Result.bind?
                match getTransformer result with
                | Ok transformer ->
                    let xf = getTransformation result

                    xf.Target.Create()

                    Validator.withValidation xf <| fun () ->
                        match transformer.Transform xf with
                        | Ok _ -> ()
                        | Error message -> Log.Error message
                | Error message -> Log.Error message

            | None -> argu.PrintUsage() |> Console.WriteLine

            0
        with
            | :? ArguParseException as ex ->
                ex.Message |> Console.WriteLine
                1
            | :? ArgumentException as ex ->
                Log.Fatal ex.Message
                Log.Error "--source takes a directory as an argument; maybe you're trying to give it a single file?"
                1
            | ex ->
                Log.Fatal ex.Message
                1
