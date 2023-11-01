build:
	dotnet build

clean:
	dotnet clean

restore:
	dotnet restore

run:
	dotnet run --project ./src/ServerCore/ServerCore.csproj

watch:
	dotnet watch --project ./src/ServerCore/ServerCore.csproj