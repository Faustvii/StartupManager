<!-- markdownlint-disable MD024 -->
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- Updated to .NET 10
- Changed set-priority command alias from "sp" to "p" for consistency with other single-letter aliases

### Added

- Added priority support for startup programs (ProcessPriority property)

## [1.6.3] - 2025-08-10

### Added

- #6 Ability to disable/enable by index

### Changed

- Updated to .NET 9

## [1.6.2] - 2021-04-12

### Changed

- Updated to .NET 5
- Updated TaskScheduler dependency
- Updated Microsoft.Win32.Registry dependency

### Fixed

- #21 Scoop installation issue due to older .NET should  be resolved
- Fixed potential null pointer warnings.

## [1.6.1] - 2020-07-18

### Fixed

- #15 Fixed an issue with DirectoryService trying to search an empty path.

### Changed

- Updated TaskScheduler dependency

## [1.6.0] -  2020-06-12

No new functionality was added in this release, but almost all packages were upgraded, which has resulted in a minor performance increase.

Before

```text
Avg: 559.7063ms
Min: 553.4852ms
Max: 575.3427ms
```

After

```text
Avg: 456.1703ms
Min: 450.5132ms
Max: 461.5471ms
```

Tested on my own machine, results may vary.

### Changed

- Updated to .NET Core 3.1
- Updated TaskScheduler dependency
- Updated Registry dependency
- Updated System.Commandline framework

## [1.5.0] - 2019-11-10

### Added

- Made the list output on average twice as fast previous versions (On test machines).
- Enable/Disable commands have also gotten a performance boost.
- Remove command has also gotten a small performance boost.
- Add command has also gotten a small performance boost.

### Fixed

- In some cases an exception would be thrown when trying to Enable/Disable a program, this is no longer the case.
- It was possible to add a new startup item with the same name as an existing one in some cases.
- The remove command wasn't able to remove shortcut based Startup programs.

## [1.4.1] - 2019-11-09

### Fixed

- Version number wasn't aligned for the version command.

## [1.4.0] - 2019-11-08

### Added

- #2 It's now possible to remove programs from starting with windows.

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

[Unreleased]: https://github.com/Faustvii/StartupManager/compare/1.6.3...HEAD
[1.6.3]: https://github.com/Faustvii/StartupManager/compare/1.6.2...1.6.3
[1.6.2]: https://github.com/Faustvii/StartupManager/compare/1.6.1...1.6.2
[1.6.1]: https://github.com/Faustvii/StartupManager/compare/1.6.0...1.6.1
[1.6.0]: https://github.com/Faustvii/StartupManager/compare/1.5.0...1.6.0
[1.5.0]: https://github.com/Faustvii/StartupManager/compare/1.4.1...1.5.0
[1.4.1]: https://github.com/Faustvii/StartupManager/compare/1.4.0...1.4.1
[1.4.0]: https://github.com/Faustvii/StartupManager/compare/1.3.0...1.4.0
[1.3.0]: https://github.com/Faustvii/StartupManager/compare/1.2.1...1.3.0
[1.2.1]: https://github.com/Faustvii/StartupManager/compare/1.2.0...1.2.1
[1.2.0]: https://github.com/Faustvii/StartupManager/compare/1.1.0...1.2.0
[1.1.0]: https://github.com/Faustvii/StartupManager/compare/1.0.0...1.1.0
[1.0.0]: https://github.com/Faustvii/StartupManager/releases/tag/1.0.0

<!-- markdownlint-enable MD024-->