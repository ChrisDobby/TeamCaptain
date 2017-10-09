module Server.Db.AzureStorage.Tables

open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Table

let table name connection = async {
    let client = (CloudStorageAccount.Parse connection).CreateCloudTableClient()
    let table = client.GetTableReference name
    do! table.CreateIfNotExistsAsync() |> Async.AwaitTask |> Async.Ignore
    return table }

let teamsTable connection = table "team" connection
let registrationsTable connection = table "registration" connection
let fixturesTable connection = table "fixture" connection
