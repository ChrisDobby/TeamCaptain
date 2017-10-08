/// Server program entry point module.
module Server.Program

open System.IO

let GetEnvVar var = 
    match System.Environment.GetEnvironmentVariable(var) with
    | null -> None
    | value -> Some value

let getPortsOrDefault defaultVal = 
    match System.Environment.GetEnvironmentVariable("SUAVE_FABLE_PORT") with
    | null -> defaultVal
    | value -> value |> uint16

[<EntryPoint>]
let main args =
    try
        let clientPath =
            match args |> Array.toList with
            | clientPath:: _  when Directory.Exists clientPath -> clientPath
            | _ -> 
                // did we start from server folder?
                let devPath = Path.Combine("..","Client")
                if Directory.Exists devPath then devPath 
                else
                    // maybe we are in root of project?
                    let devPath = Path.Combine("src","Client")
                    if Directory.Exists devPath then devPath 
                    else @"./client"
            |> Path.GetFullPath

        let database = 
            args
            |> Array.tryFind(fun arg -> arg.StartsWith("Connection="))
            |> Option.map(fun arg -> WebServer.AzureStorage(arg.Substring "Connection=".Length))
            |> Option.defaultValue WebServer.InMemory

        let port = getPortsOrDefault 8085us
        WebServer.start clientPath port database
        0
    with
    | exn ->
        let color = System.Console.ForegroundColor
        System.Console.ForegroundColor <- System.ConsoleColor.Red
        System.Console.WriteLine(exn.Message)
        System.Console.ForegroundColor <- color
        1
