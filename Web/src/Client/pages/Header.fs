module Client.Header

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Messages
open Style

type Model = 
    | User of UserProfile
    | None

let view model (dispatch: AppMsg -> unit) = 
    let navBar =
        div [ ClassName "navbar-fixed"]
            [
            nav [ ClassName "purple darken-4"]
                [
                div [ ClassName "nav-wrapper"] 
                    [
                    div [ ClassName "brand-logo" ]
                        [
                            span [] [str ("Team captain " + ReleaseNotes.Version)]
                        ]
                    ]
                ]
            ]

    let userDetails user = 
        div [ ClassName "user-details" ]
            [
            div []
                [
                words 12 user.Name
                ]
            div []
                [
                img [ ClassName "img-circle"; Src user.Picture ]
                ]
            div []
                [
                   buttonLink "" (fun _ -> dispatch Logout) [str "Logout"]
                ]
            ]

    match model with 
        | User(user) -> 
            div []
                [ 
                    navBar
                    userDetails user 
                ]                
        | _ -> 
            div []
                [
                    navBar 
                ]