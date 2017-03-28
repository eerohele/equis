namespace Equis

module Watcher =
    open System
    open System.IO
    open System.Reactive
    open System.Reactive.Linq
    open System.Xml
    open System.Threading

    type private FileSystemWatcher with
        member this.Subscribe(onNext: EventPattern<FileSystemEventArgs> -> unit) =
            Observable.FromEventPattern<FileSystemEventArgs>(this, "Changed")
                      .Throttle(TimeSpan(500000L))
                      .Subscribe(onNext) |> ignore

            this

    let private createWatcher path filter =
        new FileSystemWatcher(
            Path = path,
            Filter = filter,
            NotifyFilter = NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        )

    let private watch dir filter callback =
        let watcher = createWatcher dir filter
        watcher.Subscribe(callback)

    let private watchStylesheets (transformer: Transformer) (xf: Transformation) =
        XSLT.getStylesheets transformer.Path
        |> List.map (fun xsl ->
            watch xsl.DirectoryName xsl.Name
                (fun pattern -> Subscribers.stylesheet transformer xf pattern.EventArgs.Name))

    let private watchSource transformer (xf: Transformation) =
        watch xf.Source.FullName xf.Filter
            (fun pattern -> Subscribers.source transformer xf pattern.EventArgs.Name)

    let private doUntilCanceled action =
        use mre = new ManualResetEventSlim(false)
        action()
        use sub = Console.CancelKeyPress.Subscribe (fun _ -> mre.Set())
        mre.Wait()

    let start (transformer: Transformer) (xf: Transformation) =
        Log.Information (
            "Watching {xsl} and {dir}/{glob} for changes.",
            transformer.Path.Name,
            xf.Source.Name,
            xf.Filter
        )

        doUntilCanceled <| fun () ->
            watchSource transformer xf :: watchStylesheets transformer xf
            |> List.iter (fun watcher ->
                watcher.WaitForChanged(WatcherChangeTypes.Changed) |> ignore)
