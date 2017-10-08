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

let registerWithTeam saveRegistrationToDB (ctx: HttpContext) =
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
                saveRegistrationToDB registration token.UserName
                return! Successful.OK (FableJson.toJson registration) ctx
            else
                return! BAD_REQUEST "Registration is not valid" ctx
        with exn ->
            logger.error (eventX "Database not available" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx        
    })


let confirmRegistration getTeamFromDB updateTeamInDb (ctx: HttpContext) =
    Auth.useToken ctx (fun token -> async {
        try
            let registration = 
                ctx.request.rawForm
                |> System.Text.Encoding.UTF8.GetString
                |> FableJson.ofJson<Domain.Registration>

            return!
                getTeamFromDB registration.TeamName
                |> Option.map(fun team -> 
                                    if not (Server.Teams.userIsTeamCaptain token.UserName team) then
                                        BAD_REQUEST "User is not authorised" ctx
                                    else
                                        let updatedTeam = {team with Players = Array.append team.Players [|registration.UserName|]} 
                                        updateTeamInDb updatedTeam
                                        Successful.OK (FableJson.toJson updatedTeam) ctx)
                |> Option.defaultValue (BAD_REQUEST "Team does not exist" ctx)
            
        with exn ->
            logger.error (eventX "Database not available" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx        
    })