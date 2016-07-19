namespace Monitor
module AssemblyLoader =
    open Newtonsoft.Json
    open System.IO
    open System.Timers
    open System.Reflection
    open System.Collections.Generic

    type AssemblyFile = {path:string; operations:Operation[]}
    and Operation = {kind:string; name:string; exposedName:string; operations:Operation[]; refresh:float; arguments:Argument[];}
    and Argument = {kind:string; value:string;}

    type AssemblyRunner(file:AssemblyFile) as this =
        do
            file.operations |>
                Array.iter(fun operation ->
                    let timer = new Timer()
                    timer.Elapsed.Add(fun args -> this.RunOperation operation)
                    timer.Interval <- operation.refresh
                    timer.Enabled <- true
                    timer.Start())
        
        member this.File = file
        member this.Exposed = new Dictionary<string, obj>()
        member this.RunOperation (operation:Operation) =
            let assembly = Assembly.LoadFile(this.File.path)
            let instance, t = 
                match operation.kind with
                | "class" -> (None, Some(assembly.GetType(operation.name)))
                | "instance class" ->
                    let asm = assembly.GetType(operation.name)
                    (Some(System.Activator.CreateInstance(asm)), Some(asm))
                | _ -> (None, None)
                
            match t with
            | Some(clazz) ->
                operation.operations |> Array.iter(fun item -> this.RunParentedOperation assembly item clazz instance)
            | _ -> ()

        member this.RunParentedOperation (assembly:Assembly) (operation:Operation) (parent:System.Type) (instance:Option<obj>) =
            match (operation.kind, instance) with
            | ("instance member", Some(inst)) -> this.Exposed.Add(operation.exposedName, parent.GetField(operation.name))
            | ("member", _) -> this.Exposed.Add(operation.exposedName, parent.GetField(operation.name))
            | ("instance method", Some(inst)) ->
                this.Exposed.Add
                    (operation.exposedName,parent.InvokeMember(operation.name, BindingFlags.InvokeMethod, null, inst, [||]))
            | ("method", _) ->
                this.Exposed.Add
                    (operation.exposedName, parent.InvokeMember(operation.name, BindingFlags.InvokeMethod, null, null, [||]))
            | _ -> ()


    let getAssemblyConfiguration (path:string) : AssemblyFile =
        JsonConvert.DeserializeObject<AssemblyFile>(File.ReadAllText(path))
    
    let load (path:string) : AssemblyRunner =
        let config = getAssemblyConfiguration path
        new AssemblyRunner(config)