$version="2.1.0"

$package="dothttp"
cd src/${package}
dotnet pack -p:PackageVersion=$version
nuget push bin/Release/DotHttpTestCLI.${version}.nupkg -Source https://api.nuget.org/v3/index.json
cd ../..

$package="DotHttpTest"
cd src/${package}
dotnet pack -p:PackageVersion=$version
nuget push bin/Release/${package}.${version}.nupkg -Source https://api.nuget.org/v3/index.json
cd ../..
