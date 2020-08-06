# Config

Application configuration can be handled via many ways described [here](https://docs.microsoft.com/pl-pl/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#environment-variables).
Most common way to do it is via appsettings.json file available in src/eru.WebApp and it is our most recommended way of doing it.

## Sample appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Database": {
    "Type": "sqlite",
    "ConnectionString": "Data Source=eru.db"
  },
  "UploadKey" : "V3ryS3cureUpl0adK3y"
}

```

## Database

More connectors will be available soon!

| Type | Sample connection string |
| --- | ---|
| InMemory | No connection string is required! |
| Sqlite | Data Source=eru.db |
