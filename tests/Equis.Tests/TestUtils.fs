namespace Equis.Tests

[<AutoOpen>]
module TestUtils =
    open System.IO
    open System.Reflection

    let private assemblyLocation =
        Assembly.GetExecutingAssembly().Location

    let resourceDir =
        Path.Combine(assemblyLocation, "../../../../../resources")
        |> DirectoryInfo

    let resource path = Path.Combine(resourceDir.FullName, path)
