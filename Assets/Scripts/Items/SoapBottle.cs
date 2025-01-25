using UnityEngine;

namespace Zubble.Items
{
    public class SoapBottle : MonoBehaviour
    {
        [SerializeField] private Transform _child;

        private void Update()
        {
            _child.Rotate(new Vector3(0f, 1.0f, 0.5f), Time.deltaTime * 50);
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
