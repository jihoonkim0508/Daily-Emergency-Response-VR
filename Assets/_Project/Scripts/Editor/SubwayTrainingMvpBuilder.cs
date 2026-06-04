using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DailyEmergencyResponseVR;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using ProjectDeviceType = DailyEmergencyResponseVR.DeviceType;

public static class SubwayTrainingMvpBuilder
{
    const string ProjectRoot = "Assets/_Project";
    const string DataRoot = ProjectRoot + "/Data";
    const string SceneRoot = ProjectRoot + "/Scenes";
    const string PrefabRoot = ProjectRoot + "/Prefabs";
    const string MaterialRoot = ProjectRoot + "/Materials";
    const string MainScenePath = SceneRoot + "/SubwayTraining.unity";
    const string LibraryPath = DataRoot + "/TrainingContentLibrary.asset";

    [MenuItem("Daily Emergency Response/Build Subway Training MVP")]
    public static void Build()
    {
        EnsureFolders();

        var materials = CreateMaterials();
        var content = CreateContent();
        CreateDevicePrefabs(content, materials);
        CreateScene(content, materials);
        RegisterBuildScene();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Subway Training MVP content and scene generated.");
    }

    static void EnsureFolders()
    {
        foreach (var path in new[] { ProjectRoot, DataRoot, SceneRoot, PrefabRoot, MaterialRoot })
            EnsureFolder(path);
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        var parent = Path.GetDirectoryName(path).Replace('\\', '/');
        var name = Path.GetFileName(path);
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }

    static Dictionary<string, Material> CreateMaterials()
    {
        return new Dictionary<string, Material>
        {
            ["Platform"] = CreateMaterial("Platform", new Color(0.18f, 0.19f, 0.2f)),
            ["Rail"] = CreateMaterial("Rail", new Color(0.08f, 0.09f, 0.1f)),
            ["Screen"] = CreateMaterial("Screen", new Color(0.05f, 0.07f, 0.09f)),
            ["Accident"] = CreateMaterial("Accident Zone", new Color(0.18f, 0.42f, 0.76f)),
            ["Fire"] = CreateMaterial("Fire Zone", new Color(0.82f, 0.22f, 0.16f)),
            ["Evacuation"] = CreateMaterial("Evacuation Zone", new Color(0.18f, 0.62f, 0.36f)),
            ["Device"] = CreateMaterial("Device Body", new Color(0.88f, 0.9f, 0.92f)),
            ["Warning"] = CreateMaterial("Warning Yellow", new Color(1f, 0.86f, 0.22f)),
            ["Robot"] = CreateMaterial("Robot", new Color(0.72f, 0.78f, 0.84f))
        };
    }

    static Material CreateMaterial(string name, Color color)
    {
        var path = $"{MaterialRoot}/{name}.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        EditorUtility.SetDirty(material);
        return material;
    }

