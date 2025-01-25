using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zubble;

public class PlayerMoveController : MonoBehaviour
{
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
    private SocketPlayer _socketPlayer;
    private bool _isSpring;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spawn = GameObject.FindWithTag("Spawn").transform;
        _col = GetComponent<Collider2D>();
        _socketPlayer = GetComponent<SocketPlayer>();
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
        if (!_socketPlayer.IsLocalPlayer)
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
        if (!_socketPlayer.IsLocalPlayer)
        {
            return;
        }

        transform.position = _spawn.position;
        _rb.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        if (!_socketPlayer.IsLocalPlayer)
        {
            return;
        }

        // Handle horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        _rb.linearVelocity = new Vector2(moveInput * _moveSpeed, _rb.linearVelocity.y);

        // Check if the player is grounded
        _isGrounded = IsGrounded();

        // Handle jumping
        if (_isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            if (_isSpring)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce * 1.5f);
                _isSpring = false;
            }
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
}