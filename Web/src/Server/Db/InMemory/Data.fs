module Server.Db.InMemory.Data

open System
open Server.Domain

let mutable teams: Team list = []

let mutable registrations: Registration list = []

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

let updateTeam team = 
    teams <- List.fold (fun acc teamElement -> 
        if String.Equals(team.Name, teamElement.Name, StringComparison.OrdinalIgnoreCase) then
            team::acc
        else
            teamElement::acc) [] teams
