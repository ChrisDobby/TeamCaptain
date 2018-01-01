module Server.Fixtures

open Giraffe
open RequestErrors
open ServerErrors
open Server.Domain
open Microsoft.AspNetCore.Http
open Server
open Microsoft.Extensions.Logging
open System.Threading.Tasks

let createFixture (getTeamFromDB: string -> Task<Domain.Team option>) (saveFixture: Fixture -> Task<unit>) next (ctx: HttpContext) =
    Auth.useToken next ctx (fun token -> task {
        try
            let! fixture = FableJson.getJsonFromCtx<Domain.Fixture> ctx 

            let! team = getTeamFromDB fixture.TeamName
            if team = None then
                return! BAD_REQUEST "Team does not exist" next ctx
            else
            if not (team.Value.Captains |> List.contains(token.UserName)) then
                return! BAD_REQUEST "User does not have permission" next ctx
            else
                do! saveFixture fixture
                return! Successful.OK(FableJson.toJson fixture) next ctx
        with exn ->
            let logger = ctx.GetLogger "TeamCaptainFixtures"
            logger.LogError (EventId(), exn, "SERVICE_UNAVAILABLE")
            return! SERVICE_UNAVAILABLE "Database not available" next ctx        
    })