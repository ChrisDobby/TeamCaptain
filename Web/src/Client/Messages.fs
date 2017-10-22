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

type AppMsg = 
  | ShowLogin
  | ProfileLoaded of UserProfile
  | StorageFailure of exn
  | LoggedIn

let toHash = function
  | Home -> "#home"
  | TokenCallback(_) -> "#home"
  | Login -> "#login"
 