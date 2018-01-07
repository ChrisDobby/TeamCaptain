module Client.Logout

open Messages
open Elmish
open Fable.Import.Browser

type Model =
    | LoggedOut
    | None

let init model =
    let command model =
        match model with 
            | LoggedOut -> Cmd.ofFunc Utils.delete "teamcaptain.user" (fun _ -> AppMsg.LogoutComplete) AppMsg.StorageFailure
            | None -> Cmd.none

    model, (command model)

let view model (dispatch: AppMsg -> unit) = 
    match model with
        | LoggedOut -> dispatch AppMsg.LoggedOut
        | None -> window.location.href <- "https://chrisdobby.eu.auth0.com/v2/logout?client_id=1MD87NRRLv6doHbNL7FDUMckd0npGshr&returnTo=http%3A%2F%2Flocalhost:8080/#loggedout"
    []
