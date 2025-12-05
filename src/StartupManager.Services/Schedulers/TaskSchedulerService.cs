using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32.TaskScheduler;
using StartupManager.Models;
using StartupManager.Services.Identity;

namespace StartupManager.Services.Schedulers {
    public class TaskSchedulerService : ITaskSchedulerService {
        private static IWindowsIdentityService WindowsIdentityService = new WindowsIdentityService();
        private const string TaskSchedulerFolder = "StartupManager";
        public StartupList? GetStartupByPredicate(Func<Task, bool> predicate) {
            var startupList = GetStartupList(predicate, GetFolderPredicate(false));
            if (startupList == null) {
                return null;
            }
            return startupList;
        }
        public StartupList? GetStartupByName(string name) {
            Func<Task, bool> namePredicate = x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && x.Definition.Triggers.Any(x => x.TriggerType == TaskTriggerType.Logon);
            return GetStartupByPredicate(namePredicate);
        }

        public IEnumerable<StartupList> GetStartupPrograms(bool includeWindows) {
            return GetAllTasks(includeWindows).Select(x => Convert(x)).ToList();
        }

        public void RemoveProgramFromStartup(string name) {
            using(var taskService = new TaskService()) {
                var task = taskService.FindTask(name);
                taskService.RootFolder.DeleteTask(task.Path);
            }
        }

        public StateChange ToggleStartupState(StartupList program, bool enable) {
            try {
                using(var taskService = new TaskService()) {
                    Func<Task, bool> namePredicate = x => x.Name.Equals(program.Name, StringComparison.OrdinalIgnoreCase) && x.Definition.Triggers.Any(x => x.TriggerType == TaskTriggerType.Logon);
                    var task = GetTaskFromFolder(TaskService.Instance.RootFolder, namePredicate, GetFolderPredicate(false)) !;

                    var isAlreadyTheRequestState = task.Enabled == enable;
                    if (isAlreadyTheRequestState) {
                        return StateChange.SameState;
                    } else {
                        task.Enabled = enable;
                        return StateChange.Success;
                    }
                }
            } catch (UnauthorizedAccessException) {
                return StateChange.Unauthorized;
            }
        }

        public StateChange SetPriority(StartupList program, ProcessPriority priority) {
            try {
                using(var taskService = new TaskService()) {
                    Func<Task, bool> namePredicate = x => x.Name.Equals(program.Name, StringComparison.OrdinalIgnoreCase) && x.Definition.Triggers.Any(x => x.TriggerType == TaskTriggerType.Logon);
                    var task = GetTaskFromFolder(TaskService.Instance.RootFolder, namePredicate, GetFolderPredicate(false)) !;

                    var currentPriority = ConvertPriorityFromTask(task.Definition.Settings.Priority);
                    if (currentPriority == priority) {
                        return StateChange.SameState;
                    } else {
                        var taskDef = task.Definition;
                        taskDef.Settings.Priority = ConvertPriority(priority);
                        taskService.RootFolder.RegisterTaskDefinition(task.Path, taskDef, TaskCreation.CreateOrUpdate, null, null, TaskLogonType.InteractiveToken);
                        return StateChange.Success;
                    }
                }
            } catch (UnauthorizedAccessException) {
                return StateChange.Unauthorized;
            }
        }

        public Task AddProgramToStartup(StartupProgram program) {
            using(var taskService = new TaskService()) {
                var currentUser = WindowsIdentityService.CurrentUser();

                var taskDef = taskService.NewTask();
                taskDef.RegistrationInfo.Author = $"StartupManager";
                taskDef.RegistrationInfo.Description = $"Runs {program.Name} ({program.File.FullName}) at logon of user {currentUser}";
                taskDef.Principal.RunLevel = TaskRunLevel.Highest;
                taskDef.Settings.ExecutionTimeLimit = TimeSpan.Zero;
                taskDef.Settings.StartWhenAvailable = true;
                taskDef.Settings.StopIfGoingOnBatteries = false;
                taskDef.Settings.Priority = ConvertPriority(program.Priority);

                var action = taskDef.Actions.Add(program.File.FullName, program.Arguments, program.File.DirectoryName);

                var logonTrigger = (LogonTrigger) taskDef.Triggers.AddNew(TaskTriggerType.Logon);
                logonTrigger.UserId = currentUser;

                return taskService.RootFolder.RegisterTaskDefinition($"{TaskSchedulerFolder}\\{program.Name}", taskDef);
            }
        }

