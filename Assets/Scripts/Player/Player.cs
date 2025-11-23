using UnityEngine;

public class Player : MonoBehaviour
{
    public GatherInput gatherInput;
    public StateMachine stateMachine;
    public Animator anim;
    public PhysicsControl physicsControl;
    public PlayerStats playerStats;
    public bool facingRight = true;

    private BaseAbility[] playerAbilities;

    private void Awake()
    {
        stateMachine = new StateMachine();
        playerAbilities = GetComponents<BaseAbility>();
        stateMachine.arrayOfAbilities = playerAbilities;
    }

    private void Update()
    {
        foreach(BaseAbility ability in playerAbilities)
        {
            if(ability.thisAbilityState == stateMachine.currentState)
            {
                ability.ProcessAbility();
            }
            ability.UpdateAnimator();
        }
    }

    private void FixedUpdate()
    {
        foreach(BaseAbility ability in playerAbilities)
        {
            if(ability.thisAbilityState == stateMachine.currentState)
            {
                ability.ProcessFixedAbility();
            }
        }
    }

    public void Flip()
    {
        if(facingRight == true && gatherInput.horizontalInput < 0)
        {
            transform.Rotate(0, 180, 0);
            facingRight = !facingRight;
        }
        else if(facingRight == false && gatherInput.horizontalInput > 0)
        {
            transform.Rotate(0,180,0);
            facingRight = !facingRight;
        }
    }

    public void ForceFlip()
    {
        transform.Rotate(0,180,0);
        facingRight = !facingRight;
    }

}
