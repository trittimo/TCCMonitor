namespace Monitor
module AssemblyLoader =
    open Newtonsoft.Json
    open System.IO
    open System.Timers
    open System.Reflection
    open System.Collections.Generic
    open System.Collections.Concurrent

    type AssemblyFile = {path:string; operations:Operation[]}
    and Operation = {kind:string; name:string; exposedName:string; operations:Operation[]; refresh:float; arguments:obj[];}

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
        member val Exposed = new ConcurrentDictionary<string, System.Object>()


        member this.SetExposedValue (key:string) (item:obj) =
            match item with
            | null -> ()
            | _ ->
                if (this.Exposed.ContainsKey(key)) then
                    this.Exposed.[key] <- item
                else if not (this.Exposed.TryAdd(key, item)) then
                    this.SetExposedValue key item

        member this.ConvertArguments (args:obj[]) (parameters:ParameterInfo[]) =
            Array.map2 (fun (arg:obj) (param:ParameterInfo) ->
                System.Convert.ChangeType(arg, param.ParameterType)) args parameters

        member this.RunOperation (operation:Operation) =
            let assembly = Assembly.LoadFile(this.File.path)
            let instance, t = 
                match operation.kind with
                | "class" -> (None, Some(assembly.GetType(operation.name)))
                | "instance class" ->
                    let asm = assembly.GetType(operation.name)
                    let args = 
                        this.ConvertArguments (operation.arguments) ((asm.GetConstructors() |> Array.find(fun x -> x.GetParameters().Length = operation.arguments.Length)).GetParameters())
                    (Some(System.Activator.CreateInstance(asm, args)), Some(asm))
                | _ -> (None, None)
                
            match t with
            | Some(clazz) ->
                operation.operations |> Array.iter(fun item -> this.RunParentedOperation assembly item clazz instance)
            | _ -> ()

        member this.RunParentedOperation (assembly:Assembly) (operation:Operation) (parent:System.Type) (instance:Option<obj>) =
            match (operation.kind, instance) with
            | ("instance member", Some(inst)) -> 
                let field = parent.GetField(operation.name)
                let value = field.GetValue(inst)
                this.SetExposedValue operation.exposedName value
            | ("static member", _) ->
                let field = parent.GetField(operation.name)
                let value = field.GetValue(null)
                this.SetExposedValue operation.exposedName value
            | ("instance method", Some(inst)) ->
                let m = parent.GetMethod(operation.name)
                let args = 
                    match operation.arguments with
                    | null -> null
                    | _ -> this.ConvertArguments (operation.arguments) (m.GetParameters())
                this.SetExposedValue operation.exposedName (m.Invoke(inst, args))
            | ("static method", _) ->
                let m = parent.GetMethod(operation.name)
                let args = 
                    match operation.arguments with
                    | null -> null
                    | _ -> this.ConvertArguments (operation.arguments) (m.GetParameters())
                this.SetExposedValue operation.exposedName (m.Invoke(null, args))
            | _ -> ()


    let private getAssemblyConfiguration (path:string) : AssemblyFile =
        if not (File.Exists(path)) then
            failwith "Invalid configuration path"
        JsonConvert.DeserializeObject<AssemblyFile>(File.ReadAllText(path))
    
    let load (path:string) : AssemblyRunner =
        let config = getAssemblyConfiguration path
        new AssemblyRunner(config)