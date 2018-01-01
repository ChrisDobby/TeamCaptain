module Server.Registrations

open Giraffe
open RequestErrors
open ServerErrors
open Server.Domain
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Server
open Microsoft.Extensions.Logging

let registerWithTeam (saveRegistrationToDB: Domain.Registration -> Task<unit>) next (ctx: HttpContext) =
    Auth.useToken next ctx (fun token -> task {
        try
            let! registration = FableJson.getJsonFromCtx<Domain.Registration> ctx
            
            if token.UserName <> registration.UserName then
                return! FORBIDDEN (sprintf "Registration is not matching user %s" token.UserName) next ctx
            else
            if Validation.verifyRegistration registration then
                do! saveRegistrationToDB registration
                return! Successful.OK (FableJson.toJson registration) next ctx
            else
                return! BAD_REQUEST "Registration is not valid" next ctx
        with exn ->
            let logger = ctx.GetLogger "TeamCaptainRegistrations"
            logger.LogError (EventId(), exn, "Database not available")
            return! SERVICE_UNAVAILABLE "Database not available" next ctx        
    })


let confirmRegistration (getTeamFromDB: string -> Task<Team option>) (updateTeamInDb: Team -> Task<unit>) next (ctx: HttpContext) =
    Auth.useToken next ctx (fun token -> task {
        try
            let! registration = FableJson.getJsonFromCtx<Domain.Registration> ctx

            let! team = getTeamFromDB registration.TeamName

            if team = None then
                return! BAD_REQUEST "Team does not exist" next ctx
            else
            if not (Server.Teams.userIsTeamCaptain token.UserName team.Value) then
                return! BAD_REQUEST "User is not authorised" next ctx
            else
                let updatedTeam = { team.Value with Players = registration.UserName::team.Value.Players } 
                do! updateTeamInDb updatedTeam
                return! Successful.OK (FableJson.toJson updatedTeam) next ctx
            
        with exn ->
            let logger = ctx.GetLogger "TeamCaptainRegistrations"
            logger.LogError (EventId(), exn, "Database not available")
            return! SERVICE_UNAVAILABLE "Database not available" next ctx        
    })