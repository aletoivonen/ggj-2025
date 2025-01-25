using System;
using UnityEngine;

namespace Zubble
{
    public class BubbleLift : MonoBehaviour
    {
        public int BubbleId;

        [SerializeField] private Vector3 _spawnPosition;
        private float _duration = 2f;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _spawnPosition = transform.position;
        }
#endif

        private void Awake()
        {
            PlayerMoveController.OnPlayerDead += OnPlayerDead;
        }

        private void OnDestroy()
        {
            PlayerMoveController.OnPlayerDead -= OnPlayerDead;
        }

        private void OnPlayerDead(PlayerMoveController player)
        {
            if (!player.SocketPlayer.IsLocalPlayer)
            {
                return;
            }

            gameObject.SetActive(true);
            transform.position = _spawnPosition;
        }

        public void Initialize(int id, Vector3 spawnPosition, float duration)
        {
            BubbleId = id;
            _spawnPosition = spawnPosition;
            _duration = duration;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            var player = other.GetComponent<PlayerMoveController>();

            if (!player.SocketPlayer.IsLocalPlayer)
            {
                return;
            }

            player.RideBubble(_duration);
            gameObject.SetActive(false);
        }
    }
}