    static TrainingContentLibrary CreateContent()
    {
        var devices = new[]
        {
            Device("emergency-call", "비상통화장치", ZoneType.Accident, ProjectDeviceType.EmergencyCall, InteractionType.ConnectionConfirm, "역무원 또는 관제센터와 연결해 상황을 알리는 장치입니다.", "승강장 기둥과 대합실 벽면", "환자 발생, 사고 목격, 열차 내 비상 상황", "커버를 확인하고 호출 버튼을 눌러 위치와 상황을 말합니다.", "관제 연결 확인 완료. 위치와 상황을 또렷하게 전달했습니다.", "통화장치는 사고 상황에서 먼저 위치와 상태를 알릴 때 사용합니다."),
            Device("fire-extinguisher", "소화기", ZoneType.Fire, ProjectDeviceType.FireExtinguisher, InteractionType.GrabUse, "초기 화재를 진압하는 휴대용 장비입니다.", "승강장 벽면 소화기함", "작은 불꽃이나 초기 화재", "안전핀을 뽑고 노즐을 화점으로 향한 뒤 손잡이를 누릅니다.", "소화기 사용 절차 완료. 초기 화재 대응을 마쳤습니다.", "소화기는 작고 번지지 않은 불에만 사용하고 대피로를 확보합니다."),
            Device("aed", "AED", ZoneType.Accident, ProjectDeviceType.AED, InteractionType.CoverOpen, "심정지 의심 환자에게 사용하는 자동심장충격기입니다.", "역무실 근처와 승강장 안내 표지 주변", "의식과 호흡이 없는 응급 환자", "덮개를 열고 안내 음성 또는 화면 지시에 따라 패드를 부착합니다.", "AED 준비 완료. 안내에 따라 환자 처치를 시작합니다.", "응급 환자에게는 비상통화장치로 신고하고 AED를 준비합니다."),
            Device("emergency-door-release", "비상개폐장치", ZoneType.Evacuation, ProjectDeviceType.EmergencyDoorRelease, InteractionType.LeverPull, "비상 시 출입문 또는 게이트를 수동으로 여는 장치입니다.", "출입문 옆 보호 커버 안", "정전, 연기, 출입문 작동 불능", "보호 커버를 열고 레버를 지정 방향으로 당깁니다.", "비상개폐장치 조작 완료. 대피 경로를 확보했습니다.", "비상개폐장치는 안내에 따라 대피로 확보가 필요할 때 사용합니다."),
            Device("guide-light", "유도등", ZoneType.Evacuation, ProjectDeviceType.GuideLight, InteractionType.ButtonPress, "대피 방향을 알려주는 조명 표지입니다.", "천장과 비상구 상단", "연기 또는 정전으로 방향 확인이 어려울 때", "표지 방향을 확인하고 가장 가까운 비상구로 이동합니다.", "유도등 확인 완료. 대피 방향을 정확히 파악했습니다.", "연기 상황에서는 낮은 자세로 유도등 방향을 따라 이동합니다."),
            Device("fire-hydrant", "소화전", ZoneType.Fire, ProjectDeviceType.FireHydrant, InteractionType.CoverOpen, "큰 화재에 물을 공급하는 고정식 소방 설비입니다.", "승강장 벽면 소화전함", "번지는 화재 또는 소화기만으로 부족한 화재", "함을 열고 호스를 전개한 뒤 밸브를 돌립니다.", "소화전 준비 완료. 큰 화재 대응 절차를 확인했습니다.", "큰 화재는 즉시 신고하고 안전이 확보될 때만 소화전을 준비합니다."),
            Device("escalator-stop", "에스컬레이터 정지 버튼", ZoneType.Accident, ProjectDeviceType.EscalatorStopButton, InteractionType.ButtonPress, "에스컬레이터를 즉시 멈추는 비상 버튼입니다.", "에스컬레이터 상하부 난간 끝", "끼임, 넘어짐, 물건 걸림 사고", "빨간 정지 버튼을 강하게 누르고 주변 이용객을 멈춥니다.", "에스컬레이터 정지 완료. 추가 사고를 막았습니다.", "에스컬레이터 사고는 먼저 정지 버튼으로 움직임을 멈춥니다."),
            Device("train-emergency", "열차 비상장치", ZoneType.Accident, ProjectDeviceType.TrainEmergencyDevice, InteractionType.LeverPull, "열차 내부에서 승무원에게 비상 상황을 알리는 장치입니다.", "객실 출입문 옆 또는 연결부 주변", "객실 내 환자, 끼임, 폭력, 화재 징후", "커버를 열고 레버 또는 호출부를 조작해 승무원에게 알립니다.", "열차 비상장치 조작 완료. 승무원에게 상황이 전달됩니다.", "열차 내부 사고는 열차 비상장치로 승무원에게 즉시 알립니다.")
        };

        var scenarios = new[]
        {
            Scenario("medical-patient", "응급 환자", ScenarioType.MedicalPatient, "승강장에 쓰러진 환자를 발견했습니다.", new[] { "emergency-call", "aed" }, "로봇: 위치를 먼저 알리고 AED를 준비하세요.", "신고와 AED 준비를 순서대로 완료했습니다.", "응급 환자는 신고 후 AED 준비 순서가 기본입니다."),
            Scenario("escalator-accident", "에스컬레이터 사고", ScenarioType.EscalatorAccident, "에스컬레이터에서 이용객이 넘어졌습니다.", new[] { "escalator-stop", "emergency-call" }, "로봇: 움직임을 멈춘 뒤 도움을 요청하세요.", "정지 버튼과 비상통화 조치를 완료했습니다.", "에스컬레이터 사고는 정지 버튼이 먼저입니다."),
            Scenario("train-interior-accident", "열차 내부 사고", ScenarioType.TrainInteriorAccident, "객실 내부에서 비상 상황이 발생했습니다.", new[] { "train-emergency", "emergency-call" }, "로봇: 객실 비상장치로 승무원에게 알리고 역 도착 후 신고를 이어가세요.", "열차 내부 사고 전달 절차를 완료했습니다.", "열차 안에서는 열차 비상장치를 먼저 사용합니다."),
            Scenario("small-fire", "작은 불", ScenarioType.SmallFire, "승강장 휴지통에서 작은 불꽃이 보입니다.", new[] { "emergency-call", "fire-extinguisher" }, "로봇: 위치를 알리고 초기 진압을 시도하세요.", "초기 화재 신고와 소화기 사용을 완료했습니다.", "작은 불도 먼저 신고하고 안전하게 진압합니다."),
            Scenario("large-fire", "큰 불", ScenarioType.LargeFire, "불이 벽면으로 번지고 연기가 늘어납니다.", new[] { "emergency-call", "fire-hydrant", "guide-light" }, "로봇: 신고 후 안전하면 소화전을 확인하고 대피 방향을 확보하세요.", "큰 화재 대응과 대피 방향 확인을 완료했습니다.", "큰 불은 신고와 대피가 우선이며 소화전은 안전할 때만 확인합니다."),
            Scenario("smoke-evacuation", "연기 대피", ScenarioType.SmokeEvacuation, "통로에 연기가 차오르고 시야가 흐립니다.", new[] { "guide-light", "emergency-door-release", "emergency-call" }, "로봇: 유도등을 따라 대피로를 확보하고 상황을 알리세요.", "연기 대피 경로 확보와 신고를 완료했습니다.", "연기 상황에서는 유도등과 비상개폐장치로 대피 경로를 먼저 확보합니다.")
        };

        var zones = new[]
        {
            Zone("accident-zone", "사고 대응 존", ZoneType.Accident, "응급 환자, 에스컬레이터 사고, 열차 내부 사고 대응", new[] { "emergency-call", "aed", "escalator-stop", "train-emergency" }, new Color(0.12f, 0.24f, 0.38f), new Color(0.28f, 0.62f, 1f)),
            Zone("fire-zone", "화재 대응 존", ZoneType.Fire, "초기 화재와 큰 화재 대응", new[] { "fire-extinguisher", "fire-hydrant" }, new Color(0.38f, 0.13f, 0.11f), new Color(1f, 0.34f, 0.22f)),
            Zone("evacuation-zone", "대피/탈출 존", ZoneType.Evacuation, "연기 대피와 대피 경로 확보", new[] { "emergency-door-release", "guide-light" }, new Color(0.12f, 0.32f, 0.19f), new Color(0.24f, 0.82f, 0.44f))
        };

        var library = LoadOrCreateAsset<TrainingContentLibrary>(LibraryPath);
        library.devices = devices.Select(CreateOrUpdateDevice).ToArray();
        library.scenarios = scenarios.Select(CreateOrUpdateScenario).ToArray();
        library.zones = zones.Select(CreateOrUpdateZone).ToArray();
        EditorUtility.SetDirty(library);
        return library;
    }

