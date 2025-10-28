# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**launchbox2mqtt** is a LaunchBox plugin that bridges LaunchBox/BigBox events to MQTT, enabling real-time monitoring and automation of retro gaming sessions. It's built as a .NET 9.0 Windows Forms application that implements LaunchBox's plugin interfaces.

## Build Commands

```bash
# Restore dependencies
dotnet restore

# Build Release (preferred for deployment)
dotnet build -c Release

# Build Debug
dotnet build -c Debug

# Output location after build
# MqttPlugin.Launchbox.Core/bin/Release/net9.0-windows/MQTTPlugin/
```

The build automatically copies the plugin DLL and MQTTnet dependency into a `MQTTPlugin` folder (see MqttPlugin.Launchbox.Core.csproj:30-39).

## Architecture

### Core Components

**MQTT.cs** - Singleton MQTT client manager
- Lazy initialization with double-checked locking (MQTT.cs:14-43)
- Uses MQTTnet 5.x library
- Client ID format: `launchbox2mqtt_{MachineId}` (MQTT.cs:29)
- Fire-and-forget connection model with internal error logging
- `ReloadConfig()` disconnects and reinitializes client (MQTT.cs:61-75)

**ConfigManager.cs** - Configuration persistence
- Stores config.json in plugin folder alongside DLL (ConfigManager.cs:12-16)
- Passwords encrypted using Windows DPAPI with CurrentUser scope (ConfigManager.cs:69-85)
- Defaults: localhost:1883, no auth (MqttConfig.cs)

**Logger.cs** - File-based logging
- Writes to `<LaunchBox>/Logs/launchbox-mqtt-debug.log`
- Thread-safe append operations
- Levels: Info, Debug, Error

### Plugin Hooks (LaunchBox Integration)

The plugin implements three LaunchBox plugin interfaces:

1. **GameLaunchingEvent.cs** (`IGameLaunchingPlugin`)
   - `OnBeforeGameLaunching`: Sets content/emulator to "loaded" state
   - `OnAfterGameLaunched`: Publishes game/emulator details as JSON, sets "running" state
   - `OnGameExited`: Calls `EventUtils.UnloadContentEvent()` to reset states

2. **SystemEvents.cs** (`ISystemEventsPlugin`)
   - Maps LaunchBox system events to MQTT topics
   - Key events: LaunchBoxStartupCompleted, BigBoxStartupCompleted, BigBoxLocked/Unlocked, shutdown events
   - Publishes LaunchBox version/ID on startup (SystemEvents.cs:17-21)

3. **SystemMenuItem.cs** (`ISystemMenuItemPlugin`)
   - Adds "MQTT Configuration" menu item to Tools menu
   - Opens MqttConfigForm dialog

### Event Flow

```
Game Launch → OnBeforeGameLaunching → loaded=on
           → OnAfterGameLaunched → running=on + details
           → OnGameExited → loaded=off, running=off

LaunchBox Start → LaunchBoxStartupCompleted → launchbox/running=on
LaunchBox Exit → LaunchBoxShutdownBeginning → all states=off
```

**EventUtils.cs** provides `UnloadContentEvent()` - called on game exit and shutdown to reset all content/emulator states to "off" and clear titles (EventUtils.cs:5-14).

### MQTT Topic Structure

All topics use prefix `launchbox/`
- Content topics: `content/{loaded,running,last_played,details}`
- Emulator topics: `emulator/{loaded,running,last_loaded,details}`
- System topics: `running`, `bigbox/{running,locked}`, `system/event`
- Retained topics: `last_played`, `last_loaded`, `details` (for all entities), `launchbox/details`

### Dependencies

**External:**
- LaunchBox plugin DLLs (in deps/): `Unbroken.LaunchBox.Plugins.dll`, `Unbroken.LaunchBox.dll`
- MQTTnet 5.0.1.1416 (NuGet)
- System.Drawing.Common 9.0.0 (for UI)

**LaunchBox Plugin API:**
- `IGame`, `IEmulator`, `IAdditionalApplication` data interfaces (from Unbroken.LaunchBox.Plugins.Data)
- `SystemEventTypes` enum for event type constants
- `Unbroken.LaunchBox.State` static class for version/ID/machineId

## Development Notes

- **Cross-platform builds**: EnableWindowsTargeting allows macOS/Linux builds, but DLL only runs on Windows
- **Thread safety**: MQTT client initialization uses double-checked locking
- **Error handling**: Connection failures logged but don't block execution (fire-and-forget async)
- **UI**: MqttConfigForm.cs implements Windows Forms configuration dialog with connection test button
- **Diagnostics**: Diagnostics.cs provides environment/config debug info

## Configuration

Config saved to plugin folder as `config.json`:
```json
{
  "Host": "localhost",
  "Port": 1883,
  "Username": "optional",
  "EncryptedPassword": "base64-dpapi-encrypted"
}
```

UI accessible via: LaunchBox → Tools → MQTT Configuration
