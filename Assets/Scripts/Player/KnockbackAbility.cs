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
        if(player.playerStats.GetCanTakeDamage() == false) return;
        if(currentKnockBack == null)
        {
            currentKnockBack = StartCoroutine(Knockback(duration, force, enemyObject));
        }
    }

    public void StartSwingKnockback(float duration, Vector2 force, int direction)
    {
        if(player.playerStats.GetCanTakeDamage() == false) return;
        if(currentKnockBack == null)
        {
            currentKnockBack = StartCoroutine(SwingKnockback(duration, force, direction));
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

    public IEnumerator SwingKnockback(float duration, Vector2 force, int direction)
    {
        linkedStateMachine.ChangeState(PlayerStates.State.Knockback);
        linkedPhysics.ResetVelocity();

        force.x *= direction;
        linkedPhysics.rb.linearVelocity = force;

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