    static EmergencyDeviceData Device(string id, string displayName, ZoneType zoneType, ProjectDeviceType deviceType, InteractionType interactionType, string description, string locationText, string useSituation, string useMethod, string successMessage, string failMessage)
    {
        var data = ScriptableObject.CreateInstance<EmergencyDeviceData>();
        data.id = id;
        data.displayName = displayName;
        data.zoneType = zoneType;
        data.deviceType = deviceType;
        data.requiredInteractionType = interactionType;
        data.description = description;
        data.locationText = locationText;
        data.useSituation = useSituation;
        data.useMethod = useMethod;
        data.successMessage = successMessage;
        data.failMessage = failMessage;
        return data;
    }

    static ScenarioData Scenario(string id, string displayName, ScenarioType scenarioType, string description, string[] orderedDeviceIds, string introNpcLine, string successSummary, string failHint)
    {
        var data = ScriptableObject.CreateInstance<ScenarioData>();
        data.id = id;
        data.displayName = displayName;
        data.scenarioType = scenarioType;
        data.description = description;
        data.relatedDeviceIds = orderedDeviceIds;
        data.orderedDeviceIds = orderedDeviceIds;
        data.introNpcLine = introNpcLine;
        data.successSummary = successSummary;
        data.failHint = failHint;
        return data;
    }

