module Client.Home

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Messages
open Client.Style

let view (dispatch: AppMsg -> unit) = 
    [
        div [ ClassName "jumbotron" ]
            [
            h3 [] [str "To register with a team, create a new team or update your details"]
            buttonLink "btn btn-primary btn-lg" (fun _ -> dispatch ShowLogin) [str "Login"]
            ]
    ]
