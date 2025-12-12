using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS
using AppleAuth.Editor;
using UnityEditor.iOS.Xcode;
#endif
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class CustomBuildProcessorForAppleAuth : IPostprocessBuildWithReport
{
    public int callbackOrder => 1000;

    public void OnPostprocessBuild(BuildReport report)
    {
        switch (report.summary.platform)
        {
            case BuildTarget.Android:
                AndroidPostProcessBuild(report.summary.outputPath);
                break;
            case BuildTarget.iOS:
                IosPostProcessBuild(report.summary.outputPath);
                break;
        }
    }

    private void IosPostProcessBuild(string pathToBuildProject)
    {
#if UNITY_IOS
        var projPath = PBXProject.GetPBXProjectPath(pathToBuildProject);
        var project = new PBXProject();
        project.ReadFromString(System.IO.File.ReadAllText(projPath));
        var manager = new ProjectCapabilityManager(projPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
        manager.AddSignInWithAppleWithCompatibility(project.GetUnityFrameworkTargetGuid());
        manager.WriteToFile();

#endif
    }

    private void AndroidPostProcessBuild(string pathToBuildProject)
    {
        Debug.Log($"pathToBuildProject = ${pathToBuildProject}");
    }
}
