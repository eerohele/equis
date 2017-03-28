namespace Equis.Tests

module XSLT =
    open System.IO
    open System.Xml
    open System.Xml.Schema

    open Equis
    open Equis.Extensions
    open Expecto

    let private xsl path = path |> FileInfo |> XmlDocument.Load

    let private getStylesheetNames =
        FileInfo >> XSLT.getStylesheets >> List.map (fun x -> x.Name)

    [<Tests>]
    let tests =
        testList "XSLT" [
            test "Use xsl prefix in XPath expressions" {
                let nodes = xsl (resource "stylesheet-a.xsl") |> XSLT.select "/xsl:*"
                Expect.equal nodes.Count 1 "XPath"
            }

            test "Get imported stylesheets" {
                let actual = (resource "stylesheet-c.xsl") |> getStylesheetNames

                let expected =
                    [(resource "stylesheet-c.xsl"); (resource "stylesheet-b.xsl")]
                    |> List.map (fun x -> FileInfo(x).Name)

                Expect.equal actual expected "Imported stylesheets"
            }

            test "Get included stylesheets" {
                let actual = (resource "stylesheet-a.xsl") |> getStylesheetNames

                let expected =
                    [(resource "stylesheet-a.xsl"); (resource "stylesheet-b.xsl")]
                    |> List.map (fun x -> FileInfo(x).Name)

                Expect.equal actual expected "Included stylesheets"
            }
        ]
