using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Zubble.Mobile
{
    public class MobileInputAxisButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private string _axisName;
        [SerializeField] private float _axisValue;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            MultiInput.Instance.SetAxis(_axisName, _axisValue);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            MultiInput.Instance.SetAxis(_axisName, 0f);
        }
    }
}
