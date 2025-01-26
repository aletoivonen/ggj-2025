using System;
using UnityEngine;

namespace Zubble.Items
{
    public class SoapBottle : MonoBehaviour
    {
        [SerializeField] private Transform _child;

        private void Awake()
        {
            PlayerMoveController.OnPlayerDead += OnPlayerDead;
        }

        private void OnDestroy()
        {
            PlayerMoveController.OnPlayerDead -= OnPlayerDead;
        }

        private void OnPlayerDead(PlayerMoveController obj)
        {
            ToggleActive(true);
        }

        private void Update()
        {
            Vector3 pos = _child.transform.localPosition;
            pos.y = Mathf.Sin(Time.time) * 0.3f;
            _child.transform.localPosition = pos;
        }

        private void OnTriggerEnter2D(Collider2D other)
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
            ToggleActive(false);
        }

        private void ToggleActive(bool active)
        {
            GetComponent<Collider2D>().enabled = active;
            GetComponentInChildren<Renderer>().enabled = active;
        }
    }
}
