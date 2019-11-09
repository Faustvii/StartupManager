using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.TaskScheduler;
using StartupManager.Commands.Add;
using StartupManager.Commands.StartupList;

namespace StartupManager.Helpers {
    public static class TaskSchedulerHelper {
        public static void RemoveProgramFromStartup(string name) {
            using(var taskService = new TaskService()) {
                var task = taskService.FindTask(name);
                taskService.RootFolder.DeleteTask(task.Path);
            }
        }

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

        private static IEnumerable<Task> GetAllTasks(bool includeWindows) {
            var predicate = GetPredicate(includeWindows);

            return GetFolderTasks(TaskService.Instance.RootFolder, predicate);
        }

        private static IEnumerable<Task> GetFolderTasks(TaskFolder folder, Func<TaskFolder, bool> predicate) {
            var tasks = new List<Task>();
            tasks.AddRange(folder.Tasks.Where(x => x.Definition.Triggers.Any(x => x.TriggerType == TaskTriggerType.Logon)));

            foreach (TaskFolder subFolder in folder.SubFolders.Where(predicate)) {
                tasks.AddRange(GetFolderTasks(subFolder, predicate));
            }

            return tasks;
        }

        private static Func<TaskFolder, bool> GetPredicate(bool includeWindows) {
            if (includeWindows) {
                return x => true;
            } else {
                return x => !x.Name.Contains("Microsoft", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static List<ListProgram> GetStartupTaskScheduler(bool includeWindows) {

            var tasks = GetAllTasks(includeWindows);
            return tasks.Select(x =>
                    new ListProgram(x.Name,
                        path : string.Join($" | ", x.Definition.Actions.Select(a => GetPathFromAction(a))),
                        requireAdministrator : true,
                        disabled: !x.Enabled,
                        type : ListProgram.StartupType.TaskScheduler,
                        allUsers : x.Definition.Triggers.Where(t => t.TriggerType == TaskTriggerType.Logon).All(x => ((LogonTrigger)x).UserId != null)))
                .ToList();
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

        public static Task RegisterTask(TaskDefinition taskDef, StartupProgram program) {
            using(var taskService = new TaskService()) {
                return taskService.RootFolder.RegisterTaskDefinition($"StartupManager\\{program.Name}", taskDef);
            }
        }
    }
}