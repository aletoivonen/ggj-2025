using System;
using TMPro;
using UnityEngine;

namespace Zubble.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ServerHiScoreHUD : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshProUGUI;

        private void Awake()
        {
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            SocketManager.OnHighScoreUpdate += Refresh;
        }

        private void OnDestroy()
        {
            SocketManager.OnHighScoreUpdate -= Refresh;
        }

        public void Refresh(uint score)
        {
            _textMeshProUGUI.text = $"Server best: {score:F2}";
        }
    }
}
