module Client.Home

open Fable.Core
open Fable.Import.Browser
open Fable.Helpers.React
open Fable.Helpers.React.Props

open Messages
open Style

let view (dispatch: AppMsg -> unit) = 
    div[] [ words 20 ("Version " + ReleaseNotes.Version) ]