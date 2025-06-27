using UnityEngine;

public class MainMenuAnimControl : MonoBehaviour
{
    public void PlayEngineSound()
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.SpaceShipEngine, true);
    }

    public void StopEngineSound()
    {
        SoundManager.instance.StopSfx(SoundManager.ClipEnum.SpaceShipEngine);
    }

    public void CallHUDController()
    {
        HUDManager.instance.FadeOut("Mercum");
    }
}
