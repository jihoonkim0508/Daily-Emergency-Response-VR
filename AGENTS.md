# Repository Guidelines

## Project Structure & Module Organization

This is a Unity VR/XR project using Unity `6000.3.11f1`. Core project files live in `Assets/`, package declarations in `Packages/`, and settings in `ProjectSettings/`. Main scenes are in `Assets/Scenes/` (`BasicScene.unity`, `SampleScene.unity`). Reusable VR template scripts are in `Assets/VRTemplateAssets/Scripts/`. Imported Unity samples live under `Assets/Samples/`; treat these as reference/vendor code unless intentionally customizing a sample. Project notes belong in `docs/`.

Do not commit generated Unity folders such as `Library/`, `Temp/`, `Logs/`, or `UserSettings/`.

## Build, Test, and Development Commands

Open the project with Unity Hub or the matching editor:

```powershell
Unity.exe -projectPath "c:\Dev\Unity\Daily Emergency Response VR"
```

Run EditMode tests:

```powershell
Unity.exe -batchmode -projectPath . -runTests -testPlatform EditMode -testResults TestResults-EditMode.xml -quit
```

Run PlayMode tests:

```powershell
Unity.exe -batchmode -projectPath . -runTests -testPlatform PlayMode -testResults TestResults-PlayMode.xml -quit
```

For builds, prefer Unity Build Profiles or scripted build methods in `Assets/Editor/` when added.

## Coding Style & Naming Conventions

Use Unity C# conventions: PascalCase for classes, methods, properties, and public fields; camelCase for locals and parameters; prefix private serialized fields with `m_` only when matching existing sample style. Use four-space indentation. Keep one `MonoBehaviour` per file and name the file after the class, for example `StepManager.cs`. Prefer serialized fields over scene lookups for inspector-configured dependencies.

## Testing Guidelines

Add tests under `Assets/Tests/EditMode/` or `Assets/Tests/PlayMode/` with matching `.asmdef` files. Name test files after the behavior under test, for example `StepManagerTests.cs`. Use EditMode tests for pure logic and PlayMode tests for scene, XR interaction, timing, and prefab behavior. Before a pull request, run relevant tests and confirm changed scenes enter Play Mode without console errors.

## Commit & Pull Request Guidelines

This checkout has no local `.git` history, so no repository-specific commit convention is available. Use concise imperative messages such as `Add emergency scenario step flow` or `Fix XR knob value clamping`.

Pull requests should include a short summary, testing performed, affected scenes or prefabs, and screenshots or short captures for visible VR/UI changes. Link related issues or `docs/` notes. Call out package, input action, or Project Settings changes because they can affect all scenes.

## Agent-Specific Instructions

Keep changes scoped to project-owned assets. Avoid modifying `Assets/Samples/` and `Library/PackageCache/` unless the task explicitly requires it. Preserve Unity `.meta` files whenever moving or renaming assets.
