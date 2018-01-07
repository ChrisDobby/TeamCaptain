module Client.Dashboard

open Fable.PowerPack

open Messages
open Elmish
open Fable.PowerPack.Fetch
open Server.Domain

type Model = {
    Token: string
}

let getUserDetails token =
    promise {        
        let url = "api/userDetails/"
        let props = 
            [ Fetch.requestHeaders [
                HttpRequestHeaders.Authorization ("Bearer " + token) ]]

        return! Fable.PowerPack.Fetch.fetchAs<UserDetails> url props
    }

let loadDashboardCmd token = 
    Cmd.ofPromise getUserDetails token FetchedUserDetails FetchError

let init (user: UserProfile) = 
    {
        Token = user.BearerToken
    }, 
    loadDashboardCmd user.BearerToken

let view (dispatch: AppMsg -> unit) = 
    []
