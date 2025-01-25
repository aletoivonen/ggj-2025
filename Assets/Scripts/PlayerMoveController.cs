using System;
using UnityEngine;

namespace Zubble
{
    public class PlayerMoveController : MonoBehaviour
    {
        public static event Action<PlayerMoveController> OnPlayerDead;
        
        /// <summary>
        /// Param: duration
        /// </summary>
        public static event Action<float> OnLocalPlayerBubble;
    
        [Header("Movement Settings")]
        public float _moveSpeed = 5f;
        public float _jumpForce = 10f;

        [Header("Ground Check Settings")]
        public float _groundCheckDistance = 0.1f;
        public LayerMask _groundLayer;

        [Header("Reset Settings")]
        [SerializeField] private float _resetOffset = 1f;
        private Rigidbody2D _rb;
        private Transform _spawn;
        private bool _isGrounded;
        private Collider2D _col;
    
        public SocketPlayer SocketPlayer { get; private set; }
    
        private bool _isSpring;
        private float _previousVerticalInput;

        private bool _inBubble;
        private float _bubbleTimer;
        [SerializeField] private float _bubbleFloatSpeed;
        
        [SerializeField] private GameObject _bubbleObject;

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _spawn = GameObject.FindWithTag("Spawn").transform;
            _col = GetComponent<Collider2D>();
            SocketPlayer = GetComponent<SocketPlayer>();
        }

        private void OnEnable()
        {
            DeathZone.OnDeathTriggered += OnDeath;
            SideTrigger.OnSideTriggerEnter += OnSideTriggerEnter;
            Spring.OnSpringEnter += OnSpringEnter;
            Spring.OnSpringExit += OnSpringExit;
        }

        private void OnSpringExit()
        {
            _isSpring = false;
        }

        private void OnSpringEnter()
        {
            _isSpring = true;
        }

        private void OnSideTriggerEnter(bool isLeftSide)
        {
            if (!SocketPlayer.IsLocalPlayer)
            {
                return;
            }

            if (isLeftSide)
            {
                transform.position = new Vector2(-transform.position.x - _resetOffset, transform.position.y);
            }
            else
            {
                transform.position = new Vector2(-transform.position.x + _resetOffset, transform.position.y);
            }
        }

        private void OnDisable()
        {
            DeathZone.OnDeathTriggered -= OnDeath;
            SideTrigger.OnSideTriggerEnter -= OnSideTriggerEnter;
        }

        private void OnDeath()
        {
            if (!SocketPlayer.IsLocalPlayer)
            {
                return;
            }

            transform.position = _spawn.position;
            _rb.linearVelocity = Vector2.zero;
        
            OnPlayerDead?.Invoke(this);
        }

        void Update()
        {
            if (!SocketPlayer.IsLocalPlayer)
            {
                return;
            }

            // Handle horizontal movement
            float moveInput = Input.GetAxis("Horizontal");
            _rb.linearVelocity = new Vector2(moveInput * _moveSpeed, _rb.linearVelocity.y);

            if (_inBubble)
            {
                _rb.linearVelocityY = _bubbleFloatSpeed;
                
                _bubbleTimer -= Time.deltaTime;
                if (_bubbleTimer <= 0f)
                {
                    _inBubble = false;
                    _bubbleObject.SetActive(false);
                }

                return;
            }

            // Use soap
            float vertical = Input.GetAxis("Vertical");
            if (_previousVerticalInput == 0f && vertical > 0) 
            {
                if (Inventory.Instance.Soap >= 1f)
                {
                    Inventory.Instance.RemoveSoap(1f);
                    Debug.Log($"Used soap, soap left: {Inventory.Instance.Soap}");
                }
                else
                {
                    Debug.Log($"{Inventory.Instance.Soap} is not enough soap");
                }
            }

            // Check if the player is grounded
            _isGrounded = IsGrounded();

            // Handle jumping
            if (_isGrounded)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            }
        }

        private bool IsGrounded()
        {
            bool isNotGoingUp = _rb.linearVelocity.y < 0.1f;
            if (!isNotGoingUp)
            {
                return false;
            }

            int raycasts = 5;
            for (int i = 0; i < raycasts; i++)
            {
                float x = _col.bounds.min.x + (_col.bounds.size.x / (raycasts - 1)) * i;
                Vector2 origin = new Vector2(x, _col.bounds.min.y);
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, _groundCheckDistance, _groundLayer);
                bool hitGround = hit.collider != null;
                Debug.DrawRay(origin, Vector2.down * _groundCheckDistance, hitGround ? Color.green : Color.red);
                if (hitGround)
                {
                    return true;
                }
            }

            return false;
        }

        public void RideBubble(float duration)
        {
            if (!SocketPlayer.IsLocalPlayer)
            {
                return;
            }

            _inBubble = true;
            _bubbleTimer = duration;
            
            _bubbleObject.SetActive(true);
            
            OnLocalPlayerBubble?.Invoke(duration);
        }

        public void PickUpSoap()
        {
            Debug.Log("Picked up soap!");
        }
    }
}