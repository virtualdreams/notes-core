all: restore clean-publish publish

install-npm: clean-npm
	npm ci

clean-npm:
	rm -rf node_modules

gulp:
	./node_modules/gulp/bin/gulp.js

restore:
	dotnet restore

build:
	dotnet build -c Release

publish:
	dotnet publish -c Release /p:Version=1.0-$$(git rev-parse --short HEAD)

clean-publish:
	rm -rf bin/Release

clean:
	rm -rf bin
	rm -rf obj
	rm -rf node_modules
