using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Win32.TaskScheduler;
using StartupManager.Commands.Add;
using StartupManager.Commands.StartupList;

namespace StartupManager.Helpers {
    public static class TaskSchedulerHelper {
        public static TaskDefinition AddProgramToStartup(StartupProgram program) {
            using(var taskService = new TaskService()) {
                var taskDef = taskService.NewTask();
                taskDef.RegistrationInfo.Author = $"StartupManager";
                taskDef.RegistrationInfo.Description = $"Runs {program.Name} ({program.File.FullName}) at logon of user {WindowsIdentityHelper.CurrentUser()}";
                taskDef.Principal.RunLevel = TaskRunLevel.Highest;
                taskDef.Settings.ExecutionTimeLimit = TimeSpan.Zero;
                taskDef.Settings.StartWhenAvailable = true;
                taskDef.Settings.StopIfGoingOnBatteries = false;

                var action = taskDef.Actions.Add(program.File.FullName, program.Arguments, program.File.DirectoryName);

                var logonTrigger = (LogonTrigger)taskDef.Triggers.AddNew(TaskTriggerType.Logon);
                logonTrigger.UserId = WindowsIdentityHelper.CurrentUser();

                var valid = taskDef.Validate();
                return taskDef;
            }
        }

        public static List<ListProgram> GetStartupTaskScheduler(bool showWindows) {
            using(var taskService = new TaskService()) {
                var predicate = GetTaskPredicate(showWindows);
                var tasks = taskService.FindAllTasks(predicate);
                return tasks.Select(x =>
                        new ListProgram(x.Name,
                            path : string.Join($" | ", x.Definition.Actions.Select(a => GetPathFromAction(a))),
                            requireAdministrator : true,
                            disabled: !x.Enabled,
                            type : ListProgram.StartupType.TaskScheduler,
                            allUsers : x.Definition.Triggers.Where(t => t.TriggerType == TaskTriggerType.Logon).All(x => ((LogonTrigger)x).UserId != null)))
                    .ToList();
            }
        }

        private static string GetPathFromAction(Microsoft.Win32.TaskScheduler.Action action) {
            switch (action) {
                case ExecAction act:
                    return $"{act.Path} {act.Arguments}";
                case EmailAction act:
                    return $"Email To: {act.To} From: {act.From} Subject: {act.Subject}";
                case ComHandlerAction act:
                    return $"{act.ActionType} - {act}";
                case ShowMessageAction act:
                    return $"{act.ActionType} - {act}";
                default:
                    return $"Unknown action type: '{action.ActionType}'";
            }
        }

        private static Predicate<Task> GetTaskPredicate(bool includeWindows) {
            if (includeWindows) {
                return new Predicate<Task>(x => x.Definition.Triggers.Any(x => x.TriggerType == TaskTriggerType.Logon));
            } else {
                return new Predicate<Task>(x => x.Definition.Triggers.Any(x => x.TriggerType == TaskTriggerType.Logon) && x.Definition.RegistrationInfo.Author != "Microsoft Corporation" && !x.Path.Contains(@"\Microsoft\"));
            }
        }

        public static Task RegisterTask(TaskDefinition taskDef, StartupProgram program) {
            using(var taskService = new TaskService()) {
                return taskService.RootFolder.RegisterTaskDefinition($"StartupManager\\{program.Name}", taskDef);
            }
        }
    }
}