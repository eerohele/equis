namespace Equis

open System
open System.IO
open System.Collections.Generic
open System.Xml
open System.Xml.Schema

type Transformation = {
    Source: DirectoryInfo
    Filter: string
    Target: DirectoryInfo
    Parameters: IDictionary<XmlQualifiedName, obj>
    InputSchema: XmlSchemaSet option
    OutputSchema: XmlSchemaSet option
} with
      member this.Sources = this.Source.GetFiles(this.Filter)
      member this.Targets = this.Target.GetFiles(this.Filter)

type Transformer = {
    Path: FileInfo
    Transform: Transformation -> Result<unit, string>
    Compile: unit -> Result<Transformer, string>
}
