module Client.App

open Fable.Core
open Fable.Core.JsInterop
open Fable.Helpers.React.Props
open Fable.Import
open Elmish
open Elmish.React
open Fable.Import.Browser
open Elmish.Browser.Navigation
open Client.Messages
open Elmish.Browser.UrlParser
open Elmish.HMR
open System
open Client
open Client

importSideEffects "whatwg-fetch"
importSideEffects "babel-polyfill"

// Model

type SubModel =
  | NoSubModel
  | LoginModel of Login.Model
  | DashboardModel of Dashboard.Model
  
type Model =
  { Page : Page
    Header : Header.Model
    SubModel : SubModel }

/// The URL is turned into a Result.
let pageParser : Parser<Page->_,_> =
    oneOf
        [ 
            map Home (s "home") 
            map Login (s "login")
            map Page.LoggedOut (s "loggedout")
            map TokenCallback (Auth0Lock.auth0lockParser () </> str)
        ]

let urlUpdate (result:Page option) model =
    let homeOrDashboard header =
        match header with
            | Header.None ->
                { model with
                    Page = Home
                }
            | Header.User(_) ->
                { model with 
                    Page = Dashboard
                }

    match result with
        | None ->
            console.error("Error parsing url: " + location.href)
            ( model, Navigation.modifyUrl (toHash model.Page) )

        | Some (Home) -> homeOrDashboard model.Header, Cmd.none
        | Some (Login as page) ->
            { model with
                Page = page
            }, Cmd.none
        | Some (TokenCallback(token)) ->
            { model with
                Page = Login
                SubModel = LoginModel (Login.TokenValidation (Auth0Lock.auth0CallbackParser token))
            }, Cmd.none
        | Some (Dashboard as page) -> homeOrDashboard model.Header, Cmd.none
        | Some (Page.Logout as page) ->
            { model with
                Page = page
            }, Cmd.none
        | Some (Page.LoggedOut as page) ->
            { model with 
                Page = page
            }, Cmd.none

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

    m,Cmd.batch[cmd]

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
            }, []
        | _, LoginModel(_) -> model, []
        | _, NoSubModel -> model, []

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

/// Constructs the view for the application given the model.
let view model dispatch =
  div [ ClassName "container" ]
    [ lazyView2 Header.view model.Header dispatch
      div [] (viewPage model dispatch)
    ]

//  viewPage model dispatch

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
