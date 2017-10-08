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

let getAllTeams getTeamsFromDB (ctx: HttpContext) =
    Auth.useToken ctx (fun token -> async {
        try
            let! teams = getTeamsFromDB
            return! Successful.OK (FableJson.toJson teams) ctx
        with exn ->
            logger.error (eventX "SERVICE_UNAVAILABLE" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx
    })
