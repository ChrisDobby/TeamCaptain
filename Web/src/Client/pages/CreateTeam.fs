module Client.CreateTeam

open Elmish
open Fable.Helpers.React
open Client.Messages

type Model = {
    Token: string
}

let init (user: UserProfile) =
    { Token = user.BearerToken }, Cmd.ofMsg Load

let view model (dispatch: AppMsg -> unit) = 
    []