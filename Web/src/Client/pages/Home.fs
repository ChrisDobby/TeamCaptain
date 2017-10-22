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
                [ 
                    navBar ReleaseNotes.Version
                    userDetails user (fun _ -> dispatch Logout)
                ]
        | _ -> 
                [
                    navBar ReleaseNotes.Version
                    buttonLink "" (fun _ -> dispatch ShowLogin) [str "Login"]
                ]