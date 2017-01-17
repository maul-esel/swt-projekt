#!/bin/bash

if [[ "$DOTNETCORE" != "1" ]]; then
  echo "No backend build"
  exit
fi

echo "======= Building Backend ======="
cd "${TRAVIS_BUILD_DIR}/Lingvo/Solution/src/Backend"
dotnet restore
dotnet build -c Debug

echo "======= Building Common.Tests ======="
cd "${TRAVIS_BUILD_DIR}/Lingvo/Solution/tests/Common.Tests"
dotnet restore
dotnet build -c Debug

echo "======= Building Backend.Tests ======="
cd "${TRAVIS_BUILD_DIR}/Lingvo/Solution/tests/Backend.Tests"
dotnet restore
dotnet build -c Debug