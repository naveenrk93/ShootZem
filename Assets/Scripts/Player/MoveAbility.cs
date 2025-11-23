using UnityEngine;

public class MoveAbility : BaseAbility
{
    [SerializeField] private float speed;
    private string runAnimParameterName = "Run";
    private int runParameterId;
    protected override void Initialization()
    {
        base.Initialization();
        runParameterId = Animator.StringToHash(runAnimParameterName);
    }

    public override void EnterAbility()
    {
        player.Flip();
    }

    public override void ProcessAbility()
    {
        if(linkedPhysics.grounded && linkedInput.horizontalInput == 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
        }
        if(!linkedPhysics.grounded)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
        }
    }

    public override void ProcessFixedAbility()
    {
        linkedPhysics.rb.linearVelocity = new Vector2(speed * linkedInput.horizontalInput, linkedPhysics.rb.linearVelocityY);
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(runParameterId, linkedStateMachine.currentState == PlayerStates.State.Run);
    }
}
