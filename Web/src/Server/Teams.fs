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

let logger = Log.create "TeamCaptain"

let getTeamsFromDB =
    [|
        { Name = "Cleckheaton 2nds"; Captains = [|"CleckCaptain"|]; Players = [||]}
    |]

let getTeamFromDB teamName = 
    let filteredTeam teams =
        match teams with
            | [||] -> None
            | _ -> Some(teams |> Array.exactlyOne)

    filteredTeam
        (getTeamsFromDB |> 
                Array.filter(fun team -> String.Equals(team.Name, teamName, StringComparison.OrdinalIgnoreCase)))
    
let saveRegistrationToDB registration userName = ()

let updateTeamInDb team = ()

let userIsTeamCaptain userName team =
    team.Captains |> Array.contains(userName)

let getAllTeams (ctx: HttpContext) =
    Auth.useToken ctx (fun token -> async {
        try
            let teams = getTeamsFromDB
            return! Successful.OK (FableJson.toJson teams) ctx
        with exn ->
            logger.error (eventX "SERVICE_UNAVAILABLE" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx
    })

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
                saveRegistrationToDB registration token.UserName
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

            let team = getTeamFromDB registration.TeamName

            match team with
                | None -> return! BAD_REQUEST "Team does not exist" ctx
                | Some t -> 
                    if not (userIsTeamCaptain token.UserName t) then
                        return! BAD_REQUEST "User is not authorised" ctx
                    else
                        let updatedTeam = {t with Players = Array.append t.Players [|registration.UserName|]} 
                        updateTeamInDb updatedTeam
                        return! Successful.OK (FableJson.toJson updatedTeam) ctx
            
        with exn ->
            logger.error (eventX "Database not available" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx        
    })