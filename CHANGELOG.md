# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

[Unreleased]: https://github.com/Faustvii/StartupManager/compare/v1.2.0...HEAD
[1.2.0]: https://github.com/Faustvii/StartupManager/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/Faustvii/StartupManager/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/Faustvii/StartupManager/releases/tag/v1.0.0