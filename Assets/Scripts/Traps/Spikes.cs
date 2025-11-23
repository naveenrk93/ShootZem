using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private float spikeDamage;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private Vector2 knockbackForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        KnockbackAbility knockbackAbility = collision.GetComponentInParent<KnockbackAbility>();
        knockbackAbility.StartKnockback(knockbackDuration, knockbackForce, transform);
        PlayerStats playerStats = collision.GetComponent<PlayerStats>();
        playerStats.DamagePlayer(spikeDamage);
    }
}
