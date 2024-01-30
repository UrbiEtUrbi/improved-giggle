using UnityEngine;

public class ArrowAttack : MonoBehaviour {
    
    //::::::::::::::::::::::::::::://
    // Serialized Fields
    //::::::::::::::::::::::::::::://

    [Tooltip("Force used to fire projectile")]
    [SerializeField] private float projectileForce = 8f;
    [Tooltip("Angle projectile will be fire at. 0 == Straight up, 90 == Horizontal")]
    [SerializeField] private float projectileAngle = 45f;
    
    //::::::::::::::::::::::::::::://
    // Components
    //::::::::::::::::::::::::::::://

    private Rigidbody2D arrowRigidBody;
    
    //::::::::::::::::::::::::::::://
    // Unity Callbacks
    //::::::::::::::::::::::::::::://

    private void Awake() {
        arrowRigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        // get player position in relation to array (left or right)
        float xDifference = ControllerGame.Player.transform.position.x - transform.position.x;
        float attackPosition  = xDifference < 0f ? -1f : 1f;
        
        // calculate force as Vector2 using projectile angle and player position
        Vector2 force = new(Mathf.Sin(projectileAngle * Mathf.Deg2Rad) * attackPosition * projectileForce, Mathf.Cos(projectileAngle * Mathf.Deg2Rad) * projectileForce);
        
        // shoot arrow towards player with impulse force
        arrowRigidBody.AddForce(force, ForceMode2D.Impulse);
    }

    private void FixedUpdate() {
        // rotate arrow in the direction it is moving
        arrowRigidBody.MoveRotation(Quaternion.LookRotation(arrowRigidBody.velocity));
    }
}
