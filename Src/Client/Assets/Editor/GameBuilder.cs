using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class GameBuilder : MonoBehaviour
{
    [MenuItem("Build/Build macOS")]
    public static void PerformMacOSBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { 
            "Assets/Levels/Loading.unity",
            "Assets/Levels/CharSelect.unity",
            "Assets/Levels/MainCity.unity",
            "Assets/Levels/Map01.unity",
            "Assets/Levels/Map02.unity",
            "Assets/Levels/Map03.unity"
        };
        buildPlayerOptions.locationPathName = "build/macOS/CharacterControllerDemo.app";
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log(message: "Build succeeded: " + summary.totalSize + "bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log(message: "Build failed");
        }
    }

    [MenuItem("Build/Build Windows")]
    public static void PerformWindowsBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] {
            "Assets/Levels/Loading.unity",
            "Assets/Levels/CharSelect.unity",
            "Assets/Levels/MainCity.unity",
            "Assets/Levels/Map01.unity",
            "Assets/Levels/Map02.unity",
            "Assets/Levels/Map03.unity"
        };
        buildPlayerOptions.locationPathName = "build/Windows/FantasyRPGDemo.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log(message: "Build succeeded: " + summary.totalSize + "bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log(message: "Build failed");
        }
    }

    [MenuItem("Build/Build WebGL")]
    public static void PerformWebGLBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] {
            "Assets/Levels/Loading.unity",
            "Assets/Levels/CharSelect.unity",
            "Assets/Levels/MainCity.unity",
            "Assets/Levels/Map01.unity",
            "Assets/Levels/Map02.unity",
            "Assets/Levels/Map03.unity"
        };
        buildPlayerOptions.locationPathName = "build/WebGL";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log(message: "Build succeeded: " + summary.totalSize + "bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log(message: "Build failed");
        }
    }
}