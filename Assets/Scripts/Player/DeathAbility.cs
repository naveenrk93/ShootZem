using UnityEngine;

public class DeathAbility : BaseAbility
{

    public override void EnterAbility()
    {
        player.gatherInput.DisablePlayerMap();
        player.playerStats.DisableDamage();
        linkedPhysics.ResetVelocity();
        if(linkedPhysics.grounded)
            linkedAnimator.SetBool("Death", true);
        else
        {
            linkedAnimator.SetBool("Death", true);
        }
    }

    public void ResetGame()
    {
        Debug.Log("Reset game");
    }
}
