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
            

        override x.OnStop() = ()