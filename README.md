# Notes!

Notes! is an ASP.NET Core/MongoDB based webapp to create and manage simple text notes.

## Features

* Markdown support
* Tags and Notebooks
* Syntax highligting for code blocks

## Technology

* [.NET Core 2.1](https://www.microsoft.com/net/core)
* [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
* [MongoDB](https://www.mongodb.com/)
* [bootstrap 3](http://getbootstrap.com/)
* [nodejs](https://nodejs.org/) (v8.11 LTS)
* [gulpjs](http://gulpjs.com/)

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
	}
}
```