        private StartupList? GetStartupList(Func<Task, bool> taskPredicate, Func<TaskFolder, bool> folderPredicate) {
            var task = GetTaskFromFolder(TaskService.Instance.RootFolder, taskPredicate, folderPredicate);
            if (task != null) {
                return Convert(task);
            }
            return null;
        }

        private Task? GetTaskFromFolder(TaskFolder folder, Func<Task, bool> taskPredicate, Func<TaskFolder, bool> folderPredicate) {
            Task? task = folder.Tasks.FirstOrDefault(taskPredicate);
            if (task == null) {
                foreach (var subFolder in folder.SubFolders.Where(folderPredicate)) {
                    task = GetTaskFromFolder(subFolder, taskPredicate, folderPredicate);
                    if (task != null) {
                        return task;
                    }
                }
            }
            return task;
        }

        private static IEnumerable<Task> GetAllTasks(bool includeWindows) {
            var predicate = GetFolderPredicate(includeWindows);

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

        private static Func<TaskFolder, bool> GetFolderPredicate(bool includeWindows) {
            if (includeWindows) {
                return x => true;
            } else {
                return x => !x.Name.Contains("Microsoft", StringComparison.OrdinalIgnoreCase);
            }
        }

        private static StartupList Convert(Task task) {
            return new StartupList(task.Name,
                path : string.Join($" | ", task.Definition.Actions.Select(a => GetPathFromAction(a))),
                requireAdministrator : true,
                disabled: !task.Enabled,
                type : StartupList.StartupType.TaskScheduler,
                allUsers : task.Definition.Triggers.Where(t => t.TriggerType == TaskTriggerType.Logon).All(x => ((LogonTrigger) x).UserId != null),
                priority : ConvertPriorityFromTask(task.Definition.Settings.Priority));
        }

        public static List<StartupList> GetStartupTaskScheduler(bool includeWindows) {

            var tasks = GetAllTasks(includeWindows);
            return tasks.Select(x => Convert(x))
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

        private static System.Diagnostics.ProcessPriorityClass ConvertPriority(ProcessPriority priority) {
            return priority switch {
                ProcessPriority.Idle => System.Diagnostics.ProcessPriorityClass.Idle,
                ProcessPriority.BelowNormal => System.Diagnostics.ProcessPriorityClass.BelowNormal,
                ProcessPriority.Normal => System.Diagnostics.ProcessPriorityClass.Normal,
                ProcessPriority.AboveNormal => System.Diagnostics.ProcessPriorityClass.AboveNormal,
                ProcessPriority.High => System.Diagnostics.ProcessPriorityClass.High,
                ProcessPriority.Realtime => System.Diagnostics.ProcessPriorityClass.RealTime,
                _ => System.Diagnostics.ProcessPriorityClass.Normal
            };
        }

        private static ProcessPriority? ConvertPriorityFromTask(System.Diagnostics.ProcessPriorityClass taskPriority) {
            return taskPriority switch {
                System.Diagnostics.ProcessPriorityClass.Idle => ProcessPriority.Idle,
                System.Diagnostics.ProcessPriorityClass.BelowNormal => ProcessPriority.BelowNormal,
                System.Diagnostics.ProcessPriorityClass.Normal => ProcessPriority.Normal,
                System.Diagnostics.ProcessPriorityClass.AboveNormal => ProcessPriority.AboveNormal,
                System.Diagnostics.ProcessPriorityClass.High => ProcessPriority.High,
                System.Diagnostics.ProcessPriorityClass.RealTime => ProcessPriority.Realtime,
                _ => null
            };
        }
    }
}