namespace Monitor
open System.Reflection
open System
module AssemblyLoader =
    let Invoke path namespaceName methodName =
        let dll = Assembly.LoadFile(path)
        let t = dll.GetType(namespaceName)
        let inst = Activator.CreateInstance(t)
        t.InvokeMember(methodName, BindingFlags.InvokeMethod, null, inst, [||])
