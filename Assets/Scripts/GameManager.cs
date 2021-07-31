using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float UPDATE_PLANETS_TIME = 2.0f;

    private List<GameObject> planets;

    void Start()
    {
        planets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Planet"));
        StartCoroutine(UpdatePlanets());
    }

    void Update()
    {
        
    }

    private IEnumerator UpdatePlanets() {
        while (true) {
            yield return new WaitForSeconds(UPDATE_PLANETS_TIME);
            foreach (GameObject planetGameObject in planets) {
                planetGameObject.GetComponent<Planet>().UpdatePlanet();
            }
        }
    }

}
