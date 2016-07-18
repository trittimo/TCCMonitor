namespace Monitor
open System.IO
open System
module Logger =
    let log s =
        File.AppendAllText(Constants.LogPath, s + Environment.NewLine)