module Client.Dashboard

open Fable.PowerPack

open Messages
open Elmish
open Fable.PowerPack.Fetch
open Server.Domain
open Style
open Fable.Helpers.React
open Fable.Helpers.React.Props

type Model = {
    Token: string
    Details: UserDetails option
    LoadError: bool
}

let private getUserDetails token =
    promise {        
        let url = "api/userDetails/"
        let props = 
            [ Fetch.requestHeaders [
                HttpRequestHeaders.Authorization ("Bearer " + token) ]]

        return! Fable.PowerPack.Fetch.fetchAs<UserDetails> url props
    }

let private loadDashboardCmd token = 
    Cmd.ofPromise getUserDetails token FetchedUserDetails FetchError


let private errorView () = [div [ClassName "alert alert-danger"] [
                                words 16 "Details for the current user could not be loaded.  Refresh to try again"
                          ]]

let init (user: UserProfile) = 
    {
        Token = user.BearerToken
        Details = None
        LoadError = false
    }, 
    loadDashboardCmd user.BearerToken

let update msg model = 
    match msg with
        | FetchedUserDetails(details) ->
            { model with
                Details = Some details
                LoadError = false
            }, Cmd.none
        | FetchError(_) -> 
            { model with
                LoadError = true
                Details = None
            }, Cmd.none

let view model (dispatch: AppMsg -> unit) = 
    match model with
        | Some m ->
            match m.Details with
                | Some details -> DashboardDetail.view details
                | None -> if m.LoadError then errorView() else []
        | None -> []
