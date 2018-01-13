module Client.DashboardDetail

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Style
open Server.Domain
open Messages

let private registerView dispatch =
    div [ ClassName "row"] 
        [
            div [ClassName "col s12 m6 offset-m3"]      
                [
                div [ClassName "card blue-text text-darken-3 z-depth-2"]
                    [
                    div [ClassName "card-content"]
                        [
                        span [ClassName "card-title"] [str "Team registration"]
                        p [] 
                            [
                                str "You are not registered with any teams.  You can create a new team with you as captain or request to join an existing team using the links below."
                            ]
                        ]
                    div [ClassName "card-action"]
                        [
                            viewLink CreateTeam "Create team"
                            viewLink JoinTeam "Join team"
                        ]
                    ]
                ]
        ]

let view details dispatch = 
    match details.TeamsCaptainOf, details.TeamsMemberOf with
        | [], [] -> [registerView dispatch]
        | _ -> []
