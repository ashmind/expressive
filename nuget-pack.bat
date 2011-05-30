@echo off
mkdir !nupkg
nuget pack Expressive\Expressive.csproj -symbols -OutputDirectory .\!nupkg\