    static ZoneData Zone(string id, string displayName, ZoneType zoneType, string description, string[] deviceIds, Color defaultColor, Color selectedColor)
    {
        var data = ScriptableObject.CreateInstance<ZoneData>();
        data.id = id;
        data.displayName = displayName;
        data.zoneType = zoneType;
        data.description = description;
        data.deviceIds = deviceIds;
        data.defaultColor = defaultColor;
        data.selectedColor = selectedColor;
        return data;
    }

    static EmergencyDeviceData CreateOrUpdateDevice(EmergencyDeviceData source)
    {
        var asset = LoadOrCreateAsset<EmergencyDeviceData>($"{DataRoot}/Devices/{source.id}.asset");
        EditorUtility.CopySerialized(source, asset);
        EditorUtility.SetDirty(asset);
        return asset;
    }

    static ScenarioData CreateOrUpdateScenario(ScenarioData source)
    {
        var asset = LoadOrCreateAsset<ScenarioData>($"{DataRoot}/Scenarios/{source.id}.asset");
        EditorUtility.CopySerialized(source, asset);
        EditorUtility.SetDirty(asset);
        return asset;
    }

    static ZoneData CreateOrUpdateZone(ZoneData source)
    {
        var asset = LoadOrCreateAsset<ZoneData>($"{DataRoot}/Zones/{source.id}.asset");
        EditorUtility.CopySerialized(source, asset);
        EditorUtility.SetDirty(asset);
        return asset;
    }

    static T LoadOrCreateAsset<T>(string path) where T : ScriptableObject
    {
        EnsureFolder(Path.GetDirectoryName(path).Replace('\\', '/'));
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset != null)
            return asset;

        asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    static void CreateDevicePrefabs(TrainingContentLibrary content, Dictionary<string, Material> materials)
    {
        foreach (var device in content.devices)
        {
            var root = GameObject.CreatePrimitive(PrimitiveType.Cube);
            root.name = device.displayName;
            root.transform.localScale = GetDeviceScale(device.deviceType);
            root.GetComponent<Renderer>().sharedMaterial = materials["Device"];
            root.AddComponent<XRSimpleInteractable>();

            var emergencyDevice = root.AddComponent<EmergencyDevice>();
            var label = CreateWorldText("Label", device.displayName, root.transform, new Vector3(0f, 0.7f, 0f), 0.16f, TextAlignmentOptions.Center);
            SetSerialized(emergencyDevice, "m_Data", device);
            SetSerialized(emergencyDevice, "m_HighlightRenderers", new[] { root.GetComponent<Renderer>() });
            SetSerialized(emergencyDevice, "m_LabelText", label);

            var path = $"{PrefabRoot}/{device.id}.prefab";
            PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
        }
    }

    static Vector3 GetDeviceScale(ProjectDeviceType deviceType)
    {
        switch (deviceType)
        {
            case ProjectDeviceType.FireExtinguisher:
                return new Vector3(0.25f, 0.75f, 0.25f);
            case ProjectDeviceType.AED:
                return new Vector3(0.55f, 0.35f, 0.18f);
            case ProjectDeviceType.GuideLight:
                return new Vector3(0.9f, 0.28f, 0.12f);
            default:
                return new Vector3(0.45f, 0.45f, 0.18f);
        }
    }

    static void CreateScene(TrainingContentLibrary content, Dictionary<string, Material> materials)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "SubwayTraining";

        RenderSettings.ambientLight = new Color(0.35f, 0.37f, 0.4f);
        var lightObject = new GameObject("Directional Light");
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        CreateEnvironment(materials);
        var zones = CreateZones(content, materials);
        var devices = CreateDevices(content);
        var screen = CreateTrainingScreen();
        var wrist = CreateWristPanel();
        var npc = CreateRobotNpc(materials);
        var scenarioManager = new GameObject("Scenario Manager").AddComponent<ScenarioManager>();

