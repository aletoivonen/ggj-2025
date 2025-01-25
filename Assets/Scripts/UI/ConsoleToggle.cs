using UnityEngine;
using UnityEngine.UI;

namespace Zubble.UI
{
    [RequireComponent(typeof(Button))]
    public class ConsoleToggle : MonoBehaviour
    {
        [SerializeField] private GameObject _consoleOverlay;

        private Button _button;
        private ConsoleInputField _consoleInputField;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Click);
            _consoleInputField = _consoleOverlay.GetComponentInChildren<ConsoleInputField>();
        }

        public void Click()
        {
            bool next = !_consoleOverlay.activeSelf;
            _consoleOverlay.SetActive(next);
            if (next)
            {
                _consoleInputField.Focus();
            }
        }
    }
}
