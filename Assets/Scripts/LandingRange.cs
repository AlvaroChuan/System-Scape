using UnityEngine;

public class LandingRange : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Planet")) SpaceshipController.instance.nearPlanet = other.name;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Planet") && SpaceshipController.instance.nearPlanet == other.name) SpaceshipController.instance.nearPlanet = "";
    }
}
