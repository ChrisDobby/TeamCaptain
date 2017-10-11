module Client.Login

open Fable.Core
open Fable.Import.Browser
open Fable.Helpers.React
open Fable.Helpers.React.Props

open Messages
open Client.Auth0Lock


open Fable.PowerPack

//TODO: move client id and domain to config via webpack
let lock = auth0lock.Create("1MD87NRRLv6doHbNL7FDUMckd0npGshr", "chrisdobby.eu.auth0.com", myConfig)

type Model = 
| Login 
| TokenValidation of string

let isAccessToken() = 
    let hash = window.location.hash
    match hash.StartsWith "#access_token" with
    | true  -> Some hash
    | false -> None

//hide this once all ok
let view (model:Model) (dispatch: AppMsg -> unit) = 
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
        div [Id "loginAuth view"] [str "Auth0 token validation page"] 
    | Model.Login ->
        lock.show()
        div [] [str "Auth 0 stub"]