pushd %~dp0%
pushd ..

dotnet new -u %CD% | find "redline-app"
dotnet new -i %CD% | find "redline-app"

popd
popd
