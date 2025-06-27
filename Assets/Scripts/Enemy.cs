using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float health = 40;
    private float damage = 10;
    private bool isAlive = true;
    private Coroutine attackCoroutine;
    private GameObject target;
    [SerializeField] private Indicator healthIndicator;
    [SerializeField] private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer.SetPosition(0, transform.position);
    }

    private void Update()
    {
        if (target != null) lineRenderer.SetPosition(1, target.transform.position + Vector3.up * 0.5f);
        else lineRenderer.SetPosition(1, transform.position);
    }

    public void TakeDamage(float amount)
    {
        if (!isAlive) return;

        healthIndicator.SetFillAmount((health - amount) / 100 * 100);
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 10, 0.5f);
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Hit);

        health -= amount;
        if (health <= 0) transform.DOScale(Vector3.zero, 0.5f).OnComplete(Die);
    }

    private void Die()
    {
        isAlive = false;
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        if (GameManager.instance != null) GameManager.instance.EnemyDeath();
        Destroy(healthIndicator.gameObject);
    }

    public void StartAttackCoroutine(GameObject player)
    {
        target = player;
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackPlayer());
    }

    public void StopAttackCoroutine()
    {
        target = null;
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = null;
    }

    private IEnumerator AttackPlayer()
    {
        while (isAlive && target != null)
        {
            GameManager.instance.DamagePlayer((int)damage);
            transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f, 10, 0.5f);
            yield return new WaitForSeconds(1f);
        }
    }
}
