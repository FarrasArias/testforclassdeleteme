// BuildAutomation.cs
// Put this file inside an `Editor` folder so it is excluded from builds.
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildAutomation
{
    /// <summary>
    /// Entry‑point invoked from the command line:
    ///   -buildTarget   StandaloneWindows64 | StandaloneOSX | StandaloneLinux64 | Android | WebGL
    ///   -devBuild      1 (development)  | 0 (final)
    ///   -outputPath    Custom build folder (optional)
    /// </summary>
    public static void PerformBuildFromCommandLine()
    {
        string[] args = Environment.GetCommandLineArgs();

        string buildTargetArg = GetCommandLineValue(args, "-buildTarget", "StandaloneWindows64");
        string devBuildArg = GetCommandLineValue(args, "-devBuild", "0");
        string outputRoot = GetCommandLineValue(args, "-outputPath", "./Builds");

        // ---------- Resolve options ----------
        BuildTarget target = (BuildTarget)Enum.Parse(typeof(BuildTarget), buildTargetArg, true);
        bool devBuild = devBuildArg == "1" || devBuildArg.Equals("true", StringComparison.OrdinalIgnoreCase);

        BuildOptions options = devBuild
            ? BuildOptions.Development | BuildOptions.AllowDebugging
            : BuildOptions.None;

        // Use all enabled scenes in Build Settings; if none, fall back to SampleScene.
        string[] scenes = EditorBuildSettings.scenes.Length > 0
            ? EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray()
            : new[] { "Assets/Scenes/SampleScene.unity" };

        string productName = Application.productName.Length > 0 ? Application.productName : "MyGame";
        string platformFolder = target.ToString();
        string buildFolder = Path.Combine(outputRoot, platformFolder, devBuild ? "Dev" : "Final");
        Directory.CreateDirectory(buildFolder);

        string locationPath = target switch
        {
            BuildTarget.StandaloneWindows64 => Path.Combine(buildFolder, productName + ".exe"),
            BuildTarget.StandaloneOSX => Path.Combine(buildFolder, productName + ".app"),
            BuildTarget.StandaloneLinux64 => Path.Combine(buildFolder, productName + ".x86_64"),
            BuildTarget.Android => Path.Combine(buildFolder, productName + ".apk"),
            BuildTarget.WebGL => buildFolder,                       // folder build
            _ => Path.Combine(buildFolder, productName)
        };

        BuildPlayerOptions bpo = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = locationPath,
            target = target,
            options = options
        };

        // ---------- Build ----------
        BuildReport report = BuildPipeline.BuildPlayer(bpo);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {summary.totalSize} bytes → {locationPath}");
        }
        else
        {
            Debug.LogError("Build failed");
            // non‑zero exit code so CI knows it failed
            EditorApplication.Exit(1);
        }
    }

    // ---------- Helpers ----------
    private static string GetCommandLineValue(string[] args, string name, string fallback)
    {
        int idx = Array.IndexOf(args, name);
        return (idx >= 0 && idx < args.Length - 1) ? args[idx + 1] : fallback;
    }
}
