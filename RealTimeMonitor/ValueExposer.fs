namespace Monitor
module ValueExposer =
    open System.Net

    let expose (name:string) (value:obj) =
        let exposeName = System.Environment.MachineName + ":" + name
        let exposeValue = System.Text.Encoding.UTF8.GetBytes (value.ToString())

        let request = WebRequest.Create(Constants.PostUrl)
        (request :?> HttpWebRequest).UserAgent <- "Realtime Monitor"
        request.Method <- "POST"
        request.ContentLength <- (int64) exposeValue.Length
        request.ContentType <- "application/x-www-form-urlencoded"
        let stream = request.GetRequestStream ()
        stream.Write(exposeValue, 0, exposeValue.Length)
        stream.Close()
        request.GetResponse() |> ignore
        Logger.log ("Exposing " + name + " as " + value.ToString())