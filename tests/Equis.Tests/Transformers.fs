namespace Equis.Tests

module Transformers =
    open System
    open System.IO
    open System.Xml
    open System.Xml.Xsl

    open Equis
    open Equis.Extensions
    open Expecto

    let private stylesheet = FileInfo (resource "stylesheet-a.xsl")

    let private sources =
        [
            (resource "source-a.xml")
            (resource "source-b.xml")
        ] |> List.map FileInfo

    let private tempDir =
        let dir = DirectoryInfo (Path.Combine(Path.GetTempPath(), "Equis"))
        dir.Create()
        dir

    let private withTargetFile f () =
        let targets = sources |> List.map (fun source -> Path.Combine(tempDir.FullName, source.Name))
        f targets
        targets |> Seq.iter File.Delete

    let private transformer =
        let tf = Transformers.Default.compile (XslCompiledTransform()) stylesheet None

        match tf with
        | Ok t -> t
        | Error _ -> failwith "Couldn't load transformer."

    let private transformation = {
        Source = resourceDir
        Filter = "*.xml"
        Target = tempDir
        Parameters = [] |> Transformers.Default.parseParameters
        InputSchema = None
        OutputSchema = None
    }

    [<Tests>]
    let tests =
        testList "Transformer" [
            test "Load an XSLT stylesheet with an error" {
                let xsl = ("stylesheet-with-error.xsl" |> resource |> FileInfo)
                let tf = Transformers.Default.compile (XslCompiledTransform()) xsl None

                match tf with
                | Ok _ -> failwith "Expected stylesheet compilation to fail"
                | Error message -> Expect.stringContains message "The variable or parameter 'UNDEFINED' is either not defined or it is out of scope" "returns an error"
            }
        ]

    [<Tests>]
    let fixtures =
        testList "Transformer" [
            yield! testFixture withTargetFile [
                "Transform source into target",
                    fun targets ->
                        transformer.Transform transformation |> ignore

                        targets
                        |> List.iter (fun target -> Expect.isTrue (File.Exists target) "Target file exists")
            ]
        ]
