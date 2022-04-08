gen-code:
	bash ./eng/gen-code.sh

test:
	dotnet format --verify-no-changes
	dotnet test

build:
	dotnet build

format:
	dotnet tool run dotnet-format

update-nuget:
	dotnet tool run nukeeper update

update-dotnet-tool:
	bash ./eng/update-dotnet-tool.sh

update-sln:
	bash ./eng/update-sln.sh

clean:
	rm -rf ./bin
	rm -rf ./tmp
	rm -rf ./pub

.PHONY: test build
