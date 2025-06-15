using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float health = 100;
    private float damage = 10;
    private float speed = 5f;
    private bool isAlive = true;

    public void TakeDamage(float amount)
    {
        if (!isAlive) return;

        health -= amount;
        if (health <= 0)
        {
            Die();
        }
        Debug.Log($"Enemy took {amount} damage, remaining health: {health}");
    }

    private void Die()
    {
        isAlive = false;
        // Add death logic here, such as playing an animation or dropping loot
        Destroy(gameObject);
    }
}
