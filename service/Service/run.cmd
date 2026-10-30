@echo off

dotnet restore
dotnet build
set "ASPNETCORE_ENVIRONMENT=Development"
set "DOTNET_ROLL_FORWARD=Major"
dotnet run --no-build --no-launch-profile
