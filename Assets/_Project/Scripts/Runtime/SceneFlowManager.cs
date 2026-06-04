using System.Linq;
using UnityEngine;

namespace DailyEmergencyResponseVR
{
    public sealed class SceneFlowManager : MonoBehaviour
    {
        [SerializeField] TrainingContentLibrary m_Library;
        [SerializeField] TrainingScreen m_CentralScreen;
        [SerializeField] WristStatusPanel m_WristPanel;
        [SerializeField] RobotNpcController m_Npc;
        [SerializeField] LearningProgressManager m_Progress;
        [SerializeField] ScenarioManager m_ScenarioManager;
        [SerializeField] EmergencyDevice[] m_Devices = new EmergencyDevice[0];
        [SerializeField] ZoneDisplay[] m_Zones = new ZoneDisplay[0];

        TrainingFlowState m_State = TrainingFlowState.Start;
        EmergencyDeviceData m_ActiveDevice;

        public TrainingFlowState State => m_State;
        public TrainingContentLibrary ContentLibrary => m_Library;

        void Awake()
        {
            if (m_Progress == null)
                m_Progress = GetComponent<LearningProgressManager>();

            if (m_ScenarioManager != null)
                m_ScenarioManager.Configure(m_Library, m_Devices, m_Npc);

            foreach (var device in m_Devices.Where(device => device != null))
            {
                device.Selected += OnDeviceSelected;
                device.Operated += OnDeviceOperated;
            }
        }

        void Start()
        {
            ShowStart();
        }

        void OnDestroy()
        {
            foreach (var device in m_Devices.Where(device => device != null))
            {
                device.Selected -= OnDeviceSelected;
                device.Operated -= OnDeviceOperated;
            }
        }

        public void ShowStart()
        {
            m_State = TrainingFlowState.Start;
            m_Npc?.PresentStart();
            m_CentralScreen?.Show("지하철 비상장치 교육관", "교육관 MVP입니다. 장치 학습과 상황 연계 학습을 진행할 수 있습니다.");
            m_CentralScreen?.ConfigurePrimary("시작", ShowModeSelect);
            m_CentralScreen?.ConfigureSecondary(string.Empty, null);
            m_CentralScreen?.ConfigureBack(string.Empty, null);
            m_WristPanel?.ShowStatus("시작", "중앙 스크린에서 시작을 선택하세요.");
            m_WristPanel?.ConfigureActions(ShowStart, ReplayCurrentExplanation);
            ClearHighlights();
        }

        public void ShowModeSelect()
        {
            m_State = TrainingFlowState.ModeSelect;
            m_CentralScreen?.Show("모드 선택", "교육관은 사용 가능합니다. 체험관은 MVP 범위 밖이라 잠겨 있습니다.");
            m_CentralScreen?.ConfigurePrimary("교육관", ShowLearningSelect);
            m_CentralScreen?.ConfigureSecondary("체험관 Locked", null, false);
            m_CentralScreen?.ConfigureBack("뒤로", ShowStart);
            m_WristPanel?.ShowStatus("모드 선택", "교육관을 선택하세요.");
        }

        public void ShowLearningSelect()
        {
            m_State = TrainingFlowState.LearningSelect;
            m_CentralScreen?.Show("학습 선택", "장치 학습은 8개 비상장치를 개별 조작합니다. 상황 연계 학습은 사고 상황에 맞는 장치를 순서대로 사용합니다.");
            m_CentralScreen?.ConfigurePrimary("장치 학습", StartDeviceLearning);
            m_CentralScreen?.ConfigureSecondary("상황 연계", StartScenarioLearning);
            m_CentralScreen?.ConfigureBack("뒤로", ShowModeSelect);
            m_WristPanel?.ShowStatus("학습 선택", "학습 모드를 선택하세요.");
            ClearHighlights();
        }

        public void StartDeviceLearning()
        {
            m_State = TrainingFlowState.DeviceLearning;
            m_ActiveDevice = null;
            m_CentralScreen?.Show("장치 학습", BuildDeviceList());
            m_CentralScreen?.ConfigurePrimary("학습 선택", ShowLearningSelect);
            m_CentralScreen?.ConfigureSecondary(string.Empty, null);
            m_CentralScreen?.ConfigureBack("뒤로", ShowLearningSelect);
            m_WristPanel?.ShowStatus("장치 학습", "주변의 장치를 직접 선택하고 조작하세요.");
            ClearHighlights();
        }

        public void StartScenarioLearning()
        {
            m_State = TrainingFlowState.ScenarioLearning;
            var firstScenario = m_Library != null ? m_Library.scenarios.FirstOrDefault() : null;
            if (firstScenario != null)
                BeginScenario(firstScenario);
        }

