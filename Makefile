gen-code:
	bash ./eng/gen-code.sh

test:
	dotnet test --no-restore

build:
	dotnet build

format:
	dotnet tool run dotnet-format

update-nugut:
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
