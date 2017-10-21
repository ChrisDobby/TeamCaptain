module Client.Home

open Fable.Helpers.React

open Messages
open Client.Controls
open Client.Style

type Model = 
    | User of UserProfile
    | None

let view model (dispatch: AppMsg -> unit) = 
    match model with 
        | User(user) -> 
                [ navBar ReleaseNotes.Version user ]
        | _ -> 
                [
                    navBar ReleaseNotes.Version None 
                    buttonLink "" (fun _ -> dispatch ShowLogin) [str "Login"]
                ]