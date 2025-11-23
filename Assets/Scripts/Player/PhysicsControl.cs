using UnityEngine;

public class PhysicsControl : MonoBehaviour
{
    
    public Rigidbody2D rb;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteSetTime;
    public float coyoteTimer;

    [Header("Ground")]
    [SerializeField] private float groundRayDistance;
    [SerializeField] private Transform leftGroundPoint;
    [SerializeField] private Transform rightGroundPoint;
    [SerializeField] private LayerMask whatToDetect;

    public bool grounded;
    private RaycastHit2D hitInfoLeft;
    private RaycastHit2D hitInfoRight;

    private Player player;


    [Header("Wall")]
    [SerializeField] private float wallRayDistance;
    [SerializeField] private Transform wallCheckPointUpper;
    [SerializeField] private Transform wallCheckPointLower;
    public bool wallDetected;
    private RaycastHit2D hitInfoWallUpper;
    private RaycastHit2D hitInfoWallLower;


    [Header("Ceiling")]
    [SerializeField] private float ceilingRayDistance;
    [SerializeField] private Transform ceilingCheckPointRight;
    [SerializeField] private Transform ceilingCheckPointLeft;
    public bool ceilingDetected;
    private RaycastHit2D hitInfoCeilingRight;
    private RaycastHit2D hitInfoCeilingLeft;
    
    

    [Header("Colliders")]
    [SerializeField] private BoxCollider2D standCollider;
    [SerializeField] private BoxCollider2D crouchCollider;


    private float gravityValue;

    public float GetGravity()
    {
        return gravityValue;
    }
    
    void Start()
    {
        gravityValue = rb.gravityScale;
        player = GetComponent<Player>();
        coyoteTimer = coyoteSetTime;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(ceilingCheckPointLeft.position, new Vector3(0, ceilingRayDistance, 0));
        Debug.DrawRay(ceilingCheckPointRight.position, new Vector3(0, ceilingRayDistance, 0));
    }

    private bool CheckGround()
    {
        hitInfoLeft = Physics2D.Raycast(leftGroundPoint.position, Vector2.down, groundRayDistance, whatToDetect);
        hitInfoRight = Physics2D.Raycast(rightGroundPoint.position, Vector2.down, groundRayDistance, whatToDetect);

        Debug.DrawRay(leftGroundPoint.position, new Vector3(0,-groundRayDistance, 0), Color.red);
        Debug.DrawRay(rightGroundPoint.position, new Vector3(0,-groundRayDistance, 0), Color.red);

        if(hitInfoLeft || hitInfoRight)
            return true;

        return false;
        
    }

    private bool CheckWall()
    {
        Vector2 facingDirection;

        if(player.facingRight)
        {
            facingDirection = Vector2.right;
        }
        else
        {
            facingDirection = Vector2.left;
        }
        hitInfoWallUpper = Physics2D.Raycast(wallCheckPointUpper.position, facingDirection, wallRayDistance, whatToDetect);
        hitInfoWallLower = Physics2D.Raycast(wallCheckPointLower.position, facingDirection, wallRayDistance, whatToDetect);

        Debug.DrawRay(wallCheckPointUpper.position, new Vector3(player.facingRight ? wallRayDistance : -wallRayDistance, 0, 0));
        Debug.DrawRay(wallCheckPointLower.position, new Vector3(player.facingRight ? wallRayDistance : -wallRayDistance, 0, 0));

        if(hitInfoWallLower || hitInfoWallUpper)
        {
            return true;
        }
        return false;
    }

    private bool CheckCeiling()
    {
        hitInfoCeilingLeft = Physics2D.Raycast(ceilingCheckPointLeft.position, Vector2.up, ceilingRayDistance, whatToDetect);
        hitInfoCeilingRight = Physics2D.Raycast(ceilingCheckPointRight.position, Vector2.up, ceilingRayDistance, whatToDetect);

        if(hitInfoCeilingLeft || hitInfoCeilingRight)
        {
            return true;
        }
        return false;
    }

    public void DisableGravity()
    {
        rb.gravityScale = 0;
    }

    public void EnableGravity()
    {
        rb.gravityScale = gravityValue;
    }

    public void ResetVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    public void StandColliders()
    {
        standCollider.enabled = true;
        crouchCollider.enabled = false;
    }

    public void CrouchColliders()
    {
        standCollider.enabled = false;
        crouchCollider.enabled = true;

    }

    private void Update()
    {
        if(!grounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = coyoteSetTime;
        }
    }

    private void FixedUpdate()
    {
        grounded = CheckGround();
        wallDetected = CheckWall();
        ceilingDetected = CheckCeiling();
    }
}
