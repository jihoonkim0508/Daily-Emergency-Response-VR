using UnityEngine;

namespace DailyEmergencyResponseVR
{
    [CreateAssetMenu(menuName = "Daily Emergency Response/Zone Data")]
    public sealed class ZoneData : ScriptableObject
    {
        public string id;
        public string displayName;
        public ZoneType zoneType;
        [TextArea] public string description;
        public string[] deviceIds = new string[0];
        public Color defaultColor = Color.gray;
        public Color selectedColor = Color.white;
    }
}
