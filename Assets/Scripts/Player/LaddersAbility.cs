using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class LaddersAbility : BaseAbility
{
    public InputActionReference ladderActionRef;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float setMinLadderTime;
    [SerializeField] private float horizontalSpeedOnLadder;
    private float minimumLadderTime;
    private bool climb;
    public bool canGoOnLadder;

    private string ladderAnimParameterName = "Ladder";
    private int ladderAnimParameterID;

    private void OnEnable()
    {
        ladderActionRef.action.performed += TryToClimb;
        ladderActionRef.action.canceled += StopClimb;
    }

    private void OnDisable()
    {
        ladderActionRef.action.performed -= TryToClimb;
        ladderActionRef.action.canceled -= StopClimb;
    }


    protected override void Initialization()
    {
        base.Initialization();        
        ladderAnimParameterID = Animator.StringToHash(ladderAnimParameterName);
        minimumLadderTime = setMinLadderTime;
    }

    private void TryToClimb(InputAction.CallbackContext value)
    {
        if(!isPermitted || linkedStateMachine.currentState == PlayerStates.State.Knockback)
            return;
            
        linkedAnimator.enabled = true;
        if(linkedStateMachine.currentState == PlayerStates.State.Ladders || linkedStateMachine.currentState == PlayerStates.State.Dash || !canGoOnLadder)
        {
            return;
        }

        linkedStateMachine.ChangeState(PlayerStates.State.Ladders);
        linkedPhysics.DisableGravity();
        linkedPhysics.ResetVelocity();
        climb = true;
        minimumLadderTime = setMinLadderTime;

    }

    private void StopClimb(InputAction.CallbackContext value)
    {
        if(!isPermitted)
            return;
        if(linkedStateMachine.currentState != PlayerStates.State.Ladders)
            return;
        linkedPhysics.ResetVelocity();
        linkedAnimator.enabled = false;
    }

    public override void ExitAbility()
    {
        linkedPhysics.EnableGravity();
        climb=false;
        linkedAnimator.enabled = true;
    }

    public override void ProcessFixedAbility()
    {
        if(climb)
            linkedPhysics.rb.linearVelocity = new Vector2(0, linkedInput.verticalInput * climbSpeed);
        
        if(linkedInput.horizontalInput !=0)
        {
            linkedPhysics.rb.linearVelocity = new Vector2(horizontalSpeedOnLadder * linkedInput.horizontalInput, linkedPhysics.rb.linearVelocityY);
        }
    }

    public override void ProcessAbility()
    {
        if(climb)
            minimumLadderTime -= Time.deltaTime;
        
        if(canGoOnLadder == false)
        {
            if(linkedPhysics.grounded == false)
            {
                linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            }
        }

        if(linkedPhysics.grounded && minimumLadderTime <=0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
        }
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(ladderAnimParameterID, linkedStateMachine.currentState == PlayerStates.State.Ladders);
    }
}
