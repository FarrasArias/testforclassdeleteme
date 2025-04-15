
@echo off
REM ============================================================
REM build_pipeline.cmd
REM Builds a Unity 6 project and optionally runs tests.
REM
REM Usage:
REM   build_pipeline.cmd ProjectPath Platform BuildType RunTests
REM
REM   ProjectPath : Absolute path to the Unity project.
REM   Platform    : win | mac | linux | android | webgl
REM   BuildType   : dev | final
REM   RunTests    : test | notest
REM ============================================================
IF "%~4"=="" (
    ECHO Usage: build_pipeline.cmd ProjectPath Platform BuildType RunTests
    EXIT /B 1
)
SET PROJECT_PATH=%~1
SET PLATFORM=%~2
SET BUILDTYPE=%~3
SET RUNTESTS=%~4

REM ---- Map platform to Unity BuildTarget --------------------
SET BUILD_TARGET=
IF /I "%PLATFORM%"=="win"     SET BUILD_TARGET=StandaloneWindows64
IF /I "%PLATFORM%"=="mac"     SET BUILD_TARGET=StandaloneOSX
IF /I "%PLATFORM%"=="linux"   SET BUILD_TARGET=StandaloneLinux64
IF /I "%PLATFORM%"=="android" SET BUILD_TARGET=Android
IF /I "%PLATFORM%"=="webgl"   SET BUILD_TARGET=WebGL

IF "%BUILD_TARGET%"=="" (
    ECHO [ERROR] Unknown platform "%PLATFORM%".
    EXIT /B 1
)

REM ---- Dev / Final flag -------------------------------------
SET DEV_BUILD=0
IF /I "%BUILDTYPE%"=="dev" SET DEV_BUILD=1

REM ---- Unity installation path ------------------------------
SET UNITY_EXE=C:\Program Files\Unity\Hub\Editor\6000.0.43f1\Editor\Unity.exe

REM ---- Log files --------------------------------------------
SET LOG_DIR=%~dp0logs
IF NOT EXIST "%LOG_DIR%" mkdir "%LOG_DIR%"
SET TEST_LOG=%LOG_DIR%\tests_%PLATFORM%.txt
SET BUILD_LOG=%LOG_DIR%\build_%PLATFORM%.txt

ECHO [INFO] Settings: Platform=%BUILD_TARGET%  Dev=%DEV_BUILD%  RunTests=%RUNTESTS%
ECHO [INFO] Logs: %LOG_DIR%

REM ============================================================
REM OPTIONAL - Run Edit Mode + Play Mode Tests
REM ============================================================
REM Unity's Test Framework (UTF) is Unity's specific TestRunner 
REM and integration layer for NUnit. It understands how to find 
REM and run NUnit-style tests within a Unity project, provides 
REM Unity-specific testing features (like [UnityTest] and Play Mode testing), 
REM and offers both a GUI (Test Runner window) and command-line interface 
REM (via the Unity Editor executable with -runTests) for running these tests.

REM You write your tests using NUnit syntax, and Unity's Test Framework provides 
REM the tools to run those tests within the Unity environment (both in the Editor 
REM and in automated builds). The -runTests command in the Unity executable tells 
REM Unity's built-in TestRunner to find and execute your NUnit-style tests.

IF /I "%RUNTESTS%"=="test" (
    ECHO [INFO] Running Unity tests...
    "%UNITY_EXE%" ^
      -quit ^
      -batchmode ^
      -nographics ^
      -projectPath "%PROJECT_PATH%" ^
      -runTests ^
      -testPlatform EditMode,PlayMode ^
      -testResults "%LOG_DIR%\TestResults.xml" ^
      -logFile "%TEST_LOG%"

    IF ERRORLEVEL 1 (
        ECHO [ERROR] Tests failed.  See %TEST_LOG%
        EXIT /B 1
    )
    ECHO [INFO] Tests passed.
)

REM --------- Run tests + capture profiler  ----------
IF /I "%RUNTESTS%"=="test" (
    ECHO [INFO] Running Unity tests + profiler…
    "%UNITY_EXE%" ^
      -quit -batchmode -nographics ^
      -projectPath "%PROJECT_PATH%" ^
      -runTests -testPlatform EditMode,PlayMode ^
      -testResults "%LOG_DIR%\TestResults.xml" ^
      -profiler-enable ^
      -profiler-log-file "%PROFILE_LOG%" ^
      -profiler-capture-frame-count 300 ^  REM ~5 s @60 fps
      -logFile "%TEST_LOG%"
    IF ERRORLEVEL 1 (
        ECHO [ERROR] Tests failed – see logs
        EXIT /B 1
    )
)

REM ============================================================
REM 2) Build the project
REM ============================================================
ECHO [INFO] Building the project...
"%UNITY_EXE%" ^
  -quit ^
  -batchmode ^
  -nographics ^
  -projectPath "%PROJECT_PATH%" ^
  -executeMethod BuildAutomation.PerformBuildFromCommandLine ^
  -buildTarget %BUILD_TARGET% ^
  -devBuild %DEV_BUILD% ^
  -logFile "%BUILD_LOG%"


IF ERRORLEVEL 1 (
    ECHO [ERROR] Build failed.  See %BUILD_LOG%
    EXIT /B 1
)

ECHO [INFO] Build completed successfully!
EXIT /B 0