module Client.Login

open Fable.PowerPack

open Client.Auth0Lock
open Client.Controls
open Messages

let lock = auth0lock.Create("1MD87NRRLv6doHbNL7FDUMckd0npGshr", "chrisdobby.eu.auth0.com", myConfig)

type Model = 
    | TokenValidation of string * int * string
    | None

let view model (dispatch: AppMsg -> unit) = 
    match model with 
        | TokenValidation (accessToken, expiry, bearerToken) -> 
            promise {
                    let! profile = lock.getUserInfo accessToken |> promisify            
                    printfn "[Auth] userProfile loaded %O" profile

                    toUserProfile accessToken bearerToken expiry profile 
                    |> ProfileLoaded
                    |> dispatch
            } |> Promise.start
        | None -> lock.show()

    [ navBar ReleaseNotes.Version None ]