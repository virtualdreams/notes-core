# Install

## How to run

You need the latest **.NET**, **ASP.NET Core** and **MariaDB** or **PostgreSQL** to run this application.

## Database

**MariaDB**

Create user.

```sql
# mysql

create user 'notes'@'localhost' identified by 'password';
grant all on notes.* to 'notes'@'localhost';
```

Create database.

```sql
# mysql

create database notes;
```

Import schema.

```sh
mysql -u notes -p -D notes < contrib/database-create-mysql.sql
```

**PostgreSQL**

Create user.

```sql
# su - postgres -c psql

create user notes with password 'password';
```

Create database.

```sql
# su - postgres -c psql

create database notes with owner notes encoding 'UTF8' lc_collate = 'en_US.UTF-8' lc_ctype = 'en_US.UTF-8' template template0;
```

Remove create for public.

```sql
# su - postgres -c psql

\c notes

revoke create on schema public from public; 
grant create on schema public to notes;
```

Import schema.

```sh
psql -U notes -h localhost -d notes < contrib/database-create-psql.sql 
```

## Build

**Build and run local**

```sh
dotnet restore
dotnet build
dotnet run --project src/Notes
```

**Build and publish**

Run in PowerShell or bash:

```sh
dotnet publish -c Release /p:Version=1.0-$(git rev-parse --short HEAD) -o publish src/Notes
dotnet /path/to/Notes.dll
```

*or*

use `make`.

```sh
make publish
dotnet /publish/Notes.dll
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
        "PageSize": 10
    },
    "Mail": {
        "Enabled": false,
        "Server": "localhost",
        "Port": 25,
        "From": "admin@localhost",
        "Username": "",
        "Passwd": "",
        "SkipVerify": false
    }
}
```

## Options (appsettings.json)

**Section: ConnectionStrings**

* **Default**:  
MariaDB/MySQL connection string.  
`Server=[host];Database=[database];User=[username];Password=[password]`  
PosgreSQL connection string.  
`Host=[host];Database=[database];Username=[username];Password=[password][;SearchPath=schema,public]`

**Section: Database**

* **Provider**:  
Set database provider. Default: `PgSql`. Values: `MySql`, `PgSql`.

**Section: FeatureManagement**

* **ModelStateDebug**:  
Enable ModelState filter when loglevel is set to Debug. Default: `false`.

**Section: Settings**

* **KeyStore**:  
Directory to store encryption key files (leave empty to use in-memory). Default: `null`.
* **SiteName**:  
Site name. Default: `"Notes!"`.
* **PageSize**:  
Items per page to display. Default `10`.

**Section: Mail**
* **Enabled**:  
Enable sending mails. Default: `false`.
* **Host**:  
Mail host. Default: `"localhost"`.
* **Port**:  
Smtp port. Default `25`.
* **MailFrom**:  
Mail From. Default: `"admin@localhost"`.
* **Username**:  
Mail username (optional). Default: `null`.
* **Password**:  
Mail password (optional). Default: `null`.
* **DisableCertificateValidation**:  
Verify SSL certificates. Default: `false`.

## Logging

Configure logging in `NLog.config` and copy this file to publish directory. Also check `logsettings.Production.json` and set the appropriate values.