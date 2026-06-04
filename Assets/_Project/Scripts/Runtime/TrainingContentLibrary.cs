using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DailyEmergencyResponseVR
{
    [CreateAssetMenu(menuName = "Daily Emergency Response/Training Content Library")]
    public sealed class TrainingContentLibrary : ScriptableObject
    {
        public EmergencyDeviceData[] devices = new EmergencyDeviceData[0];
        public ScenarioData[] scenarios = new ScenarioData[0];
        public ZoneData[] zones = new ZoneData[0];

        public EmergencyDeviceData GetDevice(string id)
        {
            return devices.FirstOrDefault(device => device != null && device.id == id);
        }

        public ScenarioData GetScenario(string id)
        {
            return scenarios.FirstOrDefault(scenario => scenario != null && scenario.id == id);
        }

        public ZoneData GetZone(ZoneType zoneType)
        {
            return zones.FirstOrDefault(zone => zone != null && zone.zoneType == zoneType);
        }

        public string[] FindDuplicateIds()
        {
            var ids = new List<string>();
            ids.AddRange(devices.Where(item => item != null).Select(item => item.id));
            ids.AddRange(scenarios.Where(item => item != null).Select(item => item.id));
            ids.AddRange(zones.Where(item => item != null).Select(item => item.id));

            return ids.GroupBy(id => id)
                .Where(group => string.IsNullOrWhiteSpace(group.Key) || group.Count() > 1)
                .Select(group => group.Key)
                .ToArray();
        }
    }
}
