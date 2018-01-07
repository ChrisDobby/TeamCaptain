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

type DashboardMsg =
  | FetchedUserDetails of UserDetails
  | FetchError of exn

type AppMsg = 
  | ShowLogin
  | Logout
  | ProfileLoaded of UserProfile
  | StorageFailure of exn
  | LoggedIn
  | LoggedOut
  | LogoutComplete
  | DashboardMsg of DashboardMsg


let toHash = function
  | Home -> "#home"
  | TokenCallback(_) -> "#home"
  | Login -> "#login"
  | Page.Logout -> "#logout"
  | Dashboard -> "#dashboard"
  | Page.LoggedOut -> "#loggedout"
 