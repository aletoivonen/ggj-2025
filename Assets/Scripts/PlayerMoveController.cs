using System;
using UnityEngine;

namespace Zubble
{
    public class PlayerMoveController : MonoBehaviour
    {
        public static event Action<PlayerMoveController> OnPlayerDead;

        /// <summary>
        /// Param: position, duration
        /// </summary>
        public static event Action<Vector3, float> OnLocalPlayerBubble;

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

        [SerializeField] private BubbleLift _bubbleLiftPrefab;

        private float _timeSinceJump = 0.0f;
        private float _jumpWindow = 0.1f;

        private float _runBestHeight;
        private float _personalBest;
        private float _lastGroundedHeight;
        private bool _fallingDown;
        [SerializeField] private float _fallThreshold = 10f;

        [SerializeField] private AudioSource _playerSounds;

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

            _fallingDown = false;
            _col.enabled = true;
            _runBestHeight = 0.0f;

            Inventory.Instance.RemoveSoap(Inventory.Instance.Soap);

            OnPlayerDead?.Invoke(this);
        }

        void Update()
        {
            if (!SocketPlayer.IsLocalPlayer)
            {
                return;
            }

            if (transform.position.y < -2)
            {
                OnDeath();
            }

            if (_fallingDown)
            {
                if (transform.position.y < 3)
                {
                    _col.enabled = true;
                    _fallingDown = false;
                    _runBestHeight = 0.0f;
                }
                else
                {
                    return;
                }
            }

            if (transform.position.y > _runBestHeight)
            {
                _runBestHeight = transform.position.y;
            }

            if (_runBestHeight > _personalBest)
            {
                _personalBest = _runBestHeight;
            }

            // Handle horizontal movement
            float moveInput = Input.GetAxis("Horizontal");
            _rb.linearVelocity = new Vector2(moveInput * _moveSpeed, _rb.linearVelocity.y);

            _timeSinceJump += Time.deltaTime;

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
                RideBubble(2);
            }

            if (Input.GetButtonDown("Jump"))
            {
                _timeSinceJump = 0.0f;
            }

            // Check if the player is grounded
            _isGrounded = IsGrounded();

            if (_isGrounded)
            {
                _lastGroundedHeight = transform.position.y;
            }
            else if (_runBestHeight - transform.position.y > _fallThreshold && transform.position.y > 10)
            {
                FallDown();
            }

            // Handle jumping
            if (_isGrounded && _timeSinceJump < _jumpWindow)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            }
            else if (_isSpring && _isGrounded)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce * 1.5f);
                _isSpring = false;
            }

            _previousVerticalInput = vertical;
        }

        private void FallDown()
        {
            Debug.Log("Fall down");
            _fallingDown = true;

            _col.enabled = false;
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocityY);
            
            _playerSounds.PlayOneShot(SoundManager.Instance.RandomFallSound());
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

            if (Inventory.Instance.Soap >= 1f)
            {
                Inventory.Instance.RemoveSoap(1f);
                Debug.Log($"Used soap, soap left: {Inventory.Instance.Soap}");
            }
            else
            {
                Debug.Log($"{Inventory.Instance.Soap} is not enough soap");
                return;
            }

            _inBubble = true;
            _bubbleTimer = duration;

            _bubbleObject.SetActive(true);

            OnLocalPlayerBubble?.Invoke(transform.position, duration);
        }

        public void PickUpSoap()
        {
            Debug.Log("Picked up soap!");
        }
    }
}
