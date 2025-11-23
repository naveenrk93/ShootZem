using System.Collections;
using UnityEngine;

public class KnockbackAbility : BaseAbility
{

    private Coroutine currentKnockBack;

    public override void ExitAbility()
    {
        currentKnockBack = null;
    }

    public void StartKnockback(float duration, Vector2 force, Transform enemyObject)
    {
        if(currentKnockBack == null)
        {
            currentKnockBack = StartCoroutine(Knockback(duration, force, enemyObject));
        }
    }

    public IEnumerator Knockback(float duration, Vector2 force, Transform enemyObject)
    {
        linkedStateMachine.ChangeState(PlayerStates.State.Knockback);
        linkedPhysics.ResetVelocity();
        if(transform.position.x >= enemyObject.transform.position.x)
        {
            linkedPhysics.rb.linearVelocity = force;
        }
        else
        {
            linkedPhysics.rb.linearVelocity = new Vector2(-force.x, force.y);
        }

        yield return new WaitForSeconds(duration);

        if(player.playerStats.GetCurrentHealth() <= 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Death);
        }
        else
        {
            if(linkedPhysics.grounded)
            {
                if(linkedInput.horizontalInput !=0)
                    linkedStateMachine.ChangeState(PlayerStates.State.Run);
                else
                    linkedStateMachine.ChangeState(PlayerStates.State.Idle);        
                
            }
            else
            {
                linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            }
        }
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool("Knockback", linkedStateMachine.currentState == PlayerStates.State.Knockback);
    }
}
