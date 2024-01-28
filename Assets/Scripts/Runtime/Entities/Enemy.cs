using System;
using UnityEngine;

public abstract class Enemy : Creature {
    
    //----------------------------//
    // Public Properties
    //----------------------------//

    public bool IsGrounded => GetIsGrounded();
    public bool IsInStrikingRange => GetIsInStrkingRange();
    public bool IsChasingPlayer;
    public bool WillFall => GetWillFall();
    public bool ShouldJump => GetShouldJump();
    
    //----------------------------//
    // Enumerations
    //----------------------------//

    public enum AnimationState {
        idle, chase, jump, attack
    }
    
    //:::::::::::::::::::::::::::://
    // Constants
    //:::::::::::::::::::::::::::://
    
    private const float spriteFlipThreshold = 0.5f;
    
    //:::::::::::::::::::::::::::://
    // Private Properties
    //:::::::::::::::::::::::::::://
    
    protected bool IsFacingRight => SpriteRenderer.flipX;
    
    //:::::::::::::::::::::::::::://
    // Components
    //:::::::::::::::::::::::::::://

    private Animator Animator => GetAnimator();
    private BoxCollider2D BoxCollider => GetBoxCollider();
    private SpriteRenderer SpriteRenderer => GetSpriteRenderer();
    protected Rigidbody2D Rigidbody => GetRigidbody();
    
    //----------------------------//
    // Abstract Methods
    //----------------------------//

    public abstract void Attack(Action completionHandler);
    public abstract void Jump(Action completionHandler);
    
    //:::::::::::::::::::::::::::://
    // Serialized Fields
    //:::::::::::::::::::::::::::://

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected float acceleration = 4f;
    [SerializeField] protected float deceleration = 8f;
    [SerializeField] protected float jumpForce = 10f;
    
    [Header("Attack")]
    [SerializeField] protected float strikingDistance = 5f;
    [SerializeField] protected Vector3 strikingSize;
    
    [Header("Environment")]
    [Tooltip("The maximum ledge height enemy will fall off")]
    [SerializeField] protected float maximumFall = 8f;
    [Tooltip("The minimum distance to a wall before enemy jumps")]
    [SerializeField] protected float minimumWallDistance = 0.5f;
    
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;

    //----------------------------//
    // Enemy States
    //----------------------------//
    
    public EnemyAttackState AttackState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyJumpState JumpState { get; private set; }
    
    //:::::::::::::::::::::::::::://
    // Animator Hashes
    //:::::::::::::::::::::::::::://
    
    private readonly int stateIntegerHash = Animator.StringToHash("State");
    
    //:::::::::::::::::::::::::::://
    // Local Fields
    //:::::::::::::::::::::::::::://

    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;
    private EnemyStateMachine _enemyStateMachine;
    
    //::::::::::::::::::::::::::::://
    // Unity Callbacks
    //:::::::::::::::::::::::::::://

    protected override void Awake() {
        base.Awake(); // this will call Entity.Awake()
        
        // construct EnemyStateMachine
        _enemyStateMachine = new EnemyStateMachine();

        // construct EnemyStates
        AttackState = new EnemyAttackState(this, _enemyStateMachine);
        ChaseState = new EnemyChaseState(this, _enemyStateMachine);
        IdleState = new EnemyIdleState(this, _enemyStateMachine);
        JumpState = new EnemyJumpState(this, _enemyStateMachine);
    }

    protected override void Start() {
        base.Start(); // this will call Creature.Start()
        
        // configure state machine starting with idle state
        _enemyStateMachine.Configure(IdleState);
    }

    protected virtual void Update() {
        _enemyStateMachine.CurrentEnemyState.Update();
    }

    protected virtual void FixedUpdate() {
        _enemyStateMachine.CurrentEnemyState.FixedUpdate();
    }

    private void LateUpdate() {
        // flip sprite to always face the player (use threshold so it doesn't flip back and forth when under player)
        float xDifference = transform.position.x - ControllerGame.Player.transform.position.x;
        if (spriteFlipThreshold < Mathf.Abs(xDifference)) SpriteRenderer.flipX = xDifference < 0f;
    }

