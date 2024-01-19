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



    private void OnEnable()
    {
        ControllerInput.Instance.Horizontal.AddListener(OnHorizontal);
        ControllerInput.Instance.Vertical.AddListener(OnVertical);
        ControllerInput.Instance.Jump.AddListener(OnJump);
    }

    private void OnDisable()
    {
        ControllerInput.Instance.Horizontal.RemoveListener(OnHorizontal);
        ControllerInput.Instance.Vertical.RemoveListener(OnVertical);
        ControllerInput.Instance.Jump.RemoveListener(OnJump);
    }


    // Update is called once per frame
    void Update()
    {
       
        LastOnGroundTime -= Time.deltaTime;
      

        GroundCheck();

        
    }

    private void FixedUpdate()
    {
        UpdateVerticalMovement();


        //dont allow returning
        //UPDATE HORIZONTAL MOVEMENT
        float targetSpeed;

        //Calculate the direction we want to move in and our desired velocity
      
        targetSpeed = inputX * m_RunSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(m_RigidBody.velocity.x, targetSpeed, 1);

        float accelerationRate;
       

        
        if (OnGround)
        {

            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration : runDecceleration;

        }
        else
        {
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAcceleration * jumpAcceleration : runDecceleration * jumpDecceleration;
        }

        if (!OnGround && Mathf.Abs(m_RigidBody.velocity.y) < hangTimeThreshold)
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
        if (m_RigidBody.position.x > maxPositionX)
        {
            maxPositionX = m_RigidBody.position.x;
        }
    }

    private void UpdateVerticalMovement()
    {
        if (OnGround && jumping && m_RigidBody.velocity.y <= 0)
        {
            jumping = false;
        }
        else
        {
            //we are falling according to some weird definition which includes just chilling on the ground too
            if (m_RigidBody.velocity.y <= 0)
            {
                falling = true;
                jumping = false;
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


    }
    public void TeleportToLastGround()
    {

        m_RigidBody.velocity = default;

        transform.position = lastGroundedPosition;
    }

    private void GroundCheck()
    {
        var collider = Physics2D.OverlapBox(transform.position + groundCheckPoint, groundCheckSize, 0, groundLayer);
        if (collider != null && !jumping)
        {
            lastGroundedPosition = m_RigidBody.position;
   
            LastOnGroundTime = coyoteTime;

            falling = false;
            m_RigidBody.gravityScale = gravityMultiplier * fallingGravity;
        }
    }


    #region input
    void OnJump(bool value)
    {
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

    

    void OnHorizontal(float value)
    {
        inputX = value;
    }

    void OnVertical(float value)
    {
        inputY = value;
    }

    #endregion



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(transform.position + groundCheckPoint, groundCheckSize);
    }
}
