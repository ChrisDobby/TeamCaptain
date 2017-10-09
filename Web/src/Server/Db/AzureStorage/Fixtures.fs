module Server.Db.AzureStorage.Fixtures

open System
open Server.Domain
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Table

let createFixture (entity: DynamicTableEntity) =
    {
        TeamName = entity.PartitionKey
        Opposition = entity.Properties.["Opposition"].StringValue
        Location = match entity.Properties.["Location"].StringValue with
                    | "home" -> MatchLocation.Home
                    | _ -> MatchLocation.Away
        Availability = []
    }

let getFixturesForTeam connection team = async {
    let! fixtures = async {
        let! table = Tables.fixturesTable connection
        let query = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, team)
        return! table.ExecuteQuerySegmentedAsync(TableQuery(), null) |> Async.AwaitTask }

    return [ for fixture in fixtures -> createFixture fixture ] }

let getFixturesForTeams connection teams = async {
    return List.fold (fun acc team -> 
        let fixtures = Async.RunSynchronously (async {
            let! x = getFixturesForTeam connection team
            return x })
        List.append fixtures acc
    ) [] teams
}
