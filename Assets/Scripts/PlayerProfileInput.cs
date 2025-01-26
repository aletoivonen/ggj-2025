using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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

            _input.text = "Player#" + Random.Range(1000, 9999);
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
