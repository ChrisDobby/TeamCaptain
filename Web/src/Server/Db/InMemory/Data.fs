module Server.Db.InMemory.Data

open System
open Server.Domain

let mutable teams: Team list = []

let mutable registrations: Registration list = []

let mutable fixtures: Fixture list = []

let getTeams = teams

let getTeam teamName =
    let filteredTeam teams =
        match teams with
            | [] -> None
            | _ -> Some(teams |> List.exactlyOne)

    filteredTeam
        (teams |> 
                List.filter(fun team -> String.Equals(team.Name, teamName, StringComparison.OrdinalIgnoreCase)))

let saveRegistration registration = registrations <- registration::registrations

let updateTeam (team: Team) = 
    teams <- List.fold (fun acc (teamElement: Team) -> 
        if String.Equals(team.Name, teamElement.Name, StringComparison.OrdinalIgnoreCase) then
            team::acc
        else
            teamElement::acc) [] teams

let registerTeam teamRequest = 
    teams <- 
        { 
            Name = teamRequest.Name
            Config = teamRequest.Config
            Captains = [teamRequest.UserName]
            Players = [teamRequest.UserName]
        }
        :: teams

let fixturesForTeams teams =
    fixtures |> List.filter(fun fixture -> teams |> List.contains fixture.TeamName)