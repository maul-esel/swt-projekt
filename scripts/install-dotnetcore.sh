#!/bin/bash

if [[ "$DOTNETCORE" != "1" ]]; then
  echo "No .NET core SDK installation"
  exit
fi

export DotNetCoreSDK_Version=1.0.0-preview4-004233

echo "======= Installing .NET Core SDK  ======="
sudo apt-get install "dotnet-dev-${DotNetCoreSDK_Version}"