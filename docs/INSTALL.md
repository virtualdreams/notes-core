# Install

## Prerequisites

You need the latest **.NET Core**, **ASP.NET Core** and **PostgreSQL** or **MariaDB** to run this application.

## From source

### Run from source

```sh
dotnet run --project src/Notes/Notes.csproj
```

### Publish and run from source

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
