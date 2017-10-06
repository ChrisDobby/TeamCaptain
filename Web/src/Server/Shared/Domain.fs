/// Domain model shared between client and server.
namespace Server.Domain
   
open System

// Json web token type.
type JWT = string

type MatchLocation = 
    | Home
    | Away

type Team = 
    {
        Name : string
    }

type Fixture = 
    {
        Opposition : string
        Location : MatchLocation 
    }

// Model validation functions.  Write your validation functions once, for server and client!
//module Validation =
