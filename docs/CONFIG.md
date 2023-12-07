# Configuration

## Example configuration

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
        "PgSql": "Host=localhost;Database=notes;Username=notes;Password=notes"
    },
    "Database": {
        "Provider": "PgSql"
    },
    "FeatureFlags": {
        "ModelStateDebug": false
    },
    "Settings": {
        "KeyStore": "keystore",
        "SiteName": "Notes!",
        "PageSize": 10
    },
    "Mail": {
        "Enabled": false,
        "Server": "localhost",
        "Port": 25,
        "From": "admin@localhost",
        "Username": "demo",
        "Password": "demo",
        "DisableCertificateValidation": false,
        "DisableCheckCertificateRevocation": false
    }
}
```

## Options (appsettings.json)

**Section: ConnectionStrings**

* **PgSql**:  
PosgreSQL connection string.  
`Host=[host];Database=[database];Username=[username];Password=[password][;SearchPath=schema,public]`

* **MySql**:  
MariaDB/MySQL connection string.  
`Server=[host];Database=[database];User=[username];Password=[password]`

**Section: Database**

* **Provider**:  
Set database provider.  
Values: `"PgSql"`, `"MySql"`  
Default: `"PgSql"`

**Section: FeatureFlags**

* **ModelStateDebug**:  
Enable ModelState filter when loglevel is set to Debug.  
Default: `false`

**Section: Settings**

* **KeyStore**:  
Directory to store encryption key files (leave empty to use in-memory).  
Default: `null`
* **SiteName**:  
Site name.  
Default: `"Notes!"`
* **PageSize**:  
Items per page to display.  
Default `10`

**Section: Mail**
* **Enabled**:  
Enable sending mails.  
Default: `false`
* **Host**:  
Mail host.  
Default: `"localhost"`
* **Port**:  
Smtp port.  
Default `25`
* **MailFrom**:  
Mail From. Default:  
`"admin@localhost"`
* **Username**:  
Mail username (optional).  
Default: `null`
* **Password**:  
Mail password (optional).  
Default: `null`
* **DisableCertificateValidation**:  
Verify SSL certificates.  
Default: `false`
* **DisableCheckCertificateRevocation**:  
Check certificate revocation.  
Default: `false`