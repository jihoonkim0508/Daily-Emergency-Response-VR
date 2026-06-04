using System.Linq;
using UnityEngine;

namespace DailyEmergencyResponseVR
{
    public sealed class ScenarioManager : MonoBehaviour
    {
        [SerializeField] TrainingContentLibrary m_Library;
        [SerializeField] EmergencyDevice[] m_Devices = new EmergencyDevice[0];
        [SerializeField] RobotNpcController m_Npc;

        readonly ScenarioEvaluator m_Evaluator = new ScenarioEvaluator();

        public ScenarioData ActiveScenario => m_Evaluator.Scenario;
        public bool IsComplete => m_Evaluator.IsComplete;

        public void Configure(TrainingContentLibrary library, EmergencyDevice[] devices, RobotNpcController npc)
        {
            m_Library = library;
            m_Devices = devices;
            m_Npc = npc;
        }

        public void BeginScenario(ScenarioData scenario)
        {
            m_Evaluator.Begin(scenario);
            m_Npc?.PresentScenario(scenario);
            RefreshHighlights();
        }

        public ScenarioEvaluationResult TryOperate(EmergencyDevice device, InteractionType interactionType)
        {
            var result = m_Evaluator.Evaluate(device != null ? device.Data : null, interactionType);
            if (result == ScenarioEvaluationResult.WrongDevice || result == ScenarioEvaluationResult.WrongInteraction)
                m_Npc?.Say(ActiveScenario != null ? ActiveScenario.failHint : "현재 단계에 맞는 장치를 선택하세요.");

            RefreshHighlights();
            return result;
        }

        void RefreshHighlights()
        {
            foreach (var device in m_Devices.Where(device => device != null))
                device.SetHighlighted(ActiveScenario != null && device.Data != null && device.Data.id == m_Evaluator.CurrentDeviceId);
        }
    }
}
