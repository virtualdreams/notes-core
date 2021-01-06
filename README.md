# Notes!

Notes! is an ASP.NET Core/MariaDB based webapp to create and manage text notes.

## Features

* Markdown support
* Tags and Notebooks
* Syntax highlighting

## Technology

* [.NET Core 3.1](https://www.microsoft.com/net/core)
* [ASP.NET Core 3.1](https://docs.microsoft.com/en-us/aspnet/core/)
* [MariaDB](https://mariadb.org/)
* [PostgreSQL](https://www.postgresql.org/)
* [bootstrap 4](http://getbootstrap.com/)
* [fontawesome 4](https://fontawesome.com/)
* [nodejs](https://nodejs.org/)
* [gulpjs](http://gulpjs.com/)

## How to run

You need the latest **.NET Core**, **ASP.NET Core** and **MariaDB** or **PostgreSQL** to run this application.

## Build

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

**or**

use `make`.

```sh
$ make publish
$ dotnet /path/to/notes.dll
```

## Configuration

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
    "ConnectionStrings": {
		"Default": "Server=localhost;Database=notes;User=notes;Password=notes"
    },
    "Database": {
        "Provider": "MySql"
    },
    "FeatureManagement": {
		"ModelStateDebug": false
	},
    "Settings": {
        "KeyStore": "",
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

## Options (appsettings.json)

**Section: ConnectionStrings**

* **Default**  
MariaDB/MySQL connection string.  
`Server=[host];Database=[database];User=[username];Password=[password]`  
PosgreSQL connection string.  
`Host=[host];Database=[database];Username=[username];Password=[password][;SearchPath=schema,public]`

**Section: Database**

* **Provider**:  
Set database provider. Default: MySql. Values: MySql, PgSql

**Section: FeatureManagement**

* **ModelStateDebug**:  
Enable ModelState filter when loglevel is set to Debug.

**Section: Settings**

* **KeyStore**:  
Directory to store encryption key files (leave empty to use in-memory).
* **SiteName**:  
Site name.
* **PageSize**:  
Items per page to display. Default 10.
* ***Smtp***
	* **Enabled**:  
    Enable sending mails.
	* **Server**:  
    Mail server.
	* **Port**:  
    Smtp port.
	* **From**:  
    From mail.
	* **Username**:  
    Mail username (optional).
	* **Passwd**:  
    Mail password (optional).
	* **SkipVerify**:  
    Verify SSL certificates.

## Logging

Configure logging in `NLog.config` and copy this file to publish directory. Also check `logsettings.Production.json` and set the appropriate values.