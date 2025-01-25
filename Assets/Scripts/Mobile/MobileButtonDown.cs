using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Zubble.Mobile
{
    [RequireComponent(typeof(Button))] 
    public class MobileButtonDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private string _buttonName;

        private void Awake()
        {
            Debug.Log("asd");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MultiInput.Instance.SetButtonDown(_buttonName, true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            MultiInput.Instance.SetButtonDown(_buttonName, false);
        }
    }
}