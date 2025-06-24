using UnityEngine;

public class Transition : MonoBehaviour
{
    public void OnFadeOutFinished()
    {
        HUDManager.instance.OnFadeOutFinished();
    }
}
