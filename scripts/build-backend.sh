#!/bin/bash

if [[ "$DOTNETCORE" != "1" ]]; then
  echo "No backend build"
  exit
fi

echo "======= Building backend ======="
cd "${TRAVIS_BUILD_DIR}/Lingvo/Solution/src/Backend"
dotnet restore
dotnet build -c Release