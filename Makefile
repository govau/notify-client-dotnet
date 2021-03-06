DOTNET ?= dotnet

PACKAGES   = $(wildcard src/Notify/publish/*.nupkg)
VERSIONS   = $(PACKAGES:src/Notify/publish/%.nupkg=%)
NUGET_URL ?= https://api.nuget.org/v3/index.json
NUGET_KEY ?= # username:api_key

.DEFAULT_GOAL := help

.PHONY: help
help:
	@cat $(MAKEFILE_LIST) | grep -E '^[a-zA-Z_-]+:.*?## .*$$' | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

.PHONY: build
build: ## Build project
	dotnet build

.PHONY: test
test: ## Run unit, authentication, integration tests
	$(MAKE) unit-test
	$(MAKE) authentication-test
	$(MAKE) integration-test

.PHONY: authentication-test
authentication-test: ## Run authentication tests
	$(DOTNET) test ./src/Notify.Tests/Notify.Tests.csproj --no-build --filter TestCategory=Unit/AuthenticationTests

.PHONY: integration-teet
integration-test: ## Run integration tests
	$(DOTNET) test ./src/Notify.Tests/Notify.Tests.csproj --no-build --filter TestCategory~Integration

.PHONY: unit-test
unit-test: ## Run unit tests
	$(DOTNET) test ./src/Notify.Tests/Notify.Tests.csproj --no-build --filter TestCategory=Unit/NotificationClient
	$(DOTNET) test ./src/Notify.Tests/Notify.Tests.csproj --no-build --filter TestCategory=Unit/NotifyClient

.PHONY: single-test
single-test: ## Run a single test: make single-test test=[test name]
	$(DOTNET) test ./src/Notify.Tests/Notify.Tests.csproj --no-build --filter $(test)

.PHONY: build-single-test
build-single-test: ## Run a single test: make single-test test=[test name]
	$(DOTNET) test ./src/Notify.Tests/Notify.Tests.csproj --filter $(test)

.PHONY: build-test
build-test: ## Build and test
	$(MAKE) build
	$(MAKE) unit-test
	$(MAKE) authentication-test
	$(MAKE) integration-test

.PHONY: build-integration-test
build-integration-test: ## Build and integration test
	$(MAKE) build
	$(MAKE) integration-test

.PHONY: build-unit-test
build-unit-test: ## Build and integration test
	$(MAKE) build
	$(MAKE) unit-test

.PHONY: build-release
build-release: ## Build release version
	$(DOTNET) build -c=Release

.PHONY: build-package
build-package: build-release ## Build and package NuGet
	$(DOTNET) pack -c=Release ./src/Notify/Notify.csproj -o=publish

.PHONY: publish
.PHONY: $(VERSIONS:%=publish-%)
publish: $(VERSIONS:%=publish-%) ## Publish Nuget packages

$(VERSIONS:%=publish-%): publish-%: ## Publish Nuget package
	@$(DOTNET) nuget push src/Notify/publish/$*.nupkg --source $(NUGET_URL) --api-key $(NUGET_KEY)

.PHONY: clean
clean: ## Remove temporary files
	-rm -r src/Notify/bin
	-rm -r src/Notify/publish
	-rm -r src/Notify/obj
