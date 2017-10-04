module Client.Menu

open Fable.Core
open Fable.Import
open Elmish
open Fable.Import.Browser
open Fable.PowerPack
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Client.Style
open Client.Messages
open System
open Fable.Core.JsInterop

type Model = {
    User : UserData option
    query : string
}

let init() = { User = Utils.load "user"; query = "" },Cmd.none

let view (model:Model) dispatch =
    div [ centerStyle "row" ] [ 
          yield viewLink Home "Home"
        ]