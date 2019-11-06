<!-- markdownlint-disable MD024 -->
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed

- List output had too many newlines as seperator between header.
- #7 Documentation has been updated

## [1.3.0] - 2019-11-05

It's now finally possible to add programs to startup

### Added

- #1 Added a startup command
  - It's possible to add startup tasks for the current user to run as Administrator as well.
- List command can now also see Startup tasks in Task Scheduler.
- It's now possible to enable/disable startup tasks from Task Scheduler.

## [1.2.1] - 2019-11-02

No new application features, but streamlined build, publish and release process.

### Added

- Github actions
  - Build
  - Publish
  - Release

## [1.2.0] - 2019-10-29

### Added

- #5 Added a different enabled/disabled message when the state wouldn't change

### Changed

- #4 Changed default list output

### Fixed

- #3 Fixed a bug with "empty" entries in the list output.

## [1.1.0] - 2019-10-27

### Fixed

- List command did not find all programs
  - It has been updated to look in more locations now.

## [1.0.0] - 2019-10-26

### Added

- List command
  - It's now possible to list the programs starting with windows.
- Enable/Disable command
  - It's now possible to enable/disable existing programs starting with windows.

[Unreleased]: https://github.com/Faustvii/StartupManager/compare/1.3.0...HEAD
[1.3.0]: https://github.com/Faustvii/StartupManager/compare/1.2.1...1.3.0
[1.2.1]: https://github.com/Faustvii/StartupManager/compare/1.2.0...1.2.1
[1.2.0]: https://github.com/Faustvii/StartupManager/compare/1.1.0...1.2.0
[1.1.0]: https://github.com/Faustvii/StartupManager/compare/1.0.0...1.1.0
[1.0.0]: https://github.com/Faustvii/StartupManager/releases/tag/1.0.0

<!-- markdownlint-enable MD024-->