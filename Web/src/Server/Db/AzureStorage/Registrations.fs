module Server.Db.AzureStorage.Registrations

open System
open Giraffe
open Server.Domain
open Microsoft.WindowsAzure.Storage.Table
    
let saveRegistration connection (registration: Registration) = task {
    let! table = Tables.registrationsTable connection
    let entity = DynamicTableEntity()
    entity.PartitionKey <- registration.TeamName
    entity.RowKey <- registration.UserName

    entity.Properties.["RegisteredDateTime"] <- EntityProperty.GeneratePropertyForDateTimeOffset (Nullable<DateTimeOffset> DateTimeOffset.Now)

    let! _ = table.ExecuteAsync(TableOperation.InsertOrReplace entity)
    return () 
}

