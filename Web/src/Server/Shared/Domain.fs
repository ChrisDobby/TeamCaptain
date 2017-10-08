/// Domain model shared between client and server.
namespace Server.Domain
   
open System

// Json web token type.
type JWT = string

type Registration = 
    {
        TeamName: string
        UserName: string
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
        PlayerUserName: string
        Availability: Availability
    }

type TeamConfig =
    {
        NumberOfPlayers: int
        AvailabilityCheckDay: int
        AvailabilityCheckTime: int
        SelectionNotifyDay: int
        SelectionNotifyTime: int
    }

type Team = 
    {
        Name: string
        Config: TeamConfig
        Captains: string[]
        Players: string[]
    }

type Fixture = 
    {
        Opposition: string
        Location: MatchLocation 
        Availability: AvailabilityRecord[]
    }

type UserUpdateType =
    | RegistrationAccepted of Registration
    | RegistrationRequested of Registration
    | AvailabilityUpdated of AvailabilityRecord

type UserUpdate =
    {
        UserName: string
        UpdateType: UserUpdateType
    }

// Model validation functions.  Write your validation functions once, for server and client!
module Validation =
    let verifyRegistrationTeam teamName =
        if String.IsNullOrWhiteSpace(teamName) then Some("No team name specified") else
        None    
    let verifyRegistration registration =
        verifyRegistrationTeam registration.TeamName = None