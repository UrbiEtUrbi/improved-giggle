using System;
using UnityEngine;

public abstract class Enemy : Creature {
    
    //----------------------------//
    // Public Properties
    //----------------------------//

    public bool IsGrounded => GetIsGrounded();
    public bool IsInStrikingRange => GetIsInStrkingRange();
    public bool IsChasingPlayer;
    
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
    
    //:::::::::::::::::::::::::::://
    // Serialized Fields
    //:::::::::::::::::::::::::::://

    [Header("Enemy")]
    [SerializeField] protected float strikingDistance = 5f;
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected float acceleration = 4f;
    [SerializeField] protected float deceleration = 8f;
    
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
        // get components neede for cals
        Transform enemyTransform = transform;
        BoxCollider2D boxCollider = BoxCollider;
        Vector2 boxColliderSize = boxCollider.size;
            
        // calculate box cast values
        Vector2 size = new(boxColliderSize.x, 0.1f);
        float distance = boxColliderSize.y / 2f - boxCollider.offset.y;
        
        // cast box (returns true if it collided with the ground layer)
        return Physics2D.BoxCast(enemyTransform.position, size, 0f, -enemyTransform.up, distance, groundLayer);
    }

    private bool GetIsInStrkingRange() {
        return 0f < strikingDistance && Vector3.Distance(ControllerGame.Player.transform.position, transform.position) <= strikingDistance;
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
}
