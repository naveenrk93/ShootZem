using UnityEngine;
using UnityEngine.InputSystem;

public class MultipleJumpAbility : BaseAbility
{
    public InputActionReference jumpActionRef;

    [SerializeField] private int maxNumberOfJumps;
    private int numberOfJumps;
    private bool canActivateAdditionalJumps;
    [SerializeField] private float jumpForce;
    [SerializeField] private float airSpeed;
    [SerializeField] private float minimumAirTime;
    private float startMinimumAirTime;
    [SerializeField] private float setMaxJumpTime;    
    [SerializeField] private float gravityDivider;

    private float jumpTimer;
    private bool jumping;


    private string jumpAnimParameterName = "Jump";
    private string ySpeedAnimParameterName = "ySpeed";
    
    private int jumpParameterId;
    private int ySpeedParameterId;

    protected override void Initialization()
    {
        base.Initialization();
        startMinimumAirTime = minimumAirTime;
        numberOfJumps = maxNumberOfJumps;
        jumpParameterId = Animator.StringToHash(jumpAnimParameterName);
        ySpeedParameterId = Animator.StringToHash(ySpeedAnimParameterName);
    }

    private void OnEnable()
    {
        jumpActionRef.action.performed += TryToJump;
        jumpActionRef.action.canceled += StopJump;
    }

    private void OnDisable()
    {
        
        jumpActionRef.action.performed -= TryToJump;
        jumpActionRef.action.canceled -= StopJump;
    }

    public override void ProcessAbility()
    {
        player.Flip();
        minimumAirTime-=Time.deltaTime;

        if(jumping)
        {
            jumpTimer -= Time.deltaTime;
            if(jumpTimer <= 0)
            {
                jumping = false;
            }
        }

        if(linkedPhysics.grounded && minimumAirTime < 0)
        {
            if(linkedInput.horizontalInput != 0)
                linkedStateMachine.ChangeState(PlayerStates.State.Run);
            else
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
        }
        if(!linkedPhysics.grounded && linkedPhysics.wallDetected)
        {
            if(linkedPhysics.rb.linearVelocityY < 0)
            {
                linkedStateMachine.ChangeState(PlayerStates.State.WallSlide);
            }
        }
    }

    public override void ProcessFixedAbility()
    {
        if (!linkedPhysics.grounded)
        {
            if(jumping)
                linkedPhysics.rb.linearVelocity = new Vector2(airSpeed * linkedInput.horizontalInput, jumpForce);
            else
                linkedPhysics.rb.linearVelocity = new Vector2(airSpeed * linkedInput.horizontalInput, linkedPhysics.rb.linearVelocityY);
        }

        if(linkedPhysics.rb.linearVelocityY < 0)
        {
            linkedPhysics.rb.gravityScale = linkedPhysics.GetGravity() / gravityDivider;
        }
    }

    public override void ExitAbility()
    {
        linkedPhysics.EnableGravity();
        canActivateAdditionalJumps = false;
    }

    private void TryToJump(InputAction.CallbackContext value)
    {
        if(!isPermitted || linkedStateMachine.currentState == PlayerStates.State.Knockback)
        {
            return;
        }

        if(linkedStateMachine.currentState == PlayerStates.State.Ladders)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.rb.linearVelocity = new Vector2(airSpeed * linkedInput.horizontalInput, 0);
            minimumAirTime = startMinimumAirTime;
            jumping = true;
            jumpTimer = setMaxJumpTime;
            numberOfJumps = maxNumberOfJumps;
            canActivateAdditionalJumps = true;
            numberOfJumps -= 1;
            return;
        }

        if(linkedPhysics.coyoteTimer > 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.rb.linearVelocity = new Vector2(airSpeed * linkedInput.horizontalInput, jumpForce);
            minimumAirTime = startMinimumAirTime;
            linkedPhysics.coyoteTimer = -1;
            jumping = true;
            numberOfJumps = maxNumberOfJumps;
            jumpTimer = setMaxJumpTime;
            canActivateAdditionalJumps = true;
            numberOfJumps -= 1;
            return;
        }

        if(numberOfJumps > 0)
        {
            linkedPhysics.EnableGravity();
            linkedPhysics.rb.linearVelocity = new Vector2(airSpeed * linkedInput.horizontalInput, jumpForce);
            minimumAirTime = startMinimumAirTime;
            linkedPhysics.coyoteTimer = -1;
            jumping = true;
            jumpTimer = setMaxJumpTime;
            numberOfJumps -= 1;
        }
        else
        {
            canActivateAdditionalJumps = false;
        }
    }

    private void StopJump(InputAction.CallbackContext value)
    {
        jumping = false;
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(jumpParameterId, linkedStateMachine.currentState == PlayerStates.State.Jump || linkedStateMachine.currentState == PlayerStates.State.WallJump);
        linkedAnimator.SetFloat(ySpeedParameterId, linkedPhysics.rb.linearVelocityY);
    }

    public void SetMaxJumpNumber(int maxJumps)
    {
        maxNumberOfJumps = maxJumps;
    }
}
