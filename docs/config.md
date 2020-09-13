# Config

Application configuration can be handled via many ways described [here](https://docs.microsoft.com/pl-pl/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#environment-variables).
Most common way to do it is via appsettings.json file available in src/eru.WebApp and it is our most recommended way of doing it.

## Sample appsettings.json

```json
{
  "Serilog" : {
    "Using" : ["Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Seq"],
    "MinimumLevel" : {
      "Default": "Information"
    },
    "WriteTo": [
      {"Name" :  "Console"},
      {"Name" :  "File", "Args" :  {"path" :  "../Logs/log.txt", "rollingInterval" :  "Day"}},
      {"Name" :  "Seq", "Args" :  {"serverUrl" :  "http://localhost:5341/"}}
    ],
    "Enrich" : ["FromLogContext"]
  },
  "AllowedHosts": "*",
  "Database": {
    "AutomaticallyMigrate": true,
    "ConnectionString": "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase;"
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
  ],
  "Paths": {
    "WebAppPathBase": "/",
    "SeqUiPath": "http://localhost:5341/"
  },
  "PlatformClients": {
    "FacebookMessenger": {
      "VerifyToken": "sample-verify-token",
      "AccessToken": "sample-access-token"
    }
  }
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

Application uses Postgre DB. Use valid connection string!

```sh
Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase
```

## Upload Key

Key that will have to be provided in order to upload new substitutions.

## Culture Settings

All input in this section must be a valid ISO language code.

Available cultures shouldn't be changed unless you create a new translation for application.

DefaultCulture can be set to on of AvailableCultures.

## Admins

All provided here accounts will be able to access hangfire and admin dashboard.

## Paths

WebAppBasePath sets the base path for the application. Read more [here](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.usepathbaseextensions.usepathbase?view=aspnetcore-3.1). NOTE: WebAppBasePath SHOULD MATCH reverse proxy path on proxy server (for instance, http://myproxyserver/eru should be a proxy to http://myeruserver/eru). If you're not using reverse proxy, you probably can leave it as in sample config. 

SeqUiPath is used by admin dashboard to provide easy access to logs. You should set it to address accessible outside the local network. 

## PlatformClients

Store for data used by Platform Clients. 

### Facebook Messenger

VerifyToken sets the verify token used by the Facebook Messenger Platform to verify a webhook. See more [here](https://developers.facebook.com/docs/messenger-platform/webhook).

AccessToken is the authorization token used by the Facebook API and can be obtained in application panel. See how [here](https://developers.facebook.com/docs/messenger-platform/getting-started/quick-start). 