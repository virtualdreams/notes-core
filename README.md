# Notes!

Notes! is an ASP.NET Core/MariaDB based webapp to create and manage simple text notes.

## Features

* Markdown support
* Tags and Notebooks
* Syntax highlighting

## Technology

* [.NET Core 2.2](https://www.microsoft.com/net/core)
* [ASP.NET Core 2.2](https://docs.microsoft.com/en-us/aspnet/core/)
* [MariaDB](https://mariadb.org/)
* [bootstrap 4](http://getbootstrap.com/)
* [fontawesome 4](https://fontawesome.com/)
* [nodejs](https://nodejs.org/)
* [gulpjs](http://gulpjs.com/)

## How to run

You need the latest **.NET Core**, **ASP.NET Core** and **MariaDB** to run this application.

### Build

**Build to run on local**

```sh
dotnet restore
dotnet build
dotnet run
```

**Build and publish**

Run in PowerShell or bash:

```sh
dotnet publish -c Release /p:Version=1.0-$(git rev-parse --short HEAD)
dotnet /path/to/notes.dll
```

### Configuration

Configure application in `appsettings.json` and copy this file to publish directory.

```json
{
    "Kestrel": {
        "EndPoints": {
            "Http": {
                "Url": "http://127.0.0.1:5000"
            },
            "Https": {
                "Url": "https://127.0.0.1:5001",
                "Certificate": {
                    "Path": "/foo/bar/cert.p12|pfx",
                    "Password": "cert_password"
                }
            }
        }
    },
    "Settings": {
        "KeyStore": "",
        "ConnectionString": "Server=localhost;Database=notes;User=notes;Password=notes",
        "SiteName": "Notes!",
        "PageSize": 10,
        "Smtp": {
        "Enabled": false,
            "Server": "localhost",
            "Port": 25,
            "From": "admin@localhost",
            "Username": "",
            "Passwd": "",
            "SkipVerify": false
       }
    }
}
```

#### Options

* **KeyStore**: Directory to store encryption key files (leave empty to use memory)
* **ConnectionString**: MariaDB/MySQL connection string `Server=[host];Database=[database];User=[username];Password=[password]`
* **SiteName**: Site name
* **PageSize**: Items per page (live)
* **Smtp**
	* **Enabled**: Enable sending mails
	* **Server**: Mail server
	* **Port**: Smtp port
	* **From**: From mail
	* **Username**: Mail username
	* **Passwd**: Mail password
	* **SkipVerify**: Verify SSL certificates

### Logging

Configure logging in `NLog.config` and copy this file to publish directory.

```xml
<?xml version="1.0" encoding="utf-8" ?>
  <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
        xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
        autoReload="true"
        internalLogLevel="Warn"
        internalLogFile="nlog-internal.log">

  <!-- Load the ASP.NET Core plugin -->
  <extensions>
    <add assembly="NLog.Web.AspNetCore"/>
  </extensions>

  <!-- the targets to write to -->
  <targets>
    <!-- write logs to file -->
    <target xsi:type="File" name="file" fileName="notes-${shortdate}.log" layout="${longdate} ${pad:padding=-5:inner=${uppercase:${level}}} ${logger} ${message} ${exception}" />

    <!-- write logs to console -->
    <target xsi:type="ColoredConsole" name="console" layout="${pad:padding=-5:inner=${uppercase:${level}}} ${logger} ${message} ${exception}" />

    <!-- write to the void -->
    <target xsi:type="Null" name="blackhole" />
  </targets>

  <!-- rules to map from logger name to target -->
  <rules>
    <logger name="notes.*" minlevel="Info" writeTo="console" />
	<!-- Optional if you want to log sql statements -->
    <logger name="Microsoft.EntityFrameworkCore.*" minLevel="Debug" writeTo="file" />
    <logger name="Microsoft.*" minlevel="Trace" writeTo="blackhole" final="true" />
    <logger name="*" minlevel="Debug" writeTo="file" />
  </rules>
</nlog>
```

Additionally review `logsettings.Production.json`.

```json
{
    "Logging": {
        "Console": {
            "LogLevel": {
                "Default": "None"
            }
        }
    }
}
```