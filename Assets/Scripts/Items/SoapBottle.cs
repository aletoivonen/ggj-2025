using System;
using UnityEngine;

namespace Zubble.Items
{
    public class SoapBottle : MonoBehaviour
    {
        [SerializeField] private Transform _child;

        private void Update()
        {
            _child.Rotate(new Vector3(0.5f, 1, 0), Time.deltaTime * 50);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return; 
            }

            var player = other.GetComponent<SocketPlayer>();

            if (player == null)
            {
                return;
            }
            
            if (!player.IsLocalPlayer)
            {
                return;
            }
            
            // Pick up the soap bottle
        }
    }
}
