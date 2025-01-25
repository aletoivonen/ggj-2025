using TMPro;
using UnityEngine;

namespace Zubble.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public class DevConsole : MonoBehaviour
    {
        private TMP_InputField _inputField;
        private SocketManager _socketManager;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            var temp = FindObjectsByType<SocketManager>(FindObjectsSortMode.None);
            if (temp.Length > 0) _socketManager = temp[0] as SocketManager;
        }

        public void InputLine(string line)
        {
            switch (line.Split(' ')[0].ToLower())
            {
                case "connect":
                    _inputField.text += "\nConnecting...";
                    _socketManager.Connect();
                    break;
                default:
                    _inputField.text += $"\n{line}";
                    break;
            }
        }
    }
}
