namespace AssemblyMonitor
module Configuration =
    open Newtonsoft.Json
    open System.IO
    type Config = {definitions:string[]; refresh:float}
    
    let load (path:string) =
        if not (File.Exists(path)) then
            failwith "Invalid configuration path"
        JsonConvert.DeserializeObject<Config>(File.ReadAllText(path))