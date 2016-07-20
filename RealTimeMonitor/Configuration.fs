namespace Monitor
module Configuration =
    open Newtonsoft.Json
    open System.IO
    type Config = {definitions:string[]; refresh:float}
    
    let private load (path:string) =
        if not (File.Exists(path)) then
            Logger.log ("Encountered exception loading configuration path: " + path + ". Could not find file. Stopping service...")
            failwith "Invalid configuration path"
        JsonConvert.DeserializeObject<Config>(File.ReadAllText(path))

    let init (path:string) =
        let config = load path
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