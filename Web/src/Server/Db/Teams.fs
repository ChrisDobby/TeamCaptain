module Server.Db.Teams

open System
open Server.Domain

let updateTeam team = ()

let getTeams =
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

let getTeam teamName = 
    let filteredTeam teams =
        match teams with
            | [||] -> None
            | _ -> Some(teams |> Array.exactlyOne)

    filteredTeam
        (getTeams |> 
                Array.filter(fun team -> String.Equals(team.Name, teamName, StringComparison.OrdinalIgnoreCase)))
