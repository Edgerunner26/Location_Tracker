# Location_Tracker with .NET MAUI

Tracks the user's location every 10 seconds, saves it to SQLite, and renders a heat map with an extended .NET MAUI `Map` control.

## Requirements
- Visual Studio 2022 (Windows) with the .NET MAUI workload
- NuGet: `Microsoft.Maui.Controls.Maps`, `sqlite-net-pcl`, `SQLitePCLRaw.bundle_green`
- Google Maps API key (Android)

## Run
1. Put my API key in `Platforms/Android/AndroidManifest.xml`.
2. Select an Android emulator and press F5.
3. Use the emulator's Extended Controls > Location to play a route.
