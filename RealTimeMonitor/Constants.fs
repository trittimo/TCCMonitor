namespace Monitor
module Constants =
    [<Literal>]
    let ServiceName = "Realtime_Server_Monitor_1.0"
    [<Literal>]
    let DisplayName = "Realtime Server Monitor"
    [<Literal>]
    let LogPath = @"C:\realtime_server_monitor_log.txt"
    let WatchDirectories = [|(@"C:\Users\michael.trittin\Desktop", false)|] // path, recursive?