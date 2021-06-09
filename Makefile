project = src/Notes

.PHONY: all
all: clean publish

.PHONY: install-npm
install-npm: clean-npm
	cd $(project) && npm ci

.PHONY: clean-npm
clean-npm:
	cd $(project) && rm -rf node_modules

.PHONY: grunt
grunt:
	@if [ -d "$(project)/node_modules" ]; then \
		cd $(project) && ./node_modules/grunt/bin/grunt; \
	else \
		echo "'grunt' not installed. Please run 'make install-npm'."; \
	fi 

.PHONY: restore
restore: clean-project
	dotnet restore

.PHONY: build
build: clean-project
	dotnet build -c Release

.PHONY: publish
publish: clean-publish clean-project
	dotnet publish -c Release /p:Version=1.0-$$(git rev-parse --short HEAD) -o publish $(project)

.PHONY: clean-project
clean-project:
	cd $(project) && rm -rf bin
	cd $(project) && rm -rf obj

.PHONY: clean-publish
clean-publish:
	rm -rf publish

.PHONY: clean
clean: clean-publish clean-project clean-npm
