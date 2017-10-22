module Client.Controls

open Fable.Helpers.React
open Fable.Helpers.React.Props

open Style

let navBar version user =     
    nav[ ClassName "navbar navbar-inverse navbar-fixed-top"]
        [ 
            div [ ClassName "container-fluid" ] [ div [ ClassName "navbar-header" ]
                [ div [ ClassName "navbar-brand" ]
                    [
                        words 16 ("Team captain " + version)
                    ]
                ]
            ]            
        ]

let userImage image = 
    img [ ClassName "img-circle"; Src image ]
