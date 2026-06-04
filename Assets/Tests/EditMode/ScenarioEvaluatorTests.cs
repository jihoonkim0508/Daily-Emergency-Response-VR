using DailyEmergencyResponseVR;
using NUnit.Framework;
using UnityEngine;

public sealed class ScenarioEvaluatorTests
{
    [Test]
    public void Evaluate_AdvancesInCorrectOrder_ThenCompletes()
    {
        var call = Device("call", InteractionType.ButtonPress);
        var extinguisher = Device("extinguisher", InteractionType.GrabUse);
        var scenario = Scenario("test", "call", "extinguisher");
        var evaluator = new ScenarioEvaluator();

        evaluator.Begin(scenario);

        Assert.AreEqual(ScenarioEvaluationResult.CorrectStep, evaluator.Evaluate(call, InteractionType.ButtonPress));
        Assert.AreEqual("extinguisher", evaluator.CurrentDeviceId);
        Assert.AreEqual(ScenarioEvaluationResult.ScenarioComplete, evaluator.Evaluate(extinguisher, InteractionType.GrabUse));
        Assert.IsTrue(evaluator.IsComplete);
    }

    [Test]
    public void Evaluate_WrongSelection_DoesNotAdvance()
    {
        var call = Device("call", InteractionType.ButtonPress);
        var extinguisher = Device("extinguisher", InteractionType.GrabUse);
        var scenario = Scenario("test", "call", "extinguisher");
        var evaluator = new ScenarioEvaluator();

        evaluator.Begin(scenario);

        Assert.AreEqual(ScenarioEvaluationResult.WrongDevice, evaluator.Evaluate(extinguisher, InteractionType.GrabUse));
        Assert.AreEqual("call", evaluator.CurrentDeviceId);
        Assert.AreEqual(ScenarioEvaluationResult.WrongInteraction, evaluator.Evaluate(call, InteractionType.GrabUse));
        Assert.AreEqual("call", evaluator.CurrentDeviceId);
    }

    [Test]
    public void TrainingContentLibrary_FindsDuplicateIds()
    {
        var library = ScriptableObject.CreateInstance<TrainingContentLibrary>();
        library.devices = new[] { Device("duplicate", InteractionType.ButtonPress), Device("duplicate", InteractionType.GrabUse) };

        CollectionAssert.Contains(library.FindDuplicateIds(), "duplicate");
    }

    static EmergencyDeviceData Device(string id, InteractionType interactionType)
    {
        var device = ScriptableObject.CreateInstance<EmergencyDeviceData>();
        device.id = id;
        device.requiredInteractionType = interactionType;
        return device;
    }

    static ScenarioData Scenario(string id, params string[] orderedDeviceIds)
    {
        var scenario = ScriptableObject.CreateInstance<ScenarioData>();
        scenario.id = id;
        scenario.orderedDeviceIds = orderedDeviceIds;
        return scenario;
    }
}
