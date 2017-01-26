#!/bin/bash

if [[ "$DOTNETCORE" != "1" ]]; then
  echo "No backend test"
  exit
fi

set -e
XUNIT_RUNNER="${HOME}/.nuget/packages/xunit.runner.console/2.1.0/tools/xunit.console"

echo "======= Testing Backend  ======="
cd "${TRAVIS_BUILD_DIR}/Lingvo/Solution/tests/Backend.Tests"
dotnet test
