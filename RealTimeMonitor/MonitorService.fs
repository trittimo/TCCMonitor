namespace Monitor
open System.ServiceProcess
open System.IO

module MonitorService =
    // The windows service
    type Service() =
        inherit ServiceBase(ServiceName = Constants.ServiceName)

        override x.OnStart(args) =
            Logger.log "Started Realtime Server Monitor"

            for towatch in Constants.WatchDirectories do
                let dir, isRec = towatch
                Logger.log("Creating a new FileSystemWatcher for " + dir)
                let watcher = new FileSystemWatcher()
                watcher.Path <- dir
                watcher.EnableRaisingEvents <- true
                watcher.IncludeSubdirectories <- isRec
                let onChange(e: FileSystemEventArgs) =
                    Logger.log(e.FullPath + " was " + e.ChangeType.ToString().ToLower())
                watcher.Renamed.Add(onChange) |> ignore
                watcher.Changed.Add(onChange) |> ignore
                watcher.Created.Add(onChange) |> ignore
                watcher.Deleted.Add(onChange) |> ignore
        override x.OnStop() = ()