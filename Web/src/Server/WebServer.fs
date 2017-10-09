/// Functions for managing the Suave web server.
module Server.WebServer

open System.IO
open Suave
open Suave.Logging
open System.Net
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors

type Database = 
    | AzureStorage of Connection: string
    | InMemory

let azureDataFunctions connection = 
    Server.Db.AzureStorage.Teams.getTeams connection, 
    Server.Db.AzureStorage.Teams.getTeam connection,
    Server.Db.AzureStorage.Registrations.saveRegistration connection,
    Server.Db.AzureStorage.Teams.updateTeam connection,
    Server.Db.AzureStorage.Teams.registerTeam connection,
    Server.Db.AzureStorage.Fixtures.getFixturesForTeams connection,
    Server.Db.AzureStorage.Fixtures.saveFixture connection

let inMemoryDataFunctions =
    async { return Server.Db.InMemory.Data.getTeams },
    Server.Db.InMemory.Data.getTeam >> async.Return,
    Server.Db.InMemory.Data.saveRegistration >> async.Return,
    Server.Db.InMemory.Data.updateTeam >> async.Return,
    Server.Db.InMemory.Data.registerTeam >> async.Return,
    Server.Db.InMemory.Data.fixturesForTeams >> async.Return,
    Server.Db.InMemory.Data.saveFixture >> async.Return

// Fire up our web server!
let start clientPath port database =
    if not (Directory.Exists clientPath) then
        failwithf "Client-HomePath '%s' doesn't exist." clientPath

    let logger = Logging.Targets.create Logging.Info [| "Suave" |]
    let serverConfig =
        { defaultConfig with
            logger = Targets.create LogLevel.Debug [|"Server"; "Server" |]
            homeFolder = Some clientPath
            bindings = [ HttpBinding.create HTTP (IPAddress.Parse "0.0.0.0") port] }

    let getTeams, getTeam, saveRegistration, updateTeam, registerTeam, fixturesForTeams, saveFixture =
        (match database with
            | AzureStorage(connection) -> azureDataFunctions connection
            | InMemory -> inMemoryDataFunctions)

    let app =
        choose [
            GET >=> choose [
                path "/" >=> Files.browseFileHome "index.html"
                pathRegex @"/(public|js|css|Images)/(.*)\.(css|png|gif|jpg|js|map)" >=> Files.browseHome

                path "/api/teams/" >=> Teams.getAllTeams getTeams
                path "/api/userDetails/" >=> UserDetails.get getTeams fixturesForTeams ]

            POST >=> choose [ 

                path "/api/register/" >=> Registrations.registerWithTeam saveRegistration
                path "/api/confirmRegistration" >=> Registrations.confirmRegistration getTeam updateTeam
                path "/api/registerTeam" >=> Teams.registerTeam getTeams registerTeam
                path "/api/createFixture/" >=> Fixtures.createFixture getTeam saveFixture
            ]

            NOT_FOUND "Page not found."

        ] >=> logWithLevelStructured Logging.Info logger logFormatStructured

    startWebServer serverConfig app
