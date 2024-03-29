﻿namespace Monitor
module Constants =
    open System
    open System.IO
    [<Literal>]
    let ServiceName = "Realtime_Server_Monitor_1.0"
    [<Literal>]
    let DisplayName = "Realtime Server Monitor"
    [<Literal>]
    let PostUrl = "http://requestb.in/1kth3161"

    let LogPath = Path.Combine (@"C:\", "monitor_log.txt")
    let ConfigPath = Path.Combine (@"C:\", "monitor_config.cfg")
