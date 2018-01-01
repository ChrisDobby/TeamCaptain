/// Server program entry point module.
module Server.Program

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Giraffe
open Microsoft.AspNetCore
open ServerErrors

let GetEnvVar var = 
    match System.Environment.GetEnvironmentVariable(var) with
    | null -> None
    | value -> Some value

let getPortsOrDefault defaultVal = 
    match System.Environment.GetEnvironmentVariable("SUAVE_FABLE_PORT") with
    | null -> defaultVal
    | value -> value |> uint16

let configureLogging (loggerBuilder : ILoggingBuilder) =
    loggerBuilder.AddFilter(fun lvl -> lvl.Equals LogLevel.Error)
                 .AddConsole()
                 .AddDebug() |> ignore
                 
let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> INTERNAL_ERROR ex.Message

let configureApp db root (app : IApplicationBuilder) =
    app.UseGiraffeErrorHandler(errorHandler)
       .UseStaticFiles()
       .UseGiraffe (WebServer.webApp db root)

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

        WebHost
            .CreateDefaultBuilder()
            .UseWebRoot(clientPath)
            .UseContentRoot(clientPath)
            .ConfigureLogging(configureLogging)
            .Configure(Action<IApplicationBuilder> (configureApp database clientPath))
            .UseUrls("http://0.0.0.0:" + port.ToString() + "/")
            .Build()
            .Run()
        0
    with
    | exn ->
        let color = System.Console.ForegroundColor
        System.Console.ForegroundColor <- System.ConsoleColor.Red
        System.Console.WriteLine(exn.Message)
        System.Console.ForegroundColor <- color
        1
