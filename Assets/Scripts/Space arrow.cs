using UnityEngine;

public class Spacearrow : MonoBehaviour
{
    public GameObject target;

    private void Update()
    {
        if (target != null) transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
    }
}
