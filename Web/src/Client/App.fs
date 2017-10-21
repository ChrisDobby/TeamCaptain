module Client.App

open Fable.Core
open Fable.Core.JsInterop

open Fable.Import
open Elmish
open Elmish.React
open Fable.Import.Browser
open Elmish.Browser.Navigation
open Client.Messages
open Elmish.Browser.UrlParser
open Elmish.HMR

importSideEffects "whatwg-fetch"
importSideEffects "babel-polyfill"

// Model

type SubModel =
  | NoSubModel
  | HomeModel of Home.Model
  | LoginModel of Login.Model

type Model =
  { Page : Page
    SubModel : SubModel }

/// The URL is turned into a Result.
let pageParser : Parser<Page->_,_> =
    oneOf
        [ 
            map Home (s "home") 
            map Login (s "login")
            map TokenCallback (Auth0Lock.auth0lockParser () </> str)
        ]

let urlUpdate (result:Page option) model =
    match result with
        | None ->
            console.error("Error parsing url: " + location.href)
            ( model, Navigation.modifyUrl (toHash model.Page) )

        | Some (Home as page) ->
            { model with
                Page = page
            }, Cmd.none
        | Some (Login as page) ->
            { model with
                Page = page
            }, Cmd.none
        | Some (TokenCallback(token)) ->
            { model with
                Page = Login
                SubModel = LoginModel (Login.TokenValidation (Auth0Lock.auth0CallbackParser token))
            }, Cmd.none

let init result =
    let m =
        { Page = Home
          SubModel = HomeModel Home.None }

    let m,cmd = urlUpdate result m
    m,Cmd.batch[cmd]

let update msg model =
    match msg, model.SubModel with
        | ProfileLoaded(profile), _ -> 
            printfn "Profiled loaded: %O" profile
            let cmd = Cmd.ofFunc (Utils.save "teamcaptain.user") profile (fun _ -> LoggedIn) StorageFailure
            { model with
                Page = Home
                SubModel = HomeModel (Home.User (profile))
            }, Cmd.batch[cmd]
        | StorageFailure e, _ ->
            printfn "Unable to access local storage: %A" e
            model, []
        | LoggedIn, _ -> model, []
        | ShowLogin, _ -> 
            { model with
                Page = Login
            }, []

// VIEW

open Fable.Helpers.React

/// Constructs the view for a page given the model and dispatcher.
let viewPage model dispatch =
    match model.Page with
        | Home ->
            match model.SubModel with
                | HomeModel m -> div [] (Home.view m dispatch)
                | _ -> div [] (Home.view Home.None dispatch)
        | TokenCallback(_) -> div [] []
        | Login -> 
            match model.SubModel with
                | LoginModel m -> div [] (Login.view m dispatch)
                | _ -> div [] (Login.view Login.None dispatch)

/// Constructs the view for the application given the model.
let view model dispatch =
  viewPage model dispatch

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
