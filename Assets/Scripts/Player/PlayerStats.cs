using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float maxHealth;
    [SerializeField] private HealthBarControl healthBarControl;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        healthBarControl.SetSliderValue(currentHealth, maxHealth);
    }

    public void DamagePlayer(float damage)
    {
        currentHealth -= damage;
        healthBarControl.SetSliderValue(currentHealth, maxHealth);
        if(IsDead())
        {
            if(player.stateMachine.currentState != PlayerStates.State.Knockback)
                Debug.Log("Player is Dead!");
        }
    }

    public bool IsDead()
    {
        if(currentHealth <= 0)
            return true;
        return false;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
