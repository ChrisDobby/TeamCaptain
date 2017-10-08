module Server.Db.AzureStorage.Registrations

open Server.Domain
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Table
    
let saveRegistration connection registration = async {
    let! table = Tables.registrationsTable connection
    let entity = DynamicTableEntity()
    entity.PartitionKey <- sprintf "%s%s" registration.TeamName registration.UserName
    entity.RowKey <- (sprintf "%s%s" registration.TeamName registration.UserName).GetHashCode() |> string

    entity.Properties.["TeamName"] <- EntityProperty.GeneratePropertyForString registration.TeamName
    entity.Properties.["UserName"] <- EntityProperty.GeneratePropertyForString registration.UserName

    return! table.ExecuteAsync(TableOperation.InsertOrReplace entity) |> Async.AwaitTask |> Async.Ignore }

