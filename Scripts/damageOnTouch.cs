using UnityEngine;

public class damageOnTouch : MonoBehaviour
{
    public int damageAmount = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Health health = other.GetComponent<Health>();

        if (health != null)
        {
            health.TakeDamage(damageAmount);
            Debug.Log("Damaged " + other.name);
        }
    }
}
