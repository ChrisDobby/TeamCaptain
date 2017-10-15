module Client.App

open Fable.Core
open Fable.Core.JsInterop

open Fable.Core
open Fable.Import
open Elmish
open Elmish.React
open Fable.Import.Browser
open Fable.PowerPack
open Elmish.Browser.Navigation
open Client.Messages
open Elmish.Browser.UrlParser
open Elmish.HMR

JsInterop.importSideEffects "whatwg-fetch"
JsInterop.importSideEffects "babel-polyfill"

// Model

type SubModel =
  | NoSubModel
  | HomeModel of Home.Model

type Model =
  { Page : Page
    SubModel : SubModel }

/// The URL is turned into a Result.
let pageParser : Parser<Page->_,_> =
    oneOf
        [ 
            map Home (s "home") 
            map TokenCallback (Auth0Lock.auth0lockParser "access_token" </> str)
        ]

let urlUpdate (result:Page option) model =
    match result with
        | None ->
            Browser.console.error("Error parsing url: " + Browser.window.location.href)
            ( model, Navigation.modifyUrl (toHash model.Page) )

        | Some (Home as page) ->
            { model with
                Page = page
            }, Cmd.none
        | Some (TokenCallback(token) as page) ->
            { model with
                Page = Home
                SubModel = SubModel.HomeModel (Home.TokenValidation token)
            }, Cmd.none

let init result =
    let m =
        { Page = Home
          SubModel = HomeModel Home.Login }

    let m,cmd = urlUpdate result m
    m,Cmd.batch[cmd]

let update msg model =
    match msg, model.SubModel with
        | AppMsg.ProfileLoaded(profile), _ -> 
            printfn "Profiled loaded: %O" profile
            let cmd = Cmd.ofFunc (Utils.save "teamcaptain.user") profile (fun _ -> LoggedIn) StorageFailure
            { model with
                Page = Page.Home
                SubModel = SubModel.NoSubModel
            }, Cmd.batch[cmd]
        | StorageFailure e, _ ->
            printfn "Unable to access local storage: %A" e
            model, []
        | LoggedIn, _ -> model, []

// VIEW

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Style

/// Constructs the view for a page given the model and dispatcher.
let viewPage model dispatch =
    match model.Page with
        | Page.Home ->
            match model.SubModel with
                | HomeModel m -> [ div [] [ Home.view m dispatch ]]
                | _ -> [ div [] [ Home.view Home.Login dispatch ]]
        | Page.TokenCallback(token) -> [div[] []]

/// Constructs the view for the application given the model.
let view model dispatch =
  div [] (viewPage model dispatch)

open Elmish.React
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
