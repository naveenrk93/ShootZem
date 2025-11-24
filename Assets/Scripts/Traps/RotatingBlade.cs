using UnityEngine;

public class RotatingBlade : MonoBehaviour
{

    [SerializeField] private float bladeDamage;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private Vector2 knockbackForce;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        KnockbackAbility knockbackAbility = collision.GetComponentInParent<KnockbackAbility>();
        knockbackAbility.StartKnockback(knockbackDuration, knockbackForce, transform);
        PlayerStats playerStats = collision.GetComponent<PlayerStats>();
        playerStats.DamagePlayer(bladeDamage);
    }
}
