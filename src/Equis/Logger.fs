namespace Equis

[<AutoOpen>]
module Logger =
    open System
    open System.IO

    open Serilog

    let Log = LoggerConfiguration().WriteTo.LiterateConsole().CreateLogger()

    let logExceptionWithSource (category: string) (sourceUri: Uri) (message: string) =
        Log.Error (
            "{category}: {source}: {message}",
            category,
            (FileInfo(sourceUri.LocalPath).Name), message
        )
