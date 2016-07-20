namespace Monitor
module Configuration =
    open Newtonsoft.Json
    type Config = {definitions:string[]}
    
    let Load (path:string) =
        