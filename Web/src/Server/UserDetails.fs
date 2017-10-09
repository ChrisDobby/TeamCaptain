module Server.UserDetails

open System.IO
open Suave
open Suave.Logging
open System.Net
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open System
open Suave.ServerErrors
open Server.Domain
open Suave.Logging
open Suave.Logging.Message

let logger = Log.create "TeamCaptainUserDetails"

let get (getTeamsFromDB: Async<Team list>) getFixturesForTeam (ctx: HttpContext) =
    Auth.useToken ctx (fun token -> async {
        try
            let! teams = getTeamsFromDB
            let teamsCaptainOf = 
                teams |> 
                List.filter(fun team -> (team.Captains |> List.contains(token.UserName))) |>
                List.map(fun team -> team.Name)

            let teamsMemberOf = 
                teams |> 
                List.filter(fun team -> (team.Players |> List.contains(token.UserName)) && 
                                                         not (team.Captains |> List.contains(token.UserName))) |>
                List.map(fun team -> team.Name)                                                         
    
            let! fixtures = getFixturesForTeam (List.append teamsCaptainOf teamsMemberOf)

            let details = 
                {
                    UserName = token.UserName                    
                    TeamsCaptainOf = teamsCaptainOf
                    TeamsMemberOf = teamsMemberOf
                    Fixtures = fixtures
                }
            return! Successful.OK (FableJson.toJson details) ctx
        with exn ->
            logger.error (eventX "SERVICE_UNAVAILABLE" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx
    })