        public void BeginScenario(ScenarioData scenario)
        {
            if (scenario == null)
                return;

            m_State = TrainingFlowState.ScenarioLearning;
            m_ScenarioManager?.BeginScenario(scenario);
            m_CentralScreen?.Show(scenario.displayName, $"{scenario.description}\n\n{scenario.introNpcLine}\n\n필요 장치: {string.Join(", ", scenario.orderedDeviceIds)}");
            m_CentralScreen?.ConfigurePrimary("다음 상황", BeginNextScenario);
            m_CentralScreen?.ConfigureSecondary("학습 선택", ShowLearningSelect);
            m_CentralScreen?.ConfigureBack("뒤로", ShowLearningSelect);
            m_WristPanel?.ShowStatus("상황 연계", "번호와 하이라이트가 표시된 장치를 순서대로 조작하세요.");
        }

        public void BeginNextScenario()
        {
            if (m_Library == null || m_Library.scenarios.Length == 0)
                return;

            var current = m_ScenarioManager != null ? m_ScenarioManager.ActiveScenario : null;
            var index = current == null ? -1 : System.Array.IndexOf(m_Library.scenarios, current);
            BeginScenario(m_Library.scenarios[(index + 1) % m_Library.scenarios.Length]);
        }

        void OnDeviceSelected(EmergencyDevice device)
        {
            if (m_State != TrainingFlowState.DeviceLearning || device == null || device.Data == null)
                return;

            m_ActiveDevice = device.Data;
            m_Npc?.PresentDevice(m_ActiveDevice);
            HighlightZone(m_ActiveDevice.zoneType);
            device.SetHighlighted(true);
            m_CentralScreen?.Show(m_ActiveDevice.displayName, $"{m_ActiveDevice.description}\n\n위치: {m_ActiveDevice.locationText}\n\n사용 상황: {m_ActiveDevice.useSituation}\n\n조작: {m_ActiveDevice.useMethod}");
            m_WristPanel?.ShowStatus("장치 학습", $"{m_ActiveDevice.displayName} 조작을 완료하세요.");
        }

        void OnDeviceOperated(EmergencyDevice device, InteractionType interactionType)
        {
            if (device == null || device.Data == null)
                return;

            if (m_State == TrainingFlowState.DeviceLearning)
            {
                CompleteDeviceLearning(device.Data);
                return;
            }

            if (m_State != TrainingFlowState.ScenarioLearning || m_ScenarioManager == null)
                return;

            var result = m_ScenarioManager.TryOperate(device, interactionType);
            if (result == ScenarioEvaluationResult.ScenarioComplete)
                CompleteScenario();
            else if (result == ScenarioEvaluationResult.CorrectStep)
                m_WristPanel?.ShowStatus("상황 연계", "다음 하이라이트 장치를 조작하세요.");
        }

        void CompleteDeviceLearning(EmergencyDeviceData device)
        {
            if (device == null)
                return;

            m_Progress?.MarkDeviceComplete(device.id);
            m_State = TrainingFlowState.Summary;
            m_CentralScreen?.Show("장치 학습 완료", device.successMessage);
            m_CentralScreen?.ConfigurePrimary("다른 장치", StartDeviceLearning);
            m_CentralScreen?.ConfigureSecondary("학습 선택", ShowLearningSelect);
            m_CentralScreen?.ConfigureBack("뒤로", StartDeviceLearning);
            m_WristPanel?.ShowStatus("완료", $"{device.displayName} 완료. 누적 {m_Progress?.CompletedDeviceCount ?? 0}/8");
        }

        void CompleteScenario()
        {
            var scenario = m_ScenarioManager.ActiveScenario;
            if (scenario == null)
                return;

            m_Progress?.MarkScenarioComplete(scenario.id);
            m_State = TrainingFlowState.Summary;
            m_CentralScreen?.Show("상황 학습 완료", scenario.successSummary);
            m_CentralScreen?.ConfigurePrimary("다음 상황", BeginNextScenario);
            m_CentralScreen?.ConfigureSecondary("학습 선택", ShowLearningSelect);
            m_CentralScreen?.ConfigureBack("뒤로", ShowLearningSelect);
            m_WristPanel?.ShowStatus("완료", $"{scenario.displayName} 완료. 누적 {m_Progress?.CompletedScenarioCount ?? 0}/6");
        }

        void ReplayCurrentExplanation()
        {
            if (m_ActiveDevice != null)
                m_Npc?.PresentDevice(m_ActiveDevice);
            else if (m_ScenarioManager != null && m_ScenarioManager.ActiveScenario != null)
                m_Npc?.PresentScenario(m_ScenarioManager.ActiveScenario);
            else
                m_Npc?.PresentStart();
        }

        string BuildDeviceList()
        {
            if (m_Library == null)
                return "장치 데이터가 없습니다.";

            return string.Join("\n", m_Library.devices.Where(device => device != null).Select(device => $"- {device.displayName}: {device.locationText}"));
        }

        void HighlightZone(ZoneType zoneType)
        {
            foreach (var zone in m_Zones.Where(zone => zone != null))
                zone.ApplySelected(zone.Data != null && zone.Data.zoneType == zoneType);
        }

        void ClearHighlights()
        {
            foreach (var device in m_Devices.Where(device => device != null))
                device.SetHighlighted(false);
            foreach (var zone in m_Zones.Where(zone => zone != null))
                zone.ApplySelected(false);
        }
    }
}
