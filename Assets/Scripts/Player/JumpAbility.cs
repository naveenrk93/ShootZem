using UnityEngine;
using UnityEngine.InputSystem;

public class JumpAbility : BaseAbility
{
    public InputActionReference jumpActionRef;
    [SerializeField] private float jumpForce;
    [SerializeField] private float airSpeed;
    [SerializeField] private float minimumAirTime;
    private float startMinimumAirTime;

    private string jumpAnimParameterName = "Jump";
    private string ySpeedAnimParameterName = "ySpeed";
    
    private int jumpParameterId;
    private int ySpeedParameterId;

    protected override void Initialization()
    {
        base.Initialization();
        startMinimumAirTime = minimumAirTime;
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
            linkedPhysics.rb.linearVelocity = new Vector2(airSpeed * linkedInput.horizontalInput, linkedPhysics.rb.linearVelocityY);

        }
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
            return;
        }

        if(linkedPhysics.coyoteTimer > 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.rb.linearVelocity = new Vector2(airSpeed * linkedInput.horizontalInput, jumpForce);
            minimumAirTime = startMinimumAirTime;
            linkedPhysics.coyoteTimer = -1;
        }
    }

    private void StopJump(InputAction.CallbackContext value)
    {
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(jumpParameterId, linkedStateMachine.currentState == PlayerStates.State.Jump || linkedStateMachine.currentState == PlayerStates.State.WallJump);
        linkedAnimator.SetFloat(ySpeedParameterId, linkedPhysics.rb.linearVelocityY);
    }
}
