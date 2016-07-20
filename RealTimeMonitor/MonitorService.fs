namespace Monitor
open System.ServiceProcess
open System.IO
open Newtonsoft.Json

module MonitorService =
    // The windows service
    type Service() =
        inherit ServiceBase(ServiceName = Constants.ServiceName)

        override x.OnStart(args) =
            Logger.log "Started Realtime Server Monitor"
            let config = Configuration.init Constants.ConfigPath
            Logger.log ("Loaded configuration. Monitoring " + config.definitions.Length.ToString() + " assemblies: ")
            config.definitions |> Array.iter(fun x ->
                Logger.log ("\tMonitoring " + x.ToString()))

        override x.OnStop() =
            Logger.log "Stopping Realtime Server Monitor"