using System;
using TMPro;
using UnityEngine;

namespace Zubble.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class SoapHUD : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshProUGUI;
        private float _previousSoap;

        private void Awake()
        {
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            _previousSoap = -1;
        }

        public void Update()
        {
            var soap = Inventory.Instance.Soap;
            if (!(Math.Abs(soap - _previousSoap) > 0.01f))
            {
                return;
            }
            _textMeshProUGUI.text = $"Soap: {soap}";
            _previousSoap = soap;
        }
    }
}
