using UnityEngine;

namespace DailyEmergencyResponseVR
{
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] SceneFlowManager m_SceneFlowManager;

        public SceneFlowManager SceneFlowManager => m_SceneFlowManager;

        void Awake()
        {
            if (m_SceneFlowManager == null)
                m_SceneFlowManager = FindFirstObjectByType<SceneFlowManager>();
        }
    }
}