    //----------------------------//
    // Animation States
    //----------------------------//

    public void SetAnimationState(AnimationState animationState) {
        Animator.SetInteger(stateIntegerHash, (int)animationState);
    }
    
    //:::::::::::::::::::::::::::://
    // Property Getters
    //:::::::::::::::::::::::::::://
    
    private bool GetIsGrounded() {
        // get components needed for calcs
        Bounds boxColliderBounds = BoxCollider.bounds;
        
        // cast box (returns true if it collided with the ground layer)
        // add tiny margin as distance so it overlaps with the ground
        return Physics2D.BoxCast(boxColliderBounds.center, boxColliderBounds.size, 0f, -transform.up, 0.1f, groundLayer);
    }

    private bool GetIsInStrkingRange() {
        return 0f < strikingDistance && Vector3.Distance(ControllerGame.Player.transform.position, transform.position) <= strikingDistance;
    }

    private bool GetWillFall() {
        // get move direction (left = -1, right = 1)
        float moveDirection = IsFacingRight ? 1f : -1f;
        
        // get components needed for calcs
        Bounds boxColliderBounds = BoxCollider.bounds;
        
        // calculate starting point of ray (either bottom left or right depending which way the monster is facing)
        Vector2 origin = new(boxColliderBounds.center.x + boxColliderBounds.extents.x * moveDirection, boxColliderBounds.center.y - boxColliderBounds.extents.y);
        
        // cast ray (returns true if it does not collide with ground layer)
        // add tiny margin to maximum fall distance to snap to whole values
        return !Physics2D.Raycast(origin, -transform.up, maximumFall + 0.1f, groundLayer);
    }

    private bool GetShouldJump() {
        // get move direction (left = -1, right = 1)
        float moveDirection = IsFacingRight ? 1f : -1f;
        
        // get components needed for calcs
        Bounds boxColliderBounds = BoxCollider.bounds;
        
        // get the origin of the first ray (base of enemy)
        // add tiny margin so ray is off the ground slightly
        Vector2 origin = new(boxColliderBounds.center.x, boxColliderBounds.min.y + 0.1f);
        
        // CAST RAY #1; if it does not hit; we are done 
        RaycastHit2D raycastHit1 = Physics2D.Raycast(origin, transform.right * moveDirection, boxColliderBounds.extents.x + minimumWallDistance, groundLayer);
        if (!raycastHit1) return false;

        // update origin for next ray cast (slightly less than one whole block)
        origin.y = boxColliderBounds.min.y + 0.9f;
        
        // CAST RAY #2; if it does not hit; we are done 
        RaycastHit2D raycastHit2 = Physics2D.Raycast(origin, transform.right * moveDirection, boxColliderBounds.extents.x + minimumWallDistance, groundLayer);
        if (!raycastHit2) return false;
        
        // get the difference between the two raycast distances
        float difference = Mathf.Abs(raycastHit2.distance - raycastHit1.distance);

        // if the distance between the two is the same (within small threshold); this is a vertical wall and we need to jump
        return difference < 0.01f;
    }
    
    //:::::::::::::::::::::::::::://
    // Component Getters
    //:::::::::::::::::::::::::::://

    private Animator GetAnimator() {
        if (!_animator) _animator = GetComponent<Animator>();
        return _animator;
    }

    private BoxCollider2D GetBoxCollider() {
        if (!_boxCollider) _boxCollider = GetComponent<BoxCollider2D>(); 
        return _boxCollider;
    }

    private Rigidbody2D GetRigidbody() {
        if (!_rigidbody) _rigidbody = GetComponent<Rigidbody2D>();
        return _rigidbody;
    }

    private SpriteRenderer GetSpriteRenderer() {
        if (!_spriteRenderer) _spriteRenderer = transform.Find("Art").GetComponent<SpriteRenderer>();
        return _spriteRenderer;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var attackpos = new Vector3((IsFacingRight ? 1 : -1) * strikingDistance, 0);
        Gizmos.DrawWireCube(transform.position + attackpos, strikingSize);
    }
}
