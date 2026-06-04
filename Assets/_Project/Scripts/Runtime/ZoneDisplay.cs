using UnityEngine;

namespace DailyEmergencyResponseVR
{
    public sealed class ZoneDisplay : MonoBehaviour
    {
        [SerializeField] ZoneData m_Data;
        [SerializeField] Renderer[] m_Renderers = new Renderer[0];
        [SerializeField] Light m_ZoneLight;

        public ZoneData Data => m_Data;

        public void SetData(ZoneData data)
        {
            m_Data = data;
            ApplySelected(false);
        }

        public void ApplySelected(bool selected)
        {
            if (m_Data == null)
                return;

            var color = selected ? m_Data.selectedColor : m_Data.defaultColor;
            foreach (var targetRenderer in m_Renderers)
            {
                if (targetRenderer != null)
                    targetRenderer.material.color = color;
            }

            if (m_ZoneLight != null)
            {
                m_ZoneLight.color = color;
                m_ZoneLight.intensity = selected ? 4f : 1.2f;
            }
        }
    }
}
