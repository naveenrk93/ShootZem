using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float maxHealth;
    [SerializeField] private HealthBarControl healthBarControl;
    private float currentHealth;

    [Header("Flash")]
    [SerializeField] private float flashDuration;
    [SerializeField, Range(0,1)] private float flashStrength;
    [SerializeField] private Color flashColor;
    [SerializeField] private Material flashMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriter;
    private bool canTakeDamage = true;

    [Header("StatsColliders")]
    [SerializeField] private Collider2D standingStatsCollider;
    [SerializeField] private Collider2D crouchingStatsCollider;
    private Collider2D currentStatsCollider;


    void Start()
    {
        currentHealth = maxHealth;
        healthBarControl.SetSliderValue(currentHealth, maxHealth);
        spriter = GetComponentInParent<SpriteRenderer>();
        defaultMaterial = spriter.material;
    }

    public void DamagePlayer(float damage)
    {
        if(!canTakeDamage) return;
        currentHealth -= damage;
        healthBarControl.SetSliderValue(currentHealth, maxHealth);
        StartCoroutine(Flash());
        if(IsDead())
        {
            if(player.stateMachine.currentState != PlayerStates.State.Knockback)
                player.stateMachine.ChangeState(PlayerStates.State.Death);
        }
    }

    public bool IsDead()
    {
        if(currentHealth <= 0)
            return true;
        return false;
    }

    private IEnumerator Flash()
    {
        canTakeDamage = false;
        spriter.material = flashMaterial;
        flashMaterial.SetColor("_FlashColor", flashColor);
        flashMaterial.SetFloat("_FlashAmount", flashStrength);
        yield return new WaitForSeconds(flashDuration);
        spriter.material = defaultMaterial;
        if(currentHealth > 0)
            canTakeDamage = true;

    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool GetCanTakeDamage()
    {
        return canTakeDamage;
    }

    public void EnableStandCollider()
    {
        if(currentHealth <= 0)
            return;
        crouchingStatsCollider.enabled = false;
        standingStatsCollider.enabled = true;
        currentStatsCollider = standingStatsCollider;
    }

    public void DisableDamage()
    {
        canTakeDamage = false;
    }

    public void EnableDamage()
    {
        canTakeDamage = true;
    }

    public void EnableCrouchCollider()
    {
        if(currentHealth <= 0)
            return;
        standingStatsCollider.enabled = false;
        crouchingStatsCollider.enabled = true;
        currentStatsCollider = crouchingStatsCollider;
    }


}
