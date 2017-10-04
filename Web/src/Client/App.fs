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

type Model =
  { Page : Page
    Menu : Menu.Model
    SubModel : SubModel }


/// The URL is turned into a Result.
let pageParser : Parser<Page->_,_> =
    oneOf
        [ map Home (s "home") ]

let urlUpdate (result:Page option) model =
    match result with
    | None ->
        Browser.console.error("Error parsing url: " + Browser.window.location.href)
        ( model, Navigation.modifyUrl (toHash model.Page) )

    | Some (Home as page) ->
        { model with
            Page = page
            Menu = { model.Menu with query = "" } }, Cmd.none

let init result =
    let menu,menuCmd = Menu.init()
    let m =
        { Page = Home
          Menu = menu
          SubModel = NoSubModel }

    let m,cmd = urlUpdate result m
    m,Cmd.batch[cmd; menuCmd]

let update msg model =
    model, []
//    match msg, model.SubModel with

// VIEW

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Style

/// Constructs the view for a page given the model and dispatcher.
let viewPage model dispatch =
    match model.Page with
    | Page.Home ->
        [ words 20 ("version " + ReleaseNotes.Version) ]

/// Constructs the view for the application given the model.
let view model dispatch =
  div []
    [ lazyView2 Menu.view model.Menu dispatch
      hr []
      div [ centerStyle "column" ] (viewPage model dispatch)
    ]

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
