module Client.Controls

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Messages
open Style

let navBar version =     
    nav[ ClassName "navbar navbar-inverse navbar-fixed-top"]
        [ 
            div [ ClassName "container-fluid" ] 
                [ 
                div [ ClassName "navbar-header" ]
                    [ 
                    div [ ClassName "navbar-brand" ]
                        [
                            words 16 ("Team captain " + version)
                        ]
                    ]
                ]
        ]

let userDetails user logout = 
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
               buttonLink "" logout [str "Logout"]
            ]
        ]
