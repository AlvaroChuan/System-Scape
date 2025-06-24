using UnityEngine;

public class MainMenuAnimControl : MonoBehaviour
{
    public void CallHUDController()
    {
        HUDManager.instance.FadeOut("Mercum");
    }
}
