using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovementController : MonoBehaviour
{


    [BeginGroup("Running")]
    [SerializeField]
    [Tooltip("Top speed")]
    float m_RunSpeed = 10;
    [SerializeField]
    [Tooltip("Horizontal movement acceleration")]
    float runAcceleration = 5;
    [SerializeField]
    [Tooltip("Horizontal movement decceleration")]
    float runDecceleration = 5;
    [EndGroup]
    [Tooltip("Max distance the player can return")]
    [SerializeField]
    float ReturnDistance = 10f;

    [BeginGroup("Jumping")]
    [Tooltip("Jumping force happens once")]
    [SerializeField]
    float jumpForce = 10;

    [SerializeField]
    [Tooltip("Multiplier for horizontal movement acceleration in air")]
    float jumpAcceleration = 1;
    [SerializeField]
    [EndGroup]
    [Tooltip("Multiplier for horizontal movement decceleration in air")]
    float jumpDecceleration = 1;

    [BeginGroup("Gravity")]
    [Tooltip("Overall gravity multiplier")]
    [SerializeField]
    float gravityMultiplier = 1;
    [SerializeField]
    [Tooltip("Gravity multiplier while jumping and holding jump")]
    float jumpGravityJumpHeld = 1.7f;
    [SerializeField]
    [Tooltip("Gravity multiplier while jumping and not holding jump")]
    float jumpGravity = 3f;
    [SerializeField]

    [Tooltip("Gravity multiplier while falling and holding jump")]

    float fallingGravityJumpHeld = 2f;
    [SerializeField]
    [Tooltip("Gravity multiplier while falling and not holding jump")]
    float fallingGravity = 4f;

    [SerializeField]
    [Tooltip("Max Fall Speed")]
    [EndGroup]
    float maximumFallSpeed = 50f;

    [BeginGroup("Hang Times")]
    [SerializeField]
    [Tooltip("Vertical velocity threshold at apex of jump to toggle better manouverability")]
    float hangTimeThreshold = 0.1f;
    [SerializeField]
    [Tooltip("Extra acceleration at apex")]
    float hangTimeAccelerationMult = 1.1f;
    [SerializeField]
    [Tooltip("Extra top speed at apex")]
    float hangTimeSpeedMult = 1.1f;

    [SerializeField]
    [Tooltip("Meep Meep")]
    [EndGroup]
    float coyoteTime = 0.7f;

    [SerializeField]
    [BeginGroup("References")]
    Rigidbody2D m_RigidBody;

    public Rigidbody2D RigidBody => m_RigidBody;

    [SerializeField]
    SpriteRenderer m_PlayerSprite;

    [SerializeField]
    Animator m_PlayerAnimator;


    [EndGroup]
    [SerializeField] Player player;


    [HideInInspector]
    public Vector2 lastGroundedPosition;



    [Header("Layers")]
    public LayerMask groundLayer;

    [Header("Ground Checks")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Vector3 groundCheckPoint;


    private bool jumping = false;
    private bool falling = false;


    private float inputX, inputY;
    private bool jumpHeld = false;

    float LastOnGroundTime;


    float maxPositionX;
    public float GetMinimumXReturnPosition => maxPositionX - ReturnDistance;

    bool feetTouchingGround;
    public bool OnGround => LastOnGroundTime > 0;

    public bool FacingRight => !m_PlayerSprite.flipX;

    public float dashDirection;
    float canDashDirection;

    public bool Dashing;


    [BeginGroup("Dash")]
    [SerializeField]
    float dashSpeed = 500;
    [SerializeField]
    float dashAcceleration = 10;
    [SerializeField]
    float dashDuration = 0.1f;
    [SerializeField]
    float dashDurationAfterHitEnemy = 0.1f;
    [SerializeField]
    float forceAfterHitEnemy = 2;
    [SerializeField]
    private Vector2 dashCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField]
    private Vector3 dashCheckPointLeft;
    [EndGroup]
    [SerializeField]
    private Vector3 dashCheckPointRight;

    [BeginGroup("Rage Boost")]
    [SerializeField]
    int MaxRageEnemies;
    [SerializeField]
    float RageCooldown;
    [SerializeField]
    float AccelerationBoostPerEnemy;
    [SerializeField]
    float SpeedBoostPerEnemy;
    [EndGroup]
    [SerializeField]
    float RageEndTweenTime;


    [EditorButton(nameof(IncreaseEnemyDebug))]
    [SerializeField]
    float test;


    void IncreaseEnemyDebug()
    {
        ChangeEnemyCount(1);
    }

    int currentEnemyRageCount;

    
    public float LastDashDurationTime;
    public float RageTimer;
    public float RageEndingTimer;
    public bool RageActive;
    public bool RageEnding;


    Vector3[] points;
    float slopeCheck = 1f;
   

    float RageSpeedBoost {

        get {
            if (RageActive)
            {
                return currentEnemyRageCount * SpeedBoostPerEnemy;

            }
            else if (RageEnding)
            {
                return (currentEnemyRageCount * SpeedBoostPerEnemy) * Mathf.Lerp(0, 1, RageEndingTimer / RageCooldown);
            }
            else
            {
                return 0;
            }

        }
    }

    float RageAccelerationBoost
    {

        get
        {
            if (RageActive)
            {
                return 1 + currentEnemyRageCount * AccelerationBoostPerEnemy;

            }
            else if (RageEnding)
            {
                return 1 +(currentEnemyRageCount * AccelerationBoostPerEnemy) * Mathf.Lerp(1, 0, RageEndingTimer / RageCooldown);
            }
            else
            {
                return 1;
            }

        }
    }
   


    private void OnEnable()
    {

        points = new Vector3[3];

      


        ControllerInput.Instance.Horizontal.AddListener(OnHorizontal);
        ControllerInput.Instance.Vertical.AddListener(OnVertical);
        ControllerInput.Instance.Jump.AddListener(OnJump);
        ControllerInput.Instance.Attack.AddListener(OnAttack);
    }

    private void OnDisable()
    {
        ControllerInput.Instance.Horizontal.RemoveListener(OnHorizontal);
        ControllerInput.Instance.Vertical.RemoveListener(OnVertical);
        ControllerInput.Instance.Jump.RemoveListener(OnJump);
        ControllerInput.Instance.Attack.RemoveListener(OnAttack);
    }


    // Update is called once per frame
    void Update()
    {
        if (!player.IsAlive)
        {
            return;
        }
        LastOnGroundTime -= Time.deltaTime;
        LastDashDurationTime -= Time.deltaTime;
        RageTimer -= Time.deltaTime;
        RageEndingTimer -= Time.deltaTime;


        if (RageActive)
        {
            if (RageTimer < 0)
            {
                ChangeEnemyCount(-1);
            }
        }

        if (RageEnding)
        {
            if (RageEndingTimer < 0)
            {
                RageEnding = false;
                currentEnemyRageCount = 0;
            }
        }

        points[0] = new Vector3(transform.position.x - (groundCheckSize.x / 2f + 0.05f) + groundCheckPoint.x, transform.position.y + groundCheckPoint.y +0.1f, 0);
        points[1] = new Vector3(transform.position.x + groundCheckPoint.x, transform.position.y + groundCheckPoint.y+0.1f, 0);
        points[2] = new Vector3(transform.position.x + (groundCheckSize.x / 2f + 0.05f) + groundCheckPoint.x, transform.position.y + groundCheckPoint.y+0.1f, 0);

        GroundCheck();


        m_PlayerAnimator.SetBool("IsOnGround", OnGround);
        m_PlayerAnimator.SetBool("IsRunning", Mathf.Abs(m_RigidBody.velocity.x) > 1);
       
        m_PlayerAnimator.SetBool("IsFalling", m_RigidBody.velocity.y < -0.2f);



    }

    private void FixedUpdate()
    {
        if (!player.IsAlive)
        {
            return;
        }
        UpdateVerticalMovement();


        //dont allow returning
        //UPDATE HORIZONTAL MOVEMENT
        float targetSpeed;

        //Calculate the direction we want to move in and our desired velocity

        if (Dashing)
        {
            targetSpeed = dashDirection * dashSpeed;
        }
        else
        {

            targetSpeed = inputX * (m_RunSpeed + RageSpeedBoost);
        }
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(m_RigidBody.velocity.x, targetSpeed, 1);
       

        float accelerationRate;

        if (Dashing)
        {
            accelerationRate = dashAcceleration;
        }

        else if (OnGround)
        {

            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration : runDecceleration;

        }
        else
        {
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration * jumpAcceleration : runDecceleration * jumpDecceleration;
        }
        accelerationRate *= RageAccelerationBoost; 

        if (!OnGround  && !Dashing && Mathf.Abs(m_RigidBody.velocity.y) < hangTimeThreshold)
        {
            accelerationRate *= hangTimeAccelerationMult;
            targetSpeed *= hangTimeSpeedMult;
        }


        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - m_RigidBody.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelerationRate;


        //Convert this to a vector and apply to rigidbody
        m_RigidBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
        if (Mathf.Abs(m_RigidBody.velocity.x) > 0.1f)
        {
            m_PlayerSprite.flipX = m_RigidBody.velocity.x < 0;
        }
        if (m_RigidBody.position.x > maxPositionX && player.IsAlive)
        {
            maxPositionX = m_RigidBody.position.x;
        }

        if (LastDashDurationTime < 0)
        {

            m_PlayerAnimator.SetBool("IsAttacking", false);
            Dashing = false;
        }

        m_PlayerAnimator.SetFloat("RunningSpeed", Mathf.Abs(m_RigidBody.velocity.x) / 10f);
        UpdateDash();
       
    }

    private void UpdateVerticalMovement()
    {

        if (Dashing)
        {
            m_RigidBody.gravityScale = 0;
            jumping = false;
            falling = false;
            return;
        }


        if (OnGround && jumping && m_RigidBody.velocity.y <= 0)
        {
            jumping = false;
            falling = false;
        }
        else
        {
            //we are falling according to some weird definition which includes just chilling on the ground too
            if (m_RigidBody.velocity.y <= 0)
            {
               
                // apply correct gravity scale
                if (jumpHeld)
                {
                    m_RigidBody.gravityScale = gravityMultiplier * fallingGravityJumpHeld;

                }
                else
                {
                    m_RigidBody.gravityScale = gravityMultiplier * fallingGravity;
                }
                m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, Mathf.Clamp(m_RigidBody.velocity.y, -maximumFallSpeed, 0));
               
            }
            // apply correct gravity scale for jump
            else if(!jumpHeld)
            {
                m_RigidBody.gravityScale = gravityMultiplier * jumpGravity;
            }
        }

        if (m_RigidBody.velocity.y < 0)
        {
            falling = true;
            jumping = false;
        }


    }
    public void TeleportToLastGround()
    {

        m_RigidBody.velocity = default;

        transform.position = lastGroundedPosition;
    }

    public Vector2 nextGravity;

    private void GroundCheck()
    {

        canDashDirection = 0;
        var dashCanceledLeft = Physics2D.OverlapBox(transform.position + dashCheckPointLeft, dashCheckSize, 0, groundLayer);
        if (dashCanceledLeft)
        {
            if (Dashing && dashDirection < 0)
            {
                EndDash();
            }
            canDashDirection -= 1;
        }

        var dashCanceledRight = Physics2D.OverlapBox(transform.position + dashCheckPointRight, dashCheckSize, 0, groundLayer);
        if (dashCanceledRight)
        {
            if (Dashing && dashDirection > 0)
            {
                EndDash();
            }
            canDashDirection += 1;
        }

        var collider = Physics2D.OverlapBox(transform.position + groundCheckPoint, groundCheckSize, 0, groundLayer);
        if (collider != null && !jumping)
        {
            lastGroundedPosition = m_RigidBody.position;
   
            LastOnGroundTime = coyoteTime;

            falling = false;
            m_RigidBody.gravityScale = gravityMultiplier * fallingGravity;
        }




        if (!OnGround)
        {
           
            Physics2D.gravity = new Vector2(0, -9.81f);
        }
        else
        {

            RaycastHit2D[] hits = new RaycastHit2D[1];


            var normals = new List<Vector2>();
            var positions = new List<Vector2>();


            for (int i = 0; i < 3; i++)
            {
                var hit = Physics2D.RaycastNonAlloc(points[i], Vector2.down, hits, slopeCheck, groundLayer);
                if (hit > 0)
                {
                    normals.Add(hits[0].normal);
                    positions.Add(hits[0].point);
                }
            }


            if (normals.Count == 0)
            {
                Debug.Log($"no hits normal gravity");
                nextGravity = new Vector2(0, -9.81f);
            }

            //if (normals.Count == 3 && positions[0] != positions[2])
            //{
            //    Physics2D.gravity = new Vector2(0, -9.81f);
            //    return;
            //}
            float maxY = -1000;
            float left = 0;
            float right = 0;
            for (int i = 0; i < normals.Count; i++)
            {
              

                if (Mathf.Abs(normals[i].x) > 0.01f)
                {
                    if (normals[i].x < 0)
                    {
                        left++;
                    }
                    else if (normals[i].x > 0)
                    {
                        right++;
                    }
                }
                if (maxY < positions[i].y && i != 1)
                {
                    nextGravity = -9.81f * normals[i];
                    maxY = positions[i].y;
                  
                }
            }


            //if (left > 1)
            //{
            //    Debug.Log($"left");
            //    nextGravity = -9.81f * normals.Find(x => x.x > 0);

            //}
            //else if (right > 1)
            //{
            //    Debug.Log($"right");
            //    nextGravity = -9.81f * normals.Find(x => x.x < 0);
            //}
            Physics2D.gravity = Vector2.Lerp(Physics2D.gravity, nextGravity, 0.5f);

        }
    }


    bool wasDashing;
    private static readonly int VelocityOnYAxis = Animator.StringToHash("VelocityOnYAxis");
    private static readonly int VelocityOnXAxis = Animator.StringToHash("VelocityOnXAxis");

    private void UpdateDash()
    {

        if (wasDashing && !Dashing)
        {
            EndDash();
        }
        wasDashing = Dashing;
    }

    public void Die()
    {
        falling = false;
        jumping = false;
        m_RigidBody.velocity = default;
        m_PlayerAnimator.SetBool("IsOnGround", true);
        m_PlayerAnimator.SetBool("IsRunning",false);
    }


    void EndDash()
    {
        dashDirection = 0;
        LastOnGroundTime = 0;
        wasDashing = false;
        if (!OnGround)
        {
            m_RigidBody.gravityScale = gravityMultiplier * fallingGravity;
        }
    }


    #region input
    void OnJump(bool value)
    {
        if (!player.IsAlive)
        {
            return;
        }
        jumpHeld = value;
        if (value && OnGround && !jumping)
        {
           
            
            jumping = true;

            //no more coyote time
            LastOnGroundTime = 0;

            m_RigidBody.gravityScale = gravityMultiplier * jumpGravityJumpHeld;


            //more force applied if we are falling down (Wil. E. Coyote)
            m_RigidBody.AddForce(Vector2.up * (jumpForce - m_RigidBody.velocity.y), ForceMode2D.Impulse);
        }
    }

    void OnAttack()
    {
        if (!player.IsAlive)
        {
            return;
        }
        if (player.Attack())
        {
            SetDash(dashDuration);
            m_PlayerAnimator.SetBool("IsAttacking", true);
        }
    }
    void SetDash(float duration)
    {
        if (!player.IsAlive)
        {
            return;
        }
        //if player is holding a direction
        if (Mathf.Abs(inputX) > 0)
        {
            dashDirection = inputX;
        }
        //use the current facing direction
        else
        {
            dashDirection = FacingRight ? 1 : -1;
        }
        //check if player is already colliding with a wall
        if (canDashDirection == 0 || dashDirection != canDashDirection)
        {
            Dashing = true;
            LastDashDurationTime = duration;

            m_RigidBody.velocity = new Vector2(m_RigidBody.velocity.x, 0);
            jumping = false;
        }
        else
        {
            dashDirection = 0;

        }
        LastDashDurationTime = duration;
    }



    public void ChangeEnemyCount(int change)
    {
        if (change > 0)
        {
            if (RageEnding)
            {
                currentEnemyRageCount = 0;
            }
            SetDash(dashDurationAfterHitEnemy);
            m_RigidBody.AddForce((FacingRight ? 1 : -1) * new Vector3(forceAfterHitEnemy, 0, 0));
            currentEnemyRageCount += change;
            currentEnemyRageCount = Mathf.Min(currentEnemyRageCount, MaxRageEnemies);
            RageTimer = RageCooldown;
            RageActive = true;
            RageEnding = false;
        }
        else
        {
          
            if (RageActive)
            {
                RageEnding = true;
                RageEndingTimer = RageEndTweenTime;
            }
           
            RageTimer = -1f;
            RageActive = false;
        }
    }
    

    void OnHorizontal(float value)
    {
        inputX = value;
    }

    void OnVertical(float value)
    {
        inputY = value;
    }

    #endregion



    public void ResetReturnPosition()
    {
        maxPositionX = transform.position.x;
        Physics2D.SyncTransforms();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(transform.position + groundCheckPoint, groundCheckSize);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + dashCheckPointLeft, dashCheckSize);
        Gizmos.DrawWireCube(transform.position + dashCheckPointRight, dashCheckSize);

        Gizmos.color = Color.white;



        if (points != null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Gizmos.DrawLine(points[i], points[i] + slopeCheck * new Vector3(Vector2.down.x, Vector2.down.y, 0));
            }
        }
    }
}
