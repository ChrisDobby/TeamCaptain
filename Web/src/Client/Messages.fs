module Client.Messages

open System

open Server.Domain
open Fable.PowerPack.Fetch.Fetch_types

type UserProfile = 
    {
        AccessToken   : JWT
        BearerToken   : JWT
        Name          : string
        Picture       : string
        UserId        : string
        Expiry        : DateTime
    }

/// The different pages of the application. If you add a new page, then add an entry here.
type Page = 
  | Home
  | TokenCallback of token: string
  | Login
  | Logout
  | LoggedOut
  | Dashboard
  | CreateTeam
  | JoinTeam

type DashboardMsg =
  | FetchedUserDetails of UserDetails
  | FetchError of exn

type CreateTeamMsg =
  | TeamNameChanged of string
  | NumberOfPlayersChanged of int
  | AvailabilityCheckDayChanged of string
  | AvailabilityCheckTimeChanged of string
  | SelectionNotifyDayChanged of string
  | SelectionNotifyTimeChanged of string
  | SaveTeam
  | SaveSuccess of Response
  | SaveError of exn

type JoinTeamMsg = 
  | FetchedTeams of Team seq

type AppMsg = 
  | ShowLogin
  | Logout
  | ProfileLoaded of UserProfile
  | StorageFailure of exn
  | LoggedIn
  | LoggedOut
  | LogoutComplete
  | DashboardMsg of DashboardMsg
  | CreateTeamMsg of CreateTeamMsg
  | JoinTeamMsg of JoinTeamMsg

let toHash = function
  | Home -> "#home"
  | TokenCallback(_) -> "#home"
  | Login -> "#login"
  | Page.Logout -> "#logout"
  | Dashboard -> "#dashboard"
  | Page.LoggedOut -> "#loggedout"
  | CreateTeam -> "#createteam"
  | JoinTeam -> "#jointeam"
 