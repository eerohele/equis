namespace Equis.Tests

module Validator =
    open System
    open System.IO
    open System.Xml
    open System.Xml.Schema

    open Equis
    open Equis.Validator
    open Equis.Extensions
    open Expecto

    let schema =
        match resource "schema.xsd" |> XmlSchemaSet.Load with
        | Ok s -> s
        | Error _ -> failwith "Couldn't load schema"

    let sources =
        [
            (resource "source-a.xml")
            (resource "source-e.xml")
        ] |> List.map FileInfo

    [<Tests>]
    let tests =
        testList "Validator" [
            test "Validate a sequence of files" {
                Expect.sequenceEqual
                    [Valid; Invalid (Uri (resource "source-e.xml"), "The 'e' element is not declared.")]
                    (Validator.validate schema sources)
                    "Validate sources"
            }
        ]
