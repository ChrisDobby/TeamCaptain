module Client.Messages

open System
open ServerCode.Domain

/// The user data sent with every message.
type UserData = 
  { UserName : string 
    Token : JWT }

/// The different pages of the application. If you add a new page, then add an entry here.
type Page = 
  | Home 

let toHash =
  function
  | Home -> "#home"
