namespace Monitor
open System.ComponentModel
open System.Configuration.Install
open System.ServiceProcess
open System.Collections.Generic
open System

module MonitorBase =
    type Monitor() =
        member x.Start() = // TODO
            printfn "Starting monitor..."
            true

module Constants =
    let ServiceName = "Realtime_Server_Monitor_1.0"
    let DisplayName = "Realtime Server Monitor"

// The IMonitorService interface supplies the definitions for how we will interact with our service
// The meat of the work will happen in the MonitorBase.Monitor class
type IMonitorService =
    abstract Start : unit -> bool

// The service is where we start and stop our monitoring system, and also provide the data to any clients
type MonitorService() =
    inherit MarshalByRefObject()
    let agent = new MonitorBase.Monitor()
    interface IMonitorService with
        member x.Start() = agent.Start()

// The windows service
type MonitorWindowsService() =
    inherit ServiceBase(ServiceName = Constants.ServiceName)

    override x.OnStart(args) =
        printfn "Starting the service..."
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