module Client.App

open Fable.Core.JsInterop
open Fable.Helpers.React.Props
open Elmish
open Elmish.React
open Fable.Import.Browser
open Elmish.Browser.Navigation
open Client.Messages
open Elmish.Browser.UrlParser
open Elmish.HMR
open Client

importSideEffects "whatwg-fetch"
importSideEffects "babel-polyfill"

// Model

type SubModel =
  | NoSubModel
  | LoginModel of Login.Model
  | DashboardModel of Dashboard.Model
  | CreateTeamModel of CreateTeam.Model
  | JoinTeamModel of JoinTeam.Model
  
type Model =
  { Page : Page
    Header : Header.Model
    SubModel : SubModel }

/// The URL is turned into a Result.
let pageParser : Parser<Page->_,_> =
    oneOf
        [ 
            map Home (s "Home") 
            map Login (s "login")
            map Page.LoggedOut (s "loggedout")
            map Dashboard (s "dashboard")
            map TokenCallback (Auth0Lock.auth0lockParser () </> str)
            map CreateTeam (s "createteam")
            map JoinTeam (s "jointeam")
        ]

let urlUpdate (result:Page option) model =
    let authorisedPage model page initView = 
        match model.Header with
            | Header.None ->
                { model with
                    Page = Home
                    SubModel = SubModel.NoSubModel
                }, Cmd.none
            | Header.User(user) ->
                let m, cmd = initView user
                { model with
                    Page = page
                    SubModel = m
                }, Cmd.batch[cmd]

    let pageInit init user msgType modelType = 
        let m, cmd = init user
        (modelType m), Cmd.map msgType cmd

    match result with
        | None ->
            console.error("Error parsing url: " + location.href)
            authorisedPage model Dashboard (fun user -> pageInit Dashboard.init user DashboardMsg DashboardModel)
        | Some (Home) -> 
            authorisedPage model Dashboard (fun user -> pageInit Dashboard.init user DashboardMsg DashboardModel)
        | Some (Login as page) ->
            { model with
                Page = page
            }, Cmd.none
        | Some (TokenCallback(token)) ->
            { model with
                Page = Login
                SubModel = LoginModel (Login.TokenValidation (Auth0Lock.auth0CallbackParser token))
            }, Cmd.none
        | Some (Dashboard as page) -> 
            authorisedPage model page (fun user -> pageInit Dashboard.init user DashboardMsg DashboardModel)
        | Some (Page.Logout as page) ->
            { model with
                Page = page
            }, Cmd.none
        | Some (Page.LoggedOut as page) ->
            { model with 
                Page = page
            }, Cmd.none
        | Some (CreateTeam as page) -> 
            authorisedPage model page (fun user -> pageInit CreateTeam.init user CreateTeamMsg CreateTeamModel)
        | Some (JoinTeam as page) -> 
            authorisedPage model page (fun user -> pageInit JoinTeam.init user JoinTeamMsg JoinTeamModel)

let init result =
    let headerModel =
        match Utils.load "teamcaptain.user" with
            | None -> Header.None
            | Some(user) -> if Auth0Lock.isExpiryValid user then
                                Header.User user
                            else
                                Header.None
         
    let m,cmd = urlUpdate result (match headerModel with
                                    | Header.None -> { Page = Home; Header = headerModel; SubModel = NoSubModel }
                                    | Header.User(_) -> { Page = Dashboard; Header = headerModel; SubModel = NoSubModel })

    m, Cmd.batch[cmd]

let update msg model =
    match msg, model.SubModel with
        | ProfileLoaded(profile), _ -> 
            printfn "Profiled loaded: %O" profile
            let saveCmd = Cmd.ofFunc (Utils.save "teamcaptain.user") profile (fun _ -> LoggedIn) StorageFailure
            let m, cmd = Dashboard.init(profile)
            let cmd = Cmd.map DashboardMsg cmd
            { model with
                Page = Dashboard
                Header = Header.User profile
                SubModel = DashboardModel m
            }, Cmd.batch[saveCmd; cmd]
        | StorageFailure e, _ ->
            printfn "Unable to access local storage: %A" e
            model, []
        | LoggedIn, _ -> model, []
        | ShowLogin, _ -> 
            { model with
                Page = Login
            }, []
        | Logout, _ -> 
            let _, cmd = Logout.init Logout.None
            { model with
                Page = Page.Logout
                Header = Header.None
                SubModel = NoSubModel
            }, Cmd.batch[cmd]
        | LoggedOut, _ ->
            let _, cmd = Logout.init Logout.LoggedOut 
            model, Cmd.batch[cmd]
        | DashboardMsg(dashboardMsg), DashboardModel dashboardModel -> 
            let m, cmd = Dashboard.update dashboardMsg dashboardModel
            { model with
                SubModel = DashboardModel m
            }, cmd
        | LogoutComplete, _ ->
            { model with
                Page = Home
                Header = Header.None
                SubModel = NoSubModel
            }, Cmd.batch[Navigation.modifyUrl (toHash Home)]
        | CreateTeamMsg(createTeamMsg), CreateTeamModel createTeamModel ->
            let m, cmd = CreateTeam.update createTeamMsg createTeamModel
            { model with
                SubModel = CreateTeamModel m 
            }, cmd
        | _, _ -> model, []

// VIEW

open Fable.Helpers.React

/// Constructs the view for a page given the model and dispatcher.
let viewPage model dispatch =
    match model.Page with
        | Home -> Home.view dispatch
        | TokenCallback(_) -> []
        | Login -> 
            match model.SubModel with
                | LoginModel m -> Login.view m dispatch
                | _ -> Login.view Login.None dispatch
        | Page.Logout -> Logout.view Logout.None dispatch
        | Page.LoggedOut -> Logout.view Logout.LoggedOut dispatch
        | Dashboard -> 
            match model.SubModel with
                | DashboardModel m -> Dashboard.view (Some m) dispatch
                | _ -> Dashboard.view None dispatch        
        | CreateTeam -> 
            match model.SubModel with
                | CreateTeamModel m -> CreateTeam.view m dispatch
                | _ -> []
        | JoinTeam -> 
            match model.SubModel with
                | JoinTeamModel m -> JoinTeam.view (Some m) dispatch
                | _ -> JoinTeam.view None dispatch        

/// Constructs the view for the application given the model.
let view model dispatch =
  div [ClassName "flex-container"]
    [ 
    main [] 
        [
            lazyView2 Header.view model.Header dispatch
            div [ClassName "main-content"] (viewPage model dispatch)
        ]
    Footer.view
    ]

open Elmish.Debug

// App
Program.mkProgram init update view
|> Program.toNavigable (parseHash pageParser) urlUpdate
#if DEBUG
|> Program.withConsoleTrace
|> Program.withDebugger
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
|> Program.run
