module Server.Db.AzureStorage.Teams

open System
open Server.Domain
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Table

let createTeam (entity: DynamicTableEntity) =
    {
        Name = entity.PartitionKey
        Config = 
            {
                NumberOfPlayers = entity.Properties.["NumberOfPlayers"].Int32Value.Value
                AvailabilityCheckDay = entity.Properties.["AvailabilityCheckDay"].Int32Value.Value
                AvailabilityCheckTime = entity.Properties.["AvailabilityCheckTime"].Int32Value.Value
                SelectionNotifyDay = entity.Properties.["SelectionNotifyDay"].Int32Value.Value
                SelectionNotifyTime = entity.Properties.["SelectionNotifyTime"].Int32Value.Value
            }
        Captains = entity.Properties.["Captains"].StringValue |> Server.FableJson.ofJson
        Players = entity.Properties.["Players"].StringValue |> Server.FableJson.ofJson
    }

let createTableEntity name config captains players =
    let entity = DynamicTableEntity()
    entity.PartitionKey <- name
    entity.RowKey <- name.GetHashCode() |> string
    entity.Properties.["NumberOfPlayers"] <- EntityProperty.GeneratePropertyForInt (Nullable<int>(config.NumberOfPlayers))
    entity.Properties.["AvailabilityCheckDay"] <- EntityProperty.GeneratePropertyForInt (Nullable<int>(config.AvailabilityCheckDay))
    entity.Properties.["AvailabilityCheckTime"] <- EntityProperty.GeneratePropertyForInt (Nullable<int>(config.AvailabilityCheckTime))
    entity.Properties.["SelectionNotifyDay"] <- EntityProperty.GeneratePropertyForInt (Nullable<int>(config.SelectionNotifyDay))
    entity.Properties.["SelectionNotifyTime"] <- EntityProperty.GeneratePropertyForInt (Nullable<int>(config.SelectionNotifyTime))
    entity.Properties.["Captains"] <- EntityProperty.GeneratePropertyForString(Server.FableJson.toJson captains)
    entity.Properties.["Players"] <- EntityProperty.GeneratePropertyForString(Server.FableJson.toJson players)
    
    entity

let updateTeam connection (team: Team) = async {
    let! table = Tables.teamsTable connection
    let entity = createTableEntity team.Name team.Config team.Captains team.Players
    return! table.ExecuteAsync(TableOperation.InsertOrReplace entity) |> Async.AwaitTask |> Async.Ignore
}

let registerTeam connection teamRequest = async {
    let! table = Tables.teamsTable connection
    let entity = createTableEntity teamRequest.Name teamRequest.Config [teamRequest.UserName] [teamRequest.UserName] 
    return! table.ExecuteAsync(TableOperation.InsertOrReplace entity) |> Async.AwaitTask |> Async.Ignore
}

let getTeams connection = async {
    let! teams = async {
        let! table = Tables.teamsTable connection
        return! table.ExecuteQuerySegmentedAsync(TableQuery(), null) |> Async.AwaitTask }

    return [ for team in teams -> createTeam team ]}

let getTeam connection teamName = async {
    let! teams = async {
        let! table = Tables.teamsTable connection
        let query = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, teamName)
        return! table.ExecuteQuerySegmentedAsync(TableQuery(FilterString = query), null) |> Async.AwaitTask }

    return
        if teams.Results.Count = 1 then Some(createTeam teams.Results.[0]) else None
}
