# Config

Application configuration can be handled via many ways described [here](https://docs.microsoft.com/pl-pl/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#environment-variables).
Most common way to do it is via appsettings.json file available in src/eru.WebApp and it is our most recommended way of doing it.

## Sample appsettings.json

```json
{
  "Serilog" : {
    "Using" : ["Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Seq"],
    "MinimumLevel" : "Information",
    "WriteTo": [
      {"Name" :  "Console"},
      {"Name" :  "File", "Args" :  {"path" :  "../Logs/log.txt", "rollingInterval" :  "Day"}},
      {"Name" :  "Seq", "Args" :  {"serverUrl" :  "http://localhost:5341/"}}
    ],
    "Enrich" : ["FromLogContext"]
  },
  "AllowedHosts": "*",
  "Database": {
    "Type": "sqlite",
    "AutomaticallyMigrate": true,
    "ConnectionString": "Data Source=eru.db"
  },
  "UploadKey": "V3ryS3cureUpl0adK3y",
  "CultureSettings": {
    "AvailableCultures": ["pl", "en"],
    "DefaultCulture": "pl"
  },
  "Admins" : [
    {
      "Username" : "admin",
      "Password" : "s@mpl3P@ssword"
    }
  ]
}


```

## Serilog

Seq is recommended way of accessing logs! [More details.](https://docs.datalust.co/v2/docs/getting-started)

Logging configuration.
See more here: [serilog configuration](https://github.com/serilog/serilog/wiki/Configuration-Basics).

## AllowedHosts

The host headers that are allowed to access the web pages. See [here](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hostfiltering.hostfilteringoptions.allowedhosts?view=aspnetcore-3.1) for more details.

## Database

When AutomaticallyMigrate is set to true the database will be set up during the boot of application. Otherwise database setup with `dotnet ef database update -p src/eru.Infrastrcuture -s src/eru.WebApp` from root folder is required.

| Type | Sample connection string |
| --- | ---|
| InMemory | No connection string is required! |
| Sqlite | Data Source=eru.db |
| Postgres | Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase |

## Upload Key

Key that will have to be provided in order to upload new substitutions.

## Culture Settings

All input in this section must be a valid ISO language code.

Available cultures shouldn't be changed unless you create a new translation for application.

DefaultCulture can be set to on of AvailableCultures.

## Admins

All provided here accounts will be able to access hangfire and admindashboard.
