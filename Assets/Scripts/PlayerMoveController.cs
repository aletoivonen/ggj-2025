using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMoveController : MonoBehaviour
{
    public static event Action OnPlayerDead;
    
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
        
        OnPlayerDead?.Invoke();
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
        throw new NotImplementedException();
    }
}