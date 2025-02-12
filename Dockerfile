# build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

ENV DOTNET_EnableDiagnostics=0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# RUN apk add --no-cache \
# 	icu-libs \
# 	&& rm -rf /var/cache/apk/*

WORKDIR /source

COPY notes-core.sln .
COPY src ./src

RUN dotnet restore
RUN dotnet publish -c Release -o publish --no-restore src/Notes


# final
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

ENV DOTNET_EnableDiagnostics=0
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

RUN apk add --no-cache \
	icu-libs \
	icu-data-full \
	tzdata \
	&& rm -rf /var/cache/apk/*

VOLUME ["/data"]

WORKDIR /app

COPY --from=build /source/publish .
COPY docker/appsettings.json .
COPY docker/NLog.config .

EXPOSE 5000

ENTRYPOINT ["dotnet", "Notes.dll"]