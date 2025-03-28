using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Zubble
{
    public class PlayerMoveController : MonoBehaviour
    {
        public static event Action<PlayerMoveController> OnPlayerDead;

        /// <summary>
        /// Param: position, duration
        /// </summary>
        public static event Action<Vector3, float, bool> OnLocalPlayerBubble;

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

        public bool InBubble { get; private set; }
        private float _bubbleTimer;
        [SerializeField] private float _bubbleFloatSpeed;
        [SerializeField] private float _bubbleDuration = 2;

        [SerializeField] private GameObject _bubbleObject;

        [SerializeField] private BubbleLift _bubbleLiftPrefab;

        private float _timeSinceJump = 0.0f;
        private float _jumpWindow = 0.1f;

        private float _runBestHeight;
        private float _highestGroundedHeight;
        private bool _fallingDown;
        [SerializeField] private float _fallThreshold = 10f;

        [SerializeField] private AudioSource _playerSounds;

        private Animator _animator;

        private bool _dead;

        private float _lastPosCheckTimer;
        private Vector3 _lastRemotePos;

        private SpriteRenderer _rend;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rend = GetComponent<SpriteRenderer>();
        }

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

            _dead = true;

            StartCoroutine(DelayDeath());

            OnPlayerDead?.Invoke(this);
        }

        private IEnumerator DelayDeath()
        {
            _animator.SetTrigger("dead");
            _rb.linearVelocity = Vector2.zero;
            
            _playerSounds.PlayOneShot(SoundManager.Instance.RandomDeathSound());
            
            yield return new WaitForSeconds(0.75f);

            transform.position = _spawn.position;
            _rb.linearVelocity = Vector2.zero;

            _fallingDown = false;
            _col.enabled = true;
            _runBestHeight = 0.0f;

            Inventory.Instance.RemoveSoap(Inventory.Instance.Soap);

            _dead = false;
        }

        void Update()
        {
            if (!SocketPlayer.IsLocalPlayer)
            {
                
                _lastPosCheckTimer += Time.deltaTime;

                if (_lastPosCheckTimer > 0.1f)
                {
                    _lastPosCheckTimer = 0.0f;
                }
                else
                {
                    return;
                }
                
                Vector3 moveDir = transform.position - _lastRemotePos;
                GetComponentInChildren<SpriteFlipper>().enabled = false;

                _rend.flipX = moveDir.x < 0f;
                
                _animator.SetFloat("velocityX", Mathf.Abs(moveDir.x));
                _animator.SetFloat("velocityY", moveDir.y);
                _animator.SetBool("grounded", IsGrounded());

                _lastRemotePos = transform.position;
                
                return;
            }
            
            // not local or game not init
            if (SocketManager.Instance.PlayerID < 0 || _dead)
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

            if (_runBestHeight > Inventory.Instance.HighScore)
            {
                Inventory.Instance.SetHighScore(_runBestHeight);
            }

            // Handle horizontal movement
            float moveInput = MultiInput.Instance.GetAxis("Horizontal");
            _rb.linearVelocity = new Vector2(moveInput * _moveSpeed, _rb.linearVelocity.y);

            _timeSinceJump += Time.deltaTime;

            if (InBubble)
            {
                _rb.linearVelocityY = _bubbleFloatSpeed;

                _bubbleTimer -= Time.deltaTime;
                if (_bubbleTimer <= 0f)
                {
                    InBubble = false;
                    _bubbleObject.SetActive(false);
                }

                return;
            }

            // Use soap
            if (MultiInput.Instance.GetButtonDown("Soap") && !InBubble)
            {
                RideBubble(_bubbleDuration);
            }

            if (MultiInput.Instance.GetButtonDown("Jump"))
            {
                _timeSinceJump = 0.0f;
            }

            // Check if the player is grounded
            _isGrounded = IsGrounded();

            if (_isGrounded)
            {
                _highestGroundedHeight = Mathf.Max(_highestGroundedHeight, transform.position.y);
            }
            else if (_runBestHeight - transform.position.y > _fallThreshold && transform.position.y > 10)
            {
                FallDown();
            }

            _animator.SetFloat("velocityX", Mathf.Abs(_rb.linearVelocityX));
            _animator.SetFloat("velocityY", _rb.linearVelocityY);
            _animator.SetBool("grounded", _isGrounded);

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
        }

        private void FallDown()
        {
            Debug.Log("Fall down");
            _fallingDown = true;

            _animator.SetTrigger("hurt");

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

        public void RideBubble(float duration, bool existing = false)
        {
            if (!SocketPlayer.IsLocalPlayer)
            {
                return;
            }

            _animator.SetTrigger("victory");

            if (Inventory.Instance.Soap >= 1f && !existing)
            {
                Inventory.Instance.RemoveSoap(1f);
                Debug.Log($"Used soap, soap left: {Inventory.Instance.Soap}");
            }
            else if (!existing)
            {
                Debug.Log($"{Inventory.Instance.Soap} is not enough soap");
                return;
            }

            InBubble = true;
            _bubbleTimer = duration;

            _bubbleObject.SetActive(true);

            OnLocalPlayerBubble?.Invoke(transform.position, duration, existing);
        }

        public void ShowBubble()
        {
            StartCoroutine(RemoteBubbleCoroutine());
        }

        private IEnumerator RemoteBubbleCoroutine()
        {
            _bubbleObject.SetActive(true);
            yield return new WaitForSeconds(_bubbleDuration);
            _bubbleObject.SetActive(false);
        }
    }
}
