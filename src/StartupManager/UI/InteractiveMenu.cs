using System;
using System.Linq;
using StartupManager.Commands.Add;
using StartupManager.Commands.Remove;
using StartupManager.Commands.SetPriority;
using StartupManager.Commands.StartupList;
using StartupManager.Commands.StartupToggle;
using StartupManager.ConsoleOutputters;

namespace StartupManager.UI {
    public static class InteractiveMenu {
        public static void Show() {
            while (true) {
                Console.Clear();
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Cyan, 
                    "╔════════════════════════════════════════════════════════════╗",
                    "║           Startup Manager - Interactive Menu              ║",
                    "╚════════════════════════════════════════════════════════════╝",
                    ""
                );

                // Display the startup list
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Yellow, "Current Startup Programs:");
                Console.WriteLine();
                ListCommand.Run(false);
                Console.WriteLine();

                // Display menu options
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Green, "Menu Options:");
                Console.WriteLine("  1. Add a new startup program");
                Console.WriteLine("  2. Remove a startup program");
                Console.WriteLine("  3. Enable a startup program");
                Console.WriteLine("  4. Disable a startup program");
                Console.WriteLine("  5. Set priority for a startup program");
                Console.WriteLine("  6. Refresh list");
                Console.WriteLine("  7. Show detailed list");
                Console.WriteLine("  0. Exit");
                Console.WriteLine();

                ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Cyan, "Enter your choice: ");
                var choice = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(choice)) {
                    continue;
                }

                switch (choice) {
                    case "1":
                        HandleAdd();
                        break;
                    case "2":
                        HandleRemove();
                        break;
                    case "3":
                        HandleEnable();
                        break;
                    case "4":
                        HandleDisable();
                        break;
                    case "5":
                        HandleSetPriority();
                        break;
                    case "6":
                        // Just refresh by continuing the loop
                        break;
                    case "7":
                        HandleDetailedList();
                        break;
                    case "0":
                        ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Green, "Goodbye!");
                        return;
                    default:
                        ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, "Invalid choice. Please try again.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void HandleAdd() {
            Console.Clear();
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Cyan, "Add New Startup Program");
            Console.WriteLine();
            AddCommand.Run(null, null, null, null, null, null);
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void HandleRemove() {
            Console.Clear();
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Cyan, "Remove Startup Program");
            Console.WriteLine();
            ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, "Enter the name or index of the program to remove: ");
            var name = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(name)) {
                RemoveCommand.Run(name, true);
            } else {
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, "Name or index cannot be empty.");
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void HandleEnable() {
            Console.Clear();
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Cyan, "Enable Startup Program");
            Console.WriteLine();
            ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, "Enter the name or index of the program to enable: ");
            var name = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(name)) {
                EnableCommand.Run(name);
            } else {
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, "Name or index cannot be empty.");
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void HandleDisable() {
            Console.Clear();
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Cyan, "Disable Startup Program");
            Console.WriteLine();
            ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, "Enter the name or index of the program to disable: ");
            var name = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(name)) {
                DisableCommand.Run(name);
            } else {
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, "Name or index cannot be empty.");
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void HandleSetPriority() {
            Console.Clear();
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Cyan, "Set Priority for Startup Program");
            Console.WriteLine();
            ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, "Enter the name or index of the program: ");
            var name = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(name)) {
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, "Name or index cannot be empty.");
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            ConsoleColorHelper.ConsoleWriteColored(ConsoleColor.Yellow, "Enter priority (Idle, BelowNormal, Normal, AboveNormal, High, Realtime): ");
            var priority = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(priority)) {
                SetPriorityCommand.Run(name, priority);
            } else {
                ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Red, "Priority cannot be empty.");
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void HandleDetailedList() {
            Console.Clear();
            ConsoleColorHelper.ConsoleWriteLineColored(ConsoleColor.Cyan, "Detailed Startup Programs List");
            Console.WriteLine();
            ListCommand.Run(true);
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
