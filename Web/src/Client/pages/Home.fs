module Client.Home

open Fable.Core
open Fable.Import.Browser
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack

open Messages
open Style
open Client.Auth0Lock

let lock = auth0lock.Create("1MD87NRRLv6doHbNL7FDUMckd0npGshr", "chrisdobby.eu.auth0.com", myConfig)

type Model = 
    | Login 
    | TokenValidation of string
    | User of UserProfile

let view model (dispatch: AppMsg -> unit) = 
    match model with 
        | TokenValidation token -> 
            promise {
                    let! authResult = lock.resumeAuth token |> promisify
                    printfn "[Auth] Token received: %O" authResult
                    let! profile = lock.getUserInfo authResult.accessToken |> promisify
            
                    printfn "[Auth] userProfile loaded %O" profile

                    toUserProfile authResult profile 
                    |> AppMsg.ProfileLoaded
                    |> dispatch
            } |> Promise.start
        | Model.Login -> 
            lock.show()
        | _ -> ()

    nav[ ClassName "navbar navbar-inverse navbar-fixed-top"] 
        [ 
            div [ ClassName "container-fluid"]                
                [
                    div [ClassName "navbar-header"]
                        [
                            div [ClassName "navbar-brand"]
                                [
                                    words 16 ("Team captain " + ReleaseNotes.Version)
                                ]
                        ]
                ]            
        ]
