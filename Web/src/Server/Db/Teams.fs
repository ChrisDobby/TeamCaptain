module Server.Db.Teams

open System
open Server.Domain

let updateTeamInDb team = ()

let getTeamsFromDB =
    [|
        { 
            Name = "Cleckheaton 2nds" 
            Captains = [|"CleckCaptain"|] 
            Players = [||]
            Config = 
            {
                NumberOfPlayers = 11
                AvailabilityCheckDay = 2
                AvailabilityCheckTime = 14
                SelectionNotifyDay = 4
                SelectionNotifyTime = 9
            }
        }
    |]

let getTeamFromDB teamName = 
    let filteredTeam teams =
        match teams with
            | [||] -> None
            | _ -> Some(teams |> Array.exactlyOne)

    filteredTeam
        (getTeamsFromDB |> 
                Array.filter(fun team -> String.Equals(team.Name, teamName, StringComparison.OrdinalIgnoreCase)))
