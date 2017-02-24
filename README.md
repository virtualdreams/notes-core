# Notes!

Notes is an ASP.NET Core based app to manage text notes like OneNote, Google Keep or Evernote.

## Features

* Notes can written with markdown
	* All standard markdown features
	* Extended emphasis
	* Abbreviations
	* Task lists
* Assign a notebook name to a note
* Assign tags to a note
* Notes can searched through mongo search engine
* Multiple users
* Syntax highligting for code blocks

## Missing features

* Share notes to users or the public

## Technology

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

## Configuration

Configure application in `appsettings.json`.

```
{
	"Settings": {
		"KeyStore": "",                    // directory to store session key files (leave empty to use memory)					
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
	},
	"Logging": {
		"LogLevel": {
			"Microsoft": "Warning",
			"System": "Warning",
			"Default": "Info"
		}
	}
}
```