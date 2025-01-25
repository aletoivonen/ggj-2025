using TMPro;
using UnityEngine;

namespace Zubble.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public class ConsoleInputField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _history;

        private TMP_InputField _inputField;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _inputField.onSubmit.AddListener(OnSubmit);
        }

        private void OnSubmit(string arg0)
        {
            _history.text += $"\n{arg0}";
        }
    }
}
