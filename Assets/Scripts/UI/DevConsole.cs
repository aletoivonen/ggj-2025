using TMPro;
using UnityEngine;

namespace Zubble.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public class DevConsole : MonoBehaviour
    {
        private TMP_InputField _inputField;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
        }

        public void InputLine(string line)
        {
            _inputField.text += $"\n{line}";
            // TODO: parse line here
        }
    }
}
