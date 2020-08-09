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
  "UploadKey" : "V3ryS3cureUpl0adK3y",
  "HangfireDashboardUsers" : [
    {
      "Username" : "admin",
      "Password" : "admin"
    },
    {
      "Username" : "moderator",
      "Password" : "moderator"
    }
  ]
}

```

## Database

More connectors will be available soon!

| Type | Sample connection string |
| --- | ---|
| InMemory | No connection string is required! |
| Sqlite | Data Source=eru.db |

## Upload Key

Key that will have to be provided in order to upload new substitutions.

## Hangfire Dashboard Users

Accounts declarations of admins that will have access to hangfire panel (panel with all background tasks such as sending notifications).
