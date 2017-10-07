/// Functions for managing the Suave web server.
module Server.WebServer

open System.IO
open Suave
open Suave.Logging
open System.Net
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors

// Fire up our web server!
let start clientPath port =
    if not (Directory.Exists clientPath) then
        failwithf "Client-HomePath '%s' doesn't exist." clientPath

    let logger = Logging.Targets.create Logging.Info [| "Suave" |]
    let serverConfig =
        { defaultConfig with
            logger = Targets.create LogLevel.Debug [|"Server"; "Server" |]
            homeFolder = Some clientPath
            bindings = [ HttpBinding.create HTTP (IPAddress.Parse "0.0.0.0") port] }

    let app =
        choose [
            GET >=> choose [
                path "/" >=> Files.browseFileHome "index.html"
                pathRegex @"/(public|js|css|Images)/(.*)\.(css|png|gif|jpg|js|map)" >=> Files.browseHome

                path "/api/teams/" >=> Teams.getAllTeams ]

            POST >=> choose [ 

                path "/api/register/" >=> Teams.registerWithTeam
                path "api/confirmRegistration" >=> Teams.confirmRegistration
            ]

            NOT_FOUND "Page not found."

        ] >=> logWithLevelStructured Logging.Info logger logFormatStructured

    startWebServer serverConfig app
