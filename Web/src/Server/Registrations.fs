module Server.Registrations

open Suave
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open System
open Suave.ServerErrors
open Server.Domain
open Suave.Logging
open Suave.Logging.Message

let logger = Log.create "TeamCaptainRegistrations"

let registerWithTeam (ctx: HttpContext) =
    Auth.useToken ctx (fun token -> async {
        try
            let registration = 
                ctx.request.rawForm
                |> System.Text.Encoding.UTF8.GetString
                |> FableJson.ofJson<Domain.Registration>
            
            if token.UserName <> registration.UserName then
                return! UNAUTHORIZED (sprintf "Registration is not matching user %s" token.UserName) ctx
            else
            if Validation.verifyRegistration registration then
                Server.Db.Registrations.saveRegistrationToDB registration token.UserName
                return! Successful.OK (FableJson.toJson registration) ctx
            else
                return! BAD_REQUEST "Registration is not valid" ctx
        with exn ->
            logger.error (eventX "Database not available" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx        
    })


let confirmRegistration (ctx: HttpContext) =
    Auth.useToken ctx (fun token -> async {
        try
            let registration = 
                ctx.request.rawForm
                |> System.Text.Encoding.UTF8.GetString
                |> FableJson.ofJson<Domain.Registration>

            let team = Server.Db.Teams.getTeamFromDB registration.TeamName

            match team with
                | None -> return! BAD_REQUEST "Team does not exist" ctx
                | Some t -> 
                    if not (Server.Teams.userIsTeamCaptain token.UserName t) then
                        return! BAD_REQUEST "User is not authorised" ctx
                    else
                        let updatedTeam = {t with Players = Array.append t.Players [|registration.UserName|]} 
                        Server.Db.Teams.updateTeamInDb updatedTeam
                        return! Successful.OK (FableJson.toJson updatedTeam) ctx
            
        with exn ->
            logger.error (eventX "Database not available" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx        
    })