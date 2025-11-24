using UnityEngine;
using UnityEngine.InputSystem;

public class DashAbility : BaseAbility
{
    public InputActionReference dashActionRef;
    [SerializeField] private float dashForce;
    [SerializeField] private float maxDashDuration;
    private float dashTimer;

    private string dashAnimParameterName = "Dash";
    private int dashAnimParameterID;

    protected override void Initialization()
    {
        base.Initialization();
        dashAnimParameterID = Animator.StringToHash(dashAnimParameterName);

    }

    public override void EnterAbility()
    {
        player.playerStats.DisableDamage();
    }

    private void OnEnable()
    {
        dashActionRef.action.performed += TryToDash;
    }
    
    private void OnDisable()
    {
        dashActionRef.action.performed -= TryToDash;
    }

    private void TryToDash(InputAction.CallbackContext value)
    {
        if(!isPermitted || linkedStateMachine.currentState == PlayerStates.State.Knockback)
            return;

        if(linkedStateMachine.currentState == PlayerStates.State.Dash || linkedPhysics.wallDetected || linkedStateMachine.currentState == PlayerStates.State.Crouch)
        {
            return;
        }

        linkedPhysics.DisableGravity();
        linkedPhysics.ResetVelocity();

        linkedStateMachine.ChangeState(PlayerStates.State.Dash);
        if(player.facingRight)
        linkedPhysics.rb.linearVelocityX = dashForce;
        else linkedPhysics.rb.linearVelocityX = -dashForce;

        dashTimer = maxDashDuration;
    }

    public override void ExitAbility()
    {
        linkedPhysics.EnableGravity();
        //optional
        linkedPhysics.ResetVelocity();
        player.playerStats.EnableDamage();
    }


    public override void ProcessAbility()
    {
        dashTimer -= Time.deltaTime;
        if(linkedPhysics.wallDetected)
            dashTimer = -1;
        if(dashTimer <= 0)
        {
            if(linkedPhysics.grounded)
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            else
                linkedStateMachine.ChangeState(PlayerStates.State.Jump);
        }
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(dashAnimParameterID, linkedStateMachine.currentState == PlayerStates.State.Dash);
    }
}
