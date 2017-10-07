/// Domain model shared between client and server.
namespace Server.Domain
   
open System

// Json web token type.
type JWT = string

type Registration = 
    {
        TeamName : string
        UserName : string
    }

type MatchLocation = 
    | Home
    | Away

type Team = 
    {
        Name : string
        Captains : string[]
        Players: string[]
    }

type Fixture = 
    {
        Opposition : string
        Location : MatchLocation 
    }

// Model validation functions.  Write your validation functions once, for server and client!
module Validation =
    let verifyRegistrationTeam teamName =
        if String.IsNullOrWhiteSpace(teamName) then Some("No team name specified") else
        None    
    let verifyRegistration registration =
        verifyRegistrationTeam registration.TeamName = None