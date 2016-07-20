namespace Monitor
open System.ServiceProcess
open System.IO
open Newtonsoft.Json

module MonitorService =
    // The windows service
    type Service() =
        inherit ServiceBase(ServiceName = Constants.ServiceName)

        member this.Initialize (path:string) =
            let config = Configuration.load path
            let loaders =
                config.definitions |> Array.map(fun definition ->
                    AssemblyLoader.load definition)
            let timer = new System.Timers.Timer()
            timer.Elapsed.Add(fun _ ->
                loaders |> Seq.iter(fun x ->
                    let exposed = (sprintf "%A" (x.Exposed))
                    Logger.log("Exposed: " + exposed)))
            timer.Interval <- config.refresh
            timer.Enabled <- true
            timer.Start()
            config

        override this.OnStart(args) =
            Logger.log "Started Realtime Server Monitor"
            let config = this.Initialize Constants.ConfigPath
            Logger.log ("Loaded configuration. Monitoring " + config.definitions.Length.ToString() + " assemblies: ")
            config.definitions |> Array.iter(fun x ->
                Logger.log ("\tMonitoring " + x.ToString()))

        override this.OnStop() =
            Logger.log "Stopping Realtime Server Monitor"