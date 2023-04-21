# build
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine-amd64 AS build
WORKDIR /source
COPY notes-core.sln .
COPY src ./src
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN dotnet restore
RUN dotnet publish -c Release -o publish --no-restore src/Notes

# final
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine-amd64
WORKDIR /app
COPY --from=build /source/publish .
COPY src/Notes/appsettings.json .
COPY NLog.docker.config ./NLog.config
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE 80
ENTRYPOINT ["dotnet", "Notes.dll"]