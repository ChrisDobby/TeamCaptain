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

type Availability =
    | Available
    | NotAvailable
    | IfRequired

type AvailabilityRecord =
    {
        PlayerUserName : string
        Availability: Availability
    }

type Team = 
    {
        Name : string
        Captains : string[]
        Players: string[]
        Availability: AvailabilityRecord[]
    }

type Fixture = 
    {
        Opposition : string
        Location : MatchLocation 
    }

type UserUpdateType =
    | RegistrationAccepted of Registration
    | RegistrationRequested of Registration
    | AvailabilityUpdated of AvailabilityRecord

// Model validation functions.  Write your validation functions once, for server and client!
module Validation =
    let verifyRegistrationTeam teamName =
        if String.IsNullOrWhiteSpace(teamName) then Some("No team name specified") else
        None    
    let verifyRegistration registration =
        verifyRegistrationTeam registration.TeamName = None