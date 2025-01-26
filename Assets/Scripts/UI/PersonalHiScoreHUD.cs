using System;
using TMPro;
using UnityEngine;

namespace Zubble.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PersonalHighScoreHUD : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshProUGUI;
        private float _previousScore;

        private void Awake()
        {
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            _previousScore = -1f;
        }

        public void Update()
        {
            var highScore = Inventory.Instance.HighScore;
            if (!(Math.Abs(highScore - _previousScore) > 0.01f))
            {
                return;
            }
            _textMeshProUGUI.text = $"Personal best: {highScore:F2}";
            _previousScore = highScore;
        }
    }
}