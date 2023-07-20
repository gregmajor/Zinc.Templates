#!/bin/bash

MYDIR=$(dirname "$0")
pushd "$MYDIR"
pushd ..

dotnet new -u "$PWD" | grep "redline-app"
dotnet new -i "$PWD" | grep "redline-app"

popd
popd
