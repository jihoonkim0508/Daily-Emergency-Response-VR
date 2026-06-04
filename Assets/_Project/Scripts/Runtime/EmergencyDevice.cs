using System;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace DailyEmergencyResponseVR
{
    [RequireComponent(typeof(Collider))]
    public sealed class EmergencyDevice : MonoBehaviour
    {
        [SerializeField] EmergencyDeviceData m_Data;
        [SerializeField] Renderer[] m_HighlightRenderers = new Renderer[0];
        [SerializeField] TMP_Text m_LabelText;

        bool m_IsHighlighted;

        public EmergencyDeviceData Data => m_Data;
        public event Action<EmergencyDevice, InteractionType> Operated;
        public event Action<EmergencyDevice> Selected;

        void Awake()
        {
            var simpleInteractable = GetComponent<XRSimpleInteractable>();
            if (simpleInteractable != null)
            {
                simpleInteractable.selectEntered.AddListener(OnSelectEntered);
                simpleInteractable.activated.AddListener(OnActivated);
            }

            RefreshLabel();
            ApplyHighlight(false);
        }

        void OnDestroy()
        {
            var simpleInteractable = GetComponent<XRSimpleInteractable>();
            if (simpleInteractable != null)
            {
                simpleInteractable.selectEntered.RemoveListener(OnSelectEntered);
                simpleInteractable.activated.RemoveListener(OnActivated);
            }
        }

        public void SetData(EmergencyDeviceData data)
        {
            m_Data = data;
            RefreshLabel();
        }

        public void SetHighlighted(bool highlighted)
        {
            m_IsHighlighted = highlighted;
            ApplyHighlight(highlighted);
        }

        public void SimulateOperate()
        {
            Operated?.Invoke(this, m_Data != null ? m_Data.requiredInteractionType : InteractionType.ButtonPress);
        }

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            Selected?.Invoke(this);
            Operated?.Invoke(this, m_Data != null ? m_Data.requiredInteractionType : InteractionType.ButtonPress);
        }

        void OnActivated(ActivateEventArgs args)
        {
            Operated?.Invoke(this, m_Data != null ? m_Data.requiredInteractionType : InteractionType.ButtonPress);
        }

        void RefreshLabel()
        {
            if (m_LabelText != null && m_Data != null)
                m_LabelText.text = m_Data.displayName;
        }

        void ApplyHighlight(bool highlighted)
        {
            var color = highlighted ? new Color(1f, 0.86f, 0.22f) : Color.white;
            foreach (var targetRenderer in m_HighlightRenderers)
            {
                if (targetRenderer != null)
                    targetRenderer.material.color = color;
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = m_IsHighlighted ? Color.yellow : Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.35f);
        }
    }
}
