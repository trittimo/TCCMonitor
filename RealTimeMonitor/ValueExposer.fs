namespace Monitor
module ValueExposer =
    let expose (name:string) (value:obj) =
        // TODO
        Logger.log ("Exposing " + name + " as " + value.ToString())