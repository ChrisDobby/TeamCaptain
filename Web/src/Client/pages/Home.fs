module Client.Home

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Messages
open Client.Style

let view (dispatch: AppMsg -> unit) =
    [
        div [ ClassName "container" ]
            [
            div [ ClassName "card blue-text text-darken-3 z-depth-2"]
                [
                    div [ClassName "card-content"]
                        [
                        p [] [
                            h3 [] [str "To register with a team, create a new team or update your details"]
                          ]
                        ]
                    div [ClassName "card-action"]
                        [
                            buttonLink "" (fun _ -> dispatch ShowLogin) [str "Login"]
                        ]
                ]
            ]
    ]
