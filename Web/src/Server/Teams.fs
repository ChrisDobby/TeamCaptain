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
        { Name = "Cleckheaton 2nds" }
    |]

let saveRegistrationToDB registration = ()

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
            
            if Validation.verifyRegistration registration then
                saveRegistrationToDB registration
                return! Successful.OK (FableJson.toJson registration) ctx
            else
                return! BAD_REQUEST "Regostration is not valid" ctx
        with exn ->
            logger.error (eventX "Database not available" >> addExn exn)
            return! SERVICE_UNAVAILABLE "Database not available" ctx        
    })