#!/bin/bash

if [[ "$XAMARIN" != "1" ]]; then
  echo "No Xamarin installation"
  exit
fi

export Xamarin_iOS_Version=10.2.0.4
export Xamarin_Android_Version=7.0.1-2

echo "======= Installing Xamarin.iOS  ======="
wget -nc -P downloads "https://xamarin.azureedge.net/MonoTouch/Mac/xamarin.ios-${Xamarin_iOS_Version}.pkg"
sudo installer -pkg "downloads/xamarin.ios-${Xamarin_iOS_Version}.pkg" -target /

echo "======= Installing Xamarin.Android ======="
wget -nc -P downloads "https://xamarin.azureedge.net/MonoforAndroid/Mac/xamarin.android-${Xamarin_Android_Version}.pkg"
sudo installer -pkg "downloads/xamarin.android-${Xamarin_Android_Version}.pkg" -target /

echo "======= Installing Android SDK ======="
brew install android-sdk
sudo /usr/local/opt/android-sdk/tools/android update sdk -u -a -t 1,2,3,33,163,164 <<< "y"