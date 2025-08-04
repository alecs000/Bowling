using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace AnAppTools
{
    public class SynchronizePackagesToolConfig : ToolConfigBase
    {
        #region Settings
        [Title(SettingsTitleName)]

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MaxSpaceValue)]
        [ToggleLeft]
        public bool ShowLogs;

        [PropertySpace(SpaceBefore = MaxSpaceValue, SpaceAfter = MinSpaceValue)]
        [ValidateInput(nameof(CheckNameOfPackages))]
        public List<string> NameOfPackagesToRemove = new()
        {
            "com.unity.ide.visualstudio",
            "com.unity.ide.rider",
            "com.unity.ai.navigation",
        };
        [PropertySpace(SpaceBefore = MinSpaceValue, SpaceAfter = MaxSpaceValue)]
        [ValidateInput(nameof(CheckNameOfPackages))]
        public List<string> NameOfPackagesToAdd = new()
        {
            "com.unity.recorder",
            "com.unity.nuget.newtonsoft-json",
            "com.unity.textmeshpro",
            "com.unity.ide.vscode",
            "com.unity.device-simulator.devices",
            "com.unity.purchasing",
            "com.unity.ads.ios-support",
            "com.unity.adaptiveperformance",
            "com.unity.adaptiveperformance.samsung.android",
            "com.unity.mobile.notifications",
        };
        #endregion

        public override async void Execute()
        {
            try
            {
                RemoveRequest removeRequest;
                AddRequest addRequest;
                TaskCompletionSource<bool> taskCompletionSource;

                StartProgress(NameOfPackagesToRemove.Count + NameOfPackagesToAdd.Count);

                foreach (var nameOfPackageToDelete in NameOfPackagesToRemove)
                {
                    removeRequest = Client.Remove(nameOfPackageToDelete);
                    taskCompletionSource = new TaskCompletionSource<bool>();

                    void action()
                    {
                        if (removeRequest.IsCompleted)
                        {
                            switch (removeRequest.Status)
                            {
                                case StatusCode.Success:
                                    if (ShowLogs)
                                        Debug.Log($"Removed [{nameOfPackageToDelete}] package");
                                    EditorApplication.update -= action;
                                    taskCompletionSource.SetResult(true);
                                    break;
                                case StatusCode.Failure:
                                    if (ShowLogs)
                                        Debug.Log(removeRequest.Error.message);
                                    EditorApplication.update -= action;
                                    taskCompletionSource.SetResult(false);
                                    break;
                            }
                        }
                    }
                    EditorApplication.update += action;

                    await taskCompletionSource.Task;
                    
                    AddProgress();
                }

                foreach (var nameOfPackageToLoad in NameOfPackagesToAdd)
                {
                    addRequest = Client.Add(nameOfPackageToLoad);
                    taskCompletionSource = new TaskCompletionSource<bool>();

                    void action()
                    {
                        if (addRequest.IsCompleted)
                        {
                            switch (addRequest.Status)
                            {
                                case StatusCode.Success:
                                    if (ShowLogs)
                                        Debug.Log($"Added [{nameOfPackageToLoad}] package");
                                    EditorApplication.update -= action;
                                    taskCompletionSource.SetResult(true);
                                    break;
                                case StatusCode.Failure:
                                    if (ShowLogs)
                                        Debug.Log(addRequest.Error.message);
                                    EditorApplication.update -= action;
                                    taskCompletionSource.SetResult(false);
                                    break;
                            }
                        }
                    }
                    EditorApplication.update += action;

                    await taskCompletionSource.Task;

                    AddProgress();
                }

                AssetDatabase.Refresh();
            }
            catch
            {
                throw new System.NotImplementedException();
            }
            finally
            {
                CompleteProgress();
            }
        }

        private bool CheckNameOfPackages(List<string> nameOfPackages)
        {
            void ResetDirecotryName(int index) => nameOfPackages[index] = NULL;

            for (int i = 0; i < nameOfPackages.Count; i++)
                if (nameOfPackages[i] == string.Empty)
                    ResetDirecotryName(i);
                else
                    for (int j = 0; j < nameOfPackages.Count; j++)
                    {
                        var canReset = i != j && 
                            nameOfPackages[i] == nameOfPackages[j] && 
                            nameOfPackages[i] != NULL;
                        if (canReset)
                            ResetDirecotryName(j);
                    }
            return true;
        }
    }
}
