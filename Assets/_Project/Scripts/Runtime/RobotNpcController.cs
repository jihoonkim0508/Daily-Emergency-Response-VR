using UnityEngine;

namespace DailyEmergencyResponseVR
{
    public sealed class RobotNpcController : MonoBehaviour
    {
        [SerializeField] NpcDialoguePlayer m_DialoguePlayer;

        public void PresentStart()
        {
            Say("지하철 비상장치 교육관입니다. 교육관에서 장치와 상황 대응을 순서대로 연습합니다.");
        }

        public void PresentDevice(EmergencyDeviceData device)
        {
            if (device != null)
                Say($"{device.displayName}: {device.useMethod}");
        }

        public void PresentScenario(ScenarioData scenario)
        {
            if (scenario != null)
                Say(scenario.introNpcLine);
        }

        public void Say(string line)
        {
            if (m_DialoguePlayer != null)
                m_DialoguePlayer.Say(line);
        }
    }
}
