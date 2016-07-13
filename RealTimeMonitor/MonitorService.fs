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

// The windows service
type MonitorWindowsService() =
    inherit ServiceBase(ServiceName = Constants.ServiceName)

    override x.OnStart(args) =
        File.WriteAllText(@"C:\Users\michael.trittin\Desktop\service_log.txt",
                "Started monitor service")
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