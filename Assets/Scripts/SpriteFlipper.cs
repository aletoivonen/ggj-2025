using UnityEngine;

namespace Zubble
{
    public class SpriteFlipper : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        void Update()
        {
            if (_rb.linearVelocity.x > 0)
            {
                _spriteRenderer.flipX = false;
            }
            else if (_rb.linearVelocity.x < 0)
            {
                _spriteRenderer.flipX = true;
            }
        }
    }
}
