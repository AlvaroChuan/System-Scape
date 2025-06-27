using UnityEngine;

public class EnemyRange : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) enemy.StartAttackCoroutine(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) enemy.StopAttackCoroutine();
    }
}
