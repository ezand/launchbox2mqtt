# Development Guide

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Building from Source

### Windows Build

```bash
# Clone the repository
git clone https://github.com/ezand/launchbox2mqtt.git
cd launchbox2mqtt

# Build in Release mode
dotnet build -c Release

# The plugin files will be in:
# MqttPlugin.Launchbox.Core/bin/Release/net9.0-windows/MQTTPlugin/
```

### macOS/Linux Build

The project targets Windows-specific APIs but can be built on macOS/Linux:

```bash
dotnet build -c Release
```

The `EnableWindowsTargeting` property allows cross-platform builds. The resulting DLL will only run on Windows.

## Installation from Build

1. Copy the entire `MQTTPlugin` folder to `<LaunchBox Installation>/Plugins/`
2. Restart LaunchBox
3. Verify installation by checking `Tools > MQTT Configuration` menu item

## Dependencies

- [MQTTnet 5.x](https://github.com/dotnet/MQTTnet) - MQTT client library
