using System;
using UnityEngine;

namespace Zubble
{
    public class Spring : MonoBehaviour
    {
        public static event Action OnSpringEnter;
        public static event Action OnSpringExit;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<SocketPlayer>(out var p) || !p.IsLocalPlayer)
            {
                return;
            }

            OnSpringEnter?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.TryGetComponent<SocketPlayer>(out var p) || !p.IsLocalPlayer)
            {
                return;
            }

            OnSpringExit?.Invoke();
        }
    }
}
