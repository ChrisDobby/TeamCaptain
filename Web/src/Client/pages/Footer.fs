module Client.Footer

open Fable.Helpers.React
open Fable.Helpers.React.Props

let view = 
    footer [ClassName "page-footer grey lighten-1"] 
        [
        div [ClassName "container"]
            [
                span [] [str ("version " + ReleaseNotes.Version)]
            ]
        ]