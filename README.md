# Notes!

Notes! is an ASP.NET Core/MongoDB based webapp to create and manage simple text notes.

## Features

* Markdown support
* Tags and Notebooks
* Syntax highligting for code blocks

## Technology

* [.NET Core 2.1](https://www.microsoft.com/net/core)
* [ASP.NET Core 2.1.5](https://docs.microsoft.com/en-us/aspnet/core/)
* [MongoDB](https://www.mongodb.com/)
* [bootstrap 4](http://getbootstrap.com/)
* [fontawesome 4](https://fontawesome.com/)
* [nodejs](https://nodejs.org/)
* [gulpjs](http://gulpjs.com/)

## Configuration

You need the latest **.NET Core**, **ASP.NET Core** and **MongoDB** to run this application.

### Application

Configure application in `appsettings.json` and copy to publish directory.

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
		"KeyStore": "",                    // directory to store encryption key files (leave empty to use memory)					
		"MongoDB": "mongodb://127.0.0.1/", // mongodb connection string
		"Database": "notes",               // collection name to use
		"SiteName": "Notes!",              // site name
		"PageSize": 10,                    // items per page
		"Smtp": {
			"Enabled": false,              // enable sending mails
			"Server": "localhost",         // mail server
			"Port": 25,                    // mail port
			"From": "admin@localhost",     // from mail
			"Username": "",                // mail username
			"Passwd": "",                  // mail password
			"SkipVerify": false            // verify SSL certificates
		}
	}
}
```

### Logging

Configure logging in `NLog.config` and copy to publish directory.

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