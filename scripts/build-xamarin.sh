#!/bin/bash

if [[ "$XAMARIN" != "1" ]]; then
  echo "No Xamarin build"
  exit
fi

echo "======= Restoring nuget packages ======="
nuget restore Lingvo/Solution/Lingvo.sln

echo "======= Building Xamarin ======="
cd "${TRAVIS_BUILD_DIR}/Lingvo/Solution"
xbuild /p:AndroidSdkDirectory=/usr/local/opt/android-sdk /p:Configuration=CI-Xamarin /target:Build