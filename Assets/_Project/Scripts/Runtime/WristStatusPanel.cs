using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DailyEmergencyResponseVR
{
    public sealed class WristStatusPanel : MonoBehaviour
    {
        [SerializeField] TMP_Text m_StateText;
        [SerializeField] TMP_Text m_ActionText;
        [SerializeField] Button m_BackButton;
        [SerializeField] Button m_ReplayButton;

        public void ShowStatus(string stateText, string actionText)
        {
            if (m_StateText != null)
                m_StateText.text = stateText;
            if (m_ActionText != null)
                m_ActionText.text = actionText;
        }

        public void ConfigureActions(UnityAction backAction, UnityAction replayAction)
        {
            ConfigureButton(m_BackButton, backAction);
            ConfigureButton(m_ReplayButton, replayAction);
        }

        static void ConfigureButton(Button button, UnityAction action)
        {
            if (button == null)
                return;

            button.onClick.RemoveAllListeners();
            if (action != null)
                button.onClick.AddListener(action);
        }
    }
}
