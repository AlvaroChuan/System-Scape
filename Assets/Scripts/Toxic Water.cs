using UnityEngine;
using System.Collections;

public class ToxicWater : MonoBehaviour
{
    private Coroutine damageCoroutine;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (damageCoroutine != null) StopCoroutine(damageCoroutine);
            damageCoroutine = StartCoroutine(ApplyDamage(other.gameObject));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (damageCoroutine != null) StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator ApplyDamage(GameObject player)
    {
        while (true)
        {
            GameManager.instance.DamagePlayer(10);
            yield return new WaitForSeconds(1f);
        }
    }
}
