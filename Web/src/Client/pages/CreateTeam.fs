module Client.CreateTeam

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Import
open Client.Messages
open Client.Materialize
open Client.Style
open Server.Domain
open Fable.Core.JsInterop
open Client

type Model = {
    User: UserProfile
    NewTeam: Team
}

let optionToDay = function
    | "monday" -> Day.Monday
    | "tuesday" -> Day.Tuesday
    | "wednesday" -> Day.Wednesday
    | "thursday" -> Day.Thursday
    | "friday" -> Day.Friday
    | "saturday" -> Day.Saturday
    | "sunday" -> Day.Sunday
    | _ -> failwith "Unknown day"

let dayOptions =
    [
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

let initialiseComponents (components) =
    let selectChangeHandler id changeHandler = 
        fun () -> let elm = Fable.Import.Browser.document.getElementById id :?> Browser.HTMLSelectElement
                  changeHandler elm
    
    let initialiseSelect id changeHandler =
        InitialiseSelect (selectChangeHandler id changeHandler) ("#" + id)

    components |> Seq.iter (fun comp -> let id, changeHandler = comp
                                        initialiseSelect id changeHandler)

let addChangedHandlers (components) =
    let changeHandler id handler =
        fun () -> let elm = Fable.Import.Browser.document.getElementById id :?> Browser.HTMLInputElement
                  handler elm

    let addChangeHandler id handler =
        AddChangeHandler (changeHandler id handler) ("#" + id)

    components |> Seq.iter (fun comp -> let id, handler = comp
                                        addChangeHandler id handler)

let init user =
    InitialiseTimePickers ()

    { 
        User = user
        NewTeam = Team.New user.UserId 11
    }, Cmd.none

let update msg model =
    match msg with
        | TeamNameChanged(teamName) -> 
            let newTeam = { model.NewTeam with Name = teamName }
            { model with NewTeam = newTeam }, Cmd.none
        | NumberOfPlayersChanged(number) -> 
            let newTeam = { model.NewTeam with 
                                Config = { model.NewTeam.Config with NumberOfPlayers = number } }
            { model with NewTeam = newTeam }, Cmd.none
        | AvailabilityCheckDayChanged(day) ->
            let newTeam = { model.NewTeam with 
                                Config = { model.NewTeam.Config with AvailabilityCheckDay = optionToDay day } }
            { model with NewTeam = newTeam }, Cmd.none
        | AvailabilityCheckTimeChanged(time) ->
            let newTeam = { model.NewTeam with 
                                Config = { model.NewTeam.Config with AvailabilityCheckTime = time } }
            { model with NewTeam = newTeam }, Cmd.none
        | SelectionNotifyDayChanged(day) ->
            let newTeam = { model.NewTeam with 
                                Config = { model.NewTeam.Config with SelectionNotifyDay = optionToDay day } }
            { model with NewTeam = newTeam }, Cmd.none
        | SelectionNotifyTimeChanged(time) ->
            let newTeam = { model.NewTeam with 
                                Config = { model.NewTeam.Config with SelectionNotifyTime = time } }
            { model with NewTeam = newTeam }, Cmd.none

let view model (dispatch: AppMsg -> unit) = 
    initialiseComponents
        [
        ("availability_day", fun (elm: Browser.HTMLSelectElement) -> dispatch(CreateTeamMsg (CreateTeamMsg.AvailabilityCheckDayChanged elm.value)))
        ("selection_day", fun (elm: Browser.HTMLSelectElement) -> dispatch(CreateTeamMsg (CreateTeamMsg.SelectionNotifyDayChanged elm.value)))
        ]

    addChangedHandlers
        [
        ("availability_time", fun (elm: Browser.HTMLInputElement) -> dispatch(CreateTeamMsg (CreateTeamMsg.AvailabilityCheckTimeChanged elm.value)))
        ("selection_time", fun (elm: Browser.HTMLInputElement) -> dispatch(CreateTeamMsg (CreateTeamMsg.SelectionNotifyTimeChanged elm.value)))
        ]

    [
    div [ClassName "row"]
        [
        div [ClassName "col s12 m6 offset-m3"] 
            [
            nav [ClassName "navbar-fixed blue lighten-3"; Style [MarginBottom "5px"]]
                [
                div [ClassName "container"]
                    [
                    div [ClassName "nav-wrapper"]
                        [
                        span [ClassName "page-title"] [str "Create team"]
                        ul [ClassName "right"] 
                            [
                            li [] [floatingButton (fun _ -> () |> ignore) "add"]
                            ]
                        ]
                    ]
                ]
            form []
                [
                div [ClassName "row"]
                    [
                    div [ClassName "input-field col s12"]
                        [
                        input [
                                Id "team_name"
                                Type "text"
                                ClassName "validate"
                                OnChange (fun (ev:React.FormEvent) -> dispatch (CreateTeamMsg (CreateTeamMsg.TeamNameChanged !!ev.target?value)))] 
                        label [HtmlFor "team_name"] [str "Team name"]
                        ]
                    ]
                div [ClassName "row"]
                    [
                    div [ClassName "input-field col s12"]
                        [
                        input [
                            Id "number_players"
                            Type "number"
                            ClassName "validate"
                            DefaultValue "11"
                            OnChange (fun (ev:React.FormEvent) -> dispatch (CreateTeamMsg (CreateTeamMsg.NumberOfPlayersChanged !!ev.target?value)))] 
                        label [HtmlFor "number_players"; ClassName "active"] [str "Number of players"]
                        ]
                    ]
                div [ClassName "row"]
                    [
                    div [ClassName "input-field col s6"]
                        [
                        select [
                            Id "availability_day"
                            DefaultValue "monday"
                            OnChange (fun (ev:React.FormEvent) -> dispatch (CreateTeamMsg (CreateTeamMsg.AvailabilityCheckDayChanged !!ev.target?value)))] dayOptions
                        label [] [str "Availability day"]
                        ]
                    div [ClassName "input-field col s6"]
                        [
                        input [
                            Id "availability_time"
                            Type "text"
                            ClassName "timepicker"
                            OnChange (fun (ev:React.FormEvent) -> dispatch (CreateTeamMsg (CreateTeamMsg.AvailabilityCheckTimeChanged !!ev.target?value)))]
                        label [HtmlFor "availability_time"; ClassName "active"] [str "Availability time"]
                        ]
                    ]
                div [ClassName "row"]
                    [
                    div [ClassName "input-field col s6"]
                        [
                        select [
                            Id "selection_day"
                            DefaultValue "monday"
                            OnChange (fun (ev:React.FormEvent) -> dispatch (CreateTeamMsg (CreateTeamMsg.SelectionNotifyDayChanged !!ev.target?value)))] dayOptions
                        label [] [str "Selection day"]
                        ]
                    div [ClassName "input-field col s6"]
                        [
                        input [
                            Id "selection_time"
                            Type "text"
                            ClassName "timepicker"
                            DefaultValue "09:00"
                            OnChange (fun (ev:React.FormEvent) -> dispatch (CreateTeamMsg (CreateTeamMsg.SelectionNotifyTimeChanged !!ev.target?value)))]
                        label [HtmlFor "selection_time"; ClassName "active"] [str "Selection time"]
                        ]
                    ]
                ]
            ]
        ]
    ]