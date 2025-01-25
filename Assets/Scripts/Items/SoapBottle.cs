using System;
using UnityEngine;

namespace Zubble.Items
{
    public class SoapBottle : MonoBehaviour
    {
        [SerializeField] private Transform _child;

        private void Update()
        {
            Vector3 pos = _child.transform.localPosition;
            pos.y = Mathf.Sin(Time.time) * 0.3f;
            _child.transform.localPosition = pos;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                return;
            }

            var player = other.gameObject.GetComponent<SocketPlayer>();

            if (player == null)
            {
                return;
            }

            if (!player.IsLocalPlayer)
            {
                return;
            }

            Inventory.Instance.AddSoap(1f);
            Destroy(gameObject);
        }
    }
}
