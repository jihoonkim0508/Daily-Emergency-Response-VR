using UnityEngine;

namespace DailyEmergencyResponseVR
{
    [CreateAssetMenu(menuName = "Daily Emergency Response/Scenario Data")]
    public sealed class ScenarioData : ScriptableObject
    {
        public string id;
        public string displayName;
        public ScenarioType scenarioType;
        [TextArea] public string description;
        public string[] relatedDeviceIds = new string[0];
        public string[] orderedDeviceIds = new string[0];
        [TextArea] public string introNpcLine;
        [TextArea] public string successSummary;
        [TextArea] public string failHint;
    }
}
