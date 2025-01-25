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
            OnSpringEnter?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnSpringExit?.Invoke();
        }
    }
}
