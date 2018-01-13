module Client.JoinTeam

open Elmish
open Fable.Helpers.React
open Client.Messages

type Model = {
    Token: string
}

let init (user: UserProfile) =
    { Token = user.BearerToken }, Cmd.none

let view model (dispatch: AppMsg -> unit) = 
    []