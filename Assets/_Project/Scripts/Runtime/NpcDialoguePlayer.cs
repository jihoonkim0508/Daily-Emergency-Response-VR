using TMPro;
using UnityEngine;

namespace DailyEmergencyResponseVR
{
    public sealed class NpcDialoguePlayer : MonoBehaviour
    {
        [SerializeField] TMP_Text m_DialogueText;

        public void Say(string line)
        {
            if (m_DialogueText != null)
                m_DialogueText.text = line;
        }
    }
}
