/// Functions for managing the Suave web server.
module Server.WebServer

open Giraffe
open Giraffe.TokenRouter
open RequestErrors

type Database = 
    | AzureStorage of Connection: string
    | InMemory

let azureDataFunctions connection = 
    (fun () -> Server.Db.AzureStorage.Teams.getTeams connection), 
    Server.Db.AzureStorage.Teams.getTeam connection,
    Server.Db.AzureStorage.Registrations.saveRegistration connection,
    Server.Db.AzureStorage.Teams.updateTeam connection,
    Server.Db.AzureStorage.Teams.registerTeam connection,
    Server.Db.AzureStorage.Fixtures.getFixturesForTeams connection System.DateTimeOffset.Now,
    Server.Db.AzureStorage.Fixtures.saveFixture connection

let inMemoryDataFunctions =
    (fun () -> task { return Server.Db.InMemory.Data.getTeams () }),
    (fun name -> task { return Server.Db.InMemory.Data.getTeam name }),
    (fun reg -> task { return Server.Db.InMemory.Data.saveRegistration reg }),
    (fun team -> task { return Server.Db.InMemory.Data.updateTeam team }),
    (fun team -> task { return Server.Db.InMemory.Data.registerTeam team }),
    (fun teams -> task { return Server.Db.InMemory.Data.fixturesForTeams System.DateTimeOffset.Now teams }),
    (fun fix -> task { return Server.Db.InMemory.Data.saveFixture fix })

let webApp database root =

    let getTeams, getTeam, saveRegistration, updateTeam, registerTeam, fixturesForTeams, saveFixture =
        (match database with
            | AzureStorage(connection) -> azureDataFunctions connection
            | InMemory -> inMemoryDataFunctions)

    let notFound = NOT_FOUND "Page not found"

    router notFound [
            GET [
                route "/" (htmlFile (System.IO.Path.Combine(root,"index.html")))

                route "/api/teams/" (Teams.getAllTeams getTeams)
                route "/api/userdetails/" (UserDetails.get getTeams fixturesForTeams) ]

            POST [ 
                route "/api/register/" (Registrations.registerWithTeam saveRegistration)
                route "/api/confirmregistration" (Registrations.confirmRegistration getTeam updateTeam)
                route "/api/registerteam" (Teams.registerTeam getTeams registerTeam)
                route "/api/createfixture/" (Fixtures.createFixture getTeam saveFixture)
            ]        
    ]
