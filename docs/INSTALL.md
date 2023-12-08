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

## Docker

### Build docker image

Builder docker image.

```sh
docker build --no-cache -t notes-core .
```

### Run docker image

Run image with host mount.

```sh
docker run -d \
    -p 5000:5000 \
    -e NOTES__ConnectionString__PgSql=Host=hostname;Database=notes;Username=notes;Password=notes \
    -v /path/to/data:/data notes-core
```

Run image with named volume.

```sh
docker volume create notes
docker run -d \
    -p 5000:5000 \
    -e NOTES__ConnectionString__PgSql=Host=hostname;Database=notes;Username=notes;Password=notes \
    -v notes:/data notes-core
```
