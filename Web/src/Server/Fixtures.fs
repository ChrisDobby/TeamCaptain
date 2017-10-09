module Server.Fixtures

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

let logger = Log.create "TeamCaptainFixtures"

let createFixture getTeamFromDB saveFixture (ctx: HttpContext) =
    Auth.useToken ctx (fun token -> async {
        try
            let fixture = 
                ctx.request.rawForm
                |> System.Text.Encoding.UTF8.GetString
                |> FableJson.ofJson<Domain.Fixture>

            let! team = getTeamFromDB fixture.TeamName
            if team = None then
                return! BAD_REQUEST "Team does not exist" ctx
            else
            if not (team.Value.Captains |> List.contains(token.UserName)) then
                return! BAD_REQUEST "User does not have permission" ctx
            else
                do! saveFixture fixture
                return! Successful.OK(FableJson.toJson fixture) ctx
        with exn ->
            logger.error (eventX "SERVICE_UNAVAILABLE" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx
    })