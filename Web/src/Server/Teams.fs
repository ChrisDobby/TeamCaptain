module Server.Teams

open System.IO
open Giraffe
open Giraffe.Tasks
open System.Net
open RequestErrors
open ServerErrors
open System
open Server.Domain
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open System.Threading.Tasks
open Server

let userIsTeamCaptain userName team =
    team.Captains |> List.contains(userName)

let teamExists teamName (teams: Team list) =
    teams |> List.map(fun team -> team.Name) |> List.contains(teamName)

let getAllTeams (getTeamsFromDB: Task<Team list>) next (ctx: HttpContext) =
    Auth.useToken next ctx (fun token -> task {
        try
            let! teams = getTeamsFromDB
            return! Successful.OK (FableJson.toJson teams) next ctx
        with exn ->
            let logger = ctx.GetLogger "TeamCaptainTeams"
            logger.LogError (EventId(), exn, "SERVICE_UNAVAILABLE")
            return! SERVICE_UNAVAILABLE "Database not available" next ctx
    })

let registerTeam (getTeamsFromDB: Task<Team list>) (saveTeam: Domain.RegisterTeamRequest -> Task<unit>) next (ctx: HttpContext) =
    Auth.useToken next ctx (fun token -> task {
        try
            let! registerRequest = FableJson.getJsonFromCtx<Domain.RegisterTeamRequest> ctx 

            let! teams = getTeamsFromDB
            if not (String.Equals(registerRequest.UserName, token.UserName, StringComparison.OrdinalIgnoreCase)) then
                return! BAD_REQUEST "Incorrect user" next ctx
            else
            if teamExists registerRequest.Name teams then
                return! BAD_REQUEST "Team already exists" next ctx
            else
                let! newTeam = saveTeam registerRequest
                return! Successful.OK(FableJson.toJson newTeam) next ctx
        with exn ->
            let logger = ctx.GetLogger "TeamCaptainTeams"
            logger.LogError (EventId(), exn, "SERVICE_UNAVAILABLE")
            return! SERVICE_UNAVAILABLE "Database not available" next ctx
    })