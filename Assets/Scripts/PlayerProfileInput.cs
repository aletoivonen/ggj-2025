using System;
using TMPro;
using UnityEngine;

namespace Zubble
{
    public class PlayerProfileInput : MonoBehaviour
    {
        public static event Action<string> OnPlayerNameChosen;

        private TMP_InputField _input;

        [SerializeField] private GameObject _connectingElements;
        
        private void Awake()
        {
            _input = GetComponentInChildren<TMP_InputField>();
            SocketManager.OnConnected += OnConnected;
        }

        private void OnDestroy()
        {
            SocketManager.OnConnected -= OnConnected;
        }

        private void OnConnected()
        {
            gameObject.SetActive(false);
        }

        public void OnSubmitButton()
        {
            OnPlayerNameChosen?.Invoke(_input.text);
            
            _connectingElements.SetActive(true);
        }
    }
}
