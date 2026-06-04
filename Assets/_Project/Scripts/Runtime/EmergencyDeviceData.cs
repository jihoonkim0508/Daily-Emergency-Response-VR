using UnityEngine;

namespace DailyEmergencyResponseVR
{
    [CreateAssetMenu(menuName = "Daily Emergency Response/Device Data")]
    public sealed class EmergencyDeviceData : ScriptableObject
    {
        public string id;
        public string displayName;
        public ZoneType zoneType;
        public DeviceType deviceType;
        [TextArea] public string description;
        [TextArea] public string locationText;
        [TextArea] public string useSituation;
        [TextArea] public string useMethod;
        public InteractionType requiredInteractionType;
        [TextArea] public string successMessage;
        [TextArea] public string failMessage;
    }
}
