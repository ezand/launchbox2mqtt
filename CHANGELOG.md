# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.0.1] - 2025-10-22

### Added

- LaunchBox plugin that bridges events to MQTT for real-time monitoring and automation
- Game lifecycle event publishing (launched, running, exited)
- Emulator event tracking (loading, execution state)
- System event monitoring (LaunchBox and BigBox state changes)
- Game metadata publishing as JSON with detailed information
- MQTT configuration UI dialog accessible via Tools menu
- Secure password encryption using Windows DPAPI
- File-based debug logging to `<LaunchBox>/Logs/launchbox-mqtt-debug.log`
- GitHub Actions workflow for automated builds
- GitHub Actions workflow for automated releases
- Menu icon and dialog banner for plugin UI
- LaunchBox version and ID details fetching from core DLL
- Comprehensive MQTT topic structure for content, emulator, and system events
- Retained message support for last played content and emulator details
- Connection test functionality in configuration dialog
- Automatic reconnection after configuration changes

### Documentation

- Initial README with feature overview and installation instructions
- LICENSE file (MIT)
- Development guide documentation
- Integration guide for retro2mqtt companion project
- MQTT topics reference table

[Unreleased]: https://github.com/ezand/launchbox2mqtt/commits/main/
[0.0.1]: https://github.com/ezand/launchbox2mqtt/releases/tag/v0.0.1
