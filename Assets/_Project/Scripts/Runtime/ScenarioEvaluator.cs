using System;
using System.Linq;

namespace DailyEmergencyResponseVR
{
    public enum ScenarioEvaluationResult
    {
        CorrectStep,
        WrongDevice,
        WrongInteraction,
        ScenarioComplete,
        NoActiveScenario
    }

    public sealed class ScenarioEvaluator
    {
        ScenarioData m_Scenario;
        int m_CurrentIndex;

        public ScenarioData Scenario => m_Scenario;
        public int CurrentIndex => m_CurrentIndex;
        public bool IsComplete => m_Scenario != null && m_CurrentIndex >= m_Scenario.orderedDeviceIds.Length;
        public string CurrentDeviceId => IsComplete || m_Scenario == null ? string.Empty : m_Scenario.orderedDeviceIds[m_CurrentIndex];

        public void Begin(ScenarioData scenario)
        {
            m_Scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));
            m_CurrentIndex = 0;
        }

        public ScenarioEvaluationResult Evaluate(EmergencyDeviceData device, InteractionType interactionType)
        {
            if (m_Scenario == null)
                return ScenarioEvaluationResult.NoActiveScenario;

            if (IsComplete)
                return ScenarioEvaluationResult.ScenarioComplete;

            if (device == null || device.id != CurrentDeviceId)
                return ScenarioEvaluationResult.WrongDevice;

            if (device.requiredInteractionType != interactionType)
                return ScenarioEvaluationResult.WrongInteraction;

            m_CurrentIndex++;
            return IsComplete ? ScenarioEvaluationResult.ScenarioComplete : ScenarioEvaluationResult.CorrectStep;
        }

        public static bool HasValidDeviceReferences(ScenarioData scenario, TrainingContentLibrary library)
        {
            if (scenario == null || library == null)
                return false;

            return scenario.relatedDeviceIds.Concat(scenario.orderedDeviceIds)
                .All(id => library.GetDevice(id) != null);
        }
    }
}
