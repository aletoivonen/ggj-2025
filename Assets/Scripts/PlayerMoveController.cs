using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMoveController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float _moveSpeed = 5f;
    public float _jumpForce = 10f;

    [Header("Ground Check Settings")]
    public float _groundCheckDistance = 0.1f;
    public LayerMask _groundLayer;

    private Rigidbody2D _rb;
    private Transform _spawn;
    private bool _isGrounded;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spawn = GameObject.FindWithTag("Spawn").transform;
    }

    private void OnEnable()
    {
        DeathZone.OnDeathTriggered += OnDeath;
    }

    private void OnDisable()
    {
        DeathZone.OnDeathTriggered -= OnDeath;
    }

    private void OnDeath()
    {
        transform.position = _spawn.position;
        _rb.linearVelocity = Vector2.zero;
    }

    void Update()
    {
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
        // Get the bounds of the collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null) return false;

        // Raycast down from the bottom of the collider
        Vector2 origin = new Vector2(collider.bounds.center.x, collider.bounds.min.y); // Bottom center of the collider
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, _groundCheckDistance, _groundLayer);

        // Optional: Draw ray in the editor for debugging
        Debug.DrawRay(origin, Vector2.down * _groundCheckDistance, hit.collider != null ? Color.green : Color.red);

        return hit.collider != null && _rb.linearVelocity.y < 0.1f; // Returns true if the raycast hits the ground
    }
}