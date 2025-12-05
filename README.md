# StartupManager

[![CodeFactor](https://www.codefactor.io/repository/github/faustvii/startupmanager/badge)](https://www.codefactor.io/repository/github/faustvii/startupmanager)
![Build](https://github.com/Faustvii/StartupManager/workflows/Build/badge.svg)

A tool to manage startup programs on windows through a CLI interface.

- [StartupManager](#startupmanager)
  - [Scoop](#scoop)
  - [Commands](#commands)
    - [Help](#help)
    - [Version](#version)
    - [List](#list)
      - [Columns explanation](#columns-explanation)
    - [Enable](#enable)
    - [Disable](#disable)
    - [Add](#add)
      - [Examples of usage](#examples-of-usage)
    - [Remove](#remove)

## Scoop

You can install the application through [Scoop](https://github.com/lukesampson/scoop) if you have my bucket installed.

I recommend installing through scoop to have the app accessible on your "path"

`scoop bucket add Faustvii 'https://github.com/Faustvii/scoop-bucket.git'`

`scoop install StartupManager`

## Commands

There are currently six functional commands `list`, `enable`, `disable`, `add`, `remove`, `set-priority`

### Help

Examples of usage `StartupManager.exe --help`

```text
Usage:
  StartupManager [options] [command]

Options:
  --version    Display version information

Commands:
  l, list                             Lists the current startup programs
  d, disable <name>                   Disables one of the current startup programs
  e, enable <name>                    Enables one of the current startup programs
  a, add <name> <path> <arguments>    Adds a program to startup with windows
  r, remove <name>                    Removes the specified program from startup
  p, set-priority <name> <priority>  Sets the priority of a Task Scheduler startup program
```

The help command can also be used on any commands;

Examples of usage `StartupManager.exe list --help`

```text
list:
  Lists the current startup programs

Usage:
  StartupManager list [options]

Options:
  -d, --detailed    Shows additional output about the startup programs
```

### Version

Displays the current version of the application

Examples of usage `StartupManager.exe --version`

```text
1.6.3
```

### List

Will display a list of applications that starts with windows.

It's possible to use the `--detailed`/`-d` option to get a table showing the path and arguments for the entries as well.

Examples of usage `StartupManager.exe list` or `StartupManager.exe l`

```text
Applications starting with windows:
Name                         Admin  Enabled

f.lux                               [√]
Steam                               [√]
Discord                             [√]
SecurityHealth               [√]    [√]
```

Examples of usage `StartupManager.exe list --detailed` or `StartupManager.exe l -d`

```text
Applications starting with windows:
#  Name                         Admin  Enabled  Path

1  f.lux                               [√]      "C:\Users\Faust\Scoop\apps\flux\current\flux.exe" /noshow
2  Steam                               [√]      "C:\Users\Faust\Scoop\apps\steam\current\steam.exe" -silent
3  Discord                             [√]      C:\Users\Faust\scoop\apps\discord\current\Discord.exe --start-minimized
4  SecurityHealth               [√]    [√]      C:\Windows\system32\SecurityHealthSystray.exe
```

#### Columns explanation

- `#`
  - Numeric index used for referencing items in other commands (e.g., disabling or enabling).
- `Name` is either the
  - Registry key name
  - Shortcut filename without extension
  - Task scheduler name
- `Admin`
  - Shows if you need to have administrator priviliges to modify it
- `Enabled`
  - Shows whether the startup item is currently enabled ([√]) or disabled (blank).
- `Path`
  - Shows the path and potential arguments to the application

### Enable

Examples of usage `StartupManager.exe enable Steam` or `StartupManager.exe e Steam` or `StartupManager.exe e 2` (Assuming index 2 is steam)

```text
Steam is already enabled
```

Or

```text
Steam has been enabled
```

### Disable

Examples of usage `StartupManager.exe disable Steam` or `StartupManager.exe d Steam` or `StartupManager.exe d 2` (Assuming index 2 is steam)

```text
Steam has been disabled
```

Or

```text
Steam is already disabled
```

### Add

The add command has a "wizard" that will guide you through the required steps, if you do not supply all the needed arguments when using the command.

#### Examples of usage

`StartupManager add MyWelcomeApp "C:\new folder\test.bat" "Hello to you sir!" false false` would output

```text
Added MyWelcomeApp to startup
```

`StartupManager add [Name] [Path] [Arguments] [RunAsAdministrator] [ForAllUsers]`

 `StartupManager add MyWelcomeApp` or  `StartupManager a MyWelcomeApp` would output this (Notice it doesn't ask for the name because it was supplied already)

 ```text
PS C:\> StartupManager add MyWelcomeApp
Let's guide you through settings up a new startup program

What's the path to the program?: C:\new folder\test.bat
What's the arguments for the program?: "Hello to you sir!"
Do you want to run this program as an Administrator? y/n: n
Do you want to run this program for all users? y/n: n

Name: MyWelcomeApp
Path: C:\new folder\test.bat
Arguments: "Hello to you sir!"
Administrator: False
All Users: False

Does this look correct? y/n: y
Added MyWelcomeApp to startup
 ```

### Remove

This command will remove a program from starting with windows by deleting the registry entry, task or shortcut.

Examples of usage

`StartupManager remove MyWelcomeApp` would output

```text
Are you sure you want to delete 'MyWelcomeApp' y/n: y
MyWelcomeApp has been removed
```

It's possible to skip the confirmation by adding the option `--confirm` or `-c`

`StartupManager remove MyWelcomeApp --confirm` or `StartupManager r MyWelcomeApp -c` would output

```text
MyWelcomeApp has been removed
```

### Set-Priority

This command allows you to set the priority level for Task Scheduler startup programs. The priority levels available are: Idle, BelowNormal, Normal, AboveNormal, High, or Realtime.

Examples of usage

`StartupManager set-priority MyWelcomeApp Normal` or `StartupManager p MyWelcomeApp Normal` would output

```text
Priority for MyWelcomeApp has been set to Normal
```

You can also use the index from the list command:

`StartupManager p 1 High` would set the priority of the program at index 1 to High.
