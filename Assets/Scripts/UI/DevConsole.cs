using TMPro;
using UnityEngine;

namespace Zubble.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    [RequireComponent(typeof(CheckOS))]
    public class DevConsole : MonoBehaviour
    {
        private TMP_InputField _inputField;
        private SocketManager _socketManager;
        private CheckOS _checkOS;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _checkOS = GetComponent<CheckOS>();
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
                case "players": 
                    _inputField.text += "\nPlayers:";
                    foreach (var player in _socketManager._spawnedPlayers)
                    {
                        var pos = player.Value.transform.position;
                        _inputField.text += $"\nID={player.Value.PlayerId} x={pos.x} y={pos.y}";
                    }
                    break;
                case "os":
                    _inputField.text += $"\nOS: {_checkOS.GetOperationSystemFamilyName()} android={_checkOS.IsAndroid()} ios={_checkOS.IsIos()}";
                    break;
                default:
                    _inputField.text += $"\n{line}";
                    break;
            }
        }
    }
}
