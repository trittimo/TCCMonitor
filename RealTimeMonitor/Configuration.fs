namespace Monitor
module Configuration =
    open Newtonsoft.Json
    open System.IO
    type Config = {definitions:string[]; refresh:float}
    
    let load (path:string) =
        if not (File.Exists(path)) then
            Logger.log ("Encountered exception loading configuration path: " + path + ". Could not find file. Stopping service...")
            failwith "Invalid configuration path"
        JsonConvert.DeserializeObject<Config>(File.ReadAllText(path))