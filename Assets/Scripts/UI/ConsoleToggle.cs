using UnityEngine;
using UnityEngine.UI;

namespace Zubble.UI
{
    [RequireComponent(typeof(Button))]
    public class ConsoleToggle : MonoBehaviour
    {
        [SerializeField] private GameObject _consoleOverlay;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Click);
        }

        public void Click()
        {
            _consoleOverlay.SetActive(!_consoleOverlay.activeSelf);
        }
    }
}
