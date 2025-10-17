![Banner](docs/launchbox2mqtt_banner.png)

[![Deploy](https://github.com/ezand/launchbox2mqtt/actions/workflows/build.yml/badge.svg)](https://github.com/ezand/launchbox2mqtt/actions/workflows/build.yml)
[![GitHub License](https://img.shields.io/github/license/ezand/launchbox2mqtt)](https://choosealicense.com/licenses/mit/)
![GitHub top language](https://img.shields.io/github/languages/top/ezand/launchbox2mqtt)

> ⚠️ **Work in Progress**: This project is under active development and not ready for production use. Features may be
> incomplete, APIs may change, and bugs are expected.

# launchbox2mqtt

A LaunchBox plugin that bridges LaunchBox events to MQTT, enabling real-time monitoring and automation of retro
gaming sessions.

## Features

- **Game Lifecycle Events**: Publishes MQTT messages when games are launched, running, and exited
- **System Events**: Monitors LaunchBox system events and forwards them to MQTT
- **Game Metadata**: Publishes detailed game information including title, platform, emulator details
- **Debug Logging**: File-based logging to `<LaunchBox>/Logs/launchbox-mqtt-debug.log`
- **Configuration Menu**: Access MQTT diagnostics via LaunchBox Tools menu

## MQTT Topics

The plugin publishes to the following topics:

| Topic                        | Payload    | Description                                |
| ---------------------------- | ---------- | ------------------------------------------ |
| `launchbox/game/launching`   | `on`/`off` | Game is being launched                     |
| `launchbox/game/launched`    | `on`       | Game has started                           |
| `launchbox/game/exited`      | `on`       | Game has closed                            |
| `launchbox/game/details`     | JSON       | Full game metadata (title, platform, etc.) |
| `launchbox/app/details`      | JSON       | Additional application metadata            |
| `launchbox/emulator/details` | JSON       | Emulator configuration details             |

## Installation

### Option 1: Download Pre-built Release

1. Download the latest `MQTTPlugin-*.zip` artifact from [GitHub Actions](https://github.com/ezand/launchbox2mqtt/actions)
2. Extract the contents to `<LaunchBox Installation>/Plugins/MQTTPlugin/`
3. Restart LaunchBox

### Option 2: Build from Source

**Prerequisites:**

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

**Build Steps:**

```bash
# Clone the repository
git clone https://github.com/ezand/launchbox2mqtt.git
cd launchbox2mqtt

# Build in Release mode
dotnet build -c Release

# The plugin files will be in:
# MqttPlugin.Launchbox.Core/bin/Release/net9.0/MQTTPlugin/
```

**Install:**

1. Copy the entire `MQTTPlugin` folder to `<LaunchBox Installation>/Plugins/`
2. Restart LaunchBox
3. Verify installation by checking `Tools > MQTT configuration` menu

## Configuration

> 🔧 **Planned**: Configuration UI for broker settings without rebuilding

### Build errors on macOS/Linux

The project targets Windows-specific APIs. Use GitHub Actions or build on Windows.

## Dependencies

- [MQTTnet 5.0.1.1416](https://github.com/dotnet/MQTTnet) - MQTT client library

## 📃 License

MIT License - see [LICENSE](LICENSE) file for details.
