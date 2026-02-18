using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;
    public Slider healthSlider;

    void Start()
{
    currentHealth = maxHealth;

    if (healthSlider != null)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}


    public void TakeDamage(int amount)
{
    currentHealth -= amount;
    if (currentHealth <= 0)
    {
        currentHealth = 0;
        Die();
    }

    UpdateHealthBar();
}

public void Heal(int amount)
{
    currentHealth += amount;
    if (currentHealth > maxHealth)
        currentHealth = maxHealth;

    UpdateHealthBar();
}

public void UpdateHealthBar()
{
    if (healthSlider == null) return;

    healthSlider.value = currentHealth;

    healthSlider.gameObject.SetActive(currentHealth < maxHealth);
}


    void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        Destroy(gameObject);
    }
}
