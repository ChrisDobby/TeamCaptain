/// Domain model shared between client and server.
namespace Server.Domain
   
open System

type Day = 
    | Monday
    | Tuesday
    | Wednesday
    | Thursday
    | Friday
    | Saturday
    | Sunday

type TimeOfDay = string

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
        AvailabilityCheckDay: Day
        AvailabilityCheckTime: TimeOfDay
        SelectionNotifyDay: Day
        SelectionNotifyTime: TimeOfDay
    }

type Team = 
    {
        Name: string
        Config: TeamConfig
        Captains: string list
        Players: string list
    }

type RegisterTeamRequest = 
    {
        Name: string
        Config: TeamConfig
        UserName: string
    }
    static member New captain numberOfPlayers = {
        Name = ""
        UserName = captain
        Config = {
                    NumberOfPlayers = numberOfPlayers
                    AvailabilityCheckDay = Day.Monday
                    AvailabilityCheckTime = "09:00"
                    SelectionNotifyDay = Day.Monday
                    SelectionNotifyTime = "09:00"
                }
    }

type Fixture = 
    {
        TeamName: string
        Opposition: string
        Date: DateTimeOffset
        Location: MatchLocation 
        Availability: AvailabilityRecord list
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

type UserDetails =
    {
        UserName: string
        TeamsCaptainOf: string list
        TeamsMemberOf: string list
        Fixtures: Fixture list
    }

// Model validation functions.  Write your validation functions once, for server and client!
module Validation =
    let verifyRegistrationTeam teamName =
        if String.IsNullOrWhiteSpace(teamName) then Some("No team name specified") else
        None    
    let verifyRegistration (registration: Registration) =
        verifyRegistrationTeam registration.TeamName = None