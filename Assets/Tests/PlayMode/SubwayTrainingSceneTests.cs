using System.Collections;
using System.Linq;
using DailyEmergencyResponseVR;
using NUnit.Framework;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class SubwayTrainingSceneTests
{
    const string ScenePath = "Assets/_Project/Scenes/SubwayTraining.unity";

    [UnityTest]
    public IEnumerator MainScene_LoadsRequiredTrainingObjects()
    {
        yield return SceneManager.LoadSceneAsync(ScenePath, LoadSceneMode.Single);

        Assert.IsNotNull(Object.FindFirstObjectByType<XROrigin>(), "XR Origin is required.");
        Assert.AreEqual(3, Object.FindObjectsByType<ZoneDisplay>(FindObjectsSortMode.None).Length);
        Assert.AreEqual(8, Object.FindObjectsByType<EmergencyDevice>(FindObjectsSortMode.None).Length);

        Assert.IsNotNull(Object.FindFirstObjectByType<SceneFlowManager>());
    }

    [UnityTest]
    public IEnumerator AllScenarioDeviceReferences_AreValid()
    {
        yield return SceneManager.LoadSceneAsync(ScenePath, LoadSceneMode.Single);

        var content = Object.FindFirstObjectByType<SceneFlowManager>()?.ContentLibrary;
        Assert.IsNotNull(content, "Training content library asset must be loaded by the scene.");
        Assert.AreEqual(8, content.devices.Length);
        Assert.AreEqual(6, content.scenarios.Length);
        Assert.IsEmpty(content.FindDuplicateIds());

        foreach (var scenario in content.scenarios)
            Assert.IsTrue(ScenarioEvaluator.HasValidDeviceReferences(scenario, content), scenario.id);
    }

    [UnityTest]
    public IEnumerator EveryScenario_CompletesWithOrderedSequence()
    {
        yield return SceneManager.LoadSceneAsync(ScenePath, LoadSceneMode.Single);

        var content = Object.FindFirstObjectByType<SceneFlowManager>()?.ContentLibrary;
        Assert.IsNotNull(content);

        foreach (var scenario in content.scenarios)
        {
            var evaluator = new ScenarioEvaluator();
            evaluator.Begin(scenario);

            ScenarioEvaluationResult result = ScenarioEvaluationResult.NoActiveScenario;
            foreach (var deviceId in scenario.orderedDeviceIds)
            {
                var device = content.GetDevice(deviceId);
                result = evaluator.Evaluate(device, device.requiredInteractionType);
            }

            Assert.AreEqual(ScenarioEvaluationResult.ScenarioComplete, result, scenario.id);
            Assert.IsTrue(evaluator.IsComplete, scenario.id);
        }
    }
}
