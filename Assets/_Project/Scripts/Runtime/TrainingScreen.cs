using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DailyEmergencyResponseVR
{
    public sealed class TrainingScreen : MonoBehaviour
    {
        [SerializeField] TMP_Text m_TitleText;
        [SerializeField] TMP_Text m_BodyText;
        [SerializeField] Button m_PrimaryButton;
        [SerializeField] Button m_SecondaryButton;
        [SerializeField] Button m_BackButton;

        public void Show(string title, string body)
        {
            if (m_TitleText != null)
                m_TitleText.text = title;
            if (m_BodyText != null)
                m_BodyText.text = body;
        }

        public void ConfigurePrimary(string label, UnityAction action, bool interactable = true)
        {
            ConfigureButton(m_PrimaryButton, label, action, interactable);
        }

        public void ConfigureSecondary(string label, UnityAction action, bool interactable = true)
        {
            ConfigureButton(m_SecondaryButton, label, action, interactable);
        }

        public void ConfigureBack(string label, UnityAction action, bool interactable = true)
        {
            ConfigureButton(m_BackButton, label, action, interactable);
        }

        static void ConfigureButton(Button button, string label, UnityAction action, bool interactable)
        {
            if (button == null)
                return;

            button.gameObject.SetActive(!string.IsNullOrWhiteSpace(label));
            button.interactable = interactable;
            var text = button.GetComponentInChildren<TMP_Text>();
            if (text != null)
                text.text = label;

            button.onClick.RemoveAllListeners();
            if (action != null)
                button.onClick.AddListener(action);
        }
    }
}
