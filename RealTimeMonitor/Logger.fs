namespace Monitor
module Logger =
    open System.IO
    open System
    let log s =
        File.AppendAllText(Constants.LogPath, s + Environment.NewLine)