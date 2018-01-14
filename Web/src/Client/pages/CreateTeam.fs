module Client.CreateTeam

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Messages

type Model = {
    User: UserProfile
}

let dayOptions =
    [
    option [Value ""; HTMLAttr.Disabled true; HTMLAttr.Selected true] 
        [ str "Select a day" ]
    option [Value "monday"]
        [ str "Monday"]
    option [Value "tuesday"]
        [ str "Tuesday"]
    option [Value "wednesday"]
        [ str "Wednesday"]
    option [Value "thursday"]
        [ str "Thursday "]
    option [Value "friday"]
        [ str "Friday"]
    option [Value "saturday"]
        [ str "Saturday"]
    option [Value "sunday"]
        [ str "Sunday"]
    ] 

let init user =
    { User = user }, Cmd.ofMsg Load

let view model (dispatch: AppMsg -> unit) = 
    [
    div [ClassName "row"]
        [
        form [ClassName "col s12 m6 offset-m3"]
            [
            nav [ClassName "top-nav blue lighten-3"; Style [MarginBottom "5px"]]
                [
                div [ClassName "container"]
                    [
                    div [ClassName "nav-wrapper"]
                        [
                        span [ClassName "page-title"] [str "Create team"]
                        ]
                    ]
                ]
            div [ClassName "row"]
                [
                div [ClassName "input-field col s12"]
                    [
                    input [Id "team_name"; Type "text"; ClassName "validate"] 
                    label [HtmlFor "team_name"] [str "Team name"]
                    ]
                ]
            div [ClassName "row"]
                [
                div [ClassName "input-field col s12"]
                    [
                    input [Id "number_players"; Type "number"; ClassName "validate"; DefaultValue "11"] 
                    label [HtmlFor "number_players"; ClassName "active"] [str "Number of players"]
                    ]
                ]
            div [ClassName "row"]
                [
                div [ClassName "input-field col s6"]
                    [
                    select [] dayOptions
                    label [] [str "Availability day"]
                    ]
                ]
            div [ClassName "row"]
                [
                div [ClassName "input-field col s6"]
                    [
                    select [] dayOptions
                    label [] [str "Selection day"]
                    ]
                ]
            ]
        ]
    ]