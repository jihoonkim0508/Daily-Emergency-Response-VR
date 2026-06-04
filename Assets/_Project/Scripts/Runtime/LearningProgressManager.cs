using System.Collections.Generic;
using UnityEngine;

namespace DailyEmergencyResponseVR
{
    public sealed class LearningProgressManager : MonoBehaviour
    {
        readonly HashSet<string> m_CompletedDevices = new HashSet<string>();
        readonly HashSet<string> m_CompletedScenarios = new HashSet<string>();

        public int CompletedDeviceCount => m_CompletedDevices.Count;
        public int CompletedScenarioCount => m_CompletedScenarios.Count;

        public void MarkDeviceComplete(string deviceId)
        {
            if (!string.IsNullOrWhiteSpace(deviceId))
                m_CompletedDevices.Add(deviceId);
        }

        public void MarkScenarioComplete(string scenarioId)
        {
            if (!string.IsNullOrWhiteSpace(scenarioId))
                m_CompletedScenarios.Add(scenarioId);
        }

        public bool IsDeviceComplete(string deviceId)
        {
            return m_CompletedDevices.Contains(deviceId);
        }

        public bool IsScenarioComplete(string scenarioId)
        {
            return m_CompletedScenarios.Contains(scenarioId);
        }
    }
}
