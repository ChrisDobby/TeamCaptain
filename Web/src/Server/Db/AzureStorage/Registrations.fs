module Server.Db.AzureStorage.Registrations

open System
open Server.Domain
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Table
    
let saveRegistration connection (registration: Registration) = async {
    let! table = Tables.registrationsTable connection
    let entity = DynamicTableEntity()
    entity.PartitionKey <- registration.TeamName
    entity.RowKey <- registration.UserName

    entity.Properties.["RegisteredDateTime"] <- EntityProperty.GeneratePropertyForDateTimeOffset (Nullable<DateTimeOffset> DateTimeOffset.Now)

    return! table.ExecuteAsync(TableOperation.InsertOrReplace entity) |> Async.AwaitTask |> Async.Ignore }

