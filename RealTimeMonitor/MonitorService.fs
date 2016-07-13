namespace Monitor
open System.ComponentModel
open System.Configuration.Install
open System.ServiceProcess
open System.Collections.Generic
open System.IO
open System

module Constants =
    let ServiceName = "Realtime_Server_Monitor_1.0"
    let DisplayName = "Realtime Server Monitor"
    let LogPath = @"C:\realtime_server_monitor_log.txt"
    let WatchDirectories = [|(@"C:\Users\michael.trittin\Desktop", false)|] // path, recursive?

module Logger =
    let log s =
        File.AppendAllText(Constants.LogPath, s + Environment.NewLine)
// The windows service
type MonitorWindowsService() =
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
                Logger.log(e.FullPath + " was " + e.ChangeType.ToString())
            watcher.Renamed.Add(onChange) |> ignore
            watcher.Changed.Add(onChange) |> ignore
            watcher.Created.Add(onChange) |> ignore
    override x.OnStop() = ()

// Installer for the service
[<RunInstaller(true)>]
type FSharpServiceInstaller() =
    inherit Installer()
    do
        new ServiceProcessInstaller
            (Account = ServiceAccount.LocalSystem)
        |> base.Installers.Add |> ignore

        new ServiceInstaller (
            DisplayName = Constants.DisplayName,
            ServiceName = Constants.ServiceName,
            StartType = ServiceStartMode.Automatic
        ) |> base.Installers.Add |> ignore

module Main =
    ServiceBase.Run [| new MonitorWindowsService() :> ServiceBase |]