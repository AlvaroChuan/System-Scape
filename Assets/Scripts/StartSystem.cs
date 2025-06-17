using UnityEngine;

public class StartSystem : MonoBehaviour
{
    [SerializeField] private Transform sun;
    [SerializeField] private Transform mercum;
    [SerializeField] private Transform colis;
    [SerializeField] private Transform phobos;
    [SerializeField] private Transform regio;
    [SerializeField] private Transform platum;

    private float mercumInitialRotation;
    private float colisInitialRotation;
    private float phobosInitialRotation;
    private float regioInitialRotation;
    private float platumInitialRotation;

    private void Start()
    {
        mercumInitialRotation = mercum.rotation.eulerAngles.y;
        colisInitialRotation = colis.rotation.eulerAngles.y;
        phobosInitialRotation = phobos.rotation.eulerAngles.y;
        regioInitialRotation = regio.rotation.eulerAngles.y;
        platumInitialRotation = platum.rotation.eulerAngles.y;
        sun.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation, 0);
        sun.localScale = new Vector3(GameManager.instance.solarSystemStarSize, GameManager.instance.solarSystemStarSize, GameManager.instance.solarSystemStarSize);
        mercum.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation * 1.4f + mercumInitialRotation, 0);
        colis.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation * 1.2f + colisInitialRotation, 0);
        phobos.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation * 1f + phobosInitialRotation, 0);
        regio.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation * 0.8f + regioInitialRotation, 0);
        platum.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation * 0.6f + platumInitialRotation, 0);
        SpaceshipController.instance.SetPosition();
    }

    private void Update()
    {
        sun.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation, 0);
        sun.localScale = new Vector3(GameManager.instance.solarSystemStarSize, GameManager.instance.solarSystemStarSize, GameManager.instance.solarSystemStarSize);
        mercum.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation * 1.4f + mercumInitialRotation, 0);
        colis.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation * 1.2f + colisInitialRotation, 0);
        phobos.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation * 1f + phobosInitialRotation, 0);
        regio.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation * 0.8f + regioInitialRotation, 0);
        platum.rotation = Quaternion.Euler(0, GameManager.instance.solarSystemRotation * 0.6f + platumInitialRotation, 0);
    }
}
