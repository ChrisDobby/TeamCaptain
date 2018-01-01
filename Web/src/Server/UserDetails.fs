module Server.UserDetails

open Giraffe
open ServerErrors
open Server.Domain
open Microsoft.AspNetCore.Http
open Server
open Microsoft.Extensions.Logging
open System.Threading.Tasks

let get (getTeamsFromDB: Task<Domain.Team list>) (getFixturesForTeam: string list -> Task<Domain.Fixture list>) next (ctx: HttpContext) =
    Auth.useToken next ctx (fun token -> task {
        try
            let! teams = getTeamsFromDB
            let teamsCaptainOf = 
                teams |> 
                List.filter(fun team -> (team.Captains |> List.contains(token.UserName))) |>
                List.map(fun team -> team.Name)

            let teamsMemberOf = 
                teams |> 
                List.filter(fun team -> (team.Players |> List.contains(token.UserName)) && 
                                                         not (team.Captains |> List.contains(token.UserName))) |>
                List.map(fun team -> team.Name)                                                         
    
            let! fixtures = getFixturesForTeam (List.append teamsCaptainOf teamsMemberOf)

            let details = 
                {
                    UserName = token.UserName                    
                    TeamsCaptainOf = teamsCaptainOf
                    TeamsMemberOf = teamsMemberOf
                    Fixtures = fixtures
                }
            return! Successful.OK (FableJson.toJson details) next ctx
        with exn ->
            let logger = ctx.GetLogger "TeamCaptainUserDetails"
            logger.LogError (EventId(), exn, "SERVICE_UNAVAILABLE")
            return! SERVICE_UNAVAILABLE "Database not available" next ctx
    })