        var flowObject = new GameObject("Game Manager");
        var progress = flowObject.AddComponent<LearningProgressManager>();
        var flow = flowObject.AddComponent<SceneFlowManager>();
        var gameManager = flowObject.AddComponent<GameManager>();

        SetSerialized(flow, "m_Library", content);
        SetSerialized(flow, "m_CentralScreen", screen);
        SetSerialized(flow, "m_WristPanel", wrist);
        SetSerialized(flow, "m_Npc", npc);
        SetSerialized(flow, "m_Progress", progress);
        SetSerialized(flow, "m_ScenarioManager", scenarioManager);
        SetSerialized(flow, "m_Devices", devices);
        SetSerialized(flow, "m_Zones", zones);
        SetSerialized(scenarioManager, "m_Library", content);
        SetSerialized(scenarioManager, "m_Devices", devices);
        SetSerialized(scenarioManager, "m_Npc", npc);
        SetSerialized(gameManager, "m_SceneFlowManager", flow);

        CreateXrOrigin();

        EditorSceneManager.SaveScene(scene, MainScenePath);
    }

    static void CreateEnvironment(Dictionary<string, Material> materials)
    {
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Central Platform";
        floor.transform.localScale = new Vector3(14f, 0.2f, 9f);
        floor.transform.position = new Vector3(0f, -0.1f, 0f);
        floor.GetComponent<Renderer>().sharedMaterial = materials["Platform"];

        var railA = GameObject.CreatePrimitive(PrimitiveType.Cube);
        railA.name = "Track Rail Left";
        railA.transform.localScale = new Vector3(14f, 0.08f, 0.12f);
        railA.transform.position = new Vector3(0f, 0.02f, -4.25f);
        railA.GetComponent<Renderer>().sharedMaterial = materials["Rail"];

        var railB = GameObject.CreatePrimitive(PrimitiveType.Cube);
        railB.name = "Track Rail Right";
        railB.transform.localScale = new Vector3(14f, 0.08f, 0.12f);
        railB.transform.position = new Vector3(0f, 0.02f, 4.25f);
        railB.GetComponent<Renderer>().sharedMaterial = materials["Rail"];
    }

    static ZoneDisplay[] CreateZones(TrainingContentLibrary content, Dictionary<string, Material> materials)
    {
        var result = new List<ZoneDisplay>();
        var positions = new Dictionary<ZoneType, Vector3>
        {
            [ZoneType.Accident] = new Vector3(-4.5f, 0.03f, 0f),
            [ZoneType.Fire] = new Vector3(0f, 0.03f, 0f),
            [ZoneType.Evacuation] = new Vector3(4.5f, 0.03f, 0f)
        };

        foreach (var zone in content.zones)
        {
            var pad = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pad.name = zone.displayName;
            pad.transform.position = positions[zone.zoneType];
            pad.transform.localScale = new Vector3(3.8f, 0.08f, 3.8f);
            var renderer = pad.GetComponent<Renderer>();
            renderer.sharedMaterial = materials[zone.zoneType.ToString()];
            var light = new GameObject("Zone Light").AddComponent<Light>();
            light.transform.SetParent(pad.transform);
            light.transform.localPosition = new Vector3(0f, 2.6f, 0f);
            light.type = LightType.Point;
            light.range = 4.5f;

            CreateWorldText("Zone Label", zone.displayName, pad.transform, new Vector3(0f, 0.15f, -1.55f), 0.22f, TextAlignmentOptions.Center);

            var display = pad.AddComponent<ZoneDisplay>();
            SetSerialized(display, "m_Data", zone);
            SetSerialized(display, "m_Renderers", new[] { renderer });
            SetSerialized(display, "m_ZoneLight", light);
            display.ApplySelected(false);
            result.Add(display);
        }

        return result.ToArray();
    }

    static EmergencyDevice[] CreateDevices(TrainingContentLibrary content)
    {
        var positions = new Dictionary<string, Vector3>
        {
            ["emergency-call"] = new Vector3(-5.4f, 0.65f, -1.1f),
            ["aed"] = new Vector3(-4.3f, 0.7f, 1.05f),
            ["escalator-stop"] = new Vector3(-3.6f, 0.55f, -1.2f),
            ["train-emergency"] = new Vector3(-5.2f, 0.65f, 1.2f),
            ["fire-extinguisher"] = new Vector3(-0.8f, 0.65f, -1.1f),
            ["fire-hydrant"] = new Vector3(0.8f, 0.75f, 1.05f),
            ["emergency-door-release"] = new Vector3(3.8f, 0.65f, -1.1f),
            ["guide-light"] = new Vector3(5.1f, 1.8f, 1.05f)
        };

        var devices = new List<EmergencyDevice>();
        foreach (var device in content.devices)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabRoot}/{device.id}.prefab");
            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            instance.name = device.displayName;
            instance.transform.position = positions[device.id];
            instance.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            devices.Add(instance.GetComponent<EmergencyDevice>());
        }

        return devices.ToArray();
    }

    static TrainingScreen CreateTrainingScreen()
    {
        var canvas = CreateCanvas("Central Screen", new Vector3(0f, 2.1f, 3.9f), Quaternion.Euler(0f, 180f, 0f), new Vector2(680f, 430f));
        var background = CreatePanel(canvas.transform, new Color(0.04f, 0.06f, 0.08f, 0.95f), new Vector2(680f, 430f), Vector2.zero);
        background.name = "Screen Background";

        var title = CreateUiText("Title", canvas.transform, "지하철 비상장치 교육관", 40, new Vector2(600f, 70f), new Vector2(0f, 150f), TextAlignmentOptions.Center);
        var body = CreateUiText("Body", canvas.transform, "", 24, new Vector2(600f, 210f), new Vector2(0f, 20f), TextAlignmentOptions.TopLeft);
        var primary = CreateButton("Primary Button", canvas.transform, "시작", new Vector2(-210f, -155f));
        var secondary = CreateButton("Secondary Button", canvas.transform, "체험관 Locked", new Vector2(0f, -155f));
        var back = CreateButton("Back Button", canvas.transform, "뒤로", new Vector2(210f, -155f));

        var screen = canvas.gameObject.AddComponent<TrainingScreen>();
        SetSerialized(screen, "m_TitleText", title);
        SetSerialized(screen, "m_BodyText", body);
        SetSerialized(screen, "m_PrimaryButton", primary);
        SetSerialized(screen, "m_SecondaryButton", secondary);
        SetSerialized(screen, "m_BackButton", back);
        return screen;
    }

    static WristStatusPanel CreateWristPanel()
    {
        var canvas = CreateCanvas("Wrist UI", new Vector3(0.45f, 1.15f, 0.9f), Quaternion.Euler(50f, 180f, 0f), new Vector2(360f, 220f));
        CreatePanel(canvas.transform, new Color(0.02f, 0.05f, 0.06f, 0.92f), new Vector2(360f, 220f), Vector2.zero);
        var state = CreateUiText("State", canvas.transform, "시작", 24, new Vector2(310f, 45f), new Vector2(0f, 70f), TextAlignmentOptions.Center);
        var action = CreateUiText("Action", canvas.transform, "중앙 스크린에서 시작을 선택하세요.", 18, new Vector2(310f, 80f), new Vector2(0f, 10f), TextAlignmentOptions.TopLeft);
        var back = CreateButton("Back", canvas.transform, "뒤로", new Vector2(-85f, -75f), new Vector2(130f, 45f));
        var replay = CreateButton("Replay", canvas.transform, "다시보기", new Vector2(85f, -75f), new Vector2(130f, 45f));

        var panel = canvas.gameObject.AddComponent<WristStatusPanel>();
        SetSerialized(panel, "m_StateText", state);
        SetSerialized(panel, "m_ActionText", action);
        SetSerialized(panel, "m_BackButton", back);
        SetSerialized(panel, "m_ReplayButton", replay);
        return panel;
    }

    static RobotNpcController CreateRobotNpc(Dictionary<string, Material> materials)
    {
        var root = new GameObject("Robot NPC");
        root.transform.position = new Vector3(-1.8f, 0f, 2.45f);

        var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Robot Body";
        body.transform.SetParent(root.transform);
        body.transform.localPosition = new Vector3(0f, 0.85f, 0f);
        body.transform.localScale = new Vector3(0.55f, 0.85f, 0.55f);
        body.GetComponent<Renderer>().sharedMaterial = materials["Robot"];

        var text = CreateWorldText("Dialogue", "지하철 비상장치 교육관입니다.", root.transform, new Vector3(0f, 1.85f, 0f), 0.18f, TextAlignmentOptions.Center);
        text.rectTransform.sizeDelta = new Vector2(4f, 1.2f);

        var dialogue = root.AddComponent<NpcDialoguePlayer>();
        SetSerialized(dialogue, "m_DialogueText", text);
        var npc = root.AddComponent<RobotNpcController>();
        SetSerialized(npc, "m_DialoguePlayer", dialogue);
        return npc;
    }

    static Canvas CreateCanvas(string name, Vector3 position, Quaternion rotation, Vector2 size)
    {
        var canvasObject = new GameObject(name);
        canvasObject.transform.position = position;
        canvasObject.transform.rotation = rotation;
        canvasObject.transform.localScale = Vector3.one * 0.0025f;

        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObject.AddComponent<GraphicRaycaster>();
        canvas.GetComponent<RectTransform>().sizeDelta = size;
        return canvas;
    }

    static Image CreatePanel(Transform parent, Color color, Vector2 size, Vector2 position)
    {
        var panel = new GameObject("Panel");
        panel.transform.SetParent(parent, false);
        var image = panel.AddComponent<Image>();
        image.color = color;
        var rect = panel.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return image;
    }

    static TMP_Text CreateUiText(string name, Transform parent, string text, int fontSize, Vector2 size, Vector2 position, TextAlignmentOptions alignment)
    {
        var textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        var tmp = textObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = alignment;
        tmp.enableWordWrapping = true;
        var rect = tmp.rectTransform;
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return tmp;
    }

    static Button CreateButton(string name, Transform parent, string label, Vector2 position)
    {
        return CreateButton(name, parent, label, position, new Vector2(180f, 52f));
    }

    static Button CreateButton(string name, Transform parent, string label, Vector2 position, Vector2 size)
    {
        var buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);
        var image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.14f, 0.2f, 0.25f, 1f);
        var button = buttonObject.AddComponent<Button>();
        var rect = buttonObject.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        var text = CreateUiText("Label", buttonObject.transform, label, 20, size, Vector2.zero, TextAlignmentOptions.Center);
        text.raycastTarget = false;
        return button;
    }

    static TMP_Text CreateWorldText(string name, string text, Transform parent, Vector3 localPosition, float fontSize, TextAlignmentOptions alignment)
    {
        var textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);
        textObject.transform.localPosition = localPosition;
        textObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        var tmp = textObject.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = alignment;
        tmp.enableWordWrapping = true;
        tmp.rectTransform.sizeDelta = new Vector2(2.2f, 0.8f);
        return tmp;
    }

    static void CreateXrOrigin()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRTemplateAssets/Prefabs/Setup/Complete XR Origin Set Up Variant.prefab");
        if (prefab != null)
        {
            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            instance.name = "XR Origin (XR Rig)";
            instance.transform.position = new Vector3(0f, 0f, -2.2f);
            return;
        }

        var fallback = new GameObject("XR Origin (XR Rig)");
        fallback.AddComponent<Unity.XR.CoreUtils.XROrigin>();
        var camera = new GameObject("Main Camera");
        camera.transform.SetParent(fallback.transform);
        camera.tag = "MainCamera";
        camera.AddComponent<Camera>();
    }

    static void SetSerialized(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
    {
        var serializedObject = new SerializedObject(target);
        serializedObject.FindProperty(propertyName).objectReferenceValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    static void SetSerialized(UnityEngine.Object target, string propertyName, UnityEngine.Object[] values)
    {
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty(propertyName);
        property.arraySize = values.Length;
        for (var i = 0; i < values.Length; i++)
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    static void RegisterBuildScene()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.path != MainScenePath)
            .ToList();

        scenes.Insert(0, new EditorBuildSettingsScene(MainScenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
