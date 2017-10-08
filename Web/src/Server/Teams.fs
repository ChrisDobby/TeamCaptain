module Server.Teams

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

let logger = Log.create "TeamCaptainTeams"

let userIsTeamCaptain userName team =
    team.Captains |> List.contains(userName)

let teamExists teamName (teams: Team list) =
    teams |> List.map(fun team -> team.Name) |> List.contains(teamName)

let getAllTeams getTeamsFromDB (ctx: HttpContext) =
    Auth.useToken ctx (fun token -> async {
        try
            let! teams = getTeamsFromDB
            return! Successful.OK (FableJson.toJson teams) ctx
        with exn ->
            logger.error (eventX "SERVICE_UNAVAILABLE" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx
    })

let registerTeam (getTeamsFromDB: Async<Team list>) saveTeam (ctx: HttpContext) =
    Auth.useToken ctx (fun token -> async {
        try
            let registerRequest = 
                ctx.request.rawForm
                |> System.Text.Encoding.UTF8.GetString
                |> FableJson.ofJson<Domain.RegisterTeamRequest>

            let! teams = getTeamsFromDB
            if not (String.Equals(registerRequest.UserName, token.UserName, StringComparison.OrdinalIgnoreCase)) then
                return! BAD_REQUEST "Incorrect user" ctx
            else
            if teamExists registerRequest.Name teams then
                return! BAD_REQUEST "Team already exists" ctx
            else
                let! newTeam = saveTeam registerRequest
                return! Successful.OK(FableJson.toJson newTeam) ctx
        with exn ->
            logger.error (eventX "SERVICE_UNAVAILABLE" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx
    })