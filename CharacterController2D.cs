using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    [Header("Movement settings")]
    [Tooltip("The maximum horizontal speed of the character in units per second")]
    [SerializeField] private float speed = 5;
    [Tooltip("The maximum jump height of the character in units")]
    [SerializeField] private float jumpHeight = 2;

    [Header("Collision Settings")]
    [Tooltip("Enable this to prevent character sticking to walls")]
    [SerializeField] private bool createFrictionlessPhysicsMaterial = true;
    [Tooltip("Set this to your ground layer")]
    [SerializeField] private LayerMask ground;
    [Tooltip("The shape of the ground check")]
    [SerializeField] private GroundCheckShape groundCheckShape;
    [Tooltip("The size of the ground check in units")]
    [SerializeField] private float groundCheckSize = 1;

    // Properties
    public float Speed { get => speed; set => speed = value; }
    public float JumpHeight { get => jumpHeight; set => jumpHeight = value; }
    public float MovementDirection { get; set; }
    public Rigidbody2D Rigidbody2D => _rigidbody2D;
    public Collider2D Collider2D => _collider2D;
    public bool Grounded { get; set; }
    public float FroundCheckSize { get => groundCheckSize; set => groundCheckSize = value; }

    // Private fields
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private bool _shouldJump;
    private const float BoxCheckHeight = 0.2f;
    private enum GroundCheckShape
    {
        Box,
        Circle
    }
    
    // Awake is used to get the required components
    // as well as create and apply any objects/materials that are required by the controller
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        if (_collider2D == null)
        {
            _collider2D = gameObject.AddComponent<BoxCollider2D>();
        }
        
        if (createFrictionlessPhysicsMaterial)
        {
            PhysicsMaterial2D physicsMaterial2D = new PhysicsMaterial2D {friction = 0};
            _rigidbody2D.sharedMaterial = physicsMaterial2D;
        }
    }

    // FixedUpdate is used to apply movement
    // Movement is calculated in the physics step therefore force and velocity are applied in FixedUpdate
    private void FixedUpdate()
    {
        CheckGrounded();
        
        // Get x and y velocities
        Vector2 velocity;
        velocity.x = MovementDirection * speed;
        velocity.y = (_shouldJump) ? CalculateJumpVelocity() : _rigidbody2D.velocity.y;
        _rigidbody2D.velocity = velocity;
    }

    // Jump method
    // Call this method to make the character jump
    public void Jump()
    {
        if (Grounded)
        {
            _shouldJump = true;
        }
    }

    // Calculates the required vertical velocity to reach the maximum jumpHeight
    // Returns _shouldJump to false
    private float CalculateJumpVelocity()
    { 
        _shouldJump = false;
        return Mathf.Sqrt(jumpHeight * -2 * Physics2D.gravity.y * _rigidbody2D.gravityScale);
    }

    // Method to check if the player is grounded
    // Uses OverlapBox or OverlapCircle depending on the shape the user has selected in the controller collision settings
    // A circle shape will have a radius equal to groundCheckSize
    // A box shape will have a width equal to groundCheckSize and a height equal to boxCheckHeight
    private void CheckGrounded()
    {
        Vector2 castPoint = new Vector2(0, _collider2D.bounds.min.y);
        if (groundCheckShape == GroundCheckShape.Box)
        {
            Grounded = Physics2D.OverlapBox(castPoint,
                new Vector2(groundCheckSize, BoxCheckHeight), 0, ground);
        }
        else if (groundCheckShape == GroundCheckShape.Circle)
        {
            Grounded = Physics2D.OverlapCircle(castPoint, groundCheckSize, ground);
        }
    }
}
