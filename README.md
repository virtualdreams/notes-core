# Notes!

Notes is an ASP.NET Core based app to manage text notes like OneNote, Google Keep or Evernote.

## Features

* Notes can written with markdown
	* All standard markdown features
	* Extended emphasis
	* Abbreviations
* Assign a notebook name to a note
* Assign tags to a note
* Notes can searched through mongo search engine
* Multiple users
* Syntax highligting for code blocks

## Missing features

* Share notes to users or the public
* ~~Syntax highligting for code blocks~~

## Technolgy

* [.NET Core](https://www.microsoft.com/net/core) (v1.0 / v1.1)
* [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
* [MongoDB](https://www.mongodb.com/)
* [bootstrap 3](http://getbootstrap.com/)
* [SB Admin 2 Theme](https://github.com/BlackrockDigital/startbootstrap-sb-admin-2)
* [nodejs](https://nodejs.org/) (v6.9.1 LTS)
* [gulpjs](http://gulpjs.com/)

## Build and deploy

	npm install

	dotnet restore
	dotnet build --configuration Release
	dotnet publish --configuration Release
	dotnet note.dll