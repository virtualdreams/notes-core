# Install

## How to run

You need the latest **.NET**, **ASP.NET Core** and **MariaDB** or **PostgreSQL** to run this application.

## Database

### MariaDB

Create user.

```sql
create user 'notes'@'localhost' identified by 'password';
grant all on notes.* to 'notes'@'localhost';
```

Create database.

```sql
create database notes;
```

Import schema.

```sh
mysql -u notes -p -D notes < contrib/database-create-mysql.sql
```

### PostgreSQL

Create user.

```sql
create user notes with password 'password';
```

Create database.

```sql
create database notes with owner notes encoding 'UTF8' lc_collate = 'en_US.UTF-8' lc_ctype = 'en_US.UTF-8' template template0;
```

Remove create rights for public.

```sql
\c notes

revoke create on schema public from public; 
grant create on schema public to notes;
```

Import schema.

```sh
psql -U notes -h localhost -d notes < contrib/database-create-psql.sql 
```

## Build

### Build and run

```sh
dotnet run --project src/Notes/Notes.csproj
```

### Build and publish

Run in PowerShell or bash:

```sh
dotnet publish -c Release /p:Version=1.0-$(git rev-parse --short HEAD) -o publish src/Notes
dotnet publish/Notes.dll
```

**or**

use `make`.

```sh
make publish
dotnet publish/Notes.dll
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
                "SslProtocols": ["Tls12", "Tls13"],
                "Certificate": {
                    "Path": "/foo/bar/cert.p12|pfx",
                    "Password": "cert_password"
                }
            }
        }
    },
    "ConnectionStrings": {
        "Default": "Host=localhost;Database=notes;Username=notes;Password=notes"
    },
    "Database": {
        "Provider": "PgSql"
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
* **DisableCheckCertificateRevocation**:  
Check certificate revocation. Default: `false`.

## Logging

Configure logging in `NLog.config` and copy this file to publish directory. 

```xml
<nlog>
  <rules>
    <logger name="System.*" finalMinLevel="Warn" />
    <logger name="Microsoft.*" finalMinLevel="Warn" />
    <logger name="Microsoft.Hosting.Lifetime*" finalMinLevel="Info" />
    <logger name="*" minlevel="Info" writeTo="console,file" />
  </rules>
</nlog>
```