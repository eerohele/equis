namespace Equis

module Subscribers =
    open System
    open System.IO
    open System.Reactive
    open System.Xml
    open System.Xml.Schema

    open FSharp.Collections.ParallelSeq

    let private tryExecute fileName action =
        try
            action()
            Log.Information fileName
        with
            | :? XmlException as ex ->
                logExceptionWithSource "Transformation" (Uri ex.SourceUri) ex.Message
            | :? InvalidOperationException as ex ->
                Log.Error ex.Message

    let private transform args transformer xf =
        tryExecute args <| fun () ->
            Validator.withValidation xf <| fun () ->
                transformer.Compile()
                |> Result.bind (fun t -> t.Transform xf)
                |> Result.mapError Log.Error
                |> ignore

    let stylesheet transformer xf fileName = transform fileName transformer xf

    // TODO: Investigate whether it's possible to avoid recompiling the stylesheet when only the
    // source changes.
    let source transformer xf fileName =
        transform fileName transformer { xf with Filter = fileName }
