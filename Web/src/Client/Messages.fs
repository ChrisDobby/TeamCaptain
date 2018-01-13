module Client.Messages

open System

open Server.Domain

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
  | Load

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
  | ShowCreateTeam
  | ShowJoinTeam
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
 