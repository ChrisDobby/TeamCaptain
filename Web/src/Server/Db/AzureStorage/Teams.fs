module Server.Db.AzureStorage.Teams

open System
open Server.Domain
open Giraffe
open Microsoft.WindowsAzure.Storage.Table
open Server.Data

let createTeam (entity: DynamicTableEntity) =
    {
        Name = entity.RowKey
        Config = 
            {
                NumberOfPlayers = entity.Properties.["NumberOfPlayers"].Int32Value.Value
                AvailabilityCheckDay = entity.Properties.["AvailabilityCheckDay"].Int32Value.Value |> Converter.ToDay
                AvailabilityCheckTime = entity.Properties.["AvailabilityCheckTime"].StringValue
                SelectionNotifyDay = entity.Properties.["SelectionNotifyDay"].Int32Value.Value |> Converter.ToDay
                SelectionNotifyTime = entity.Properties.["SelectionNotifyTime"].StringValue
            }
        Captains = entity.Properties.["Captains"].StringValue |> Server.FableJson.ofJson
        Players = entity.Properties.["Players"].StringValue |> Server.FableJson.ofJson
    }

let createTableEntity name config captains players =
    let entity = DynamicTableEntity()
    entity.PartitionKey <- "Teams"
    entity.RowKey <- name
    entity.Properties.["NumberOfPlayers"] <- EntityProperty.GeneratePropertyForInt (Nullable<int>(config.NumberOfPlayers))
    entity.Properties.["AvailabilityCheckDay"] <- EntityProperty.GeneratePropertyForInt (Nullable<int>(Converter.ToInt config.AvailabilityCheckDay))
    entity.Properties.["AvailabilityCheckTime"] <- EntityProperty.GeneratePropertyForString (string(config.AvailabilityCheckTime))
    entity.Properties.["SelectionNotifyDay"] <- EntityProperty.GeneratePropertyForInt (Nullable<int>(Converter.ToInt config.SelectionNotifyDay))
    entity.Properties.["SelectionNotifyTime"] <- EntityProperty.GeneratePropertyForString (string(config.SelectionNotifyTime))
    entity.Properties.["Captains"] <- EntityProperty.GeneratePropertyForString(Server.FableJson.toJson captains)
    entity.Properties.["Players"] <- EntityProperty.GeneratePropertyForString(Server.FableJson.toJson players)
    
    entity

let updateTeam connection (team: Team) = task {
    let! table = Tables.teamsTable connection
    let entity = createTableEntity team.Name team.Config team.Captains team.Players
    let! _ = table.ExecuteAsync(TableOperation.InsertOrReplace entity)
    return ()
}

let registerTeam connection teamRequest = task {
    let! table = Tables.teamsTable connection
    let entity = createTableEntity teamRequest.Name teamRequest.Config [teamRequest.UserName] [teamRequest.UserName] 
    let! _ = table.ExecuteAsync(TableOperation.InsertOrReplace entity)
    return ()
}

let getTeams connection = task {
    let! teams = async {
        let! table = Tables.teamsTable connection
        return! table.ExecuteQuerySegmentedAsync(TableQuery(), null) |> Async.AwaitTask }

    return [ for team in teams -> createTeam team ]}

let getTeam connection teamName = task {
    let! teams = async {
        let! table = Tables.teamsTable connection
        let query = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, teamName)
        return! table.ExecuteQuerySegmentedAsync(TableQuery(FilterString = query), null) |> Async.AwaitTask }

    return
        if teams.Results.Count = 1 then Some(createTeam teams.Results.[0]) else None
}
