module Client.Messages

open System
open Server.Domain

type UserProfile = 
    {
        AccessToken: JWT
        Name       : string
        Email      : string
        Picture    : string
        UserId     : string
    }

/// The different pages of the application. If you add a new page, then add an entry here.
type Page = 
  | Home 

type AppMsg = 
  | StorageFailure of exn
  | ShowLogin
  | Logout
  | LoggedIn
  | LoggedOut
  | ProfileLoaded of UserProfile

let toHash =
  function
  | Home -> "#home"
